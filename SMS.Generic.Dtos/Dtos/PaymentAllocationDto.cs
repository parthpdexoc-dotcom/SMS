using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for PaymentAllocation operations.
    /// </summary>
    public class PaymentAllocationDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the payment allocation.
        /// Optional: when empty, a new record is created.
        /// </summary>
        public Guid? PaymentAllocationID { get; set; }

        /// <summary>
        /// Gets or sets the parent invoice identifier.
        /// </summary>
        public Guid InvoicesID { get; set; }

        /// <summary>
        /// Gets or sets the payment identifier.
        /// </summary>
        public Guid PaymentID { get; set; }

        /// <summary>
        /// Gets or sets how much of the payment was applied to this invoice.
        /// </summary>
        public decimal AmountApplied { get; set; }

        /// <summary>
        /// Gets or sets when the allocation was recorded.
        /// </summary>
        public DateTime? AllocationDate { get; set; }

        /// <summary>
        /// Gets or sets whether this allocation applies funds to the invoice (Debit),
        /// returns funds to escrow (Credit), or voids a prior allocation (Discard).
        /// </summary>
        public PaymentAllocationTransactionType TransactionType { get; set; } = PaymentAllocationTransactionType.Debit;

        /// <summary>
        /// Gets or sets the reason when <see cref="TransactionType"/> is Discard.
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// Sums net allocation amount: debited minus credited.
        /// </summary>
        public static decimal SumNetAmountApplied(IEnumerable<PaymentAllocationDto>? allocations)
        {
            var debitedAmount = SumDebitedAmount(allocations);
            var creditedAmount = SumCreditedAmount(allocations);
            return debitedAmount - creditedAmount;
        }

        /// <summary>
        /// Sums debited amount across Debit allocations.
        /// </summary>
        public static decimal SumDebitedAmount(IEnumerable<PaymentAllocationDto>? allocations) => Sum(allocations?.Where(a => a.TransactionType == PaymentAllocationTransactionType.Debit),a => a.AmountApplied);

        /// <summary>
        /// Sums credited amount across Credit allocations.
        /// </summary>
        public static decimal SumCreditedAmount(IEnumerable<PaymentAllocationDto>? allocations) => Sum(allocations?.Where(a => a.TransactionType == PaymentAllocationTransactionType.Credit || a.TransactionType == PaymentAllocationTransactionType.Discard),a => a.AmountApplied);

        /// <summary>
        /// Invoice display numbers for active allocation groups (net applied amount greater than zero).
        /// </summary>
        public static IReadOnlyList<string> GetActiveInvoiceDisplayNumbers(
            IEnumerable<PaymentAllocationDto>? allocations,
            IReadOnlyDictionary<Guid, InvoiceDto>? invoicesById = null) =>
            allocations?
                .GroupBy(a => a.InvoicesID)
                .Where(g => SumNetAmountApplied(g) > 0m)
                .Select(g =>
                {
                    var invoiceNo = g.OfType<PaymentInvoiceAllocationDto>()
                        .Select(a => a.InvoiceNo)
                        .FirstOrDefault(n => !string.IsNullOrWhiteSpace(n));
                    if (string.IsNullOrWhiteSpace(invoiceNo) && invoicesById != null && invoicesById.TryGetValue(g.Key, out var invoice))
                        invoiceNo = invoice.InvoiceNo;
                    return string.IsNullOrWhiteSpace(invoiceNo) ? g.Key.ToString() : invoiceNo.Trim();
                })
                .Distinct()
                .ToList()
            ?? [];

        public bool MatchesInvoice(Guid invoiceId) => InvoicesID == invoiceId;

        public bool MatchesPayment(Guid paymentId) => PaymentID == paymentId;

        public void HasSameInvoiceId(Guid invoiceId)
        {
            if (!MatchesInvoice(invoiceId))
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_NOT_FOUND_FOR_INVOICE));
        }

        public void HasSamePaymentId(Guid paymentId)
        {
            if (!MatchesPayment(paymentId))
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_NOT_BELONG_TO_PAYMENT));
        }

        /// <summary>
        /// Validates a credit against existing allocations for the same invoice and payment.
        /// When <see cref="AmountApplied"/> is zero, fills it from net paid.
        /// </summary>
        public void ValidateCredit(IEnumerable<PaymentAllocationDto>? existingAllocationsForInvoiceAndPayment)
        {
            if (TransactionType != PaymentAllocationTransactionType.Credit && TransactionType != PaymentAllocationTransactionType.Discard)
                return;

            if (TransactionType == PaymentAllocationTransactionType.Discard && string.IsNullOrWhiteSpace(Reason))
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_DISCARD_REQUIRES_REASON));

            var existing = existingAllocationsForInvoiceAndPayment?.ToList() ?? [];
            if (existing.Count == 0)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_CREDIT_REQUIRES_EXISTING_ALLOCATIONS));

            var netPaid = SumNetAmountApplied(existing);
            if (AmountApplied <= 0m)
                AmountApplied = netPaid;

            if (AmountApplied > netPaid + 0.001m)
                throw new InvalidOperationException(string.Format(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVOICE_REFUND_EXCEEDS_PAID_ON_PAYMENT),
                    AmountApplied,
                    netPaid));
        }

        public static PaymentAllocationDto? FindByPaymentAllocationId(IEnumerable<PaymentAllocationDto>? allocations, Guid paymentAllocationId) =>
            allocations?.FirstOrDefault(x => x.PaymentAllocationID == paymentAllocationId);

        public static decimal TotalAppliedOnInvoiceAfterEdits(IEnumerable<PaymentAllocationDto> invoiceAllocations, IReadOnlyDictionary<Guid, decimal> editAmounts)
        {
            var adjustedAllocations = invoiceAllocations.Select(a => new PaymentAllocationDto
            {
                PaymentAllocationID = a.PaymentAllocationID,
                InvoicesID = a.InvoicesID,
                PaymentID = a.PaymentID,
                AmountApplied = editAmounts.TryGetValue(a.PaymentAllocationID ?? Guid.Empty, out var editedAmount)
                    ? editedAmount
                    : a.AmountApplied,
                AllocationDate = a.AllocationDate,
                TransactionType = a.TransactionType
            });

            return SumNetAmountApplied(adjustedAllocations);
        }

        public static (List<PaymentAdjustAllocationItemDto> Apply, List<PaymentInvoiceAllocationDto> Skipped, List<string> Messages) ResolveAllocationEdits(
            Guid paymentId,
            IReadOnlyList<PaymentAdjustAllocationItemDto> edits,
            IEnumerable<PaymentInvoiceAllocationDto>? paymentAllocations,
            IReadOnlyDictionary<Guid, IEnumerable<PaymentAllocationDto>> invoiceAllocationsByInvoiceId)
        {
            var editAmounts = edits.ToDictionary(e => e.PaymentAllocationId, e => e.Amount);
            var skippedInvoiceIds = new HashSet<Guid>();
            var skipped = new List<PaymentInvoiceAllocationDto>();
            var messages = new List<string>();

            foreach (var invoiceId in edits.Select(e => e.InvoiceId).Distinct())
            {
                if (!invoiceAllocationsByInvoiceId.TryGetValue(invoiceId, out var invoiceAllocs))
                    continue;

                var invoiceRow = paymentAllocations?.FirstOrDefault(a => a.MatchesInvoice(invoiceId));
                var invoiceAmount = invoiceRow?.InvoiceAmount ?? 0m;
                var totalAfter = TotalAppliedOnInvoiceAfterEdits(invoiceAllocs, editAmounts);
                if (totalAfter <= invoiceAmount)
                    continue;

                skippedInvoiceIds.Add(invoiceId);
                var currentlyPaid = SumNetAmountApplied(invoiceAllocs);
                var message = $"Invoice {invoiceRow?.InvoiceNo ?? invoiceId.ToString()}: total {totalAfter:N2} exceeds invoice amount {invoiceAmount:N2}. Already paid {currentlyPaid:N2}, balance due {Math.Max(0m, invoiceAmount - currentlyPaid):N2}.";
                messages.Add(message);

                foreach (var row in paymentAllocations?.Where(a => a.MatchesInvoice(invoiceId)) ?? [])
                    skipped.Add(row.MarkSkipped(message));
            }

            var apply = new List<PaymentAdjustAllocationItemDto>();
            foreach (var item in edits)
            {
                if (skippedInvoiceIds.Contains(item.InvoiceId))
                    continue;

                var allocation = FindByPaymentAllocationId(paymentAllocations, item.PaymentAllocationId) as PaymentInvoiceAllocationDto;
                if (allocation == null)
                    throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_NOT_FOUND_FOR_INVOICE));

                allocation.HasSameInvoiceId(item.InvoiceId);
                allocation.HasSamePaymentId(paymentId);
                apply.Add(item);
            }

            return (apply, skipped, messages);
        }

        public static void ValidatePaymentTotalForEdits(PaymentDto payment, IEnumerable<PaymentInvoiceAllocationDto>? paymentAllocations, IReadOnlyList<PaymentAdjustAllocationItemDto> edits, decimal paymentAmount, decimal refundedAmount = 0m)
        {
            var editedIds = edits.Select(a => a.PaymentAllocationId).ToHashSet();
            var invoiceAllocationTotal =
                SumNetAmountApplied(paymentAllocations?.Where(a => !editedIds.Contains(a.PaymentAllocationID ?? Guid.Empty)))
                + edits.Sum(a => a.Amount);
            payment.ValidateTotalAllocatedAmount(
                invoiceAllocationTotal + refundedAmount,
                paymentAmount,
                refundedAmount,
                invoiceAllocationTotal);
        }
    }
}
