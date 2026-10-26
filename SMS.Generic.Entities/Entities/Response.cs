using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a response entity in the system.
    /// </summary>
    [Table("Response", Schema = "dbo")]
    public class Response : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the response.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ResponseId { get; set; }

        /// <summary>
        /// Gets or sets the questionnaire identifier (foreign key to Questionnaire).
        /// </summary>
        public Guid? QuestionnaireId { get; set; }

        /// <summary>
        /// Gets or sets the email of the submitter.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string SubmitterEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the response was submitted.
        /// </summary>
        public DateTime? SubmittedOn { get; set; }

        /// <summary>
        /// Gets or sets the post position identifier (foreign key to PostPosition).
        /// </summary>
        public Guid? PostPositionId { get; set; }

        /// <summary>
        /// Navigation property for the questionnaire.
        /// </summary>
        [ForeignKey(nameof(QuestionnaireId))]
        public virtual Questionnaire? Questionnaire { get; set; }

        /// <summary>
        /// Navigation property for the employee who created/submitted the response.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who edited the response.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the post position.
        /// </summary>
        [ForeignKey(nameof(PostPositionId))]
        public virtual PostPosition? PostPosition { get; set; }
    }
}
