using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for employee status service operations.
    /// </summary>
    public interface IEmployeeStatusService : IGenericService<EmployeeStatusDto, EmployeeStatus>
    {
        /// <summary>
        /// Gets an employee status by name (case-insensitive match on StatusLong or StatusShort).
        /// </summary>
        /// <param name="name">The status name (e.g. "Active Assignment") to find.</param>
        /// <returns>The employee status DTO if found; otherwise, null.</returns>
        Task<EmployeeStatusDto?> GetByNameAsync(string name,Guid departmentId);

        /// <summary>
        /// Ensures an Active status row exists for the department (case-insensitive short/long).
        /// </summary>
        Task EnsureActiveStatusExistsForDepartmentAsync(Guid departmentId);

        /// <summary>
        /// Active statuses for dropdowns, optionally including selected inactive rows.
        /// Paged query may omit selected rows; those are merged by ID after the query.
        /// </summary>
        Task<IEnumerable<EmployeeStatusDto>> GetActiveEmployeeStatusesForDropdownAsync(
            Guid? departmentId,
            PageParam? pagination,
            IEnumerable<Guid>? selectedIds);
    }
}

