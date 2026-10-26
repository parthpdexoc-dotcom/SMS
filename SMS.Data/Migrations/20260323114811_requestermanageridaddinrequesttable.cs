using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class requestermanageridaddinrequesttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
    IF EXISTS (
        SELECT 1
        FROM sys.foreign_keys
        WHERE name = 'FK_Request_Request_RelatedRequestID'
    )
    BEGIN
        ALTER TABLE [dbo].[Request]
        DROP CONSTRAINT [FK_Request_Request_RelatedRequestID];
    END
    ");

            migrationBuilder.DropColumn(
                name: "IsChild",
                schema: "dbo",
                table: "Request");

            migrationBuilder.RenameColumn(
                name: "RelatedRequestID",
                schema: "dbo",
                table: "Request",
                newName: "RequesterManagerID");

            migrationBuilder.RenameIndex(
                name: "IX_Request_RelatedRequestID",
                schema: "dbo",
                table: "Request",
                newName: "IX_Request_RequesterManagerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_RequesterManager_RequesterManagerID",
                schema: "dbo",
                table: "Request",
                column: "RequesterManagerID",
                principalSchema: "dbo",
                principalTable: "RequesterManager",
                principalColumn: "RequesterManagerID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_RequesterManager_RequesterManagerID",
                schema: "dbo",
                table: "Request");

            migrationBuilder.RenameColumn(
                name: "RequesterManagerID",
                schema: "dbo",
                table: "Request",
                newName: "RelatedRequestID");

            migrationBuilder.RenameIndex(
                name: "IX_Request_RequesterManagerID",
                schema: "dbo",
                table: "Request",
                newName: "IX_Request_RelatedRequestID");

            migrationBuilder.AddColumn<bool>(
                name: "IsChild",
                schema: "dbo",
                table: "Request",
                type: "bit",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Request_RelatedRequestID",
                schema: "dbo",
                table: "Request",
                column: "RelatedRequestID",
                principalSchema: "dbo",
                principalTable: "Request",
                principalColumn: "RequestID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
