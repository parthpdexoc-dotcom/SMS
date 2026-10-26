using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for EmployeeUpload operations.
    /// </summary>
    public class EmployeeUploadDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the employee upload record.
        /// </summary>
        public int UploadID { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        [Required(ErrorMessage = "EmployeeID is required")]
        public int EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        [Required(ErrorMessage = "DepartmentID is required")]
        public int DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        [Required(ErrorMessage = "EmployeeNo is required")]
        [MaxLength(50, ErrorMessage = "EmployeeNo cannot exceed 50 characters")]
        public string EmployeeNo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Required(ErrorMessage = "FirstName is required")]
        [MaxLength(50, ErrorMessage = "FirstName cannot exceed 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Required(ErrorMessage = "LastName is required")]
        [MaxLength(50, ErrorMessage = "LastName cannot exceed 50 characters")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the appointment date.
        /// </summary>
        public DateTime? AppointmentDate { get; set; }

        /// <summary>
        /// Gets or sets the seniority date.
        /// </summary>
        public DateTime? SeniorityDate { get; set; }

        /// <summary>
        /// Gets or sets the sequence number for position.
        /// </summary>
        public int? SequenceNoPO { get; set; }

        /// <summary>
        /// Gets or sets the sequence number.
        /// </summary>
        public int? SequenceNo { get; set; }

        /// <summary>
        /// Gets or sets the promotion date.
        /// </summary>
        public DateTime? PromotionDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the record was uploaded.
        /// </summary>
        public DateTime? UploadedOn { get; set; }

        /// <summary>
        /// Gets or sets the user who uploaded the record.
        /// </summary>
        [MaxLength(50, ErrorMessage = "UploadedBy cannot exceed 50 characters")]
        public string? UploadedBy { get; set; }
    }
}

