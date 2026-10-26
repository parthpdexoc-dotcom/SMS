using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPayCodeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PayCode",
                schema: "dbo",
                columns: table => new
                {
                    PayCodeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayCodeValue = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    PayCodeDescription = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    PayCodeStatus = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayCode", x => x.PayCodeID);
                    table.ForeignKey(
                        name: "FK_PayCode_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayCode_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayCode_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayCode_CreatedUserID",
                schema: "dbo",
                table: "PayCode",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_PayCode_DepartmentID",
                schema: "dbo",
                table: "PayCode",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_PayCode_EditedUserID",
                schema: "dbo",
                table: "PayCode",
                column: "EditedUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayCode",
                schema: "dbo");
        }
    }
}
