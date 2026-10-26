using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Role operations.
    /// </summary>
    public class RoleDto
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the role name.
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;
    }
}

