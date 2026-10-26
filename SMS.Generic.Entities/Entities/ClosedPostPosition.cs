using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a closed job position record.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(ClosedPostPositionID), OnlyAttributedProperties = true)]
    public class ClosedPostPosition : BaseEntity
    {
        /// <summary>
        /// Primary Key - Closed job position ID (GUID).
        /// </summary>
        public Guid ClosedPostPositionID { get; set; }

        /// <summary>
        /// Foreign Key - Job position ID.
        /// </summary>
        public Guid PostPositionID { get; set; }

        /// <summary>
        /// Foreign Key - Job ID.
        /// </summary>
        [AuditedProperty("Job", LookupEntityType = typeof(Post), LookupDisplayPropertyName = nameof(Post.PostTitle))]
        public Guid PostID { get; set; }

        [Required]
        [AuditedProperty("Job start time")]
        public TimeOnly PostStartTime { get; set; }

        [Required]
        [AuditedProperty("Job end time")]
        public TimeOnly PostEndTime { get; set; }

        /// <summary>
        /// Indicates whether this record was edited (derived from audit fields).
        /// </summary>
        [AuditedProperty("Is edited")]
        public bool IsEdited { get; set; }

        /// <summary>
        /// Status of this closed job position history row.
        /// </summary>
        [AuditedProperty("Status")]
        public ClosedPostPositionStatus? Status { get; set; }

        /// <summary>
        /// Parent closed job position ID (derived from job position parent link).
        /// </summary>
        public Guid? ParentClosedPostPositionID { get; set; }

        /// <summary>
        /// Navigation property for the parent closed job position (self reference).
        /// </summary>
        [ForeignKey(nameof(ParentClosedPostPositionID))]
        public virtual ClosedPostPosition? ParentClosedPostPosition { get; set; }

        /// <summary>
        /// Work start date.
        /// </summary>
        [AuditedProperty("Work start date")]
        public DateOnly? WorkStartDate { get; set; }

        /// <summary>
        /// Work start time.
        /// </summary>
        [AuditedProperty("Work start time")]
        public TimeOnly? WorkStartTime { get; set; }

        /// <summary>
        /// Work end time.
        /// </summary>
        [AuditedProperty("Work end time")]
        public TimeOnly? WorkEndTime { get; set; }

        /// <summary>
        /// Hours worked.
        /// </summary>
        [AuditedProperty("Hours worked")]
        public int HoursWorked { get; set; }

        /// <summary>
        /// Hours worked on duty.
        /// </summary>
        [AuditedProperty("Worked on duty")]
        public bool WorkedOnDuty { get; set; }

        /// <summary>
        /// Comment.
        /// </summary>
        [AuditedProperty("Comment")]
        public string? Comment { get; set; }

        /// <summary>
        /// Requested start date.
        /// </summary>
        [AuditedProperty("Requested start date")]
        public DateOnly? RequestedStartDate { get; set; }

        /// <summary>
        /// Requested start time.
        /// </summary>
        [AuditedProperty("Requested start time")]
        public TimeOnly? RequestedStartTime { get; set; }

        /// <summary>
        /// Requested end time.
        /// </summary>
        [AuditedProperty("Requested end time")]
        public TimeOnly? RequestedEndTime { get; set; }

        /// <summary>
        /// User or system that closed the position.
        /// </summary>
        [AuditedProperty("Close by")]
        public string? CloseBy { get; set; }

        /// <summary>
        /// Date and time when the position was closed.
        /// </summary>
        [AuditedProperty("Close date/time")]
        public DateTime? CloseDateTime { get; set; }

        /// <summary>
        /// Indicates whether minimum hours are due.
        /// </summary>
        [AuditedProperty("Minimum hours due flag")]
        public bool IsMinimumDue { get; set; }

        /// <summary>
        /// Minimum hours due (when IsMinimumDue is true).
        /// </summary>
        [AuditedProperty("Minimum hours due")]
        public int? MinimumHoursDue { get; set; }

        /// <summary>
        /// Seniority indicator.
        /// </summary>
        [AuditedProperty("Seniority")]
        public bool? Seniority { get; set; }

        /// <summary>
        /// Indicates whether this closed job position is related to a split.
        /// </summary>
        [AuditedProperty("Is split")]
        public bool IsSplit { get; set; }

        /// <summary>
        /// Indicates whether a questionnaire is required.
        /// </summary>
        [AuditedProperty("Questionnaire required")]
        public bool QuestionaireRequired { get; set; }

        /// <summary>
        /// Foreign Key - Questionnaire response ID (links to Response).
        /// </summary>
        public Guid? QuestionaireResponseID { get; set; }

        /// <summary>
        /// Navigation property for job position.
        /// </summary>
        public virtual PostPosition? PostPosition { get; set; }

        /// <summary>
        /// Navigation property for the related job.
        /// </summary>
        public virtual Post? Post { get; set; }

        /// <summary>
        /// Navigation property for questionnaire response (Response).
        /// </summary>
        [ForeignKey(nameof(QuestionaireResponseID))]
        public virtual Response? QuestionaireResponse { get; set; }

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
