using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleQuantityAndVehicleUsedHoursToClosedPostPositionEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VehicleQuantity",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VehicleUsedHours",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleQuantity",
                schema: "dbo",
                table: "ClosedPostPositionEquipment");

            migrationBuilder.DropColumn(
                name: "VehicleUsedHours",
                schema: "dbo",
                table: "ClosedPostPositionEquipment");
        }
    }
}
