using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Emergency contact-specific repository implementation.
    /// </summary>
    public class EmergencyContactRepository : GenericRepository<EmergencyContact, SMSDbContext>, IEmergencyContactRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmergencyContactRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EmergencyContactRepository(
            SMSDbContext context,
            ILogger<EmergencyContactRepository> logger) : base(context, logger)
        {
        }
    }
}

