using SMS.Generic.Dtos.Interfaces;
using System.Text.Json.Serialization;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for CancelledPostPosition operations.
    /// </summary>
    public class CancelledPostPositionDto : BaseDto, IEmployeeScoped
    {
        /// <summary>
        /// Primary Key - Cancelled Post Position ID (GUID).
        /// </summary>
        public Guid? CancelledPostPositionID { get; set; }

        /// <summary>
        /// Foreign Key - Employee ID (set from token).
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Foreign Key - Post ID.
        /// </summary>
        public Guid? PostID { get; set; }

        /// <summary>
        /// Foreign Key - Post Position ID.
        /// </summary>
        public Guid? PostPositionID { get; set; }

        /// <summary>
        /// Foreign Key - Department Equipment ID.
        /// </summary>
        public Guid? DepartmentEquipmentID { get; set; }

        /// <summary>
        /// Hours worked.
        /// </summary>
        public int? WorkedHours { get; set; }

        /// <summary>
        /// Equipment used hours.
        /// </summary>
        public int? EquipmentUsedHours { get; set; }

        /// <summary>
        /// Mileage.
        /// </summary>
        public decimal? Mileage { get; set; }

        /// <summary>
        /// Foreign Key - Division ID.
        /// </summary>
        public Guid? DivisionID { get; set; }

        /// <summary>
        /// Hours worked on duty.
        /// </summary>
        public int? WorkedOnDuty { get; set; }

        /// <summary>
        /// Seniority indicator.
        /// </summary>
        public bool? Seniority { get; set; }

        /// <summary>
        /// Reimbursement status.
        /// </summary>
        public string? ReimburseStatus { get; set; }

        /// <summary>
        /// Pay rate (money).
        /// </summary>
        public decimal? PayRate { get; set; }

        /// <summary>
        /// Reimbursement pay rate (money).
        /// </summary>
        public decimal? ReimbursePayRate { get; set; }

        /// <summary>
        /// Indicates whether a questionnaire is required.
        /// </summary>
        public bool? QuestionaireRequired { get; set; }

        /// <summary>
        /// Foreign Key - Questionnaire response ID.
        /// </summary>
        public Guid? QuestionaireResponseID { get; set; }

        /// <summary>
        /// Compensation (money).
        /// </summary>
        public decimal? Compensation { get; set; }

        /// <summary>
        /// User or system that cancelled the position.
        /// </summary>
        public string? CancelledBy { get; set; }

        /// <summary>
        /// Date and time when the position was cancelled.
        /// </summary>
        public DateTime? CancelledDateTime { get; set; }

        /// <summary>
        /// IP address of the user who cancelled.
        /// </summary>
        public string? IPAddressCancelledBy { get; set; }

        /// <summary>
        /// Indicates whether the record is private.
        /// </summary>
        public bool? IsPrivate { get; set; }

        /// <summary>
        /// Required hours.
        /// </summary>
        public int? RequiredHours { get; set; }
    }
}
