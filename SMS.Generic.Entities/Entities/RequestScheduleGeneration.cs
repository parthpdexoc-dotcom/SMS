using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// One generate or edit of request schedules.
    /// </summary>
    [Table("RequestScheduleGeneration", Schema = "dbo")]
    public class RequestScheduleGeneration : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RequestScheduleGenerationID { get; set; }

        [Required]
        public Guid RequestId { get; set; }

        [ForeignKey(nameof(RequestId))]
        public virtual Request? Request { get; set; }

        public RequestScheduleGenerationAction Action { get; set; }

        public RequestTypes RequestType { get; set; }

        [Column(TypeName = "date")]
        public DateOnly RequestStartDate { get; set; }

        [Column(TypeName = "date")]
        public DateOnly RequestEndDate { get; set; }

        [Column(TypeName = "date")]
        public DateOnly EffectiveFromDate { get; set; }

        [Column("ReleaseBeforeDaysOfWeek", TypeName = "nvarchar(max)")]
        public string? ReleaseBeforeDaysOfWeek { get; set; }

        [Column("ReleaseBeforeDaysOfMonth", TypeName = "nvarchar(max)")]
        public string? ReleaseBeforeDaysOfMonth { get; set; }

        public MonthlyMode? ReleaseBeforeMonthlyMode { get; set; }

        public int? ReleaseBeforeWeekIndex { get; set; }

        [Column(TypeName = "time")]
        public TimeOnly? ScheduledTime { get; set; }
    }
}
