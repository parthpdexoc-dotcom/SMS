using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequestApprovalRouting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
         name: "FK_RequestApprovalRouting_DepartmentEquipment_DepartmentEquipmentID",
         schema: "dbo",
         table: "RequestApprovalRouting");

            migrationBuilder.DropIndex(
                name: "IX_RequestApprovalRouting_DepartmentEquipmentID_Rank",
                schema: "dbo",
                table: "RequestApprovalRouting");

            migrationBuilder.DropColumn(
                name: "DepartmentEquipmentID",
                schema: "dbo",
                table: "RequestApprovalRouting");

            // 2. Add RequestID
            migrationBuilder.AddColumn<Guid>(
                name: "RequestID",
                schema: "dbo",
                table: "RequestApprovalRouting",
                type: "uniqueidentifier",
                nullable: false);

            // 3. Create FK to Request table
            migrationBuilder.AddForeignKey(
                name: "FK_RequestApprovalRouting_Request_RequestID",
                schema: "dbo",
                table: "RequestApprovalRouting",
                column: "RequestID",
                principalSchema: "dbo",
                principalTable: "Request",
                principalColumn: "RequestID",
                onDelete: ReferentialAction.Restrict);

            // 4. Unique rank per request
            migrationBuilder.CreateIndex(
                name: "IX_RequestApprovalRouting_RequestID_Rank",
                schema: "dbo",
                table: "RequestApprovalRouting",
                columns: new[] { "RequestID", "Rank" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Drop FK & index for RequestID
            migrationBuilder.DropForeignKey(
                name: "FK_RequestApprovalRouting_Request_RequestID",
                schema: "dbo",
                table: "RequestApprovalRouting");

            migrationBuilder.DropIndex(
                name: "IX_RequestApprovalRouting_RequestID_Rank",
                schema: "dbo",
                table: "RequestApprovalRouting");

            // 2. Drop RequestID column
            migrationBuilder.DropColumn(
                name: "RequestID",
                schema: "dbo",
                table: "RequestApprovalRouting");

            // 3. Re-add DepartmentEquipmentID
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentEquipmentID",
                schema: "dbo",
                table: "RequestApprovalRouting",
                type: "uniqueidentifier",
                nullable: false);

            // 4. Re-create index
            migrationBuilder.CreateIndex(
                name: "IX_RequestApprovalRouting_DepartmentEquipmentID_Rank",
                schema: "dbo",
                table: "RequestApprovalRouting",
                columns: new[] { "DepartmentEquipmentID", "Rank" },
                unique: true);

            // 5. Re-create FK
            migrationBuilder.AddForeignKey(
                name: "FK_RequestApprovalRouting_DepartmentEquipment_DepartmentEquipmentID",
                schema: "dbo",
                table: "RequestApprovalRouting",
                column: "DepartmentEquipmentID",
                principalSchema: "dbo",
                principalTable: "DepartmentEquipment",
                principalColumn: "DepartmentEquipmentID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
