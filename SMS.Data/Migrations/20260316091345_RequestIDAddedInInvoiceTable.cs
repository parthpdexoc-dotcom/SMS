using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequestIDAddedInInvoiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RequestId",
                schema: "dbo",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_RequestId",
                schema: "dbo",
                table: "Invoices",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Request_RequestId",
                schema: "dbo",
                table: "Invoices",
                column: "RequestId",
                principalSchema: "dbo",
                principalTable: "Request",
                principalColumn: "RequestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Request_RequestId",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_RequestId",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "RequestId",
                schema: "dbo",
                table: "Invoices");
        }
    }
}
