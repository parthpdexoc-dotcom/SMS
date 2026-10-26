using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class adddepartmentdefinedfieldtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepartmentDefinedFields",
                schema: "dbo",
                columns: table => new
                {
                    DepartmentDefinedFieldID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestDepartmentDefinedFieldInterfaceLabel1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestDepartmentDefinedFieldInterfaceLabel2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestDepartmentDefinedFieldInterfaceLabel3 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestDepartmentDefinedFieldInterfaceLabel4 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestDepartmentDefinedFieldInterfaceLabel5 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestDepartmentDefinedFieldInterfaceLabel6 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestDepartmentDefinedFieldInterfaceLabel7 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestDepartmentDefinedFieldInterfaceLabel8 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestDepartmentDefinedFieldInterfaceLabel9 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestDepartmentDefinedFieldInterfaceLabel10 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostDepartmentDefinedFieldInterfaceLabel1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostDepartmentDefinedFieldInterfaceLabel2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostDepartmentDefinedFieldInterfaceLabel3 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostDepartmentDefinedFieldInterfaceLabel4 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostDepartmentDefinedFieldInterfaceLabel5 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostDepartmentDefinedFieldInterfaceLabel6 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostDepartmentDefinedFieldInterfaceLabel7 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostDepartmentDefinedFieldInterfaceLabel8 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostDepartmentDefinedFieldInterfaceLabel9 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostDepartmentDefinedFieldInterfaceLabel10 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentDefinedFields", x => x.DepartmentDefinedFieldID);
                    table.ForeignKey(
                        name: "FK_DepartmentDefinedFields_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentDefinedFields_DepartmentID",
                schema: "dbo",
                table: "DepartmentDefinedFields",
                column: "DepartmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentDefinedFields",
                schema: "dbo");
        }
    }
}
