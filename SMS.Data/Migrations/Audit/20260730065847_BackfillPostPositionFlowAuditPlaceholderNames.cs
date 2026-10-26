using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations.Audit
{
    /// <summary>
    /// Backfills <c>PostPositionFlow_Audit.EmployeeId</c>, <c>EmployeeName</c>, and placeholder
    /// subject names inside <c>DetailsJson</c> for placeholder positions.
    /// Joins the sibling main database (e.g. <c>SMS_Audit</c> → <c>SMS</c>).
    /// </summary>
    public partial class BackfillPostPositionFlowAuditPlaceholderNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @MainDb sysname =
    CASE
        WHEN DB_NAME() LIKE N'%\_Audit' ESCAPE N'\'
            THEN LEFT(DB_NAME(), LEN(DB_NAME()) - LEN(N'_Audit'))
        ELSE N'SMS'
    END;

DECLARE @sql nvarchar(max) = N'
;WITH PlaceholderSubjects AS (
    SELECT
        a.[Id],
        pp.[PlaceholderID],
        LEFT(
            CASE
                WHEN NULLIF(LTRIM(RTRIM(ph.[LastName])), N'''') IS NOT NULL
                 AND NULLIF(LTRIM(RTRIM(ph.[FirstName])), N'''') IS NOT NULL
                    THEN CONCAT(LTRIM(RTRIM(ph.[LastName])), N'', '', LTRIM(RTRIM(ph.[FirstName])))
                WHEN NULLIF(LTRIM(RTRIM(ph.[LastName])), N'''') IS NOT NULL
                    THEN LTRIM(RTRIM(ph.[LastName]))
                WHEN NULLIF(LTRIM(RTRIM(ph.[FirstName])), N'''') IS NOT NULL
                    THEN LTRIM(RTRIM(ph.[FirstName]))
                ELSE NULL
            END, 200) AS [PlaceholderName]
    FROM [dbo].[PostPositionFlow_Audit] a
    INNER JOIN ' + QUOTENAME(@MainDb) + N'.[dbo].[PostPosition] pp
        ON pp.[PostPositionID] = a.[PostPositionId]
    INNER JOIN ' + QUOTENAME(@MainDb) + N'.[dbo].[Placeholder] ph
        ON ph.[PlaceholderID] = pp.[PlaceholderID]
    WHERE pp.[PlaceholderID] IS NOT NULL
      AND (
            a.[EmployeeId] IS NULL
            OR a.[EmployeeName] IS NULL
            OR LTRIM(RTRIM(a.[EmployeeName])) = N''''
            OR (
                a.[DetailsJson] IS NOT NULL
                AND ISJSON(a.[DetailsJson]) = 1
                AND LEFT(LTRIM(a.[DetailsJson]), 1) = N''[''
            )
          )
)
UPDATE a
SET
    a.[EmployeeId] = COALESCE(a.[EmployeeId], ps.[PlaceholderID]),
    a.[EmployeeName] = COALESCE(
        NULLIF(LTRIM(RTRIM(a.[EmployeeName])), N''''),
        ps.[PlaceholderName])
FROM [dbo].[PostPositionFlow_Audit] a
INNER JOIN PlaceholderSubjects ps
    ON ps.[Id] = a.[Id]
WHERE a.[EmployeeId] IS NULL
   OR a.[EmployeeName] IS NULL
   OR LTRIM(RTRIM(a.[EmployeeName])) = N'''';

;WITH PlaceholderSubjects AS (
    SELECT
        a.[Id],
        LEFT(
            CASE
                WHEN NULLIF(LTRIM(RTRIM(ph.[LastName])), N'''') IS NOT NULL
                 AND NULLIF(LTRIM(RTRIM(ph.[FirstName])), N'''') IS NOT NULL
                    THEN CONCAT(LTRIM(RTRIM(ph.[LastName])), N'', '', LTRIM(RTRIM(ph.[FirstName])))
                WHEN NULLIF(LTRIM(RTRIM(ph.[LastName])), N'''') IS NOT NULL
                    THEN LTRIM(RTRIM(ph.[LastName]))
                WHEN NULLIF(LTRIM(RTRIM(ph.[FirstName])), N'''') IS NOT NULL
                    THEN LTRIM(RTRIM(ph.[FirstName]))
                ELSE NULL
            END, 200) AS [PlaceholderName]
    FROM [dbo].[PostPositionFlow_Audit] a
    INNER JOIN ' + QUOTENAME(@MainDb) + N'.[dbo].[PostPosition] pp
        ON pp.[PostPositionID] = a.[PostPositionId]
    INNER JOIN ' + QUOTENAME(@MainDb) + N'.[dbo].[Placeholder] ph
        ON ph.[PlaceholderID] = pp.[PlaceholderID]
    WHERE pp.[PlaceholderID] IS NOT NULL
      AND a.[DetailsJson] IS NOT NULL
      AND ISJSON(a.[DetailsJson]) = 1
      AND LEFT(LTRIM(a.[DetailsJson]), 1) = N''[''
)
UPDATE a
SET a.[DetailsJson] = rebuilt.[DetailsJson]
FROM [dbo].[PostPositionFlow_Audit] a
INNER JOIN PlaceholderSubjects ps
    ON ps.[Id] = a.[Id]
CROSS APPLY (
    SELECT
        (
            SELECT
                d.[label],
                CASE
                    WHEN d.[label] IN (
                            N''Assigned to'',
                            N''Employee'',
                            N''Taken by'',
                            N''To'',
                            N''Offered to'')
                         AND (d.[value] IS NULL OR LTRIM(RTRIM(d.[value])) = N'''')
                        THEN ps.[PlaceholderName]
                    ELSE d.[value]
                END AS [value]
            FROM OPENJSON(a.[DetailsJson])
            WITH (
                [label] nvarchar(200) ''$.label'',
                [value] nvarchar(max) ''$.value''
            ) AS d
            FOR JSON PATH
        ) AS [DetailsJson]
) AS rebuilt
WHERE ps.[PlaceholderName] IS NOT NULL
  AND rebuilt.[DetailsJson] IS NOT NULL
  AND rebuilt.[DetailsJson] <> ISNULL(a.[DetailsJson], N'''');
';

EXEC sp_executesql @sql;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data backfill is not reversed.
        }
    }
}
