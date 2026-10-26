using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a holiday day entity in the system.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(Name), OnlyAttributedProperties = true)]
    [Table("HolidayDay", Schema = "dbo")]
    public class HolidayDay : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the holiday day record.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid HolidayDayID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (foreign key to Department).
        /// </summary>
        [Required]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the name of the holiday.
        /// </summary>
        [MaxLength(100)]
        [AuditedProperty("Name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the date of the holiday.
        /// </summary>
        [Column(TypeName = "date")]
        [AuditedProperty("Date")]
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets or sets whether premium rate applies.
        /// </summary>
        [AuditedProperty("Premium rate")]
        public bool? IsPremiumRate { get; set; }

        /// <summary>
        /// Gets or sets whether the holiday is active.
        /// </summary>
        [AuditedProperty("Active")]
        public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets whether to pay premium for entire shift.
        /// </summary>
        [AuditedProperty("Pay premium for entire shift")]
        public bool? PayPremiumForEntireShift { get; set; }


        /// <summary>
        /// Navigation property to the department.
        /// </summary>
        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation property to the employee who added the record.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? AddedByEmployee { get; set; }

        /// <summary>
        /// Navigation property to the employee who last edited the record.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
