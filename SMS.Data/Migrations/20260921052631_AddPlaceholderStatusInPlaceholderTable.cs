using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaceholderStatusInPlaceholderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PlaceholderStatus",
                schema: "dbo",
                table: "Placeholder",
                type: "bit",
                nullable: false,
                defaultValue: true);
                
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlaceholderStatus",
                schema: "dbo",
                table: "Placeholder");
        }
    }
}
