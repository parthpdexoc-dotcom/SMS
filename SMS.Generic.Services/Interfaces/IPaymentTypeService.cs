using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for payment type service operations.
    /// </summary>
    public interface IPaymentTypeService : IGenericService<PaymentTypeDto, PaymentType>
    {
        /// <summary>
        /// Returns the payment type name for the given id, or null when not found.
        /// </summary>
        Task<string?> GetNameByIdAsync(Guid paymentTypeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ensures default seeded payment types exist for the department (see <see cref="Enums.PaymentTypeSeed"/>).
        /// </summary>
        Task EnsureDefaultPaymentTypesExistAsync(Guid departmentId);
    }
}
