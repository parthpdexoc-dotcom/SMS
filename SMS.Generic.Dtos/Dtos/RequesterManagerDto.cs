using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Entities.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for RequesterManager operations.
    /// </summary>
    public class RequesterManagerDto : BaseDto
    {
        public Guid? RequesterManagerID { get; set; }
        public Guid? RequesterID { get; set; }
        public Guid EmployeeID { get; set; }
        public bool IsEmployeeShared { get; set; }
        public bool IsRequestShared { get; set; }
        public List<RequesterEmployeeDto> RequesterEmployees { get; set; } = new();
        public EmployeeDto? Employee { get; set; }

        /// <summary>
        /// Returns true when this requester-manager row matches the provided manager id.
        /// </summary>
        public bool IsManager(Guid managerId) => RequesterManagerID == managerId;

        /// <summary>
        /// Returns true when the provided employee is this manager employee.
        /// </summary>
        public bool IsManagerEmployee(Guid employeeId) => EmployeeID == employeeId;

        /// <summary>
        /// Returns true when the provided employee is under this manager's employee list.
        /// </summary>
        public bool HasEmployee(Guid employeeId)
            => RequesterEmployees?.Any(e => e.IsEmployee(employeeId)) == true;

        /// <summary>
        /// Returns true when the employee belongs to this manager row
        /// (manager user itself or one of its requester employees).
        /// </summary>
        public bool ContainsEmployee(Guid employeeId) => IsManagerEmployee(employeeId) || HasEmployee(employeeId);

        /// <summary>
        /// Returns true when this manager row allows the employee for the given release type.
        /// Managers are always eligible for scoped release checks once visible.
        /// </summary>
        public bool IsEmployeeEligibleForReleaseAs(Guid employeeId, SMS.Generic.Enums.ReleaseAs releaseAs)
        {
            if (IsManagerEmployee(employeeId))
                return true;

            return RequesterEmployees?.Any(e => e.IsEligibleForReleaseAs(employeeId, releaseAs)) == true;
        }
    }
    public class RequestManagerResponceDto : RequesterManagerDto
    {
        public string ? EmployeeName { get; set; }
    }
}


