namespace SMS.Generic.Enums
{
    /// <summary>
    /// Type of approval (used in ApprovalDetails table).
    /// </summary>
    public enum ApprovalType
    {
        /// <summary>
        /// Special event approval.
        /// </summary>
        SpecialEventApproval = 0,

        /// <summary>
        /// Vehicle approval.
        /// </summary>
        VehicleApproval = 1,

        /// <summary>
        /// Permit approval.
        /// </summary>
        PremitApproval = 2
    }
}
