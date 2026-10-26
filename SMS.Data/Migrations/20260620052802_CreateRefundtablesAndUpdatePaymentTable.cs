using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateRefundtablesAndUpdatePaymentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                schema: "dbo",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                schema: "dbo",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                schema: "dbo",
                table: "PaymentAllocations");

            migrationBuilder.CreateTable(
                name: "RefundPayments",
                schema: "dbo",
                columns: table => new
                {
                    RefundPaymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    RequesterID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RefundPaymentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
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
                    table.PrimaryKey("PK_RefundPayments", x => x.RefundPaymentID);
                    table.ForeignKey(
                        name: "FK_RefundPayments_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefundPayments_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                    table.ForeignKey(
                        name: "FK_RefundPayments_Employee_DeletedUserID",
                        column: x => x.DeletedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefundPayments_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                    table.ForeignKey(
                        name: "FK_RefundPayments_Requester_RequesterID",
                        column: x => x.RequesterID,
                        principalSchema: "dbo",
                        principalTable: "Requester",
                        principalColumn: "RequesterID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefundPaymentAllocations",
                schema: "dbo",
                columns: table => new
                {
                    RefundPaymentAllocationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    PaymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RefundPaymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmountApplied = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AllocationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    TransactionType = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
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
                    table.PrimaryKey("PK_RefundPaymentAllocations", x => x.RefundPaymentAllocationID);
                    table.ForeignKey(
                        name: "FK_RefundPaymentAllocations_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefundPaymentAllocations_Employee_DeletedUserID",
                        column: x => x.DeletedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefundPaymentAllocations_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefundPaymentAllocations_Payments_PaymentID",
                        column: x => x.PaymentID,
                        principalSchema: "dbo",
                        principalTable: "Payments",
                        principalColumn: "PaymentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefundPaymentAllocations_RefundPayments_RefundPaymentID",
                        column: x => x.RefundPaymentID,
                        principalSchema: "dbo",
                        principalTable: "RefundPayments",
                        principalColumn: "RefundPaymentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefundPaymentAllocations_CreatedUserID",
                schema: "dbo",
                table: "RefundPaymentAllocations",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RefundPaymentAllocations_DeletedUserID",
                schema: "dbo",
                table: "RefundPaymentAllocations",
                column: "DeletedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RefundPaymentAllocations_EditedUserID",
                schema: "dbo",
                table: "RefundPaymentAllocations",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RefundPaymentAllocations_PaymentID",
                schema: "dbo",
                table: "RefundPaymentAllocations",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_RefundPaymentAllocations_RefundPaymentID",
                schema: "dbo",
                table: "RefundPaymentAllocations",
                column: "RefundPaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_RefundPayments_CreatedUserID",
                schema: "dbo",
                table: "RefundPayments",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RefundPayments_DeletedUserID",
                schema: "dbo",
                table: "RefundPayments",
                column: "DeletedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RefundPayments_DepartmentID",
                schema: "dbo",
                table: "RefundPayments",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_RefundPayments_EditedUserID",
                schema: "dbo",
                table: "RefundPayments",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RefundPayments_RequesterID",
                schema: "dbo",
                table: "RefundPayments",
                column: "RequesterID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefundPaymentAllocations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RefundPayments",
                schema: "dbo");

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                schema: "dbo",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "TransactionType",
                schema: "dbo",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                schema: "dbo",
                table: "PaymentAllocations",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }
    }
}
