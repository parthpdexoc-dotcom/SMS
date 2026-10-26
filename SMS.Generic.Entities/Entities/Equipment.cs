using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an equipment entity in the system.
    /// </summary>
    [Table("Equipement", Schema = "dbo")]
    public class Equipment : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the equipment.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // because NEWID() is in SQL, not EF
        public Guid EquipementID { get; set; }

        /// <summary>
        /// Gets or sets the department ID that this equipment belongs to.
        /// </summary>
        [Required]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the equipment type ID.
        /// </summary>
        [Required]
        public Guid EquipementTypeID { get; set; }

        /// <summary>
        /// Gets or sets the short name of the equipment.
        /// </summary>
        [MaxLength(50)]
        public string? EquipementShort { get; set; }

        /// <summary>
        /// Gets or sets the long name or description of the equipment.
        /// </summary>
        [MaxLength(200)]
        public string? EquipementLong { get; set; }

        /// <summary>
        /// Gets or sets the charged type (per hour/per item).
        /// </summary>
        public int? ChargedType { get; set; }

        /// <summary>
        /// Gets or sets whether mileage is charged (will be hidden for vehicle).
        /// </summary>
        public bool? ChargeMileage { get; set; }

        /// <summary>
        /// Gets or sets the mileage charge amount (money value per mile).
        /// </summary>
        public decimal? MileageCharge { get; set; }

        /// <summary>
        /// Gets or sets whether the equipment is reimbursable.
        /// </summary>
        public bool? IsReimburseable { get; set; }

        /// <summary>
        /// Gets or sets who to reimburse to (city/officer).
        /// </summary>
        public int? ReimburseTo { get; set; }

        /// <summary>
        /// Gets or sets the equipment status.
        /// </summary>
        public bool? EquipementStatus { get; set; }

        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation property for the equipment type.
        /// </summary>
        [ForeignKey(nameof(EquipementTypeID))]
        public virtual EquipmentType? EquipementType { get; set; }

        /// <summary>
        /// Navigation property for the admin who added this equipment.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Admin? CreateByAdmin { get; set; }

        /// <summary>
        /// Navigation property for the admin who last edited this equipment.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Admin? EditedbyAdmin { get; set; }
    }
}

