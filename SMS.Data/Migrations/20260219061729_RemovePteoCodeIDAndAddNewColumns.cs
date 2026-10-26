using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovePteoCodeIDAndAddNewColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_PteoCode_PteoCodeID",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_PteoCodeID",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PteoCodeID",
                schema: "dbo",
                table: "Post");

            migrationBuilder.AddColumn<string>(
                name: "AwardNumber",
                schema: "dbo",
                table: "Post",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FundNumber",
                schema: "dbo",
                table: "Post",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationName",
                schema: "dbo",
                table: "Post",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectNumber",
                schema: "dbo",
                table: "Post",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaskNumber",
                schema: "dbo",
                table: "Post",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwardNumber",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "FundNumber",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "OrganizationName",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "ProjectNumber",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "TaskNumber",
                schema: "dbo",
                table: "Post");

            migrationBuilder.AddColumn<Guid>(
                name: "PteoCodeID",
                schema: "dbo",
                table: "Post",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_PteoCodeID",
                schema: "dbo",
                table: "Post",
                column: "PteoCodeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_PteoCode_PteoCodeID",
                schema: "dbo",
                table: "Post",
                column: "PteoCodeID",
                principalSchema: "dbo",
                principalTable: "PteoCode",
                principalColumn: "PteoCodeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
