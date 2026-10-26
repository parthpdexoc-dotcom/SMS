using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    [Table("PostCallSign", Schema = "dbo")]
    public class PostCallSign : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PostCallSignID { get; set; }

        [Required]
        public Guid PostID { get; set; }

        [MaxLength(50)]
        public string? CallSign { get; set; }

        [ForeignKey(nameof(PostID))]
        public virtual Post? Post { get; set; }

        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
