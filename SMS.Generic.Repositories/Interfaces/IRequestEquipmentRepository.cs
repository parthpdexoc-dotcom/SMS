using SMS.Generic.Entities.Entities;
using System.Linq.Expressions;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for request equipment repository operations, extending IGenericRepository.
    /// </summary>
    public interface IRequestEquipmentRepository : IGenericRepository<RequestEquipment>
    {
        /// <summary>
        /// Deletes RequestEquipment records directly using ExecuteDelete based on the provided filter expression.
        /// </summary>
        /// <param name="filter">The filter expression to identify records to delete.</param>
        /// <returns>The number of records deleted.</returns>
        Task<int> DeleteByFilterAsync(Expression<Func<RequestEquipment, bool>> filter);
    }
}
