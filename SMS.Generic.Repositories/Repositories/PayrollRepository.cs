using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

/// <summary>
/// Repository for payroll data operations
/// </summary>
public class PayrollRepository : IPayrollRepository
{
    private readonly SMSDbContext _context;
    private readonly ILogger<PayrollRepository> _logger;

    public PayrollRepository(SMSDbContext context, ILogger<PayrollRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Gets payroll data for the given close window (one export row per matching closed post-position row, not deduplicated by assignment).
    /// </summary>
    /// <param name="startDate">Inclusive close-window start in each row&apos;s department-local wall clock (UTC <c>CloseDateTime</c> is converted before comparison).</param>
    /// <param name="endDate">Exclusive close-window end (same convention as <paramref name="startDate"/>).</param>
    /// <returns>List of payroll records</returns>
    public async Task<List<PayrollDataRecordDto>> GetPayrollDataAsync(
        DateTime startDate,
        DateTime endDate,
        Guid? departmentId = null)
    {
        try
        {
            _logger.LogInformation(
                "Retrieving payroll data using close window {StartDate} to {EndDate}{DeptFilter}",
                startDate,
                endDate,
                departmentId.HasValue && departmentId.Value != Guid.Empty
                    ? $", department {departmentId.Value}"
                    : string.Empty);

            var applyDepartmentFilter = departmentId.HasValue && departmentId.Value != Guid.Empty;
            var departmentIdFilter = applyDepartmentFilter ? departmentId!.Value : Guid.Empty;

            string? scopedDepartmentTimeZoneId = null;
            if (applyDepartmentFilter)
            {
                scopedDepartmentTimeZoneId = await _context.Set<Department>().AsNoTracking()
                    .Where(x => x.DepartmentID == departmentIdFilter).Select(x => x.TimeZoneId).FirstOrDefaultAsync();
            }

            // Join PostPosition by ClosedPostPosition.PostPositionID (not PostID — same PostID can have many positions).
            // Join Post by that position's PostID for PTAEO fields and pay code.
            var rawDataQuery = from cpp in _context.Set<ClosedPostPosition>()
                               join pp in _context.Set<PostPosition>() on cpp.PostPositionID equals pp.PostPositionID
                               join post in _context.Set<Post>() on cpp.PostID equals post.PostID
                               join e in _context.Set<Employee>() on pp.EmployeeID equals e.EmployeeID into employeeGroup
                               from e in employeeGroup.DefaultIfEmpty()
                               join d in _context.Set<Department>() on e.DepartmentID equals d.DepartmentID into deptGroup
                               from d in deptGroup.DefaultIfEmpty()
                               join pc in _context.Set<PayCode>() on post.PayCodeID equals pc.PayCodeID into payCodeGroup
                               from pc in payCodeGroup.DefaultIfEmpty()
                                   // LEFT JOIN Request
                               join r in _context.Set<Request>()
                                   on post.RequestID equals r.RequestID into requestGroup
                               from r in requestGroup.DefaultIfEmpty()
                               join ph in _context.Set<Placeholder>() on pp.PlaceholderID equals ph.PlaceholderID into placeholderGroup
                               from ph in placeholderGroup.DefaultIfEmpty()

                               where cpp.CloseDateTime.HasValue
                                     && cpp.Status == ClosedPostPositionStatus.Closed
                                && !cpp.WorkedOnDuty
                                     && (!pp.IsPlaceholder || pp.EnableInvoiceAndPayroll)
                                && (
                                       !applyDepartmentFilter
                                       || (e != null && e.DepartmentID == departmentIdFilter)
                                       || (e == null && r != null && r.DepartmentID == departmentIdFilter)
                                   ) && cpp.HoursWorked > 0
                               select new
                               {
                                   cpp,
                                   pp,
                                   post,
                                   e,
                                   d,
                                   pc,
                                   r,
                                   ph
                               };

            // Close timestamps are persisted in UTC; startDate/endDate are interpreted in each row department time zone.
            // EF Core cannot translate TimeZoneInfo.ConvertTimeFromUtc — filter after materializing this narrow join.
            var staged = await rawDataQuery.ToListAsync();

            var closeWindowFiltered = staged
                .Where(x =>
                {
                    var closeUtc = x.cpp.CloseDateTime!.Value;
                    var closeDeptLocal = closeUtc.UtcInstantToDepartmentClock(scopedDepartmentTimeZoneId);
                    return closeDeptLocal >= startDate && closeDeptLocal < endDate;
                }).ToList();

            var rawData = closeWindowFiltered
                .GroupBy(x => new
                {
                    EmployeeKey = x.e != null ? x.e.EmployeeID : x.pp.PostPositionID,
                    x.post.RequestID,
                    x.post.PostID,
                    x.cpp.WorkStartDate,
                    x.cpp.WorkStartTime,
                    x.cpp.HoursWorked
                })
                .Select(g => g.First())
                .ToList();

            var records = rawData
                .Select(x => new PayrollDataRecordDto
                {
                    IsPlaceholder = x.pp.IsPlaceholder,
                    EmployeeNumber = x.pp.IsPlaceholder
                        ? (x.ph?.EmployeeNumber ?? string.Empty)
                        : (x.e?.EmployeeNumber ?? string.Empty),
                    StartDateTime = DateHelper.CombineDateAndTime(
                        x.cpp.WorkStartDate!.Value,
                        x.cpp.WorkStartTime),

                    EndDateTime = DateHelper.CalculateEndDateTime(
                        x.cpp.WorkStartDate!.Value,
                        x.cpp.WorkStartTime,
                        x.cpp.HoursWorked),

                    Duration = TimeSpan.FromMinutes(x.cpp.HoursWorked).TotalHours,

                    PayCode = x.pc != null ? x.pc.PayCodeValue ?? "N/A" : "N/A",
                    Project = x.post.ProjectNumber,
                    Task = x.post.TaskNumber,
                    Award = x.post.AwardNumber,
                    Fund = x.post.FundNumber,

                    DepartmentId = x.e?.DepartmentID ?? x.r?.DepartmentID ?? Guid.Empty,
                    DepartmentExcelFolderName = x.d != null ? x.d.EmployeeDataFolderName : null
                })
                .ToList();

            _logger.LogInformation("Retrieved {Count} payroll records", records.Count);
            return records;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payroll data");
            throw;
        }
    }
}