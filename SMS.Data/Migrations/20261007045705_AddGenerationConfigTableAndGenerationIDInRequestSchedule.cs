using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGenerationConfigTableAndGenerationIDInRequestSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestScheduleGeneration",
                schema: "dbo",
                columns: table => new
                {
                    RequestScheduleGenerationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    RequestType = table.Column<int>(type: "int", nullable: false),
                    RequestStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RequestEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveFromDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReleaseBeforeDaysOfWeek = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReleaseBeforeDaysOfMonth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReleaseBeforeMonthlyMode = table.Column<int>(type: "int", nullable: true),
                    ReleaseBeforeWeekIndex = table.Column<int>(type: "int", nullable: true),
                    ScheduledTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestScheduleGeneration", x => x.RequestScheduleGenerationID);
                    table.ForeignKey(
                        name: "FK_RequestScheduleGeneration_Request_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "dbo",
                        principalTable: "Request",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestScheduleGeneration_RequestId",
                schema: "dbo",
                table: "RequestScheduleGeneration",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestScheduleGeneration",
                schema: "dbo");
        }
    }
}
