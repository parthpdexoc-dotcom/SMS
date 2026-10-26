using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Common.Enums;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Tenure waiting position-specific repository implementation.
    /// </summary>
    public class WaitingPostPositionRepository : GenericRepository<WaitingPostPosition, SMSDbContext>, IWaitingPostPositionRepository
    {
        private readonly ILogger<WaitingPostPositionRepository> _logger;
        private readonly SMSDbContext _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitingPostPositionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public WaitingPostPositionRepository(
            SMSDbContext context,
            ILogger<WaitingPostPositionRepository> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<List<WaitingPostPosition>> GetForLoggedInEmployeeListingAsync(Guid employeeId)
        {
            return await _context.WaitingPostPosition
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Where(wp =>
                        wp.EmployeeID == employeeId &&
                        (wp.PostPositionStatus == PostPositionStatus.Waiting ||
                         wp.PostPositionStatus == PostPositionStatus.SpecialEvent))
                    .Include(wp => wp.Post)
                        .ThenInclude(p => p.Request)
                            .ThenInclude(r => r.RequestSchedules)
                    .Include(wp => wp.Request)
                        .ThenInclude(r => r.Posts)
                    .Include(wp => wp.Request)
                        .ThenInclude(r => r.RequestSchedules)
                    .ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<WaitingPostPosition>> GetWaitingPostPositionsByEmployeeAndDateRangeAsync(Guid employeeId,DateOnly startDate,DateOnly endDate,Guid? departmentId = null)
        {
            try
            {
                var query = _context.WaitingPostPosition
                    .Where(wp => wp.EmployeeID == employeeId &&
                                 wp.DateOfWork >= startDate &&
                                 wp.DateOfWork <= endDate  );

                if (departmentId.HasValue && departmentId.Value != Guid.Empty)
                {
                    query = query.Where(wp =>
                        (wp.Post != null &&
                         wp.Post.Request != null &&
                         wp.Post.Request.DepartmentID == departmentId.Value) ||
                        (wp.Request != null &&
                         wp.Request.DepartmentID == departmentId.Value));
                }
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve waiting post positions for DepartmentId {DepartmentId}", departmentId);
                return new List<WaitingPostPosition>();
            }
        }
    }
}

