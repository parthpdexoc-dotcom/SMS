using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations.Audit
{
    /// <summary>
    /// Drops <c>PerformedBy</c> and <c>CorrelationId</c> from <c>PostPositionFlow_Audit</c>.
    /// Assign role is carried by <c>EventType</c>
    /// (AssignedByAdmin / AssignedByOther / AssignedBySystem / SelfTaken).
    /// Backfills legacy AssignedByAdmin rows that were performed by employee/system.
    /// </summary>
    public partial class DropPostPositionFlowAuditPerformedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
-- Legacy: EventType AssignedByAdmin (3) + PerformedBy Employee (2) → AssignedByOther (15)
UPDATE [dbo].[PostPositionFlow_Audit]
SET
    [EventType] = 15,
    [Description] = LEFT(REPLACE([Description], N'Assigned by admin ', N'Assigned by '), 512),
    [DetailsJson] = REPLACE(
        REPLACE([DetailsJson], N'""Admin assign""', N'""Assigned by other employee""'),
        N'""Employee assign""', N'""Assigned by other employee""')
WHERE [EventType] = 3
  AND [PerformedBy] = 2;

-- Legacy: EventType AssignedByAdmin (3) + PerformedBy System (0) → AssignedBySystem (16)
UPDATE [dbo].[PostPositionFlow_Audit]
SET
    [EventType] = 16,
    [Description] = LEFT(
        CASE
            WHEN NULLIF(LTRIM(RTRIM([EmployeeName])), N'') IS NOT NULL
                THEN CONCAT(N'Assigned by System to ', LTRIM(RTRIM([EmployeeName])))
            ELSE N'Assigned by System'
        END, 512),
    [DetailsJson] = CONCAT(
        N'[{""label"":""Assigned by"",""value"":""System""},{""label"":""Assigned to"",""value"":""',
        STRING_ESCAPE(ISNULL([EmployeeName], N''), 'json'),
        N'""},{""label"":""Assignment type"",""value"":""System""}]'),
    [PerformedByEmployeeId] = NULL,
    [PerformedByName] = N'System'
WHERE [EventType] = 3
  AND [PerformedBy] = 0;

-- Strip remaining ""admin"" wording on true admin assigns
UPDATE [dbo].[PostPositionFlow_Audit]
SET [Description] = LEFT(REPLACE([Description], N'Assigned by admin ', N'Assigned by '), 512)
WHERE [EventType] = 3
  AND [Description] LIKE N'%Assigned by admin %';
");

            migrationBuilder.DropColumn(
                name: "PerformedBy",
                schema: "dbo",
                table: "PostPositionFlow_Audit");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                schema: "dbo",
                table: "PostPositionFlow_Audit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                schema: "dbo",
                table: "PostPositionFlow_Audit",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerformedBy",
                schema: "dbo",
                table: "PostPositionFlow_Audit",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
UPDATE [dbo].[PostPositionFlow_Audit]
SET [PerformedBy] = CASE [EventType]
    WHEN 16 THEN 0
    WHEN 3 THEN 1
    WHEN 15 THEN 2
    WHEN 2 THEN 2
    ELSE 2
END;

UPDATE [dbo].[PostPositionFlow_Audit]
SET [EventType] = 3
WHERE [EventType] IN (15, 16);
");
        }
    }
}
