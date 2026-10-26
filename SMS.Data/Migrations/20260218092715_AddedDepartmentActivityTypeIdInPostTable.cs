using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedDepartmentActivityTypeIdInPostTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActivityTypeID",
                schema: "dbo",
                table: "DepartmentActivityType",
                newName: "DepartmentActivityTypeId");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentActivityTypeId",
                schema: "dbo",
                table: "Post",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_DepartmentActivityTypeId",
                schema: "dbo",
                table: "Post",
                column: "DepartmentActivityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_DepartmentActivityType_DepartmentActivityTypeId",
                schema: "dbo",
                table: "Post",
                column: "DepartmentActivityTypeId",
                principalSchema: "dbo",
                principalTable: "DepartmentActivityType",
                principalColumn: "DepartmentActivityTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_DepartmentActivityType_DepartmentActivityTypeId",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_DepartmentActivityTypeId",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "DepartmentActivityTypeId",
                schema: "dbo",
                table: "Post");

            migrationBuilder.RenameColumn(
                name: "DepartmentActivityTypeId",
                schema: "dbo",
                table: "DepartmentActivityType",
                newName: "ActivityTypeID");
        }
    }
}
