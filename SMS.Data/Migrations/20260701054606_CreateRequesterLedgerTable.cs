using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateRequesterLedgerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequesterLedgers",
                schema: "dbo",
                columns: table => new
                {
                    RequesterLedgerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequesterID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TransactionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SourceType = table.Column<int>(type: "int", nullable: false),
                    SourceID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequesterLedgers", x => x.RequesterLedgerID);
                    table.ForeignKey(
                        name: "FK_RequesterLedgers_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequesterLedgers_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                    table.ForeignKey(
                        name: "FK_RequesterLedgers_Employee_DeletedUserID",
                        column: x => x.DeletedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                    table.ForeignKey(
                        name: "FK_RequesterLedgers_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                    table.ForeignKey(
                        name: "FK_RequesterLedgers_Requester_RequesterID",
                        column: x => x.RequesterID,
                        principalSchema: "dbo",
                        principalTable: "Requester",
                        principalColumn: "RequesterID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequesterLedgers_CreatedUserID",
                schema: "dbo",
                table: "RequesterLedgers",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequesterLedgers_DeletedUserID",
                schema: "dbo",
                table: "RequesterLedgers",
                column: "DeletedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequesterLedgers_DepartmentID",
                schema: "dbo",
                table: "RequesterLedgers",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_RequesterLedgers_EditedUserID",
                schema: "dbo",
                table: "RequesterLedgers",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequesterLedgers_RequesterID",
                schema: "dbo",
                table: "RequesterLedgers",
                column: "RequesterID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequesterLedgers",
                schema: "dbo");
        }
    }
}
