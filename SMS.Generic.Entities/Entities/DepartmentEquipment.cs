using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents department-specific equipment configuration and charging rules.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(DepartmentEquipmentID), OnlyAttributedProperties = true)]
    [Table("DepartmentEquipment", Schema = "dbo")]
    public class DepartmentEquipment : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DepartmentEquipmentID { get; set; }  // PK

        [Required]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }           // FK

        [Required]
        [AuditedProperty("Equipment type", LookupEntityType = typeof(EquipmentType), LookupDisplayPropertyName = nameof(EquipmentType.EquipementTypeShort))]
        public Guid EquipmentTypeID { get; set; }        // FK

        /// <summary>
        /// Charged type (per hour / per item).
        /// </summary>
        [AuditedProperty("Charged type")]
        public int? ChargedType { get; set; }

        /// <summary>
        /// Base charge amount.
        /// </summary>
        [Column(TypeName = "money")]
        [AuditedProperty("Charge")]
        public decimal? Charge { get; set; }

        /// <summary>
        /// Whether mileage is charged.
        /// </summary>
        [AuditedProperty("Charge mileage")]
        public bool? ChargeMileage { get; set; }

        /// <summary>
        /// Money value per mile.
        /// </summary>
        [Column(TypeName = "money")]
        [AuditedProperty("Mileage charge")]
        public decimal? MileageCharge { get; set; }

        [AuditedProperty("Mileage condition")]
        public int? MileageCondition { get; set; }

        /// <summary>
        /// Equipment status (active/inactive).
        /// </summary>
        [AuditedProperty("Active")]
        public bool? EquipmentStatus { get; set; }

        [AuditedProperty("Requires approval")]
        public bool? RequiresApproval { get; set; }

        [AuditedProperty("Default")]
        public bool IsDefault { get; set; }

        [AuditedProperty("Requires utilization recorded")]
        public bool? RequiresUtilizationRecorded { get; set; }


        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }

        [ForeignKey(nameof(EquipmentTypeID))]
        public virtual EquipmentType? EquipmentType { get; set; }

        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? AddedByEmployee { get; set; }

        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}


