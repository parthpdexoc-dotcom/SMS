using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    [Table("PostNotes", Schema = "dbo")]
    public class PostNotes : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid NoteId { get; set; }

        // Foreign Keys
        [Required]
        public Guid PostID { get; set; }

        [ForeignKey(nameof(PostID))]
        public virtual Post Post { get; set; }
        public string Notes { get; set; }
        public string Comments { get; set; }
        public bool IsLatest { get; set; }

        // 🔹 AUDIT → Employee (BaseEntity)
        [ForeignKey("CreatedUserID")]
        public Employee? CreatedByEmployee { get; set; }
        [ForeignKey("EditedUserID")]
        public Employee? EditedByEmployee { get; set; }

    }
}

