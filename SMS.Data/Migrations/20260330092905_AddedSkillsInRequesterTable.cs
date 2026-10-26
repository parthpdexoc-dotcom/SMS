using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedSkillsInRequesterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSkillsRequired",
                schema: "dbo",
                table: "Requester",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SkillIDs",
                schema: "dbo",
                table: "Requester",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSkillsRequired",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropColumn(
                name: "SkillIDs",
                schema: "dbo",
                table: "Requester");
        }
    }
}
