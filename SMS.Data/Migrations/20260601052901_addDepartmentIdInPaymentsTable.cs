using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class addDepartmentIdInPaymentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentID",
                schema: "dbo",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Backfill existing payments from their requester's department.
            migrationBuilder.Sql(@"
UPDATE p
SET p.[DepartmentID] = r.[DepartmentID]
FROM [dbo].[Payments] p
INNER JOIN [dbo].[Requester] r ON r.[RequesterID] = p.[RequesterID]
WHERE r.[DepartmentID] <> '00000000-0000-0000-0000-000000000000';
");

            // Fallback: payments still unassigned use department from a linked invoice (via allocations).
            migrationBuilder.Sql(@"
UPDATE p
SET p.[DepartmentID] = x.[DepartmentID]
FROM [dbo].[Payments] p
INNER JOIN (
    SELECT pa.[PaymentID], MIN(i.[DepartmentID]) AS [DepartmentID]
    FROM [dbo].[PaymentAllocations] pa
    INNER JOIN [dbo].[Invoices] i ON i.[InvoicesID] = pa.[InvoicesID]
    WHERE i.[DepartmentID] IS NOT NULL
      AND i.[DepartmentID] <> '00000000-0000-0000-0000-000000000000'
    GROUP BY pa.[PaymentID]
) x ON x.[PaymentID] = p.[PaymentID]
WHERE p.[DepartmentID] = '00000000-0000-0000-0000-000000000000';
");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_DepartmentID",
                schema: "dbo",
                table: "Payments",
                column: "DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Department_DepartmentID",
                schema: "dbo",
                table: "Payments",
                column: "DepartmentID",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Department_DepartmentID",
                schema: "dbo",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_DepartmentID",
                schema: "dbo",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "DepartmentID",
                schema: "dbo",
                table: "Payments");
        }
    }
}
