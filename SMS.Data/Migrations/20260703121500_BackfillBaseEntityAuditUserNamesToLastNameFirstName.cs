using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <summary>
    /// Converts legacy <c>FirstName LastName</c> values to <c>LastName, FirstName</c> on
    /// <c>CreatedBy</c>, <c>EditedBy</c>, and <c>DeletedBy</c> for SMS main tables only (not <c>*_Audit</c>).
    /// </summary>
    public partial class BackfillBaseEntityAuditUserNamesToLastNameFirstName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
-- SMS main tables only: skip change-history audit tables (e.g. Request_Audit).
DECLARE @Table sysname;
DECLARE @Column sysname;
DECLARE @sql nvarchar(max);

DECLARE col_cursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT c.TABLE_NAME, c.COLUMN_NAME
    FROM INFORMATION_SCHEMA.COLUMNS c
    WHERE c.TABLE_SCHEMA = N'dbo'
      AND c.COLUMN_NAME IN (N'CreatedBy', N'EditedBy', N'DeletedBy')
      AND c.TABLE_NAME NOT LIKE N'%[_]Audit' ESCAPE N'\';

OPEN col_cursor;
FETCH NEXT FROM col_cursor INTO @Table, @Column;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Assumes legacy values use ""FirstName LastName"" (first token = first name, remainder = last name).
    -- Skips null/empty, values already containing a comma, single-token values, and system literals.
    SET @sql = N'
    UPDATE t
    SET t.[' + @Column + N'] = CONCAT(
            LTRIM(RTRIM(SUBSTRING(s.[Val], p.[SpacePos] + 1, LEN(s.[Val])))),
            N'', '',
            LTRIM(RTRIM(LEFT(s.[Val], p.[SpacePos] - 1))))
    FROM [dbo].[' + @Table + N'] t
    CROSS APPLY (SELECT LTRIM(RTRIM(t.[' + @Column + N'])) AS [Val]) s
    CROSS APPLY (SELECT NULLIF(CHARINDEX(N'' '', s.[Val]), 0) AS [SpacePos]) p
    WHERE t.[' + @Column + N'] IS NOT NULL
      AND s.[Val] <> N''''
      AND s.[Val] NOT LIKE N''%,%''
      AND p.[SpacePos] > 0
      AND s.[Val] NOT IN (N''System'', N''Super User'', N''Master System'');';
    EXEC sp_executesql @sql;

    FETCH NEXT FROM col_cursor INTO @Table, @Column;
END

CLOSE col_cursor;
DEALLOCATE col_cursor;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data backfill is not reversed.
        }
    }
}
