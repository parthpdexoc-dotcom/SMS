using System.Collections.Generic;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Response DTO for routing group details including employee first names mapped from EmployeeIDs.
    /// </summary>
    public class RoutingGroupDetailsDto : RoutingGroupDto
    {
        /// <summary>
        /// First names of employees corresponding to EmployeeIDs (from employee service), in same order.
        /// </summary>
        public List<string>? EmployeeFirstNames { get; set; }
    }
}
