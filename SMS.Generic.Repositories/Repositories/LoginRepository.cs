using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Login-specific repository implementation.
    /// </summary>
    public class LoginRepository : GenericRepository<Login, SMSDbContext>, ILoginRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public LoginRepository(
            SMSDbContext context,
            ILogger<LoginRepository> logger) : base(context, logger)
        {
        }
    }
}

