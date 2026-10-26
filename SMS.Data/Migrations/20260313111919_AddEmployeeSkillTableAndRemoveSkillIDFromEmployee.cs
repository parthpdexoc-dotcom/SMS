using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeSkillTableAndRemoveSkillIDFromEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkillIDs",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.CreateTable(
                name: "EmployeeSkill",
                schema: "dbo",
                columns: table => new
                {
                    EmployeeSkillID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSkill", x => x.EmployeeSkillID);
                    table.ForeignKey(
                        name: "FK_EmployeeSkill_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                    table.ForeignKey(
                        name: "FK_EmployeeSkill_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                    table.ForeignKey(
                        name: "FK_EmployeeSkill_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeSkill_Skill_SkillID",
                        column: x => x.SkillID,
                        principalSchema: "dbo",
                        principalTable: "Skill",
                        principalColumn: "SkillID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkill_CreatedUserID",
                schema: "dbo",
                table: "EmployeeSkill",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkill_EditedUserID",
                schema: "dbo",
                table: "EmployeeSkill",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkill_EmployeeID",
                schema: "dbo",
                table: "EmployeeSkill",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkill_SkillID",
                schema: "dbo",
                table: "EmployeeSkill",
                column: "SkillID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeSkill",
                schema: "dbo");

            migrationBuilder.AddColumn<string>(
                name: "SkillIDs",
                schema: "dbo",
                table: "Employee",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
