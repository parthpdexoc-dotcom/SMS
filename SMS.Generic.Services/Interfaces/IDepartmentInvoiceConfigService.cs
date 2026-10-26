using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using System;
using System.Threading.Tasks;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for department invoice config service operations.
    /// </summary>
    public interface IDepartmentInvoiceConfigService : IGenericService<DepartmentInvoiceConfigDto, DepartmentInvoiceConfig>
    {
        /// <summary>
        /// Returns the fiscal year (int) for a given department.
        /// If config doesn't exist or FiscalYear is empty/invalid, it falls back to current year.
        /// </summary>
        Task<int> GetFiscalYearForDepartmentAsync(Guid departmentId);
    }
}
