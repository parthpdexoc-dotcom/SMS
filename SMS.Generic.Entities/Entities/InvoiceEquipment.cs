using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an invoice equipment entity in the system.
    /// </summary>
    [Table("InvoiceEquipment", Schema = "dbo")]
    public class InvoiceEquipment
    {
        /// <summary>
        /// Gets or sets the unique identifier for the invoice equipment.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceEquipmentID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the abbreviation.
        /// </summary>
        [MaxLength(5)]
        public string? Abbrev { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [MaxLength(50)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the unit rate.
        /// </summary>
        [Column(TypeName = "smallmoney")]
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

