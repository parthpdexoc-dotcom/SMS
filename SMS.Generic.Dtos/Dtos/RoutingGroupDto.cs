using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Generic.Dtos.Dtos
{
    public class RoutingGroupDto : IDepartmentScoped
    {
        public Guid? RoutingGroupId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Required. Each department can have only one routing group per approval type.
        /// </summary>
        public ApprovalType ApprovalType { get; set; }

        public List<Guid> EmployeeIDs { get; set; }

        public string? ApproverLong { get; set; }

        public string? ApproverShort { get; set; }

        public string ApproverEmail { get; set; }
        public bool ApproverStatus { get; set; }

    }
}
