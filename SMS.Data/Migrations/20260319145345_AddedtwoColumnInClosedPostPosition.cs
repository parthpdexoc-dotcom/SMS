using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedtwoColumnInClosedPostPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentClosedPostPositionID",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClosedPostPosition_ParentClosedPostPositionID",
                schema: "dbo",
                table: "ClosedPostPosition",
                column: "ParentClosedPostPositionID");

            migrationBuilder.AddForeignKey(
                name: "FK_ClosedPostPosition_ClosedPostPosition_ParentClosedPostPositionID",
                schema: "dbo",
                table: "ClosedPostPosition",
                column: "ParentClosedPostPositionID",
                principalSchema: "dbo",
                principalTable: "ClosedPostPosition",
                principalColumn: "ClosedPostPositionID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClosedPostPosition_ClosedPostPosition_ParentClosedPostPositionID",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.DropIndex(
                name: "IX_ClosedPostPosition_ParentClosedPostPositionID",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.DropColumn(
                name: "IsEdited",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.DropColumn(
                name: "ParentClosedPostPositionID",
                schema: "dbo",
                table: "ClosedPostPosition");
        }
    }
}
