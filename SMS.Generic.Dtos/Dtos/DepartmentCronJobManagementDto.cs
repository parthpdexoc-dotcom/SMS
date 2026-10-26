using System.ComponentModel.DataAnnotations;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data transfer object for department cron job management rows.
    /// </summary>
    public class DepartmentCronJobManagementDto : BaseDto
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public Guid DepartmentCronJobManagementId { get; set; }

        /// <summary>
        /// Owning department.
        /// </summary>
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Which job is configured.
        /// </summary>
        public JobType Job { get; set; }

        /// <summary>
        /// Cron expression (5-field, UTC in host).
        /// </summary>
        [MaxLength(128)]
        public string? CronExpression { get; set; }
    }
}
