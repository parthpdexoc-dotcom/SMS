using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedConstraintsInWaitingListTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WaitingPostPosition_EmployeeID_PostID_DateOfWork_PostPositionStatus",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropIndex(
                name: "IX_WaitingPostPosition_EmployeeID_RequestID_DateOfWork_PostPositionStatus",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingPostPosition_EmployeeID_PostID_DateOfWork",
                schema: "dbo",
                table: "WaitingPostPosition",
                columns: new[] { "EmployeeID", "PostID", "DateOfWork" },
                unique: true,
                filter: "[PostID] IS NOT NULL AND [PostPositionStatus] = 10");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingPostPosition_EmployeeID_RequestID_DateOfWork",
                schema: "dbo",
                table: "WaitingPostPosition",
                columns: new[] { "EmployeeID", "RequestID", "DateOfWork" },
                unique: true,
                filter: "[RequestID] IS NOT NULL AND [PostPositionStatus] = 15");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WaitingPostPosition_EmployeeID_PostID_DateOfWork",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropIndex(
                name: "IX_WaitingPostPosition_EmployeeID_RequestID_DateOfWork",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingPostPosition_EmployeeID_PostID_DateOfWork_PostPositionStatus",
                schema: "dbo",
                table: "WaitingPostPosition",
                columns: new[] { "EmployeeID", "PostID", "DateOfWork", "PostPositionStatus" },
                unique: true,
                filter: "[PostID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingPostPosition_EmployeeID_RequestID_DateOfWork_PostPositionStatus",
                schema: "dbo",
                table: "WaitingPostPosition",
                columns: new[] { "EmployeeID", "RequestID", "DateOfWork", "PostPositionStatus" },
                unique: true,
                filter: "[RequestID] IS NOT NULL");
        }
    }
}
