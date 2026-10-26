using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentTypeinPaymentandPaymentAllocationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransactionType",
                schema: "dbo",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "TransactionType",
                schema: "dbo",
                table: "PaymentAllocations",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionType",
                schema: "dbo",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                schema: "dbo",
                table: "PaymentAllocations");
        }
    }
}
