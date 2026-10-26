namespace SMS.Generic.Common.Helpers
{
    /// <summary>
    /// Invoice number format: fiscal year + project number prefix, with optional sequence suffix (e.g. 202600001, 20260000102).
    /// </summary>
    public static class InvoiceNumberFormat
    {
        public const int PrefixLength = 9;

        public static string GetPrefix(string? invoiceNo)
        {
            if (string.IsNullOrWhiteSpace(invoiceNo))
                return string.Empty;

            var trimmed = invoiceNo.Trim();
            return trimmed.Length <= PrefixLength ? trimmed : trimmed[..PrefixLength];
        }

        public static string FormatNext(string prefix, IEnumerable<string?> existingInvoiceNumbers)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                return prefix;

            var maxSeq = 0;
            foreach (var invoiceNo in existingInvoiceNumbers)
            {
                if (string.IsNullOrWhiteSpace(invoiceNo))
                    continue;

                var value = invoiceNo.Trim();
                if (!value.StartsWith(prefix, StringComparison.Ordinal))
                    continue;

                if (value.Length == prefix.Length)
                    maxSeq = Math.Max(maxSeq, 1);
                else if (value.Length > prefix.Length && int.TryParse(value[prefix.Length..], out var seq))
                    maxSeq = Math.Max(maxSeq, seq);
            }

            var next = maxSeq + 1;
            return next == 1 ? prefix : $"{prefix}{next:D2}";
        }
    }
}
