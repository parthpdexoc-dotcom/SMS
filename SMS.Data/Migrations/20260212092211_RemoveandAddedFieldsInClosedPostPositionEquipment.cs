using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveandAddedFieldsInClosedPostPositionEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayRate",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.DropColumn(
                name: "ReimbursePayRate",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.DropColumn(
                name: "ReimburseStatus",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.DropColumn(
                name: "Split",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.AddColumn<decimal>(
                name: "PayRate",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReimbursePayRate",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReimburseStatus",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSplit",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayRate",
                schema: "dbo",
                table: "ClosedPostPositionEquipment");

            migrationBuilder.DropColumn(
                name: "ReimbursePayRate",
                schema: "dbo",
                table: "ClosedPostPositionEquipment");

            migrationBuilder.DropColumn(
                name: "ReimburseStatus",
                schema: "dbo",
                table: "ClosedPostPositionEquipment");

            migrationBuilder.DropColumn(
                name: "IsSplit",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.AddColumn<decimal>(
                name: "PayRate",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReimbursePayRate",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReimburseStatus",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Split",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
