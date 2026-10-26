using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostWaitingList",
                schema: "dbo");

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedAt",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessingStatus",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ResultMessage",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusCode",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedAt",
                schema: "dbo",
                table: "RequestSchedule",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessingJobId",
                schema: "dbo",
                table: "RequestSchedule",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessingResult",
                schema: "dbo",
                table: "RequestSchedule",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessingStatus",
                schema: "dbo",
                table: "RequestSchedule",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "ProcessingStatus",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "ResultMessage",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "StatusCode",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                schema: "dbo",
                table: "RequestSchedule");

            migrationBuilder.DropColumn(
                name: "ProcessingJobId",
                schema: "dbo",
                table: "RequestSchedule");

            migrationBuilder.DropColumn(
                name: "ProcessingResult",
                schema: "dbo",
                table: "RequestSchedule");

            migrationBuilder.DropColumn(
                name: "ProcessingStatus",
                schema: "dbo",
                table: "RequestSchedule");

            migrationBuilder.CreateTable(
                name: "PostWaitingList",
                schema: "dbo",
                columns: table => new
                {
                    PostWaitingListID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    AssignedPostPositionID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateOfWork = table.Column<DateOnly>(type: "date", nullable: false),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostWaitingList", x => x.PostWaitingListID);
                    table.ForeignKey(
                        name: "FK_PostWaitingList_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostWaitingList_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostWaitingList_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostWaitingList_PostPosition_AssignedPostPositionID",
                        column: x => x.AssignedPostPositionID,
                        principalSchema: "dbo",
                        principalTable: "PostPosition",
                        principalColumn: "PostPositionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostWaitingList_Post_PostID",
                        column: x => x.PostID,
                        principalSchema: "dbo",
                        principalTable: "Post",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostWaitingList_AssignedPostPositionID",
                schema: "dbo",
                table: "PostWaitingList",
                column: "AssignedPostPositionID");

            migrationBuilder.CreateIndex(
                name: "IX_PostWaitingList_CreatedUserID",
                schema: "dbo",
                table: "PostWaitingList",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PostWaitingList_EditedUserID",
                schema: "dbo",
                table: "PostWaitingList",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PostWaitingList_EmployeeID",
                schema: "dbo",
                table: "PostWaitingList",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_PostWaitingList_PostID_EmployeeID_DateOfWork",
                schema: "dbo",
                table: "PostWaitingList",
                columns: new[] { "PostID", "EmployeeID", "DateOfWork" },
                unique: true,
                filter: "[Status] = 0");
        }
    }
}
