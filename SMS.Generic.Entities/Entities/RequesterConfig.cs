using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a requester configuration entity in the system.
    /// </summary>
    [Table("RequesterConfig", Schema = "dbo")]
    public class RequesterConfig : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the requester configuration.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RequesterConfigID { get; set; }

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
        /// Gets or sets whether skills are enabled.
        /// </summary>
        [Required]
        public bool EnableSkills { get; set; }

        /// <summary>
        /// Gets or sets the defined skills.
        /// </summary>
        public string? DefinedSkills { get; set; }

        /// <summary>
        /// Gets or sets whether requester manager is enabled.
        /// </summary>
        [Required]
        public bool EnableRequesterManager { get; set; }

        /// <summary>
        /// Gets or sets the defined requester manager.
        /// </summary>
        public string? DefineRequesterManager { get; set; }

        /// <summary>
        /// Gets or sets whether pre-employment paperwork is enabled.
        /// </summary>
        [Required]
        public bool EnablePreEmploymentPaperwork { get; set; }

        /// <summary>
        /// Gets or sets whether activity reporting is enabled.
        /// </summary>
        [Required]
        public bool EnableActivityReporting { get; set; }

        /// <summary>
        /// Gets or sets whether restricted employee list is enabled.
        /// </summary>
        [Required]
        public bool EnableRestrictedEmployeeList { get; set; }

        /// <summary>
        /// Gets or sets whether parent child invoicing is enabled.
        /// </summary>
        [Required]
        public bool EnableParentChildInvoicing { get; set; }

        /// <summary>
        /// Gets or sets whether exempt from invoicing is enabled.
        /// </summary>
        [Required]
        public bool EnableExemptFromInvoicing { get; set; }

        /// <summary>
        /// Gets or sets whether exempt from payroll is enabled.
        /// </summary>
        [Required]
        public bool EnableExemptFromPayroll { get; set; }

        /// <summary>
        /// Gets or sets whether exempt from hours rules is enabled.
        /// </summary>
        [Required]
        public bool EnableExemptFromHoursRules { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this requester config.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this requester config.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
