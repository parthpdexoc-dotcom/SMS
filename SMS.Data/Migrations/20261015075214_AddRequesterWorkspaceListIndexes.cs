using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRequesterWorkspaceListIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestSchedule_RequestId",
                schema: "dbo",
                table: "RequestSchedule");

            migrationBuilder.DropIndex(
                name: "IX_Request_DepartmentID",
                schema: "dbo",
                table: "Request");

            migrationBuilder.CreateIndex(
                name: "IX_RequestSchedule_RequestId_RequestStatus",
                schema: "dbo",
                table: "RequestSchedule",
                columns: new[] { "RequestId", "RequestStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Request_DepartmentID_RequesterID",
                schema: "dbo",
                table: "Request",
                columns: new[] { "DepartmentID", "RequesterID" });

            migrationBuilder.CreateIndex(
                name: "IX_Request_DepartmentID_StartDate_EndDate",
                schema: "dbo",
                table: "Request",
                columns: new[] { "DepartmentID", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PostPosition_DateOfWork_PostID",
                schema: "dbo",
                table: "PostPosition",
                columns: new[] { "DateOfWork", "PostID" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestSchedule_RequestId_RequestStatus",
                schema: "dbo",
                table: "RequestSchedule");

            migrationBuilder.DropIndex(
                name: "IX_Request_DepartmentID_RequesterID",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_DepartmentID_StartDate_EndDate",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_PostPosition_DateOfWork_PostID",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.CreateIndex(
                name: "IX_RequestSchedule_RequestId",
                schema: "dbo",
                table: "RequestSchedule",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_DepartmentID",
                schema: "dbo",
                table: "Request",
                column: "DepartmentID");
        }
    }
}
