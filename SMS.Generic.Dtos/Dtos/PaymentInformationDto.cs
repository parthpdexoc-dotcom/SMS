namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Payment allocation with payment header fields (populated when loaded with payment join).
    /// </summary>
    public class PaymentInformationDto : PaymentAllocationDto
    {
        [Newtonsoft.Json.JsonProperty("paymentTypeId")]
        [System.Text.Json.Serialization.JsonPropertyName("paymentTypeId")]
        public Guid? PaymentTypeID { get; set; }

        public string? PaymentTypeName { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? PaymentNumber { get; set; }

        /// <summary>
        /// Original payment amount from the payment record (may differ from net applied on this invoice).
        /// </summary>
        public decimal PaymentAmount { get; set; }
    }
}
