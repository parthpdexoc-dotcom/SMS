using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAdminPasswordToHashed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Change the column type from nvarchar(max) to nvarchar(60)
            // Password hashing will be handled automatically during login if password is not already hashed
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "dbo",
                table: "Admin",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert the column type back to nvarchar(max)
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                schema: "dbo",
                table: "Admin",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            // Note: We cannot revert the password back to plain text for security reasons
            // The password will remain hashed even after rollback
        }
    }
}
