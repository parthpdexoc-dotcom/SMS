using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequestDepartmentEquipmentPostColumnChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiresVehicleAtEveryPost",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "NumberOfVehicles",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "RequiresEquipmentAtEachPost",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "NumberOfEquipment",
                table: "Request");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiresVehicleAtEveryPost",
                table: "Request",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfVehicles",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 5);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresEquipmentAtEachPost",
                table: "Request",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfEquipment",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 5);
        }
    }
}
