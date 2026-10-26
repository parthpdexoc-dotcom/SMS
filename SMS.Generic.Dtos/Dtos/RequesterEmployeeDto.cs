using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// DTO for RequesterEmployee. EmployeeID is provided manually in the request (not taken from token).
    /// </summary>
    public class RequesterEmployeeDto : BaseDto
    {
        public Guid? RequesterEmployeeID { get; set; }
        public Guid RequesterManagerID { get; set; }
        public Guid EmployeeID { get; set; }
        public bool IsPreferred { get; set; }
        public EmployeeDto? Employee { get; set; }

        /// <summary>
        /// Returns true when this requester-employee row maps to the provided employee id.
        /// </summary>
        public bool IsEmployee(Guid employeeId) => EmployeeID == employeeId;

        /// <summary>
        /// Returns true when this employee row is eligible for the provided release type.
        /// Preferred requires IsPreferred=true, other scoped releases only require membership.
        /// </summary>
        public bool IsEligibleForReleaseAs(Guid employeeId, ReleaseAs releaseAs)
        {
            if (!IsEmployee(employeeId))
                return false;

            return releaseAs switch
            {
                ReleaseAs.Preferred => IsPreferred,
                ReleaseAs.Managed => true,
                ReleaseAs.TenureBased => true,
                _ => false
            };
        }
    }
    public class RequestEmployeeResponseDto : RequesterEmployeeDto
    {
        public string? EmployeeName { get; set; }
    }
}