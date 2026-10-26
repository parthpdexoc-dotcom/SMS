namespace SMS.Generic.Enums
{
    /// <summary>
    /// Source document that produced a ledger entry. Allocations are intentionally excluded.
    /// </summary>
    public enum RequesterLedgerSourceType
    {
        Invoice = 1,
        Payment = 2,
        Refund = 3
    }
}
