using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for invoice repository operations, extending IGenericRepository.
    /// </summary>
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        /// <summary>
        /// Gets invoices (with details, request, and requester) for the request matching <paramref name="projectNumber"/>.
        /// </summary>
        /// <param name="projectNumber">Request project number (not fiscal year + project).</param>
        /// <param name="departmentId">Optional department scope.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IReadOnlyList<Invoice>> GetByProjectNumberAsync(
            string projectNumber,
            Guid? departmentId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets paged requesters with invoice counts (one grouped query).
        /// </summary>
        Task<(IReadOnlyList<RequesterInvoiceCountDto> Data, long TotalRecords)> GetRequesterInvoiceCountsAsync(
            Guid? departmentId,
            Guid? requesterId,
            bool? isPaid,
            string? searchTerm,
            int? dueDateRangeDays,
            DateTime departmentToday,
            PageParam pageParam,
            CancellationToken cancellationToken = default);

        Task<IReadOnlySet<Guid>> GetRequesterIdsWithUnpaidInvoicesDueTillDateAsync(
            IReadOnlyCollection<Guid> requesterIds,
            Guid? departmentId,
            DateTime departmentToday,
            CancellationToken cancellationToken = default);

        Task<string> GetNextInvoiceNumberForPrefixAsync(
            string prefix,
            Guid? departmentId = null,
            CancellationToken cancellationToken = default);
    }
}

