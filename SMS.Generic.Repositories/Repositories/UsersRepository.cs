using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Users-specific repository implementation.
    /// </summary>
    public class UsersRepository : GenericRepository<Users, SMSDbContext>, IUsersRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsersRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public UsersRepository(
            SMSDbContext context,
            ILogger<UsersRepository> logger) : base(context, logger)
        {
        }
    }
}

