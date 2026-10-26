using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        /// <summary>
        /// Returns the next payment number for a department and fiscal year (e.g. P-202600001).
        /// Sequence resets per department when the fiscal year prefix changes.
        /// </summary>
        Task<string> GetNextPaymentNumberForDepartmentAndYearAsync(
            Guid departmentId,
            int fiscalYear,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Payment with active, non-deleted allocations and related invoices for adjust-allocations.
        /// </summary>
        Task<Payment?> GetFullPaymentInformationByPaymentId(Guid paymentId, CancellationToken cancellationToken = default);
    }
}
