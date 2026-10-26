using Microsoft.Extensions.Options;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Services.Interfaces;

namespace SMS.Services
{
    /// <summary>
    /// Reads Vehicle Approval base URL from configuration.
    /// </summary>
    public class VehicleApprovalLinkBuilder : IVehicleApprovalLinkBuilder
    {
        private readonly VehicleApprovalEmailSettings _settings;

        public VehicleApprovalLinkBuilder(IOptions<VehicleApprovalEmailSettings> options)
        {
            _settings = options?.Value ?? new VehicleApprovalEmailSettings();
        }

        /// <inheritdoc />
        public string GetBaseUrl()
        {
            return string.IsNullOrWhiteSpace(_settings.BaseUrl) ? string.Empty : _settings.BaseUrl.TrimEnd('/');
        }
    }
}
