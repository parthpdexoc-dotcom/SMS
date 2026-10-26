using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPayableColumnsToDepartmentInvoiceConfigTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PayableDirectlyTo",
                schema: "dbo",
                table: "DepartmentInvoiceConfig",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "PayableToOfficer",
                schema: "dbo",
                table: "DepartmentInvoiceConfig",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayableDirectlyTo",
                schema: "dbo",
                table: "DepartmentInvoiceConfig");

            migrationBuilder.DropColumn(
                name: "PayableToOfficer",
                schema: "dbo",
                table: "DepartmentInvoiceConfig");
        }
    }
}
