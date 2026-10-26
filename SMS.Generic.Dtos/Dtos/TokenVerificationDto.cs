namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for JWT token verification response containing all decoded claims.
    /// </summary>
    public class TokenVerificationDto
    {
        /// <summary>
        /// Gets or sets a value indicating whether the token is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the error message if token is invalid.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the token expiration date and time.
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets the token issued at date and time.
        /// </summary>
        public DateTime? IssuedAt { get; set; }

        /// <summary>
        /// Gets or sets the token issuer.
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// Gets or sets the token audience.
        /// </summary>
        public string? Audience { get; set; }

        /// <summary>
        /// Gets or sets all claims from the token as a dictionary.
        /// </summary>
        public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the employee ID from the token.
        /// </summary>
        public int? EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the username from the token.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the badge number from the token.
        /// </summary>
        public string? BadgeNumber { get; set; }

        /// <summary>
        /// Gets or sets the full name from the token.
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Gets or sets the first name from the token.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name from the token.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the department ID from the token.
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the department name from the token.
        /// </summary>
        public string? DepartmentName { get; set; }

        /// <summary>
        /// Gets or sets the department code from the token.
        /// </summary>
        public string? DepartmentCode { get; set; }
        /// <summary>
        /// Gets or sets the department timezone from the token.
        /// </summary>
        public string? TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets the employee type ID from the token.
        /// </summary>
        public int? EmployeeTypeID { get; set; }

        /// <summary>
        /// Gets or sets the employee type short from the token.
        /// </summary>
        public string? EmployeeTypeShort { get; set; }

        /// <summary>
        /// Gets or sets the employee type long from the token.
        /// </summary>
        public string? EmployeeTypeLong { get; set; }

        /// <summary>
        /// Gets or sets the status ID from the token.
        /// </summary>
        public int? StatusID { get; set; }

        /// <summary>
        /// Gets or sets the status short from the token.
        /// </summary>
        public string? StatusShort { get; set; }

        /// <summary>
        /// Gets or sets the status long from the token.
        /// </summary>
        public string? StatusLong { get; set; }

        /// <summary>
        /// Gets or sets the admin types from the token.
        /// </summary>
        public List<string>? AdminTypes { get; set; }

        /// <summary>
        /// Gets or sets the admin type IDs from the token.
        /// </summary>
        public List<int>? AdminTypeIDs { get; set; }

        /// <summary>
        /// Gets or sets the roles from the token.
        /// </summary>
        public List<string>? Roles { get; set; }
    }
}
