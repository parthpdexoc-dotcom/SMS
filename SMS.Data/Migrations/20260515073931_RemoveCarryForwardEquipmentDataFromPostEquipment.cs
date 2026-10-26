using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCarryForwardEquipmentDataFromPostEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM dbo.PostEquipment
                WHERE RequestID IS NOT NULL");

            migrationBuilder.DropForeignKey(
                name: "FK_PostEquipment_Request_RequestID",
                schema: "dbo",
                table: "PostEquipment");

            migrationBuilder.DropIndex(
                name: "IX_PostEquipment_RequestID",
                schema: "dbo",
                table: "PostEquipment");

            migrationBuilder.DropColumn(
                name: "RequestID",
                schema: "dbo",
                table: "PostEquipment");

            migrationBuilder.RenameColumn(
                name: "Equipment_Quantity",
                schema: "dbo",
                table: "RequestEquipment",
                newName: "EquipmentQuantity");

            migrationBuilder.RenameColumn(
                name: "Equipment_Per_Request",
                schema: "dbo",
                table: "RequestEquipment",
                newName: "EquipmentPerRequest");

            migrationBuilder.RenameColumn(
                name: "Equipment_Quantity",
                schema: "dbo",
                table: "PostEquipment",
                newName: "EquipmentQuantity");

            migrationBuilder.RenameColumn(
                name: "Equipment_Per_Post",
                schema: "dbo",
                table: "PostEquipment",
                newName: "IsVehicle");

            migrationBuilder.AddColumn<bool>(
                name: "EquipmentPerPostPosition",
                schema: "dbo",
                table: "PostEquipment",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EquipmentPerPostPosition",
                schema: "dbo",
                table: "PostEquipment");

            migrationBuilder.RenameColumn(
                name: "EquipmentQuantity",
                schema: "dbo",
                table: "RequestEquipment",
                newName: "Equipment_Quantity");

            migrationBuilder.RenameColumn(
                name: "EquipmentPerRequest",
                schema: "dbo",
                table: "RequestEquipment",
                newName: "Equipment_Per_Request");

            migrationBuilder.RenameColumn(
                name: "IsVehicle",
                schema: "dbo",
                table: "PostEquipment",
                newName: "Equipment_Per_Post");

            migrationBuilder.RenameColumn(
                name: "EquipmentQuantity",
                schema: "dbo",
                table: "PostEquipment",
                newName: "Equipment_Quantity");

            migrationBuilder.AddColumn<Guid>(
                name: "RequestID",
                schema: "dbo",
                table: "PostEquipment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostEquipment_RequestID",
                schema: "dbo",
                table: "PostEquipment",
                column: "RequestID");

            migrationBuilder.AddForeignKey(
                name: "FK_PostEquipment_Request_RequestID",
                schema: "dbo",
                table: "PostEquipment",
                column: "RequestID",
                principalSchema: "dbo",
                principalTable: "Request",
                principalColumn: "RequestID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
