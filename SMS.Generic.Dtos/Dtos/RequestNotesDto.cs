using System;
using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    public class RequestNotesDto : BaseDto
    {
        public Guid NoteId { get; set; }

        [Required]
        public Guid RequestId { get; set; }

        public string Notes { get; set; }

        public string? Comments { get; set; }

        public bool IsLatest { get; set; }
    }
}
