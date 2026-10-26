using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedDEpartmentQuestionnaireIdInRequester : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentQuestionnaireId",
                schema: "dbo",
                table: "Requester",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requester_DepartmentQuestionnaireId",
                schema: "dbo",
                table: "Requester",
                column: "DepartmentQuestionnaireId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requester_DepartmentQuestionnaire_DepartmentQuestionnaireId",
                schema: "dbo",
                table: "Requester",
                column: "DepartmentQuestionnaireId",
                principalSchema: "dbo",
                principalTable: "DepartmentQuestionnaire",
                principalColumn: "DepartmentQuestionnaireId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requester_DepartmentQuestionnaire_DepartmentQuestionnaireId",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropIndex(
                name: "IX_Requester_DepartmentQuestionnaireId",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropColumn(
                name: "DepartmentQuestionnaireId",
                schema: "dbo",
                table: "Requester");
        }
    }
}
