using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for department service operations.
    /// </summary>
    public interface IDepartmentService : IGenericService<DepartmentDto, Department>
    {
        Task<bool> DeleteDepartmentAsync(Guid departmentId);

        /// <summary>
        /// Gets a department by its name (case-insensitive match on DepartmentName).
        /// </summary>
        /// <param name="departmentName">The department name to find.</param>
        /// <returns>The department DTO if found; otherwise, null.</returns>
        Task<DepartmentDto?> GetByDepartmentNameAsync(string departmentName);

        /// <summary>
        /// Gets a department by its employee data folder name (case-insensitive match on EmployeeDataFolderName).
        /// Used by EmployeeJob to resolve department from the folder containing the Excel sheet(s).
        /// </summary>
        /// <param name="folderName">The folder name (e.g. from the path where the sheet is read).</param>
        /// <returns>The department DTO if found; otherwise, null.</returns>
        Task<DepartmentDto?> GetByEmployeeDataFolderNameAsync(string folderName);

        /// <summary>
        /// Gets the location coordinates and address details for the authenticated department from JWT token.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The department location DTO if found; otherwise, null.</returns>
        Task<DepartmentLocationDto?> GetDepartmentLocationAsync(CancellationToken cancellationToken = default);
    }
}

