using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for employee contact type service operations.
    /// </summary>
    public interface IEmployeeContactTypeService : IGenericService<EmployeeContactTypeDto, EmployeeContactType>
    {
        /// <summary>
        /// Retrieves an employee contact type by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The employee contact type DTO if found; otherwise, null.</returns>
        Task<EmployeeContactTypeDto?> GetByIdAsync(int id);
    }
}

