using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Links a request to selected department fees.
    /// </summary>
    [Table("RequestDepartmentFee", Schema = "dbo")]
    public class RequestDepartmentFee : BaseEntity
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RequestDepartmentFeeID { get; set; }

        /// <summary>
        /// Foreign key to Request.
        /// </summary>
        [Required]
        public Guid RequestID { get; set; }

        /// <summary>
        /// Foreign key to DepartmentFee.FeeId.
        /// </summary>
        [Required]
        public Guid DepartmentFeeID { get; set; }

        [ForeignKey(nameof(RequestID))]
        public virtual Request? Request { get; set; }

        [ForeignKey(nameof(DepartmentFeeID))]
        public virtual DepartmentFee? DepartmentFee { get; set; }

        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
