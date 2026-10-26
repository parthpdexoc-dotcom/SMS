using SMS.Generic.Common.Audit;
using SMS.Generic.Common.Enums;
using SMS.Generic.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a scheduled window for a request.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(RequestScheduleID), OnlyAttributedProperties = true)]
    [Table("RequestSchedule", Schema = "dbo")]
    public class RequestSchedule : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RequestScheduleID { get; set; }

        [Required]
        [AuditedProperty("Request", LookupEntityType = typeof(Request), LookupDisplayPropertyName = nameof(Request.Title))]
        public Guid RequestId { get; set; }

        [ForeignKey(nameof(RequestId))]
        public virtual Request Request { get; set; } = null!;

        [Column(TypeName = "date")]
        [AuditedProperty("Start date")]
        public DateOnly? StartDate { get; set; }

        [Column(TypeName = "date")]
        [AuditedProperty("End date")]
        public DateOnly? EndDate { get; set; }

        [AuditedProperty("Release as")]
        public ReleaseAs ReleaseAs { get; set; }

        [AuditedProperty("Release hours")]
        public int? ReleaseHours { get; set; }

        /// <summary>
        /// Release lifecycle for this window (stored as int). EF maps DB values (e.g. 1 = <see cref="RequestStatus.ReleaseNow"/>).
        /// Not derived from <see cref="ProcessingStatus"/>; set from API/business logic or loaded from the database.
        /// </summary>
        [AuditedProperty("Request status")]
        public RequestStatus RequestStatus { get; set; }

        /// <summary>Background processing state for this row (stored as int).</summary>
        [AuditedProperty("Processing status")]
        public ProcessingStatus ProcessingStatus { get; set; }

        [AuditedProperty("Processing result")]
        public string? ProcessingResult { get; set; }

        public Guid? ProcessingJobId { get; set; }

        [Column(TypeName = "date")]
        [AuditedProperty("Schedule date")]
        public DateOnly? ScheduleDate { get; set; }

        [Column(TypeName = "time")]
        [AuditedProperty("Scheduled time")]
        public TimeOnly? ScheduledTime { get; set; }

        [Column(TypeName = "date")]
        [AuditedProperty("Process date")]
        public DateOnly? ProcessDate { get; set; }

        [Column(TypeName = "time")]
        [AuditedProperty("Process time")]
        public TimeOnly? ProcessTime { get; set; }

        [Column(TypeName = "datetime2")]
        [AuditedProperty("Processed at")]
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// When true, auto-generate and request-driven schedule regeneration leave this row unchanged.
        /// </summary>
        [AuditedProperty("Preserve on request update")]
        public bool PreserveOnRegenerate { get; set; }
    }
}

