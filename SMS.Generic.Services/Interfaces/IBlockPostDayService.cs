using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for block post day service operations.
    /// </summary>
    public interface IBlockPostDayService : IGenericService<BlockPostDayDto, BlockPostDay>
    {
        /// <summary>
        /// Retrieves a block post day by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The block post day DTO if found; otherwise, null.</returns>
        Task<BlockPostDayDto?> GetByIdAsync(int id);
    }
}

