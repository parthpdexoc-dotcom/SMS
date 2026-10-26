using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SMS.Data;
using SMS.Generic.Common;
using SMS.Generic.Common.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Net;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing RequestSchedule operations using the generic service pattern.
    /// </summary>
    public class RequestScheduleService : GenericService<RequestScheduleDto, RequestSchedule>, IRequestScheduleService
    {
        private readonly IRequestScheduleRepository _repository;
        private readonly IRequestRepository _requestRepository;
        private readonly IDepartmentConfigurationService _departmentConfigurationService;
        private readonly IRequestScheduleGenerationService _generationService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IPostService _postService;
        private readonly SMSDbContext _dbContext;

        public RequestScheduleService(
            IRequestScheduleRepository repository,
            IRequestRepository requestRepository,
            IDepartmentConfigurationService departmentConfigurationService,
            IRequestScheduleGenerationService generationService,
            IMapper mapper,
            ITokenParser tokenParser,
            ILogger<RequestScheduleService> logger,
            IPostService postService,
            SMSDbContext dbContext)
            : base(repository, mapper, "RequestScheduleID", tokenParser)
        {
            _repository = repository;
            _requestRepository = requestRepository;
            _departmentConfigurationService = departmentConfigurationService;
            _generationService = generationService;
            _mapper = mapper;
            _logger = logger;
            _postService = postService;
            _dbContext = dbContext;
        }

        public override async Task<RequestScheduleDto> UpsertAsync(RequestScheduleDto dto, bool isSuperUser = false)
        {
            RequestScheduleDto? before = null;
            if (dto.RequestScheduleID != Guid.Empty)
                before = await GetByIdAsync(dto.RequestScheduleID).ConfigureAwait(false);

            var result = await base.UpsertAsync(dto, isSuperUser).ConfigureAwait(false);

            var isReleaseNow = result.RequestStatus == RequestStatus.ReleaseNow;
            if (isReleaseNow && result.RequestId != Guid.Empty)
            {
                await _postService.NotifyJobsPostedAsync(result.RequestId).ConfigureAwait(false);
            }

            return result;
        }

        public override async Task<bool> DeleteAsync<TKey>(TKey id)
        {
            if (id is Guid scheduleId && scheduleId != Guid.Empty)
            {
                var schedule = await GetByIdAsync(scheduleId).ConfigureAwait(false);
                if (schedule != null)
                {
                    var scheduleCount = await GetFilteredCountAsync(s => s.RequestId == schedule.RequestId).ConfigureAwait(false);
                    if (scheduleCount <= 1)
                    {
                        throw new WebApiException(
                            HttpStatusCode.BadRequest,
                            "400",
                            ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_MUST_HAVE_AT_LEAST_ONE_SCHEDULE));
                    }
                }
            }

            return await base.DeleteAsync(id).ConfigureAwait(false);
        }

        public Task<RequestScheduleBulkUpdateResult> UpdateReleasedOnToReleasedAsync(DateTime asOfUtc) =>
            _repository.UpdateReleasedOnToReleasedAsync(asOfUtc);

        public Task<RequestScheduleBulkUpdateResult> UpdateReleasedToCompleteAsync(DateTime asOfUtc) =>
            _repository.UpdateReleasedToCompleteAsync(asOfUtc);

        public async Task UpsertRequestProcessingResultAsync(Guid requestId,RequestScheduleProcessingResultDto result,CancellationToken ct,ReleaseAs? releaseAsOnComplete = null)
        {
            // ✅ FIX: use repository/service instead of DbContext
            var schedule = (await _repository.GetAllAsync(
                filter: s => s.RequestId == requestId &&
                             s.ProcessingStatus == ProcessingStatus.InProgress
            )).FirstOrDefault();

            if (schedule == null)
                return;

            // ✅ Update fields
            schedule.ProcessingResult = JsonConvert.SerializeObject(result);

            schedule.ProcessingStatus = result.IsSuccess
                ? ProcessingStatus.Complete
                : ProcessingStatus.Failed;

            if (releaseAsOnComplete.HasValue)
                schedule.ReleaseAs = releaseAsOnComplete.Value;

            schedule.ProcessedAt = DateTime.UtcNow;
            schedule.EditedDate = DateTime.UtcNow;
            var dto = _mapper.Map<RequestScheduleDto>(schedule);
            await UpsertAsync(dto);
        }

        /// <inheritdoc />
        public async Task<bool> GetHasAppliedInTenureByDateAsync(Guid requestId, DateOnly dateOfWork)
        {
            if (requestId == Guid.Empty)
                return false;

            var schedules = await GetAllAsync(
                filter: s => s.RequestId == requestId &&
                             s.ReleaseAs == ReleaseAs.TenureBased &&
                             (s.StartDate == null || dateOfWork >= s.StartDate) &&
                             (s.EndDate == null || dateOfWork <= s.EndDate)
            );

            return schedules.Any();
        }

        /// <inheritdoc />
        public Task<PagedList<RequestScheduleDto>> GetByRequestIdAsync(
            Guid requestId,
            DateOnly? startDate,
            DateOnly? endDate,
            IReadOnlyCollection<ReleaseAs>? releaseAs,
            IReadOnlyCollection<RequestStatus>? requestStatus,
            PageParam? pageParam)
        {
            var selectedReleaseAs = releaseAs is { Count: > 0 } ? releaseAs.ToArray() : Array.Empty<ReleaseAs>();
            var selectedStatuses = requestStatus is { Count: > 0 } ? requestStatus.ToArray() : Array.Empty<RequestStatus>();
            var filterReleaseAs = selectedReleaseAs.Length > 0;
            var filterStatus = selectedStatuses.Length > 0;
            if (!startDate.HasValue && !endDate.HasValue && !filterReleaseAs && !filterStatus)
                return GetAllPagedAsync(x => x.RequestId == requestId, pageParam);

            return GetAllPagedAsync(
                x => x.RequestId == requestId
                    && (!startDate.HasValue || (x.EndDate.HasValue && x.EndDate >= startDate))
                    && (!endDate.HasValue || (x.StartDate.HasValue && x.StartDate <= endDate))
                    && (!filterReleaseAs || selectedReleaseAs.Contains(x.ReleaseAs))
                    && (!filterStatus || selectedStatuses.Contains(x.RequestStatus)),
                pageParam);
        }

        /// <inheritdoc />
        public async Task<RequestScheduleDto> GenerateAsync(GenerateRequestScheduleDto dto, bool isSuperUser = false)
        {
            if (dto.RequestId == Guid.Empty)
                ThrowBadRequest(ModelValidationErrorCodes.REQUEST_NOT_FOUND, dto.RequestId);

            var request = await _requestRepository
                .GetByIdAsync(dto.RequestId, nameof(Request.RequestID), x => x.RequestType!)
                .ConfigureAwait(false);
            if (request == null)
            {
                throw new WebApiException(
                    HttpStatusCode.NotFound,
                    ((int)HttpStatusCode.NotFound).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_NOT_FOUND, dto.RequestId));
            }

            var weekStartDay = DayOfWeek.Sunday;
            if (request.DepartmentID is Guid departmentId)
            {
                var departmentConfig = await _departmentConfigurationService
                    .GetByDepartmentIdAsync(departmentId)
                    .ConfigureAwait(false);
                if (departmentConfig != null && !departmentConfig.EnableGenerateRequestSchedule)
                    ThrowBadRequest(ModelValidationErrorCodes.REQUEST_SCHEDULE_GENERATE_NOT_ENABLED);
                if (departmentConfig?.OnDutyStartDayOfWeek is >= 0 and <= 6)
                    weekStartDay = (DayOfWeek)departmentConfig.OnDutyStartDayOfWeek;
            }

            var type = request.RequestType?.Type ?? RequestTypes.OneTime;
            if (type is RequestTypes.Daily or RequestTypes.OneTime)
                ThrowBadRequest(ModelValidationErrorCodes.REQUEST_SCHEDULE_GENERATE_DAILY_NOT_ALLOWED);
            if (request.EndDate < request.StartDate)
                ThrowBadRequest(ModelValidationErrorCodes.INVALID_REQUEST_SCHEDULE_REQUEST_DATE_RANGE);

            // Schedule windows do not use the request "On N" day.
            // Only the generate modal Release Before selection must include a day.
            var existingGeneration = (await _generationService
                .GetAllAsync(g => g.RequestId == dto.RequestId, null, null)
                .ConfigureAwait(false))
                .OrderByDescending(g => g.CreatedDate)
                .FirstOrDefault();
            var (typeChanged, endChanged, patternChanged, timeChanged) = GetGenerateChanges(dto, existingGeneration);
            var startChanged = existingGeneration != null && request.StartDate != existingGeneration.RequestStartDate;
            var (effectiveType, effectiveEnd, scheduledTime, releaseBefore, occurrence) =
                GetGenerateSettings(dto, request, type, existingGeneration, typeChanged);
            RequireGenerateDays(effectiveType, releaseBefore);
            var replaceFuture = existingGeneration == null || typeChanged;
            var scheduleChanged = replaceFuture || patternChanged || timeChanged || endChanged || startChanged;

            var createdCount = 0;
            string? tenureNotice = null;
            var saved = await _dbContext.ExecuteInTransactionAsync(async () =>
            {
                var existing = (await _repository
                    .GetAllAsync(s => s.RequestId == dto.RequestId)
                    .ConfigureAwait(false)).ToList();
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var current = existing
                    .Where(s => s.StartDate <= today && s.EndDate >= today)
                    .OrderByDescending(s => s.RequestStatus == RequestStatus.ReleaseNow)
                    .ThenByDescending(s => s.StartDate)
                    .FirstOrDefault();
                var past = existing.Where(s => s.EndDate < today).ToList();
                var future = existing.Where(s => s != current && s.EndDate >= today).ToList();
                var releaseAsChanged = future.Any(s =>
                    s.RequestStatus != RequestStatus.ReleaseNow && s.ReleaseAs != request.ReleaseAs);
                var (toInsert, toDelete, notice) = BuildRegeneration(
                    request, dto, existingGeneration, replaceFuture, endChanged, startChanged, patternChanged, timeChanged, releaseAsChanged,
                    effectiveType, effectiveEnd, scheduledTime, releaseBefore, occurrence, current, future, weekStartDay);
                tenureNotice = notice;

                if (toInsert.Any(o => OverlapsAny(past, o)))
                    ThrowBadRequest(ModelValidationErrorCodes.DUPLICATE_REQUEST_SCHEDULE);

                if (scheduleChanged)
                {
                    await _generationService.UpsertAsync(new RequestScheduleGenerationDto
                    {
                        RequestScheduleGenerationID = existingGeneration?.RequestScheduleGenerationID ?? Guid.Empty,
                        RequestId = dto.RequestId,
                        Action = existingGeneration == null
                            ? RequestScheduleGenerationAction.Generated
                            : RequestScheduleGenerationAction.Updated,
                        RequestType = effectiveType,
                        RequestStartDate = request.StartDate,
                        RequestEndDate = effectiveEnd,
                        EffectiveFromDate = existingGeneration == null || replaceFuture
                            || ResolveGenerateRangeStart(request.StartDate) < existingGeneration.EffectiveFromDate
                            ? ResolveGenerateRangeStart(request.StartDate)
                            : existingGeneration.EffectiveFromDate,
                        ReleaseBeforeDaysOfWeek = releaseBefore.DaysOfWeek,
                        ReleaseBeforeDaysOfMonth = releaseBefore.DaysOfMonth,
                        ReleaseBeforeMonthlyMode = releaseBefore.MonthlyMode,
                        ReleaseBeforeWeekIndex = releaseBefore.WeekIndex,
                        ScheduledTime = scheduledTime,
                        CreatedDate = existingGeneration?.CreatedDate ?? DateTime.UtcNow
                    }, isSuperUser).ConfigureAwait(false);
                }

                if (toDelete.Count > 0)
                    _dbContext.RequestSchedule.RemoveRange(toDelete);

                // The week that includes today must stay. A past release day makes that week Open,
                // and it still belongs on the Current tab.
                if (effectiveType == RequestTypes.Weekly && today >= request.StartDate && today <= effectiveEnd)
                {
                    bool CoversToday(DateOnly? start, DateOnly? end) =>
                        start is DateOnly rowStart && end is DateOnly rowEnd && rowStart <= today && rowEnd >= today;

                    var alreadyCoversToday =
                        (current != null && !toDelete.Contains(current) && CoversToday(current.StartDate, current.EndDate))
                        || future.Any(s => !toDelete.Contains(s) && CoversToday(s.StartDate, s.EndDate))
                        || toInsert.Any(o => CoversToday(o.StartDate, o.EndDate));
                    if (!alreadyCoversToday)
                    {
                        var weekEnd = today.GetDepartmentWeekRange(weekStartDay).WeekEnd;
                        if (weekEnd > effectiveEnd)
                            weekEnd = effectiveEnd;
                        if (weekEnd >= today)
                        {
                            var currentWeek = BuildGenerateOccurrences(
                                request, effectiveType, occurrence, releaseBefore, dto.ReleaseBeforeDate, scheduledTime,
                                today, weekEnd, dto.ReleaseHours, weekStartDay, adjustTenureNotice: false);
                            var kept = future.Where(s => !toDelete.Contains(s));
                            if (current != null && !toDelete.Contains(current))
                                kept = kept.Append(current);
                            var protectedKept = kept.Where(LeaveScheduleUntouched).ToList();
                            toInsert.AddRange(ExpandOccurrencesAfterProtected(
                                    currentWeek.Schedules,
                                    protectedKept,
                                    today,
                                    request,
                                    effectiveType,
                                    occurrence,
                                    releaseBefore,
                                    dto.ReleaseBeforeDate,
                                    scheduledTime,
                                    dto.ReleaseHours,
                                    weekStartDay)
                                .Where(o =>
                                    !OverlapsAny(past, o)
                                    && !kept.Any(s => o.Overlaps(s.StartDate, s.EndDate))
                                    && !toInsert.Any(row => o.Overlaps(row.StartDate, row.EndDate))));
                        }
                    }
                }

                if (effectiveType == RequestTypes.Monthly && today >= request.StartDate && today <= effectiveEnd)
                {
                    AppendMonthlyScheduleGapFills(
                        toInsert,
                        past,
                        current,
                        future,
                        toDelete,
                        request,
                        occurrence,
                        releaseBefore,
                        dto,
                        scheduledTime,
                        today,
                        effectiveEnd,
                        weekStartDay);
                }

                var releaseHoursApplied = false;
                var releaseDatesCorrected = false;
                if (effectiveType is RequestTypes.Weekly or RequestTypes.Monthly)
                {
                    foreach (var row in current == null ? future : future.Prepend(current))
                    {
                        if (toDelete.Contains(row) || LeaveScheduleUntouched(row) || row.StartDate is not DateOnly start)
                            continue;
                        var releaseDate = ResolveReleaseBeforeDate(effectiveType, start, releaseBefore, dto.ReleaseBeforeDate, weekStartDay);
                        if (effectiveType == RequestTypes.Weekly)
                            releaseDate = BringReleaseDateToTodayOrLater(releaseDate, start, today);
                        row.ScheduleDate = releaseDate;
                        releaseDatesCorrected = true;
                        var isCurrentWindow = start <= today && row.EndDate >= today;
                        if (!isCurrentWindow || request.ReleaseAs != ReleaseAs.TenureBased)
                            continue;
                        if (releaseDate < today)
                        {
                            row.ReleaseAs = ReleaseAs.Open;
                            row.RequestStatus = RequestStatus.ReleaseNow;
                            row.PreserveOnRegenerate = true;
                        }
                        else if (row.ReleaseAs == ReleaseAs.Open && row.RequestStatus == RequestStatus.ReleaseNow)
                        {
                            row.ReleaseAs = ReleaseAs.TenureBased;
                            row.RequestStatus = RequestStatus.ReleasedOn;
                            row.PreserveOnRegenerate = false;
                        }
                    }
                }
                var keptRows = (current == null ? future : future.Prepend(current))
                    .Where(s => !toDelete.Contains(s));
                if (dto.ReleaseHours is int releaseHours && tenureNotice == null)
                {
                    foreach (var row in keptRows)
                    {
                        if (LeaveScheduleUntouched(row))
                            continue;
                        ApplyProcessFromReleaseHours(row, releaseHours);
                    }
                    releaseHoursApplied = true;
                }

                createdCount = toInsert.Count;
                if (toInsert.Count > 0)
                    return (await BulkUpsertAsync(toInsert, isSuperUser).ConfigureAwait(false)).First();

                if (scheduleChanged || releaseAsChanged || releaseHoursApplied || releaseDatesCorrected)
                    await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return _mapper.Map<RequestScheduleDto>(current ?? existing.FirstOrDefault());
            }).ConfigureAwait(false);

            if (saved == null)
                ThrowBadRequest(ModelValidationErrorCodes.REQUEST_SCHEDULE_GENERATE_NO_OCCURRENCES);

            if (createdCount > 0)
                await NotifyReleaseNowAsync(saved).ConfigureAwait(false);
            saved.GeneratedScheduleCount = createdCount;
            saved.GenerationWarning = tenureNotice;
            _logger.LogInformation(
                "Generated {Count} schedules for request {RequestId} ({RequestType}).",
                createdCount, dto.RequestId, effectiveType);
            return saved;
        }

        private static (bool TypeChanged, bool EndChanged, bool PatternChanged, bool TimeChanged) GetGenerateChanges(
            GenerateRequestScheduleDto dto,
            RequestScheduleGenerationDto? existing) => (
            existing != null && dto.RequestType.HasValue && dto.RequestType.Value != existing.RequestType,
            existing != null && dto.EndDate.HasValue && dto.EndDate.Value != existing.RequestEndDate,
            existing != null && (
                (dto.MonthlyMode.HasValue && dto.MonthlyMode != existing.ReleaseBeforeMonthlyMode)
                || (dto.WeekIndex.HasValue && dto.WeekIndex != existing.ReleaseBeforeWeekIndex)
                || (dto.DaysOfWeek != null && !AreSameDays(existing.ReleaseBeforeDaysOfWeek, dto.DaysOfWeek))
                || (dto.DaysOfMonth != null && !AreSameDays(existing.ReleaseBeforeDaysOfMonth, dto.DaysOfMonth))),
            existing != null && dto.ScheduledTime.HasValue && dto.ScheduledTime != existing.ScheduledTime);

        private static (RequestTypes Type, DateOnly End, TimeOnly ScheduledTime, RequestTypeDto ReleaseBefore, RequestTypeDto Occurrence) GetGenerateSettings(
            GenerateRequestScheduleDto dto,
            Request request,
            RequestTypes requestType,
            RequestScheduleGenerationDto? existing,
            bool typeChanged)
        {
            var effectiveType = typeChanged ? dto.RequestType!.Value : existing?.RequestType ?? requestType;
            if (effectiveType is RequestTypes.Daily or RequestTypes.OneTime)
                ThrowBadRequest(ModelValidationErrorCodes.REQUEST_SCHEDULE_GENERATE_DAILY_NOT_ALLOWED);

            return (
                effectiveType,
                dto.EndDate ?? existing?.RequestEndDate ?? request.EndDate,
                dto.ScheduledTime ?? existing?.ScheduledTime ?? TimeOnly.MinValue,
                new RequestTypeDto
                {
                    RequestType = effectiveType,
                    RepeatEvery = 1,
                    DaysOfWeek = dto.DaysOfWeek ?? existing?.ReleaseBeforeDaysOfWeek,
                    DaysOfMonth = dto.DaysOfMonth ?? existing?.ReleaseBeforeDaysOfMonth,
                    MonthlyMode = dto.MonthlyMode ?? existing?.ReleaseBeforeMonthlyMode,
                    WeekIndex = dto.WeekIndex ?? existing?.ReleaseBeforeWeekIndex
                },
                ToRequestTypeDto(effectiveType, request.RequestType));
        }

        private (List<RequestScheduleDto> ToInsert, List<RequestSchedule> ToDelete, string? TenureNotice) BuildRegeneration(
            Request request,
            GenerateRequestScheduleDto dto,
            RequestScheduleGenerationDto? existing,
            bool replaceFuture,
            bool endChanged,
            bool startChanged,
            bool patternChanged,
            bool timeChanged,
            bool releaseAsChanged,
            RequestTypes type,
            DateOnly end,
            TimeOnly scheduledTime,
            RequestTypeDto releaseBefore,
            RequestTypeDto occurrence,
            RequestSchedule? current,
            List<RequestSchedule> future,
            DayOfWeek weekStartDay)
        {
            if (replaceFuture)
            {
                var built = BuildGenerateOccurrences(
                    request, type, occurrence, releaseBefore, dto.ReleaseBeforeDate, scheduledTime, null, end, dto.ReleaseHours, weekStartDay);
                if (built.Schedules.Count == 0 && built.TenureNotice == null)
                    ThrowBadRequest(ModelValidationErrorCodes.REQUEST_SCHEDULE_GENERATE_NO_OCCURRENCES);
                var occurrences = built.Schedules;

                var toUpdate = new List<RequestSchedule>();
                if (current != null && !LeaveScheduleUntouched(current))
                    toUpdate.Add(current);

                var assignCount = Math.Min(toUpdate.Count, occurrences.Count);
                for (var i = 0; i < assignCount; i++)
                {
                    var previousReleaseAs = toUpdate[i].ReleaseAs;
                    var previousReleaseHours = toUpdate[i].ReleaseHours;
                    var previousProcessDate = toUpdate[i].ProcessDate;
                    var previousProcessTime = toUpdate[i].ProcessTime;
                    ApplyGeneratedWindow(toUpdate[i], occurrences[i]);
                    if (toUpdate[i] == current || LeaveScheduleUntouched(toUpdate[i]))
                    {
                        toUpdate[i].ReleaseAs = previousReleaseAs;
                        toUpdate[i].ReleaseHours = previousReleaseHours;
                        toUpdate[i].ProcessDate = previousProcessDate;
                        toUpdate[i].ProcessTime = previousProcessTime;
                    }
                }

                var protectedRows = future.Where(LeaveScheduleUntouched).ToList();
                if (current != null && LeaveScheduleUntouched(current))
                    protectedRows.Add(current);
                var toDelete = future.Where(s => !LeaveScheduleUntouched(s)).ToList();
                toDelete.AddRange(toUpdate.Skip(assignCount).Where(s => s != current));
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var inserts = ExpandOccurrencesAfterProtected(
                        occurrences.Skip(assignCount),
                        protectedRows,
                        today,
                        request,
                        type,
                        occurrence,
                        releaseBefore,
                        dto.ReleaseBeforeDate,
                        scheduledTime,
                        dto.ReleaseHours,
                        weekStartDay)
                    .ToList();
                return (inserts, toDelete, built.TenureNotice);
            }

            var insert = new List<RequestScheduleDto>();
            var delete = new List<RequestSchedule>();
            string? tenureNotice = null;
            if (patternChanged || timeChanged)
            {
                foreach (var row in current == null ? future : future.Prepend(current))
                {
                    if (LeaveScheduleUntouched(row))
                        continue;
                    ApplyReleaseChange(row, patternChanged, timeChanged, type, releaseBefore, dto.ReleaseBeforeDate, scheduledTime, weekStartDay);
                }
            }

            if (releaseAsChanged)
            {
                foreach (var row in future.Where(s => !LeaveScheduleUntouched(s)))
                    row.ReleaseAs = request.ReleaseAs;
            }

            if (endChanged && dto.EndDate!.Value > existing!.RequestEndDate)
            {
                var kept = current == null ? future : future.Append(current);
                var extended = BuildGenerateOccurrences(
                        request, type, occurrence, releaseBefore, dto.ReleaseBeforeDate, scheduledTime,
                        existing.RequestEndDate.AddDays(1), dto.EndDate.Value, dto.ReleaseHours, weekStartDay);
                tenureNotice = extended.TenureNotice ?? tenureNotice;
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var protectedKept = kept.Where(LeaveScheduleUntouched).ToList();
                insert = ExpandOccurrencesAfterProtected(
                        extended.Schedules,
                        protectedKept,
                        today,
                        request,
                        type,
                        occurrence,
                        releaseBefore,
                        dto.ReleaseBeforeDate,
                        scheduledTime,
                        dto.ReleaseHours,
                        weekStartDay)
                    .Where(o => !OverlapsAny(kept, o))
                    .ToList();
            }
            else if (endChanged && dto.EndDate!.Value < existing!.RequestEndDate)
            {
                var newEnd = dto.EndDate.Value;
                delete = future.Where(s => s.StartDate > newEnd && !LeaveScheduleUntouched(s)).ToList();
                foreach (var row in future.Where(s => !delete.Contains(s) && !LeaveScheduleUntouched(s) && CrossesEnd(s, newEnd)))
                    row.EndDate = newEnd;
                if (current != null && !LeaveScheduleUntouched(current) && CrossesEnd(current, newEnd))
                    current.EndDate = newEnd;
            }

            // Start moved earlier. A future start fills from that date. A past start fills from today.
            if (startChanged && existing != null)
            {
                var newFrom = ResolveGenerateRangeStart(request.StartDate);
                var oldFrom = existing.EffectiveFromDate;
                if (newFrom < oldFrom)
                {
                    var kept = current == null ? future : future.Append(current);
                    var added = BuildGenerateOccurrences(
                            request, type, occurrence, releaseBefore, dto.ReleaseBeforeDate, scheduledTime,
                            newFrom, oldFrom.AddDays(-1), dto.ReleaseHours, weekStartDay);
                    tenureNotice = added.TenureNotice ?? tenureNotice;
                    var today = DateOnly.FromDateTime(DateTime.UtcNow);
                    var overlapBlocking = type == RequestTypes.Monthly
                        ? kept.ToList()
                        : kept.Where(LeaveScheduleUntouched).ToList();
                    insert.AddRange(ExpandOccurrencesAfterProtected(
                            added.Schedules,
                            overlapBlocking,
                            today,
                            request,
                            type,
                            occurrence,
                            releaseBefore,
                            dto.ReleaseBeforeDate,
                            scheduledTime,
                            dto.ReleaseHours,
                            weekStartDay)
                        .Where(o =>
                            (type == RequestTypes.Monthly || !OverlapsAny(kept, o))
                            && !insert.Any(row => o.Overlaps(row.StartDate, row.EndDate))));
                }
            }

            return (insert, delete, tenureNotice);
        }

        /// <summary>
        /// Monthly: fill from today (or day after current row) through month-end, stopping before the next schedule.
        /// </summary>
        private void AppendMonthlyScheduleGapFills(
            List<RequestScheduleDto> toInsert,
            List<RequestSchedule> past,
            RequestSchedule? current,
            List<RequestSchedule> future,
            List<RequestSchedule> toDelete,
            Request request,
            RequestTypeDto occurrence,
            RequestTypeDto releaseBefore,
            GenerateRequestScheduleDto dto,
            TimeOnly scheduledTime,
            DateOnly today,
            DateOnly effectiveEnd,
            DayOfWeek weekStartDay)
        {
            bool CoversToday(DateOnly? start, DateOnly? end) =>
                start is DateOnly s && end is DateOnly e && s <= today && e >= today;

            var kept = future.Where(s => !toDelete.Contains(s)).ToList();
            if (current is { } cur && !toDelete.Contains(cur))
                kept.Insert(0, cur);

            DateOnly? through = kept
                .OrderByDescending(r => r.RequestStatus == RequestStatus.ReleaseNow)
                .FirstOrDefault(r => CoversToday(r.StartDate, r.EndDate))
                ?.EndDate;
            foreach (var row in toInsert.Where(r => CoversToday(r.StartDate, r.EndDate)))
            {
                if (row.EndDate is DateOnly end && (through == null || end > through))
                    through = end;
            }

            var gapStart = through?.AddDays(1) ?? today;
            var gapEnd = ResolveOccurrenceEndDate(RequestTypes.Monthly, today, weekStartDay);
            if (gapEnd > effectiveEnd)
                gapEnd = effectiveEnd;

            var after = through ?? today.AddDays(-1);
            var nextStart = kept.Select(r => r.StartDate)
                .Concat(toInsert.Select(r => r.StartDate))
                .Where(d => d is DateOnly s && s > after)
                .Select(d => d!.Value)
                .OrderBy(d => d)
                .FirstOrDefault();
            if (nextStart != default && nextStart.AddDays(-1) < gapEnd)
                gapEnd = nextStart.AddDays(-1);
            if (gapStart > gapEnd)
                return;

            foreach (var row in BuildGenerateOccurrences(
                         request, RequestTypes.Monthly, occurrence, releaseBefore, dto.ReleaseBeforeDate,
                         scheduledTime, gapStart, gapEnd, dto.ReleaseHours, weekStartDay, adjustTenureNotice: false)
                     .Schedules)
            {
                if (OverlapsAny(past, row) || OverlapsAny(kept, row)
                    || toInsert.Any(x => x.Overlaps(row.StartDate, row.EndDate)))
                    continue;
                toInsert.Add(row);
            }
        }

        /// <summary>
        /// Schedules flagged to preserve stay as saved during regenerate and request changes.
        /// </summary>
        private static bool LeaveScheduleUntouched(RequestSchedule row) =>
            row.PreserveOnRegenerate;

        /// <summary>
        /// When a generated window overlaps preserved rows, insert the tail after them (from today onward) instead of dropping the whole window.
        /// </summary>
        private IEnumerable<RequestScheduleDto> ExpandOccurrencesAfterProtected(
            IEnumerable<RequestScheduleDto> occurrences,
            IReadOnlyList<RequestSchedule> protectedRows,
            DateOnly today,
            Request request,
            RequestTypes type,
            RequestTypeDto occurrenceType,
            RequestTypeDto releaseBeforeType,
            DateOnly? releaseBeforeOverride,
            TimeOnly scheduledTime,
            int? releaseHours,
            DayOfWeek weekStartDay)
        {
            foreach (var occurrence in occurrences)
            {
                foreach (var row in TailOccurrenceAfterProtectedRows(
                             occurrence,
                             protectedRows,
                             today,
                             request,
                             type,
                             occurrenceType,
                             releaseBeforeType,
                             releaseBeforeOverride,
                             scheduledTime,
                             releaseHours,
                             weekStartDay))
                    yield return row;
            }
        }

        private List<RequestScheduleDto> TailOccurrenceAfterProtectedRows(
            RequestScheduleDto occurrence,
            IReadOnlyList<RequestSchedule> protectedRows,
            DateOnly today,
            Request request,
            RequestTypes type,
            RequestTypeDto occurrenceType,
            RequestTypeDto releaseBeforeType,
            DateOnly? releaseBeforeOverride,
            TimeOnly scheduledTime,
            int? releaseHours,
            DayOfWeek weekStartDay)
        {
            if (occurrence.StartDate is not DateOnly occStart || occurrence.EndDate is not DateOnly occEnd)
                return [occurrence];

            var blocking = protectedRows
                .Where(p => occurrence.Overlaps(p.StartDate, p.EndDate) && p.EndDate is DateOnly end && end >= today)
                .ToList();
            if (blocking.Count == 0)
                return [occurrence];

            var tailStart = occStart;
            if (today > tailStart)
                tailStart = today;
            var afterPreserved = blocking.Max(p => p.EndDate!.Value).AddDays(1);
            if (afterPreserved > tailStart)
                tailStart = afterPreserved;

            if (tailStart > occEnd)
                return [];

            if (tailStart == occStart)
                return [occurrence];

            var built = BuildGenerateOccurrences(
                request,
                type,
                occurrenceType,
                releaseBeforeType,
                releaseBeforeOverride,
                scheduledTime,
                tailStart,
                occEnd,
                releaseHours,
                weekStartDay,
                adjustTenureNotice: false);
            return built.Schedules;
        }

        private static bool CrossesEnd(RequestSchedule row, DateOnly newEnd) =>
            row.EndDate > newEnd && (row.StartDate == null || row.StartDate <= newEnd);

        private static bool AreSameDays(List<int>? left, List<int>? right)
        {
            var a = left ?? [];
            var b = right ?? [];
            return a.Count == b.Count && a.SequenceEqual(b);
        }

        private static void ApplyGeneratedWindow(RequestSchedule row, RequestScheduleDto occurrence)
        {
            row.StartDate = occurrence.StartDate;
            row.EndDate = occurrence.EndDate;
            row.ReleaseAs = occurrence.ReleaseAs;
            row.ScheduleDate = occurrence.ScheduleDate;
            row.ScheduledTime = occurrence.ScheduledTime;
            row.ReleaseHours = occurrence.ReleaseHours;
            row.ProcessDate = occurrence.ProcessDate;
            row.ProcessTime = occurrence.ProcessTime;
        }

        private static void ApplyProcessFromReleaseHours(RequestSchedule row, int releaseHours)
        {
            row.ReleaseHours = releaseHours;
            if (row.ScheduleDate is not DateOnly scheduleDate || row.ScheduledTime is not TimeOnly scheduledTime)
                return;

            var processAt = scheduleDate.ToDateTime(scheduledTime).AddHours(releaseHours);
            row.ProcessDate = DateOnly.FromDateTime(processAt);
            row.ProcessTime = TimeOnly.FromDateTime(processAt);
        }

        private static void ApplyReleaseChange(
            RequestSchedule row,
            bool dateChanged,
            bool timeChanged,
            RequestTypes type,
            RequestTypeDto releaseBeforeType,
            DateOnly? releaseBeforeOverride,
            TimeOnly scheduledTime,
            DayOfWeek weekStartDay)
        {
            if (dateChanged && row.StartDate is DateOnly start)
                row.ScheduleDate = ResolveReleaseBeforeDate(type, start, releaseBeforeType, releaseBeforeOverride, weekStartDay);
            if (timeChanged)
                row.ScheduledTime = scheduledTime;
        }

        private static bool OverlapsAny(IEnumerable<RequestSchedule> rows, RequestScheduleDto occurrence) =>
            rows.Any(s => occurrence.Overlaps(s.StartDate, s.EndDate));

        private static void ThrowBadRequest(string errorCode, params object[] args) =>
            throw new WebApiException(
                HttpStatusCode.BadRequest,
                ((int)HttpStatusCode.BadRequest).ToString(),
                ModelValidationErrorCodes.GetErrorMessage(errorCode, args));

        private static RequestTypeDto ToRequestTypeDto(RequestTypes type, RequestType? requestType) => new()
        {
            RequestType = type,
            RepeatEvery = requestType?.RepeatEvery > 0 ? requestType.RepeatEvery : 1,
            DaysOfWeek = requestType?.DaysOfWeek,
            DaysOfMonth = requestType?.DaysOfMonth,
            MonthlyMode = requestType?.MonthlyMode,
            WeekIndex = requestType?.WeekIndex
        };

        private static void RequireGenerateDays(RequestTypes type, RequestTypeDto dto)
        {
            var ok = type switch
            {
                RequestTypes.Weekly => dto.DaysOfWeek is { Count: > 0 },
                RequestTypes.Monthly when (dto.MonthlyMode ?? MonthlyMode.ByDate) == MonthlyMode.ByDate
                    => dto.DaysOfMonth is { Count: > 0 },
                RequestTypes.Monthly => dto.DaysOfWeek is { Count: > 0 } && dto.WeekIndex is >= 0,
                _ => true
            };
            if (!ok)
                ThrowBadRequest(ModelValidationErrorCodes.REQUEST_SCHEDULE_GENERATE_DAYS_REQUIRED);
        }

        /// <summary>
        /// Generate range start: request StartDate, or today when StartDate is in the past.
        /// </summary>
        private static DateOnly ResolveGenerateRangeStart(DateOnly requestStartDate)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return requestStartDate < today ? today : requestStartDate;
        }

        private readonly record struct GeneratedOccurrenceResult(List<RequestScheduleDto> Schedules, string? TenureNotice);

        private GeneratedOccurrenceResult BuildGenerateOccurrences(
            Request request,
            RequestTypes type,
            RequestTypeDto occurrenceType,
            RequestTypeDto releaseBeforeType,
            DateOnly? releaseBeforeOverride,
            TimeOnly scheduledTime,
            DateOnly? rangeStartOverride = null,
            DateOnly? rangeEndOverride = null,
            int? releaseHours = null,
            DayOfWeek weekStartDay = DayOfWeek.Sunday,
            bool adjustTenureNotice = true)
        {
            var rangeEnd = rangeEndOverride ?? request.EndDate;
            var rangeStart = rangeStartOverride ?? ResolveGenerateRangeStart(request.StartDate);
            if (rangeStart > rangeEnd)
                return new([], null);

            RequestScheduleDto CreateScheduleWindow(DateOnly start, DateOnly end, RequestTypes releaseType)
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var releaseDate = ResolveReleaseBeforeDate(
                    releaseType, start, releaseBeforeType, releaseBeforeOverride, weekStartDay);
                if (releaseType == RequestTypes.Weekly)
                    releaseDate = BringReleaseDateToTodayOrLater(releaseDate, start, today);
                var isCurrentWindow = start <= today && end >= today;
                var row = RequestScheduleDto.CreateGenerated(
                    request.RequestID, start, end, request.ReleaseAs, releaseDate, scheduledTime, releaseHours);
                if (isCurrentWindow
                    && request.ReleaseAs == ReleaseAs.TenureBased
                    && releaseType is RequestTypes.Weekly or RequestTypes.Monthly
                    && releaseDate < today)
                {
                    row.ReleaseAs = ReleaseAs.Open;
                    row.RequestStatus = RequestStatus.ReleaseNow;
                    row.PreserveOnRegenerate = true;
                }
                return row;
            }

            // Custom with no weekday or day-of-month pattern is one window for the whole range.
            if (type == RequestTypes.Custom
                && occurrenceType.DaysOfWeek is not { Count: > 0 }
                && occurrenceType.DaysOfMonth is not { Count: > 0 })
                return new([CreateScheduleWindow(rangeStart, rangeEnd, type)], null);

            var collectType = type == RequestTypes.Custom
                ? (occurrenceType.DaysOfMonth is { Count: > 0 } || occurrenceType.MonthlyMode == MonthlyMode.ByPattern
                    ? RequestTypes.Monthly
                    : RequestTypes.Weekly)
                : type;
            releaseBeforeType.RequestType = collectType;

            // Weekly windows end the day before the department week starts again.
            // Monthly windows run through the last day of the month.
            var interval = occurrenceType.RepeatEvery > 0 ? occurrenceType.RepeatEvery : 1;
            var schedules = new List<RequestScheduleDto>();
            for (var start = rangeStart; start <= rangeEnd;)
            {
                var naturalEnd = ResolveOccurrenceEndDate(collectType, start, weekStartDay);
                var end = naturalEnd > rangeEnd ? rangeEnd : naturalEnd;
                if (end >= start)
                    schedules.Add(CreateScheduleWindow(start, end, collectType));

                start = collectType == RequestTypes.Weekly
                    ? naturalEnd.AddDays(1 + 7 * (interval - 1))
                    : naturalEnd.AddDays(1).AddMonths(interval - 1);
            }

            if (adjustTenureNotice
                && request.ReleaseAs == ReleaseAs.TenureBased
                && releaseHours is int hours
                && type == RequestTypes.Weekly
                && collectType == RequestTypes.Weekly)
            {
                return ApplyTenureNotice(
                    schedules, request, type, occurrenceType, releaseBeforeType, releaseBeforeOverride,
                    scheduledTime, hours, rangeStart, rangeEnd, weekStartDay);
            }

            return new(schedules, null);
        }

        /// <summary>
        /// Tenure only. Short-window lead-in may go out as Open + Release now; tenure rows follow from the day after.
        /// </summary>
        private GeneratedOccurrenceResult ApplyTenureNotice(
            List<RequestScheduleDto> schedules,
            Request request,
            RequestTypes type,
            RequestTypeDto occurrenceType,
            RequestTypeDto releaseBeforeType,
            DateOnly? releaseBeforeOverride,
            TimeOnly scheduledTime,
            int releaseHours,
            DateOnly rangeStart,
            DateOnly rangeEnd,
            DayOfWeek weekStartDay)
        {
            if (!TryGetTenureShortWindow(
                    request.StartDate, rangeStart, rangeEnd, occurrenceType, releaseBeforeType, releaseBeforeOverride,
                    scheduledTime, releaseHours, weekStartDay, out var openStart, out var openEnd, out var offerDate))
            {
                return new(schedules, null);
            }

            var open = RequestScheduleDto.CreateGenerated(
                request.RequestID, openStart, openEnd, ReleaseAs.Open, offerDate, scheduledTime);
            open.RequestStatus = RequestStatus.ReleaseNow;
            open.ScheduleDate = openStart;
            open.PreserveOnRegenerate = true;

            var tenureFrom = openEnd.AddDays(1);
            if (tenureFrom < rangeStart)
                tenureFrom = rangeStart;
            if (tenureFrom > rangeEnd)
                return new([open], null);

            var tenure = BuildGenerateOccurrences(
                request, type, occurrenceType, releaseBeforeType, releaseBeforeOverride, scheduledTime,
                tenureFrom, rangeEnd, releaseHours, weekStartDay, adjustTenureNotice: false);
            var rows = new List<RequestScheduleDto> { open };
            rows.AddRange(tenure.Schedules);
            return new(rows, null);
        }

        private bool TryGetTenureShortWindow(
            DateOnly requestStart,
            DateOnly rangeStart,
            DateOnly rangeEnd,
            RequestTypeDto occurrenceType,
            RequestTypeDto releaseBeforeType,
            DateOnly? releaseBeforeOverride,
            TimeOnly scheduledTime,
            int releaseHours,
            DayOfWeek weekStartDay,
            out DateOnly openStart,
            out DateOnly openEnd,
            out DateOnly offerDate)
        {
            openStart = default;
            openEnd = default;
            offerDate = default;
            var workDays = TenureWorkDays(occurrenceType, weekStartDay);
            DateOnly? firstWork = null;
            for (var day = requestStart; day <= rangeEnd; day = day.AddDays(1))
            {
                if (!workDays.Contains(day.DayOfWeek))
                    continue;
                firstWork = day;
                break;
            }

            if (firstWork is not DateOnly workDay)
                return false;

            var weekStart = workDay.GetDepartmentWeekRange(weekStartDay).WeekStart;
            var offer = ResolveReleaseBeforeDate(
                RequestTypes.Weekly, weekStart, releaseBeforeType, releaseBeforeOverride, weekStartDay);
            var effectiveOffer = offer < requestStart ? requestStart : offer;
            var rawProcess = DateOnly.FromDateTime(offer.ToDateTime(scheduledTime).AddHours(releaseHours));
            var effectiveProcess = DateOnly.FromDateTime(effectiveOffer.ToDateTime(scheduledTime).AddHours(releaseHours));
            var processDate = rawProcess >= workDay ? rawProcess : effectiveProcess;
            if (processDate < workDay)
                return false;

            var bufferEnd = processDate.AddDays(1);
            if (requestStart < offer || requestStart > bufferEnd)
                return false;

            if (bufferEnd > rangeEnd)
                bufferEnd = rangeEnd;
            if (bufferEnd < rangeStart || bufferEnd < requestStart)
                return false;

            openStart = requestStart;
            openEnd = bufferEnd;
            offerDate = effectiveOffer;
            return true;
        }

        private List<DayOfWeek> TenureWorkDays(RequestTypeDto occurrenceType, DayOfWeek weekStartDay)
        {
            if (occurrenceType.DaysOfWeek is { Count: > 0 } days)
            {
                var parsed = days
                    .Where(day => Enum.IsDefined(typeof(DayOfWeek), day))
                    .Select(day => (DayOfWeek)day)
                    .Distinct()
                    .ToList();
                if (parsed.Count > 0)
                    return parsed;
            }

            return [weekStartDay];
        }

        private static DateOnly ResolveOccurrenceEndDate(RequestTypes type, DateOnly start, DayOfWeek weekStartDay) => type switch
        {
            RequestTypes.Weekly => start.GetDepartmentWeekRange(weekStartDay).WeekEnd,
            RequestTypes.Monthly => new DateOnly(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month)),
            _ => start
        };

        /// <summary>
        /// A release weekday that is already past moves forward one week at a time,
        /// while that day is still before the work window starts.
        /// Monday 10/05 for a week that starts 10/14 becomes Monday 10/12.
        /// </summary>
        private static DateOnly BringReleaseDateToTodayOrLater(DateOnly releaseDate, DateOnly occurrenceStart, DateOnly today)
        {
            while (releaseDate < today)
            {
                var next = releaseDate.AddDays(7);
                if (next >= occurrenceStart)
                    break;
                releaseDate = next;
            }
            return releaseDate;
        }

        // Weekly: selected weekday of the previous department week. Monthly: same rule one month earlier.
        private static DateOnly ResolveReleaseBeforeDate(
            RequestTypes type,
            DateOnly occurrenceStart,
            RequestTypeDto? typeDto,
            DateOnly? releaseBeforeOverride,
            DayOfWeek weekStartDay)
        {
            if (releaseBeforeOverride is DateOnly explicitDate)
                return explicitDate;

            if (type == RequestTypes.Weekly)
            {
                // Release Before is the selected weekday before the schedule starts.
                // Wednesday 10/14 → Monday 10/12. Sunday 10/18 → Thursday 10/15, not Thursday 10/22.
                var weekStart = occurrenceStart.GetDepartmentWeekRange(DayOfWeek.Sunday).WeekStart;
                if (typeDto?.DaysOfWeek is { Count: > 0 } days
                    && Enum.IsDefined(typeof(DayOfWeek), days[0]))
                {
                    var release = weekStart.AddDays(days[0]);
                    return release >= occurrenceStart ? release.AddDays(-7) : release;
                }
                return weekStart;
            }

            if (type == RequestTypes.Monthly)
                return ResolveMonthlyReleaseBeforeDate(occurrenceStart, typeDto);

            return occurrenceStart;
        }

        private static DateOnly ResolveMonthlyReleaseBeforeDate(DateOnly occurrenceStart, RequestTypeDto? typeDto)
        {
            var previous = occurrenceStart.AddMonths(-1);

            if ((typeDto?.MonthlyMode ?? MonthlyMode.ByDate) == MonthlyMode.ByPattern
                && typeDto?.WeekIndex is int weekIndex and >= 0
                && typeDto.DaysOfWeek is { Count: > 0 } days
                && Enum.IsDefined(typeof(DayOfWeek), days[0]))
            {
                // Every week (0) releases on the last matching weekday of the previous month.
                var patterned = GetNthWeekdayOfMonth(
                    previous.Year, previous.Month, weekIndex == 0 ? 5 : weekIndex, (DayOfWeek)days[0]);
                if (patterned.HasValue)
                    return patterned.Value;
            }

            var monthDays = typeDto?.DaysOfMonth;
            var configuredDay = monthDays is { Count: > 0 } && monthDays[0] >= 1
                ? monthDays[0]
                : occurrenceStart.Day;
            return RequestDto.GetScheduleDayWithinMonth(previous.Year, previous.Month, configuredDay);
        }

        private static DateOnly? GetNthWeekdayOfMonth(int year, int month, int n, DayOfWeek dayOfWeek)
        {
            if (n is < 1 or > 5)
                return null;

            var daysInMonth = DateTime.DaysInMonth(year, month);
            var first = new DateOnly(year, month, 1);
            var firstMatch = 1 + (((int)dayOfWeek - (int)first.DayOfWeek + 7) % 7);

            if (n < 5)
            {
                var day = firstMatch + (n - 1) * 7;
                return day > daysInMonth ? null : new DateOnly(year, month, day);
            }

            var last = new DateOnly(year, month, daysInMonth);
            return last.AddDays(-(((int)last.DayOfWeek - (int)dayOfWeek + 7) % 7));
        }

        private async Task NotifyReleaseNowAsync(RequestScheduleDto result)
        {
            if (result.RequestStatus == RequestStatus.ReleaseNow && result.RequestId != Guid.Empty)
                await _postService.NotifyJobsPostedAsync(result.RequestId).ConfigureAwait(false);
        }
    }
}
