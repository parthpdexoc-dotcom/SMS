using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnsNameInInvoiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestPhoneNumber",
                schema: "dbo",
                table: "Invoices",
                newName: "BillingToPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "RequestName",
                schema: "dbo",
                table: "Invoices",
                newName: "BillingToName");

            migrationBuilder.RenameColumn(
                name: "RequestEmail",
                schema: "dbo",
                table: "Invoices",
                newName: "BillingToEmail");

            migrationBuilder.RenameColumn(
                name: "RequestContact",
                schema: "dbo",
                table: "Invoices",
                newName: "BillingToContact");

            migrationBuilder.RenameColumn(
                name: "RequestAddress",
                schema: "dbo",
                table: "Invoices",
                newName: "BillingToAddress");

            migrationBuilder.RenameColumn(
                name: "DepartmentPhoneNumber",
                schema: "dbo",
                table: "Invoices",
                newName: "BillingFromPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "DepartmentName",
                schema: "dbo",
                table: "Invoices",
                newName: "BillingFromName");

            migrationBuilder.RenameColumn(
                name: "DepartmentEmail",
                schema: "dbo",
                table: "Invoices",
                newName: "BillingFromEmail");

            migrationBuilder.RenameColumn(
                name: "DepartmentContact",
                schema: "dbo",
                table: "Invoices",
                newName: "BillingFromContact");

            migrationBuilder.RenameColumn(
                name: "DepartmentAddress",
                schema: "dbo",
                table: "Invoices",
                newName: "BillingFromAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BillingToPhoneNumber",
                schema: "dbo",
                table: "Invoices",
                newName: "RequestPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "BillingToName",
                schema: "dbo",
                table: "Invoices",
                newName: "RequestName");

            migrationBuilder.RenameColumn(
                name: "BillingToEmail",
                schema: "dbo",
                table: "Invoices",
                newName: "RequestEmail");

            migrationBuilder.RenameColumn(
                name: "BillingToContact",
                schema: "dbo",
                table: "Invoices",
                newName: "RequestContact");

            migrationBuilder.RenameColumn(
                name: "BillingToAddress",
                schema: "dbo",
                table: "Invoices",
                newName: "RequestAddress");

            migrationBuilder.RenameColumn(
                name: "BillingFromPhoneNumber",
                schema: "dbo",
                table: "Invoices",
                newName: "DepartmentPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "BillingFromName",
                schema: "dbo",
                table: "Invoices",
                newName: "DepartmentName");

            migrationBuilder.RenameColumn(
                name: "BillingFromEmail",
                schema: "dbo",
                table: "Invoices",
                newName: "DepartmentEmail");

            migrationBuilder.RenameColumn(
                name: "BillingFromContact",
                schema: "dbo",
                table: "Invoices",
                newName: "DepartmentContact");

            migrationBuilder.RenameColumn(
                name: "BillingFromAddress",
                schema: "dbo",
                table: "Invoices",
                newName: "DepartmentAddress");
        }
    }
}
