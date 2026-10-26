using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DivisionID",
                schema: "dbo",
                table: "Employee",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeDepartmentDefinedField1",
                schema: "dbo",
                table: "Employee",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeDepartmentDefinedField2",
                schema: "dbo",
                table: "Employee",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeDepartmentDefinedField3",
                schema: "dbo",
                table: "Employee",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeDepartmentDefinedField4",
                schema: "dbo",
                table: "Employee",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HourlySalary",
                schema: "dbo",
                table: "Employee",
                type: "decimal(18,5)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HrCostCenterCode",
                schema: "dbo",
                table: "Employee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_DivisionID",
                schema: "dbo",
                table: "Employee",
                column: "DivisionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_DepartmentDivisions_DivisionID",
                schema: "dbo",
                table: "Employee",
                column: "DivisionID",
                principalSchema: "dbo",
                principalTable: "DepartmentDivisions",
                principalColumn: "DivisionID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_DepartmentDivisions_DivisionID",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_DivisionID",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "DivisionID",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "EmployeeDepartmentDefinedField1",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "EmployeeDepartmentDefinedField2",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "EmployeeDepartmentDefinedField3",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "EmployeeDepartmentDefinedField4",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "HourlySalary",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "HrCostCenterCode",
                schema: "dbo",
                table: "Employee");
        }
    }
}
