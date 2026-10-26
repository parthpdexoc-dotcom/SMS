using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRequesterSubTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequesterManager",
                schema: "dbo",
                columns: table => new
                {
                    PrivateJobManagerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    RequesterID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequesterManager", x => x.PrivateJobManagerID);
                    table.ForeignKey(
                        name: "FK_RequesterManager_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequesterManager_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequesterManager_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequesterManager_Requesters_RequesterID",
                        column: x => x.RequesterID,
                        principalSchema: "dbo",
                        principalTable: "Requesters",
                        principalColumn: "RequesterID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequesterRestrictedList",
                schema: "dbo",
                columns: table => new
                {
                    RequesterRestrictID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    RequesterID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EffectiveStartDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: true),
                    EffectiveEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequesterRestrictedList", x => x.RequesterRestrictID);
                    table.ForeignKey(
                        name: "FK_RequesterRestrictedList_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequesterRestrictedList_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequesterRestrictedList_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequesterRestrictedList_Requesters_RequesterID",
                        column: x => x.RequesterID,
                        principalSchema: "dbo",
                        principalTable: "Requesters",
                        principalColumn: "RequesterID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequesterManager_CreatedUserID",
                schema: "dbo",
                table: "RequesterManager",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequesterManager_EditedUserID",
                schema: "dbo",
                table: "RequesterManager",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequesterManager_EmployeeID",
                schema: "dbo",
                table: "RequesterManager",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_RequesterManager_RequesterID",
                schema: "dbo",
                table: "RequesterManager",
                column: "RequesterID");

            migrationBuilder.CreateIndex(
                name: "IX_RequesterRestrictedList_CreatedUserID",
                schema: "dbo",
                table: "RequesterRestrictedList",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequesterRestrictedList_EditedUserID",
                schema: "dbo",
                table: "RequesterRestrictedList",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequesterRestrictedList_EmployeeID",
                schema: "dbo",
                table: "RequesterRestrictedList",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_RequesterRestrictedList_RequesterID",
                schema: "dbo",
                table: "RequesterRestrictedList",
                column: "RequesterID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequesterManager",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RequesterRestrictedList",
                schema: "dbo");
        }
    }
}
