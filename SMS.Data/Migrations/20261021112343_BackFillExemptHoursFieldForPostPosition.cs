using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <summary>
    /// Backfills <c>PostPosition.IsExemptPosition</c> from <c>{MainDb}_Audit.dbo.Post_Audit</c> timeline + dept config.
    /// </summary>
    public partial class BackFillExemptHoursFieldForPostPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @AuditDb sysname = DB_NAME() + N'_Audit';

IF DB_ID(@AuditDb) IS NULL
BEGIN
    DECLARE @missingAuditDbMessage nvarchar(400) =
        N'Audit database not found for IsExemptPosition backfill: ' + @AuditDb;
    THROW 50001, @missingAuditDbMessage, 1;
END;

IF OBJECT_ID(N'tempdb..#ExemptAudit') IS NOT NULL DROP TABLE #ExemptAudit;

CREATE TABLE #ExemptAudit (
    [PostID] uniqueidentifier NOT NULL,
    [AuditId] bigint NOT NULL,
    [Action] int NOT NULL,
    [ChangedOn] datetime2 NOT NULL,
    [NewExempt] bit NULL,
    [OldExempt] bit NULL
);

DECLARE @LoadAuditSql nvarchar(max) = N'
INSERT INTO #ExemptAudit ([PostID], [AuditId], [Action], [ChangedOn], [NewExempt], [OldExempt])
SELECT
    sp.[PostID],
    a.[Id],
    a.[Action],
    a.[ChangedOn],
    CASE LOWER(LTRIM(RTRIM(fc.[new])))
        WHEN N''true'' THEN CAST(1 AS bit)
        WHEN N''false'' THEN CAST(0 AS bit)
        WHEN N''1'' THEN CAST(1 AS bit)
        WHEN N''0'' THEN CAST(0 AS bit)
        ELSE NULL
    END,
    CASE LOWER(LTRIM(RTRIM(fc.[old])))
        WHEN N''true'' THEN CAST(1 AS bit)
        WHEN N''false'' THEN CAST(0 AS bit)
        WHEN N''1'' THEN CAST(1 AS bit)
        WHEN N''0'' THEN CAST(0 AS bit)
        ELSE NULL
    END
FROM (
    SELECT DISTINCT pp.[PostID]
    FROM [dbo].[PostPosition] pp
    WHERE pp.[IsPlaceholder] = 0
      AND pp.[EmployeeID] IS NOT NULL
      AND pp.[DateOfWork] IS NOT NULL
      AND pp.[PostPositionStatus] IN (1, 2, 4, 5, 11, 14)
) sp
INNER JOIN ' + QUOTENAME(@AuditDb) + N'.[dbo].[Post_Audit] a
    ON a.[EntityId] = CONVERT(nvarchar(36), sp.[PostID])
CROSS APPLY OPENJSON(a.[ChangedFields])
WITH (
    [property] nvarchar(200) ''$.property'',
    [old] nvarchar(max) ''$.old'',
    [new] nvarchar(max) ''$.new''
) fc
WHERE ISJSON(a.[ChangedFields]) = 1
  AND fc.[property] IN (N''Exempt hours'', N''IsExemptHours'')
  AND a.[Action] IN (0, 1);';

EXEC sys.sp_executesql @LoadAuditSql;

CREATE CLUSTERED INDEX [CX_ExemptAudit] ON #ExemptAudit ([PostID], [ChangedOn], [AuditId]);

;WITH EligiblePostPositions AS (
    SELECT
        pp.[PostPositionID],
        pp.[PostID],
        pp.[DateOfWork],
        pp.[IsExemptPosition],
        p.[IsExemptHours]
    FROM [dbo].[PostPosition] pp
    INNER JOIN [dbo].[Post] p ON p.[PostID] = pp.[PostID]
    WHERE pp.[IsPlaceholder] = 0
      AND pp.[EmployeeID] IS NOT NULL
      AND pp.[DateOfWork] IS NOT NULL
      AND pp.[PostPositionStatus] IN (1, 2, 4, 5, 11, 14)
),
DistinctWork AS (
    SELECT DISTINCT [PostID], [DateOfWork] FROM EligiblePostPositions
),
FirstAuditRow AS (
    SELECT
        [PostID],
        [OldExempt],
        [NewExempt],
        CAST([ChangedOn] AS date) AS [FirstChangeDate]
    FROM (
        SELECT
            ea.*,
            ROW_NUMBER() OVER (PARTITION BY ea.[PostID] ORDER BY ea.[ChangedOn], ea.[AuditId]) AS [rn]
        FROM #ExemptAudit ea
        WHERE ea.[NewExempt] IS NOT NULL
    ) x
    WHERE [rn] = 1
),
ExemptSegments AS (
    SELECT
        ea.[PostID],
        ea.[AuditId],
        ea.[ChangedOn],
        ea.[NewExempt],
        CAST(ea.[ChangedOn] AS date) AS [SegmentStartDate],
        LEAD(CAST(ea.[ChangedOn] AS date)) OVER (
            PARTITION BY ea.[PostID] ORDER BY ea.[ChangedOn], ea.[AuditId]
        ) AS [SegmentEndDateExclusive]
    FROM #ExemptAudit ea
    WHERE ea.[NewExempt] IS NOT NULL
),
ExemptByPostAndDate AS (
    SELECT
        d.[PostID],
        d.[DateOfWork],
        CASE
            WHEN seg.[AuditId] IS NOT NULL THEN seg.[NewExempt]
            WHEN fa.[PostID] IS NOT NULL AND d.[DateOfWork] < fa.[FirstChangeDate]
                THEN COALESCE(fa.[OldExempt], p.[IsExemptHours])
            ELSE p.[IsExemptHours]
        END AS [ExemptAtDateOfWork]
    FROM DistinctWork d
    INNER JOIN [dbo].[Post] p ON p.[PostID] = d.[PostID]
    OUTER APPLY (
        SELECT TOP (1) seg0.[AuditId], seg0.[NewExempt]
        FROM ExemptSegments seg0
        WHERE seg0.[PostID] = d.[PostID]
          AND d.[DateOfWork] >= seg0.[SegmentStartDate]
          AND (seg0.[SegmentEndDateExclusive] IS NULL OR d.[DateOfWork] < seg0.[SegmentEndDateExclusive])
        ORDER BY seg0.[ChangedOn] DESC, seg0.[AuditId] DESC
    ) seg
    LEFT JOIN FirstAuditRow fa ON fa.[PostID] = d.[PostID]
),
EffectiveExemptByWorkDate AS (
    SELECT
        e.[PostPositionID],
        x.[ExemptAtDateOfWork]
    FROM EligiblePostPositions e
    INNER JOIN ExemptByPostAndDate x
        ON x.[PostID] = e.[PostID] AND x.[DateOfWork] = e.[DateOfWork]
)
UPDATE pp
SET pp.[IsExemptPosition] = CASE
        WHEN e.[ExemptAtDateOfWork] = 1 AND deptCfg.[DeptEnableExemptRules] = 1
        THEN 1
        ELSE 0
    END
FROM [dbo].[PostPosition] pp
INNER JOIN EffectiveExemptByWorkDate e
    ON e.[PostPositionID] = pp.[PostPositionID]
INNER JOIN [dbo].[Post] p
    ON p.[PostID] = pp.[PostID]
INNER JOIN [dbo].[Request] r
    ON r.[RequestID] = p.[RequestID]
CROSS APPLY (
    SELECT ISNULL((
        SELECT TOP (1) CAST(dc0.[EnableExemptRequestsJobsFromHoursRules] AS int)
        FROM [dbo].[DepartmentConfiguration] dc0
        WHERE dc0.[DepartmentID] = r.[DepartmentID]
        ORDER BY dc0.[DeptConfigID]
    ), 0) AS [DeptEnableExemptRules]
) deptCfg
WHERE pp.[IsExemptPosition] <> CASE
        WHEN e.[ExemptAtDateOfWork] = 1 AND deptCfg.[DeptEnableExemptRules] = 1
        THEN 1
        ELSE 0
    END;

DROP TABLE #ExemptAudit;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data backfill is not reversed.
        }
    }
}
