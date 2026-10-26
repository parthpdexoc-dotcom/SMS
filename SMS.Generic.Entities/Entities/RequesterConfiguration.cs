using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a requester configuration entity in the system.
    /// </summary>
    [Table("RequesterConfiguration", Schema = "dbo")]
    public class RequesterConfiguration
    {
        /// <summary>
        /// Gets or sets the unique identifier for the requester configuration.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ReqConID { get; set; }

        /// <summary>
        /// Gets or sets the requester identifier (references Requesters table).
        /// </summary>
        [Required]
        public Guid RequesterID { get; set; }

        /// <summary>
        /// Navigation property for the requester.
        /// </summary>
        [ForeignKey(nameof(RequesterID))]
        public virtual Requester? Requester { get; set; }

        /// <summary>
        /// Gets or sets the open 24 hour value.
        /// </summary>
        [Required]
        public int Open24Hr { get; set; }

        /// <summary>
        /// Gets or sets the SBA 24 hour value.
        /// </summary>
        [Required]
        public int Sba24Hr { get; set; }

        /// <summary>
        /// Gets or sets the hours per day.
        /// </summary>
        [Required]
        public int HourPerDay { get; set; }

        /// <summary>
        /// Gets or sets the hours per week.
        /// </summary>
        [Required]
        public int HourPerWeek { get; set; }

        /// <summary>
        /// Gets or sets whether supervisor override is enabled.
        /// </summary>
        [Required]
        public bool SupervisorOverride { get; set; }

        /// <summary>
        /// Gets or sets whether this is the requester default configuration.
        /// </summary>
        [Required]
        public bool RequesterDefault { get; set; }

        /// <summary>
        /// Gets or sets the list of not eligible employees.
        /// </summary>
        public string? NotEligibleEmployee { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        [Required]
        public int DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the track history information.
        /// </summary>
        public string? TrackHistory { get; set; }
    }
}

