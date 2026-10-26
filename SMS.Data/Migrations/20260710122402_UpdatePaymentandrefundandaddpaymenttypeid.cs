using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentandrefundandaddpaymenttypeid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PaymentTypeID",
                schema: "dbo",
                table: "RefundPayments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentTypeID",
                schema: "dbo",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefundPayments_PaymentTypeID",
                schema: "dbo",
                table: "RefundPayments",
                column: "PaymentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentTypeID",
                schema: "dbo",
                table: "Payments",
                column: "PaymentTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentType_PaymentTypeID",
                schema: "dbo",
                table: "Payments",
                column: "PaymentTypeID",
                principalSchema: "dbo",
                principalTable: "PaymentType",
                principalColumn: "PaymentTypeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RefundPayments_PaymentType_PaymentTypeID",
                schema: "dbo",
                table: "RefundPayments",
                column: "PaymentTypeID",
                principalSchema: "dbo",
                principalTable: "PaymentType",
                principalColumn: "PaymentTypeID",
                onDelete: ReferentialAction.Restrict);

            // Backfill: departments with payment/refund history get Prepaid (Cash=1) and Invoiced (Check=2),
            // Invoiced is the default; then existing rows are linked by PaymentTypeID.
            migrationBuilder.Sql(@"
                ;WITH DepartmentsWithPaymentActivity AS
                (
                    SELECT DISTINCT DepartmentID
                    FROM dbo.Payments
                    WHERE DepartmentID IS NOT NULL

                    UNION

                    SELECT DISTINCT DepartmentID
                    FROM dbo.RefundPayments
                    WHERE DepartmentID IS NOT NULL
                )
                INSERT INTO dbo.PaymentType (
                    PaymentTypeID,
                    DepartmentID,
                    Name,
                    IsDefault,
                    IsDeleted,
                    CreatedDate,
                    CreatedBy)
                SELECT
                    NEWID(),
                    d.DepartmentID,
                    'Prepaid',
                    0,
                    0,
                    GETUTCDATE(),
                    'MIGRATION'
                FROM DepartmentsWithPaymentActivity d
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM dbo.PaymentType pt
                    WHERE pt.DepartmentID = d.DepartmentID
                      AND pt.IsDeleted = 0
                      AND LOWER(LTRIM(RTRIM(pt.Name))) = N'prepaid');

                ;WITH DepartmentsWithPaymentActivity AS
                (
                    SELECT DISTINCT DepartmentID
                    FROM dbo.Payments
                    WHERE DepartmentID IS NOT NULL

                    UNION

                    SELECT DISTINCT DepartmentID
                    FROM dbo.RefundPayments
                    WHERE DepartmentID IS NOT NULL
                )
                INSERT INTO dbo.PaymentType (
                    PaymentTypeID,
                    DepartmentID,
                    Name,
                    IsDefault,
                    IsDeleted,
                    CreatedDate,
                    CreatedBy)
                SELECT
                    NEWID(),
                    d.DepartmentID,
                    'Invoiced',
                    1,
                    0,
                    GETUTCDATE(),
                    'MIGRATION'
                FROM DepartmentsWithPaymentActivity d
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM dbo.PaymentType pt
                    WHERE pt.DepartmentID = d.DepartmentID
                      AND pt.IsDeleted = 0
                      AND LOWER(LTRIM(RTRIM(pt.Name))) = N'invoiced');

                ;WITH DepartmentsWithPaymentActivity AS
                (
                    SELECT DISTINCT DepartmentID
                    FROM dbo.Payments
                    WHERE DepartmentID IS NOT NULL

                    UNION

                    SELECT DISTINCT DepartmentID
                    FROM dbo.RefundPayments
                    WHERE DepartmentID IS NOT NULL
                )
                UPDATE pt
                SET pt.IsDefault = CASE
                    WHEN LOWER(LTRIM(RTRIM(pt.Name))) = N'invoiced' THEN 1
                    ELSE 0
                END
                FROM dbo.PaymentType pt
                INNER JOIN DepartmentsWithPaymentActivity d
                    ON d.DepartmentID = pt.DepartmentID
                WHERE pt.IsDeleted = 0;

                UPDATE p
                SET p.PaymentTypeID = pt.PaymentTypeID
                FROM dbo.Payments p
                INNER JOIN dbo.PaymentType pt
                    ON pt.DepartmentID = p.DepartmentID
                   AND pt.IsDeleted = 0
                   AND LOWER(LTRIM(RTRIM(pt.Name))) = N'prepaid'
                WHERE p.PaymentType = 1
                  AND p.PaymentTypeID IS NULL;

                UPDATE p
                SET p.PaymentTypeID = pt.PaymentTypeID
                FROM dbo.Payments p
                INNER JOIN dbo.PaymentType pt
                    ON pt.DepartmentID = p.DepartmentID
                   AND pt.IsDeleted = 0
                   AND LOWER(LTRIM(RTRIM(pt.Name))) = N'invoiced'
                WHERE p.PaymentType = 2
                  AND p.PaymentTypeID IS NULL;

                UPDATE r
                SET r.PaymentTypeID = pt.PaymentTypeID
                FROM dbo.RefundPayments r
                INNER JOIN dbo.PaymentType pt
                    ON pt.DepartmentID = r.DepartmentID
                   AND pt.IsDeleted = 0
                   AND LOWER(LTRIM(RTRIM(pt.Name))) = N'prepaid'
                WHERE r.PaymentType = 1
                  AND r.PaymentTypeID IS NULL;

                UPDATE r
                SET r.PaymentTypeID = pt.PaymentTypeID
                FROM dbo.RefundPayments r
                INNER JOIN dbo.PaymentType pt
                    ON pt.DepartmentID = r.DepartmentID
                   AND pt.IsDeleted = 0
                   AND LOWER(LTRIM(RTRIM(pt.Name))) = N'invoiced'
                WHERE r.PaymentType = 2
                  AND r.PaymentTypeID IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentType_PaymentTypeID",
                schema: "dbo",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_RefundPayments_PaymentType_PaymentTypeID",
                schema: "dbo",
                table: "RefundPayments");

            migrationBuilder.DropIndex(
                name: "IX_RefundPayments_PaymentTypeID",
                schema: "dbo",
                table: "RefundPayments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentTypeID",
                schema: "dbo",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentTypeID",
                schema: "dbo",
                table: "RefundPayments");

            migrationBuilder.DropColumn(
                name: "PaymentTypeID",
                schema: "dbo",
                table: "Payments");
        }
    }
}
