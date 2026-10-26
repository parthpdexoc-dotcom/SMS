using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for InvoiceEquipment operations.
    /// </summary>
    public class InvoiceEquipmentDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the invoice equipment.
        /// </summary>
        public int InvoiceEquipmentID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the abbreviation.
        /// </summary>
        [MaxLength(5, ErrorMessage = "Abbrev cannot exceed 5 characters")]
        public string? Abbrev { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [MaxLength(50, ErrorMessage = "Description cannot exceed 50 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the unit rate.
        /// </summary>
        public decimal? UnitRate { get; set; }

        /// <summary>
        /// Gets or sets whether the equipment is fixed.
        /// </summary>
        public bool? Fixed { get; set; }

        /// <summary>
        /// Gets or sets whether to show in close.
        /// </summary>
        public bool? ShowInClose { get; set; }

        /// <summary>
        /// Gets or sets the display order.
        /// </summary>
        public int? DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets whether the equipment is city owned.
        /// </summary>
        public bool? IsCityOwned { get; set; }
    }
}

