using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Post-specific repository implementation.
    /// </summary>
    public class PostRepository : GenericRepository<Post, SMSDbContext>, IPostRepository
    {
        private readonly ILogger<PostRepository> _logger;
        private readonly SMSDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public PostRepository(
            SMSDbContext context,
            ILogger<PostRepository> logger) : base(context, logger)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<Post?> GetPostWithRequestAndPositionsAsync(
            Guid postId, 
            DateOnly? dateOfWork = null)
        {
            var post = await _context.Post
                .AsSplitQuery()
                .Include(p => p.Request)
                    .ThenInclude(r => r.RequestType)
                .Include(p => p.PostPositions
                    .Where(pp => 
                        (!dateOfWork.HasValue || pp.DateOfWork == dateOfWork.Value)&&
                        (pp.PostPositionStatus != PostPositionStatus.Cancelled)))
                    .ThenInclude(pp => pp.PostCallSign)
                .Where(p => p.PostID == postId)
                .FirstOrDefaultAsync();

            return post;
        }

        /// <inheritdoc />
        public async Task<List<Post>> GetPostsByRequestIdAsync(Guid requestId)
        {
            return await _context.Post
                .AsNoTracking()
                .Where(p => p.RequestID == requestId)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Post?> GetPostWithPositionsByPostIdAsync(
            Guid postId,
            Guid departmentId,
            DateTime shiftWindowStart,
            DateTime shiftWindowEnd,
            CancellationToken cancellationToken = default)
        {
            var startDt = shiftWindowStart;
            var endDt = shiftWindowEnd;
            bool isSameDate = startDt.Date == endDt.Date;
            var targetDate = DateOnly.FromDateTime(startDt);
            var startDatePart = targetDate;
            var endDatePart = DateOnly.FromDateTime(endDt);

            IQueryable<Post> query = _context.Post
                .AsNoTracking()
                .Where(p => p.PostID == postId);

            if (departmentId != Guid.Empty)
            {
                query = query.Where(p => p.Request != null && p.Request.DepartmentID == departmentId);
            }

            var queryWithIncludes = isSameDate
                ? query.Include(p => p.PostPositions.Where(pp =>
                            pp.PostPositionStatus != PostPositionStatus.Cancelled &&
                            pp.DateOfWork == targetDate))
                            .ThenInclude(pp => pp.Employee)
                                .ThenInclude(e => e!.EmployeeType)
                : query.Include(p => p.PostPositions.Where(pp =>
                            pp.PostPositionStatus != PostPositionStatus.Cancelled &&
                            (pp.DateOfWork == startDatePart || pp.DateOfWork == endDatePart)))
                            .ThenInclude(pp => pp.Employee)
                                .ThenInclude(e => e!.EmployeeType);

            return await queryWithIncludes
                .AsSplitQuery()
                .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}

