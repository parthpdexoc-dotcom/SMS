using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a cancelled post position record.
    /// </summary>
    public class CancelledPostPosition : BaseEntity
    {
        /// <summary>
        /// Primary Key - Cancelled Post Position ID (GUID).
        /// </summary>
        public Guid CancelledPostPositionID { get; set; }

        /// <summary>
        /// Foreign Key - Employee ID.
        /// </summary>
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Foreign Key - Post ID.
        /// </summary>
        public Guid PostID { get; set; }

        /// <summary>
        /// Foreign Key - Post Position ID.
        /// </summary>
        public Guid PostPositionID { get; set; }

        /// <summary>
        /// Foreign Key - Department Equipment ID.
        /// </summary>
        public Guid DepartmentEquipmentID { get; set; }

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
        [Column(TypeName = "smallmoney")]
        public decimal? PayRate { get; set; }

        /// <summary>
        /// Reimbursement pay rate (money).
        /// </summary>
        [Column(TypeName = "smallmoney")]
        public decimal? ReimbursePayRate { get; set; }

        /// <summary>
        /// Indicates whether a questionnaire is required.
        /// </summary>
        public bool QuestionaireRequired { get; set; }

        /// <summary>
        /// Foreign Key - Questionnaire response ID (links to Response).
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

        /// <summary>
        /// Navigation property for employee.
        /// </summary>
        public virtual Employee? Employee { get; set; }

        /// <summary>
        /// Navigation property for post.
        /// </summary>
        public virtual Post? Post { get; set; }

        /// <summary>
        /// Navigation property for post position.
        /// </summary>
        public virtual PostPosition? PostPosition { get; set; }

        /// <summary>
        /// Navigation property for department equipment.
        /// </summary>
        public virtual DepartmentEquipment? DepartmentEquipment { get; set; }

        /// <summary>
        /// Navigation property for questionnaire response (Response).
        /// </summary>
        [ForeignKey(nameof(QuestionaireResponseID))]
        public virtual Response? QuestionaireResponse { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this record.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this record.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
