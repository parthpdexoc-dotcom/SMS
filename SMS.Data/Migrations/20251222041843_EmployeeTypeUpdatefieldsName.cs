using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeTypeUpdatefieldsName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_EmployeesTypes_EmployeeTypeID",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeesTypes",
                schema: "dbo",
                table: "EmployeesTypes");

            migrationBuilder.RenameTable(
                name: "EmployeesTypes",
                schema: "dbo",
                newName: "EmployeeType",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "UserNameEditingRecord",
                schema: "dbo",
                table: "EmployeeType",
                newName: "EditedBy");

            migrationBuilder.RenameColumn(
                name: "UserEditingID",
                schema: "dbo",
                table: "EmployeeType",
                newName: "EditedUserID");

            migrationBuilder.RenameColumn(
                name: "RecordAddedDateTimeStamp",
                schema: "dbo",
                table: "EmployeeType",
                newName: "EditedDate");

            migrationBuilder.RenameColumn(
                name: "NameOfUserAddingRecord",
                schema: "dbo",
                table: "EmployeeType",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "EditDate",
                schema: "dbo",
                table: "EmployeeType",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "AddedByID",
                schema: "dbo",
                table: "EmployeeType",
                newName: "EditedByEmployeeEmployeeID");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeTypeShort",
                schema: "dbo",
                table: "EmployeeType",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeTypeLong",
                schema: "dbo",
                table: "EmployeeType",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeTypeID",
                schema: "dbo",
                table: "EmployeeType",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "newid()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByEmployeeEmployeeID",
                schema: "dbo",
                table: "EmployeeType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUserID",
                schema: "dbo",
                table: "EmployeeType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentID1",
                schema: "dbo",
                table: "EmployeeType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeType",
                schema: "dbo",
                table: "EmployeeType",
                column: "EmployeeTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeType_CreatedByEmployeeEmployeeID",
                schema: "dbo",
                table: "EmployeeType",
                column: "CreatedByEmployeeEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeType_CreatedUserID",
                schema: "dbo",
                table: "EmployeeType",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeType_DepartmentID",
                schema: "dbo",
                table: "EmployeeType",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeType_DepartmentID1",
                schema: "dbo",
                table: "EmployeeType",
                column: "DepartmentID1");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeType_EditedByEmployeeEmployeeID",
                schema: "dbo",
                table: "EmployeeType",
                column: "EditedByEmployeeEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeType_EditedUserID",
                schema: "dbo",
                table: "EmployeeType",
                column: "EditedUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_EmployeeType_EmployeeTypeID",
                schema: "dbo",
                table: "Employee",
                column: "EmployeeTypeID",
                principalSchema: "dbo",
                principalTable: "EmployeeType",
                principalColumn: "EmployeeTypeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeType_Department_DepartmentID",
                schema: "dbo",
                table: "EmployeeType",
                column: "DepartmentID",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeType_Department_DepartmentID1",
                schema: "dbo",
                table: "EmployeeType",
                column: "DepartmentID1",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeType_Employee_CreatedByEmployeeEmployeeID",
                schema: "dbo",
                table: "EmployeeType",
                column: "CreatedByEmployeeEmployeeID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeType_Employee_CreatedUserID",
                schema: "dbo",
                table: "EmployeeType",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeType_Employee_EditedByEmployeeEmployeeID",
                schema: "dbo",
                table: "EmployeeType",
                column: "EditedByEmployeeEmployeeID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeType_Employee_EditedUserID",
                schema: "dbo",
                table: "EmployeeType",
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
                name: "FK_Employee_EmployeeType_EmployeeTypeID",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeType_Department_DepartmentID",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeType_Department_DepartmentID1",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeType_Employee_CreatedByEmployeeEmployeeID",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeType_Employee_CreatedUserID",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeType_Employee_EditedByEmployeeEmployeeID",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeType_Employee_EditedUserID",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeType",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeType_CreatedByEmployeeEmployeeID",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeType_CreatedUserID",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeType_DepartmentID",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeType_DepartmentID1",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeType_EditedByEmployeeEmployeeID",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeType_EditedUserID",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropColumn(
                name: "CreatedByEmployeeEmployeeID",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropColumn(
                name: "CreatedUserID",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropColumn(
                name: "DepartmentID1",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.RenameTable(
                name: "EmployeeType",
                schema: "dbo",
                newName: "EmployeesTypes",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "EditedUserID",
                schema: "dbo",
                table: "EmployeesTypes",
                newName: "UserEditingID");

            migrationBuilder.RenameColumn(
                name: "EditedDate",
                schema: "dbo",
                table: "EmployeesTypes",
                newName: "RecordAddedDateTimeStamp");

            migrationBuilder.RenameColumn(
                name: "EditedByEmployeeEmployeeID",
                schema: "dbo",
                table: "EmployeesTypes",
                newName: "AddedByID");

            migrationBuilder.RenameColumn(
                name: "EditedBy",
                schema: "dbo",
                table: "EmployeesTypes",
                newName: "UserNameEditingRecord");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                schema: "dbo",
                table: "EmployeesTypes",
                newName: "EditDate");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                schema: "dbo",
                table: "EmployeesTypes",
                newName: "NameOfUserAddingRecord");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeTypeShort",
                schema: "dbo",
                table: "EmployeesTypes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeTypeLong",
                schema: "dbo",
                table: "EmployeesTypes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeTypeID",
                schema: "dbo",
                table: "EmployeesTypes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "newid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeesTypes",
                schema: "dbo",
                table: "EmployeesTypes",
                column: "EmployeeTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_EmployeesTypes_EmployeeTypeID",
                schema: "dbo",
                table: "Employee",
                column: "EmployeeTypeID",
                principalSchema: "dbo",
                principalTable: "EmployeesTypes",
                principalColumn: "EmployeeTypeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
