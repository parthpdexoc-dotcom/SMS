using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations.Audit
{
    /// <inheritdoc />
    public partial class AddedNewCronJobs_AuditTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeJob_Audit",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CronExpression = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ScheduledSlotUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Succeeded = table.Column<bool>(type: "bit", nullable: false),
                    FailureSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeJob_Audit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ptaeo_Audit",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CronExpression = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ScheduledSlotUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Succeeded = table.Column<bool>(type: "bit", nullable: false),
                    FailureSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ptaeo_Audit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UpdatePreferredToManaged_Audit",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CronExpression = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ScheduledSlotUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Succeeded = table.Column<bool>(type: "bit", nullable: false),
                    FailureSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdatePreferredToManaged_Audit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UpdateReleasedOnToReleased_Audit",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CronExpression = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ScheduledSlotUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Succeeded = table.Column<bool>(type: "bit", nullable: false),
                    FailureSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdateReleasedOnToReleased_Audit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WaitingPostPosition_Audit",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CronExpression = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ScheduledSlotUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Succeeded = table.Column<bool>(type: "bit", nullable: false),
                    FailureSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaitingPostPosition_Audit", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeJob_Audit",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Ptaeo_Audit",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UpdatePreferredToManaged_Audit",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UpdateReleasedOnToReleased_Audit",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WaitingPostPosition_Audit",
                schema: "dbo");
        }
    }
}
