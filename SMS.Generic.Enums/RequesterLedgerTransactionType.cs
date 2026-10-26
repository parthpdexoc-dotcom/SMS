namespace SMS.Generic.Enums
{
    /// <summary>
    /// Debit or credit direction for a requester ledger row (bank-statement style).
    /// Credit increases running balance; debit decreases it.
    /// </summary>
    public enum RequesterLedgerTransactionType
    {
        Debit = 1,
        Credit = 2
    }
}
