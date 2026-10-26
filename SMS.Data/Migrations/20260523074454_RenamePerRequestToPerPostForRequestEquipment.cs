using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenamePerRequestToPerPostForRequestEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EquipmentPerRequest",
                schema: "dbo",
                table: "RequestEquipment",
                newName: "EquipmentPerPost");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EquipmentPerPost",
                schema: "dbo",
                table: "RequestEquipment",
                newName: "EquipmentPerRequest");
        }
    }
}
