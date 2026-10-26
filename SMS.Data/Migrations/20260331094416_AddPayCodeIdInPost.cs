using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPayCodeIdInPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayCode",
                schema: "dbo",
                table: "Post");

            migrationBuilder.AddColumn<Guid>(
                name: "PayCodeID",
                schema: "dbo",
                table: "Post",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "PayCodeStatus",
                schema: "dbo",
                table: "PayCode",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_PayCodeID",
                schema: "dbo",
                table: "Post",
                column: "PayCodeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_PayCode_PayCodeID",
                schema: "dbo",
                table: "Post",
                column: "PayCodeID",
                principalSchema: "dbo",
                principalTable: "PayCode",
                principalColumn: "PayCodeID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_PayCode_PayCodeID",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_PayCodeID",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PayCodeID",
                schema: "dbo",
                table: "Post");

            migrationBuilder.AddColumn<string>(
                name: "PayCode",
                schema: "dbo",
                table: "Post",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "PayCodeStatus",
                schema: "dbo",
                table: "PayCode",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
