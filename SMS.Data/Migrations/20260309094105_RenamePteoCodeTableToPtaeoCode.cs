using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenamePteoCodeTableToPtaeoCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PteoCode",
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "PtaeoCode",
                schema: "dbo",
                columns: table => new
                {
                    PtaeoCodeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    ProjectNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TaskNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AwardNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FundNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OrganizationName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OrganizationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PtaeoCode", x => x.PtaeoCodeID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PtaeoCode",
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "PteoCode",
                schema: "dbo",
                columns: table => new
                {
                    PteoCodeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    AwardNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FundNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OrganizationName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OrganizationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProjectNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TaskNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PteoCode", x => x.PteoCodeID);
                });
        }
    }
}
