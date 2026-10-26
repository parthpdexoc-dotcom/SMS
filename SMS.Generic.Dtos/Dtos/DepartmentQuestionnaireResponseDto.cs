using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Questionnaire operations.
    /// </summary>
    public class DepartmentQuestionnaireResponseDto : QuestionnaireDto
    {
        public DepartmentQuestionnaireResponseDto() { }

        public DepartmentQuestionnaireResponseDto(
            Guid? questionnaireId,
            string? title,
            string? description,
            bool isActive,
            QuestionnaireType questionnaireType,
            Guid? sourceQuestionnaireId,
            Guid? departmentQuestionnaireId)
        {
            QuestionnaireId = questionnaireId;
            Title = title;
            Description = description;
            IsActive = isActive;
            QuestionnaireType = questionnaireType;
            SourceQuestionnaireId = sourceQuestionnaireId;
            DepartmentQuestionnaireId = departmentQuestionnaireId;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the questionnaire.
        /// </summary>
        public Guid? DepartmentQuestionnaireId { get; set; }
    }
}
