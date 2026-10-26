using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for DepartmentQuestionnaire operations.
    /// </summary>
    public class DepartmentQuestionnaireDto : BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Gets or sets the unique identifier for the department questionnaire.
        /// </summary>
        public Guid? DepartmentQuestionnaireId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the department.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the questionnaire.
        /// </summary>
        public Guid QuestionnaireId { get; set; }

        public Questionnaire Questionnaire { get; set; }
}
}
