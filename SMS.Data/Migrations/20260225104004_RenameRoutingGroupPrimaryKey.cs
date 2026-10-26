using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameRoutingGroupPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApproverID",
                schema: "dbo",
                table: "RoutingGroup",
                newName: "RoutingGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoutingGroupId",
                schema: "dbo",
                table: "RoutingGroup",
                newName: "ApproverID");
        }
    }
}
