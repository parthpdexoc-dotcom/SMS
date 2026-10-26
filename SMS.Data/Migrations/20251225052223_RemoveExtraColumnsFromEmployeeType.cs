using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExtraColumnsFromEmployeeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove foreign keys
    // 1️⃣ Drop Foreign Keys
    migrationBuilder.DropForeignKey(
        name: "FK_EmployeeType_Employee_EditedByEmployeeEmployeeID",
        table: "EmployeeType");

    migrationBuilder.DropForeignKey(
        name: "FK_EmployeeType_Employee_CreatedByEmployeeEmployeeID",
        table: "EmployeeType");

    migrationBuilder.DropForeignKey(
        name: "FK_EmployeeType_Department_DepartmentID1",
        table: "EmployeeType");

    // 2️⃣ Drop Indexes (THIS WAS MISSING)
    migrationBuilder.DropIndex(
        name: "IX_EmployeeType_EditedByEmployeeEmployeeID",
        table: "EmployeeType");

    migrationBuilder.DropIndex(
        name: "IX_EmployeeType_CreatedByEmployeeEmployeeID",
        table: "EmployeeType");

    migrationBuilder.DropIndex(
        name: "IX_EmployeeType_DepartmentID1",
        table: "EmployeeType");

    // 3️⃣ Drop Columns
    migrationBuilder.DropColumn(
        name: "EditedByEmployeeEmployeeID",
        table: "EmployeeType");

    migrationBuilder.DropColumn(
        name: "CreatedByEmployeeEmployeeID",
        table: "EmployeeType");

    migrationBuilder.DropColumn(
        name: "DepartmentID1",
        table: "EmployeeType");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
 migrationBuilder.AddColumn<Guid>(
        name: "EditedByEmployeeEmployeeID",
        table: "EmployeeType",
        type: "uniqueidentifier",
        nullable: true);

    migrationBuilder.AddColumn<Guid>(
        name: "CreatedByEmployeeEmployeeID",
        table: "EmployeeType",
        type: "uniqueidentifier",
        nullable: true);

    migrationBuilder.AddColumn<Guid>(
        name: "DepartmentID1",
        table: "EmployeeType",
        type: "uniqueidentifier",
        nullable: true);

    migrationBuilder.CreateIndex(
        name: "IX_EmployeeType_EditedByEmployeeEmployeeID",
        table: "EmployeeType",
        column: "EditedByEmployeeEmployeeID");

    migrationBuilder.CreateIndex(
        name: "IX_EmployeeType_CreatedByEmployeeEmployeeID",
        table: "EmployeeType",
        column: "CreatedByEmployeeEmployeeID");

    migrationBuilder.CreateIndex(
        name: "IX_EmployeeType_DepartmentID1",
        table: "EmployeeType",
        column: "DepartmentID1");

    migrationBuilder.AddForeignKey(
        name: "FK_EmployeeType_Employee_EditedByEmployeeEmployeeID",
        table: "EmployeeType",
        column: "EditedByEmployeeEmployeeID",
        principalTable: "Employee",
        principalColumn: "EmployeeID");

    migrationBuilder.AddForeignKey(
        name: "FK_EmployeeType_Employee_CreatedByEmployeeEmployeeID",
        table: "EmployeeType",
        column: "CreatedByEmployeeEmployeeID",
        principalTable: "Employee",
        principalColumn: "EmployeeID");

    migrationBuilder.AddForeignKey(
        name: "FK_EmployeeType_Department_DepartmentID1",
        table: "EmployeeType",
        column: "DepartmentID1",
        principalTable: "Department",
        principalColumn: "DepartmentID");
        }
    }
}
