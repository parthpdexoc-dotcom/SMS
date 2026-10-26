using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFiscalYearAndProjectNumberToRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FiscalYear",
                schema: "dbo",
                table: "Request",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectNumber",
                schema: "dbo",
                table: "Request",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FiscalYear",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "ProjectNumber",
                schema: "dbo",
                table: "Request");
        }
    }
}
