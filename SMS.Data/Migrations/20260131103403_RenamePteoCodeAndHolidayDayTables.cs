using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenamePteoCodeAndHolidayDayTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Holiday_Days_Department_DepartmentID",
                schema: "dbo",
                table: "Holiday_Days");

            migrationBuilder.DropForeignKey(
                name: "FK_Holiday_Days_Employee_CreatedUserID",
                schema: "dbo",
                table: "Holiday_Days");

            migrationBuilder.DropForeignKey(
                name: "FK_Holiday_Days_Employee_EditedUserID",
                schema: "dbo",
                table: "Holiday_Days");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PTEO_Code",
                schema: "dbo",
                table: "PTEO_Code");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Holiday_Days",
                schema: "dbo",
                table: "Holiday_Days");

            migrationBuilder.RenameTable(
                name: "PTEO_Code",
                schema: "dbo",
                newName: "PteoCode",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Holiday_Days",
                schema: "dbo",
                newName: "HolidayDay",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_Holiday_Days_EditedUserID",
                schema: "dbo",
                table: "HolidayDay",
                newName: "IX_HolidayDay_EditedUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Holiday_Days_DepartmentID",
                schema: "dbo",
                table: "HolidayDay",
                newName: "IX_HolidayDay_DepartmentID");

            migrationBuilder.RenameIndex(
                name: "IX_Holiday_Days_CreatedUserID",
                schema: "dbo",
                table: "HolidayDay",
                newName: "IX_HolidayDay_CreatedUserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PteoCode",
                schema: "dbo",
                table: "PteoCode",
                column: "PteoCodeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HolidayDay",
                schema: "dbo",
                table: "HolidayDay",
                column: "HolidayDayID");

            migrationBuilder.AddForeignKey(
                name: "FK_HolidayDay_Department_DepartmentID",
                schema: "dbo",
                table: "HolidayDay",
                column: "DepartmentID",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HolidayDay_Employee_CreatedUserID",
                schema: "dbo",
                table: "HolidayDay",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HolidayDay_Employee_EditedUserID",
                schema: "dbo",
                table: "HolidayDay",
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
                name: "FK_HolidayDay_Department_DepartmentID",
                schema: "dbo",
                table: "HolidayDay");

            migrationBuilder.DropForeignKey(
                name: "FK_HolidayDay_Employee_CreatedUserID",
                schema: "dbo",
                table: "HolidayDay");

            migrationBuilder.DropForeignKey(
                name: "FK_HolidayDay_Employee_EditedUserID",
                schema: "dbo",
                table: "HolidayDay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PteoCode",
                schema: "dbo",
                table: "PteoCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HolidayDay",
                schema: "dbo",
                table: "HolidayDay");

            migrationBuilder.RenameTable(
                name: "PteoCode",
                schema: "dbo",
                newName: "PTEO_Code",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "HolidayDay",
                schema: "dbo",
                newName: "Holiday_Days",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_HolidayDay_EditedUserID",
                schema: "dbo",
                table: "Holiday_Days",
                newName: "IX_Holiday_Days_EditedUserID");

            migrationBuilder.RenameIndex(
                name: "IX_HolidayDay_DepartmentID",
                schema: "dbo",
                table: "Holiday_Days",
                newName: "IX_Holiday_Days_DepartmentID");

            migrationBuilder.RenameIndex(
                name: "IX_HolidayDay_CreatedUserID",
                schema: "dbo",
                table: "Holiday_Days",
                newName: "IX_Holiday_Days_CreatedUserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PTEO_Code",
                schema: "dbo",
                table: "PTEO_Code",
                column: "PteoCodeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Holiday_Days",
                schema: "dbo",
                table: "Holiday_Days",
                column: "HolidayDayID");

            migrationBuilder.AddForeignKey(
                name: "FK_Holiday_Days_Department_DepartmentID",
                schema: "dbo",
                table: "Holiday_Days",
                column: "DepartmentID",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Holiday_Days_Employee_CreatedUserID",
                schema: "dbo",
                table: "Holiday_Days",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Holiday_Days_Employee_EditedUserID",
                schema: "dbo",
                table: "Holiday_Days",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
