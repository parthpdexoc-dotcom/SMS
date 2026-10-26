using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class addenablegeneralpolicyandtenurepolicyindepartmentconfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnableGeneralPolicy",
                schema: "dbo",
                table: "DepartmentConfiguration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableTenurePolicy",
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
                name: "EnableGeneralPolicy",
                schema: "dbo",
                table: "DepartmentConfiguration");

            migrationBuilder.DropColumn(
                name: "EnableTenurePolicy",
                schema: "dbo",
                table: "DepartmentConfiguration");
        }
    }
}
