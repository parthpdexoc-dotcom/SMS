namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Response for invoice pay flow: invoice summary, payment history, and requester unapplied balance.
    /// </summary>
    public class InvoicePaymentInformationResponseDto
    {
        /// <summary>
        /// Invoice header used when paying an invoice.
        /// </summary>
        public InvoiceDto Invoice { get; set; } = null!;

        /// <summary>
        /// Active payment allocations applied to this invoice.
        /// </summary>
        public List<PaymentInformationDto> PaymentHistory { get; set; } = new();

        /// <summary>
        /// Requester-level unapplied payment balance (never negative).
        /// </summary>
        public decimal AvailableBalance { get; set; }
        public decimal? RemainingDue { get; set; } = 0;
        public decimal? AppliedPayment { get; set; } = 0;

    }

    /// <summary>
    /// Invoice payment totals with net amount applied per payment source.
    /// </summary>
    public class InvoicePaymentSummaryResponseDto
    {
        public Guid InvoiceId { get; set; }
        public decimal Total { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceDue { get; set; }
        public List<PaymentInformationDto> Payments { get; set; } = new();
    }
}
