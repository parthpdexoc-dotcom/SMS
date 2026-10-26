using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentAllocationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentAllocations",
                schema: "dbo",
                columns: table => new
                {
                    PaymentAllocationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    InvoicesID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmountApplied = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AllocationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAllocations", x => x.PaymentAllocationID);
                    table.ForeignKey(
                        name: "FK_PaymentAllocations_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentAllocations_Employee_DeletedUserID",
                        column: x => x.DeletedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentAllocations_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentAllocations_Invoices_InvoicesID",
                        column: x => x.InvoicesID,
                        principalSchema: "dbo",
                        principalTable: "Invoices",
                        principalColumn: "InvoicesID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentAllocations_Payments_PaymentID",
                        column: x => x.PaymentID,
                        principalSchema: "dbo",
                        principalTable: "Payments",
                        principalColumn: "PaymentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAllocations_CreatedUserID",
                schema: "dbo",
                table: "PaymentAllocations",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAllocations_DeletedUserID",
                schema: "dbo",
                table: "PaymentAllocations",
                column: "DeletedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAllocations_EditedUserID",
                schema: "dbo",
                table: "PaymentAllocations",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAllocations_InvoicesID",
                schema: "dbo",
                table: "PaymentAllocations",
                column: "InvoicesID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAllocations_PaymentID",
                schema: "dbo",
                table: "PaymentAllocations",
                column: "PaymentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentAllocations",
                schema: "dbo");
        }
    }
}
