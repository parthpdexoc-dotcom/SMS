using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Common.Implementations.Paginations;

namespace SMS.Generic.Services.Interfaces
{
    public interface IRequesterLedgerService : IGenericService<RequesterLedgerDto, RequesterLedger>
    {
        /// <summary>
        /// Records a ledger entry from an invoice, payment, or refund source with computed balances and particulars.
        /// Not used for payment/refund allocations.
        /// </summary>
        Task<RequesterLedgerDto> AddTransactionOfRequesterLedgerAsync(
            RequesterLedgerDto entry,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bank-statement style ledger for a requester within an optional date range.
        /// </summary>
        Task<RequesterLedgerStatementDto> GetRequesterLedgerStatementAsync(
            Guid requesterId,
            DateTime? startDateTime = null,
            DateTime? endDateTime = null,
            PageParam? pagination = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Running balance for a requester from the latest ledger row, or 0 when none exist.
        /// </summary>
        Task<decimal> GetRequesterCurrentBalanceAsync(
            Guid requesterId,
            CancellationToken cancellationToken = default);
    }
}
