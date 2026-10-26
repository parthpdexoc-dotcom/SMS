using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateDepartmentInvoiceConfigTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepartmentInvoiceConfig",
                schema: "dbo",
                columns: table => new
                {
                    InvoiceConfigID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FiscalYear = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true),
                    StaffReimbursement = table.Column<decimal>(type: "smallmoney", nullable: true),
                    MileageReimburseEligible = table.Column<bool>(type: "bit", nullable: true),
                    VehicleReimbursement = table.Column<decimal>(type: "smallmoney", nullable: true),
                    MileageReimbursedRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DefaultAdminFeeDollar = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DefaultAdminFeePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DefaultPayrollTax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalCombinedAdminFees = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DepartmentAdminFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CancellationMinimumFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MinimumFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AgencySupportFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentInvoiceConfig", x => x.InvoiceConfigID);
                    table.ForeignKey(
                        name: "FK_DepartmentInvoiceConfig_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentInvoiceConfig_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                    table.ForeignKey(
                        name: "FK_DepartmentInvoiceConfig_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentInvoiceConfig_CreatedUserID",
                schema: "dbo",
                table: "DepartmentInvoiceConfig",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentInvoiceConfig_DepartmentID",
                schema: "dbo",
                table: "DepartmentInvoiceConfig",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentInvoiceConfig_EditedUserID",
                schema: "dbo",
                table: "DepartmentInvoiceConfig",
                column: "EditedUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentInvoiceConfig",
                schema: "dbo");
        }
    }
}
