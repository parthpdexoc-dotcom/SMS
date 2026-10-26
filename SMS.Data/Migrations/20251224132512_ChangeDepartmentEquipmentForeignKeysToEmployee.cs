using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDepartmentEquipmentForeignKeysToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentEquipment_Admin_CreatedUserID",
                schema: "dbo",
                table: "DepartmentEquipment");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentEquipment_Admin_EditedUserID",
                schema: "dbo",
                table: "DepartmentEquipment");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentEquipment_Employee_CreatedUserID",
                schema: "dbo",
                table: "DepartmentEquipment",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentEquipment_Employee_EditedUserID",
                schema: "dbo",
                table: "DepartmentEquipment",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentEquipment_Employee_CreatedUserID",
                schema: "dbo",
                table: "DepartmentEquipment");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentEquipment_Employee_EditedUserID",
                schema: "dbo",
                table: "DepartmentEquipment");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentEquipment_Admin_CreatedUserID",
                schema: "dbo",
                table: "DepartmentEquipment",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Admin",
                principalColumn: "AdminID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentEquipment_Admin_EditedUserID",
                schema: "dbo",
                table: "DepartmentEquipment",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "Admin",
                principalColumn: "AdminID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
