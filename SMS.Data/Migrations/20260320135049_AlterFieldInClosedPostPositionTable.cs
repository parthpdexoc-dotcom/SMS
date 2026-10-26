using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterFieldInClosedPostPositionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Invalid",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.AddColumn<bool>(
                name: "Invalid",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "bit",
                nullable: true);
        }
    }
}
