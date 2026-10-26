using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedRequestIdInPostEquipmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
