using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class renamedeptconfigfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnableRetainTenureListForReassignOnCancellations",
                schema: "dbo",
                table: "DepartmentConfiguration",
                newName: "ConsiderTenureWaitListOnCancellation");

            migrationBuilder.RenameColumn(
                name: "EnableRetainOpenDeptListForReassignOnCancellations",
                schema: "dbo",
                table: "DepartmentConfiguration",
                newName: "ConsiderOpenWaitListOnCancellation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConsiderTenureWaitListOnCancellation",
                schema: "dbo",
                table: "DepartmentConfiguration",
                newName: "EnableRetainTenureListForReassignOnCancellations");

            migrationBuilder.RenameColumn(
                name: "ConsiderOpenWaitListOnCancellation",
                schema: "dbo",
                table: "DepartmentConfiguration",
                newName: "EnableRetainOpenDeptListForReassignOnCancellations");
        }
    }
}
