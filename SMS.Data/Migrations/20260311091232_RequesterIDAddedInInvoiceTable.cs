using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequesterIDAddedInInvoiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RequesterID",
                schema: "dbo",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_RequesterID",
                schema: "dbo",
                table: "Invoices",
                column: "RequesterID");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Requester_RequesterID",
                schema: "dbo",
                table: "Invoices",
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
                name: "FK_Invoices_Requester_RequesterID",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_RequesterID",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "RequesterID",
                schema: "dbo",
                table: "Invoices");
        }
    }
}
