using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequestForMonthlyCustomType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomDates",
                schema: "dbo",
                table: "Request",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DaysOfMonth",
                schema: "dbo",
                table: "Request",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomDates",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "DaysOfMonth",
                schema: "dbo",
                table: "Request");
        }
    }
}
