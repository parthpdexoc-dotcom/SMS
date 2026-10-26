using SMS.Generic.Common.Enums;
using SMS.Generic.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    [Table("WaitingPostPosition", Schema = "dbo")]
    public class WaitingPostPosition : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid WaitingPostPositionID { get; set; }

        // 🔹 FOREIGN KEYS
        public Guid? PostID { get; set; } = Guid.Empty;
        public Guid? RequestID { get; set; } = Guid.Empty;
        public Guid EmployeeID { get; set; }
        public Guid? PostCallSignID { get; set; }

        // 🔹 BUSINESS FIELDS
        public DateOnly DateOfWork { get; set; }
        public PostPositionStatus PostPositionStatus { get; set; }

        /// <summary>
        /// Queue position for <see cref="PostPositionStatus.Waiting"/> rows sharing the same PostID, DateOfWork, and status (1-based).
        /// Set to 0 when the employee is assigned from the waiting list (<see cref="PostPositionStatus.Applied"/>).
        /// </summary>
        public int OrderNumber { get; set; }

        /// <summary>
        /// True when the employee has already been applied/assigned from this tenure waiting-list row.
        /// </summary>
        public bool HasAppliedInTenure { get; set; }

        public ProcessingStatus ProcessingStatus { get; set; } = ProcessingStatus.Pending;

        public int? StatusCode { get; set; }

        [MaxLength(2000)]
        public string? ResultMessage { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ProcessedAt { get; set; }

        public bool EquipmentRequired { get; set; }

        [MaxLength(50)]
        public string? CallSign { get; set; }

        [Column(TypeName = "money")]
        public decimal WaitingPostPositionPayRate { get; set; }

        public bool IsPremiumPaidEvent { get; set; }

        // 🔹 NAVIGATION PROPERTIES
        public Post? Post { get; set; }
        public Request? Request { get; set; }
        public Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(PostCallSignID))]
        public PostCallSign? PostCallSign { get; set; }

        // 🔹 AUDIT → Employee (BaseEntity)
        [ForeignKey("CreatedUserID")]
        public Employee? CreatedByEmployee { get; set; }

        [ForeignKey("EditedUserID")]
        public Employee? EditedByEmployee { get; set; }
    }
}

