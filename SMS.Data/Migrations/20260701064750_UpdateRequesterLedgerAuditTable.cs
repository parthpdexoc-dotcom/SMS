using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequesterLedgerAuditTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequesterLedgers_Department_DepartmentID",
                schema: "dbo",
                table: "RequesterLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_RequesterLedgers_Employee_CreatedUserID",
                schema: "dbo",
                table: "RequesterLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_RequesterLedgers_Employee_DeletedUserID",
                schema: "dbo",
                table: "RequesterLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_RequesterLedgers_Employee_EditedUserID",
                schema: "dbo",
                table: "RequesterLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_RequesterLedgers_Requester_RequesterID",
                schema: "dbo",
                table: "RequesterLedgers");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequesterLedgerID",
                schema: "dbo",
                table: "RequesterLedgers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "newid()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterLedgers_Department_DepartmentID",
                schema: "dbo",
                table: "RequesterLedgers",
                column: "DepartmentID",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterLedgers_Employee_CreatedUserID",
                schema: "dbo",
                table: "RequesterLedgers",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterLedgers_Employee_DeletedUserID",
                schema: "dbo",
                table: "RequesterLedgers",
                column: "DeletedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterLedgers_Employee_EditedUserID",
                schema: "dbo",
                table: "RequesterLedgers",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterLedgers_Requester_RequesterID",
                schema: "dbo",
                table: "RequesterLedgers",
                column: "RequesterID",
                principalSchema: "dbo",
                principalTable: "Requester",
                principalColumn: "RequesterID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequesterLedgers_Department_DepartmentID",
                schema: "dbo",
                table: "RequesterLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_RequesterLedgers_Employee_CreatedUserID",
                schema: "dbo",
                table: "RequesterLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_RequesterLedgers_Employee_DeletedUserID",
                schema: "dbo",
                table: "RequesterLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_RequesterLedgers_Employee_EditedUserID",
                schema: "dbo",
                table: "RequesterLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_RequesterLedgers_Requester_RequesterID",
                schema: "dbo",
                table: "RequesterLedgers");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequesterLedgerID",
                schema: "dbo",
                table: "RequesterLedgers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "newid()");

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterLedgers_Department_DepartmentID",
                schema: "dbo",
                table: "RequesterLedgers",
                column: "DepartmentID",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterLedgers_Employee_CreatedUserID",
                schema: "dbo",
                table: "RequesterLedgers",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterLedgers_Employee_DeletedUserID",
                schema: "dbo",
                table: "RequesterLedgers",
                column: "DeletedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterLedgers_Employee_EditedUserID",
                schema: "dbo",
                table: "RequesterLedgers",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_RequesterLedgers_Requester_RequesterID",
                schema: "dbo",
                table: "RequesterLedgers",
                column: "RequesterID",
                principalSchema: "dbo",
                principalTable: "Requester",
                principalColumn: "RequesterID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
