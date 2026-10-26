using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class DepartmentCronJobManagement_AuditToAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentCronJobManagement_Employee_CreatedUserID",
                schema: "dbo",
                table: "DepartmentCronJobManagement");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentCronJobManagement_Employee_EditedUserID",
                schema: "dbo",
                table: "DepartmentCronJobManagement");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentCronJobManagement_Admin_CreatedUserID",
                schema: "dbo",
                table: "DepartmentCronJobManagement",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Admin",
                principalColumn: "AdminID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentCronJobManagement_Admin_EditedUserID",
                schema: "dbo",
                table: "DepartmentCronJobManagement",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "Admin",
                principalColumn: "AdminID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentCronJobManagement_Admin_CreatedUserID",
                schema: "dbo",
                table: "DepartmentCronJobManagement");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentCronJobManagement_Admin_EditedUserID",
                schema: "dbo",
                table: "DepartmentCronJobManagement");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentCronJobManagement_Employee_CreatedUserID",
                schema: "dbo",
                table: "DepartmentCronJobManagement",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentCronJobManagement_Employee_EditedUserID",
                schema: "dbo",
                table: "DepartmentCronJobManagement",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
