using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPostQuestionnaireTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostQuestionnaire",
                schema: "dbo");
        }
    }
}
