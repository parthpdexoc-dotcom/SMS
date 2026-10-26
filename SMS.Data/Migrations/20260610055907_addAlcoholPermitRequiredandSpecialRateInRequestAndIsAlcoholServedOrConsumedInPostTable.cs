using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class addAlcoholPermitRequiredandSpecialRateInRequestAndIsAlcoholServedOrConsumedInPostTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AlcoholPermitRequired",
                schema: "dbo",
                table: "Request",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "SpecialRate",
                schema: "dbo",
                table: "Request",
                type: "money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsAlcoholServedOrConsumed",
                schema: "dbo",
                table: "Post",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlcoholPermitRequired",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "SpecialRate",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "IsAlcoholServedOrConsumed",
                schema: "dbo",
                table: "Post");
        }
    }
}
