using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentAdminTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepartmentAdmin",
                schema: "dbo",
                columns: table => new
                {
                    DepartmentAdminID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentAdmin", x => x.DepartmentAdminID);
                    table.ForeignKey(
                        name: "FK_DepartmentAdmin_AdminType_AdminTypeID",
                        column: x => x.AdminTypeID,
                        principalSchema: "dbo",
                        principalTable: "AdminType",
                        principalColumn: "AdminTypeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentAdmin_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentAdmin_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentAdmin_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentAdmin_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentAdmin_AdminTypeID",
                schema: "dbo",
                table: "DepartmentAdmin",
                column: "AdminTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentAdmin_CreatedUserID",
                schema: "dbo",
                table: "DepartmentAdmin",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentAdmin_DepartmentID",
                schema: "dbo",
                table: "DepartmentAdmin",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentAdmin_EditedUserID",
                schema: "dbo",
                table: "DepartmentAdmin",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentAdmin_EmployeeID",
                schema: "dbo",
                table: "DepartmentAdmin",
                column: "EmployeeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentAdmin",
                schema: "dbo");

        }
    }
}
