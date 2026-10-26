using SMS.Generic.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    /// <remarks>Audit <see cref="Audit.AuditBase.DisplayName"/> is set from <see cref="Post.PostTitle"/> via <see cref="PostID"/> in <c>AuditChangeCapture</c>.</remarks>
    [AuditedEntity(DisplayPropertyName = nameof(PostPositionID), OnlyAttributedProperties = true)]
    [Table("PostPosition", Schema = "dbo")]
    public class PostPosition : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PostPositionID { get; set; }

        /// <summary>
        /// Unique code (nvarchar(20)) from dbo.Seq_PostpositionCode; default is six-digit zero-padded (e.g. 000001).
        /// </summary>
        [Column("PostpositionCode")]
        [MaxLength(20)]
        [AuditedProperty("Job position code")]
        public string PostPositionCode { get; set; } = null!;

        // 🔹 FOREIGN KEYS
        [AuditedProperty("Job", LookupEntityType = typeof(Post), LookupDisplayPropertyName = nameof(Post.PostTitle))]
        public Guid PostID { get; set; }
        [AuditedProperty("Employee", LookupEntityType = typeof(Employee), LookupDisplayPropertyName = nameof(Employee.FullName))]
        public Guid? EmployeeID { get; set; }
        public Guid? PostCallSignID { get; set; }

        // 🔹 PLACEHOLDER SUPPORT
        /// <summary>
        /// When true, this post position represents a virtual/placeholder assignment (no real employee).
        /// </summary>
        [AuditedProperty("Is placeholder")]
        public bool IsPlaceholder { get; set; }

        /// <summary>
        /// Foreign key to <see cref="Placeholder"/> when <see cref="IsPlaceholder"/> is true.
        /// </summary>
        [AuditedProperty("Placeholder", LookupEntityType = typeof(Placeholder), LookupDisplayPropertyName = nameof(Placeholder.LastName))]
        public Guid? PlaceholderID { get; set; }

        /// <summary>
        /// When true, invoice and payroll processing is enabled for this assignment (employee or placeholder).
        /// </summary>
        [AuditedProperty("Enable invoice and payroll")]
        public bool EnableInvoiceAndPayroll { get; set; }

        // 🔹 BUSINESS FIELDS
        [AuditedProperty("Date of work")]
        public DateOnly? DateOfWork { get; set; }

        [AuditedProperty("Start time")]
        public TimeOnly? StartTime { get; set; }

        [AuditedProperty("End time")]
        public TimeOnly? EndTime { get; set; }

        [MaxLength(50)]
        [AuditedProperty("Length of work")]
        public int LengthOfWork { get; set; }

        [AuditedProperty("Equipment required")]
        public bool EquipmentRequired { get; set; }

        [MaxLength(50)]
        [AuditedProperty("Call sign")]
        public string? CallSign { get; set; }

        [Column(TypeName = "money")]
        [AuditedProperty("Job position pay rate")]
        public decimal PostPositionPayRate { get; set; }

        [AuditedProperty("Is premium paid event")]
        public bool IsPremiumPaidEvent { get; set; }

        [AuditedProperty("Status")]
        public PostPositionStatus PostPositionStatus { get; set; }

        [MaxLength(500)]
        [AuditedProperty("Reason for cancel")]
        public string? ReasonForCancel { get; set; }

        // 🔹 SPLIT / PARENT LINKS
        [AuditedProperty("Parent job position", LookupEntityType = typeof(PostPosition), LookupDisplayPropertyName = nameof(PostPosition.PostPositionID))]
        public Guid? ParentPostPositionId { get; set; }
        [AuditedProperty("Is split")]
        public bool IsSplit { get; set; }
        [AuditedProperty("Was unclosed")]
        public bool WasUnclosed { get; set; }
        [AuditedProperty("Split job position", LookupEntityType = typeof(PostPosition), LookupDisplayPropertyName = nameof(PostPosition.PostPositionID))]
        public Guid? SplitPostPositionID { get; set; }

        /// <summary>
        /// When true, vehicle/equipment allocation has already been snapshotted for this position
        /// (including an empty kit). The finalize cron must not run again for this row.
        /// </summary>
        [AuditedProperty("Is allocation finalized")]
        public bool IsAllocationFinalized { get; set; }

        /// <summary>
        /// When true, this assignment is left out of tenure lookback totals.
        /// </summary>
        [AuditedProperty("Is exempt position")]
        public bool IsExemptPosition { get; set; }

        // 🔹 DEPARTMENT-DEFINED VALUES
        [MaxLength(200)] [AuditedProperty("Dept defined 1")] public string? PostDepartmentDefinedFieldValue1 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 2")] public string? PostDepartmentDefinedFieldValue2 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 3")] public string? PostDepartmentDefinedFieldValue3 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 4")] public string? PostDepartmentDefinedFieldValue4 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 5")] public string? PostDepartmentDefinedFieldValue5 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 6")] public string? PostDepartmentDefinedFieldValue6 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 7")] public string? PostDepartmentDefinedFieldValue7 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 8")] public string? PostDepartmentDefinedFieldValue8 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 9")] public string? PostDepartmentDefinedFieldValue9 { get; set; }
        [MaxLength(200)] [AuditedProperty("Dept defined 10")] public string? PostDepartmentDefinedFieldValue10 { get; set; }

        // 🔹 NAVIGATION PROPERTIES
        public Post Post { get; set; } = null!;
        public Employee? Employee { get; set; }

        [ForeignKey(nameof(PostCallSignID))]
        public PostCallSign? PostCallSign { get; set; }

        [ForeignKey(nameof(PlaceholderID))]
        public Placeholder? Placeholder { get; set; }

        public PostPosition? ParentPostPosition { get; set; }
        public PostPosition? SplitPostPosition { get; set; }
        public ICollection<ClosedPostPosition> ClosedPostPositions { get; set; }
    = new List<ClosedPostPosition>();

        // 🔹 AUDIT → Employee (BaseEntity)
        [ForeignKey("CreatedUserID")]
        public Employee? CreatedByEmployee { get; set; }

        [ForeignKey("EditedUserID")]
        public Employee? EditedByEmployee { get; set; }
    }
}

