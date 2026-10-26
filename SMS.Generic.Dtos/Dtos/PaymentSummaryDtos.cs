using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Shared payment balance fields for payment-based response models.
    /// </summary>
    public abstract class PaymentBalanceBaseDto
    {
        protected PaymentBalanceBaseDto() { }

        protected PaymentBalanceBaseDto(PaymentDto payment, decimal appliedAmount, decimal refundedAmount = 0m)
        {
            PaymentId = payment.PaymentID;
            Amount = payment.Amount;
            AppliedAmount = appliedAmount;
            RefundedAmount = refundedAmount;
            AvailableAmount = payment.RemainingBalanceFrom(appliedAmount + refundedAmount);
        }

        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public decimal AppliedAmount { get; set; }

        /// <summary>
        /// Net refund allocation total for this payment (debit allocations minus credit reversals).
        /// </summary>
        public decimal RefundedAmount { get; set; }
        public decimal AvailableAmount { get; set; }
    }

    public class PaymentAllocateDto
    {
        public Guid PaymentId { get; set; }
        public Guid RequesterId { get; set; }
        public DateTime AllocationDate { get; set; }
        public List<PaymentInvoiceAllocateItemDto> Invoices { get; set; } = new();
    }

    public class PaymentInvoiceAllocateItemDto
    {
        public Guid InvoiceId { get; set; }

        /// <summary>When true, apply the invoice balance due.</summary>
        public bool ApplyFullAmount { get; set; }

        /// <summary>Used when <see cref="ApplyFullAmount"/> is false.</summary>
        public decimal Amount { get; set; }
    }

    public class PaymentAllocateResponseDto
    {
        public PaymentAllocateResponseDto() { }

        public PaymentAllocateResponseDto(PaymentDto payment, decimal totalPaid, IReadOnlyList<Guid> invoiceIds, decimal refundedAmount = 0m)
        {
            PaymentId = payment.PaymentID;
            Amount = payment.Amount;
            TotalPaid = totalPaid;
            RemainingBalance = payment.RemainingBalanceFrom(totalPaid + refundedAmount);
            InvoiceIds = invoiceIds;
        }

        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingBalance { get; set; }
        public IReadOnlyList<Guid> InvoiceIds { get; set; } = Array.Empty<Guid>();
    }

    /// <summary>
    /// Adjusts payment fields together with existing allocation amounts.
    /// </summary>
    public class PaymentAdjustAllocationsDto
    {
        public Guid PaymentId { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? ReferenceNumber { get; set; }

        [Newtonsoft.Json.JsonProperty("paymentTypeId")]
        [System.Text.Json.Serialization.JsonPropertyName("paymentTypeId")]
        public Guid? PaymentTypeID { get; set; }

        public List<PaymentAdjustAllocationItemDto> Allocations { get; set; } = new();

        public void ApplyTo(PaymentDto payment)
        {
            payment.Amount = PaymentAmount;
            payment.PaymentDate = PaymentDate;
            payment.ReferenceNumber = ReferenceNumber;
            payment.PaymentTypeID = PaymentTypeID;
        }

        public void RevertAmount(PaymentDto payment, decimal oldAmount) => payment.Amount = oldAmount;

        public bool HasPaymentFieldChanges(PaymentDto currentPayment)
        {
            ArgumentNullException.ThrowIfNull(currentPayment);

            return currentPayment.Amount != PaymentAmount || currentPayment.PaymentDate != PaymentDate
                || currentPayment.PaymentTypeID != PaymentTypeID
                || !string.Equals(currentPayment.ReferenceNumber ?? string.Empty,ReferenceNumber ?? string.Empty,StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns true when payment fields or submitted allocation amounts differ from current values.
        /// Pass <paramref name="existingActiveAllocations"/> when payment fields are unchanged and <see cref="Allocations"/> is not empty.
        /// </summary>
        public bool HasChanges(PaymentDto currentPayment,IReadOnlyList<PaymentAllocationDto>? existingActiveAllocations = null)
        {
            if (HasPaymentFieldChanges(currentPayment))
                return true;

            if (Allocations == null || Allocations.Count == 0)
                return false;

            foreach (var item in Allocations)
            {
                var existing = existingActiveAllocations?.FirstOrDefault(a => a.PaymentAllocationID == item.PaymentAllocationId);
                if (existing == null || existing.AmountApplied != item.Amount)
                    return true;
            }

            return false;
        }
    }

    public class PaymentAdjustAllocationItemDto
    {
        public Guid PaymentAllocationId { get; set; }

        public Guid InvoiceId { get; set; }

        public decimal Amount { get; set; }
    }

    public class PaymentUpdateAllocationAmountDto
    {
        public Guid PaymentId { get; set; }

        public List<PaymentAdjustAllocationItemDto> Allocations { get; set; } = new();
    }

    public class PaymentAllocationVoidRequestDto
    {
        public string Reason { get; set; } = string.Empty;
    }

    public class PaymentDeleteRequestDto
    {
        public string Reason { get; set; } = string.Empty;
    }

    public class RefundPaymentDeleteRequestDto
    {
        public string Reason { get; set; } = string.Empty;
    }

    public class PaymentAllocationUpdateResultDto
    {
        public List<PaymentAllocationDto> Updated { get; set; } = new();
        public List<PaymentInvoiceAllocationDto> SkippedAllocations { get; set; } = new();
        public List<string> Messages { get; set; } = new();
    }

    /// <summary>
    /// Credit allocations applied across payments before unclosing an overpaid invoice.
    /// </summary>
    public class InvoiceRefundAllocationsDto
    {
        public Guid InvoiceId { get; set; }
        public decimal RequiredRefundAmount { get; set; }
        public List<InvoiceRefundAllocationItemDto> Payments { get; set; } = new();
    }

    public class InvoiceRefundAllocationItemDto
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
    }

    public class RefundPaymentAllocateDto
    {
        public RefundPaymentDto RefundPayment { get; set; } = new();
        public List<RefundPaymentAllocationItemDto> Payments { get; set; } = new();

        public List<RefundPaymentAllocationItemDto> Validate()
        {
            ArgumentNullException.ThrowIfNull(RefundPayment);

            var items = RefundPaymentDto.NormalizePayments(Payments);
            if (items.Count == 0)
                throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_REFUND_PAYMENT_ALLOCATIONS), nameof(Payments));

            RefundPayment.ValidateAmountMatchesAllocationTotal(items.Sum(i => i.Amount));
            return items;
        }
    }

    public class RefundPaymentAllocationItemDto
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }

        public void ValidateAmount(PaymentDto payment, decimal invoiceAllocatedAmount, decimal refundedAmount)
        {
            RefundPaymentAllocationDto.ValidateAmountForPayment(Amount, payment, invoiceAllocatedAmount, refundedAmount);
        }

        public void ValidateForAllocation(PaymentDto payment, Guid refundRequesterId, decimal invoiceAllocatedAmount, decimal refundedAmount)
        {
            ValidateRequester(payment, refundRequesterId);
            ValidateAmount(payment, invoiceAllocatedAmount, refundedAmount);
        }

        public void ValidateRequester(PaymentDto payment, Guid refundRequesterId)
        {
            if (refundRequesterId != Guid.Empty && payment.RequesterID != refundRequesterId)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_INVOICE_REQUESTER_MISMATCH));
        }
    }

    public class RefundPaymentAllocateResponseDto
    {
        public RefundPaymentDto RefundPayment { get; set; } = new();
        public List<RefundPaymentAllocationDto> Allocations { get; set; } = new();
    }

    /// <summary>
    /// Payment header, balances, and active allocations for the review / readjust screen (GET).
    /// </summary>
    public class PaymentReviewAllocationsResponseDto : RequesterPaymentRowDto
    {
        public PaymentReviewAllocationsResponseDto() { }

        public PaymentReviewAllocationsResponseDto(
            PaymentDto payment,
            decimal appliedAmount,
            PagedList<PaymentInvoiceAllocationDto> allocations,
            decimal refundedAmount = 0m)
            : base(payment, appliedAmount, refundedAmount)
        {
            Allocations = allocations;
        }

        public PagedList<PaymentInvoiceAllocationDto> Allocations { get; set; } = new();
    }

    /// <summary>
    /// Result of adjust-allocations after payment and allocation updates succeed.
    /// </summary>
    public class PaymentAdjustAllocationsResultDto
    {
        public PaymentDto? Payment { get; set; }
        public PaymentAllocationUpdateResultDto? AllocationUpdate { get; set; }
        public decimal OldPaymentAmount { get; set; }
        public decimal NewPaymentAmount { get; set; }

        public PaymentAdjustAllocationsResultDto(PaymentDto payment, decimal oldPaymentAmount)
        {
            Payment = payment;
            OldPaymentAmount = oldPaymentAmount;
            NewPaymentAmount = payment.Amount;
        }

        public PaymentAdjustAllocationsResultDto(PaymentDto payment, decimal oldPaymentAmount, PaymentAllocationUpdateResultDto allocationUpdate)
            : this(payment, oldPaymentAmount)
        {
            AllocationUpdate = allocationUpdate;
        }
    }

    /// <summary>
    /// Invoice row for payment detail and unpaid-invoice endpoints.
    /// </summary>
    public class PaymentInvoiceRowDto
    {
        public PaymentInvoiceRowDto() { }

        public PaymentInvoiceRowDto(InvoiceDto invoice)
        {
            InvoiceId = invoice.InvoicesID;
            InvoiceNo = invoice.InvoiceNo;
            InvoiceDate = invoice.DateCreated;
            DueDate = invoice.DueDate;
            InvoiceAmount = invoice.Total ?? 0m;
            PaidAmount = invoice.PaymentsApplied ?? 0m;
            DueAmount = invoice.BalanceDue ?? 0m;
        }

        public PaymentInvoiceRowDto(InvoiceDto invoice, decimal paidFromPayment)
            : this(invoice)
        {
            PaidAmount = paidFromPayment;
            SettlementStatus = invoice.IsPaid || DueAmount <= 0m
                ? InvoiceSettlementStatus.Paid
                : InvoiceSettlementStatus.PartiallyPaid;
        }

        public Guid InvoiceId { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DueAmount { get; set; }
        public DateTime? DueDate { get; set; }
        public InvoiceSettlementStatus? SettlementStatus { get; set; }
    }

    /// <summary>
    /// Payment balances and header fields shared by invoice-context and requester-row responses.
    /// </summary>
    public abstract class PaymentHeaderBalanceDto : PaymentBalanceBaseDto
    {
        protected PaymentHeaderBalanceDto() { }

        protected PaymentHeaderBalanceDto(PaymentDto payment, decimal appliedAmount, decimal refundedAmount = 0m)
            : base(payment, appliedAmount, refundedAmount)
        {
            PaymentNumber = payment.PaymentNumber;
            PaymentDate = payment.PaymentDate;
            ReferenceNumber = payment.ReferenceNumber;
            PaymentTypeID = payment.PaymentTypeID;
            PaymentTypeName = payment.PaymentTypeName;
            CreatedBy = payment.CreatedBy;
            EditedBy = payment.EditedBy;
        }

        public string PaymentNumber { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string? ReferenceNumber { get; set; }

        [Newtonsoft.Json.JsonProperty("paymentTypeId")]
        [System.Text.Json.Serialization.JsonPropertyName("paymentTypeId")]
        public Guid? PaymentTypeID { get; set; }

        public string? PaymentTypeName { get; set; }
        public string? CreatedBy { get; set; }
        public string? EditedBy { get; set; }
    }

    /// <summary>
    /// Payment header, balances, statistics, and paged invoices (detail and unpaid-invoices).
    /// </summary>
    public class PaymentInvoicesResponseDto : PaymentHeaderBalanceDto
    {
        public PaymentInvoicesResponseDto() { }

        public PaymentInvoicesResponseDto(
            PaymentDto payment,
            decimal appliedAmount,
            decimal totalInvoicesDueAmount,
            PagedList<PaymentInvoiceRowDto> invoices,
            Guid? requesterId = null,
            decimal refundedAmount = 0m)
            : base(payment, appliedAmount, refundedAmount)
        {
            RequesterId = requesterId ?? payment.RequesterID;
            TotalInvoicesDueAmount = totalInvoicesDueAmount;
            Invoices = invoices;
        }

        public Guid? RequesterId { get; set; }
        public decimal TotalInvoicesDueAmount { get; set; }
        public PagedList<PaymentInvoiceRowDto> Invoices { get; set; } = new();
    }

    /// <summary>
    /// Single payment row with its used/remaining balance and allocation status.
    /// </summary>
    public class RequesterPaymentRowDto : PaymentHeaderBalanceDto
    {
        public RequesterPaymentRowDto() { }

        public RequesterPaymentRowDto(PaymentDto payment, decimal appliedAmount, decimal refundedAmount = 0m)
            : base(payment, appliedAmount, refundedAmount)
        {
            AllocationStatus = AvailableAmount <= 0m
                ? InvoiceSettlementStatus.Paid
                : InvoiceSettlementStatus.PartiallyPaid;
        }

        /// <summary>
        /// Fully used = <see cref="InvoiceSettlementStatus.Paid"/>;
        /// still has unused balance = <see cref="InvoiceSettlementStatus.PartiallyPaid"/>.
        /// </summary>
        public InvoiceSettlementStatus AllocationStatus { get; set; }
    }

    /// <summary>
    /// Requester payment records for a date range with overall totals.
    /// </summary>
    public class RequesterPaymentSummaryResponseDto
    {
        public RequesterPaymentSummaryResponseDto() { }

        public RequesterPaymentSummaryResponseDto(
            Guid requesterId,
            IReadOnlyList<RequesterPaymentRowDto> allRows,
            decimal totalInvoiceDueAmount,
            decimal requesterAvailableAmount,
            PagedList<RequesterPaymentRowDto> payments)
        {
            RequesterId = requesterId;
            TotalAmount = allRows.Sum(r => r.Amount);
            TotalAppliedAmount = allRows.Sum(r => r.AppliedAmount);
            TotalRefundedAmount = allRows.Sum(r => r.RefundedAmount);
            TotalAvailableAmount = allRows.Sum(r => r.AvailableAmount);
            TotalInvoiceDueAmount = totalInvoiceDueAmount;
            RequesterAvailableAmount = requesterAvailableAmount;
            Payments = payments;
        }

        public Guid RequesterId { get; set; }

        /// <summary>
        /// Total amount across all returned payment records.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Total active allocations across all returned payments.
        /// </summary>
        public decimal TotalAppliedAmount { get; set; }

        /// <summary>
        /// Net refund allocation total across all returned payments (debit minus credit).
        /// </summary>
        public decimal TotalRefundedAmount { get; set; }

        /// <summary>
        /// Sum of remaining (unallocated) balance across all returned payments.
        /// </summary>
        public decimal TotalAvailableAmount { get; set; }

        /// <summary>
        /// Sum of <c>BalanceDue</c> across all non-deleted, unpaid invoices for the requester.
        /// </summary>
        public decimal TotalInvoiceDueAmount { get; set; }

        /// <summary>
        /// Requester-level available balance (all active payments minus all active allocations, not filtered by date).
        /// </summary>
        public decimal RequesterAvailableAmount { get; set; }

        public PagedList<RequesterPaymentRowDto> Payments { get; set; } = new();
    }

    /// <summary>
    /// Refund payment header fields shared by requester-row and detail responses (mirrors <see cref="PaymentHeaderBalanceDto"/>).
    /// </summary>
    public abstract class RefundPaymentHeaderDto
    {
        protected RefundPaymentHeaderDto() { }

        protected RefundPaymentHeaderDto(RefundPaymentDto refund)
        {
            RefundPaymentId = refund.RefundPaymentID;
            RefundPaymentNumber = refund.RefundPaymentNumber;
            PaymentDate = refund.PaymentDate;
            Amount = refund.Amount;
            ReferenceNumber = refund.ReferenceNumber;
            PaymentTypeID = refund.PaymentTypeID;
            PaymentTypeName = refund.PaymentTypeName;
            CreatedBy = refund.CreatedBy;
            EditedBy = refund.EditedBy;
            DeletedBy = refund.DeletedBy;
        }

        public Guid RefundPaymentId { get; set; }
        public string RefundPaymentNumber { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string? ReferenceNumber { get; set; }

        [Newtonsoft.Json.JsonProperty("paymentTypeId")]
        [System.Text.Json.Serialization.JsonPropertyName("paymentTypeId")]
        public Guid? PaymentTypeID { get; set; }

        public string? PaymentTypeName { get; set; }
        public string? CreatedBy { get; set; }
        public string? EditedBy { get; set; }
        public string? DeletedBy { get; set; }
    }

    /// <summary>
    /// Single refund payment row for requester refund history.
    /// </summary>
    public class RequesterRefundPaymentRowDto : RefundPaymentHeaderDto
    {
        public RequesterRefundPaymentRowDto() { }

        public RequesterRefundPaymentRowDto(RefundPaymentDto refund)
            : base(refund)
        {
        }
    }

    /// <summary>
    /// Requester refund payment records for a date range with overall totals.
    /// </summary>
    public class RequesterRefundPaymentSummaryResponseDto
    {
        public RequesterRefundPaymentSummaryResponseDto() { }

        public RequesterRefundPaymentSummaryResponseDto(
            Guid requesterId,
            decimal totalAmount,
            PagedList<RequesterRefundPaymentRowDto> refunds)
        {
            RequesterId = requesterId;
            TotalAmount = totalAmount;
            Refunds = refunds;
        }

        public Guid RequesterId { get; set; }

        /// <summary>
        /// Total refund amount across all returned refund records.
        /// </summary>
        public decimal TotalAmount { get; set; }

        public PagedList<RequesterRefundPaymentRowDto> Refunds { get; set; } = new();
    }

    /// <summary>
    /// Refund payment header and full allocation history (all transaction rows).
    /// </summary>
    public class RefundPaymentAllocationHistoryResponseDto : RefundPaymentHeaderDto
    {
        public RefundPaymentAllocationHistoryResponseDto() { }

        public RefundPaymentAllocationHistoryResponseDto(
            RefundPaymentDto refund,
            List<RefundPaymentAllocationDetailDto> allocationHistory)
            : base(refund)
        {
            AllocationHistory = allocationHistory;
        }

        public List<RefundPaymentAllocationDetailDto> AllocationHistory { get; set; } = new();
    }

    /// <summary>
    /// Refund payment header and paged allocations for the detail view.
    /// </summary>
    public class RefundPaymentAllocationsResponseDto : RefundPaymentHeaderDto
    {
        public RefundPaymentAllocationsResponseDto() { }

        public RefundPaymentAllocationsResponseDto(
            RefundPaymentDto refund,
            PagedList<RefundPaymentAllocationDetailDto> allocations)
            : base(refund)
        {
            Allocations = allocations;
        }

        public PagedList<RefundPaymentAllocationDetailDto> Allocations { get; set; } = new();
    }
}
