using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameVehicleApprovalDetailToApprovalDetailsAndAddType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "VehicleApprovalDetail",
                schema: "dbo",
                newName: "ApprovalDetails",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "VehicleApprovalDetailID",
                schema: "dbo",
                table: "ApprovalDetails",
                newName: "ApprovalDetailID");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "dbo",
                table: "ApprovalDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                schema: "dbo",
                table: "ApprovalDetails");

            migrationBuilder.RenameColumn(
                name: "ApprovalDetailID",
                schema: "dbo",
                table: "ApprovalDetails",
                newName: "VehicleApprovalDetailID");

            migrationBuilder.RenameTable(
                name: "ApprovalDetails",
                schema: "dbo",
                newName: "VehicleApprovalDetail",
                newSchema: "dbo");
        }
    }
}
