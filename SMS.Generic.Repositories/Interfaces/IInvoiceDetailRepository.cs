using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for invoice detail repository operations, extending IGenericRepository.
    /// </summary>
    public interface IInvoiceDetailRepository : IGenericRepository<InvoiceDetail>
    {
        Task<List<InvoiceDetail>> GetLatestInvoiceDetailsByInvoiceIdAsync(Guid invoicesId, bool excludeModifiedRows = false);

        /// <summary>
        /// Returns the most recent version for the invoice detail chain that contains <paramref name="invoiceDetailId"/>.
        /// </summary>
        Task<InvoiceDetail?> GetLatestVersionByDetailIdAsync(Guid invoiceDetailId);

        /// <summary>
        /// The billing version immediately before <paramref name="anchorCreatedDate"/> for the same closed post position.
        /// </summary>
        Task<InvoiceDetail?> GetPreviousInvoiceVersionByCreatedDateAsync(
            Guid invoicesId,
            Guid closePostPositionId,
            DateTime anchorCreatedDate);
    }
}
