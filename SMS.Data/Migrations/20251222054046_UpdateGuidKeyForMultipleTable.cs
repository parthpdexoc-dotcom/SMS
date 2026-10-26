using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGuidKeyForMultipleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentDivisions_Department_DepartmentID",
                table: "DepartmentDivisions");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentDivisions_Employee_CreatedUserID",
                table: "DepartmentDivisions");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentDivisions_Employee_EditedUserID",
                table: "DepartmentDivisions");

            migrationBuilder.RenameTable(
                name: "DepartmentDivisions",
                newName: "DepartmentDivisions",
                newSchema: "dbo");

            migrationBuilder.AlterColumn<Guid>(
                name: "EquipementTypeID",
                schema: "dbo",
                table: "EquipmentType",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "newid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdminTypeID",
                schema: "dbo",
                table: "AdminType",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "newid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "DivisionID",
                schema: "dbo",
                table: "DepartmentDivisions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "newid()");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentDivisions_Department_DepartmentID",
                schema: "dbo",
                table: "DepartmentDivisions",
                column: "DepartmentID",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentDivisions_Employee_CreatedUserID",
                schema: "dbo",
                table: "DepartmentDivisions",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentDivisions_Employee_EditedUserID",
                schema: "dbo",
                table: "DepartmentDivisions",
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
                name: "FK_DepartmentDivisions_Department_DepartmentID",
                schema: "dbo",
                table: "DepartmentDivisions");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentDivisions_Employee_CreatedUserID",
                schema: "dbo",
                table: "DepartmentDivisions");

            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentDivisions_Employee_EditedUserID",
                schema: "dbo",
                table: "DepartmentDivisions");

            migrationBuilder.RenameTable(
                name: "DepartmentDivisions",
                schema: "dbo",
                newName: "DepartmentDivisions");

            migrationBuilder.AlterColumn<Guid>(
                name: "EquipementTypeID",
                schema: "dbo",
                table: "EquipmentType",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "newid()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdminTypeID",
                schema: "dbo",
                table: "AdminType",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "newid()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()");

            migrationBuilder.AlterColumn<Guid>(
                name: "DivisionID",
                table: "DepartmentDivisions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "newid()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentDivisions_Department_DepartmentID",
                table: "DepartmentDivisions",
                column: "DepartmentID",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentDivisions_Employee_CreatedUserID",
                table: "DepartmentDivisions",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentDivisions_Employee_EditedUserID",
                table: "DepartmentDivisions",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID");
        }
    }
}
