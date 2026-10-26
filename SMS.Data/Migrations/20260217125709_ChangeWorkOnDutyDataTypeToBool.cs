using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeWorkOnDutyDataTypeToBool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "WorkedOnDuty",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WorkedOnDuty",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "int",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
