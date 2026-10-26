using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentQuestionnaireTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepartmentQuestionnaire",
                schema: "dbo",
                columns: table => new
                {
                    DepartmentQuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentQuestionnaire", x => x.DepartmentQuestionnaireId);
                    table.ForeignKey(
                        name: "FK_DepartmentQuestionnaire_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentQuestionnaire_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentQuestionnaire_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentQuestionnaire_Questionnaire_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalSchema: "dbo",
                        principalTable: "Questionnaire",
                        principalColumn: "QuestionnaireId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentQuestionnaire_CreatedUserID",
                schema: "dbo",
                table: "DepartmentQuestionnaire",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentQuestionnaire_DepartmentID",
                schema: "dbo",
                table: "DepartmentQuestionnaire",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentQuestionnaire_EditedUserID",
                schema: "dbo",
                table: "DepartmentQuestionnaire",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentQuestionnaire_QuestionnaireId",
                schema: "dbo",
                table: "DepartmentQuestionnaire",
                column: "QuestionnaireId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentQuestionnaire",
                schema: "dbo");
        }
    }
}
