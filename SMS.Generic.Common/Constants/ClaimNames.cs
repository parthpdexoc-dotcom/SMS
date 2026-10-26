namespace SMS.Generic.Common.Constants
{
    /// <summary>
    /// Constants for JWT claim names used throughout the application.
    /// </summary>
    public static class ClaimNames
    {
        /// <summary>
        /// Claim name for user type (Admin or Employee).
        /// </summary>
        public const string UserType = "UserType";

        /// <summary>
        /// Claim name for admin identifier.
        /// </summary>
        public const string AdminID = "AdminID";

        /// <summary>
        /// Claim name for employee identifier.
        /// </summary>
        public const string EmployeeID = "EmployeeID";

        /// <summary>
        /// Claim name for username.
        /// </summary>
        public const string Username = "Username";

        /// <summary>
        /// Claim name for first name.
        /// </summary>
        public const string FirstName = "FirstName";

        /// <summary>
        /// Claim name for last name.
        /// </summary>
        public const string LastName = "LastName";

        /// <summary>
        /// Claim name for full name.
        /// </summary>
        public const string FullName = "FullName";

        /// <summary>
        /// Claim name for employee primary email.
        /// </summary>
        public const string PrimaryEmail = "PrimaryEmail";

        /// <summary>
        /// Claim name for department identifier.
        /// </summary>
        public const string DepartmentID = "DepartmentID";

        /// <summary>
        /// Claim name for department name.
        /// </summary>
        public const string DepartmentName = "DepartmentName";

        /// <summary>
        /// Claim name for department code.
        /// </summary>
        public const string DepartmentCode = "DepartmentCode";
        /// <summary>
        /// Claim name for department timezone.
        /// </summary>
        public const string TimeZoneId = "TimeZoneId";
        /// <summary>
        /// Claim name for DepartmentPolicyConfiguration.
        /// </summary>
        public const string DepartmentPolicyConfiguration = "DepartmentPolicyConfiguration";

        /// <summary>
        /// Claim name: mirrors DepartmentConfiguration.EnableOpenAgencyWaitLists (issued on employee login).
        /// </summary>
        public const string EnableOpenWaitingList = "EnableOpenWaitingList";

        /// <summary>Mirrors DepartmentConfiguration.AllowEmployeeCancellation.</summary>
        public const string AllowEmployeeCancellation = "AllowEmployeeCancellation";

        /// <summary>Mirrors DepartmentConfiguration.EnableEmployeeSplittingPosts.</summary>
        public const string EnableEmployeeSplittingPosts = "EnableEmployeeSplittingPosts";

        /// <summary>Mirrors DepartmentConfiguration.EnablePolicyManagement.</summary>
        public const string EnablePolicyManagement = "EnablePolicyManagement";

        /// <summary>Mirrors DepartmentConfiguration.UseAgencyDefinedFields.</summary>
        public const string UseAgencyDefinedFields = "UseAgencyDefinedFields";

        /// <summary>Mirrors DepartmentConfiguration.EnableEmployeeLocation.</summary>
        public const string EnableEmployeeLocation = "EnableEmployeeLocation";

        /// <summary>
        /// Employee JWT: Interval in seconds (e.g. 30).
        /// </summary>
        public const string Interval = "Interval";

        /// <summary>
        /// Claim name that indicates if current user satisfies DepartmentAdmin policy scope.
        /// True for DepartmentAdmin and SuperUser.
        /// </summary>
        public const string IsDepartmentAdminPolicy = "IsDepartmentAdminPolicy";

        /// <summary>
        /// Claim name for badge number.
        /// </summary>
        public const string BadgeNumber = "BadgeNumber";

        /// <summary>
        /// Claim name for employee type identifier.
        /// </summary>
        public const string EmployeeTypeID = "EmployeeTypeID";

        /// <summary>
        /// Claim name for employee type IsSupervisor.
        /// </summary>
        public const string IsSupervisor = "IsSupervisor";

        /// <summary>
        /// Claim name for employee type short name.
        /// </summary>
        public const string EmployeeTypeShort = "EmployeeTypeShort";

        /// <summary>
        /// Claim name for employee type long name.
        /// </summary>
        public const string EmployeeTypeLong = "EmployeeTypeLong";

        /// <summary>
        /// Claim name for status identifier.
        /// </summary>
        public const string StatusID = "StatusID";

        /// <summary>
        /// Claim name for status short name.
        /// </summary>
        public const string StatusShort = "StatusShort";

        /// <summary>
        /// Claim name for status long name.
        /// </summary>
        public const string StatusLong = "StatusLong";

        /// <summary>
        /// Claim name for admin types (JSON array).
        /// </summary>
        public const string AdminTypes = "AdminTypes";

        /// <summary>
        /// Claim name for admin type identifiers (JSON array).
        /// </summary>
        public const string AdminTypeIDs = "AdminTypeIDs";

        /// <summary>
        /// Claim name for admin type identifier.
        /// </summary>
        public const string AdminTypeID = "AdminTypeID";

        /// <summary>
        /// Claim name for admin type short name.
        /// </summary>
        public const string AdminTypeShort = "AdminTypeShort";

        /// <summary>
        /// Claim name for admin type long name.
        /// </summary>
        public const string AdminTypeLong = "AdminTypeLong";

        /// <summary>
        /// Claim name for status (boolean).
        /// </summary>
        public const string Status = "Status";

        /// <summary>
        /// Claim name for IsSuperUser (bit/boolean).
        /// </summary>
        public const string IsSuperUser = "IsSuperUser";

        /// <summary>
        /// Claim name indicating the token was issued via impersonation flow.
        /// </summary>
        public const string IsImpersonation = "IsImpersonation";

        /// <summary>
        /// Claim name containing the identifier of the superuser who initiated impersonation.
        /// </summary>
        public const string ImpersonatedBy = "ImpersonatedBy";

        /// <summary>
        /// Claim name for role used in JWT payload and must match RoleClaimType in JWT bearer options ("role").
        /// </summary>
        public const string Role = "role";

        /// <summary>
        /// Employee JWT: <see cref="SMS.Generic.Enums.AvailablePostReleaseTabTier"/> as integer (0–3); 0 = no employee context / do not use for tabs.
        /// </summary>
        public const string ReleaseAsTabTier = "ReleaseAsTabTier";

        /// <summary>
        /// Employee JWT: whether a profile photo file exists for the employee (<c>true</c> / <c>false</c>).
        /// </summary>
        public const string IsLogo = "isLogo";

        /// <summary>
        /// Decrypted login client value sent by the web app.
        /// </summary>
        public const string WebLoginClient = "web";

        public const string SessionId = "sid";
    }
}

