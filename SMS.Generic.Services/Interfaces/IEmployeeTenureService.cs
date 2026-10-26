using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Service interface for EmployeeTenure.
    /// </summary>
    public interface IEmployeeTenureService : IGenericService<EmployeeTenureDto, EmployeeTenure>
    {
        /// <summary>
        /// Gets all tenure records for the specified employee, oldest promotion first when no sort is requested.
        /// </summary>
        Task<PagedList<EmployeeTenureDto>> GetByEmployeeIdAsync(Guid employeeId, PageParam? pagination = null);
    }
}


