using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class ForeignKeyUpgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Employee_DepartmentID",
                schema: "dbo",
                table: "Employee",
                column: "DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Department_DepartmentID",
                schema: "dbo",
                table: "Employee",
                column: "DepartmentID",
                principalSchema: "dbo",
                principalTable: "Department",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Department_DepartmentID",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_DepartmentID",
                schema: "dbo",
                table: "Employee");
        }
    }
}
