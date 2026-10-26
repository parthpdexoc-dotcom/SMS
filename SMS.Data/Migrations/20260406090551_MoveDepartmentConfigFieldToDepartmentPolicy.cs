using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveDepartmentConfigFieldToDepartmentPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeToClosePost",
                schema: "dbo",
                table: "DepartmentConfiguration");

            migrationBuilder.AddColumn<int>(
                name: "TimeToClosePost",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeToClosePost",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.AddColumn<int>(
                name: "TimeToClosePost",
                schema: "dbo",
                table: "DepartmentConfiguration",
                type: "int",
                nullable: true);
        }
    }
}
