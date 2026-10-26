using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Department divisions-specific repository implementation.
    /// </summary>
    public class DepartmentDivisionsRepository : GenericRepository<DepartmentDivisions, SMSDbContext>, IDepartmentDivisionsRepository
    {
        private readonly SMSDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentDivisionsRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public DepartmentDivisionsRepository(
            SMSDbContext context,
            ILogger<DepartmentDivisionsRepository> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}

