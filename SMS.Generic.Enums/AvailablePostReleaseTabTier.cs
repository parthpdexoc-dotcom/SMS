namespace SMS.Generic.Enums
{
    /// <summary>
    /// Which Available Post "Release as" tab set applies to the current user (employee).
    /// Aligns with the SPA logic in <c>resolveAvailablePostReleaseAccess</c>.
    /// </summary>
    public enum AvailablePostReleaseTabTier
    {
        /// <summary>
        /// Value 0: No employee ID on the token; do not drive Available Post release tabs from this claim.
        /// </summary>
        None = 0,

        /// <summary>
        /// Value 1: Department admin / SuperUser policy scope, or user is a requester manager,
        /// or user is a requester employee with at least one preferred mapping.
        /// </summary>
        FullManagedPreferred = 1,

        /// <summary>
        /// Value 2: User is mapped as a requester employee but has no preferred mapping.
        /// </summary>
        RequesterEmployeeNonPreferred = 2,

        /// <summary>
        /// Value 3: No requester manager or requester employee context (plain employee).
        /// </summary>
        RegularEmployee = 3
    }
}
