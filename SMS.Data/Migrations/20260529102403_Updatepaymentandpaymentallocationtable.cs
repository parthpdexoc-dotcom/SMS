using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class Updatepaymentandpaymentallocationtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVoided",
                schema: "dbo",
                table: "Payments");

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                schema: "dbo",
                table: "Payments",
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

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
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
                name: "PaymentStatus",
                schema: "dbo",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                schema: "dbo",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                schema: "dbo",
                table: "PaymentAllocations");

            migrationBuilder.AddColumn<bool>(
                name: "IsVoided",
                schema: "dbo",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
