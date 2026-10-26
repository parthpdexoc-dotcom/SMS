using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSupervisorAndIsEligibleToEmployeeTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEligible",
                schema: "dbo",
                table: "EmployeeType",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSupervisor",
                schema: "dbo",
                table: "EmployeeType",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEligible",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropColumn(
                name: "IsSupervisor",
                schema: "dbo",
                table: "EmployeeType");
        }
    }
}
