using Newtonsoft.Json;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Response for "view original" invoice lines: only normal billing rows (financial impact None / 0)
    /// with flattened work date/time for the grid. Summary totals match the persisted invoice header.
    /// </summary>
    public class ViewOriginalInvoiceDetailsResponseDto
    {
        /// <summary>Original billing line items (minimal payload for the invoice table).</summary>
        public List<ViewOriginalInvoiceLineItemDto> Items { get; set; } = new();

        /// <summary>Stored invoice summary (same as full invoice document).</summary>
        public decimal? OfficerHourlyTotal { get; set; }

        public decimal? OfficerFeesTotal { get; set; }

        public decimal? AgencyFeesTotal { get; set; }

        public decimal? VehicleFeesTotal { get; set; }

        public decimal? EquipmentFeesTotal { get; set; }

        public decimal? Total { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? PaymentsApplied { get; set; }

        public decimal? BalanceDue { get; set; }
    }
}
