using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsInClosePostPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClosedPostPosition_Employee_EmployeeID",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.DropForeignKey(
                name: "FK_ClosedPostPositionEquipment_Employee_EmployeeID",
                schema: "dbo",
                table: "ClosedPostPositionEquipment");

            migrationBuilder.DropIndex(
                name: "IX_ClosedPostPositionEquipment_EmployeeID",
                schema: "dbo",
                table: "ClosedPostPositionEquipment");

            migrationBuilder.DropIndex(
                name: "IX_ClosedPostPosition_EmployeeID",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.DropColumn(
                name: "EmployeeID",
                schema: "dbo",
                table: "ClosedPostPositionEquipment");

            migrationBuilder.DropColumn(
                name: "EmployeeID",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.AddColumn<Guid>(
                name: "ClosePostPositionID",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinancialImpactType",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsInvoiced",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_ClosePostPositionID",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "ClosePostPositionID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_ClosedPostPosition_ClosePostPositionID",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "ClosePostPositionID",
                principalSchema: "dbo",
                principalTable: "ClosedPostPosition",
                principalColumn: "ClosedPostPositionID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_ClosedPostPosition_ClosePostPositionID",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetails_ClosePostPositionID",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "ClosePostPositionID",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "FinancialImpactType",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.DropColumn(
                name: "IsInvoiced",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeID",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeID",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClosedPostPositionEquipment_EmployeeID",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_ClosedPostPosition_EmployeeID",
                schema: "dbo",
                table: "ClosedPostPosition",
                column: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_ClosedPostPosition_Employee_EmployeeID",
                schema: "dbo",
                table: "ClosedPostPosition",
                column: "EmployeeID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClosedPostPositionEquipment_Employee_EmployeeID",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                column: "EmployeeID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
