using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class removedEnableNumberOfEmployeesBeforeSupervisorRequiredField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnableNumberOfEmployeesBeforeSupervisorRequired",
                schema: "dbo",
                table: "DepartmentConfiguration");

            migrationBuilder.RenameColumn(
                name: "NoOfPositionsPerSupervisor",
                schema: "dbo",
                table: "DepartmentConfiguration",
                newName: "NumberOfPositionsPerSupervisor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberOfPositionsPerSupervisor",
                schema: "dbo",
                table: "DepartmentConfiguration",
                newName: "NoOfPositionsPerSupervisor");

            migrationBuilder.AddColumn<bool>(
                name: "EnableNumberOfEmployeesBeforeSupervisorRequired",
                schema: "dbo",
                table: "DepartmentConfiguration",
                type: "bit",
                nullable: true);
        }
    }
}
