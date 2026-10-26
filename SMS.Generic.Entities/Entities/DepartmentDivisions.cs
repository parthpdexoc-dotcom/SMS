using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    [AuditedEntity(DisplayPropertyName = nameof(DivisionShort), OnlyAttributedProperties = true)]
    [Table("DepartmentDivisions", Schema = "dbo")]
    public class DepartmentDivisions:BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // because NEWID() is in SQL, not EF
        public Guid DivisionID { get; set; }

        [Required]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }

        [MaxLength(50)]
        [AuditedProperty("Title")]
        public string? DivisionShort { get; set; }

        [AuditedProperty("Active")]
        public bool? DivisionActive { get; set; }

        [MaxLength(200)]
        [AuditedProperty("Description")]
        public string? DivisionLong { get; set; }


        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }


        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedUser { get; set; }


        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedUser { get; set; }

    }
}
