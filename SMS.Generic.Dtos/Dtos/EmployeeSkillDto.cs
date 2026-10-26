using Newtonsoft.Json;
using SMS.Generic.Dtos.Interfaces;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for EmployeeSkill operations.
    /// </summary>
    public class EmployeeSkillDto : BaseDto
    {
        /// <summary>
        /// Primary Key - EmployeeSkill ID (GUID)
        /// </summary>
        public Guid? EmployeeSkillID { get; set; }

        /// <summary>
        /// Foreign key to Employee.
        /// </summary>
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Foreign key to Skill.
        /// </summary>
        public Guid SkillID { get; set; }

        /// <summary>
        /// Start date for which this skill is effective.
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Optional end date for which this skill is effective.
        /// </summary>
        public DateOnly? EndDate { get; set; }

    /// <summary>
    /// Short display name of the linked skill.
    /// </summary>
    public string? SkillShort { get; set; }

        /// <summary>
        /// True when the skill is still in effect: no end date, or today is on or before the end date.
        /// False when an end date is set and the current date is after that day (skill period has passed).
        /// </summary>
        public bool IsActive =>
            !EndDate.HasValue
            || DateOnly.FromDateTime(DateTime.UtcNow) <= EndDate.Value;
    }
}

