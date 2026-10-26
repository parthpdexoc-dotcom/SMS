using System.Collections.Generic;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Composite DTO that bundles a closed post position with its related equipment records.
    /// </summary>
    public class ClosedPostPositionDetailsDto
    {
        /// <summary>
        /// Closed post position record.
        /// </summary>
        public ClosedPostPositionDto? ClosedPostPosition { get; set; }

        /// <summary>
        /// Collection of related closed post position equipment records.
        /// </summary>
        public List<ClosedPostPositionEquipmentDto> Equipments { get; set; } = new();

        /// <summary>
        /// Questionnaire submissions (responses + answers) for this post position.
        /// </summary>
        public List<QuestionnaireSubmissionDto> QuestionnaireSubmissions { get; set; } = new();
    }
}

