using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVehicleDataForPostAndRequestEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

        -- Sync PostEquipment.IsVehicle from EquipmentType
        UPDATE PE
        SET PE.IsVehicle = ET.IsVehicle
        FROM PostEquipment PE
        INNER JOIN DepartmentEquipment DE
            ON PE.DepartmentEquipmentID = DE.DepartmentEquipmentID
        INNER JOIN EquipmentType ET
            ON DE.EquipmentTypeID = ET.EquipementTypeID;



        -- Sync RequestEquipment.IsVehicle from EquipmentType
        UPDATE RE
        SET RE.IsVehicle = ET.IsVehicle
        FROM RequestEquipment RE
        INNER JOIN DepartmentEquipment DE
            ON RE.DepartmentEquipmentID = DE.DepartmentEquipmentID
        INNER JOIN EquipmentType ET
            ON DE.EquipmentTypeID = ET.EquipementTypeID;

    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No rollback required for data sync
        }
    }
}
