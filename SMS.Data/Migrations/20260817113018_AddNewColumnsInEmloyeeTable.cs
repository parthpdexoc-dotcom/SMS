using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnsInEmloyeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NotifyJobStartingIn1Hour",
                schema: "dbo",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyJobStartingIn24Hours",
                schema: "dbo",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyWhenJobsPosted",
                schema: "dbo",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifyJobStartingIn1Hour",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "NotifyJobStartingIn24Hours",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "NotifyWhenJobsPosted",
                schema: "dbo",
                table: "Employee");
        }
    }
}
