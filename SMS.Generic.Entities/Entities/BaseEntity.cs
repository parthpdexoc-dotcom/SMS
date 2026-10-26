using System.ComponentModel.DataAnnotations;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    public class BaseEntity
    {
        [AuditedProperty("Is deleted")]
        public bool IsDeleted { get; set; }
        [AuditedProperty("Deleted at")]
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedUserID { get; set; }

        [MaxLength(255)]
        [AuditedProperty("Deleted by")]
        public string? DeletedBy { get; set; }

        /// <summary>
        /// Record added timestamp.
        /// </summary>
        [AuditedProperty("Created date/time")]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// User who added record.
        /// </summary>
        public Guid? CreatedUserID { get; set; }

        /// <summary>
        /// Name of user who added record.
        /// </summary>
        [MaxLength(255)]
        [AuditedProperty("Created by employee")]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Edit timestamp.
        /// </summary>
        [AuditedProperty("Edited date/time")]
        public DateTime? EditedDate { get; set; }

        /// <summary>
        /// User who edited record.
        /// </summary>
        public Guid? EditedUserID { get; set; }

        /// <summary>
        /// Name of user editing record.
        /// </summary>
        [MaxLength(255)]
        [AuditedProperty("Edited by employee")]
        public string? EditedBy { get; set; }
    }
}
