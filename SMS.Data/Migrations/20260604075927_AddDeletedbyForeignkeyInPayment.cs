using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedbyForeignkeyInPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Payments_DeletedUserID",
                schema: "dbo",
                table: "Payments",
                column: "DeletedUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Employee_DeletedUserID",
                schema: "dbo",
                table: "Payments",
                column: "DeletedUserID",
                principalSchema: "dbo",
                principalTable: "Employee",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Employee_DeletedUserID",
                schema: "dbo",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_DeletedUserID",
                schema: "dbo",
                table: "Payments");
        }
    }
}
