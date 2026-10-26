namespace SMS.Generic.Common.Helpers
{
    /// <summary>
    /// Settings for Vehicle Approval email (link base URL). Bind from config section "VehicleApproval".
    /// </summary>
    public class VehicleApprovalEmailSettings
    {
        public const string SectionName = "VehicleApproval";
        /// <summary>
        /// Base URL of the application (e.g. https://yourapp.com) used to build the full Vehicle Approval link in emails.
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;
    }
}
