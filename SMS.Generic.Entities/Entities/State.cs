using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a state entity in the system.
    /// </summary>
    [Table("State", Schema = "dbo")]
    public class State
    {
        /// <summary>
        /// Gets or sets the unique identifier for the state.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StateID { get; set; }

        /// <summary>
        /// Gets or sets the state code.
        /// </summary>
        [MaxLength(2)]
        public string? Code { get; set; }

        /// <summary>
        /// Gets or sets the state name.
        /// </summary>
        [MaxLength(50)]
        public string? Name { get; set; }
    }
}

