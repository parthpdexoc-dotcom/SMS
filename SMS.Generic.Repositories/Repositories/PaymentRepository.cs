using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    public class PaymentRepository : GenericRepository<Payment, SMSDbContext>, IPaymentRepository
    {
        private readonly SMSDbContext _context;

        public PaymentRepository(
            SMSDbContext context,
            ILogger<PaymentRepository> logger) : base(context, logger)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<string> GetNextPaymentNumberForDepartmentAndYearAsync(
            Guid departmentId,
            int fiscalYear,
            CancellationToken cancellationToken = default)
        {
            var rangeStart = fiscalYear * 100_000;
            var rangeEnd = rangeStart + 100_000;
            var fiscalYearPrefix = PaymentNumberFormat.FiscalYearSearchPrefix(fiscalYear);

            var paymentNumbers = await _context.Payment
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(p => p.DepartmentID == departmentId && p.PaymentNumber.StartsWith(fiscalYearPrefix))
                .Select(p => p.PaymentNumber)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var maxPaymentNumber = paymentNumbers
                .Select(PaymentNumberFormat.Parse)
                .Where(n => n >= rangeStart && n < rangeEnd)
                .DefaultIfEmpty(rangeStart)
                .Max();

            return PaymentNumberFormat.Format(maxPaymentNumber + 1);
        }

        /// <inheritdoc />
        public async Task<Payment?> GetFullPaymentInformationByPaymentId(Guid paymentId, CancellationToken cancellationToken = default)
        {
            if (paymentId == Guid.Empty)
                return null;

            return await _context.Payment
                .AsNoTracking()
                .Include(p => p.PaymentTypeMaster)
                .Include(p => p.PaymentAllocations.Where(pa => !pa.IsDeleted))
                    .ThenInclude(pa => pa.Invoice)
                .FirstOrDefaultAsync(p => p.PaymentID == paymentId && !p.IsDeleted, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
