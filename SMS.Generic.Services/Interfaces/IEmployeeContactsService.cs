using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for employee contacts service operations.
    /// </summary>
    public interface IEmployeeContactsService : IGenericService<EmployeeContactsDto, EmployeeContacts>
    {
        /// <summary>
        /// Retrieves an employee contact by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The employee contact DTO if found; otherwise, null.</returns>
        Task<EmployeeContactsDto?> GetByIdAsync(int id);
    }
}

