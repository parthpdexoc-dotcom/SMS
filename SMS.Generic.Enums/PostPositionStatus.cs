namespace SMS.Generic.Enums
{
    public enum PostPositionStatus
    {
        Assigned = 1,
        Closed = 2,
        Cancelled = 3,
        ClosedAndSplit = 4,
        SplitDecisionPending = 5,
        Declined = 6,
        Waiting = 10,
        Unclosed = 11,
        Transferred = 13,
        CancellationPending = 14,
        /// <summary>
        /// Employee applied to a special-event request (dynamic post, no concrete post id yet).
        /// </summary>
        SpecialEvent = 15
    }
}
