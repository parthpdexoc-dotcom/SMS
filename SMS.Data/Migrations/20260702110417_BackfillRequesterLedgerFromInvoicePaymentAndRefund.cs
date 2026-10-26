using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class BackfillRequesterLedgerFromInvoicePaymentAndRefund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @EmptyGuid uniqueidentifier = '00000000-0000-0000-0000-000000000000';

;WITH SourceTransactions AS
(
    SELECT
        i.[InvoicesID] AS [SourceID],
        1 AS [SourceType],
        i.[RequesterID],
        COALESCE(i.[DepartmentID], r.[DepartmentID]) AS [DepartmentID],
        ABS(ISNULL(i.[Total], 0)) AS [Amount],
        CASE WHEN ISNULL(i.[Total], 0) > 0 THEN 1 ELSE 2 END AS [TransactionType],
        COALESCE(i.[EditedDate], i.[CreatedDate], SYSUTCDATETIME()) AS [TransactionDateTime],
        LEFT(COALESCE(NULLIF(LTRIM(RTRIM(i.[InvoiceNo])), N''), CONVERT(nvarchar(36), i.[InvoicesID])), 30) AS [SourceNumber],
        LTRIM(RTRIM(COALESCE(r.[Name], N'-'))) AS [RequesterName],
        i.[CreatedDate],
        i.[CreatedUserID],
        i.[CreatedBy],
        i.[EditedDate],
        i.[EditedUserID],
        i.[EditedBy]
    FROM [dbo].[Invoices] i
    LEFT JOIN [dbo].[Requester] r ON r.[RequesterID] = i.[RequesterID]
    WHERE i.[IsDeleted] = 0
      AND i.[RequesterID] IS NOT NULL
      AND i.[RequesterID] <> @EmptyGuid
      AND COALESCE(i.[DepartmentID], r.[DepartmentID]) IS NOT NULL
      AND COALESCE(i.[DepartmentID], r.[DepartmentID]) <> @EmptyGuid
      AND ISNULL(i.[Total], 0) <> 0
      AND NOT EXISTS
      (
          SELECT 1
          FROM [dbo].[RequesterLedgers] rl
          WHERE rl.[IsDeleted] = 0
            AND rl.[SourceType] = 1
            AND rl.[SourceID] = i.[InvoicesID]
      )

    UNION ALL

    SELECT
        p.[PaymentID] AS [SourceID],
        2 AS [SourceType],
        p.[RequesterID],
        p.[DepartmentID],
        ABS(p.[Amount]) AS [Amount],
        CASE WHEN p.[Amount] > 0 THEN 2 ELSE 1 END AS [TransactionType],
        COALESCE(p.[EditedDate], p.[CreatedDate], SYSUTCDATETIME()) AS [TransactionDateTime],
        LEFT(COALESCE(NULLIF(LTRIM(RTRIM(p.[PaymentNumber])), N''), CONVERT(nvarchar(36), p.[PaymentID])), 30) AS [SourceNumber],
        LTRIM(RTRIM(COALESCE(r.[Name], N'-'))) AS [RequesterName],
        p.[CreatedDate],
        p.[CreatedUserID],
        p.[CreatedBy],
        p.[EditedDate],
        p.[EditedUserID],
        p.[EditedBy]
    FROM [dbo].[Payments] p
    LEFT JOIN [dbo].[Requester] r ON r.[RequesterID] = p.[RequesterID]
    WHERE p.[IsDeleted] = 0
      AND p.[RequesterID] <> @EmptyGuid
      AND p.[DepartmentID] <> @EmptyGuid
      AND p.[Amount] <> 0
      AND NOT EXISTS
      (
          SELECT 1
          FROM [dbo].[RequesterLedgers] rl
          WHERE rl.[IsDeleted] = 0
            AND rl.[SourceType] = 2
            AND rl.[SourceID] = p.[PaymentID]
      )

    UNION ALL

    SELECT
        rp.[RefundPaymentID] AS [SourceID],
        3 AS [SourceType],
        rp.[RequesterID],
        rp.[DepartmentID],
        ABS(rp.[Amount]) AS [Amount],
        CASE WHEN rp.[Amount] > 0 THEN 1 ELSE 2 END AS [TransactionType],
        COALESCE(rp.[EditedDate], rp.[CreatedDate], SYSUTCDATETIME()) AS [TransactionDateTime],
        LEFT(COALESCE(NULLIF(LTRIM(RTRIM(rp.[RefundPaymentNumber])), N''), CONVERT(nvarchar(36), rp.[RefundPaymentID])), 30) AS [SourceNumber],
        LTRIM(RTRIM(COALESCE(r.[Name], N'-'))) AS [RequesterName],
        rp.[CreatedDate],
        rp.[CreatedUserID],
        rp.[CreatedBy],
        rp.[EditedDate],
        rp.[EditedUserID],
        rp.[EditedBy]
    FROM [dbo].[RefundPayments] rp
    LEFT JOIN [dbo].[Requester] r ON r.[RequesterID] = rp.[RequesterID]
    WHERE rp.[IsDeleted] = 0
      AND rp.[RequesterID] <> @EmptyGuid
      AND rp.[DepartmentID] <> @EmptyGuid
      AND rp.[Amount] <> 0
      AND NOT EXISTS
      (
          SELECT 1
          FROM [dbo].[RequesterLedgers] rl
          WHERE rl.[IsDeleted] = 0
            AND rl.[SourceType] = 3
            AND rl.[SourceID] = rp.[RefundPaymentID]
      )
)
INSERT INTO [dbo].[RequesterLedgers]
(
    [RequesterLedgerID],
    [RequesterID],
    [DepartmentID],
    [Description],
    [TransactionDateTime],
    [OpeningBalance],
    [Amount],
    [TransactionType],
    [Balance],
    [SourceType],
    [SourceID],
    [SourceNumber],
    [IsDeleted],
    [CreatedDate],
    [CreatedUserID],
    [CreatedBy],
    [EditedDate],
    [EditedUserID],
    [EditedBy]
)
SELECT
    NEWID() AS [RequesterLedgerID],
    s.[RequesterID],
    s.[DepartmentID],
    CONCAT
    (
        CASE s.[SourceType]
            WHEN 1 THEN N'INVOICE'
            WHEN 2 THEN N'PAYMENT'
            ELSE N'REFUND'
        END,
        N'/',
        s.[SourceNumber],
        N'/',
        s.[RequesterName],
        N'/',
        CASE s.[TransactionType]
            WHEN 1 THEN N'Debit'
            ELSE N'Credit'
        END
    ) AS [Description],
    s.[TransactionDateTime],
    0 AS [OpeningBalance],
    s.[Amount],
    s.[TransactionType],
    0 AS [Balance],
    s.[SourceType],
    s.[SourceID],
    s.[SourceNumber],
    0 AS [IsDeleted],
    s.[CreatedDate],
    CASE
        WHEN s.[CreatedUserID] IS NOT NULL
         AND EXISTS (SELECT 1 FROM [dbo].[Employee] e WHERE e.[EmployeeID] = s.[CreatedUserID])
        THEN s.[CreatedUserID]
        ELSE NULL
    END AS [CreatedUserID],
    s.[CreatedBy],
    s.[EditedDate],
    CASE
        WHEN s.[EditedUserID] IS NOT NULL
         AND EXISTS (SELECT 1 FROM [dbo].[Employee] e WHERE e.[EmployeeID] = s.[EditedUserID])
        THEN s.[EditedUserID]
        ELSE NULL
    END AS [EditedUserID],
    s.[EditedBy]
FROM SourceTransactions s;

;WITH OrderedLedger AS
(
    SELECT
        rl.[RequesterLedgerID],
        CASE WHEN rl.[TransactionType] = 2 THEN rl.[Amount] ELSE -rl.[Amount] END AS [SignedAmount],
        SUM(CASE WHEN rl.[TransactionType] = 2 THEN rl.[Amount] ELSE -rl.[Amount] END) OVER
        (
            PARTITION BY rl.[RequesterID]
            ORDER BY rl.[TransactionDateTime], rl.[CreatedDate], rl.[RequesterLedgerID]
            ROWS UNBOUNDED PRECEDING
        ) AS [RunningBalance]
    FROM [dbo].[RequesterLedgers] rl
    WHERE rl.[IsDeleted] = 0
)
UPDATE rl
SET
    rl.[OpeningBalance] = ol.[RunningBalance] - ol.[SignedAmount],
    rl.[Balance] = ol.[RunningBalance]
FROM [dbo].[RequesterLedgers] rl
INNER JOIN OrderedLedger ol ON ol.[RequesterLedgerID] = rl.[RequesterLedgerID];
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE rl
FROM [dbo].[RequesterLedgers] rl
WHERE rl.[IsDeleted] = 0
  AND rl.[SourceType] IN (1, 2, 3)
  AND rl.[SourceID] IS NOT NULL
  AND
  (
      (rl.[SourceType] = 1 AND EXISTS
      (
          SELECT 1
          FROM [dbo].[Invoices] i
          WHERE i.[InvoicesID] = rl.[SourceID]
            AND i.[IsDeleted] = 0
      ))
      OR
      (rl.[SourceType] = 2 AND EXISTS
      (
          SELECT 1
          FROM [dbo].[Payments] p
          WHERE p.[PaymentID] = rl.[SourceID]
            AND p.[IsDeleted] = 0
      ))
      OR
      (rl.[SourceType] = 3 AND EXISTS
      (
          SELECT 1
          FROM [dbo].[RefundPayments] rp
          WHERE rp.[RefundPaymentID] = rl.[SourceID]
            AND rp.[IsDeleted] = 0
      ))
  );
");
        }
    }
}
