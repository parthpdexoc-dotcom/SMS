using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents equipment utilization for a closed job position.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(ClosedPostPositionEquipmentID), OnlyAttributedProperties = true)]
    [Table("ClosedPostPositionEquipment", Schema = "dbo")]
    public class ClosedPostPositionEquipment : BaseEntity
    {
        /// <summary>
        /// Primary Key - Closed job position equipment ID (GUID).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ClosedPostPositionEquipmentID { get; set; }

        /// <summary>
        /// Foreign Key - Closed job position ID.
        /// </summary>
        [Required]
        public Guid ClosedPostPositionID { get; set; }

        /// <summary>
        /// Foreign Key - Department Equipment ID.
        /// </summary>
        [Required]
        public Guid DepartmentEquipmentID { get; set; }

        /// <summary>
        /// Job ID.
        /// </summary>
        public Guid? PostID { get; set; }

        /// <summary>
        /// Work start date.
        /// </summary>
        [AuditedProperty("Work start date")]
        public DateTime? WorkStartDate { get; set; }

        /// <summary>
        /// Work start time.
        /// </summary>
        [AuditedProperty("Work start time")]
        public TimeOnly? WorkStartTime { get; set; }

        /// <summary>
        /// Hours worked.
        /// </summary>
        [AuditedProperty("Hours worked")]
        public int HoursWorked { get; set; }

        /// <summary>
        /// Indicates whether the position was worked.
        /// </summary>
        [AuditedProperty("Is worked")]
        public bool IsWorked { get; set; }

        /// <summary>
        /// Reason when no work was performed.
        /// </summary>
        [MaxLength(500)]
        [AuditedProperty("No work reason")]
        public string? NoWorkReason { get; set; }

        /// <summary>
        /// Indicates whether break time is unpaid.
        /// </summary>
        [AuditedProperty("Unpaid break time")]
        public bool IsUnPaidBreakTime { get; set; }

        /// <summary>
        /// Break time (e.g. in hours).
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Break time")]
        public decimal? BreakTime { get; set; }

        /// <summary>
        /// Quantity of equipment used.
        /// </summary>
        [AuditedProperty("Used equipment quantity")]
        public int? UsedEquipmentQuantity { get; set; }

        /// <summary>
        /// Hours the equipment was used.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Equipment used hours")]
        public decimal? EquipmentUsedHours { get; set; }

        /// <summary>
        /// Paid hours.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Paid hours")]
        public decimal? PaidHours { get; set; }

        /// <summary>
        /// Reimbursement recipient (Department / Employee); derived from the assigned employee's <see cref="Employee.IsSubsidizedVehicle"/> flag on save.
        /// </summary>
        [AuditedProperty("Reimburse to")]
        public ReimburseTo? ReimburseTo { get; set; }

        /// <summary>
        /// Pay rate (money).
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Pay rate")]
        public decimal? PayRate { get; set; }

        /// <summary>
        /// Reimbursement pay rate (money).
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Reimburse pay rate")]
        public decimal? ReimbursePayRate { get; set; }

        /// <summary>
        /// Indicates whether the equipment used is a vehicle.
        /// </summary>
        [AuditedProperty("Equipment used is vehicle")]
        public bool EquipmentUsedIsVehicle { get; set; }

        /// <summary>
        /// Vehicle ownership (City / Employee / Other); applicable when EquipmentUsedIsVehicle is true.
        /// </summary>
        [AuditedProperty("Vehicle owned by")]
        public VehicleOwnedByType? VehicleOwnedBy { get; set; }

        /// <summary>
        /// Vehicle number.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Vehicle number")]
        public string? VehNumber { get; set; }

        /// <summary>
        /// Start mileage; applicable when EquipmentUsedIsVehicle is true.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Start mileage")]
        public decimal? StartMileage { get; set; }

        /// <summary>
        /// End mileage; applicable when EquipmentUsedIsVehicle is true.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("End mileage")]
        public decimal? EndMileage { get; set; }

        /// <summary>
        /// Total mileage (calculated).
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Total mileage")]
        public decimal? TotalMileage { get; set; }

        /// <summary>
        /// Vehicle quantity.
        /// </summary>
        [AuditedProperty("Vehicle quantity")]
        public int? VehicleQuantity { get; set; }

        /// <summary>
        /// Vehicle used hours.
        /// </summary>
        [AuditedProperty("Vehicle used hours")]
        public int? VehicleUsedHours { get; set; }

        /// <summary>
        /// Navigation property for closed job position.
        /// </summary>
        [ForeignKey(nameof(ClosedPostPositionID))]
        public virtual ClosedPostPosition? ClosedPostPosition { get; set; }

        /// <summary>
        /// Navigation property for department equipment.
        /// </summary>
        [ForeignKey(nameof(DepartmentEquipmentID))]
        public virtual DepartmentEquipment? DepartmentEquipment { get; set; }

        /// <summary>
        /// Navigation property for the related job.
        /// </summary>
        [ForeignKey(nameof(PostID))]
        public virtual Post? Post { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this record.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this record.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
