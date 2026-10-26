using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyInvoiceColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invoices",
                schema: "dbo",
                columns: table => new
                {
                    InvoicesID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    DueDate = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DepartmentAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DepartmentPhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DepartmentEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DepartmentContact = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequestAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestPhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RequestEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestContact = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OfficerHourlyTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OfficerFeesTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AgencyFeesTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VehicleFeesTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EquipmentFeesTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentsApplied = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BalanceDue = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoicesID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices",
                schema: "dbo");
        }
    }
}
