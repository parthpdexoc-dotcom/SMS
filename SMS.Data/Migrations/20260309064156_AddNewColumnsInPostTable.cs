using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnsInPostTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostMinimumHoursRequired",
                schema: "dbo",
                table: "Post",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "PostResponseAreaId",
                schema: "dbo",
                table: "Post",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PostUniform",
                schema: "dbo",
                table: "Post",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RequiredRank",
                schema: "dbo",
                table: "Post",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Post_PostResponseAreaId",
                schema: "dbo",
                table: "Post",
                column: "PostResponseAreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_DepartmentResponseArea_PostResponseAreaId",
                schema: "dbo",
                table: "Post",
                column: "PostResponseAreaId",
                principalSchema: "dbo",
                principalTable: "DepartmentResponseArea",
                principalColumn: "DepartmentResponseAreaID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_DepartmentResponseArea_PostResponseAreaId",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_PostResponseAreaId",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PostMinimumHoursRequired",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PostResponseAreaId",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PostUniform",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "RequiredRank",
                schema: "dbo",
                table: "Post");
        }
    }
}
