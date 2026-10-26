using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Employee contacts-specific repository implementation.
    /// </summary>
    public class EmployeeContactsRepository : GenericRepository<EmployeeContacts, SMSDbContext>, IEmployeeContactsRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeContactsRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EmployeeContactsRepository(
            SMSDbContext context,
            ILogger<EmployeeContactsRepository> logger) : base(context, logger)
        {
        }
    }
}

