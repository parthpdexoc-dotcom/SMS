using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    public partial class ChangeDataTypeFromLongToGUIDinInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop Foreign Key
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Invoices_InvoicesID",
                schema: "dbo",
                table: "InvoiceDetails");

            // Drop Index
            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetails_InvoicesID",
                schema: "dbo",
                table: "InvoiceDetails");

            // Drop Primary Keys
            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoices",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceDetails",
                schema: "dbo",
                table: "InvoiceDetails");

            // Drop old columns
            migrationBuilder.DropColumn(
                name: "InvoicesID",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceDetailID",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "InvoicesID",
                schema: "dbo",
                table: "InvoiceDetails");

            // Add GUID columns
            migrationBuilder.AddColumn<Guid>(
                name: "InvoicesID",
                schema: "dbo",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()");

            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceDetailID",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()");

            migrationBuilder.AddColumn<Guid>(
                name: "InvoicesID",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "uniqueidentifier",
                nullable: false);

            // Recreate Primary Keys
            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoices",
                schema: "dbo",
                table: "Invoices",
                column: "InvoicesID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceDetails",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "InvoiceDetailID");

            // Recreate Index
            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_InvoicesID",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "InvoicesID");

            // Recreate Foreign Key
            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_Invoices_InvoicesID",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "InvoicesID",
                principalSchema: "dbo",
                principalTable: "Invoices",
                principalColumn: "InvoicesID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Invoices_InvoicesID",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetails_InvoicesID",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoices",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceDetails",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "InvoicesID",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceDetailID",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "InvoicesID",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.AddColumn<long>(
                name: "InvoicesID",
                schema: "dbo",
                table: "Invoices",
                type: "bigint",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "InvoiceDetailID",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "bigint",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "InvoicesID",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "bigint",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoices",
                schema: "dbo",
                table: "Invoices",
                column: "InvoicesID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceDetails",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "InvoiceDetailID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_InvoicesID",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "InvoicesID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_Invoices_InvoicesID",
                schema: "dbo",
                table: "InvoiceDetails",
                column: "InvoicesID",
                principalSchema: "dbo",
                principalTable: "Invoices",
                principalColumn: "InvoicesID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}