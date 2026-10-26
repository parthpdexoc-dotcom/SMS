using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDepartmentPolicyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DepartmentPolicy_DepartmentID",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentConfiguration_DepartmentID",
                schema: "dbo",
                table: "DepartmentConfiguration");

            migrationBuilder.DropColumn(
                name: "Managed24HourQuota",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "Position24HourQuota",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "PositionExempt24HourQuota",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "PostToSupervisor",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "Previewable",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "SupervisorsCanViewLowerEmpTypeJobsCount",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "Tenure24HourLimit",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.RenameColumn(
                name: "UsesTenureJobsForWeekRule",
                schema: "dbo",
                table: "DepartmentPolicy",
                newName: "PostToSupervise");

            migrationBuilder.RenameColumn(
                name: "Tenure24HourLimitPosts",
                schema: "dbo",
                table: "DepartmentPolicy",
                newName: "ManagedPostTakenOnSameday");

            migrationBuilder.AlterColumn<bool>(
                name: "SupervisorsCanViewLowerEmpTypeJobs",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "PostsTakenPerDay",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "int",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 3);

            migrationBuilder.AlterColumn<bool>(
                name: "OverlapJobConflictConfirmation",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "MinHoursPaid",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "int",
                nullable: false,
                defaultValue: 4,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeptOpenPostsTakenPerDay",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeptOpenPostsTakenPerWeek",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHoursPerDayWorked",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHoursPerMonthWorked",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHoursPerWeekWorked",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsManagedPostTakenOnSameday",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMinHoursPaid",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPositionTakenOnSameday",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPostExemptTakenOnSameday",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPostsTakenPerDay",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPreviewable",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTenurePostsAllowedPerDay",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTenurePostsAllowedPerWeek",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "PositionTakenOnSameday",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "smallint",
                nullable: false,
                defaultValue: (short)1);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentPolicy_DepartmentID",
                schema: "dbo",
                table: "DepartmentPolicy",
                column: "DepartmentID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentConfiguration_DepartmentID",
                schema: "dbo",
                table: "DepartmentConfiguration",
                column: "DepartmentID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DepartmentPolicy_DepartmentID",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentConfiguration_DepartmentID",
                schema: "dbo",
                table: "DepartmentConfiguration");

            migrationBuilder.DropColumn(
                name: "IsDeptOpenPostsTakenPerDay",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "IsDeptOpenPostsTakenPerWeek",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "IsHoursPerDayWorked",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "IsHoursPerMonthWorked",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "IsHoursPerWeekWorked",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "IsManagedPostTakenOnSameday",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "IsMinHoursPaid",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "IsPositionTakenOnSameday",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "IsPostExemptTakenOnSameday",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "IsPostsTakenPerDay",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "IsPreviewable",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "IsTenurePostsAllowedPerDay",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "IsTenurePostsAllowedPerWeek",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.DropColumn(
                name: "PositionTakenOnSameday",
                schema: "dbo",
                table: "DepartmentPolicy");

            migrationBuilder.RenameColumn(
                name: "PostToSupervise",
                schema: "dbo",
                table: "DepartmentPolicy",
                newName: "UsesTenureJobsForWeekRule");

            migrationBuilder.RenameColumn(
                name: "ManagedPostTakenOnSameday",
                schema: "dbo",
                table: "DepartmentPolicy",
                newName: "Tenure24HourLimitPosts");

            migrationBuilder.AlterColumn<bool>(
                name: "SupervisorsCanViewLowerEmpTypeJobs",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "PostsTakenPerDay",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "int",
                nullable: false,
                defaultValue: 3,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 2);

            migrationBuilder.AlterColumn<bool>(
                name: "OverlapJobConflictConfirmation",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "MinHoursPaid",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 4);

            migrationBuilder.AddColumn<short>(
                name: "Managed24HourQuota",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "Position24HourQuota",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "PositionExempt24HourQuota",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PostToSupervisor",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "Previewable",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupervisorsCanViewLowerEmpTypeJobsCount",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "Tenure24HourLimit",
                schema: "dbo",
                table: "DepartmentPolicy",
                type: "smallint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentPolicy_DepartmentID",
                schema: "dbo",
                table: "DepartmentPolicy",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentConfiguration_DepartmentID",
                schema: "dbo",
                table: "DepartmentConfiguration",
                column: "DepartmentID");
        }
    }
}
