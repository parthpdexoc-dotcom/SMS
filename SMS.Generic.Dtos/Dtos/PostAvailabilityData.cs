using SMS.Generic.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Generic.Dtos.Dtos
{
    public class PostAvailabilityData
    {
        public Guid RequestID { get; set; }
        public Post Post { get; set; } = null!;
        public List<PostPosition> PostPositions { get; set; } = new();
    }
}
