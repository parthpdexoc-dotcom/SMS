using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPteoCodeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PTEO_Code",
                schema: "dbo",
                columns: table => new
                {
                    PteoCodeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    ProjectNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TaskNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AwardNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FundNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OrganizationName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PTEO_Code", x => x.PteoCodeID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PTEO_Code",
                schema: "dbo");
        }
    }
}
