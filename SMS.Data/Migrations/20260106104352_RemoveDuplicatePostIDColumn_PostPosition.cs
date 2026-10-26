using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicatePostIDColumn_PostPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostPosition_Post_PostID1",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropIndex(
                name: "IX_PostPosition_PostID1",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropColumn(
                name: "PostID1",
                schema: "dbo",
                table: "PostPosition");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PostID1",
                schema: "dbo",
                table: "PostPosition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostPosition_PostID1",
                schema: "dbo",
                table: "PostPosition",
                column: "PostID1");

            migrationBuilder.AddForeignKey(
                name: "FK_PostPosition_Post_PostID1",
                schema: "dbo",
                table: "PostPosition",
                column: "PostID1",
                principalSchema: "dbo",
                principalTable: "Post",
                principalColumn: "PostID");
        }
    }
}
