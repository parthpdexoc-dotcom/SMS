using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameRequesterManagerKeyNameAndRequesterEmployeetableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requester_Employees_Employee_CreatedUserID",
                schema: "dbo",
                table: "Requester_Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Requester_Employees_Employee_EditedUserID",
                schema: "dbo",
                table: "Requester_Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Requester_Employees_Employee_EmployeeID",
                schema: "dbo",
                table: "Requester_Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Requester_Employees_RequesterManager_RequesterManagerID",
                schema: "dbo",
                table: "Requester_Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Requester_Employees",
                schema: "dbo",
                table: "Requester_Employees");

            migrationBuilder.RenameTable(
                name: "Requester_Employees",
                schema: "dbo",
                newName: "RequesterEmployee",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "PrivateJobManagerID",
                schema: "dbo",
                table: "RequesterManager",
                newName: "RequesterManagerID");

            migrationBuilder.RenameIndex(
                name: "IX_Requester_Employees_RequesterManagerID",
                schema: "dbo",
                table: "RequesterEmployee",
                newName: "IX_RequesterEmployee_RequesterManagerID");

            migrationBuilder.RenameIndex(
                name: "IX_Requester_Employees_EmployeeID",
                schema: "dbo",
                table: "RequesterEmployee",
                newName: "IX_RequesterEmployee_EmployeeID");

            migrationBuilder.RenameIndex(
                name: "IX_Requester_Employees_EditedUserID",
                schema: "dbo",
                table: "RequesterEmployee",
                newName: "IX_RequesterEmployee_EditedUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Requester_Employees_CreatedUserID",
                schema: "dbo",
                table: "RequesterEmployee",
                newName: "IX_RequesterEmployee_CreatedUserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequesterEmployee",
                schema: "dbo",
                table: "RequesterEmployee",
                column: "RequesterEmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterEmployee_Employee_CreatedUserID",
                schema: "dbo",
                table: "RequesterEmployee",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterEmployee_Employee_EditedUserID",
                schema: "dbo",
                table: "RequesterEmployee",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterEmployee_Employee_EmployeeID",
                schema: "dbo",
                table: "RequesterEmployee",
                column: "EmployeeID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterEmployee_RequesterManager_RequesterManagerID",
                schema: "dbo",
                table: "RequesterEmployee",
                column: "RequesterManagerID",
                principalSchema: "dbo",
                principalTable: "RequesterManager",
                principalColumn: "RequesterManagerID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequesterEmployee_Employee_CreatedUserID",
                schema: "dbo",
                table: "RequesterEmployee");

            migrationBuilder.DropForeignKey(
                name: "FK_RequesterEmployee_Employee_EditedUserID",
                schema: "dbo",
                table: "RequesterEmployee");

            migrationBuilder.DropForeignKey(
                name: "FK_RequesterEmployee_Employee_EmployeeID",
                schema: "dbo",
                table: "RequesterEmployee");

            migrationBuilder.DropForeignKey(
                name: "FK_RequesterEmployee_RequesterManager_RequesterManagerID",
                schema: "dbo",
                table: "RequesterEmployee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequesterEmployee",
                schema: "dbo",
                table: "RequesterEmployee");

            migrationBuilder.RenameTable(
                name: "RequesterEmployee",
                schema: "dbo",
                newName: "Requester_Employees",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "RequesterManagerID",
                schema: "dbo",
                table: "RequesterManager",
                newName: "PrivateJobManagerID");

            migrationBuilder.RenameIndex(
                name: "IX_RequesterEmployee_RequesterManagerID",
                schema: "dbo",
                table: "Requester_Employees",
                newName: "IX_Requester_Employees_RequesterManagerID");

            migrationBuilder.RenameIndex(
                name: "IX_RequesterEmployee_EmployeeID",
                schema: "dbo",
                table: "Requester_Employees",
                newName: "IX_Requester_Employees_EmployeeID");

            migrationBuilder.RenameIndex(
                name: "IX_RequesterEmployee_EditedUserID",
                schema: "dbo",
                table: "Requester_Employees",
                newName: "IX_Requester_Employees_EditedUserID");

            migrationBuilder.RenameIndex(
                name: "IX_RequesterEmployee_CreatedUserID",
                schema: "dbo",
                table: "Requester_Employees",
                newName: "IX_Requester_Employees_CreatedUserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Requester_Employees",
                schema: "dbo",
                table: "Requester_Employees",
                column: "RequesterEmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Requester_Employees_Employee_CreatedUserID",
                schema: "dbo",
                table: "Requester_Employees",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requester_Employees_Employee_EditedUserID",
                schema: "dbo",
                table: "Requester_Employees",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requester_Employees_Employee_EmployeeID",
                schema: "dbo",
                table: "Requester_Employees",
                column: "EmployeeID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requester_Employees_RequesterManager_RequesterManagerID",
                schema: "dbo",
                table: "Requester_Employees",
                column: "RequesterManagerID",
                principalSchema: "dbo",
                principalTable: "RequesterManager",
                principalColumn: "PrivateJobManagerID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
