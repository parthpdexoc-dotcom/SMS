using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a post type entity in the system.
    /// </summary>
    [Table("PostType", Schema = "dbo")]
    public class PostType
    {
        /// <summary>
        /// Gets or sets the unique identifier for the post type.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostTypeID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [MaxLength(35)]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [MaxLength(255)]
        public string? Description { get; set; }
    }
}

