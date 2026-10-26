using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBaseEntityFromQuestionnaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaire_Employee_CreatedUserID",
                schema: "dbo",
                table: "Questionnaire");

            migrationBuilder.DropIndex(
                name: "IX_Questionnaire_CreatedUserID",
                schema: "dbo",
                table: "Questionnaire");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "dbo",
                table: "Questionnaire");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "dbo",
                table: "Questionnaire");

            migrationBuilder.DropColumn(
                name: "CreatedUserID",
                schema: "dbo",
                table: "Questionnaire");

            migrationBuilder.DropColumn(
                name: "EditedBy",
                schema: "dbo",
                table: "Questionnaire");

            migrationBuilder.DropColumn(
                name: "EditedDate",
                schema: "dbo",
                table: "Questionnaire");

            migrationBuilder.DropColumn(
                name: "EditedUserID",
                schema: "dbo",
                table: "Questionnaire");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "dbo",
                table: "Questionnaire",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "Questionnaire",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUserID",
                schema: "dbo",
                table: "Questionnaire",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditedBy",
                schema: "dbo",
                table: "Questionnaire",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedDate",
                schema: "dbo",
                table: "Questionnaire",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EditedUserID",
                schema: "dbo",
                table: "Questionnaire",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaire_CreatedUserID",
                schema: "dbo",
                table: "Questionnaire",
                column: "CreatedUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaire_Employee_CreatedUserID",
                schema: "dbo",
                table: "Questionnaire",
                column: "CreatedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
