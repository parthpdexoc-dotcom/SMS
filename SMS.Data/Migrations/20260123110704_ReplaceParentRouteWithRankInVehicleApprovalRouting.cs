using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceParentRouteWithRankInVehicleApprovalRouting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint for VehicleApproverParentRouteId
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleApprovalRouting_VehicleApprovalRouting_ParentRoute",
                schema: "dbo",
                table: "VehicleApprovalRouting");

            // Drop the index for VehicleApproverParentRouteId
            migrationBuilder.DropIndex(
                name: "IX_VehicleApprovalRouting_VehicleApproverParentRouteId",
                schema: "dbo",
                table: "VehicleApprovalRouting");

            // Drop the VehicleApproverParentRouteId column
            migrationBuilder.DropColumn(
                name: "VehicleApproverParentRouteId",
                schema: "dbo",
                table: "VehicleApprovalRouting");

            // Add Rank column (int, not null, default 1)
            migrationBuilder.AddColumn<int>(
                name: "Rank",
                schema: "dbo",
                table: "VehicleApprovalRouting",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // Create index on Rank and DepartmentEquipmentID for efficient queries
            migrationBuilder.CreateIndex(
                name: "IX_VehicleApprovalRouting_DepartmentEquipmentID_Rank",
                schema: "dbo",
                table: "VehicleApprovalRouting",
                columns: new[] { "DepartmentEquipmentID", "Rank" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the Rank index
            migrationBuilder.DropIndex(
                name: "IX_VehicleApprovalRouting_DepartmentEquipmentID_Rank",
                schema: "dbo",
                table: "VehicleApprovalRouting");

            // Drop Rank column
            migrationBuilder.DropColumn(
                name: "Rank",
                schema: "dbo",
                table: "VehicleApprovalRouting");

            // Add back VehicleApproverParentRouteId column
            migrationBuilder.AddColumn<System.Guid>(
                name: "VehicleApproverParentRouteId",
                schema: "dbo",
                table: "VehicleApprovalRouting",
                type: "uniqueidentifier",
                nullable: true);

            // Recreate the index
            migrationBuilder.CreateIndex(
                name: "IX_VehicleApprovalRouting_VehicleApproverParentRouteId",
                schema: "dbo",
                table: "VehicleApprovalRouting",
                column: "VehicleApproverParentRouteId");

            // Recreate the foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_VehicleApprovalRouting_VehicleApprovalRouting_ParentRoute",
                schema: "dbo",
                table: "VehicleApprovalRouting",
                column: "VehicleApproverParentRouteId",
                principalSchema: "dbo",
                principalTable: "VehicleApprovalRouting",
                principalColumn: "VehicleApprovalRouteID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
