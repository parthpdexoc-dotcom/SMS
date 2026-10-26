using System.Text.Json;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;
using System.Collections.Generic;

namespace SMS.Generic.Services.Services;

public class TenureEmployeeStatisticsService : ITenureEmployeeStatisticsService
{
    private readonly IPostPositionService _postPositionService;
    private readonly IEmployeeService _employeeService;
    private readonly IEmployeeTenureService _employeeTenureService;

    public TenureEmployeeStatisticsService(
        IPostPositionService postPositionService,
        IEmployeeService employeeService,
        IEmployeeTenureService employeeTenureService)
    {
        _postPositionService = postPositionService ?? throw new ArgumentNullException(nameof(postPositionService));
        _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
        _employeeTenureService = employeeTenureService ?? throw new ArgumentNullException(nameof(employeeTenureService));
    }

    public async Task<List<TenureEmployeeStatisticsDto>> GetTenureEmployeeStatisticsAsync(DepartmentPolicyDto departmentPolicy, List<Guid> employeeIds, DateOnly statisticsWindowEndDate, Guid? departmentId = null, CancellationToken cancellationToken = default)
    {
        if (departmentPolicy == null)
            throw new ArgumentNullException(nameof(departmentPolicy));

        var distinctEmployeeIds = employeeIds?
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList() ?? new List<Guid>();

        if (distinctEmployeeIds.Count == 0)
            return new List<TenureEmployeeStatisticsDto>();

        var rules = GetNormalizedTenureRules(departmentPolicy.TenureRulesJson);
        var tenureType = departmentPolicy.GetEffectiveTenureMetricType();

        var maxLookbackDays = rules.Max(r => r.Days);
        var fetchRangeStart = statisticsWindowEndDate.AddDays(-maxLookbackDays);

        var postPositions = await _postPositionService.GetEmployeesPostPositionsByEmployeeIdAndDateRangeAsync(
                distinctEmployeeIds,
                fetchRangeStart,
                DateOnly.MaxValue,
                departmentId)
            .ConfigureAwait(false);

        // ✅ Group once
        var postsByEmployee = postPositions
            .Where(pp => pp.EmployeeID != Guid.Empty &&
                         pp.DateOfWork.HasValue &&
                         pp.PostPositionStatus != PostPositionStatus.Cancelled &&
                         !pp.IsExemptPosition)
            .GroupBy(pp => pp.EmployeeID)
            .ToDictionary(g => g.Key, g => g.ToList());

        // ✅ Precompute hire dates
        var employeeTenures = (await _employeeTenureService.GetAllAsync(t => distinctEmployeeIds.Contains(t.EmployeeID))).ToList();

        var hireDateMap = employeeTenures.GroupBy(t => t.EmployeeID)
            .ToDictionary(g => g.Key,
                g => g.Where(t => t.IsCurrent && t.DateOfHire.HasValue)
                      .Select(t => t.DateOfHire)
                      .FirstOrDefault()
                  ?? g.Where(t => t.DateOfHire.HasValue)
                      .Select(t => t.DateOfHire)
                      .OrderBy(d => d)
                      .FirstOrDefault()
            );

        // ✅ FINAL RESULT BUILD
        var results = distinctEmployeeIds.Select(empId =>
        {
            postsByEmployee.TryGetValue(empId, out var empPosts);
            empPosts ??= new List<PostPositionDto>();

            var levelValues = new Dictionary<int, decimal>();

            foreach (var rule in rules)
            {
                var windowStart = statisticsWindowEndDate.AddDays(-rule.Days);

                var value = empPosts
                    .Where(p =>
                        p.DateOfWork >= windowStart && p.DateOfWork <= statisticsWindowEndDate
                        || p.DateOfWork > statisticsWindowEndDate)
                    .Sum(p => tenureType == TenureMetricType.Hours
                        ? CountedTenureHours(p)
                        : 1);

                levelValues[rule.Level] = value;
            }

            var firstLevel = rules.First();

            return new TenureEmployeeStatisticsDto
            {
                EmployeeID = empId,
                TenureType = tenureType,
                WindowStart = statisticsWindowEndDate.AddDays(-firstLevel.Days),
                WindowEnd = statisticsWindowEndDate,
                EmployeeHireDateUtc = hireDateMap.TryGetValue(empId, out var hd) ? hd : null,
                LevelValues = levelValues
            };
        }).ToList();

        // ✅ CLEAN MULTI-LEVEL SORT
        IOrderedEnumerable<TenureEmployeeStatisticsDto>? ordered = null;

        foreach (var rule in rules)
        {
            ordered = ordered == null
                ? results.OrderBy(r => r.LevelValues[rule.Level])
                : ordered.ThenBy(r => r.LevelValues[rule.Level]);
        }

        // ✅ FINAL TIE BREAKERS
        return (ordered ?? results.OrderBy(r => r.EmployeeID))
            .ThenBy(r => r.EmployeeHireDateUtc ?? DateOnly.MaxValue)
            .ThenBy(r => r.EmployeeID)
            .ToList();
    }

    /// <summary>
    /// Closed hours when a close record exists for the position; otherwise the post position hours.
    /// </summary>
    private static decimal CountedTenureHours(PostPositionDto post)
    {
        var minutes = post.HoursWorked ?? post.LengthOfWork;
        return minutes / 60m;
    }

    /// <inheritdoc />
    public List<TenureLookbackColumnDto> BuildLookbackColumns(string? tenureRulesJson)
    {
        var rules = GetNormalizedTenureRules(tenureRulesJson);

        return rules
            .Select(r => new TenureLookbackColumnDto
            {
                Level = r.Level,
                LookbackDays = r.Days,
                Label = r.Days > 0 ? $"L{r.Days}" : "L0",
            })
            .ToList();
    }

    private sealed class TenureRulesRoot
    {
        public List<TenureRuleItem>? Rules { get; set; }
    }

    private sealed class TenureRuleItem
    {
        public int Level { get; set; }
        public int Days { get; set; }
        public int Value { get; set; }
    }

    /// <summary>
    /// Same rule list used by <see cref="GetTenureEmployeeStatisticsAsync"/> and <see cref="BuildLookbackColumns"/>:
    /// parsed JSON rules ordered by level, or a single default level when none apply.
    /// </summary>
    private static List<TenureRuleItem> GetNormalizedTenureRules(string? tenureRulesJson)
    {
        var rules = ParseTenureRules(tenureRulesJson)
            .OrderBy(r => r.Level)
            .ToList();

        if (rules.Count == 0)
            rules.Add(new TenureRuleItem { Level = 1, Days = 0 });

        return rules;
    }

    private static List<TenureRuleItem> ParseTenureRules(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<TenureRuleItem>();

        try
        {
            var root = JsonSerializer.Deserialize<TenureRulesRoot>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return root?.Rules?
                .Where(r => r != null && r.Days > 0)
                .OrderBy(r => r.Level)
                .ThenBy(r => r.Days)
                .ToList() ?? new List<TenureRuleItem>();
        }
        catch
        {
            return new List<TenureRuleItem>();
        }
    }
}

