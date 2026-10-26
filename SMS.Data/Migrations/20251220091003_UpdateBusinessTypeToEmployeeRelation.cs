using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBusinessTypeToEmployeeRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessType_EmployeeAdmin_CreatedUserID",
                schema: "dbo",
                table: "BusinessType");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessType_EmployeeAdmin_EditedUserID",
                schema: "dbo",
                table: "BusinessType");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessType_Employee_CreatedUserID",
                schema: "dbo",
                table: "BusinessType",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessType_Employee_EditedUserID",
                schema: "dbo",
                table: "BusinessType",
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
                name: "FK_BusinessType_Employee_CreatedUserID",
                schema: "dbo",
                table: "BusinessType");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessType_Employee_EditedUserID",
                schema: "dbo",
                table: "BusinessType");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessType_EmployeeAdmin_CreatedUserID",
                schema: "dbo",
                table: "BusinessType",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "EmployeeAdmin",
                principalColumn: "EmployeeAdminID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessType_EmployeeAdmin_EditedUserID",
                schema: "dbo",
                table: "BusinessType",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "EmployeeAdmin",
                principalColumn: "EmployeeAdminID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
