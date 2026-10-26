using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReimburseStatusTypeInClosePostPositionEquipmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReimburseStatus",
                schema: "dbo",
                table: "ClosedPostPositionEquipment");

            migrationBuilder.AddColumn<int>(
                name: "ReimburseTo",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReimburseTo",
                schema: "dbo",
                table: "ClosedPostPositionEquipment");

            migrationBuilder.AddColumn<string>(
                name: "ReimburseStatus",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
