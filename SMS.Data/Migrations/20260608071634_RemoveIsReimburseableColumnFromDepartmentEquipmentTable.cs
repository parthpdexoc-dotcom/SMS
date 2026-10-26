using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsReimburseableColumnFromDepartmentEquipmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReimburseable",
                schema: "dbo",
                table: "DepartmentEquipment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReimburseable",
                schema: "dbo",
                table: "DepartmentEquipment",
                type: "bit",
                nullable: true);
        }
    }
}
