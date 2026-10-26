using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents the request type / recurrence rules (formerly RequestRecurrence).
    /// </summary>
    [Table("RequestType", Schema = "dbo")]
    public class RequestType : BaseEntity
    {
        /// <summary>
        /// Gets or sets the primary key for the request type.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RequestTypeID { get; set; }

        /// <summary>
        /// Gets or sets the foreign key to the Request table.
        /// </summary>
        public Guid RequestID { get; set; }

        /// <summary>
        /// Navigation property for the associated Request.
        /// </summary>
        [ForeignKey(nameof(RequestID))]
        public virtual Request Request { get; set; }

        /// <summary>
        /// Gets or sets the request type (formerly RecurrenceUnit).
        /// </summary>
        [Column("RequestType")]
        // Renamed to 'Type' to avoid Member name collision with Class name 'RequestType'.
        public RequestTypes Type { get; set; }

        /// <summary>
        /// Gets or sets the repeat interval (formerly Interval).
        /// </summary>
        public int RepeatEvery { get; set; }

        /// <summary>
        /// Gets or sets the days of week (formerly WeekDays).
        /// </summary>
        [NotMapped]
        public List<int>? DaysOfWeek { get; set; }

        /// <summary>
        /// Backing field for DaysOfWeek (JSON storage).
        /// </summary>
        [Column("DaysOfWeek", TypeName = "nvarchar(max)")]
        public string? DaysOfWeekJson
        {
            get => DaysOfWeek == null
                ? null
                : System.Text.Json.JsonSerializer.Serialize(DaysOfWeek);

            set => DaysOfWeek = string.IsNullOrWhiteSpace(value)
                ? null
                : System.Text.Json.JsonSerializer.Deserialize<List<int>>(value);
        }

        /// <summary>
        /// Gets or sets the days of month (formerly MonthDays).
        /// </summary>
        [NotMapped]
        public List<int>? DaysOfMonth { get; set; }

        /// <summary>
        /// Backing field for DaysOfMonth (JSON storage).
        /// </summary>
        [Column("DaysOfMonth", TypeName = "nvarchar(max)")]
        public string? DaysOfMonthJson
        {
            get => DaysOfMonth == null
                ? null
                : System.Text.Json.JsonSerializer.Serialize(DaysOfMonth);

            set => DaysOfMonth = string.IsNullOrWhiteSpace(value)
                ? null
                : System.Text.Json.JsonSerializer.Deserialize<List<int>>(value);
        }

        /// <summary>
        /// Gets or sets the monthly mode (ByDate or ByPattern).
        /// </summary>
        public MonthlyMode? MonthlyMode { get; set; }

        /// <summary>
        /// Gets or sets the week index for pattern-based monthly recurrence. 0 is every week, 1–4 are first through fourth, and 5 is last.
        /// </summary>
        public int? WeekIndex { get; set; }

        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }

        [ForeignKey(nameof(DeletedUserID))]
        public virtual Employee? DeletedByEmployee { get; set; }
    }
}
