using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveFinancialImpactTypeToInvoiceDetails_RemoveInvoicedFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinancialImpactType",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.DropColumn(
                name: "IsInvoiced",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.AddColumn<int>(
                name: "FinancialImpactType",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinancialImpactType",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.AddColumn<int>(
                name: "FinancialImpactType",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsInvoiced",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
