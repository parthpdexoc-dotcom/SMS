using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRestrictWebLoginForNonAdminEmployeeToDepartmentPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RestrictWebLoginForNonAdminEmployee",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestrictWebLoginForNonAdminEmployee",
                schema: "dbo",
                table: "DepartmentPolicy");
        }
    }
}
