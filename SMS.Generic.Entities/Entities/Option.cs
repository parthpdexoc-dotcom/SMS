using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an Option entity in the system.
    /// </summary>
    [Table("Options", Schema = "dbo")]
    public class Option
    {
        /// <summary>
        /// Gets or sets the unique identifier for the Option.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OptionId { get; set; }

        /// <summary>
        /// Gets or sets the Question ID (foreign key to Question table).
        /// </summary>
        [Required]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Gets or sets the option text.
        /// </summary>
        [MaxLength(500)]
        [Column(TypeName = "nvarchar(500)")]
        public string? OptionText { get; set; }

        /// <summary>
        /// Gets or sets the display order of the option.
        /// </summary>
        [Required]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Navigation property to the Question.
        /// </summary>
        [ForeignKey(nameof(QuestionId))]
        public virtual Question? Question { get; set; }
    }
}
