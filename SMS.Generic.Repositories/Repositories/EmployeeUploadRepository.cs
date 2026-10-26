using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Employee upload-specific repository implementation.
    /// </summary>
    public class EmployeeUploadRepository : GenericRepository<EmployeeUpload, SMSDbContext>, IEmployeeUploadRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeUploadRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EmployeeUploadRepository(
            SMSDbContext context,
            ILogger<EmployeeUploadRepository> logger) : base(context, logger)
        {
        }
    }
}

