using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class renameresponsearea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create DepartmentResponseArea table (replacing the manually removed ResponseArea table)
            migrationBuilder.CreateTable(
                name: "DepartmentResponseArea",
                schema: "dbo",
                columns: table => new
                {
                    DepartmentResponseAreaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentResponseAreaShort = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    DepartmentResponseAreaLong = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    DepartmentResponseAreaStatus = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentResponseArea", x => x.DepartmentResponseAreaID);
                    table.ForeignKey(
                        name: "FK_DepartmentResponseArea_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentResponseArea_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentResponseArea_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentResponseArea_CreatedUserID",
                schema: "dbo",
                table: "DepartmentResponseArea",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentResponseArea_DepartmentID",
                schema: "dbo",
                table: "DepartmentResponseArea",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentResponseArea_EditedUserID",
                schema: "dbo",
                table: "DepartmentResponseArea",
                column: "EditedUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentResponseArea",
                schema: "dbo");
        }
    }
}
