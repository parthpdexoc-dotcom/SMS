using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameRequestColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DaysOfMonth",
                schema: "dbo",
                table: "Request",
                newName: "MonthDays");

            migrationBuilder.AlterColumn<string>(
                name: "MonthDays",
                schema: "dbo",
                table: "Request",
                type: "varchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MonthDays",
                schema: "dbo",
                table: "Request",
                newName: "DaysOfMonth");

            migrationBuilder.AlterColumn<string>(
                name: "DaysOfMonth",
                schema: "dbo",
                table: "Request",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldNullable: true);
        }
    }
}
