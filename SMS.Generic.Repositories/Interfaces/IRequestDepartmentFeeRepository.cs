using SMS.Generic.Entities.Entities;
using System.Linq.Expressions;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Repository for request department fee junction records.
    /// </summary>
    public interface IRequestDepartmentFeeRepository : IGenericRepository<RequestDepartmentFee>
    {
        Task<int> DeleteByFilterAsync(Expression<Func<RequestDepartmentFee, bool>> filter);
    }
}
