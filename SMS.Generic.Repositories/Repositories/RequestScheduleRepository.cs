using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// RequestSchedule-specific repository implementation.
    /// </summary>
    public class RequestScheduleRepository : GenericRepository<RequestSchedule, SMSDbContext>, IRequestScheduleRepository
    {
        private readonly SMSDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestScheduleRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RequestScheduleRepository(
            SMSDbContext context,
            ILogger<RequestScheduleRepository> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RequestScheduleBulkUpdateResult> UpdateReleasedOnToReleasedAsync(DateTime asOfUtc)
        {
            var asOfDate = DateOnly.FromDateTime(asOfUtc);
            var asOfTime = TimeOnly.FromDateTime(asOfUtc);
            _logger.LogInformation(
                   "Started UpdateReleasedOnToReleased at {AsOfUtc}. Checking schedules before {AsOfDate} {AsOfTime}",
                   asOfUtc,
                   asOfDate,
                   asOfTime);             var rows = await _context.RequestSchedule
                .AsSplitQuery()
                .Include(s => s.Request)
                .Where(s => (int)s.RequestStatus == (int)RequestStatus.ReleasedOn
                    && s.ScheduleDate.HasValue
                    && s.ScheduledTime.HasValue
                    && (s.ScheduleDate.Value < asOfDate
                        || (s.ScheduleDate.Value == asOfDate && s.ScheduledTime.Value < asOfTime)))
                .ToListAsync()
                .ConfigureAwait(false);

            if (rows.Count == 0)
            {
                _logger.LogInformation("No ReleasedOn schedules found for release at {AsOfUtc}",asOfUtc);

                return new RequestScheduleBulkUpdateResult
                {
                    RowCount = 0,
                    AffectedRequests = Array.Empty<RequestScheduleAffectedRequest>(),
                };
            }
            foreach (var s in rows)
            {
                s.RequestStatus = RequestStatus.ReleaseNow;
                // Match auto-generate / UI: Open + Release Now rows must survive the next regenerate.
                if (s.ReleaseAs == ReleaseAs.Open)
                    s.PreserveOnRegenerate = true;
            }
            _logger.LogInformation("Found {Count} ReleasedOn schedules ready to move to ReleaseNow",rows.Count);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            _logger.LogInformation("Completed UpdateReleasedOnToReleased. Updated {Count} schedules.",rows.Count);
            return new RequestScheduleBulkUpdateResult
            {
                RowCount = rows.Count,
                AffectedRequests = DistinctAffectedRequests(rows),
            };
        }

        public async Task<RequestScheduleBulkUpdateResult> UpdateReleasedToCompleteAsync(DateTime asOfUtc)
        {
            var asOfDate = DateOnly.FromDateTime(asOfUtc);
            _logger.LogInformation("Started UpdateReleasedToComplete at {AsOfUtc}. Checking EndDate before {AsOfDate}",asOfUtc,asOfDate);
            var rows = await _context.RequestSchedule
                .AsSplitQuery()
                .Include(s => s.Request)
                .Where(s => (int)s.RequestStatus == (int)RequestStatus.ReleaseNow
                    && s.EndDate.HasValue
                    && s.EndDate.Value < asOfDate)
                .ToListAsync()
                .ConfigureAwait(false);

            if (rows.Count == 0)
            {
                _logger.LogInformation("No ReleaseNow schedules found for completion at {AsOfUtc}",asOfUtc);
                return new RequestScheduleBulkUpdateResult
                {
                    RowCount = 0,
                    AffectedRequests = Array.Empty<RequestScheduleAffectedRequest>(),
                };
            }

            foreach (var s in rows)
                s.RequestStatus = RequestStatus.Completed;
            _logger.LogInformation("Found {Count} ReleaseNow schedules ready to move to Completed",rows.Count);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            _logger.LogInformation("Completed UpdateReleasedToComplete. Updated {Count} schedules.",rows.Count);
            return new RequestScheduleBulkUpdateResult
            {
                RowCount = rows.Count,
                AffectedRequests = DistinctAffectedRequests(rows),
            };
        }

        private static IReadOnlyList<RequestScheduleAffectedRequest> DistinctAffectedRequests(
            IEnumerable<RequestSchedule> rows) =>
            rows
                .GroupBy(s => s.RequestId)
                .Select(g =>
                {
                    var first = g.First();
                    var title = first.Request?.Title?.Trim();
                    if (string.IsNullOrEmpty(title))
                        title = g.Key.ToString();
                    return new RequestScheduleAffectedRequest
                    {
                        RequestId = g.Key,
                        RequestTitle = title,
                        DepartmentId = first.Request?.DepartmentID,
                    };
                })
                .ToList();
    }
}

