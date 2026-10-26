using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a division entity in the system.
    /// </summary>
    [Table("Division", Schema = "dbo")]
    public class Division
    {
        /// <summary>
        /// Gets or sets the unique identifier for the division.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DivisionID { get; set; }

        /// <summary>
        /// Gets or sets the department ID that this division belongs to.
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the division name.
        /// </summary>
        [MaxLength(50)]
        public string? DivisionName { get; set; }

        /// <summary>
        /// Gets or sets the division description.
        /// </summary>
        [MaxLength(255)]
        public string? DivisionDescription { get; set; }

        /// <summary>
        /// Gets or sets the contact person name.
        /// </summary>
        [MaxLength(50)]
        public string? Contact { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        [MaxLength(25)]
        public string? Phone { get; set; }

        /// <summary>
        /// Gets or sets the private line number.
        /// </summary>
        [MaxLength(15)]
        public string? PvtLine { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [MaxLength(35)]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the billing name.
        /// </summary>
        [MaxLength(20)]
        public string? NmBilling { get; set; }

        /// <summary>
        /// Gets or sets whether the division is a member of agency.
        /// </summary>
        public bool? MemberOfAgency { get; set; }
    }
}



