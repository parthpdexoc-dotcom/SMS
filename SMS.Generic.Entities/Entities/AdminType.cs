using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an admin type entity in the system.
    /// </summary>
    [Table("AdminType", Schema = "dbo")]
    public class AdminType : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the admin type.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // because NEWID() is in SQL, not EF
        public Guid AdminTypeID { get; set; }

        /// <summary>
        /// Gets or sets the short name of the admin type.
        /// </summary>
        public string AdminTypeShort { get; set; }

        /// <summary>
        /// Gets or sets the long name or description of the admin type.
        /// </summary>
        public string AdminTypeLong { get; set; }

        /// <summary>
        /// Gets or sets the status of the admin type.
        /// </summary>
        public bool Status { get; set; } = true;

        /// <summary>
        /// Gets or sets whether this admin type can manage posts.
        /// </summary>
        public bool IsManagePost { get; set; }

        /// <summary>
        /// Gets or sets whether this admin type is a super user.
        /// </summary>
        public bool IsSuperUser { get; set; }

        /// <summary>
        /// Gets or sets whether this admin type is a super user type (e.g. Super User, Customer Support, IA Admin).
        /// When true, only super-user logins see this type in dropdowns; when false, non-super users see it.
        /// </summary>
        public bool IsSuperUserType { get; set; } = false;

        /// <summary>
        /// Gets or sets the parent admin type identifier (self-referencing foreign key).
        /// </summary>
        public Guid? ParentID { get; set; }

        /// <summary>
        /// Gets or sets whether this admin type is a child type.
        /// </summary>
        public bool IsChild { get; set; }

        /// <summary>
        /// Navigation property for the parent admin type.
        /// </summary>
        [ForeignKey("ParentID")]
        public virtual AdminType? Parent { get; set; }

        /// <summary>
        /// Navigation property for the admin who created this admin type.
        /// </summary>
        [ForeignKey("CreatedUserID")]
        public virtual Admin? CreatedByAdmin { get; set; }

        /// <summary>
        /// Navigation property for the admin who last edited this admin type.
        /// </summary>
        [ForeignKey("EditedUserID")]
        public virtual Admin? EditedByAdmin { get; set; }
    }
}
