using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for RefundPaymentAllocation operations.
    /// </summary>
    public class RefundPaymentAllocationDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the refund payment allocation.
        /// Optional: when empty, a new record is created.
        /// </summary>
        public Guid? RefundPaymentAllocationID { get; set; }

        /// <summary>
        /// Gets or sets the parent payment identifier.
        /// </summary>
        public Guid PaymentID { get; set; }

        /// <summary>
        /// Gets or sets the refund payment identifier.
        /// </summary>
        public Guid RefundPaymentID { get; set; }

        /// <summary>
        /// Gets or sets how much of the refund was applied to the payment.
        /// </summary>
        public decimal AmountApplied { get; set; }

        /// <summary>
        /// Gets or sets when the allocation was recorded.
        /// </summary>
        public DateTime? AllocationDate { get; set; }

        /// <summary>
        /// Gets or sets whether this allocation applies a refund to the payment (Debit)
        /// or reverses a prior refund allocation (Credit/Discard).
        /// </summary>
        public RefundPaymentAllocationTransactionType TransactionType { get; set; } = RefundPaymentAllocationTransactionType.Debit;

        /// <summary>
        /// Gets or sets the reason for voiding/discarding this allocation.
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// Sums net allocation amount: debited minus credited.
        /// </summary>
        public static decimal SumNetAmountApplied(IEnumerable<RefundPaymentAllocationDto>? allocations)
        {
            var debitedAmount = SumDebitedAmount(allocations);
            var creditedAmount = SumCreditedAmount(allocations);
            return debitedAmount - creditedAmount;
        }

        public static decimal SumDebitedAmount(IEnumerable<RefundPaymentAllocationDto>? allocations) =>
            Sum(allocations?.Where(a => a.TransactionType == RefundPaymentAllocationTransactionType.Debit), a => a.AmountApplied);

        public static decimal SumCreditedAmount(IEnumerable<RefundPaymentAllocationDto>? allocations) =>
            Sum(allocations?.Where(a => a.TransactionType == RefundPaymentAllocationTransactionType.Credit
                                     || a.TransactionType == RefundPaymentAllocationTransactionType.Discard), a => a.AmountApplied);

        /// <summary>
        /// Per payment: net applied amount when greater than zero.
        /// </summary>
        public static IReadOnlyList<(Guid PaymentID, decimal NetAmount)> GetActivePaymentNets(IEnumerable<RefundPaymentAllocationDto>? allocations) =>
            allocations?
                .GroupBy(a => a.PaymentID)
                .Select(g => (PaymentID: g.Key, NetAmount: SumNetAmountApplied(g)))
                .Where(x => x.NetAmount > 0m)
                .ToList()
            ?? [];

        /// <summary>
        /// Validates a credit against existing allocations for the same payment and refund.
        /// When <see cref="AmountApplied"/> is zero, fills it from net refunded.
        /// </summary>
        public void ValidateCredit(IEnumerable<RefundPaymentAllocationDto>? existingAllocationsForPaymentAndRefund)
        {
            if (TransactionType != RefundPaymentAllocationTransactionType.Credit)
                return;

            var existing = existingAllocationsForPaymentAndRefund?.ToList() ?? [];
            if (existing.Count == 0)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_CREDIT_REQUIRES_EXISTING_ALLOCATIONS));

            var netRefunded = SumNetAmountApplied(existing);
            if (AmountApplied <= 0m)
                AmountApplied = netRefunded;

            if (AmountApplied > netRefunded + 0.001m)
                throw new InvalidOperationException(string.Format(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVOICE_REFUND_EXCEEDS_PAID_ON_PAYMENT),
                    AmountApplied,
                    netRefunded));
        }

        public static void ValidateAmountForPayment(decimal amountApplied,PaymentDto payment,decimal invoiceAllocatedAmount,decimal refundedAmount)
        {
            if (amountApplied <= 0m)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED));

            if (amountApplied > payment.Amount)
            {
                throw new InvalidOperationException(string.Format(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_EXCEEDS_PAYMENT_AMOUNT),
                    amountApplied,
                    0m,
                    payment.Amount,
                    payment.Amount));
            }

            var available = payment.RemainingBalanceFrom(invoiceAllocatedAmount + refundedAmount);
            if (amountApplied > available + 0.001m)
                throw new InvalidOperationException(string.Format(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REFUND_PAYMENT_EXCEEDS_UNUSED_BALANCE),payment.PaymentNumber,available));
        }

        /// <summary>
        /// Refund payment display numbers for active allocation groups (net applied amount greater than zero).
        /// </summary>
        public static IReadOnlyList<string> GetActiveRefundDisplayNumbers(
            IEnumerable<RefundPaymentAllocationDto>? allocations,
            IReadOnlyDictionary<Guid, RefundPaymentDto>? refundPaymentsById = null) =>
            allocations?
                .GroupBy(a => a.RefundPaymentID)
                .Where(g => SumNetAmountApplied(g) > 0m)
                .Select(g =>
                {
                    RefundPaymentDto? refundPayment = null;
                    refundPaymentsById?.TryGetValue(g.Key, out refundPayment);
                    return string.IsNullOrWhiteSpace(refundPayment?.RefundPaymentNumber) ? g.Key.ToString() : refundPayment.RefundPaymentNumber.Trim();
                })
                .Distinct()
                .ToList()
            ?? [];

    }

    /// <summary>
    /// Refund payment allocation with source payment header fields (mirrors <see cref="PaymentInformationDto"/>).
    /// </summary>
    public class RefundPaymentAllocationInformationDto : RefundPaymentAllocationDto
    {
        public RefundPaymentAllocationInformationDto() { }

        public RefundPaymentAllocationInformationDto(RefundPaymentAllocationDto allocation, PaymentDto payment)
        {
            RefundPaymentAllocationID = allocation.RefundPaymentAllocationID;
            PaymentID = allocation.PaymentID;
            RefundPaymentID = allocation.RefundPaymentID;
            AmountApplied = allocation.AmountApplied;
            AllocationDate = allocation.AllocationDate;
            TransactionType = allocation.TransactionType;
            Reason = allocation.Reason;
            CreatedBy = allocation.CreatedBy;
            EditedBy = allocation.EditedBy;
            PaymentNumber = payment.PaymentNumber;
            PaymentDate = payment.PaymentDate;
            PaymentTypeID = payment.PaymentTypeID;
            PaymentTypeName = payment.PaymentTypeName;
            ReferenceNumber = payment.ReferenceNumber;
        }

        [Newtonsoft.Json.JsonProperty("paymentTypeId")]
        [System.Text.Json.Serialization.JsonPropertyName("paymentTypeId")]
        public Guid? PaymentTypeID { get; set; }

        public string? PaymentTypeName { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? PaymentNumber { get; set; }

        public string PaymentDisplayNumber =>
            string.IsNullOrWhiteSpace(PaymentNumber) ? PaymentID.ToString() : PaymentNumber;
    }

    /// <summary>
    /// Refund allocation row with source payment display fields for detail/history views.
    /// </summary>
    public class RefundPaymentAllocationDetailDto : RefundPaymentAllocationInformationDto
    {
        public RefundPaymentAllocationDetailDto() { }

        public RefundPaymentAllocationDetailDto(RefundPaymentAllocationDto allocation, PaymentDto payment)
            : base(allocation, payment)
        {
        }
    }
}
