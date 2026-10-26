using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMS.Generic.Common.Audit;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    [AuditedEntity(DisplayPropertyName = nameof(ApproverShort), OnlyAttributedProperties = true)]
    [Table("RoutingGroup", Schema = "dbo")]
    public class RoutingGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RoutingGroupId { get; set; }

        [Required]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Required. Each department can have only one routing group per approval type (e.g. one SpecialEventApproval, one VehicleApproval).
        /// </summary>
        [AuditedProperty("Approval type")]
        public ApprovalType ApprovalType { get; set; }

        // Stores employee IDs as comma-separated or JSON string
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        [AuditedProperty("Employees", GetEmployeeNamesByIds = true)]
        public List<Guid> EmployeeIDs { get; set; }

        [MaxLength(200)]
        [AuditedProperty("Description")]
        public string? ApproverLong { get; set; }

        [MaxLength(50)]
        [AuditedProperty("Title")]
        public string? ApproverShort { get; set; }

       
        [MaxLength(150)]
        [AuditedProperty("Email")]
        public string? ApproverEmail { get; set; }

        [AuditedProperty("Active")]
        public bool? ApproverStatus { get; set; }


        // Navigation property
        [ForeignKey(nameof(DepartmentID))]
        public Department Department { get; set; }
    }
}
