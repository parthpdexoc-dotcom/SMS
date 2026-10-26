using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an emergency contact entity in the system.
    /// </summary>
    [Table("EmergencyContact", Schema = "dbo")]
    public class EmergencyContact
    {
        /// <summary>
        /// Gets or sets the unique identifier for the emergency contact.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmergencyContactID { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        public int? EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the name of the emergency contact.
        /// </summary>
        [MaxLength(50)]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the relationship to the employee.
        /// </summary>
        [MaxLength(30)]
        public string? Relationship { get; set; }

        /// <summary>
        /// Gets or sets the primary phone number.
        /// </summary>
        [MaxLength(30)]
        public string? Phone1 { get; set; }

        /// <summary>
        /// Gets or sets the secondary phone number.
        /// </summary>
        [MaxLength(30)]
        public string? Phone2 { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        [MaxLength(150)]
        public string? Comments { get; set; }

        /// <summary>
        /// Gets or sets whether the contact is approved.
        /// </summary>
        public bool? Approve { get; set; }

        /// <summary>
        /// Gets or sets the list order.
        /// </summary>
        public short? ListOrder { get; set; }

        /// <summary>
        /// Gets or sets who added the contact.
        /// </summary>
        [MaxLength(50)]
        public string? AddedBy { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        [Column(TypeName = "smalldatetime")]
        public DateTime? TimeStamp { get; set; }
    }
}

