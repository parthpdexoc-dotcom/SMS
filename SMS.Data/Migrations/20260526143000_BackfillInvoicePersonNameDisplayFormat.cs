using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <summary>
    /// Converts person names stored as "FirstName LastName" to "LastName, FirstName"
    /// for invoice contact and officer name fields.
    /// </summary>
    public partial class BackfillInvoicePersonNameDisplayFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Assumes legacy values use "FirstName LastName" (first token = first name, remainder = last name).
            // Skips null/empty, values already containing a comma, and single-token values (e.g. department names).
            migrationBuilder.Sql(@"
UPDATE i
SET i.[BillingToContact] = CONCAT(
        LTRIM(RTRIM(SUBSTRING(s.[Val], p.[SpacePos] + 1, LEN(s.[Val])))),
        N', ',
        LTRIM(RTRIM(LEFT(s.[Val], p.[SpacePos] - 1))))
FROM [dbo].[Invoices] i
CROSS APPLY (SELECT LTRIM(RTRIM(i.[BillingToContact])) AS [Val]) s
CROSS APPLY (SELECT NULLIF(CHARINDEX(N' ', s.[Val]), 0) AS [SpacePos]) p
WHERE i.[BillingToContact] IS NOT NULL
  AND s.[Val] <> N''
  AND s.[Val] NOT LIKE N'%,%'
  AND p.[SpacePos] > 0;

UPDATE i
SET i.[BillingFromContact] = CONCAT(
        LTRIM(RTRIM(SUBSTRING(s.[Val], p.[SpacePos] + 1, LEN(s.[Val])))),
        N', ',
        LTRIM(RTRIM(LEFT(s.[Val], p.[SpacePos] - 1))))
FROM [dbo].[Invoices] i
CROSS APPLY (SELECT LTRIM(RTRIM(i.[BillingFromContact])) AS [Val]) s
CROSS APPLY (SELECT NULLIF(CHARINDEX(N' ', s.[Val]), 0) AS [SpacePos]) p
WHERE i.[BillingFromContact] IS NOT NULL
  AND s.[Val] <> N''
  AND s.[Val] NOT LIKE N'%,%'
  AND p.[SpacePos] > 0;

UPDATE d
SET d.[OfficerName] = CONCAT(
        LTRIM(RTRIM(SUBSTRING(s.[Val], p.[SpacePos] + 1, LEN(s.[Val])))),
        N', ',
        LTRIM(RTRIM(LEFT(s.[Val], p.[SpacePos] - 1))))
FROM [dbo].[InvoiceDetails] d
CROSS APPLY (SELECT LTRIM(RTRIM(d.[OfficerName])) AS [Val]) s
CROSS APPLY (SELECT NULLIF(CHARINDEX(N' ', s.[Val]), 0) AS [SpacePos]) p
WHERE d.[OfficerName] IS NOT NULL
  AND s.[Val] <> N''
  AND s.[Val] NOT LIKE N'%,%'
  AND p.[SpacePos] > 0;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data backfill is not reversed (cannot distinguish updated rows from originally comma-formatted values).
        }
    }
}
