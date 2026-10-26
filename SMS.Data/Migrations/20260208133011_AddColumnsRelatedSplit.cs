using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsRelatedSplit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSplit",
                schema: "dbo",
                table: "PostPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentPostPositionId",
                schema: "dbo",
                table: "PostPosition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SplitPostPositionID",
                schema: "dbo",
                table: "PostPosition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostPosition_ParentPostPositionId",
                schema: "dbo",
                table: "PostPosition",
                column: "ParentPostPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_PostPosition_SplitPostPositionID",
                schema: "dbo",
                table: "PostPosition",
                column: "SplitPostPositionID");

            migrationBuilder.AddForeignKey(
                name: "FK_PostPosition_PostPosition_ParentPostPositionId",
                schema: "dbo",
                table: "PostPosition",
                column: "ParentPostPositionId",
                principalSchema: "dbo",
                principalTable: "PostPosition",
                principalColumn: "PostPositionID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostPosition_PostPosition_SplitPostPositionID",
                schema: "dbo",
                table: "PostPosition",
                column: "SplitPostPositionID",
                principalSchema: "dbo",
                principalTable: "PostPosition",
                principalColumn: "PostPositionID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostPosition_PostPosition_ParentPostPositionId",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropForeignKey(
                name: "FK_PostPosition_PostPosition_SplitPostPositionID",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropIndex(
                name: "IX_PostPosition_ParentPostPositionId",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropIndex(
                name: "IX_PostPosition_SplitPostPositionID",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropColumn(
                name: "IsSplit",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropColumn(
                name: "ParentPostPositionId",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropColumn(
                name: "SplitPostPositionID",
                schema: "dbo",
                table: "PostPosition");
        }
    }
}
