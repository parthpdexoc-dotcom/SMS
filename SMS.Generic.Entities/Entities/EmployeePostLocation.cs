using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// A GPS reading for an employee assigned to a job slot.
    /// The same <see cref="PostPosition"/> can have many readings over time.
    /// </summary>
    [Table("EmployeePostLocation", Schema = "dbo")]
    public class EmployeePostLocation : BaseEntity
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmployeePostLocationID { get; set; }

        /// <summary>
        /// Assigned job slot this reading belongs to.
        /// </summary>
        [Required]
        public Guid PostPositionID { get; set; }

        /// <summary>
        /// Employee who was on that slot when the reading was taken.
        /// </summary>
        [Required]
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Latitude from the device.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(9,6)")]
        public decimal Latitude { get; set; }

        /// <summary>
        /// Longitude from the device.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(9,6)")]
        public decimal Longitude { get; set; }

        /// <summary>
        /// When this GPS reading was taken.
        /// </summary>
        [Required]
        public DateTime CapturedAt { get; set; }

        [ForeignKey(nameof(PostPositionID))]
        public virtual PostPosition? PostPosition { get; set; }

        [ForeignKey(nameof(EmployeeID))]
        public virtual Employee? Employee { get; set; }

        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
