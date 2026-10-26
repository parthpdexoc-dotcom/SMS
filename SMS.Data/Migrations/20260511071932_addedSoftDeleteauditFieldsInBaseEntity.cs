using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedSoftDeleteauditFieldsInBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "dbo",
                table: "RequestType",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "RequestType",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUserID",
                schema: "dbo",
                table: "RequestType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditedBy",
                schema: "dbo",
                table: "RequestType",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedDate",
                schema: "dbo",
                table: "RequestType",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EditedUserID",
                schema: "dbo",
                table: "RequestType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "dbo",
                table: "Invoices",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "Invoices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUserID",
                schema: "dbo",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditedBy",
                schema: "dbo",
                table: "Invoices",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedDate",
                schema: "dbo",
                table: "Invoices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EditedUserID",
                schema: "dbo",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUserID",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditedBy",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedDate",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EditedUserID",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestType_CreatedUserID",
                schema: "dbo",
                table: "RequestType",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestType_DeletedUserID",
                schema: "dbo",
                table: "RequestType",
                column: "DeletedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestType_EditedUserID",
                schema: "dbo",
                table: "RequestType",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CreatedUserID",
                schema: "dbo",
                table: "Invoices",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_DeletedUserID",
                schema: "dbo",
                table: "Invoices",
                column: "DeletedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_EditedUserID",
                schema: "dbo",
                table: "Invoices",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_CreatedUserID",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_DeletedUserID",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "DeletedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_EditedUserID",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "EditedUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_Employee_CreatedUserID",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_Employee_DeletedUserID",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "DeletedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_Employee_EditedUserID",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Employee_CreatedUserID",
                schema: "dbo",
                table: "Invoices",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Employee_DeletedUserID",
                schema: "dbo",
                table: "Invoices",
                column: "DeletedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Employee_EditedUserID",
                schema: "dbo",
                table: "Invoices",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestType_Employee_CreatedUserID",
                schema: "dbo",
                table: "RequestType",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestType_Employee_DeletedUserID",
                schema: "dbo",
                table: "RequestType",
                column: "DeletedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestType_Employee_EditedUserID",
                schema: "dbo",
                table: "RequestType",
                column: "EditedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Employee_CreatedUserID",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Employee_DeletedUserID",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Employee_EditedUserID",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Employee_CreatedUserID",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Employee_DeletedUserID",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Employee_EditedUserID",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestType_Employee_CreatedUserID",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestType_Employee_DeletedUserID",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestType_Employee_EditedUserID",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropIndex(
                name: "IX_RequestType_CreatedUserID",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropIndex(
                name: "IX_RequestType_DeletedUserID",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropIndex(
                name: "IX_RequestType_EditedUserID",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_CreatedUserID",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_DeletedUserID",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_EditedUserID",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetails_CreatedUserID",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetails_DeletedUserID",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetails_EditedUserID",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropColumn(
                name: "CreatedUserID",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropColumn(
                name: "EditedBy",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropColumn(
                name: "EditedDate",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropColumn(
                name: "EditedUserID",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CreatedUserID",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "EditedBy",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "EditedDate",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "EditedUserID",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "CreatedUserID",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "EditedBy",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "EditedDate",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "EditedUserID",
                schema: "dbo",
                table: "InvoiceDetails");
        }
    }
}
