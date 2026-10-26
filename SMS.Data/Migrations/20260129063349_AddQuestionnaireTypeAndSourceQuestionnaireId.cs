using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionnaireTypeAndSourceQuestionnaireId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaire_Department_DepartmentID",
                schema: "dbo",
                table: "Questionnaire");

            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaire_Questionnaire_ParentID",
                schema: "dbo",
                table: "Questionnaire");

            migrationBuilder.DropIndex(
                name: "IX_Questionnaire_DepartmentID",
                schema: "dbo",
                table: "Questionnaire");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                schema: "dbo",
                table: "Questionnaire");

            migrationBuilder.RenameColumn(
                name: "ParentID",
                schema: "dbo",
                table: "Questionnaire",
                newName: "SourceQuestionnaireId");

            migrationBuilder.RenameIndex(
                name: "IX_Questionnaire_ParentID",
                schema: "dbo",
                table: "Questionnaire",
                newName: "IX_Questionnaire_SourceQuestionnaireId");

            migrationBuilder.AddColumn<int>(
                name: "QuestionnaireType",
                schema: "dbo",
                table: "Questionnaire",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaire_Questionnaire_SourceQuestionnaireId",
                schema: "dbo",
                table: "Questionnaire",
                column: "SourceQuestionnaireId",
                principalSchema: "dbo",
                principalTable: "Questionnaire",
                principalColumn: "QuestionnaireId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaire_Questionnaire_SourceQuestionnaireId",
                schema: "dbo",
                table: "Questionnaire");

            migrationBuilder.DropColumn(
                name: "QuestionnaireType",
                schema: "dbo",
                table: "Questionnaire");

            migrationBuilder.RenameColumn(
                name: "SourceQuestionnaireId",
                schema: "dbo",
                table: "Questionnaire",
                newName: "ParentID");

            migrationBuilder.RenameIndex(
                name: "IX_Questionnaire_SourceQuestionnaireId",
                schema: "dbo",
                table: "Questionnaire",
                newName: "IX_Questionnaire_ParentID");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentID",
                schema: "dbo",
                table: "Questionnaire",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaire_DepartmentID",
                schema: "dbo",
                table: "Questionnaire",
                column: "DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaire_Department_DepartmentID",
                schema: "dbo",
                table: "Questionnaire",
                column: "DepartmentID",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaire_Questionnaire_ParentID",
                schema: "dbo",
                table: "Questionnaire",
                column: "ParentID",
                principalSchema: "dbo",
                principalTable: "Questionnaire",
                principalColumn: "QuestionnaireId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
