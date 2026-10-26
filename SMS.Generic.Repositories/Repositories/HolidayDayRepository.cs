using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository implementation for HolidayDay operations.
    /// </summary>
    public class HolidayDayRepository : GenericRepository<HolidayDay, SMSDbContext>, IHolidayDayRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HolidayDayRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger.</param>
        public HolidayDayRepository(
            SMSDbContext context,
            ILogger<HolidayDayRepository> logger)
            : base(context, logger)
        { }
    }
}
