using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NumberOfVehicles",
                schema: "dbo",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 5,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 5);

            migrationBuilder.AlterColumn<int>(
                name: "NumberOfEquipment",
                schema: "dbo",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 5,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 5);

            migrationBuilder.AddColumn<string>(
                name: "WeekDays",
                schema: "dbo",
                table: "Request",
                type: "varchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeekDays",
                schema: "dbo",
                table: "Request");

            migrationBuilder.AlterColumn<int>(
                name: "NumberOfVehicles",
                schema: "dbo",
                table: "Request",
                type: "int",
                nullable: true,
                defaultValue: 5,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 5);

            migrationBuilder.AlterColumn<int>(
                name: "NumberOfEquipment",
                schema: "dbo",
                table: "Request",
                type: "int",
                nullable: true,
                defaultValue: 5,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 5);
        }
    }
}
