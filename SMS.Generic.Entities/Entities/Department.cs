using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a department entity in the system.
    /// </summary>
    [Table("Department", Schema = "dbo")]
    public class Department : BaseEntity
    {
        /// <summary>
        /// Unique identifier for the department.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DepartmentID { get; set; }   

        /// <summary>
        /// Department code.
        /// </summary>
        [MaxLength(15)]
        public string? DepartmentCode { get; set; }

        /// <summary>
        /// Name of the department.
        /// </summary>
        [MaxLength(75)]
        public string? DepartmentName { get; set; }

        /// <summary>
        /// Address of the department.
        /// </summary>
        [MaxLength(50)]
        public string? Address { get; set; }

        /// <summary>
        /// City.
        /// </summary>
        [MaxLength(40)]
        public string? City { get; set; }

        /// <summary>
        /// State.
        /// </summary>
        [MaxLength(30)]
        public string? State { get; set; }

        /// <summary>
        /// Zip code.
        /// </summary>
        [MaxLength(12)]
        public string? Zip { get; set; }

        /// <summary>
        /// First name of contact.
        /// </summary>
        [MaxLength(15)]
        public string? FirstName { get; set; }

        /// <summary>
        /// Last name of contact.
        /// </summary>
        [MaxLength(20)]
        public string? LastName { get; set; }

        /// <summary>
        /// Phone number.
        /// </summary>
        [MaxLength(25)]
        public string? Phone { get; set; }

        /// <summary>
        /// Fax number.
        /// </summary>
        [MaxLength(25)]
        public string? Fax { get; set; }

        /// <summary>
        /// Email.
        /// </summary>
        [MaxLength(70)]
        public string? Email { get; set; }

        /// <summary>
        /// Email domain.
        /// </summary>
        [MaxLength(60)]
        public string? Domain { get; set; }

        /// <summary>
        /// Whether active.
        /// </summary>
        public bool? Active { get; set; }

        /// <summary>
        /// Department TimeZone.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string TimeZoneId { get; set; } = "Eastern Standard Time";

        /// <summary>
        /// Folder name for the department's employee Excel sheet(s) containing employee info data.
        /// </summary>
        [MaxLength(255)]
        public string? EmployeeDataFolderName { get; set; }

        /// <summary>
        /// Navigation property for the admin who created this department.
        /// </summary>
        [ForeignKey("CreatedUserID")]
        public virtual Admin? CreatedByAdmin { get; set; }

        /// <summary>
        /// Navigation property for the admin who last edited this department.
        /// </summary>
        [ForeignKey("EditedUserID")]
        public virtual Admin? EditedByAdmin { get; set; }
        public virtual DepartmentConfiguration? DepartmentConfiguration { get; set; }
        public virtual DepartmentPolicy? DepartmentPolicy { get; set; }
    }
}

