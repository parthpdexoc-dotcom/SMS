using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an employee upload entity in the system (staging table for bulk employee uploads).
    /// </summary>
    [Table("EmployeeUpload", Schema = "dbo")]
    public class EmployeeUpload
    {
        /// <summary>
        /// Gets or sets the unique identifier for the employee upload record.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UploadID { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        [Required]
        public int EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        [Required]
        public int DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string EmployeeNo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Required]
        [MaxLength(50)]
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
        [MaxLength(50)]
        public string? UploadedBy { get; set; }
    }
}

