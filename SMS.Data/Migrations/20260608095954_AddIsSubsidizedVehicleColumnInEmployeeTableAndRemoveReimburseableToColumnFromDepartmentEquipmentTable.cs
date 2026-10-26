using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSubsidizedVehicleColumnInEmployeeTableAndRemoveReimburseableToColumnFromDepartmentEquipmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReimburseTo",
                schema: "dbo",
                table: "DepartmentEquipment");

            migrationBuilder.AddColumn<bool>(
                name: "IsSubsidizedVehicle",
                schema: "dbo",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSubsidizedVehicle",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.AddColumn<int>(
                name: "ReimburseTo",
                schema: "dbo",
                table: "DepartmentEquipment",
                type: "int",
                nullable: true);
        }
    }
}
