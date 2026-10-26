using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameEmpIdsToEmployeeIDs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmpIds",
                schema: "dbo",
                table: "RoutingGroup",
                newName: "EmployeeIDs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
               name: "EmployeeIDs",
               schema: "dbo",
               table: "RoutingGroup",
               newName: "EmpIds");
        }
    }
}
