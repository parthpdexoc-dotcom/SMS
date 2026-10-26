using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddContactColumnsInPostTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CellPhone",
                schema: "dbo",
                table: "Post",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "dbo",
                table: "Post",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "dbo",
                table: "Post",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryEmail",
                schema: "dbo",
                table: "Post",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryPhone",
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
                name: "CellPhone",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PrimaryEmail",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PrimaryPhone",
                schema: "dbo",
                table: "Post");
        }
    }
}
