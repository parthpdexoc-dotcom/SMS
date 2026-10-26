using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Requester restricted list-specific repository implementation.
    /// </summary>
    public class RequesterRestrictedListRepository : GenericRepository<RequesterRestrictedList, SMSDbContext>, IRequesterRestrictedListRepository
    {
        private readonly SMSDbContext _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterRestrictedListRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RequesterRestrictedListRepository(
            SMSDbContext context,
            ILogger<RequesterRestrictedListRepository> logger) : base(context, logger)
        {
            _context = context;
        }
        public async Task<List<RequesterRestrictedList>> GetRequesterIdsByEmployeeId(Guid employeeId,DateOnly startDate,DateOnly endDate)
        {
            return await _context.RequesterRestrictedList
                .Where(rr =>
                    rr.EmployeeID == employeeId &&
                    rr.EffectiveStartDate <= endDate &&
                    rr.EffectiveEndDate >= startDate)
                .ToListAsync();
        }
    }
}

