using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class addpteocodeidforeignkeyinpostfrompteocode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
