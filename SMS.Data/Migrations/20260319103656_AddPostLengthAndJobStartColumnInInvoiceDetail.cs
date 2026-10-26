using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPostLengthAndJobStartColumnInInvoiceDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostLength",
                schema: "dbo",
                table: "Invoices",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "JobTime",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PostLength",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostLength",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "JobTime",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "PostLength",
                schema: "dbo",
                table: "InvoiceDetails");
        }
    }
}
