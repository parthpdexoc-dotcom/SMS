using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Minimal fields for the invoice line grid when viewing original billing rows only.
    /// </summary>
    public class ViewOriginalInvoiceLineItemDto
    {
        public Guid InvoiceDetailID { get; set; }

        public string? JobId { get; set; }

        public string? JobName { get; set; }

        public DateOnly? JobDate { get; set; }

        public TimeOnly? JobTime { get; set; }

        public TimeOnly? PostStartTime { get; set; }

        public int? PostLength { get; set; }

        public FinancialImpactType FinancialImpactType { get; set; }

        public string? OfficerName { get; set; }

        public decimal? Rate { get; set; }

        public decimal? SpecialRate { get; set; }

        public decimal? OfficerRate => Rate.HasValue ? Rate.Value - (SpecialRate ?? 0m) : null;

        public decimal? Hours { get; set; }

        public decimal? VehicleFeeAmount { get; set; }

        public decimal? EquipmentFeeAmount { get; set; }

        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// True when the linked closed post position was billed using department minimum hours paid.
        /// </summary>
        public bool IsMinimumDue { get; set; }
    }
}
