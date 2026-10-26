using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an invoice equipment list entity in the system.
    /// </summary>
    [Table("InvoiceEquipmentList", Schema = "dbo")]
    public class InvoiceEquipmentList
    {
        /// <summary>
        /// Gets or sets the unique identifier for the invoice equipment list.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceEquipmentListID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the requester identifier.
        /// </summary>
        public int? RequesterID { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        public int? RequestID { get; set; }

        /// <summary>
        /// Gets or sets the post identifier.
        /// </summary>
        public int? PostID { get; set; }

        /// <summary>
        /// Gets or sets the project code.
        /// </summary>
        [MaxLength(7)]
        public string? ProjectCode { get; set; }

        /// <summary>
        /// Gets or sets the invoice equipment identifier.
        /// </summary>
        public int? InvoiceEquipmentID { get; set; }

        /// <summary>
        /// Gets or sets how many items.
        /// </summary>
        public int? HowMany { get; set; }

        /// <summary>
        /// Gets or sets the number of hours.
        /// </summary>
        public int? NumberOfHours { get; set; }

        /// <summary>
        /// Gets or sets whether the record is locked.
        /// </summary>
        public bool? Locked { get; set; }

        /// <summary>
        /// Gets or sets the post day.
        /// </summary>
        [Column(TypeName = "smalldatetime")]
        public DateTime? PostDay { get; set; }

        /// <summary>
        /// Gets or sets the position identifier.
        /// </summary>
        public long? PositionID { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        [Column(TypeName = "smalldatetime")]
        public DateTime? TimeStamp { get; set; }
    }
}

