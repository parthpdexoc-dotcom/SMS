namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Payment allocation with invoice display fields (populated when loaded with invoice join).
    /// </summary>
    public class PaymentInvoiceAllocationDto : PaymentInformationDto
    {
        public string? InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public DateTime? DueDate { get; set; }

        public decimal InvoiceAmount { get; set; }
        public decimal BalanceDue { get; set; }

        public bool EditSkipped { get; set; }

        public string? EditSkipMessage { get; set; }

        public PaymentInvoiceAllocationDto MarkSkipped(string message) =>
            new()
            {
                PaymentAllocationID = PaymentAllocationID,
                InvoicesID = InvoicesID,
                PaymentID = PaymentID,
                AmountApplied = AmountApplied,
                TransactionType = TransactionType,
                InvoiceNo = InvoiceNo,
                InvoiceAmount = InvoiceAmount,
                InvoiceDate = InvoiceDate,
                DueDate = DueDate,
                EditSkipped = true,
                EditSkipMessage = message
            };
    }
}
