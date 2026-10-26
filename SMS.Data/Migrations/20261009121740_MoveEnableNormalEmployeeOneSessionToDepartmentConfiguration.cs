using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveEnableNormalEmployeeOneSessionToDepartmentConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnableNormalEmployeeOneSession",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.AddColumn<bool>(
                name: "EnableNormalEmployeeOneSession",
                schema: "dbo",
                table: "DepartmentConfiguration",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnableNormalEmployeeOneSession",
                schema: "dbo",
                table: "DepartmentConfiguration");

            migrationBuilder.AddColumn<bool>(
                name: "EnableNormalEmployeeOneSession",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
