using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an answers entity in the system.
    /// </summary>
    [Table("Answer", Schema = "dbo")]
    public class Answers
    {
        /// <summary>
        /// Gets or sets the unique identifier for the answer.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AnswerId { get; set; }

        /// <summary>
        /// Gets or sets the response identifier (foreign key to Response).
        /// </summary>
        [Required]
        public Guid ResponseId { get; set; }

        /// <summary>
        /// Gets or sets the question identifier (foreign key to Question).
        /// </summary>
        [Required]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Gets or sets the answer value as a JSON string.
        /// </summary>
        [Column("Answer", TypeName = "nvarchar(max)")]
        public string? Answer { get; set; }

        /// <summary>
        /// Navigation property for the response.
        /// </summary>
        [ForeignKey(nameof(ResponseId))]
        public virtual Response? Response { get; set; }

        /// <summary>
        /// Navigation property for the question.
        /// </summary>
        [ForeignKey(nameof(QuestionId))]
        public virtual Question? Question { get; set; }
    }
}
