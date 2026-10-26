using System;
using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    public class PostNotesDto : BaseDto
    {
        public Guid NoteId { get; set; }

        [Required]
        public Guid PostID { get; set; }

        public string Notes { get; set; }

        public string Comments { get; set; }

        public bool IsLatest { get; set; }
    }
}
