using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSupervisorColumnsInDepartmentConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSupervisorRequired",
                schema: "dbo",
                table: "DepartmentConfiguration",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoOfPositionsPerSupervisor",
                schema: "dbo",
                table: "DepartmentConfiguration",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSupervisorRequired",
                schema: "dbo",
                table: "DepartmentConfiguration");

            migrationBuilder.DropColumn(
                name: "NoOfPositionsPerSupervisor",
                schema: "dbo",
                table: "DepartmentConfiguration");
        }
    }
}
