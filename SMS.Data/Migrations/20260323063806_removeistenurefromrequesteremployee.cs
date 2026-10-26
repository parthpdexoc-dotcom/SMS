using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class removeistenurefromrequesteremployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTenure",
                schema: "dbo",
                table: "RequesterEmployee");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTenure",
                schema: "dbo",
                table: "RequesterEmployee",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
