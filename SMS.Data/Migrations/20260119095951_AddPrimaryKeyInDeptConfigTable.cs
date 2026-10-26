using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPrimaryKeyInDeptConfigTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepartmentConfiguration",
                schema: "dbo",
                columns: table => new
                {
                    DeptConfigID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    DepartmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefaultPayRate = table.Column<decimal>(type: "smallmoney", nullable: true),
                    AdminFeeRate = table.Column<decimal>(type: "smallmoney", nullable: true),
                    DefaultVehicleRate = table.Column<decimal>(type: "smallmoney", nullable: true),
                    OnDutyStartDayOfWeek = table.Column<short>(type: "smallint", nullable: true),
                    Tenure = table.Column<bool>(type: "bit", nullable: true),
                    TenureWindow = table.Column<short>(type: "smallint", nullable: true),
                    Reimburse = table.Column<bool>(type: "bit", nullable: true),
                    Invoicing = table.Column<bool>(type: "bit", nullable: true),
                    AllowEmployeeTransfers = table.Column<bool>(type: "bit", nullable: true),
                    TwentyFourHourBasedOnTimeOfDay = table.Column<bool>(type: "bit", nullable: true),
                    CanEmployeeSubmitForm = table.Column<bool>(type: "bit", nullable: true),
                    AllowableNonClosedDays = table.Column<short>(type: "smallint", nullable: true),
                    EnableActivityTypes = table.Column<bool>(type: "bit", nullable: true),
                    StandardOffset = table.Column<int>(type: "int", nullable: true),
                    PasswordChangedPeriod = table.Column<int>(type: "int", nullable: true),
                    IsDefinePost = table.Column<bool>(type: "bit", nullable: true),
                    IsSupervisorOverride = table.Column<bool>(type: "bit", nullable: true),
                    SupervisorOverrideHour = table.Column<int>(type: "int", nullable: true),
                    SendNotificationsToAllOnTransfer = table.Column<bool>(type: "bit", nullable: true),
                    ReservesAllowedHoursPerWeek = table.Column<byte>(type: "tinyint", nullable: true),
                    Skills = table.Column<bool>(type: "bit", nullable: true),
                    Equipment = table.Column<bool>(type: "bit", nullable: true),
                    CancellationSettings = table.Column<bool>(type: "bit", nullable: true),
                    OverrideSettings = table.Column<bool>(type: "bit", nullable: true),
                    NotificationSettings = table.Column<bool>(type: "bit", nullable: true),
                    CloseRequestSettings = table.Column<bool>(type: "bit", nullable: true),
                    EnableRequesterEmployeeRestrictedList = table.Column<bool>(type: "bit", nullable: true),
                    EnablePreEmploymentPaperworkRequired = table.Column<bool>(type: "bit", nullable: true),
                    EnableRequesterSpecificConfigurations = table.Column<bool>(type: "bit", nullable: true),
                    EnablePayrollConfigurations = table.Column<bool>(type: "bit", nullable: true),
                    EnablePolicyManagement = table.Column<bool>(type: "bit", nullable: true),
                    ManageSchedulePriority = table.Column<bool>(type: "bit", nullable: true),
                    ManageSchedulePriorityList = table.Column<bool>(type: "bit", nullable: true),
                    ManageSchedulePriorityReleasedList = table.Column<bool>(type: "bit", nullable: true),
                    SchedulePrioritySnapshot = table.Column<bool>(type: "bit", nullable: true),
                    DepartmentSchedulingPriority = table.Column<bool>(type: "bit", nullable: true),
                    RequesterSchedulingPriority = table.Column<bool>(type: "bit", nullable: true),
                    CoordinatedRequestSchedulingPriority = table.Column<bool>(type: "bit", nullable: true),
                    AllowEmployeeCancellation = table.Column<bool>(type: "bit", nullable: true),
                    SelfCancellationAllowedWithin = table.Column<bool>(type: "bit", nullable: true),
                    UploadPreEmploymentPaperwork = table.Column<bool>(type: "bit", nullable: true),
                    ManageCommunications = table.Column<bool>(type: "bit", nullable: true),
                    RequesterPage = table.Column<bool>(type: "bit", nullable: true),
                    SkillManagement = table.Column<bool>(type: "bit", nullable: true),
                    EmployeeVehicleReimbursement = table.Column<bool>(type: "bit", nullable: true),
                    EmployeeMileageReimbursement = table.Column<bool>(type: "bit", nullable: true),
                    EnableTenureBasedPost24HourLimit = table.Column<bool>(type: "bit", nullable: true),
                    EnableTenureBasedPost24Allowed = table.Column<bool>(type: "bit", nullable: true),
                    EnableTenureBasedPostPosition = table.Column<bool>(type: "bit", nullable: true),
                    TenureSelfCancel = table.Column<bool>(type: "bit", nullable: true),
                    EnableRetainTenureListForReassignOnCancellations = table.Column<bool>(type: "bit", nullable: true),
                    EnableSeniorityBasedonPrivateJobs = table.Column<bool>(type: "bit", nullable: true),
                    EnableRetainOpenDeptListForReassignOnCancellations = table.Column<bool>(type: "bit", nullable: true),
                    EnableOpenAgencyWaitLists = table.Column<bool>(type: "bit", nullable: true),
                    EnablePrivateJobWaitLists = table.Column<bool>(type: "bit", nullable: true),
                    EnableEmployeeSubmittingRequestsForApproval = table.Column<bool>(type: "bit", nullable: true),
                    EnableEmployeeSplittingPosts = table.Column<bool>(type: "bit", nullable: true),
                    EnableReportingHoursWorkedAsOnOrOffDuty = table.Column<bool>(type: "bit", nullable: true),
                    EnableNumberOfEmployeesBeforeSupervisorRequired = table.Column<bool>(type: "bit", nullable: true),
                    EnableAutomaticSuspensionOfEmployeeForPolicyViolations = table.Column<bool>(type: "bit", nullable: true),
                    EnableDepartmentExemptions = table.Column<bool>(type: "bit", nullable: true),
                    RequesterSpecificSkills = table.Column<bool>(type: "bit", nullable: true),
                    RequesterApprovalProcessRequired = table.Column<bool>(type: "bit", nullable: true),
                    SharedSchedulingPriorityPools = table.Column<bool>(type: "bit", nullable: true),
                    UseAgencyDefinedFields = table.Column<bool>(type: "bit", nullable: true),
                    EnableLiquorLicenseTracking = table.Column<bool>(type: "bit", nullable: true),
                    EnableAttachFilesToRequests = table.Column<bool>(type: "bit", nullable: true),
                    EnableOverlapJobConflictConfirmation = table.Column<bool>(type: "bit", nullable: true),
                    EnableSupervisorsViewingLowerEmpTypeJobs = table.Column<bool>(type: "bit", nullable: true),
                    EnableEmployeeEditStartEndTimesAfterPostBegins = table.Column<bool>(type: "bit", nullable: true),
                    EnableDepartmentEditEmployeesClosedAssignments = table.Column<bool>(type: "bit", nullable: true),
                    EnableExemptRequestsJobsFromHoursRules = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentConfiguration", x => x.DeptConfigID);
                    table.ForeignKey(
                        name: "FK_DepartmentConfiguration_Department_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "dbo",
                        principalTable: "Department",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentConfiguration_DepartmentID",
                schema: "dbo",
                table: "DepartmentConfiguration",
                column: "DepartmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentConfiguration",
                schema: "dbo");
        }
    }
}
