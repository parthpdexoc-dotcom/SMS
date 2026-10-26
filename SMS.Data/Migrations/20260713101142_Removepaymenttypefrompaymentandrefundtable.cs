using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class Removepaymenttypefrompaymentandrefundtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentType",
                schema: "dbo",
                table: "RefundPayments");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                schema: "dbo",
                table: "Payments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                schema: "dbo",
                table: "RefundPayments",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                schema: "dbo",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }
    }
}
