using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for AdminType operations.
    /// </summary>
    public class AdminTypeDto:BaseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the admin type.
        /// If null or empty, a new Guid will be generated when creating a new record.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("adminTypeID")]
        [System.Text.Json.Serialization.JsonPropertyName("adminTypeID")]
        public Guid? AdminTypeID { get; set; }

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
        [Required(ErrorMessage = "Status is required")]
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
        /// Gets or sets the parent admin type identifier (foreign key to AdminTypeID).
        /// </summary>
        public Guid? ParentID { get; set; }

        /// <summary>
        /// Gets or sets whether this admin type is a child type.
        /// </summary>
        public bool IsChild { get; set; }
    }
}

