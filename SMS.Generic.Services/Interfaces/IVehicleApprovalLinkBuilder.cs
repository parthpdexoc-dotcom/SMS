namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Provides the base URL for building Vehicle Approval links in emails.
    /// </summary>
    public interface IVehicleApprovalLinkBuilder
    {
        /// <summary>
        /// Gets the application base URL (e.g. https://yourapp.com) used to build full links.
        /// </summary>
        string GetBaseUrl();
    }
}
