using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateInvoiceDetailTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvoiceDetails",
                schema: "dbo",
                columns: table => new
                {
                    InvoiceDetailID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoicesID = table.Column<long>(type: "bigint", nullable: false),
                    JobId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    JobName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    JobDate = table.Column<DateOnly>(type: "date", nullable: true),
                    OfficerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Hours = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FeesAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceDetails", x => x.InvoiceDetailID);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_Invoices_InvoicesID",
                        column: x => x.InvoicesID,
                        principalSchema: "dbo",
                        principalTable: "Invoices",
                        principalColumn: "InvoicesID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_InvoicesID",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "InvoicesID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceDetails",
                schema: "dbo");
        }
    }
}
