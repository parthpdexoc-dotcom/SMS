using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixRecurrenceSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestRecurrence",
                schema: "dbo",
                columns: table => new
                {
                    RequestRecurrenceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecurrenceUnit = table.Column<int>(type: "int", nullable: false),
                    Interval = table.Column<int>(type: "int", nullable: false),
                    WeekDays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonthDays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonthlyMode = table.Column<int>(type: "int", nullable: true),
                    WeekIndex = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestRecurrence", x => x.RequestRecurrenceID);
                    table.ForeignKey(
                        name: "FK_RequestRecurrence_Request_RequestID",
                        column: x => x.RequestID,
                        principalSchema: "dbo",
                        principalTable: "Request",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestRecurrence_RequestID",
                schema: "dbo",
                table: "RequestRecurrence",
                column: "RequestID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestRecurrence",
                schema: "dbo");
        }
    }
}
