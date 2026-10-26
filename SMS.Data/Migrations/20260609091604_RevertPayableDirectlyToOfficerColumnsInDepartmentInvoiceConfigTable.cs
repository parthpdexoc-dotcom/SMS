using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RevertPayableDirectlyToOfficerColumnsInDepartmentInvoiceConfigTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PayableDirectlyToOfficer",
                schema: "dbo",
                table: "DepartmentInvoiceConfig",
                newName: "PayableDirectlyTo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PayableDirectlyTo",
                schema: "dbo",
                table: "DepartmentInvoiceConfig",
                newName: "PayableDirectlyToOfficer");
        }
    }
}
