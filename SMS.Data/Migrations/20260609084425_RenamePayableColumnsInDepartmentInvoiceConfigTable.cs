using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenamePayableColumnsInDepartmentInvoiceConfigTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PayableToOfficer",
                schema: "dbo",
                table: "DepartmentInvoiceConfig",
                newName: "IsPayableToOfficer");

            migrationBuilder.RenameColumn(
                name: "PayableDirectlyTo",
                schema: "dbo",
                table: "DepartmentInvoiceConfig",
                newName: "PayableDirectlyToOfficer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PayableDirectlyToOfficer",
                schema: "dbo",
                table: "DepartmentInvoiceConfig",
                newName: "PayableDirectlyTo");

            migrationBuilder.RenameColumn(
                name: "IsPayableToOfficer",
                schema: "dbo",
                table: "DepartmentInvoiceConfig",
                newName: "PayableToOfficer");
        }
    }
}
