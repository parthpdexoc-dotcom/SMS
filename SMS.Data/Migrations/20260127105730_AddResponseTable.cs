using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddResponseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Response",
                schema: "dbo",
                columns: table => new
                {
                    ResponseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubmittedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmitterEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SubmittedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    PostPositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Response", x => x.ResponseId);
                    table.ForeignKey(
                        name: "FK_Response_Employee_SubmittedBy",
                        column: x => x.SubmittedBy,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Response_PostPosition_PostPositionId",
                        column: x => x.PostPositionId,
                        principalSchema: "dbo",
                        principalTable: "PostPosition",
                        principalColumn: "PostPositionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Response_Questionnaire_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalSchema: "dbo",
                        principalTable: "Questionnaire",
                        principalColumn: "QuestionnaireId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Response_PostPositionId",
                schema: "dbo",
                table: "Response",
                column: "PostPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Response_QuestionnaireId",
                schema: "dbo",
                table: "Response",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_Response_SubmittedBy",
                schema: "dbo",
                table: "Response",
                column: "SubmittedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Response",
                schema: "dbo");
        }
    }
}
