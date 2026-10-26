using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    public class RequesterLedgerDto : BaseDto, IDepartmentScoped
    {
        public RequesterLedgerDto()
        {
        }

        public Guid RequesterLedgerID { get; set; }

        public Guid RequesterID { get; set; }

        public Guid DepartmentID { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime TransactionDateTime { get; set; }

        public decimal OpeningBalance { get; set; }

        public decimal Amount { get; set; }

        public RequesterLedgerTransactionType TransactionType { get; set; }

        public decimal Balance { get; set; }

        public RequesterLedgerSourceType SourceType { get; set; }

        public Guid? SourceID { get; set; }

        public string? SourceNumber { get; set; }

        public string? RequesterName { get; set; }

        /// <summary>
        /// Optional payment type display name for payment/refund ledger descriptions (not a DB column).
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string? PaymentTypeName { get; set; }

        public RequesterLedgerDto(
            Guid requesterId,
            Guid departmentId,
            decimal amount,
            RequesterLedgerTransactionType transactionType,
            RequesterLedgerSourceType sourceType,
            Guid? sourceId,
            string? sourceNumber,
            DateTime transactionDateTime,
            string? requesterName = null)
        {
            RequesterID = requesterId;
            DepartmentID = departmentId;
            Amount = amount;
            TransactionType = transactionType;
            SourceType = sourceType;
            SourceID = sourceId;
            SourceNumber = sourceNumber;
            TransactionDateTime = transactionDateTime;
            RequesterName = requesterName;
        }

        /// <summary>
        /// Ledger ordering time from audit fields (not business dates like <c>PaymentDate</c> or invoice <c>DateCreated</c>).
        /// </summary>
        public static DateTime TransactionDateTimeFrom(BaseDto source) =>
            source.EditedDate ?? source.CreatedDate ?? DateTime.UtcNow;

        /// <summary>When the source row is being removed.</summary>
        public static DateTime TransactionDateTimeForDelete(BaseDto? source = null) =>
            source?.DeletedAt ?? DateTime.UtcNow;

        /// <summary>
        /// Sets opening balance, running balance, and description for a new invoice, payment, or refund ledger row.
        /// </summary>
        public RequesterLedgerDto ApplySourceTransaction(decimal previousBalance, string? requesterName)
        {
            OpeningBalance = previousBalance;
            Balance = GetBalanceAfterTransaction(previousBalance, Amount, TransactionType);
            Description = FormatDescriptionFromSource(SourceType, SourceNumber, SourceID, requesterName, TransactionType, PaymentTypeName);
            return this;
        }

        public static decimal GetBalanceAfterTransaction(
            decimal openingBalance,
            decimal amount,
            RequesterLedgerTransactionType transactionType) =>
            transactionType == RequesterLedgerTransactionType.Credit
                ? openingBalance + amount
                : openingBalance - amount;

        public static string FormatDescriptionFromSource(
            RequesterLedgerSourceType sourceType,
            string? sourceNumber,
            Guid? sourceId,
            string? requesterName,
            RequesterLedgerTransactionType transactionType,
            string? paymentTypeName = null)
        {
            var documentNumber = string.IsNullOrWhiteSpace(sourceNumber)
                ? sourceId?.ToString() ?? "-"
                : sourceNumber.Trim();

            var requester = requesterName?.Trim() ?? "-";
            var prefix = $"{sourceType.ToString().ToUpperInvariant()}/{documentNumber}/{requester}";

            if (!string.IsNullOrWhiteSpace(paymentTypeName))
                return $"{prefix}/{paymentTypeName.Trim()}/{transactionType}";

            return $"{prefix}/{transactionType}";
        }

        public static decimal SumDebitAmount(IEnumerable<RequesterLedgerDto>? entries) =>
            Sum(entries, e => e.TransactionType == RequesterLedgerTransactionType.Debit ? e.Amount : 0m);

        public static decimal SumCreditAmount(IEnumerable<RequesterLedgerDto>? entries) =>
            Sum(entries, e => e.TransactionType == RequesterLedgerTransactionType.Credit ? e.Amount : 0m);

        public static RequesterLedgerDto? GetLastEntryBefore(IEnumerable<RequesterLedgerDto> entries, DateTime periodStart) =>
            entries
                .Where(e => e.TransactionDateTime < periodStart)
                .OrderByDescending(e => e.TransactionDateTime)
                .ThenByDescending(e => e.CreatedDate)
                .FirstOrDefault();

        public static List<RequesterLedgerDto> GetEntriesInPeriod(
            IEnumerable<RequesterLedgerDto> entries,
            DateTime? periodStart,
            DateTime? periodEnd) =>
            entries
                .Where(e =>
                    (!periodStart.HasValue || e.TransactionDateTime >= periodStart.Value)
                    && (!periodEnd.HasValue || e.TransactionDateTime <= periodEnd.Value))
                .OrderBy(e => e.TransactionDateTime)
                .ThenBy(e => e.CreatedDate)
                .ToList();
    }

    /// <summary>
    /// Bank-statement summary for a requester ledger period. Requester details come from <see cref="RequesterLedgerStatementDto.Entries"/>.
    /// </summary>
    public class RequesterLedgerStatementDto
    {
        public RequesterLedgerStatementDto(
            DateTime? periodStart,
            DateTime? periodEnd,
            IReadOnlyList<RequesterLedgerDto> statementEntries,
            PageParam pageParam)
        {
            var entryBeforePeriod = periodStart.HasValue
                ? RequesterLedgerDto.GetLastEntryBefore(statementEntries, periodStart.Value)
                : null;
            var periodEntries = RequesterLedgerDto.GetEntriesInPeriod(statementEntries, periodStart, periodEnd);

            PeriodStart = periodStart;
            PeriodEnd = periodEnd;
            OpeningBalance = entryBeforePeriod?.Balance ?? periodEntries.FirstOrDefault()?.OpeningBalance ?? 0m;
            TotalInvoices = RequesterLedgerDto.SumDebitAmount(periodEntries);
            TotalPayment = RequesterLedgerDto.SumCreditAmount(periodEntries);
            OutstandingInvoices = periodEntries.LastOrDefault()?.Balance ?? OpeningBalance;

            var offset = pageParam.Offset ?? 0;
            var limit = pageParam.Limit ?? periodEntries.Count;
            var pagedEntries = periodEntries.Skip(offset).Take(limit).ToList();
            Entries = new PagedList<RequesterLedgerDto>(pagedEntries, pageParam, periodEntries.Count);
        }

        public DateTime? PeriodStart { get; set; }

        public DateTime? PeriodEnd { get; set; }

        public decimal OpeningBalance { get; set; }

        public decimal TotalInvoices { get; set; }

        public decimal TotalPayment { get; set; }

        public decimal OutstandingInvoices { get; set; }

        public PagedList<RequesterLedgerDto> Entries { get; set; } = new PagedList<RequesterLedgerDto>();
    }
}
