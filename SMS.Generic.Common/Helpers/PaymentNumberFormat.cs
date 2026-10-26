namespace SMS.Generic.Common.Helpers
{
    /// <summary>
    /// Payment number display/storage format (e.g. P-202600002).
    /// </summary>
    public static class PaymentNumberFormat
    {
        public const string Prefix = "P-";
        public const string RefundPrefix = "R-";

        public static string Format(int paymentNumber) =>
            paymentNumber > 0 ? $"{Prefix}{paymentNumber}" : string.Empty;

        public static string FormatRefund(int refundPaymentNumber) =>
            refundPaymentNumber > 0 ? $"{RefundPrefix}{refundPaymentNumber}" : string.Empty;

        public static int Parse(string? paymentNumber) => ParseWithPrefix(paymentNumber, Prefix);

        public static int ParseRefund(string? refundPaymentNumber) => ParseWithPrefix(refundPaymentNumber, RefundPrefix);

        /// <summary>
        /// Prefix used to find existing numbers for a fiscal year (e.g. P-2026 matches P-202600001).
        /// </summary>
        public static string FiscalYearSearchPrefix(int fiscalYear) => $"{Prefix}{fiscalYear}";

        public static string RefundFiscalYearSearchPrefix(int fiscalYear) => $"{RefundPrefix}{fiscalYear}";

        private static int ParseWithPrefix(string? paymentNumber, string prefix)
        {
            if (string.IsNullOrWhiteSpace(paymentNumber))
                return 0;

            var value = paymentNumber.Trim();
            if (value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                value = value[prefix.Length..];

            return int.TryParse(value, out var numeric) ? numeric : 0;
        }
    }
}
