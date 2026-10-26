using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Request equipment-specific repository implementation.
    /// </summary>
    public class RequestEquipmentRepository : GenericRepository<RequestEquipment, SMSDbContext>, IRequestEquipmentRepository
    {
        private readonly SMSDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestEquipmentRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RequestEquipmentRepository(
            SMSDbContext context,
            ILogger<RequestEquipmentRepository> logger) : base(context, logger)
        {
            _context = context;
        }

        /// <summary>
        /// Deletes RequestEquipment records directly using ExecuteDelete based on the provided filter expression.
        /// </summary>
        /// <param name="filter">The filter expression to identify records to delete.</param>
        /// <returns>The number of records deleted.</returns>
        public async Task<int> DeleteByFilterAsync(Expression<Func<RequestEquipment, bool>> filter)
        {
            return await _context.RequestEquipment.Where(filter).ExecuteDeleteAsync();
        }
    }
}
