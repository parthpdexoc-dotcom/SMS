using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an employee type entity in the system.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(EmployeeTypeShort), OnlyAttributedProperties = true)]
    [Table("EmployeeType", Schema = "dbo")]
    public class EmployeeType : BaseEntity
    {
        /// <summary>
        /// Primary Key (GUID)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmployeeTypeID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (foreign key to Department).
        /// </summary>
        [Required]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the employee type short name.
        /// </summary>
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [AuditedProperty("Title")]
        public string? EmployeeTypeShort { get; set; }

        /// <summary>
        /// Gets or sets the employee type long name.
        /// </summary>
        [MaxLength(200)]
        [Column(TypeName = "varchar(200)")]
        [AuditedProperty("Description")]
        public string? EmployeeTypeLong { get; set; }

        /// <summary>
        /// Gets or sets whether the employee type is active.
        /// </summary>
        [AuditedProperty("Active")]
        public bool? EmployeeTypeActive { get; set; }

        /// <summary>
        /// Gets or sets the pay rate.
        /// </summary>
        [Column(TypeName = "money")]
        [AuditedProperty("Pay rate")]
        public decimal? PayRate { get; set; }

        /// <summary>
        /// Gets or sets the premium pay rate.
        /// </summary>
        [Column(TypeName = "money")]
        [AuditedProperty("Premium pay rate")]
        public decimal? PremiumPayRate { get; set; }

        /// <summary>
        /// Gets or sets the holiday pay rate.
        /// </summary>
        [Column(TypeName = "money")]
        [AuditedProperty("Holiday pay rate")]
        public decimal? HolidayPayRate { get; set; }

        /// <summary>
        /// Gets or sets the rank (0 = no rank/unassigned).
        /// </summary>
        [AuditedProperty("Rank")]
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets whether the employee type is for system users.
        /// </summary>
        [AuditedProperty("System user")]
        public bool IsSystemUser { get; set; }

        /// <summary>
        /// Gets or sets whether the employee type is a supervisor role.
        /// </summary>
        [AuditedProperty("Supervisor")]
        public bool IsSupervisor { get; set; }

        /// <summary>
        /// Gets or sets whether the employee type is eligible.
        /// </summary>
        [AuditedProperty("Eligible")]
        public bool IsEligible { get; set; }


        /// <summary>
        /// Gets or sets the list of employee type IDs considered the same rank (for eligibility/assignment).
        /// </summary>
        [AuditedProperty("Same rank", GetEmployeeTypeTitlesByIds = true)]
        public List<Guid>? SameRankIds { get; set; }

        // ✅ Navigation properties WITHOUT ForeignKey attributes
        public virtual Department? Department { get; set; }
        public virtual Employee? CreatedByEmployee { get; set; }
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
