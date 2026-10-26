using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoutingGroupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoutingGroup",
                schema: "dbo",
                columns: table => new
                {
                    ApproverID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmpIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApproverLong = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApproverShort = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApproverEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutingGroup", x => x.ApproverID);
                    table.ForeignKey(
                        name: "FK_RoutingGroup_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoutingGroup_DepartmentID",
                schema: "dbo",
                table: "RoutingGroup",
                column: "DepartmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoutingGroup",
                schema: "dbo");
        }
    }
}
