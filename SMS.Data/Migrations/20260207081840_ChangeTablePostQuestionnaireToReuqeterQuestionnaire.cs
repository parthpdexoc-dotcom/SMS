using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTablePostQuestionnaireToReuqeterQuestionnaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostQuestionnaire",
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "RequesterQuestionnaire",
                schema: "dbo",
                columns: table => new
                {
                    RequesterQuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    RequesterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequesterQuestionnaire", x => x.RequesterQuestionnaireId);
                    table.ForeignKey(
                        name: "FK_RequesterQuestionnaire_Questionnaire_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalSchema: "dbo",
                        principalTable: "Questionnaire",
                        principalColumn: "QuestionnaireId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequesterQuestionnaire_Requester_RequesterId",
                        column: x => x.RequesterId,
                        principalSchema: "dbo",
                        principalTable: "Requester",
                        principalColumn: "RequesterID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequesterQuestionnaire_QuestionnaireId",
                schema: "dbo",
                table: "RequesterQuestionnaire",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_RequesterQuestionnaire_RequesterId",
                schema: "dbo",
                table: "RequesterQuestionnaire",
                column: "RequesterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequesterQuestionnaire",
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "PostQuestionnaire",
                schema: "dbo",
                columns: table => new
                {
                    PostQuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostQuestionnaire", x => x.PostQuestionnaireId);
                    table.ForeignKey(
                        name: "FK_PostQuestionnaire_Post_PostId",
                        column: x => x.PostId,
                        principalSchema: "dbo",
                        principalTable: "Post",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostQuestionnaire_Questionnaire_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalSchema: "dbo",
                        principalTable: "Questionnaire",
                        principalColumn: "QuestionnaireId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostQuestionnaire_PostId",
                schema: "dbo",
                table: "PostQuestionnaire",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostQuestionnaire_QuestionnaireId",
                schema: "dbo",
                table: "PostQuestionnaire",
                column: "QuestionnaireId");
        }
    }
}
