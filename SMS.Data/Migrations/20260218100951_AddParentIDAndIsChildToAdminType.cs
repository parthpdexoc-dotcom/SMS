using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddParentIDAndIsChildToAdminType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsChild",
                schema: "dbo",
                table: "AdminType",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentID",
                schema: "dbo",
                table: "AdminType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminType_ParentID",
                schema: "dbo",
                table: "AdminType",
                column: "ParentID");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminType_AdminType_ParentID",
                schema: "dbo",
                table: "AdminType",
                column: "ParentID",
                principalSchema: "dbo",
                principalTable: "AdminType",
                principalColumn: "AdminTypeID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminType_AdminType_ParentID",
                schema: "dbo",
                table: "AdminType");

            migrationBuilder.DropIndex(
                name: "IX_AdminType_ParentID",
                schema: "dbo",
                table: "AdminType");

            migrationBuilder.DropColumn(
                name: "IsChild",
                schema: "dbo",
                table: "AdminType");

            migrationBuilder.DropColumn(
                name: "ParentID",
                schema: "dbo",
                table: "AdminType");
        }
    }
}
