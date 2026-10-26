namespace SMS.Generic.Enums
{
    public enum FinancialImpactType
    {
        None = 0,
        Debit = 1,
        Credit = 2,
        /// <summary>Manual invoice line edit from the UI (PUT).</summary>
        Modified = 3
    }
}
