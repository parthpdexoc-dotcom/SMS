using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for employee repository operations, extending IGenericRepository.
    /// </summary>
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        /// <summary>
        /// Retrieves an employee by employee number.
        /// </summary>
        /// <param name="employeeNo">The employee number.</param>
        /// <returns>The employee if found; otherwise, null.</returns>
        Task<Employee?> GetByEmployeeNoAsync(string employeeNo);

        /// <summary>
        /// Retrieves an employee with all related details (Department, EmployeeType, EmployeeStatus, AdminTypes) for login operations.
        /// Uses a single optimized query with LINQ joins to fetch all required data.
        /// </summary>
        /// <param name="departmentCode">The department code from Department table.</param>
        /// <param name="username">The username of the employee.</param>
        /// <returns>The employee login information DTO if found; otherwise, null.</returns>
        Task<EmployeeLoginInfoDto?> GetEmployeeWithDetailsForLoginAsync(string departmentCode, string username);

        /// <summary>
        /// Retrieves an employee with all related details for impersonation token generation.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <returns>The employee login information DTO if found; otherwise, null.</returns>
        Task<EmployeeLoginInfoDto?> GetEmployeeWithDetailsByIdAsync(Guid employeeId);

        /// <summary>
        /// Retrieves an employee by ID with employee skills and nested skill details.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <returns>The employee with skills loaded if found; otherwise, null.</returns>
        Task<Employee?> GetByIdWithSkillsAsync(Guid employeeId);

        /// <summary>
        /// Updates the password for an employee.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <param name="hashedPassword">The hashed password to set.</param>
        /// <returns>True if the password was updated successfully; otherwise, false.</returns>
        Task<bool> UpdatePasswordAsync(Guid employeeId, string hashedPassword);
        Task<bool> SetActiveSessionIdAsync(Guid employeeId, Guid sessionId);
        Task<bool> IsActiveSessionAsync(Guid employeeId, Guid sessionId);

        /// <summary>Loads an employee for Job Position Flow read, including soft-deleted rows.</summary>
        Task<Employee?> GetByIdForFlowAuditAsync(
            Guid employeeId,
            CancellationToken cancellationToken = default);
    }
}

