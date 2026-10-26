using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Backfill existing data
            migrationBuilder.Sql(@"
                UPDATE dbo.Request
                SET NumberOfEquipment = 5
                WHERE NumberOfEquipment IS NULL;

                UPDATE dbo.Request
                SET NumberOfVehicles = 5
                WHERE NumberOfVehicles IS NULL;
            ");


            migrationBuilder.AlterColumn<int>(
                name: "NumberOfVehicles",
                schema: "dbo",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 5,
                oldClrType: typeof(int),
                oldNullable: true);

            // Optional: keep NumberOfEquipment nullable, but add default
            migrationBuilder.AlterColumn<int>(
                name: "NumberOfEquipment",
                schema: "dbo",
                table: "Request",
                type: "int",
                nullable: true,
                defaultValue: 5,
                oldClrType: typeof(int),
                oldNullable: true);

            // 3. Add enum column
            migrationBuilder.AddColumn<int>(
                name: "RequestType",
                schema: "dbo",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 1); // OneTime
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestType",
                schema: "dbo",
                table: "Request");

            migrationBuilder.AlterColumn<int>(
                name: "NumberOfVehicles",
                schema: "dbo",
                table: "Request",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 5);

            migrationBuilder.AlterColumn<int>(
                name: "NumberOfEquipment",
                schema: "dbo",
                table: "Request",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 5);
        }
    }
}
