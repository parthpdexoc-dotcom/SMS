using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    [Table("PostDocument", Schema = "dbo")]
    public class PostDocument : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PostDocumentId { get; set; }

        [Required]
        public Guid PostID { get; set; }

        [MaxLength(200)]
        public string? DocumentName { get; set; }

        [MaxLength(500)]
        public string? DocumentPath { get; set; }

        [ForeignKey(nameof(PostID))]
        public virtual Post? Post { get; set; }

        // 🔥 Use BaseEntity FKs
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }

}
