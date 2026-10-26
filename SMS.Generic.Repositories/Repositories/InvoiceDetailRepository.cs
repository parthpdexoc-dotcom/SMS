using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Invoice detail-specific repository implementation.
    /// </summary>
    public class InvoiceDetailRepository : GenericRepository<InvoiceDetail, SMSDbContext>, IInvoiceDetailRepository
    {
        private readonly SMSDbContext _context;

        public InvoiceDetailRepository(
            SMSDbContext context,
            ILogger<InvoiceDetailRepository> logger) : base(context, logger)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<List<InvoiceDetail>> GetLatestInvoiceDetailsByInvoiceIdAsync(Guid invoicesId, bool excludeModifiedRows = false)
        {
            var query = BillingLinesForInvoice(invoicesId);
            if (excludeModifiedRows)
                query = query.Where(x => x.FinancialImpactType != FinancialImpactType.Modified);

            var lines = await query.ToListAsync().ConfigureAwait(false);

            return lines
                .GroupBy(x => x.ClosePostPositionID!.Value)
                .Select(g => SelectDisplayBillingLine(g))
                .OrderBy(x => x.JobDate ?? DateOnly.MaxValue)
                .ThenBy(x => x.JobTime ?? TimeOnly.MaxValue)
                .ThenBy(x => x.OfficerName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        /// <inheritdoc />
        public async Task<InvoiceDetail?> GetLatestVersionByDetailIdAsync(Guid invoiceDetailId)
        {
            var currentInvoice = await _context.InvoiceDetails.AsNoTracking()
                .Where(x => x.InvoiceDetailID == invoiceDetailId && !x.IsDeleted)
                .Select(x => new { x.InvoicesID, x.ClosePostPositionID }).FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (currentInvoice == null)
                return null;

            if (!currentInvoice.ClosePostPositionID.HasValue || currentInvoice.ClosePostPositionID.Value == Guid.Empty)
            {
                return await _context.InvoiceDetails.AsNoTracking().FirstOrDefaultAsync(x => x.InvoiceDetailID == invoiceDetailId && !x.IsDeleted)
                    .ConfigureAwait(false);
            }

            var lines = await BillingLinesForInvoice(currentInvoice.InvoicesID)
                .Where(x => x.ClosePostPositionID == currentInvoice.ClosePostPositionID).ToListAsync()
                .ConfigureAwait(false);

            return SelectDisplayBillingLine(lines);
        }

        /// <inheritdoc />
        public async Task<InvoiceDetail?> GetPreviousInvoiceVersionByCreatedDateAsync(
            Guid invoicesId,
            Guid closePostPositionId,
            DateTime anchorCreatedDate)
        {
            return await BillingVersionHistory(invoicesId, closePostPositionId)
                .Where(x => (x.CreatedDate ?? DateTime.MinValue) < anchorCreatedDate)
                .OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.InvoiceDetailID)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }

        private IQueryable<InvoiceDetail> BillingVersionHistory(Guid invoicesId, Guid closePostPositionId)
        {
            return _context.InvoiceDetails
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.ClosePostPosition)
                .Where(x =>
                    x.InvoicesID == invoicesId &&
                    x.ClosePostPositionID == closePostPositionId &&
                    !x.IsDeleted &&
                    (x.FinancialImpactType == FinancialImpactType.None ||
                     x.FinancialImpactType == FinancialImpactType.Modified ||
                     x.FinancialImpactType == FinancialImpactType.Credit ||
                     (x.FinancialImpactType == FinancialImpactType.Debit && (x.Hours == null || x.Hours >= 0m))));
        }

        private IQueryable<InvoiceDetail> BillingLinesForInvoice(Guid invoicesId)
        {
            return _context.InvoiceDetails
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.ClosePostPosition)
                .Where(x =>
                    x.InvoicesID == invoicesId &&
                    !x.IsDeleted &&
                    x.ClosePostPositionID != null &&
                    x.ClosePostPosition != null &&
                    x.ClosePostPosition.Status == ClosedPostPositionStatus.Closed &&
                    !x.ClosePostPosition.IsDeleted &&
                    (x.FinancialImpactType == FinancialImpactType.None ||
                     x.FinancialImpactType == FinancialImpactType.Modified ||
                     x.FinancialImpactType == FinancialImpactType.Credit));
        }

        /// <summary>
        /// Display priority: latest <see cref="FinancialImpactType.Modified"/>, then latest <see cref="FinancialImpactType.Credit"/>, then <see cref="FinancialImpactType.None"/>.
        /// </summary>
        internal static InvoiceDetail SelectDisplayBillingLine(IEnumerable<InvoiceDetail> lines)
        {
            var list = lines.ToList();

            var modified = list
                .Where(x => x.FinancialImpactType == FinancialImpactType.Modified).OrderByDescending(GetUpdatedDateTime).FirstOrDefault();
            if (modified != null)
                return modified;

            var credit = list
                .Where(x => x.FinancialImpactType == FinancialImpactType.Credit).OrderByDescending(GetUpdatedDateTime).FirstOrDefault();
            if (credit != null)
                return credit;

            return list
                .Where(x => x.FinancialImpactType == FinancialImpactType.None)
                .OrderByDescending(GetUpdatedDateTime).ThenByDescending(x => x.InvoiceDetailID).First();
        }

        private static DateTime GetUpdatedDateTime(InvoiceDetail line) =>
            line.EditedDate ?? line.CreatedDate ?? DateTime.MinValue;
    }
}
