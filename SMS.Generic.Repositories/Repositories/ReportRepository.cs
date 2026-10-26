using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

/// <inheritdoc />
public class ReportRepository : IReportRepository
{
    /// <summary>
    /// Very large <c>IN</c> lists can degrade plan cache and execution time; chunk vehicle lookups instead.
    /// </summary>
    private const int VehicleEquipmentIdChunkSize = 2000;

    private readonly SMSDbContext _context;
    private readonly ILogger<ReportRepository> _logger;

    public ReportRepository(SMSDbContext context, ILogger<ReportRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<PagedList<ReportRowDto>> GetReportRowsAsync(
        Guid departmentId,
        DateOnly? filterStartDate,
        DateOnly? filterEndDate,
        Guid? postId,
        string? searchTerm,
        PageParam pageParam,
        bool includeAllMatchingRows = false,
        bool? isPlaceholder = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var searchPattern = string.IsNullOrWhiteSpace(searchTerm)
                ? null
                : $"%{searchTerm.Trim()}%";

            var baseQuery =
                from pp in _context.PostPosition.AsNoTracking()
                join cpp in _context.ClosedPostPosition.AsNoTracking() on pp.PostPositionID equals cpp.PostPositionID into cppJoin
                from cpp in cppJoin
                    .Where(c => c.Status == null || c.Status == ClosedPostPositionStatus.Closed)
                    .OrderByDescending(c => c.CloseDateTime ?? DateTime.MinValue)
                    .Take(1)
                    .DefaultIfEmpty()
                join e in _context.Employee.AsNoTracking() on pp.EmployeeID equals e.EmployeeID into eJoin
                from e in eJoin.DefaultIfEmpty()
                join pl in _context.Placeholder.AsNoTracking() on pp.PlaceholderID equals pl.PlaceholderID into plJoin
                from pl in plJoin.DefaultIfEmpty()
                join p in _context.Post.AsNoTracking() on pp.PostID equals p.PostID
                join et in _context.EmployeeType.AsNoTracking() on (e != null ? e.EmployeeTypeID : (Guid?)null) equals et.EmployeeTypeID into etJoin
                from et in etJoin.DefaultIfEmpty()
                join pcs in _context.PostCallSign.AsNoTracking() on pp.PostCallSignID equals pcs.PostCallSignID into pcsJoin
                from pcs in pcsJoin.DefaultIfEmpty()
                join r in _context.Requests.AsNoTracking() on p.RequestID equals r.RequestID into rJoin
                from r in rJoin.DefaultIfEmpty()
                join req in _context.Requester.AsNoTracking() on r.RequesterID equals req.RequesterID into reqJoin
                from req in reqJoin.DefaultIfEmpty()
                join pc in _context.PayCode.AsNoTracking() on p.PayCodeID equals pc.PayCodeID into pcJoin
                from pc in pcJoin.DefaultIfEmpty()
                join dra in _context.DepartmentResponseArea.AsNoTracking() on p.PostResponseAreaId equals dra.DepartmentResponseAreaID into draJoin
                from dra in draJoin.DefaultIfEmpty()
                join postEt in _context.EmployeeType.AsNoTracking() on p.EmployeeTypeID equals postEt.EmployeeTypeID into postEtJoin
                from postEt in postEtJoin.DefaultIfEmpty()
                where pp.IsPlaceholder
                      || (cpp != null
                          && (pp.PostPositionStatus == PostPositionStatus.Closed || pp.PostPositionStatus == PostPositionStatus.ClosedAndSplit))
                where !isPlaceholder.HasValue || pp.IsPlaceholder == isPlaceholder.Value
                where (e != null && e.DepartmentID == departmentId) || (pl != null && pl.DepartmentID == departmentId)
                where !postId.HasValue || p.PostID == postId.Value
                where searchPattern == null
                      || (e != null && e.FirstName != null && EF.Functions.Like(e.FirstName, searchPattern))
                      || (e != null && e.LastName != null && EF.Functions.Like(e.LastName, searchPattern))
                      || (pl != null && pl.FirstName != null && EF.Functions.Like(pl.FirstName, searchPattern))
                      || (pl != null && pl.LastName != null && EF.Functions.Like(pl.LastName, searchPattern))
                      || (req != null && req.Name != null && EF.Functions.Like(req.Name, searchPattern))
                select new
                {
                    pp.PostPositionID,
                    ClosedPostPositionID = (Guid?)(cpp != null ? cpp.ClosedPostPositionID : null),
                    CloseDateTime = cpp != null ? cpp.CloseDateTime : (DateTime?)null,
                    HoursWorked = cpp != null ? cpp.HoursWorked : pp.LengthOfWork,
                    Comment = cpp != null ? cpp.Comment : null,
                    CloseBy = cpp != null ? cpp.CloseBy : null,
                    WorkStartDate = cpp != null ? cpp.WorkStartDate : pp.DateOfWork,
                    pp.DateOfWork,
                    pp.IsPlaceholder,
                    CallSign = !string.IsNullOrWhiteSpace(pp.CallSign) ? pp.CallSign : (pcs != null ? pcs.CallSign : null),
                    pp.EquipmentRequired,
                    p.PostRequiresVehicle,
                    EmployeeCode = pp.IsPlaceholder ? (pl != null ? pl.EmployeeNumber : null) : (e != null ? e.EmployeeCode : null),
                    FirstName = pp.IsPlaceholder ? (pl != null ? pl.FirstName : null) : (e != null ? e.FirstName : null),
                    LastName = pp.IsPlaceholder ? (pl != null ? pl.LastName : null) : (e != null ? e.LastName : null),
                    p.PostCode,
                    RequestCode = r != null ? r.RequestCode : null,
                    WorkStartTime = cpp != null ? cpp.WorkStartTime : pp.StartTime,
                    p.PostLength,
                    p.PostPayRate,
                    HourlySalary = pp.IsPlaceholder ? (decimal?)null : (e != null ? e.HourlySalary : null),
                    PayCodeValue = pc != null ? pc.PayCodeValue : null,
                    ResponseArea = dra != null
                        ? dra.DepartmentResponseAreaShort
                        : null,
                    RequestTitle = r != null ? r.Title : null,
                    RequestLocation = r != null ? r.Location : null,
                    RequesterName = req != null ? req.Name : null,
                    RequesterAddress = req != null ? req.Address : null,
                    RequesterAddress2 = req != null ? req.Address2 : null,
                    RequesterCity = req != null ? req.City : null,
                    RequesterState = req != null ? req.State : null,
                    RequesterZip = req != null ? req.Zip : null,
                    PostFor = et != null ? et.EmployeeTypeShort : null,
                    onDuty = cpp != null && cpp.WorkedOnDuty,
                    Payroll = pp.IsPlaceholder ? (pl != null ? pl.EmployeeNumber : null) : (e != null ? e.EmployeeNumber : null),
                    OffDutyCallBackPhoneNumber = pp.IsPlaceholder ? (pl != null ? pl.ContactNumber : null) : (e != null ? e.OffDutyCallBackPhoneNumber : null),
                    Ptaeo = (!string.IsNullOrWhiteSpace(p.ProjectNumber) &&
                             !string.IsNullOrWhiteSpace(p.TaskNumber) &&
                             !string.IsNullOrWhiteSpace(p.AwardNumber) &&
                             !string.IsNullOrWhiteSpace(p.FundNumber) &&
                             !string.IsNullOrWhiteSpace(p.OrganizationNumber))
                        ? p.ProjectNumber.Trim() + "." + p.TaskNumber.Trim() + "." + p.AwardNumber.Trim() + "." + p.FundNumber.Trim() + "." + p.OrganizationNumber.Trim()
                        : null,
                    Invoice = ((cpp != null && _context.InvoiceDetails.Any(d => d.ClosePostPositionID == cpp.ClosedPostPositionID && !d.IsDeleted))
                              || (r != null && _context.Invoices.Any(i => i.RequestId == r.RequestID && !i.IsDeleted)))
                        ? "Yes" : "No",
                    IsClosed = pp.PostPositionStatus == PostPositionStatus.Closed || pp.PostPositionStatus == PostPositionStatus.ClosedAndSplit,
                    PersonalRank = et != null ? et.EmployeeTypeShort : null,
                    ShiftRank = postEt != null ? postEt.EmployeeTypeShort : null
                };

            if (filterStartDate.HasValue || filterEndDate.HasValue)
            {
                baseQuery = baseQuery.Where(x =>
                    (x.DateOfWork.HasValue &&
                     (!filterStartDate.HasValue || x.DateOfWork.Value >= filterStartDate.Value) &&
                     (!filterEndDate.HasValue || x.DateOfWork.Value <= filterEndDate.Value))
                    ||
                    (!x.DateOfWork.HasValue && x.WorkStartDate.HasValue &&
                     (!filterStartDate.HasValue || x.WorkStartDate.Value >= filterStartDate.Value) &&
                     (!filterEndDate.HasValue || x.WorkStartDate.Value <= filterEndDate.Value)));
            }

            // Archived report: requester → job title → date → start time → officer name.
            // Null/blank requester and job title sort last (0 = has value, 1 = missing).
            var orderedQuery = baseQuery
                .OrderBy(x => string.IsNullOrWhiteSpace(x.RequesterName) ? 1 : 0)
                .ThenBy(x => x.RequesterName)
                .ThenBy(x => string.IsNullOrWhiteSpace(x.RequestTitle) ? 1 : 0)
                .ThenBy(x => x.RequestTitle)
                .ThenBy(x => (x.DateOfWork ?? x.WorkStartDate) ?? DateOnly.MaxValue)
                .ThenBy(x => x.WorkStartTime ?? TimeOnly.MaxValue)
                .ThenBy(x => x.LastName)
                .ThenBy(x => x.FirstName);

            // Fast native SQL COUNT (without sorting overhead)
            long totalRecords = await baseQuery.LongCountAsync(cancellationToken).ConfigureAwait(false);

            // Database-level paging (SQL Server executes OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY)
            var rawRows = includeAllMatchingRows
                ? await orderedQuery.ToListAsync(cancellationToken).ConfigureAwait(false)
                : await orderedQuery
                    .Skip(pageParam.Offset ?? PageParam.MinOffset)
                    .Take(pageParam.Limit ?? PageParam.MinLimit)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

            var cppIds = rawRows.Select(x => x.ClosedPostPositionID).Where(id => id.HasValue).Select(id => id.Value).Distinct().ToList();
            var vehicleEquipment = await LoadVehicleEquipmentRowsAsync(cppIds, cancellationToken).ConfigureAwait(false);

            var vehicleByCpp = vehicleEquipment
                .GroupBy(x => x.ClosedPostPositionId)
                .ToDictionary(
                    g => g.Key,
                    g => (
                        Numbers: (string?)string.Join(", ", g.Select(v => v.VehNumber).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct()),
                        Hours: g.Any(v => v.VehicleUsedHours.HasValue)
                            ? g.Sum(v => (v.VehicleUsedHours ?? 0) / 60m)
                            : (decimal?)null));

            var pageDtos = rawRows.Select(x =>
            {
                (string? Numbers, decimal? Hours) veh = default;
                if (x.ClosedPostPositionID.HasValue)
                {
                    vehicleByCpp.TryGetValue(x.ClosedPostPositionID.Value, out veh);
                }
                var vehicleNumber = string.IsNullOrWhiteSpace(veh.Numbers) ? null : veh.Numbers;
                var vehicleHrs = veh.Hours;
                var payCode = string.IsNullOrWhiteSpace(x.PayCodeValue)
                    ? null
                    : string.Join(' ', x.PayCodeValue.Replace('\u00A0', ' ').Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
                var hoursWorked = x.HoursWorked / 60m;
                var empHrlyRate = x.HourlySalary;
                var postPayRate = x.PostPayRate;

                return new ReportRowDto(
                    employeeCode: x.EmployeeCode,
                    employeeName: BuildPersonName(x.LastName, x.FirstName),
                    payroll: x.Payroll,
                    pvt: string.IsNullOrWhiteSpace(x.OffDutyCallBackPhoneNumber)
                        ? null
                        : x.OffDutyCallBackPhoneNumber.Trim(),
                    requestCode: x.RequestCode,
                    postCode: x.PostCode,
                    hoursWorked: hoursWorked,
                    postDay: x.DateOfWork ?? x.WorkStartDate,
                    payCode: payCode,
                    comments: x.Comment,
                    requesterName: x.RequesterName,
                    requesterAddress: AddressFormatHelper.BuildSingleLine(
                        x.RequesterAddress,
                        x.RequesterAddress2,
                        x.RequesterCity,
                        x.RequesterState,
                        x.RequesterZip),
                    startTime: x.WorkStartTime,
                    hoursRequired: x.PostLength / 60m,
                    responseArea: x.ResponseArea,
                    requestAddress: x.RequestLocation,
                    callSign: x.CallSign,
                    vehicle: x.PostRequiresVehicle ? "Yes" : "No",
                    onDuty: x.onDuty,
                    postFor: x.PostFor,
                    empHrlyRate: empHrlyRate,
                    postPayRate: postPayRate,
                    vehicleNumber: vehicleNumber,
                    vehicleHrs: vehicleHrs,
                    requestName: x.RequestTitle,
                    agency: null,
                    closedBy: x.CloseBy,
                    ptaeo: x.Ptaeo,
                    invoice: x.Invoice,
                    isPlaceholder: x.IsPlaceholder,
                    isClosed: x.IsClosed,
                    vehicleClosedHours: vehicleHrs,
                    personalRank: x.PersonalRank,
                    shiftRank: x.ShiftRank);
            }).ToList();

            var responsePaging = includeAllMatchingRows
                ? new PageParam
                {
                    Offset = 0,
                    Limit = totalRecords > int.MaxValue ? int.MaxValue : (int)totalRecords,
                }
                : pageParam;

            return new PagedList<ReportRowDto>(pageDtos, responsePaging, totalRecords);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error loading report rows (department: {DepartmentId}, filterStart: {FilterStart}, filterEnd: {FilterEnd})",
                departmentId,
                filterStartDate,
                filterEndDate);
            throw;
        }
    }

    private async Task<List<(Guid ClosedPostPositionId, string? VehNumber, int? VehicleUsedHours)>> LoadVehicleEquipmentRowsAsync(
        List<Guid> cppIds,
        CancellationToken cancellationToken)
    {
        if (cppIds.Count == 0)
        {
            return new List<(Guid, string?, int?)>();
        }

        async Task<List<(Guid ClosedPostPositionId, string? VehNumber, int? VehicleUsedHours)>> QueryChunkAsync(
            IReadOnlyList<Guid> ids)
        {
            var equipRows = await (
                from cpe in _context.ClosedPostPositionEquipment.AsNoTracking()
                join de in _context.DepartmentEquipments.AsNoTracking() on cpe.DepartmentEquipmentID equals de.DepartmentEquipmentID
                join et in _context.EquipmentTypes.AsNoTracking() on de.EquipmentTypeID equals et.EquipementTypeID
                where ids.Contains(cpe.ClosedPostPositionID) && et.IsVehicle == true
                select new { cpe.ClosedPostPositionID, cpe.VehNumber, cpe.VehicleUsedHours }
            ).ToListAsync(cancellationToken).ConfigureAwait(false);

            return equipRows
                .Select(c => (ClosedPostPositionId: c.ClosedPostPositionID, VehNumber: (string?)c.VehNumber, VehicleUsedHours: c.VehicleUsedHours))
                .ToList();
        }

        if (cppIds.Count <= VehicleEquipmentIdChunkSize)
        {
            return await QueryChunkAsync(cppIds).ConfigureAwait(false);
        }

        var merged = new List<(Guid ClosedPostPositionId, string? VehNumber, int? VehicleUsedHours)>();
        foreach (var chunk in cppIds.Chunk(VehicleEquipmentIdChunkSize))
        {
            merged.AddRange(await QueryChunkAsync(chunk).ConfigureAwait(false));
        }

        return merged;
    }

    private static string? BuildPersonName(string? lastName, string? firstName)
    {
        var parts = new[] { lastName,firstName }
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s!.Trim())
            .ToList();
        return parts.Count == 0 ? null : string.Join(", ", parts);
    }

    /// <inheritdoc />
    public async Task<PagedList<WatchCommanderReportDto>> GetWatchCommanderReportPagedAsync(
        Guid departmentId,
        DateOnly? startDate,
        DateOnly? endDate,
        string? searchTerm,
        PageParam? pageParam,
        bool isGenerateReport = false,
        CancellationToken cancellationToken = default)
    {
        var effectivePage = pageParam ?? new PageParam();

        var filtered = BuildWatchCommanderFilteredQuery(departmentId, startDate, endDate, searchTerm);

        var total = await filtered.CountAsync(cancellationToken).ConfigureAwait(false);

        var skip = effectivePage.Offset!.Value;
        var take = effectivePage.Limit!.Value;

        var sorted = ApplyWatchCommanderSorting(filtered, effectivePage);
        var windowed = isGenerateReport ? sorted : sorted.Skip(skip).Take(take);

        var rawRows = await windowed
            .Select(pp => new
            {
                StartDate = pp.DateOfWork,
                EmployeeName = pp.Employee == null
                    ? null
                    : (pp.Employee.LastName == null || pp.Employee.LastName == "")
                        && (pp.Employee.FirstName == null || pp.Employee.FirstName == "")
                        ? null
                        : (pp.Employee.LastName == null || pp.Employee.LastName == "")
                            ? pp.Employee.FirstName
                            : (pp.Employee.FirstName == null || pp.Employee.FirstName == "")
                                ? pp.Employee.LastName
                                : pp.Employee.LastName + ", " + pp.Employee.FirstName,
                CallSign = !string.IsNullOrWhiteSpace(pp.CallSign) ? pp.CallSign : (pp.PostCallSign != null ? pp.PostCallSign.CallSign : null),
                EmployeeType = pp.Employee != null && pp.Employee.EmployeeType != null
                    ? pp.Employee.EmployeeType.EmployeeTypeShort
                    : null,
                StartTime = pp.StartTime,
                EndTime = pp.EndTime,
                HoursWorked = pp.Post != null ? (decimal?)pp.Post.PostLength / 60m : null,
                Requester = pp.Post != null && pp.Post.Request != null && pp.Post.Request.Requester != null
                    ? pp.Post.Request.Requester.Name
                    : null,
                LocAddress = pp.Post != null && pp.Post.Request != null ? pp.Post.Request.Address : null,
                LocCity = pp.Post != null && pp.Post.Request != null ? pp.Post.Request.City : null,
                LocState = pp.Post != null && pp.Post.Request != null ? pp.Post.Request.State : null,
                LocZip = pp.Post != null && pp.Post.Request != null ? pp.Post.Request.Zip : null,
                Vehicle = pp.Post != null && pp.Post.PostRequiresVehicle,
                Payroll = pp.Employee.EmployeeNumber
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var rows = rawRows
            .Select(r => new WatchCommanderReportDto
            {
                StartDate = r.StartDate,
                EmployeeName = r.EmployeeName,
                Payroll = r.Payroll,
                CallSign = r.CallSign,
                EmployeeType = r.EmployeeType,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                HoursWorked = r.HoursWorked,
                Requester = r.Requester,
                RequesterAddress = AddressFormatHelper.BuildStreetCityStateZipLine(
                    r.LocAddress,
                    r.LocCity,
                    r.LocState,
                    r.LocZip),
                Vehicle = r.Vehicle,
            })
            .ToList();

        if (rows.Count == 0)
        {
            _logger.LogDebug("Watch commander report: page empty (total {Total}).", total);
        }
        else if (isGenerateReport)
        {
            _logger.LogDebug("Watch commander report: all {Count} rows (total {Total}).", rows.Count, total);
        }

        var resultPageParam = isGenerateReport
            ? new PageParam
            {
                SortBy = effectivePage.SortBy,
                SortDirection = effectivePage.SortDirection,
                Offset = 0,
                Limit = rows.Count > 0 ? rows.Count : effectivePage.Limit,
            }
            : effectivePage;

        return new PagedList<WatchCommanderReportDto>(rows, resultPageParam, total);
    }

    /// <summary>
    /// Base watch-commander query: status/department filters and optional date/search predicates are translated to SQL before paging.
    /// </summary>
    private IQueryable<PostPosition> BuildWatchCommanderFilteredQuery(
        Guid departmentId,
        DateOnly? startDate,
        DateOnly? endDate,
        string? searchTerm)
    {
        var hasRange = startDate.HasValue || endDate.HasValue;
        var rangeStart = hasRange ? startDate ?? DateOnly.MinValue : default;
        var rangeEnd = hasRange ? endDate ?? DateOnly.MaxValue : default;
        var searchPattern = string.IsNullOrWhiteSpace(searchTerm)
            ? null
            : $"%{searchTerm.Trim()}%";

        var employees = _context.Employee.AsNoTracking().Where(e => e.DepartmentID == departmentId);

        if (searchPattern != null)
        {
            employees = employees.Where(e =>
                (e.FirstName != null && EF.Functions.Like(e.FirstName, searchPattern))
                || (e.LastName != null && EF.Functions.Like(e.LastName, searchPattern)));
        }

        var q = _context.PostPosition
            .AsNoTracking()
            .Where(pp =>
                (pp.PostPositionStatus == PostPositionStatus.Assigned
                 || pp.PostPositionStatus == PostPositionStatus.CancellationPending)
                && !pp.IsPlaceholder
                && pp.EmployeeID.HasValue
                && pp.EmployeeID != Guid.Empty
                && employees.Any(e => e.EmployeeID == pp.EmployeeID));

        if (hasRange)
        {
            q = q.Where(pp => pp.DateOfWork >= rangeStart && pp.DateOfWork <= rangeEnd);
        }

        return q;
    }

    private static IOrderedQueryable<PostPosition> ApplyWatchCommanderSorting(
        IQueryable<PostPosition> query,
        PageParam pageParam)
    {
        return query
            .OrderBy(pp => string.IsNullOrWhiteSpace(
                pp.Post != null && pp.Post.Request != null && pp.Post.Request.Requester != null
                    ? pp.Post.Request.Requester.Name
                    : null)
                ? 1
                : 0)
            .ThenBy(pp => pp.Post != null && pp.Post.Request != null && pp.Post.Request.Requester != null
                ? pp.Post.Request.Requester.Name
                : null)
            .ThenBy(pp => pp.DateOfWork)
            .ThenBy(pp => pp.StartTime)
            .ThenBy(pp => pp.Employee != null ? pp.Employee.LastName : null);
    }
}
