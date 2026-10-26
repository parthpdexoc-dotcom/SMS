using SMS.Generic.Dtos.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Generic.Dtos.Dtos
{
    public class PostDocumentDto : BaseDto
    {
        public Guid PostDocumentId { get; set; }

        public Guid PostID { get; set; }

        public string? DocumentName { get; set; }

        public string? DocumentPath { get; set; }
    }
}
