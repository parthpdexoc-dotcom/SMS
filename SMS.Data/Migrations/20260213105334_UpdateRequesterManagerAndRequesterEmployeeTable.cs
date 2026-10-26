using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequesterManagerAndRequesterEmployeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsShared",
                schema: "dbo",
                table: "RequesterManager",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPreferred",
                schema: "dbo",
                table: "Requester_Employees",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTenure",
                schema: "dbo",
                table: "Requester_Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShared",
                schema: "dbo",
                table: "RequesterManager");

            migrationBuilder.DropColumn(
                name: "IsTenure",
                schema: "dbo",
                table: "Requester_Employees");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPreferred",
                schema: "dbo",
                table: "Requester_Employees",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);
        }
    }
}
