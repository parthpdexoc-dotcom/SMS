using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsUsedForActiveInActiveInRequiredTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                schema: "dbo",
                table: "Skill",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                schema: "dbo",
                table: "RoutingGroup",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                schema: "dbo",
                table: "HolidayDay",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                schema: "dbo",
                table: "EquipmentType",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                schema: "dbo",
                table: "EmployeeType",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                schema: "dbo",
                table: "EmployeeStatus",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                schema: "dbo",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                schema: "dbo",
                table: "DepartmentResponseArea",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                schema: "dbo",
                table: "DepartmentFee",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                schema: "dbo",
                table: "DepartmentEquipment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                schema: "dbo",
                table: "DepartmentDivisions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                schema: "dbo",
                table: "DepartmentActivityType",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                schema: "dbo",
                table: "BusinessType",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUsed",
                schema: "dbo",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                schema: "dbo",
                table: "RoutingGroup");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                schema: "dbo",
                table: "HolidayDay");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                schema: "dbo",
                table: "EquipmentType");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                schema: "dbo",
                table: "EmployeeStatus");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                schema: "dbo",
                table: "DepartmentResponseArea");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                schema: "dbo",
                table: "DepartmentFee");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                schema: "dbo",
                table: "DepartmentEquipment");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                schema: "dbo",
                table: "DepartmentDivisions");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                schema: "dbo",
                table: "DepartmentActivityType");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                schema: "dbo",
                table: "BusinessType");
        }
    }
}
