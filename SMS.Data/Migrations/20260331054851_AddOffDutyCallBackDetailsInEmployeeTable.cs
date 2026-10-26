using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOffDutyCallBackDetailsInEmployeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OffDutyCallBackEmail",
                schema: "dbo",
                table: "Employee",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OffDutyCallBackPhoneNumber",
                schema: "dbo",
                table: "Employee",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OffDutyCallBackEmail",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "OffDutyCallBackPhoneNumber",
                schema: "dbo",
                table: "Employee");
        }
    }
}
