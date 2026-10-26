using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository for request department fee junction records.
    /// </summary>
    public class RequestDepartmentFeeRepository : GenericRepository<RequestDepartmentFee, SMSDbContext>, IRequestDepartmentFeeRepository
    {
        private readonly SMSDbContext _context;

        public RequestDepartmentFeeRepository(
            SMSDbContext context,
            ILogger<RequestDepartmentFeeRepository> logger) : base(context, logger)
        {
            _context = context;
        }

        public async Task<int> DeleteByFilterAsync(Expression<Func<RequestDepartmentFee, bool>> filter)
        {
            return await _context.RequestDepartmentFee.Where(filter).ExecuteDeleteAsync();
        }
    }
}
