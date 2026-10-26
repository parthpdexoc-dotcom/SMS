using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsAllowedLoginAndAddIsApprovedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAllowedLogin",
                schema: "dbo",
                table: "EmployeeStatus");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                schema: "dbo",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.AddColumn<bool>(
                name: "IsAllowedLogin",
                schema: "dbo",
                table: "EmployeeStatus",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }
    }
}
