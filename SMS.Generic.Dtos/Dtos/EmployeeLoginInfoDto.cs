using System;
using System.Collections.Generic;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object containing all employee data needed for JWT token generation during login.
    /// </summary>
    public class EmployeeLoginInfoDto
    {
        // Employee Info
        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the human-readable employee code (e.g. 000001).
        /// </summary>
        public string? EmployeeCode { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the badge number.
        /// </summary>
        public string? BadgeNumber { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the password (for validation only).
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the primary email address.
        /// </summary>
        public string? PrimaryEmail { get; set; }

        /// <summary>
        /// Gets or sets the secondary email address.
        /// </summary>
        public string? SecondaryEmail { get; set; }

        // Department Info
        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the department name.
        /// </summary>
        public string? DepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the department code.
        /// </summary>
        public string? DepartmentCode { get; set; }

        // EmployeeType Info
        /// <summary>
        /// Gets or sets the employee type identifier.
        /// </summary>
        public Guid? EmployeeTypeID { get; set; }

        /// <summary>
        /// Gets or sets the employee type short name.
        /// </summary>
        public string? EmployeeTypeShort { get; set; }

        /// <summary>
        /// Gets or sets the employee type long name.
        /// </summary>
        public string? EmployeeTypeLong { get; set; }

        /// <summary>
        /// Gets or sets whether the employee type is for system users (Master). These cannot login via UI.
        /// </summary>
        public bool IsSystemUser { get; set; }

        /// <summary>
        /// Gets or sets whether the employee type is a supervisor role.
        /// </summary>
        public bool IsSupervisor { get; set; }

        // EmployeeStatus Info
        /// <summary>
        /// Gets or sets the status identifier (optional).
        /// </summary>
        public Guid? StatusID { get; set; }

        /// <summary>
        /// Gets or sets the status short name.
        /// </summary>
        public string? StatusShort { get; set; }

        /// <summary>
        /// Gets or sets the status long name.
        /// </summary>
        public string? StatusLong { get; set; }

        /// <summary>
        /// Gets or sets whether the employee is approved for login.
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// Gets or sets whether the employee has completed a password reset flow.
        /// </summary>
        public bool IsPasswordReset { get; set; }

        // AdminTypes (List of roles)
        /// <summary>
        /// Gets or sets the list of admin types (roles) associated with the employee.
        /// </summary>
        public List<AdminTypeInfoDto> AdminTypes { get; set; } = new();

        /// <summary>
        /// True when the employee has an active Suspended skill assignment for current UTC date (used for JWT isSuspended role at login).
        /// </summary>
        public bool HasActiveSuspendedPeriod { get; set; }

        /// <summary>
        /// True when this department requires an admin type for web login.
        /// </summary>
        public bool RestrictWebLoginForNonAdminEmployee { get; set; }

        /// <summary>
        /// True when this department allows only one active login for a normal employee.
        /// </summary>
        public bool EnableNormalEmployeeOneSession { get; set; }
    }
}

