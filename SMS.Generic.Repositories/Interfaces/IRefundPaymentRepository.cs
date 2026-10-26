using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    public interface IRefundPaymentRepository : IGenericRepository<RefundPayment>
    {
        Task<string> GetNextRefundPaymentNumberForDepartmentAndYearAsync(
            Guid departmentId,
            int fiscalYear,
            CancellationToken cancellationToken = default);
    }
}
