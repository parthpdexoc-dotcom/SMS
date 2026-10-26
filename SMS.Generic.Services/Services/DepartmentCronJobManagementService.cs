using System;
using System.Linq;
using System.Net;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Mapping;

namespace SMS.Generic.Services.Services;

/// <summary>Configures per-department cron rows; field ↔ cron mapping is in <see cref="DepartmentCronScheduleMapping" />.</summary>
public class DepartmentCronJobManagementService : IDepartmentCronJobManagementService
{
    private readonly IDepartmentCronJobManagementRepository _repository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ITokenParser _tokenParser;
    private readonly IMapper _mapper;
    private readonly ILogger<DepartmentCronJobManagementService> _logger;

    public DepartmentCronJobManagementService(
        IDepartmentCronJobManagementRepository repository,
        IDepartmentRepository departmentRepository,
        ITokenParser tokenParser,
        IMapper mapper,
        ILogger<DepartmentCronJobManagementService> logger)
    {
        _repository = repository;
        _departmentRepository = departmentRepository;
        _tokenParser = tokenParser;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DepartmentCronScheduleDto>> GetSchedulesByDepartmentIdAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        EnsureCanAccessDepartment(departmentId);
        if (await _departmentRepository.GetByIdAsync<Guid>(departmentId, "DepartmentID").ConfigureAwait(false) is null)
            throw new WebApiException(HttpStatusCode.NotFound, ((int)HttpStatusCode.NotFound).ToString(), "Department not found.");

        var rows = (await _repository
            .GetAllAsync(x => x.DepartmentID == departmentId, null, null)
            .ConfigureAwait(false))
            .ToList();
        var list = new List<DepartmentCronScheduleDto>();
        foreach (var row in rows.OrderBy(r => (int)r.Job))
        {
            list.Add(_mapper.Map<DepartmentCronScheduleDto>(row));
        }
        return list;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DepartmentCronJobTypeOptionDto>> GetUnconfiguredJobTypeOptionsForDepartmentAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        EnsureCanAccessDepartment(departmentId);
        if (await _departmentRepository.GetByIdAsync<Guid>(departmentId, "DepartmentID").ConfigureAwait(false) is null)
            throw new WebApiException(HttpStatusCode.NotFound, ((int)HttpStatusCode.NotFound).ToString(), "Department not found.");

        var configured = (await _repository
            .GetAllAsync(x => x.DepartmentID == departmentId, null, null)
            .ConfigureAwait(false))
            .Select(x => x.Job)
            .ToHashSet();

        var list = new List<DepartmentCronJobTypeOptionDto>();
        foreach (var job in Enum.GetValues<JobType>().Cast<JobType>().OrderBy(j => (int)j))
        {
            if (configured.Contains(job))
            {
                continue;
            }
            list.Add(
                new DepartmentCronJobTypeOptionDto
                {
                    Job = job,
                    JobName = EnumHelper.ToStringValue(job),
                });
        }
        return list;
    }

    /// <inheritdoc />
    public async Task<DepartmentCronScheduleDto> UpsertScheduleAsync(DepartmentCronScheduleUpsertDto dto,CancellationToken cancellationToken = default)
    {
        EnsureCanAccessDepartment(dto.DepartmentID);
        var department = await _departmentRepository
            .GetByIdAsync<Guid>(dto.DepartmentID, "DepartmentID")
            .ConfigureAwait(false);
        if (department is null)
            throw new WebApiException(HttpStatusCode.NotFound, ((int)HttpStatusCode.NotFound).ToString(), "Department not found.");

        string cron;
        try
        {
            cron = DepartmentCronScheduleMapping.BuildStoredCron(dto);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid schedule fields for job {Job}", dto.Job);
            throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), ex.Message);
        }

        var fullName = ResolveFullName();
        var adminId = _tokenParser.GetAdminID();

        // At most one row per (DepartmentID, Job) — see unique index; use a filter like everywhere else in the app.
        var existing = (await _repository
            .GetAllAsync(
                x => x.DepartmentID == dto.DepartmentID && x.Job == dto.Job,
                null,
                null)
            .ConfigureAwait(false))
            .FirstOrDefault();
        if (existing != null)
        {
            if (dto.DepartmentCronJobManagementId is { } cid && cid != existing.DepartmentCronJobManagementId)
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    "Row id is not valid for this department and job.");
            }
            existing.CronExpression = cron;
            existing.EditedDate = DateTime.UtcNow;
            if (adminId.HasValue)
                existing.EditedUserID = adminId;
            existing.EditedBy = fullName;
            await _repository
                .UpsertAsync(existing, nameof(DepartmentCronJobManagement.DepartmentCronJobManagementId))
                .ConfigureAwait(false);
            return _mapper.Map<DepartmentCronScheduleDto>(existing);
        }

        // Primary key is generated in SQL (newid()); do not set DepartmentCronJobManagementId in code.
        var row = new DepartmentCronJobManagement
        {
            DepartmentID = dto.DepartmentID,
            Job = dto.Job,
            CronExpression = cron,
            CreatedDate = DateTime.UtcNow,
            EditedDate = null,
        };
        if (adminId.HasValue)
            row.CreatedUserID = adminId;
        row.CreatedBy = fullName;
        await _repository
            .UpsertAsync(row, nameof(DepartmentCronJobManagement.DepartmentCronJobManagementId))
            .ConfigureAwait(false);
        return _mapper.Map<DepartmentCronScheduleDto>(row);
    }

    private void EnsureCanAccessDepartment(Guid departmentId)
    {
        // Align with [Authorize(DepartmentAdminOnly)]: superusers are identified by the role claim,
        // not only IsSuperUser — tokens may omit the latter while still carrying role "SuperUser".
        if (_tokenParser.IsInRole(RoleTypeNames.SuperUser()))
            return;

        var isSuperUserClaim = _tokenParser.GetClaimValue(ClaimNames.IsSuperUser);
        if (string.Equals(isSuperUserClaim, "true", StringComparison.OrdinalIgnoreCase)
            || string.Equals(isSuperUserClaim, "1", StringComparison.Ordinal)
            || (bool.TryParse(isSuperUserClaim, out var isSu) && isSu)
            || _tokenParser.GetIntClaim(ClaimNames.IsSuperUser) == 1)
            return;

        var fromToken = _tokenParser.GetDepartmentID();
        if (!fromToken.HasValue || fromToken.Value != departmentId)
            throw new WebApiException(
                HttpStatusCode.Forbidden,
                ((int)HttpStatusCode.Forbidden).ToString(),
                "You are not allowed to access this department.");
    }

    private string? ResolveFullName()
    {
        var full = _tokenParser.GetClaimValue(ClaimNames.FullName);
        if (!string.IsNullOrWhiteSpace(full))
            return full;
        var f = _tokenParser.GetClaimValue(ClaimNames.FirstName);
        var l = _tokenParser.GetClaimValue(ClaimNames.LastName);
        return $"{f} {l}".Trim();
    }
}
