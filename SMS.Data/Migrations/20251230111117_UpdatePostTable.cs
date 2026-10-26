using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePostTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // FK
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Department",
                table: "Post");

            // Index
            migrationBuilder.DropIndex(
                name: "IX_Post_DepartmentID",
                table: "Post");

            // Column
            migrationBuilder.DropColumn(
                name: "DepartmentID",
                table: "Post");

            // Repeat for RequesterID if applicable
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Requester",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_RequesterID",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "RequesterID",
                table: "Post");
        }



        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentID",
                table: "Post",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.AddColumn<Guid>(
                name: "RequesterID",
                table: "Post",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_DepartmentID",
                table: "Post",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Post_RequesterID",
                table: "Post",
                column: "RequesterID");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Department",
                table: "Post",
                column: "DepartmentID",
                principalTable: "Department",
                principalColumn: "DepartmentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Requester",
                table: "Post",
                column: "RequesterID",
                principalTable: "Requesters",
                principalColumn: "RequesterID",
                onDelete: ReferentialAction.Restrict);
        }


    }
}
