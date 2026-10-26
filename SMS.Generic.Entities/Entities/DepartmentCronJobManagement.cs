using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities;

/// <summary>
/// Optional per-department cron and job pairing. The scheduler only uses this table for a
/// <see cref="Job"/> when at least one row exists for that job type. If <em>no</em> row exists
/// for a given <see cref="Job"/>, that job is scheduled only with the default cron from
/// application configuration, in UTC, as a single global schedule. Rows for other jobs are unchanged.
/// </summary>
[Table("DepartmentCronJobManagement", Schema = "dbo")]
public class DepartmentCronJobManagement : BaseEntity
{
    /// <summary>
    /// Primary key.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid DepartmentCronJobManagementId { get; set; }

    /// <summary>
    /// Department this cron configuration applies to.
    /// </summary>
    [Required]
    public Guid DepartmentID { get; set; }

    /// <summary>
    /// Which background job is configured.
    /// </summary>
    [Required]
    public JobType Job { get; set; }

    /// <summary>
    /// Cron expression in standard 5-field form (e.g. "0 7 * * *"), evaluated in the department's time zone (see <see cref="Department.TimeZoneId"/>).
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string CronExpression { get; set; } = null!;

    /// <summary>
    /// Navigation: owning department.
    /// </summary>
    [ForeignKey(nameof(DepartmentID))]
    public virtual Department? Department { get; set; }

    /// <summary>
    /// Navigation: admin who created this row.
    /// </summary>
    [ForeignKey("CreatedUserID")]
    public virtual Admin? CreatedByAdmin { get; set; }

    /// <summary>
    /// Navigation: admin who last edited this row.
    /// </summary>
    [ForeignKey("EditedUserID")]
    public virtual Admin? EditedByAdmin { get; set; }
}
