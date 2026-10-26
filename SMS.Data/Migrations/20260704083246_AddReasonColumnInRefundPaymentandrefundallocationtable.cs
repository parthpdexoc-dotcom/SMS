using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReasonColumnInRefundPaymentandrefundallocationtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                schema: "dbo",
                table: "RefundPayments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                schema: "dbo",
                table: "RefundPaymentAllocations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                schema: "dbo",
                table: "Payments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                schema: "dbo",
                table: "RefundPayments");

            migrationBuilder.DropColumn(
                name: "Reason",
                schema: "dbo",
                table: "RefundPaymentAllocations");

            migrationBuilder.DropColumn(
                name: "Reason",
                schema: "dbo",
                table: "Payments");
        }
    }
}
