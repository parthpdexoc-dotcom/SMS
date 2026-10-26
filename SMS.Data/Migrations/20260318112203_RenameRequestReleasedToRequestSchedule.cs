using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameRequestReleasedToRequestSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestReleased",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "ProcessDate",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropColumn(
                name: "ProcessTime",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropColumn(
                name: "ReleaseAs",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "ReleaseDays",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "ReleaseEndDate",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "RequestStatus",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "ScheduleDate",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "ScheduledTime",
                schema: "dbo",
                table: "Request");

            migrationBuilder.CreateTable(
                name: "RequestSchedule",
                schema: "dbo",
                columns: table => new
                {
                    RequestScheduleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ReleaseAs = table.Column<int>(type: "int", nullable: false),
                    RequestStatus = table.Column<int>(type: "int", nullable: false),
                    ScheduleDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ScheduledTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    ProcessDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ProcessTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestSchedule", x => x.RequestScheduleID);
                    table.ForeignKey(
                        name: "FK_RequestSchedule_Request_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "dbo",
                        principalTable: "Request",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestSchedule_RequestId",
                schema: "dbo",
                table: "RequestSchedule",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestSchedule",
                schema: "dbo");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ProcessDate",
                schema: "dbo",
                table: "RequestType",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ProcessTime",
                schema: "dbo",
                table: "RequestType",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReleaseAs",
                schema: "dbo",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReleaseDays",
                schema: "dbo",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ReleaseEndDate",
                schema: "dbo",
                table: "Request",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestStatus",
                schema: "dbo",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ScheduleDate",
                schema: "dbo",
                table: "Request",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ScheduledTime",
                schema: "dbo",
                table: "Request",
                type: "time",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RequestReleased",
                schema: "dbo",
                columns: table => new
                {
                    RequestReleasedID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestReleased", x => x.RequestReleasedID);
                    table.ForeignKey(
                        name: "FK_RequestReleased_Request_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "dbo",
                        principalTable: "Request",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestReleased_RequestId",
                schema: "dbo",
                table: "RequestReleased",
                column: "RequestId");
        }
    }
}
