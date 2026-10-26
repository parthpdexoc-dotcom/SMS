using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an export roster entity in the system.
    /// </summary>
    [Table("ExportRoster", Schema = "dbo")]
    public class ExportRoster
    {
        /// <summary>
        /// Gets or sets the unique identifier (serial number).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SL { get; set; }

        /// <summary>
        /// Gets or sets the requester name.
        /// </summary>
        public string? RequesterName { get; set; }

        /// <summary>
        /// Gets or sets the request name.
        /// </summary>
        public string? RequestName { get; set; }

        /// <summary>
        /// Gets or sets the post day.
        /// </summary>
        public string? PostDay { get; set; }

        /// <summary>
        /// Gets or sets the position name.
        /// </summary>
        public string? PositionName { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public string? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the hours required.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? HourRequired { get; set; }

        /// <summary>
        /// Gets or sets the end date and time.
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// Gets or sets the post ID.
        /// </summary>
        public string? PostID { get; set; }

        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        public string? EmployeeNo { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets whether the position is filled.
        /// </summary>
        public string? Filled { get; set; }

        /// <summary>
        /// Gets or sets the primary contact.
        /// </summary>
        public string? PrimaryContact { get; set; }
    }
}

