using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for HR employee type service operations.
    /// </summary>
    public interface IHrEmployeeTypeService : IGenericService<HrEmployeeTypeDto, HrEmployeeType>
    {
        /// <summary>
        /// Retrieves an HR employee type by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The HR employee type DTO if found; otherwise, null.</returns>
        Task<HrEmployeeTypeDto?> GetByIdAsync(int id);
    }
}

