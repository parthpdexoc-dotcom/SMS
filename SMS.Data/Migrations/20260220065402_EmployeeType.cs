using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "HolidayPayRate",
                schema: "dbo",
                table: "EmployeeType",
                type: "money",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PayRate",
                schema: "dbo",
                table: "EmployeeType",
                type: "money",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PremiumPayRate",
                schema: "dbo",
                table: "EmployeeType",
                type: "money",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HolidayPayRate",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropColumn(
                name: "PayRate",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropColumn(
                name: "PremiumPayRate",
                schema: "dbo",
                table: "EmployeeType");
        }
    }
}
