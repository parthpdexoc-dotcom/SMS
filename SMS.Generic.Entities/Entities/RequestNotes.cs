using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a request entity in the system.
    /// </summary>
    [Table("RequestNotes", Schema = "dbo")]
    public class RequestNotes : BaseEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid NoteId { get; set; }

        [Required]
        public Guid RequestId { get; set; }

        [ForeignKey(nameof(RequestId))]
        public virtual Request Request { get; set; }

        public string Notes { get; set; }
        public string Comments { get; set; }
        public bool IsLatest { get; set; }


        // ?? AUDIT ? Employee (BaseEntity)
        [ForeignKey("CreatedUserID")]
        public Employee? CreatedByEmployee { get; set; }
        [ForeignKey("EditedUserID")]
        public Employee? EditedByEmployee { get; set; }
    }
}