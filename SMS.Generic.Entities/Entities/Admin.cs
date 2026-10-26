using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an admin entity in the system.
    /// </summary>
    [Table("Admin", Schema = "dbo")]
    public class Admin : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the admin.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid AdminID { get; set; }

        /// <summary>
        /// Gets or sets the admin type identifier.
        /// </summary>
        [Required]
        public Guid AdminTypeID { get; set; }

        /// <summary>
        /// Navigation property for the admin type.
        /// </summary>
        [ForeignKey("AdminTypeID")]
        public virtual AdminType? AdminType { get; set; }

        /// <summary>
        /// Gets or sets the first name of the admin.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the admin.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the username of the admin.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password of the admin (BCrypt hashed).
        /// </summary>
        [Required]
        [MaxLength(60)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the status of the admin.
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Navigation property for departments created by this admin.
        /// </summary>
        public virtual ICollection<Department> CreatedDepartments { get; set; } = new List<Department>();

        /// <summary>
        /// Navigation property for departments edited by this admin.
        /// </summary>
        public virtual ICollection<Department> EditedDepartments { get; set; } = new List<Department>();

        /// <summary>
        /// Navigation property for admin types created by this admin.
        /// </summary>
        public virtual ICollection<AdminType> CreatedAdminTypes { get; set; } = new List<AdminType>();

        /// <summary>
        /// Navigation property for admin types edited by this admin.
        /// </summary>
        public virtual ICollection<AdminType> EditedAdminTypes { get; set; } = new List<AdminType>();

        /// <summary>
        /// Navigation property for admin records created by this admin.
        /// </summary>
        public virtual ICollection<Admin> CreatedAdmins { get; set; } = new List<Admin>();

        /// <summary>
        /// Navigation property for admin records edited by this admin.
        /// </summary>
        public virtual ICollection<Admin> EditedAdmins { get; set; } = new List<Admin>();

        /// <summary>
        /// Navigation property for department cron job management rows created by this admin.
        /// </summary>
        public virtual ICollection<DepartmentCronJobManagement> CreatedDepartmentCronJobManagements { get; set; } = new List<DepartmentCronJobManagement>();

        /// <summary>
        /// Navigation property for department cron job management rows edited by this admin.
        /// </summary>
        public virtual ICollection<DepartmentCronJobManagement> EditedDepartmentCronJobManagements { get; set; } = new List<DepartmentCronJobManagement>();
    }
}
