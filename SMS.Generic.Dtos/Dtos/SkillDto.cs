using SMS.Generic.Dtos.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Skill operations.
    /// </summary>
    public class SkillDto : BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Primary Key - Skill ID (GUID)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("skillId")]
        [System.Text.Json.Serialization.JsonPropertyName("skillId")]
        public Guid? SkillID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (auto-populated from token, not required in request).
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the short name of the skill.
        /// </summary>
        public string? SkillShort { get; set; }

        /// <summary>
        /// Gets or sets the long description of the Skill.
        /// </summary>
        public string? SkillLong { get; set; }

        /// <summary>
        /// Gets or sets the status of the Skill.
        /// </summary>
        public bool SkillStatus { get; set; }

    }
}
