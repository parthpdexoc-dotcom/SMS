using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for department fee service operations.
    /// </summary>
    public interface IDepartmentFeeService : IGenericService<DepartmentFeeDto, DepartmentFee>
    {
        Task<IReadOnlyList<ApplicableDepartmentFeeDto>> GetFeesForRequestSelectionAsync(
            Guid? requestId = null);

        Task<IReadOnlyList<DepartmentFeeDto>> GetInvoiceEligibleFeesForRequestAsync(
            Guid departmentId,
            Guid requestId);
    }
}
