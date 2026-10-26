using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Models.Constants;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for RefundPayment operations.
    /// </summary>
    public class RefundPaymentDto : BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Gets or sets the unique identifier for the refund payment.
        /// Use <see cref="Guid.Empty"/> when creating a new record; the database assigns the value.
        /// </summary>
        public Guid RefundPaymentID { get; set; }

        public Guid RequesterID { get; set; }

        /// <summary>
        /// Department identifier (auto-populated from token when not provided).
        /// </summary>
        public Guid DepartmentID { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string? ReferenceNumber { get; set; }

        /// <summary>
        /// Department-scoped refund payment number with R- prefix (assigned on create, e.g. R-202600001).
        /// </summary>
        public string RefundPaymentNumber { get; set; } = string.Empty;

        /// <summary>
        /// Department-scoped payment type FK.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("paymentTypeId")]
        [System.Text.Json.Serialization.JsonPropertyName("paymentTypeId")]
        public Guid? PaymentTypeID { get; set; }

        /// <summary>
        /// Display name from the payment type master (populated when navigation is loaded).
        /// </summary>
        public string? PaymentTypeName { get; set; }

        /// <summary>
        /// Reason provided when this refund payment was deleted.
        /// </summary>
        public string? Reason { get; set; }

        public void ValidateAmountMatchesAllocationTotal(decimal allocationTotal)
        {
            if (Amount <= 0m)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVALID_REFUND_PAYMENT_AMOUNT));

            if (Math.Abs(Amount - allocationTotal) > 0.001m)
                throw new InvalidOperationException(string.Format(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REFUND_PAYMENT_TOTAL_MISMATCH),
                    allocationTotal,
                    Amount));
        }

        public static List<RefundPaymentAllocationItemDto> NormalizePayments(IEnumerable<RefundPaymentAllocationItemDto>? payments) =>
            payments?
                .Where(p => p.PaymentId != Guid.Empty && p.Amount > 0m)
                .GroupBy(p => p.PaymentId)
                .Select(g => new RefundPaymentAllocationItemDto { PaymentId = g.Key, Amount = g.Sum(x => x.Amount) })
                .ToList() ?? new();
    }
}
