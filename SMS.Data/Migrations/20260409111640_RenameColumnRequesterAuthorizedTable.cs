using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnRequesterAuthorizedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthorizedRequesterID",
                schema: "dbo",
                table: "RequesterAuthorized",
                newName: "RequesterAuthorizedID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequesterAuthorizedID",
                schema: "dbo",
                table: "RequesterAuthorized",
                newName: "AuthorizedRequesterID");
        }
    }
}
