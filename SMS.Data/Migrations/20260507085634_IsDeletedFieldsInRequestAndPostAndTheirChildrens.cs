using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class IsDeletedFieldsInRequestAndPostAndTheirChildrens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "WaitingPostPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "VehicleApprovalRouting",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "VehicleApprovalRouting",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "VehicleApprovalRouting",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "VehicleApprovalRouting",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "Skill",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "Skill",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Skill",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Skill",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "Response",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "Response",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Response",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Response",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequestType",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequestType",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequestType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequestType",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequestSchedule",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequestSchedule",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequestSchedule",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequestSchedule",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequestNotes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequestNotes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequestNotes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequestNotes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequesterRestrictedList",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequesterRestrictedList",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequesterRestrictedList",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequesterRestrictedList",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequesterManager",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequesterManager",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequesterManager",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequesterManager",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequesterEmployee",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequesterEmployee",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequesterEmployee",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequesterEmployee",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequesterConfig",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequesterConfig",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequesterConfig",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequesterConfig",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequesterAuthorized",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequesterAuthorized",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequesterAuthorized",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequesterAuthorized",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "Requester",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "Requester",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Requester",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Requester",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequestEquipment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequestEquipment",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequestEquipment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequestEquipment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequestApprovalRouting",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequestApprovalRouting",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequestApprovalRouting",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequestApprovalRouting",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "Request",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "Request",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Request",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Request",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "PostPosition",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "PostPosition",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "PostPosition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "PostPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "PostNotes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "PostNotes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "PostNotes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "PostNotes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "PostEquipment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "PostEquipment",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "PostEquipment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "PostEquipment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "PostDocument",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "PostDocument",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "PostDocument",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "PostDocument",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "Post",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "Post",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Post",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Post",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "PermitApprovalRouting",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "PermitApprovalRouting",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "PermitApprovalRouting",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "PermitApprovalRouting",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "PayCode",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "PayCode",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "PayCode",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "PayCode",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "Invoices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "Invoices",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Invoices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "InvoiceDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "HolidayDay",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "HolidayDay",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "HolidayDay",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "HolidayDay",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "Files",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "Files",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Files",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Files",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "EquipmentType",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "EquipmentType",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "EquipmentType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "EquipmentType",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "Equipement",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "Equipement",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Equipement",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Equipement",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "EmployeeType",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "EmployeeType",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "EmployeeType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "EmployeeType",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "EmployeeTenure",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "EmployeeTenure",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "EmployeeTenure",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "EmployeeTenure",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "EmployeeStatus",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "EmployeeStatus",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "EmployeeStatus",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "EmployeeStatus",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "EmployeeSkill",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "EmployeeSkill",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "EmployeeSkill",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "EmployeeSkill",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "EmployeeAdmin",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "EmployeeAdmin",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "EmployeeAdmin",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "EmployeeAdmin",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "Employee",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "Employee",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Employee",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Employee",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentResponseArea",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentResponseArea",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentResponseArea",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentResponseArea",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentQuestionnaire",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentQuestionnaire",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentQuestionnaire",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentQuestionnaire",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentInvoiceConfig",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentInvoiceConfig",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentInvoiceConfig",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentInvoiceConfig",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentFee",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentFee",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentFee",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentFee",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentEquipment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentEquipment",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentEquipment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentEquipment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentDivisions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentDivisions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentDivisions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentDivisions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentCronJobManagement",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentCronJobManagement",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentCronJobManagement",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentCronJobManagement",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentAdmin",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentAdmin",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentAdmin",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentAdmin",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentActivityType",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentActivityType",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentActivityType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentActivityType",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "Department",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "Department",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Department",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Department",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "ClosedPostPositionEquipment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "ClosedPostPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "CancelledPostPosition",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "CancelledPostPosition",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "CancelledPostPosition",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "CancelledPostPosition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "BusinessType",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "BusinessType",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "BusinessType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "BusinessType",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "BlockDate",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "BlockDate",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "BlockDate",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "BlockDate",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "ApprovalDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "ApprovalDetails",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "ApprovalDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "ApprovalDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "AdminType",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "AdminType",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "AdminType",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "AdminType",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "dbo",
                table: "Admin",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "dbo",
                table: "Admin",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Admin",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "dbo",
                table: "Admin",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "WaitingPostPosition");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "VehicleApprovalRouting");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "VehicleApprovalRouting");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "VehicleApprovalRouting");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "VehicleApprovalRouting");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Skill");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "Response");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "Response");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Response");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Response");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequestType");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequestSchedule");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequestSchedule");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequestSchedule");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequestSchedule");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequestNotes");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequestNotes");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequestNotes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequestNotes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequesterRestrictedList");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequesterRestrictedList");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequesterRestrictedList");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequesterRestrictedList");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequesterManager");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequesterManager");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequesterManager");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequesterManager");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequesterEmployee");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequesterEmployee");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequesterEmployee");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequesterEmployee");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequesterConfig");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequesterConfig");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequesterConfig");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequesterConfig");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequesterAuthorized");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequesterAuthorized");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequesterAuthorized");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequesterAuthorized");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Requester");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequestEquipment");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequestEquipment");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequestEquipment");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequestEquipment");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "RequestApprovalRouting");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "RequestApprovalRouting");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "RequestApprovalRouting");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "RequestApprovalRouting");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "PostPosition");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "PostNotes");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "PostNotes");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "PostNotes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "PostNotes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "PostEquipment");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "PostEquipment");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "PostEquipment");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "PostEquipment");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "PostDocument");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "PostDocument");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "PostDocument");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "PostDocument");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "PermitApprovalRouting");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "PermitApprovalRouting");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "PermitApprovalRouting");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "PermitApprovalRouting");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "PayCode");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "PayCode");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "PayCode");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "PayCode");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "HolidayDay");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "HolidayDay");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "HolidayDay");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "HolidayDay");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "EquipmentType");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "EquipmentType");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "EquipmentType");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "EquipmentType");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "Equipement");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "Equipement");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Equipement");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Equipement");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "EmployeeType");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "EmployeeTenure");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "EmployeeTenure");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "EmployeeTenure");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "EmployeeTenure");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "EmployeeStatus");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "EmployeeStatus");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "EmployeeStatus");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "EmployeeStatus");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "EmployeeSkill");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "EmployeeSkill");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "EmployeeSkill");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "EmployeeSkill");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "EmployeeAdmin");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "EmployeeAdmin");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "EmployeeAdmin");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "EmployeeAdmin");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentResponseArea");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentResponseArea");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentResponseArea");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentResponseArea");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentQuestionnaire");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentQuestionnaire");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentQuestionnaire");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentQuestionnaire");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentInvoiceConfig");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentInvoiceConfig");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentInvoiceConfig");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentInvoiceConfig");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentFee");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentFee");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentFee");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentFee");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentEquipment");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentEquipment");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentEquipment");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentEquipment");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentDivisions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentDivisions");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentDivisions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentDivisions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentCronJobManagement");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentCronJobManagement");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentCronJobManagement");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentCronJobManagement");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentAdmin");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentAdmin");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentAdmin");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentAdmin");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "DepartmentActivityType");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "DepartmentActivityType");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "DepartmentActivityType");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "DepartmentActivityType");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "ClosedPostPositionEquipment");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "ClosedPostPositionEquipment");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "ClosedPostPositionEquipment");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "ClosedPostPositionEquipment");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "ClosedPostPosition");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "CancelledPostPosition");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "CancelledPostPosition");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "CancelledPostPosition");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "CancelledPostPosition");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "BusinessType");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "BusinessType");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "BusinessType");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "BusinessType");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "BlockDate");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "BlockDate");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "BlockDate");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "BlockDate");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "ApprovalDetails");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "ApprovalDetails");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "ApprovalDetails");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "ApprovalDetails");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "AdminType");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "AdminType");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "AdminType");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "AdminType");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "dbo",
                table: "Admin");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "dbo",
                table: "Admin");

            migrationBuilder.DropColumn(
                name: "DeletedUserID",
                schema: "dbo",
                table: "Admin");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "dbo",
                table: "Admin");
        }
    }
}
