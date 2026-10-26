using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    public class RefundPaymentRepository : GenericRepository<RefundPayment, SMSDbContext>, IRefundPaymentRepository
    {
        private readonly SMSDbContext _context;

        public RefundPaymentRepository(
            SMSDbContext context,
            ILogger<RefundPaymentRepository> logger) : base(context, logger)
        {
            _context = context;
        }

        public async Task<string> GetNextRefundPaymentNumberForDepartmentAndYearAsync(
            Guid departmentId,
            int fiscalYear,
            CancellationToken cancellationToken = default)
        {
            var rangeStart = fiscalYear * 100_000;
            var rangeEnd = rangeStart + 100_000;
            var fiscalYearPrefix = PaymentNumberFormat.RefundFiscalYearSearchPrefix(fiscalYear);

            var refundPaymentNumbers = await _context.RefundPayment
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(p => p.DepartmentID == departmentId && p.RefundPaymentNumber.StartsWith(fiscalYearPrefix))
                .Select(p => p.RefundPaymentNumber)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var maxRefundPaymentNumber = refundPaymentNumbers
                .Select(PaymentNumberFormat.ParseRefund)
                .Where(n => n >= rangeStart && n < rangeEnd)
                .DefaultIfEmpty(rangeStart)
                .Max();

            return PaymentNumberFormat.FormatRefund(maxRefundPaymentNumber + 1);
        }
    }
}
