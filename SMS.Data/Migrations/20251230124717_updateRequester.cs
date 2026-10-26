using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateRequester : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmittedBy",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.RenameColumn(
                name: "RequiresLiquorLicense",
                schema: "dbo",
                table: "Requester",
                newName: "IsLiquorLicenseRequired");

            migrationBuilder.RenameColumn(
                name: "DateStamp",
                schema: "dbo",
                table: "Requester",
                newName: "EditedDate");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "dbo",
                table: "Requester",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "Requester",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUserID",
                schema: "dbo",
                table: "Requester",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditedBy",
                schema: "dbo",
                table: "Requester",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EditedUserID",
                schema: "dbo",
                table: "Requester",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requester_CreatedUserID",
                schema: "dbo",
                table: "Requester",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Requester_EditedUserID",
                schema: "dbo",
                table: "Requester",
                column: "EditedUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Requester_Employee_CreatedUserID",
                schema: "dbo",
                table: "Requester",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requester_Employee_EditedUserID",
                schema: "dbo",
                table: "Requester",
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
                name: "FK_Requester_Employee_CreatedUserID",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropForeignKey(
                name: "FK_Requester_Employee_EditedUserID",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropIndex(
                name: "IX_Requester_CreatedUserID",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropIndex(
                name: "IX_Requester_EditedUserID",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropColumn(
                name: "CreatedUserID",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropColumn(
                name: "EditedBy",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropColumn(
                name: "EditedUserID",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.RenameColumn(
                name: "IsLiquorLicenseRequired",
                schema: "dbo",
                table: "Requester",
                newName: "RequiresLiquorLicense");

            migrationBuilder.RenameColumn(
                name: "EditedDate",
                schema: "dbo",
                table: "Requester",
                newName: "DateStamp");

            migrationBuilder.AddColumn<string>(
                name: "SubmittedBy",
                schema: "dbo",
                table: "Requester",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
