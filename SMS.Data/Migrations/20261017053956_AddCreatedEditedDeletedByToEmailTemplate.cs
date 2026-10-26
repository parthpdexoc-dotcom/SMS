using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedEditedDeletedByToEmailTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "dbo",
                table: "EmailTemplate",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "EmailTemplate",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUserID",
                schema: "dbo",
                table: "EmailTemplate",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "EmailTemplate",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "EmailTemplate",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "EmailTemplate",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditedBy",
                schema: "dbo",
                table: "EmailTemplate",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedDate",
                schema: "dbo",
                table: "EmailTemplate",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EditedUserID",
                schema: "dbo",
                table: "EmailTemplate",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "EmailTemplate",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "dbo",
                table: "EmailTemplate");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "dbo",
                table: "EmailTemplate");

            migrationBuilder.DropColumn(
                name: "CreatedUserID",
                schema: "dbo",
                table: "EmailTemplate");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "EmailTemplate");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "EmailTemplate");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "EmailTemplate");

            migrationBuilder.DropColumn(
                name: "EditedBy",
                schema: "dbo",
                table: "EmailTemplate");

            migrationBuilder.DropColumn(
                name: "EditedDate",
                schema: "dbo",
                table: "EmailTemplate");

            migrationBuilder.DropColumn(
                name: "EditedUserID",
                schema: "dbo",
                table: "EmailTemplate");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "EmailTemplate");
        }
    }
}
