using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Admin type-specific repository implementation.
    /// </summary>
    public class AdminTypeRepository : GenericRepository<AdminType, SMSDbContext>, IAdminTypeRepository
    {
        private readonly SMSDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminTypeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public AdminTypeRepository(
            SMSDbContext context,
            ILogger<AdminTypeRepository> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}

