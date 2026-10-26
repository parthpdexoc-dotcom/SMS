using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRequesterConfigTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequesterConfig",
                schema: "dbo",
                columns: table => new
                {
                    RequesterConfigID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    RequesterID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnableSkills = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DefinedSkills = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    EnableRequesterManager = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DefineRequesterManager = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    EnablePreEmploymentPaperwork = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EnableActivityReporting = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EnableRestrictedEmployeeList = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EnableParentChildInvoicing = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EnableExemptFromInvoicing = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EnableExemptFromPayroll = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EnableExemptFromHoursRules = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequesterConfig", x => x.RequesterConfigID);
                    table.ForeignKey(
                        name: "FK_RequesterConfig_Employee_CreatedUserID",
                        column: x => x.CreatedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequesterConfig_Employee_EditedUserID",
                        column: x => x.EditedUserID,
                        principalSchema: "dbo",
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequesterConfig_Requester_RequesterID",
                        column: x => x.RequesterID,
                        principalSchema: "dbo",
                        principalTable: "Requester",
                        principalColumn: "RequesterID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequesterConfig_CreatedUserID",
                schema: "dbo",
                table: "RequesterConfig",
                column: "CreatedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequesterConfig_EditedUserID",
                schema: "dbo",
                table: "RequesterConfig",
                column: "EditedUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RequesterConfig_RequesterID",
                schema: "dbo",
                table: "RequesterConfig",
                column: "RequesterID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequesterConfig",
                schema: "dbo");
        }
    }
}
