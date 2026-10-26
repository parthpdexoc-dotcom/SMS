using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateResponseWithBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Response_Employee_SubmittedBy",
                schema: "dbo",
                table: "Response");

            migrationBuilder.DropIndex(
                name: "IX_Response_SubmittedBy",
                schema: "dbo",
                table: "Response");

            migrationBuilder.DropColumn(
                name: "SubmittedBy",
                schema: "dbo",
                table: "Response");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "dbo",
                table: "Response",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "Response",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUserID",
                schema: "dbo",
                table: "Response",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditedBy",
                schema: "dbo",
                table: "Response",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedDate",
                schema: "dbo",
                table: "Response",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EditedUserID",
                schema: "dbo",
                table: "Response",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Response_CreatedUserID",
                schema: "dbo",
                table: "Response",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Response_EditedUserID",
                schema: "dbo",
                table: "Response",
                column: "EditedUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Response_Employee_CreatedUserID",
                schema: "dbo",
                table: "Response",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Response_Employee_EditedUserID",
                schema: "dbo",
                table: "Response",
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
                name: "FK_Response_Employee_CreatedUserID",
                schema: "dbo",
                table: "Response");

            migrationBuilder.DropForeignKey(
                name: "FK_Response_Employee_EditedUserID",
                schema: "dbo",
                table: "Response");

            migrationBuilder.DropIndex(
                name: "IX_Response_CreatedUserID",
                schema: "dbo",
                table: "Response");

            migrationBuilder.DropIndex(
                name: "IX_Response_EditedUserID",
                schema: "dbo",
                table: "Response");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "dbo",
                table: "Response");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "dbo",
                table: "Response");

            migrationBuilder.DropColumn(
                name: "CreatedUserID",
                schema: "dbo",
                table: "Response");

            migrationBuilder.DropColumn(
                name: "EditedBy",
                schema: "dbo",
                table: "Response");

            migrationBuilder.DropColumn(
                name: "EditedDate",
                schema: "dbo",
                table: "Response");

            migrationBuilder.DropColumn(
                name: "EditedUserID",
                schema: "dbo",
                table: "Response");

            migrationBuilder.AddColumn<Guid>(
                name: "SubmittedBy",
                schema: "dbo",
                table: "Response",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Response_SubmittedBy",
                schema: "dbo",
                table: "Response",
                column: "SubmittedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Response_Employee_SubmittedBy",
                schema: "dbo",
                table: "Response",
                column: "SubmittedBy",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
