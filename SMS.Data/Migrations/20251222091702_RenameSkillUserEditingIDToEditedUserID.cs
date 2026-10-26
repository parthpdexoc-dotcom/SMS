using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameSkillUserEditingIDToEditedUserID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skill_Employee_UserEditingID",
                schema: "dbo",
                table: "Skill");

            migrationBuilder.RenameColumn(
                name: "UserEditingID",
                schema: "dbo",
                table: "Skill",
                newName: "EditedUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Skill_UserEditingID",
                schema: "dbo",
                table: "Skill",
                newName: "IX_Skill_EditedUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_Employee_EditedUserID",
                schema: "dbo",
                table: "Skill",
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
                name: "FK_Skill_Employee_EditedUserID",
                schema: "dbo",
                table: "Skill");

            migrationBuilder.RenameColumn(
                name: "EditedUserID",
                schema: "dbo",
                table: "Skill",
                newName: "UserEditingID");

            migrationBuilder.RenameIndex(
                name: "IX_Skill_EditedUserID",
                schema: "dbo",
                table: "Skill",
                newName: "IX_Skill_UserEditingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_Employee_UserEditingID",
                schema: "dbo",
                table: "Skill",
                column: "UserEditingID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
