using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnInDepartmentFeeFromDepartmentIdToDepartmenTID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentFee_Department_DepartmentId",
                schema: "dbo",
                table: "DepartmentFee");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                schema: "dbo",
                table: "DepartmentFee",
                newName: "DepartmentID");

            migrationBuilder.RenameIndex(
                name: "IX_DepartmentFee_DepartmentId",
                schema: "dbo",
                table: "DepartmentFee",
                newName: "IX_DepartmentFee_DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentFee_Department_DepartmentID",
                schema: "dbo",
                table: "DepartmentFee",
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
                name: "FK_DepartmentFee_Department_DepartmentID",
                schema: "dbo",
                table: "DepartmentFee");

            migrationBuilder.RenameColumn(
                name: "DepartmentID",
                schema: "dbo",
                table: "DepartmentFee",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_DepartmentFee_DepartmentID",
                schema: "dbo",
                table: "DepartmentFee",
                newName: "IX_DepartmentFee_DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentFee_Department_DepartmentId",
                schema: "dbo",
                table: "DepartmentFee",
                column: "DepartmentId",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
