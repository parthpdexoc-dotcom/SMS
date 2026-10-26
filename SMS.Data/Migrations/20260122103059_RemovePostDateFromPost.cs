using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovePostDateFromPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostDate",
                schema: "dbo",
                table: "Post");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 🔹 ADD BACK PostDate COLUMN (rollback support)
            migrationBuilder.AddColumn<DateOnly>(
                name: "PostDate",
                schema: "dbo",
                table: "Post",
                type: "date",
                nullable: true);
        }
    }
}
