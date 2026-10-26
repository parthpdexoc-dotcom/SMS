using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequesterManagerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsShared",
                schema: "dbo",
                table: "RequesterManager",
                newName: "IsRequestShared");

            migrationBuilder.AddColumn<bool>(
                name: "IsEmployeeShared",
                schema: "dbo",
                table: "RequesterManager",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEmployeeShared",
                schema: "dbo",
                table: "RequesterManager");

            migrationBuilder.RenameColumn(
                name: "IsRequestShared",
                schema: "dbo",
                table: "RequesterManager",
                newName: "IsShared");
        }
    }
}
