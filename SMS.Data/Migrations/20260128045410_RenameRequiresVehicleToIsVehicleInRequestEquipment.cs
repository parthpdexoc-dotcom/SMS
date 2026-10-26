using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameRequiresVehicleToIsVehicleInRequestEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequiresVehicle",
                schema: "dbo",
                table: "RequestEquipment",
                newName: "IsVehicle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVehicle",
                schema: "dbo",
                table: "RequestEquipment",
                newName: "RequiresVehicle");
        }
    }
}
