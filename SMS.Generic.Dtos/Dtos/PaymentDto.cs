using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Models.Constants;

namespace SMS.Generic.Dtos.Dtos
{
    public class PaymentDto : BaseDto, IDepartmentScoped
    {
        public Guid PaymentID { get; set; }

        public Guid RequesterID { get; set; }

        /// <summary>
        /// Department identifier (auto-populated from token when not provided).
        /// </summary>
        public Guid DepartmentID { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string? ReferenceNumber { get; set; }

        /// <summary>
        /// Department-scoped payment number with P- prefix (assigned on create, e.g. P-202600001).
        /// </summary>
        public string PaymentNumber { get; set; } = string.Empty;

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
        /// Reason provided when this payment was deleted.
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// Unallocated amount after applied allocations are subtracted from <see cref="Amount"/>.
        /// </summary>
        public decimal RemainingBalanceFrom(decimal allocatedAmount) => Math.Max(0m, Amount - allocatedAmount);

        public void EnsureActive()
        {
            if (IsDeleted)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_NOT_FOUND_OR_DELETED));
        }

        public static void EnsureExists(PaymentDto? payment)
        {
            if (payment == null)
                throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_NOT_FOUND_OR_DELETED));

            payment.EnsureActive();
        }

        public void ValidateTotalAllocatedAmount(
            decimal totalAllocatedAfterChange,
            decimal? amount = null,
            decimal refundedAmount = 0m,
            decimal? invoiceAllocationTotal = null)
        {
            var paymentCap = amount ?? Amount;
            if (totalAllocatedAfterChange > paymentCap)
            {
                var allocated = invoiceAllocationTotal ?? Math.Max(0m, totalAllocatedAfterChange - refundedAmount);
                var available = Math.Max(0m, paymentCap - refundedAmount);
                throw new InvalidOperationException(string.Format(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_ALLOCATION_EXCEEDS_PAYMENT_AMOUNT),
                    allocated,
                    refundedAmount,
                    paymentCap,
                    available));
            }
        }

        /// <summary>
        /// Blocks delete when net invoice or refund allocations remain on this payment.
        /// </summary>
        public void EnsureCanDelete(decimal appliedAmount,decimal refundedAmount,IEnumerable<string>? invoiceNumbers = null,IEnumerable<string>? refundNumbers = null)
        {
            if (appliedAmount <= 0m && refundedAmount <= 0m)
                return;

            var blockingDetails = new List<string>();
            if (appliedAmount > 0m)
            {
                var numbers = (invoiceNumbers ?? []).Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().ToList();
                if (numbers.Count > 0)
                    blockingDetails.Add(FormatAllocationNumbers("invoice", numbers));
            }
            if (refundedAmount > 0m)
            {
                var numbers = (refundNumbers ?? []).Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().ToList();
                if (numbers.Count > 0)
                    blockingDetails.Add(FormatAllocationNumbers("refund", numbers));
            }
            throw new InvalidOperationException(string.Format(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PAYMENT_DELETE_BLOCKED_HAS_ALLOCATIONS),blockingDetails.Count > 0 ? string.Join(" and ", blockingDetails) : "active allocations"));
        }

        private static string FormatAllocationNumbers(string label, IReadOnlyList<string> numbers) =>
            numbers.Count == 1 ? $"{label} {numbers[0]}" : $"{label}s {string.Join(", ", numbers)}";
    }
}
