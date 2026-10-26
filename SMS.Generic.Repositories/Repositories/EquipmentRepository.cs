using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Equipment-specific repository implementation.
    /// </summary>
    public class EquipmentRepository : GenericRepository<Equipment, SMSDbContext>, IEquipmentRepository
    {
        private readonly SMSDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EquipmentRepository(
            SMSDbContext context,
            ILogger<EquipmentRepository> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}

