using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class DatatypeChangeInCancelledPostPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ReimbursePayRate",
                schema: "dbo",
                table: "CancelledPostPosition",
                type: "smallmoney",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PayRate",
                schema: "dbo",
                table: "CancelledPostPosition",
                type: "smallmoney",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ReimbursePayRate",
                schema: "dbo",
                table: "CancelledPostPosition",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "smallmoney",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PayRate",
                schema: "dbo",
                table: "CancelledPostPosition",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "smallmoney",
                oldNullable: true);
        }
    }
}
