using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPostCallSignIDinPostPositionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PostCallSignID",
                schema: "dbo",
                table: "PostPosition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostPosition_PostCallSignID",
                schema: "dbo",
                table: "PostPosition",
                column: "PostCallSignID");

            migrationBuilder.AddForeignKey(
                name: "FK_PostPosition_PostCallSign_PostCallSignID",
                schema: "dbo",
                table: "PostPosition",
                column: "PostCallSignID",
                principalSchema: "dbo",
                principalTable: "PostCallSign",
                principalColumn: "PostCallSignID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostPosition_PostCallSign_PostCallSignID",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropIndex(
                name: "IX_PostPosition_PostCallSignID",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropColumn(
                name: "PostCallSignID",
                schema: "dbo",
                table: "PostPosition");
        }
    }
}
