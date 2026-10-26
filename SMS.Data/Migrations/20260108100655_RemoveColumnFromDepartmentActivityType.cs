using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColumnFromDepartmentActivityType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepartmentActivityType",
                schema: "dbo",
                columns: table => new
                {
                    ActivityTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityTypeShort = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    ActivityTypeLong = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    ActivityTypeStatus = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentActivityType", x => x.ActivityTypeID);
                    table.ForeignKey(
                        name: "FK_DepartmentActivityType_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentActivityType_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentActivityType_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentActivityType_CreatedUserID",
                schema: "dbo",
                table: "DepartmentActivityType",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentActivityType_DepartmentID",
                schema: "dbo",
                table: "DepartmentActivityType",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentActivityType_EditedUserID",
                schema: "dbo",
                table: "DepartmentActivityType",
                column: "EditedUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentActivityType",
                schema: "dbo");
        }
    }
}
