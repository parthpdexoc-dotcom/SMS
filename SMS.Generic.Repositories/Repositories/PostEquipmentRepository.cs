using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Post equipment-specific repository implementation.
    /// </summary>
    public class PostEquipmentRepository : GenericRepository<PostEquipment, SMSDbContext>, IPostEquipmentRepository
    {
        private readonly ILogger _logger;
        private readonly SMSDbContext _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="PostEquipmentRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public PostEquipmentRepository(
            SMSDbContext context,
            ILogger<PostEquipmentRepository> logger) : base(context, logger)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<int> DeleteByFilterAsync(Expression<Func<PostEquipment, bool>> filter)
        {
            return await _context.PostEquipment.Where(filter).ExecuteDeleteAsync().ConfigureAwait(false);
        }
    }
}
