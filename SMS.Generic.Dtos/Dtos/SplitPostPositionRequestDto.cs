using System.Collections.Generic;
using System.Text.Json.Serialization;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Request DTO for splitting/closing a post position with equipment details.
    /// Inherits closed post position fields.
    /// </summary>
    public class SplitPostPositionRequestDto : ClosedPostPositionDto
    {
        public List<SplitPostPositionEquipmentDto>? EquipmentDetails { get; set; }
        public bool? IsSplitRequest { get; set; }
        
        /// <summary>
        /// When true, indicates the splitter chose "split post with myself" in the UI.
        /// Backend will take the employee ID from the token and ignore any SplitEmployeeID passed.
        /// </summary>
        public bool IsSplitWithSelf { get; set; }
        
        /// <summary>
        /// Optional questionnaire submissions (1 or more). Processed on final submit only.
        /// </summary>
        public List<QuestionnaireSubmissionDto>? QuestionnaireSubmissions { get; set; }

        /// <summary>
        /// Calculated vehicle fee total from <see cref="EquipmentDetails"/> (returned on refund conflict for invoice modify).
        /// </summary>
        public decimal? TotalVehicleFee { get; set; }

        /// <summary>
        /// Calculated equipment fee total from <see cref="EquipmentDetails"/> (returned on refund conflict for invoice modify).
        /// </summary>
        public decimal? TotalEquipmentFee { get; set; }
    }

    /// <summary>
    /// Equipment details for split/close operation.
    /// Inherits closed post position equipment fields.
    /// </summary>
    public class SplitPostPositionEquipmentDto : ClosedPostPositionEquipmentDto
    {
    }
}
