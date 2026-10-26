using System.Linq.Expressions;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for post equipment repository operations, extending IGenericRepository.
    /// </summary>
    public interface IPostEquipmentRepository : IGenericRepository<PostEquipment>
    {
        /// <summary>
        /// Deletes PostEquipment rows matching the filter (bulk delete via EF ExecuteDelete).
        /// </summary>
        Task<int> DeleteByFilterAsync(Expression<Func<PostEquipment, bool>> filter);
    }
}