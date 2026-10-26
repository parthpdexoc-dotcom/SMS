using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for employee upload service operations.
    /// </summary>
    public interface IEmployeeUploadService : IGenericService<EmployeeUploadDto, EmployeeUpload>
    {
        /// <summary>
        /// Retrieves an employee upload record by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The employee upload DTO if found; otherwise, null.</returns>
        Task<EmployeeUploadDto?> GetByIdAsync(int id);
    }
}

