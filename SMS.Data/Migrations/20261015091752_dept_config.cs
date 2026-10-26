using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class dept_config : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                schema: "dbo",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "Longitude",
                schema: "dbo",
                table: "Department");

            migrationBuilder.AddColumn<decimal>(
                name: "DepartmentHomeLatitude",
                schema: "dbo",
                table: "DepartmentConfiguration",
                type: "decimal(18,8)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentHomeLocation",
                schema: "dbo",
                table: "DepartmentConfiguration",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DepartmentHomeLongitude",
                schema: "dbo",
                table: "DepartmentConfiguration",
                type: "decimal(18,8)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentHomeLatitude",
                schema: "dbo",
                table: "DepartmentConfiguration");

            migrationBuilder.DropColumn(
                name: "DepartmentHomeLocation",
                schema: "dbo",
                table: "DepartmentConfiguration");

            migrationBuilder.DropColumn(
                name: "DepartmentHomeLongitude",
                schema: "dbo",
                table: "DepartmentConfiguration");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                schema: "dbo",
                table: "Department",
                type: "decimal(9,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                schema: "dbo",
                table: "Department",
                type: "decimal(9,6)",
                nullable: true);
        }
    }
}
