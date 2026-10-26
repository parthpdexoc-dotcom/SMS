using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestIdInPlaceHoldertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RequestID",
                schema: "dbo",
                table: "Placeholder",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Backfill from linked PostPosition → Post before adding the FK.
            migrationBuilder.Sql(@"
                UPDATE ph
                SET ph.[RequestID] = src.[RequestID]
                FROM [dbo].[Placeholder] ph
                INNER JOIN (
                    SELECT pp.[PlaceholderID], MIN(p.[RequestID]) AS [RequestID]
                    FROM [dbo].[PostPosition] pp
                    INNER JOIN [dbo].[Post] p ON p.[PostID] = pp.[PostID]
                    WHERE pp.[PlaceholderID] IS NOT NULL
                      AND p.[RequestID] IS NOT NULL
                    GROUP BY pp.[PlaceholderID]
                ) src ON src.[PlaceholderID] = ph.[PlaceholderID];
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Placeholder_RequestID",
                schema: "dbo",
                table: "Placeholder",
                column: "RequestID");

            migrationBuilder.AddForeignKey(
                name: "FK_Placeholder_Request_RequestID",
                schema: "dbo",
                table: "Placeholder",
                column: "RequestID",
                principalSchema: "dbo",
                principalTable: "Request",
                principalColumn: "RequestID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Placeholder_Request_RequestID",
                schema: "dbo",
                table: "Placeholder");

            migrationBuilder.DropIndex(
                name: "IX_Placeholder_RequestID",
                schema: "dbo",
                table: "Placeholder");

            migrationBuilder.DropColumn(
                name: "RequestID",
                schema: "dbo",
                table: "Placeholder");
        }
    }
}
