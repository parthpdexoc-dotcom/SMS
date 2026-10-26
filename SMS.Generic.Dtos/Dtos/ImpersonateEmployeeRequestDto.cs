namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Request payload for superuser employee impersonation.
    /// </summary>
    public class ImpersonateEmployeeRequestDto
    {
        /// <summary>
        /// Employee identifier to impersonate.
        /// </summary>
        public string EmployeeId { get; set; } = string.Empty;
    }
}
