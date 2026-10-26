using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequesterAuthorizedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PPIsCell",
                schema: "dbo",
                table: "RequesterAuthorized");

            migrationBuilder.DropColumn(
                name: "SPIsCellPhone",
                schema: "dbo",
                table: "RequesterAuthorized");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PPIsCell",
                schema: "dbo",
                table: "RequesterAuthorized",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SPIsCellPhone",
                schema: "dbo",
                table: "RequesterAuthorized",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
