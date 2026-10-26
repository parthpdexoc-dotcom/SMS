using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedBaseEntityFromDepartmentPolicyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentPolicy_Employee_CreatedUserID",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentPolicy_Employee_EditedUserID",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentPolicy_Employee_CreatedUserID",
                schema: "dbo",
                table: "DepartmentPolicy",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentPolicy_Employee_EditedUserID",
                schema: "dbo",
                table: "DepartmentPolicy",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentPolicy_Employee_CreatedUserID",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentPolicy_Employee_EditedUserID",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentPolicy_Employee_CreatedUserID",
                schema: "dbo",
                table: "DepartmentPolicy",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentPolicy_Employee_EditedUserID",
                schema: "dbo",
                table: "DepartmentPolicy",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
