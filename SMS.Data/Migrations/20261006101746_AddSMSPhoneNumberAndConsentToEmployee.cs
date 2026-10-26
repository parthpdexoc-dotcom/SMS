using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSMSPhoneNumberAndConsentToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasSmsConsent",
                schema: "dbo",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SmsTelephoneNumber",
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
                name: "HasSmsConsent",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "SmsTelephoneNumber",
                schema: "dbo",
                table: "Employee");
        }
    }
}
