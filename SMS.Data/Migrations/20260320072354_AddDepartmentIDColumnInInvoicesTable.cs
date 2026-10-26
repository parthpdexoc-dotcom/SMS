using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentIDColumnInInvoicesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentID",
                schema: "dbo",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_DepartmentID",
                schema: "dbo",
                table: "Invoices",
                column: "DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Department_DepartmentID",
                schema: "dbo",
                table: "Invoices",
                column: "DepartmentID",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Department_DepartmentID",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_DepartmentID",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                schema: "dbo",
                table: "Invoices");
        }
    }
}
