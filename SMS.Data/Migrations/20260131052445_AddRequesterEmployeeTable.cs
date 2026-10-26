using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRequesterEmployeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requester_Employees",
                schema: "dbo",
                columns: table => new
                {
                    RequesterEmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    RequesterManagerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPreferred = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requester_Employees", x => x.RequesterEmployeeID);
                    table.ForeignKey(
                        name: "FK_Requester_Employees_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requester_Employees_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requester_Employees_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requester_Employees_RequesterManager_RequesterManagerID",
                        column: x => x.RequesterManagerID,
                        principalSchema: "dbo",
                        principalTable: "RequesterManager",
                        principalColumn: "PrivateJobManagerID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requester_Employees_CreatedUserID",
                schema: "dbo",
                table: "Requester_Employees",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Requester_Employees_EditedUserID",
                schema: "dbo",
                table: "Requester_Employees",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Requester_Employees_EmployeeID",
                schema: "dbo",
                table: "Requester_Employees",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Requester_Employees_RequesterManagerID",
                schema: "dbo",
                table: "Requester_Employees",
                column: "RequesterManagerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Requester_Employees",
                schema: "dbo");
        }
    }
}
