using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeetenureTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeTenure",
                schema: "dbo",
                columns: table => new
                {
                    EmployeeTenureID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateOfHire = table.Column<DateTime>(type: "date", nullable: true),
                    DateOfPromotion = table.Column<DateTime>(type: "date", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTenure", x => x.EmployeeTenureID);
                    table.ForeignKey(
                        name: "FK_EmployeeTenure_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTenure_EmployeeType_EmployeeTypeID",
                        column: x => x.EmployeeTypeID,
                        principalSchema: "dbo",
                        principalTable: "EmployeeType",
                        principalColumn: "EmployeeTypeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTenure_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTenure_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeTenure_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTenure_CreatedUserID",
                schema: "dbo",
                table: "EmployeeTenure",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTenure_DepartmentID",
                schema: "dbo",
                table: "EmployeeTenure",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTenure_EditedUserID",
                schema: "dbo",
                table: "EmployeeTenure",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTenure_EmployeeID",
                schema: "dbo",
                table: "EmployeeTenure",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTenure_EmployeeTypeID",
                schema: "dbo",
                table: "EmployeeTenure",
                column: "EmployeeTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeTenure",
                schema: "dbo");
        }
    }
}
