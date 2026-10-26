using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for employee position service operations.
    /// </summary>
    public interface IEmployeePositionService : IGenericService<EmployeePositionDto, EmployeePosition>
    {
        /// <summary>
        /// Retrieves an employee position by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The employee position DTO if found; otherwise, null.</returns>
        Task<EmployeePositionDto?> GetByIdAsync(int id);
    }
}

