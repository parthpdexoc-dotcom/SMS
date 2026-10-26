using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class DepartmentFee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultFeeConditions",
                schema: "dbo",
                table: "DepartmentFee",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdditionalFees",
                schema: "dbo",
                table: "DepartmentFee",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultFeeConditions",
                schema: "dbo",
                table: "DepartmentFee");

            migrationBuilder.DropColumn(
                name: "IsAdditionalFees",
                schema: "dbo",
                table: "DepartmentFee");
        }
    }
}
