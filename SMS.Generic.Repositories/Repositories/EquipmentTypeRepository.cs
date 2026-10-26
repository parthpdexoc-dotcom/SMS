using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Equipment type-specific repository implementation.
    /// </summary>
    public class EquipmentTypeRepository : GenericRepository<EquipmentType, SMSDbContext>, IEquipmentTypeRepository
    {
        private readonly SMSDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentTypeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EquipmentTypeRepository(
            SMSDbContext context,
            ILogger<EquipmentTypeRepository> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}


