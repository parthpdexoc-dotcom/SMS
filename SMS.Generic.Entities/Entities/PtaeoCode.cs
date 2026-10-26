using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents the PtaeoCode table (Project/Task/Award/Fund/Organization codes).
    /// </summary>
    [Table("PtaeoCode", Schema = "dbo")]
    [AuditedEntity(DisplayPropertyName = nameof(ProjectNumber), OnlyAttributedProperties = true)]
    public class PtaeoCode
    {
        /// <summary>
        /// Primary key (GUID).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PtaeoCodeID { get; set; }

        /// <summary>
        /// Department that owns this PTAEO code.
        /// </summary>
        [Required]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Project number (e.g. 20419).
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Project Number")]
        public string? ProjectNumber { get; set; }

        /// <summary>
        /// Task number/name (e.g. Police).
        /// </summary>
        [MaxLength(100)]
        [AuditedProperty("Task Number")]
        public string? TaskNumber { get; set; }

        /// <summary>
        /// Award number (e.g. 82187).
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Award Number")]
        public string? AwardNumber { get; set; }

        /// <summary>
        /// Fund number (e.g. 0001).
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Fund Number")]
        public string? FundNumber { get; set; }

        /// <summary>
        /// Organization number.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Organization Number")]
        public string? OrganizationNumber { get; set; }

        /// <summary>
        /// Organization name (e.g. 1401385 PD Administrative Service Bureau).
        /// </summary>
        [MaxLength(500)]
        [AuditedProperty("Organization Name")]
        public string? OrganizationName { get; set; }

        /// <summary>
        /// Effective start date for this PTAEO row (from import).
        /// </summary>
        [AuditedProperty("Start Date")]
        public DateOnly? StartDate { get; set; }

        /// <summary>
        /// Effective end date for this PTAEO row (from import).
        /// </summary>
        [AuditedProperty("End Date")]
        public DateOnly? EndDate { get; set; }

        /// <summary>
        /// True when active (1), false when inactive (0); stored as <c>bit</c>.
        /// </summary>
        [AuditedProperty("Status")]
        public bool Status { get; set; } = true;

        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }
    }
}
