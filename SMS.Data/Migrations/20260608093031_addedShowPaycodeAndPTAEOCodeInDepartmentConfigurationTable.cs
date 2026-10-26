using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedShowPaycodeAndPTAEOCodeInDepartmentConfigurationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowPayCode",
                schema: "dbo",
                table: "DepartmentConfiguration",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowPtaeoCode",
                schema: "dbo",
                table: "DepartmentConfiguration",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowPayCode",
                schema: "dbo",
                table: "DepartmentConfiguration");

            migrationBuilder.DropColumn(
                name: "ShowPtaeoCode",
                schema: "dbo",
                table: "DepartmentConfiguration");
        }
    }
}
