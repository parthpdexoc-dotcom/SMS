using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsParentFromVehicleApprovalRouting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the VehicleApproverIsParent column
            migrationBuilder.DropColumn(
                name: "VehicleApproverIsParent",
                schema: "dbo",
                table: "VehicleApprovalRouting");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add back VehicleApproverIsParent column with default value
            migrationBuilder.AddColumn<bool>(
                name: "VehicleApproverIsParent",
                schema: "dbo",
                table: "VehicleApprovalRouting",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
