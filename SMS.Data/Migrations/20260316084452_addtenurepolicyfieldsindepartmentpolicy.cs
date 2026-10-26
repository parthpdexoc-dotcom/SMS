using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class addtenurepolicyfieldsindepartmentpolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenureRulesJson",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenureType",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenureRulesJson",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "TenureType",
                schema: "dbo",
                table: "DepartmentPolicy");
        }
    }
}
