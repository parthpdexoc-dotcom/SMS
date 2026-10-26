using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations.Audit
{
    /// <summary>
    /// Backfills <c>PostPositionFlow_Audit</c> from historical <c>PostPosition_Audit</c> rows.
    /// Matches on <c>EntityId</c> = <c>PostPositionId</c>, maps performer/worker/department/display name,
    /// derives flow <c>EventType</c> from status/employee changes (with assign classification:
    /// SelfTaken / AssignedByAdmin / AssignedByOther / AssignedBySystem), and stores
    /// <c>DetailsJson</c> in the same shape as live flow logging (Assigned by / Assigned to / Assignment type).
    /// When audit <c>DepartmentID</c> is null, resolves it from main <c>Request.DepartmentID</c>
    /// via <c>Post.RequestID</c>.
    /// When <c>Is placeholder</c> is true (ChangedFields and/or main <c>PostPosition.IsPlaceholder</c>),
    /// <c>EmployeeId</c> is set from <c>PostPosition.PlaceholderID</c> and <c>EmployeeName</c> from
    /// the Placeholder profile (falling back to the ChangedFields <c>Placeholder</c> display name).
    /// Only rows whose <c>PostPositionId</c> exists in main <c>PostPosition</c> are migrated
    /// (including soft-deleted rows; <c>IsDeleted</c> is not filtered on main DB joins).
    /// Idempotent via matching <c>PostPositionId</c> + <c>EventType</c> + <c>RecordedDate</c>.
    /// </summary>
    public partial class BackfillPostPositionFlowAuditFromPostPositionAudit : Migration
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
;WITH FieldChanges AS (
    SELECT
        a.[Id] AS [AuditId],
        TRY_CONVERT(uniqueidentifier, a.[EntityId]) AS [PostPositionId],
        a.[DisplayName],
        a.[DepartmentID],
        a.[EmployeeId] AS [AuditEmployeeId],
        a.[UserId],
        a.[ChangedOn],
        a.[Action],
        fc.[Property],
        fc.[OldValue],
        fc.[NewValue]
    FROM [dbo].[PostPosition_Audit] a
    CROSS APPLY OPENJSON(a.[ChangedFields])
    WITH (
        [Property] nvarchar(200) ''$.property'',
        [OldValue] nvarchar(max) ''$.old'',
        [NewValue] nvarchar(max) ''$.new''
    ) fc
    WHERE ISJSON(a.[ChangedFields]) = 1
      AND TRY_CONVERT(uniqueidentifier, a.[EntityId]) IS NOT NULL
),
AuditPivot AS (
    SELECT
        [AuditId],
        [PostPositionId],
        [DisplayName],
        [DepartmentID],
        [AuditEmployeeId],
        [UserId],
        [ChangedOn],
        [Action],
        MAX(CASE WHEN [Property] = N''Status'' THEN NULLIF(NULLIF(LTRIM(RTRIM([NewValue])), N''''), N''null'') END) AS [StatusNew],
        MAX(CASE WHEN [Property] = N''Status'' THEN NULLIF(NULLIF(LTRIM(RTRIM([OldValue])), N''''), N''null'') END) AS [StatusOld],
        MAX(CASE WHEN [Property] = N''Employee'' THEN NULLIF(NULLIF(LTRIM(RTRIM([NewValue])), N''''), N''null'') END) AS [EmployeeNewName],
        MAX(CASE WHEN [Property] = N''Employee'' THEN NULLIF(NULLIF(LTRIM(RTRIM([OldValue])), N''''), N''null'') END) AS [EmployeeOldName],
        MAX(CASE WHEN [Property] = N''Date of work'' THEN NULLIF(NULLIF(LTRIM(RTRIM([NewValue])), N''''), N''null'') END) AS [DateOfWork],
        MAX(CASE WHEN [Property] = N''Start time'' THEN NULLIF(NULLIF(LTRIM(RTRIM([NewValue])), N''''), N''null'') END) AS [StartTime],
        MAX(CASE WHEN [Property] = N''End time'' THEN NULLIF(NULLIF(LTRIM(RTRIM([NewValue])), N''''), N''null'') END) AS [EndTime],
        MAX(CASE WHEN [Property] IN (N''Is placeholder'', N''IsPlaceholder'') THEN COALESCE(
            NULLIF(NULLIF(LTRIM(RTRIM([NewValue])), N''''), N''null''),
            NULLIF(NULLIF(LTRIM(RTRIM([OldValue])), N''''), N''null'')) END) AS [IsPlaceholder],
        MAX(CASE WHEN [Property] IN (N''Placeholder'', N''PlaceholderID'') THEN COALESCE(
            NULLIF(NULLIF(LTRIM(RTRIM([NewValue])), N''''), N''null''),
            NULLIF(NULLIF(LTRIM(RTRIM([OldValue])), N''''), N''null'')) END) AS [PlaceholderName],
        MAX(CASE WHEN [Property] = N''Reason for cancel'' THEN NULLIF(NULLIF(LTRIM(RTRIM([NewValue])), N''''), N''null'') END) AS [CancelReason],
        MAX(CASE WHEN [Property] = N''Parent job position'' THEN COALESCE(
            NULLIF(NULLIF(LTRIM(RTRIM([NewValue])), N''''), N''null''),
            NULLIF(NULLIF(LTRIM(RTRIM([OldValue])), N''''), N''null'')) END) AS [ParentPostPositionIdRaw]
    FROM FieldChanges
    GROUP BY [AuditId], [PostPositionId], [DisplayName], [DepartmentID], [AuditEmployeeId], [UserId], [ChangedOn], [Action]
),
FlowSource AS (
    SELECT
        ap.*,
        CASE
            WHEN ap.[StatusNew] = N''Transferred'' THEN 4
            WHEN ap.[StatusNew] = N''Closed'' THEN 6
            WHEN ap.[StatusNew] = N''ClosedAndSplit'' THEN 7
            WHEN ap.[StatusNew] = N''CancellationPending'' THEN 8
            WHEN ap.[StatusNew] = N''Cancelled'' THEN 9
            WHEN ap.[StatusNew] = N''SplitDecisionPending'' THEN 14
            WHEN ap.[StatusNew] = N''Declined'' THEN 11
            WHEN ap.[StatusNew] = N''Unclosed'' THEN 12
            WHEN ap.[StatusNew] = N''Assigned'' AND ap.[StatusOld] = N''SplitDecisionPending'' THEN 10
            WHEN ap.[StatusNew] = N''Assigned'' THEN 3
            WHEN ap.[EmployeeNewName] IS NOT NULL
                 AND (ap.[EmployeeOldName] IS NULL OR LTRIM(RTRIM(ap.[EmployeeOldName])) = N'''')
                 AND ap.[StatusNew] IS NULL THEN 3
            WHEN ap.[EmployeeNewName] IS NOT NULL
                 AND ap.[EmployeeOldName] IS NOT NULL
                 AND ap.[EmployeeNewName] <> ap.[EmployeeOldName]
                 AND ap.[StatusNew] IS NULL THEN 4
            WHEN ap.[EmployeeOldName] IS NOT NULL
                 AND (ap.[EmployeeNewName] IS NULL OR LTRIM(RTRIM(ap.[EmployeeNewName])) = N'''')
                 AND ap.[StatusNew] IS NULL THEN 5
            ELSE NULL
        END AS [EventType]
    FROM AuditPivot ap
    WHERE ap.[StatusNew] IS NOT NULL
       OR (ap.[EmployeeNewName] IS NOT NULL AND ap.[EmployeeNewName] <> ISNULL(ap.[EmployeeOldName], N''''))
       OR (ap.[EmployeeOldName] IS NOT NULL AND (ap.[EmployeeNewName] IS NULL OR LTRIM(RTRIM(ap.[EmployeeNewName])) = N''''))
),
FlowClassified AS (
    SELECT
        fs.[AuditId],
        fs.[PostPositionId],
        fs.[DisplayName],
        COALESCE(fs.[DepartmentID], req.[DepartmentID]) AS [DepartmentID],
        fs.[AuditEmployeeId],
        fs.[UserId],
        fs.[ChangedOn],
        fs.[StatusNew],
        fs.[StatusOld],
        fs.[EmployeeNewName],
        fs.[EmployeeOldName],
        fs.[DateOfWork],
        fs.[StartTime],
        fs.[EndTime],
        fs.[IsPlaceholder],
        fs.[PlaceholderName],
        fs.[CancelReason],
        fs.[ParentPostPositionIdRaw],
        pp.[PostID],
        po.[RequestID],
        pp.[PlaceholderID],
        COALESCE(
            pp.[ParentPostPositionId],
            TRY_CONVERT(uniqueidentifier, fs.[ParentPostPositionIdRaw]),
            pp.[SplitPostPositionID],
            parentBySplitChild.[PostPositionID]) AS [RelatedPostPositionId],
        COALESCE(pp.[CreatedUserID], TRY_CONVERT(uniqueidentifier, fs.[UserId])) AS [PerformerEmployeeId],
        COALESCE(
            NULLIF(pp.[EmployeeID], ''00000000-0000-0000-0000-000000000000''),
            fs.[AuditEmployeeId]) AS [PositionEmployeeId],
        LEFT(
            CASE
                WHEN placeholderFlag.[IsPlaceholderRow] = 1
                    THEN COALESCE(
                        fs.[PlaceholderName],
                        NULLIF(
                            CASE
                                WHEN NULLIF(LTRIM(RTRIM(ph.[LastName])), N'''') IS NOT NULL
                                 AND NULLIF(LTRIM(RTRIM(ph.[FirstName])), N'''') IS NOT NULL
                                    THEN CONCAT(LTRIM(RTRIM(ph.[LastName])), N'', '', LTRIM(RTRIM(ph.[FirstName])))
                                WHEN NULLIF(LTRIM(RTRIM(ph.[LastName])), N'''') IS NOT NULL
                                    THEN LTRIM(RTRIM(ph.[LastName]))
                                WHEN NULLIF(LTRIM(RTRIM(ph.[FirstName])), N'''') IS NOT NULL
                                    THEN LTRIM(RTRIM(ph.[FirstName]))
                                ELSE NULL
                            END, N''''),
                        fs.[EmployeeNewName])
                ELSE COALESCE(fs.[EmployeeNewName], worker.[EmployeeName])
            END, 200) AS [EmployeeName],
        LEFT(COALESCE(
            splitFromWorker.[EmployeeName],
            parentClosedSplitAudit.[EmployeeName],
            parentAuditEmployee.[EmployeeName],
            fs.[EmployeeOldName]), 200) AS [SplitFromEmployeeName],
        LEFT(COALESCE(performer.[EmployeeName], NULLIF(LTRIM(RTRIM(fs.[UserId])), N''''), N''System''), 200) AS [PerformerName],
        assignerAdmin.[AssignmentType],
        CASE
            WHEN fs.[EventType] <> 3 THEN fs.[EventType]
            -- Placeholder assigns are never SelfTaken (CreatedUserID often equals AuditEmployeeId).
            WHEN placeholderFlag.[IsPlaceholderRow] = 1 THEN
                CASE
                    WHEN COALESCE(pp.[CreatedUserID], TRY_CONVERT(uniqueidentifier, fs.[UserId])) IS NULL THEN 16
                    WHEN assignerAdmin.[AssignmentType] IS NOT NULL THEN 3
                    ELSE 15
                END
            WHEN COALESCE(pp.[CreatedUserID], TRY_CONVERT(uniqueidentifier, fs.[UserId])) IS NULL THEN 16
            WHEN COALESCE(pp.[CreatedUserID], TRY_CONVERT(uniqueidentifier, fs.[UserId]))
                 = COALESCE(NULLIF(pp.[EmployeeID], ''00000000-0000-0000-0000-000000000000''), fs.[AuditEmployeeId]) THEN 2
            WHEN assignerAdmin.[AssignmentType] IS NOT NULL THEN 3
            ELSE 15
        END AS [ClassifiedEventType],
        CASE
            WHEN placeholderFlag.[IsPlaceholderRow] = 1
                THEN pp.[PlaceholderID]
            ELSE fs.[AuditEmployeeId]
        END AS [EmployeeId]
    FROM FlowSource fs
    INNER JOIN ' + QUOTENAME(@MainDb) + N'.[dbo].[PostPosition] pp
        ON pp.[PostPositionID] = fs.[PostPositionId]
    CROSS APPLY (
        SELECT CASE
            WHEN LOWER(LTRIM(RTRIM(ISNULL(fs.[IsPlaceholder], N'''')))) IN (N''true'', N''1'')
              OR pp.[IsPlaceholder] = 1
              OR (
                    pp.[PlaceholderID] IS NOT NULL
                    AND NULLIF(pp.[EmployeeID], ''00000000-0000-0000-0000-000000000000'') IS NULL
                    AND NULLIF(LTRIM(RTRIM(ISNULL(fs.[EmployeeNewName], N''''))), N'''') IS NULL
                 )
            THEN 1
            ELSE 0
        END AS [IsPlaceholderRow]
    ) placeholderFlag
    LEFT JOIN ' + QUOTENAME(@MainDb) + N'.[dbo].[Post] po
        ON po.[PostID] = pp.[PostID]
    LEFT JOIN ' + QUOTENAME(@MainDb) + N'.[dbo].[Request] req
        ON req.[RequestID] = po.[RequestID]
    LEFT JOIN ' + QUOTENAME(@MainDb) + N'.[dbo].[Placeholder] ph
        ON ph.[PlaceholderID] = pp.[PlaceholderID]
    LEFT JOIN ' + QUOTENAME(@MainDb) + N'.[dbo].[PostPosition] parentBySplitChild
        ON parentBySplitChild.[SplitPostPositionID] = fs.[PostPositionId]
    LEFT JOIN ' + QUOTENAME(@MainDb) + N'.[dbo].[PostPosition] parentPp
        ON parentPp.[PostPositionID] = COALESCE(
            pp.[ParentPostPositionId],
            TRY_CONVERT(uniqueidentifier, fs.[ParentPostPositionIdRaw]),
            pp.[SplitPostPositionID],
            parentBySplitChild.[PostPositionID])
    OUTER APPLY (
        SELECT
            LEFT(
                CASE
                    WHEN NULLIF(LTRIM(RTRIM(e.[LastName])), N'''') IS NOT NULL
                     AND NULLIF(LTRIM(RTRIM(e.[FirstName])), N'''') IS NOT NULL
                        THEN CONCAT(LTRIM(RTRIM(e.[LastName])), N'', '', LTRIM(RTRIM(e.[FirstName])))
                    WHEN NULLIF(LTRIM(RTRIM(e.[LastName])), N'''') IS NOT NULL
                        THEN LTRIM(RTRIM(e.[LastName]))
                    WHEN NULLIF(LTRIM(RTRIM(e.[FirstName])), N'''') IS NOT NULL
                        THEN LTRIM(RTRIM(e.[FirstName]))
                    ELSE NULL
                END, 200) AS [EmployeeName]
        FROM ' + QUOTENAME(@MainDb) + N'.[dbo].[Employee] e
        WHERE parentPp.[EmployeeID] IS NOT NULL
          AND parentPp.[EmployeeID] <> ''00000000-0000-0000-0000-000000000000''
          AND e.[EmployeeID] = parentPp.[EmployeeID]
    ) splitFromWorker
    OUTER APPLY (
        SELECT TOP (1)
            LEFT(COALESCE(
                NULLIF(
                    CASE
                        WHEN NULLIF(LTRIM(RTRIM(e.[LastName])), N'''') IS NOT NULL
                         AND NULLIF(LTRIM(RTRIM(e.[FirstName])), N'''') IS NOT NULL
                            THEN CONCAT(LTRIM(RTRIM(e.[LastName])), N'', '', LTRIM(RTRIM(e.[FirstName])))
                        WHEN NULLIF(LTRIM(RTRIM(e.[LastName])), N'''') IS NOT NULL
                            THEN LTRIM(RTRIM(e.[LastName]))
                        WHEN NULLIF(LTRIM(RTRIM(e.[FirstName])), N'''') IS NOT NULL
                            THEN LTRIM(RTRIM(e.[FirstName]))
                        ELSE NULL
                    END, N''''),
                NULLIF(NULLIF(LTRIM(RTRIM(parentPivot.[EmployeeDisplayName])), N''''), N''null'')),
                200) AS [EmployeeName]
        FROM [dbo].[PostPosition_Audit] pa
        CROSS APPLY (
            SELECT
                MAX(CASE WHEN pfc.[Property] = N''Status'' THEN NULLIF(NULLIF(LTRIM(RTRIM(pfc.[NewValue])), N''''), N''null'') END) AS [StatusNew],
                MAX(CASE WHEN pfc.[Property] = N''Employee'' THEN COALESCE(
                    NULLIF(NULLIF(LTRIM(RTRIM(pfc.[NewValue])), N''''), N''null''),
                    NULLIF(NULLIF(LTRIM(RTRIM(pfc.[OldValue])), N''''), N''null'')) END) AS [EmployeeDisplayName]
            FROM OPENJSON(pa.[ChangedFields])
            WITH (
                [Property] nvarchar(200) ''$.property'',
                [OldValue] nvarchar(max) ''$.old'',
                [NewValue] nvarchar(max) ''$.new''
            ) pfc
        ) parentPivot
        LEFT JOIN ' + QUOTENAME(@MainDb) + N'.[dbo].[Employee] e
            ON e.[EmployeeID] = pa.[EmployeeId]
        WHERE parentPp.[PostPositionID] IS NOT NULL
          AND TRY_CONVERT(uniqueidentifier, pa.[EntityId]) = parentPp.[PostPositionID]
          AND parentPivot.[StatusNew] = N''ClosedAndSplit''
          AND pa.[ChangedOn] <= DATEADD(minute, 5, fs.[ChangedOn])
          AND (
                pa.[EmployeeId] IS NOT NULL
                OR parentPivot.[EmployeeDisplayName] IS NOT NULL
              )
        ORDER BY pa.[ChangedOn] DESC, pa.[Id] DESC
    ) parentClosedSplitAudit
    OUTER APPLY (
        SELECT TOP (1)
            LEFT(COALESCE(
                NULLIF(
                    CASE
                        WHEN NULLIF(LTRIM(RTRIM(e.[LastName])), N'''') IS NOT NULL
                         AND NULLIF(LTRIM(RTRIM(e.[FirstName])), N'''') IS NOT NULL
                            THEN CONCAT(LTRIM(RTRIM(e.[LastName])), N'', '', LTRIM(RTRIM(e.[FirstName])))
                        WHEN NULLIF(LTRIM(RTRIM(e.[LastName])), N'''') IS NOT NULL
                            THEN LTRIM(RTRIM(e.[LastName]))
                        WHEN NULLIF(LTRIM(RTRIM(e.[FirstName])), N'''') IS NOT NULL
                            THEN LTRIM(RTRIM(e.[FirstName]))
                        ELSE NULL
                    END, N''''),
                NULLIF(NULLIF(LTRIM(RTRIM(parentEmp.[EmployeeDisplayName])), N''''), N''null'')),
                200) AS [EmployeeName]
        FROM [dbo].[PostPosition_Audit] pa
        CROSS APPLY (
            SELECT MAX(CASE WHEN pfc.[Property] = N''Employee'' THEN COALESCE(
                NULLIF(NULLIF(LTRIM(RTRIM(pfc.[NewValue])), N''''), N''null''),
                NULLIF(NULLIF(LTRIM(RTRIM(pfc.[OldValue])), N''''), N''null'')) END) AS [EmployeeDisplayName]
            FROM OPENJSON(pa.[ChangedFields])
            WITH (
                [Property] nvarchar(200) ''$.property'',
                [OldValue] nvarchar(max) ''$.old'',
                [NewValue] nvarchar(max) ''$.new''
            ) pfc
        ) parentEmp
        LEFT JOIN ' + QUOTENAME(@MainDb) + N'.[dbo].[Employee] e
            ON e.[EmployeeID] = pa.[EmployeeId]
        WHERE parentPp.[PostPositionID] IS NOT NULL
          AND TRY_CONVERT(uniqueidentifier, pa.[EntityId]) = parentPp.[PostPositionID]
          AND pa.[ChangedOn] <= fs.[ChangedOn]
          AND (
                pa.[EmployeeId] IS NOT NULL
                OR parentEmp.[EmployeeDisplayName] IS NOT NULL
              )
        ORDER BY pa.[ChangedOn] DESC, pa.[Id] DESC
    ) parentAuditEmployee
    OUTER APPLY (
        SELECT
            LEFT(
                CASE
                    WHEN NULLIF(LTRIM(RTRIM(e.[LastName])), N'''') IS NOT NULL
                     AND NULLIF(LTRIM(RTRIM(e.[FirstName])), N'''') IS NOT NULL
                        THEN CONCAT(LTRIM(RTRIM(e.[LastName])), N'', '', LTRIM(RTRIM(e.[FirstName])))
                    WHEN NULLIF(LTRIM(RTRIM(e.[LastName])), N'''') IS NOT NULL
                        THEN LTRIM(RTRIM(e.[LastName]))
                    WHEN NULLIF(LTRIM(RTRIM(e.[FirstName])), N'''') IS NOT NULL
                        THEN LTRIM(RTRIM(e.[FirstName]))
                    ELSE NULL
                END, 200) AS [EmployeeName]
        FROM ' + QUOTENAME(@MainDb) + N'.[dbo].[Employee] e
        WHERE e.[EmployeeID] = COALESCE(pp.[CreatedUserID], TRY_CONVERT(uniqueidentifier, fs.[UserId]))
    ) performer
    OUTER APPLY (
        SELECT
            LEFT(
                CASE
                    WHEN NULLIF(LTRIM(RTRIM(e.[LastName])), N'''') IS NOT NULL
                     AND NULLIF(LTRIM(RTRIM(e.[FirstName])), N'''') IS NOT NULL
                        THEN CONCAT(LTRIM(RTRIM(e.[LastName])), N'', '', LTRIM(RTRIM(e.[FirstName])))
                    WHEN NULLIF(LTRIM(RTRIM(e.[LastName])), N'''') IS NOT NULL
                        THEN LTRIM(RTRIM(e.[LastName]))
                    WHEN NULLIF(LTRIM(RTRIM(e.[FirstName])), N'''') IS NOT NULL
                        THEN LTRIM(RTRIM(e.[FirstName]))
                    ELSE NULL
                END, 200) AS [EmployeeName]
        FROM ' + QUOTENAME(@MainDb) + N'.[dbo].[Employee] e
        WHERE e.[EmployeeID] = fs.[AuditEmployeeId]
    ) worker
    OUTER APPLY (
        SELECT
            LEFT(STRING_AGG(x.[AdminTypeShort], N'', '') WITHIN GROUP (ORDER BY x.[AdminTypeShort]), 200) AS [AssignmentType]
        FROM (
            SELECT DISTINCT at.[AdminTypeShort]
            FROM ' + QUOTENAME(@MainDb) + N'.[dbo].[EmployeeAdmin] ea
            INNER JOIN ' + QUOTENAME(@MainDb) + N'.[dbo].[AdminType] at
                ON at.[AdminTypeID] = ea.[AdminTypeID]
            WHERE ea.[EmployeeID] = COALESCE(pp.[CreatedUserID], TRY_CONVERT(uniqueidentifier, fs.[UserId]))
              AND NULLIF(LTRIM(RTRIM(at.[AdminTypeShort])), N'''') IS NOT NULL
            UNION
            SELECT DISTINCT at.[AdminTypeShort]
            FROM ' + QUOTENAME(@MainDb) + N'.[dbo].[DepartmentAdmin] da
            INNER JOIN ' + QUOTENAME(@MainDb) + N'.[dbo].[AdminType] at
                ON at.[AdminTypeID] = da.[AdminTypeID]
            WHERE da.[EmployeeID] = COALESCE(pp.[CreatedUserID], TRY_CONVERT(uniqueidentifier, fs.[UserId]))
              AND NULLIF(LTRIM(RTRIM(at.[AdminTypeShort])), N'''') IS NOT NULL
        ) x
    ) assignerAdmin
)
INSERT INTO [dbo].[PostPositionFlow_Audit] (
    [EventType],
    [PostPositionId],
    [RelatedPostPositionId],
    [PostId],
    [RequestId],
    [DepartmentID],
    [EmployeeId],
    [EmployeeName],
    [PerformedByEmployeeId],
    [PerformedByName],
    [Description],
    [DetailsJson],
    [PostTitle],
    [RecordedDate]
)
SELECT
    fc.[ClassifiedEventType] AS [EventType],
    fc.[PostPositionId],
    fc.[RelatedPostPositionId],
    fc.[PostID],
    fc.[RequestID],
    fc.[DepartmentID],
    fc.[EmployeeId],
    fc.[EmployeeName],
    CASE WHEN fc.[ClassifiedEventType] = 16 THEN NULL ELSE fc.[PerformerEmployeeId] END AS [PerformedByEmployeeId],
    CASE WHEN fc.[ClassifiedEventType] = 16 THEN N''System'' ELSE fc.[PerformerName] END AS [PerformedByName],
    LEFT(
        CASE fc.[ClassifiedEventType]
            WHEN 2 THEN
                CASE
                    WHEN NULLIF(fc.[EmployeeName], N'''') IS NOT NULL
                        THEN CONCAT(N''Self taken by '', fc.[EmployeeName])
                    ELSE N''Self taken''
                END
            WHEN 3 THEN
                CASE
                    WHEN NULLIF(fc.[EmployeeName], N'''') IS NOT NULL
                        THEN CONCAT(N''Assigned by '', fc.[PerformerName], N'' to '', fc.[EmployeeName])
                    ELSE CONCAT(N''Assigned by '', fc.[PerformerName])
                END
            WHEN 15 THEN
                CASE
                    WHEN NULLIF(fc.[EmployeeName], N'''') IS NOT NULL
                        THEN CONCAT(N''Assigned by '', fc.[PerformerName], N'' to '', fc.[EmployeeName])
                    ELSE CONCAT(N''Assigned by '', fc.[PerformerName])
                END
            WHEN 16 THEN
                CASE
                    WHEN NULLIF(fc.[EmployeeName], N'''') IS NOT NULL
                        THEN CONCAT(N''Assigned by System to '', fc.[EmployeeName])
                    ELSE N''Assigned by System''
                END
            WHEN 4 THEN CONCAT(
                N''Transferred by admin from '',
                COALESCE(fc.[EmployeeOldName], N''previous employee''),
                N'' to '',
                COALESCE(fc.[EmployeeNewName], N''employee''))
            WHEN 6 THEN N''Position closed''
            WHEN 7 THEN N''Position closed and split''
            WHEN 8 THEN N''Cancellation requested''
            WHEN 9 THEN N''Position cancelled''
            WHEN 10 THEN N''Split assignment accepted''
            WHEN 11 THEN N''Split assignment declined''
            WHEN 12 THEN N''Position unclosed''
            WHEN 14 THEN N''Split assignment pending decision''
            WHEN 5 THEN CONCAT(N''Unassigned by admin '', fc.[PerformerName])
            ELSE N''Job position updated''
        END, 512) AS [Description],
    CASE fc.[ClassifiedEventType]
        WHEN 2 THEN CONCAT(
            N''[{""label"":""Taken by"",""value"":""'',
            STRING_ESCAPE(ISNULL(fc.[EmployeeName], N''''), ''json''),
            N''""},{""label"":""Assignment type"",""value"":""Self take""}]'')
        WHEN 3 THEN CONCAT(
            N''[{""label"":""Assigned by"",""value"":""'',
            STRING_ESCAPE(fc.[PerformerName], ''json''),
            N''""},{""label"":""Assigned to"",""value"":""'',
            STRING_ESCAPE(ISNULL(fc.[EmployeeName], N''''), ''json''),
            N''""},{""label"":""Assignment type"",""value"":""'',
            STRING_ESCAPE(COALESCE(fc.[AssignmentType], N''Admin''), ''json''),
            N''""}]'')
        WHEN 15 THEN CONCAT(
            N''[{""label"":""Assigned by"",""value"":""'',
            STRING_ESCAPE(fc.[PerformerName], ''json''),
            N''""},{""label"":""Assigned to"",""value"":""'',
            STRING_ESCAPE(ISNULL(fc.[EmployeeName], N''''), ''json''),
            N''""},{""label"":""Assignment type"",""value"":""Assigned by other employee""}]'')
        WHEN 16 THEN CONCAT(
            N''[{""label"":""Assigned by"",""value"":""System""},{""label"":""Assigned to"",""value"":""'',
            STRING_ESCAPE(ISNULL(fc.[EmployeeName], N''''), ''json''),
            N''""},{""label"":""Assignment type"",""value"":""System""}]'')
        WHEN 4 THEN CONCAT(
            N''[{""label"":""Transferred by"",""value"":""'',
            STRING_ESCAPE(fc.[PerformerName], ''json''),
            N''""},{""label"":""From"",""value"":""'',
            STRING_ESCAPE(ISNULL(fc.[EmployeeOldName], N''''), ''json''),
            N''""},{""label"":""To"",""value"":""'',
            STRING_ESCAPE(ISNULL(fc.[EmployeeNewName], N''''), ''json''),
            N''""}]'')
        WHEN 5 THEN CONCAT(
            N''[{""label"":""Unassigned by"",""value"":""'',
            STRING_ESCAPE(fc.[PerformerName], ''json''),
            N''""},{""label"":""Employee"",""value"":""'',
            STRING_ESCAPE(ISNULL(fc.[EmployeeName], N''''), ''json''),
            N''""}]'')
        WHEN 6 THEN CONCAT(
            N''[{""label"":""Closed by"",""value"":""'',
            STRING_ESCAPE(fc.[PerformerName], ''json''),
            N''""},{""label"":""Employee"",""value"":""'',
            STRING_ESCAPE(ISNULL(fc.[EmployeeName], N''''), ''json''),
            N''""}]'')
        WHEN 7 THEN CONCAT(
            N''[{""label"":""Closed by"",""value"":""'',
            STRING_ESCAPE(fc.[PerformerName], ''json''),
            N''""},{""label"":""Employee"",""value"":""'',
            STRING_ESCAPE(ISNULL(fc.[EmployeeName], N''''), ''json''),
            N''""}]'')
        WHEN 8 THEN
            CASE
                WHEN NULLIF(fc.[CancelReason], N'''') IS NOT NULL
                    THEN CONCAT(
                        N''[{""label"":""Cancelled by"",""value"":""'',
                        STRING_ESCAPE(fc.[PerformerName], ''json''),
                        N''""},{""label"":""Employee"",""value"":""'',
                        STRING_ESCAPE(ISNULL(fc.[EmployeeName], N''''), ''json''),
                        N''""},{""label"":""Reason"",""value"":""'',
                        STRING_ESCAPE(fc.[CancelReason], ''json''),
                        N''""}]'')
                ELSE CONCAT(
                    N''[{""label"":""Cancelled by"",""value"":""'',
                    STRING_ESCAPE(fc.[PerformerName], ''json''),
                    N''""},{""label"":""Employee"",""value"":""'',
                    STRING_ESCAPE(ISNULL(fc.[EmployeeName], N''''), ''json''),
                    N''""}]'')
            END
        WHEN 9 THEN
            CASE
                WHEN NULLIF(fc.[CancelReason], N'''') IS NOT NULL
                    THEN CONCAT(
                        N''[{""label"":""Cancelled by"",""value"":""'',
                        STRING_ESCAPE(fc.[PerformerName], ''json''),
                        N''""},{""label"":""Employee"",""value"":""'',
                        STRING_ESCAPE(ISNULL(fc.[EmployeeName], N''''), ''json''),
                        N''""},{""label"":""Reason"",""value"":""'',
                        STRING_ESCAPE(fc.[CancelReason], ''json''),
                        N''""}]'')
                ELSE CONCAT(
                    N''[{""label"":""Cancelled by"",""value"":""'',
                    STRING_ESCAPE(fc.[PerformerName], ''json''),
                    N''""},{""label"":""Employee"",""value"":""'',
                    STRING_ESCAPE(ISNULL(fc.[EmployeeName], N''''), ''json''),
                    N''""}]'')
            END
        WHEN 10 THEN CONCAT(
            N''[{""label"":""Decision"",""value"":""Accepted""},{""label"":""Employee"",""value"":""'',
            STRING_ESCAPE(ISNULL(fc.[EmployeeName], N''''), ''json''),
            N''""}]'')
        WHEN 11 THEN CONCAT(
            N''[{""label"":""Decision"",""value"":""Declined""},{""label"":""Employee"",""value"":""'',
            STRING_ESCAPE(ISNULL(fc.[EmployeeName], N''''), ''json''),
            N''""}]'')
        WHEN 14 THEN CONCAT(
            N''[{""label"":""Split from"",""value"":""'',
            STRING_ESCAPE(ISNULL(fc.[SplitFromEmployeeName], N''''), ''json''),
            N''""},{""label"":""Offered to"",""value"":""'',
            STRING_ESCAPE(ISNULL(fc.[EmployeeName], N''''), ''json''),
            N''""},{""label"":""Status"",""value"":""Split decision pending""}]'')
        ELSE NULL
    END AS [DetailsJson],
    LEFT(NULLIF(LTRIM(RTRIM(fc.[DisplayName])), N''''), 200) AS [PostTitle],
    fc.[ChangedOn] AS [RecordedDate]
FROM FlowClassified fc
WHERE fc.[ClassifiedEventType] IS NOT NULL
  AND NOT EXISTS (
      SELECT 1
      FROM [dbo].[PostPositionFlow_Audit] existing
      WHERE existing.[PostPositionId] = fc.[PostPositionId]
        AND existing.[EventType] = fc.[ClassifiedEventType]
        AND existing.[RecordedDate] = fc.[ChangedOn]
  )
ORDER BY fc.[PostPositionId], fc.[ChangedOn], fc.[AuditId];
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
