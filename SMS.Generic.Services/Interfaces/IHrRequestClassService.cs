using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for HR request class service operations.
    /// </summary>
    public interface IHrRequestClassService : IGenericService<HrRequestClassDto, HrRequestClass>
    {
        /// <summary>
        /// Retrieves an HR request class by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The HR request class DTO if found; otherwise, null.</returns>
        Task<HrRequestClassDto?> GetByIdAsync(int id);
    }
}

