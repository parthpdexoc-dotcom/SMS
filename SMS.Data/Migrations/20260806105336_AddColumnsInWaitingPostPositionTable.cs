using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsInWaitingPostPositionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CallSign",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EquipmentRequired",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPremiumPaidEvent",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PostCallSignID",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WaitingPostPositionPayRate",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(@"

                UPDATE w
                SET
                    w.[PostCallSignID] = src.[PostCallSignID],
                    w.[CallSign] = src.[CallSign]
                FROM [dbo].[WaitingPostPosition] w
                INNER JOIN (
                    SELECT
                        pp.[PostID],
                        pp.[EmployeeID],
                        pp.[DateOfWork],
                        pp.[PostCallSignID],
                        COALESCE(pcs.[CallSign], pp.[CallSign]) AS [CallSign],
                        ROW_NUMBER() OVER (
                            PARTITION BY pp.[PostID], pp.[EmployeeID], pp.[DateOfWork]
                            ORDER BY CASE WHEN pp.[PostCallSignID] IS NOT NULL THEN 0 ELSE 1 END,
                                     pp.[CreatedDate] DESC
                        ) AS rn
                    FROM [dbo].[PostPosition] pp
                    LEFT JOIN [dbo].[PostCallSign] pcs ON pcs.[PostCallSignID] = pp.[PostCallSignID]
                    WHERE pp.[PostCallSignID] IS NOT NULL
                       OR NULLIF(LTRIM(RTRIM(pp.[CallSign])), N'') IS NOT NULL
                ) src
                    ON src.[PostID] = w.[PostID]
                   AND src.[EmployeeID] = w.[EmployeeID]
                   AND src.[DateOfWork] = w.[DateOfWork]
                   AND src.rn = 1;

            ");

            migrationBuilder.AlterColumn<decimal>(
                name: "PostPositionPayRate",
                schema: "dbo",
                table: "PostPosition",
                type: "money",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaitingPostPosition_PostCallSignID",
                schema: "dbo",
                table: "WaitingPostPosition",
                column: "PostCallSignID");

            migrationBuilder.AddForeignKey(
                name: "FK_WaitingPostPosition_PostCallSign_PostCallSignID",
                schema: "dbo",
                table: "WaitingPostPosition",
                column: "PostCallSignID",
                principalSchema: "dbo",
                principalTable: "PostCallSign",
                principalColumn: "PostCallSignID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaitingPostPosition_PostCallSign_PostCallSignID",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropIndex(
                name: "IX_WaitingPostPosition_PostCallSignID",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "CallSign",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "EquipmentRequired",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "IsPremiumPaidEvent",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "PostCallSignID",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "WaitingPostPositionPayRate",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.AlterColumn<decimal>(
                name: "PostPositionPayRate",
                schema: "dbo",
                table: "PostPosition",
                type: "money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "money");
        }
    }
}
