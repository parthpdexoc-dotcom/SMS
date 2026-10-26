using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrganizationNameToOrganizationNumberInPostTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganizationName",
                schema: "dbo",
                table: "Post");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationNumber",
                schema: "dbo",
                table: "Post",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganizationNumber",
                schema: "dbo",
                table: "Post");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationName",
                schema: "dbo",
                table: "Post",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
