using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialDataEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Execute all data seeding in a single SQL block to handle variables properly
            // Check if data already exists to make migration idempotent
            migrationBuilder.Sql(@"
                -- Check if initial data already exists
                IF NOT EXISTS (SELECT 1 FROM [dbo].[AdminType] WHERE [AdminTypeShort] = N'SuperUser')
                BEGIN
                    -- Declare variables for GUIDs
                    DECLARE @superUserAdminTypeId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @customerSupportAdminTypeId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @departmentAdminAdminTypeId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @departmentAdminStaffAdminTypeId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @requesterSchedulerAdminTypeId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @departmentHRStaffAdminTypeId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @departmentFiscalStaffAdminTypeId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @departmentAuditorAdminTypeId UNIQUEIDENTIFIER = NEWID();
                    
                    DECLARE @superUserAdminId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @departmentId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @activeEmployeeStatusId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @inactiveEmployeeStatusId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @firstEmployeeId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @chiefOfPoliceEmployeeTypeId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @employeeAdminId UNIQUEIDENTIFIER = NEWID();
                    
                    DECLARE @deputyChiefEmployeeTypeId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @captainEmployeeTypeId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @lieutenantEmployeeTypeId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @sergeantEmployeeTypeId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @corporalEmployeeTypeId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @detectiveEmployeeTypeId UNIQUEIDENTIFIER = NEWID();
                    DECLARE @policeOfficerEmployeeTypeId UNIQUEIDENTIFIER = NEWID();
                    
                    DECLARE @currentDate DATETIME = GETUTCDATE();

                    -- Step 1: Insert AdminType records (with NULL CreatedUserID/EditedUserID initially)
                    INSERT INTO [dbo].[AdminType] ([AdminTypeID], [AdminTypeShort], [AdminTypeLong], [Status], [CreatedDate], [CreatedUserID], [CreatedBy], [EditedDate], [EditedUserID], [EditedBy])
                    VALUES
                        (@superUserAdminTypeId, N'SuperUser', N'Stone Software Administrator', 1, @currentDate, NULL, NULL, @currentDate, NULL, NULL),
                        (@customerSupportAdminTypeId, N'CustomerSupport', N'Customer Support', 1, @currentDate, NULL, NULL, @currentDate, NULL, NULL),
                        (@departmentAdminAdminTypeId, N'DepartmentAdmin', N'Depatment Administrator, can manage everything within Department', 1, @currentDate, NULL, NULL, @currentDate, NULL, NULL),
                        (@departmentAdminStaffAdminTypeId, N'DepartmentAdminStaff', N'Department Administrative Staff responsible for day-to-day Scheduling', 1, @currentDate, NULL, NULL, @currentDate, NULL, NULL),
                        (@requesterSchedulerAdminTypeId, N'RequesterScheduler', N'Employee assigned to manage schedule of specific Requester.', 1, @currentDate, NULL, NULL, @currentDate, NULL, NULL),
                        (@departmentHRStaffAdminTypeId, N'DepartmentHRStaff', N'Employee assigned to manage employee profile related information', 1, @currentDate, NULL, NULL, @currentDate, NULL, NULL),
                        (@departmentFiscalStaffAdminTypeId, N'DepartmentFiscalStaff', N'Employee assigned to manage invoice related tasks, run reports, view Requests, Manage/Update invoice related cords', 1, @currentDate, NULL, NULL, @currentDate, NULL, NULL),
                        (@departmentAuditorAdminTypeId, N'DepartmentAuditor', N'Employee assigned view only access to all records within system. Past, Present, and future for auditing purposes', 1, @currentDate, NULL, NULL, @currentDate, NULL, NULL);

                    -- Step 2: Create SuperUser Admin (self-referencing CreatedUserID/EditedUserID)
                    INSERT INTO [dbo].[Admin] ([AdminID], [AdminTypeID], [FirstName], [LastName], [UserName], [Password], [Status], [CreatedDate], [CreatedUserID], [CreatedBy], [EditedDate], [EditedUserID], [EditedBy])
                    VALUES
                        (@superUserAdminId, @superUserAdminTypeId, N'Super', N'User', N'superuser', N'SuperUser@123', 1, @currentDate, @superUserAdminId, N'Super User', @currentDate, @superUserAdminId, N'Super User');

                    -- Step 3: Update AdminType records to set CreatedUserID and EditedUserID to SuperUser
                    UPDATE [dbo].[AdminType]
                    SET [CreatedUserID] = @superUserAdminId,
                        [CreatedBy] = N'Super User',
                        [EditedUserID] = @superUserAdminId,
                        [EditedBy] = N'Super User'
                    WHERE [AdminTypeID] IN (
                        @superUserAdminTypeId,
                        @customerSupportAdminTypeId,
                        @departmentAdminAdminTypeId,
                        @departmentAdminStaffAdminTypeId,
                        @requesterSchedulerAdminTypeId,
                        @departmentHRStaffAdminTypeId,
                        @departmentFiscalStaffAdminTypeId,
                        @departmentAuditorAdminTypeId
                    );

                    -- Step 4: Create Department by SuperUser
                    INSERT INTO [dbo].[Department] ([DepartmentID], [DepartmentCode], [DepartmentName], [Address], [City], [State], [Zip], [FirstName], [LastName], [Phone], [Fax], [Email], [Domain], [Active], [CreatedDate], [CreatedUserID], [CreatedBy], [EditedDate], [EditedUserID], [EditedBy])
                    VALUES
                        (@departmentId, N'DEPT001', N'First Department', N'123 Main St', N'City', N'State', N'12345', N'Admin', N'Contact', N'555-0100', N'555-0101', N'admin@department.com', N'department.com', 1, @currentDate, @superUserAdminId, N'Super User', @currentDate, @superUserAdminId, N'Super User');

                    -- Step 5: Create EmployeeStatus (Active and Inactive) for the Department
                    INSERT INTO [dbo].[EmployeeStatus] ([EmployeeStatusID], [DepartmentID], [StatusShort], [StatusLong], [Status], [CreatedDate], [CreatedUserID], [CreatedBy], [EditedDate], [EditedUserID], [EditedBy])
                    VALUES
                        (@activeEmployeeStatusId, @departmentId, N'Active', N'Active Employee Status', 1, @currentDate, NULL, NULL, @currentDate, NULL, NULL),
                        (@inactiveEmployeeStatusId, @departmentId, N'Inactive', N'Inactive Employee Status', 1, @currentDate, NULL, NULL, @currentDate, NULL, NULL);

                    -- Step 6: Create EmployeeType (Chief of Police) for the Department
                    INSERT INTO [dbo].[EmployeesTypes] ([EmployeeTypeID], [DepartmentID], [EmployeeTypeShort], [EmployeeTypeLong], [EmployeeTypeActive], [RecordAddedDateTimeStamp], [AddedByID], [NameOfUserAddingRecord], [UserEditingID], [UserNameEditingRecord], [EditDate])
                    VALUES
                        (@chiefOfPoliceEmployeeTypeId, @departmentId, N'Chief of Police', N'The highest-ranking officer; often appointed by a mayor or city council.', 1, @currentDate, NULL, NULL, NULL, NULL, NULL);

                    -- Step 7: Create first Employee (DepartmentAdmin) with circular CreatedUserID/EditedUserID
                    INSERT INTO [dbo].[Employee] ([EmployeeID], [DepartmentID], [EmployeeTypeID], [BadgeNumber], [StatusID], [FirstName], [LastName], [Username], [Password], [PrimaryPhone], [SecondaryPhone], [PrimaryEmail], [SecondaryEmail], [CreatedDate], [CreatedUserID], [CreatedBy], [EditedDate], [EditedUserID], [EditedBy])
                    VALUES
                        (@firstEmployeeId, @departmentId, @chiefOfPoliceEmployeeTypeId, N'EMP001', @activeEmployeeStatusId, N'Department', N'Admin', N'deptadmin', N'DeptAdmin@123', N'555-0200', NULL, N'deptadmin@department.com', NULL, @currentDate, @firstEmployeeId, N'Department Admin', @currentDate, @firstEmployeeId, N'Department Admin');

                    -- Step 8: Update EmployeeType to set AddedByID and UserEditingID to first employee
                    UPDATE [dbo].[EmployeesTypes]
                    SET [AddedByID] = @firstEmployeeId,
                        [NameOfUserAddingRecord] = N'Department Admin',
                        [UserEditingID] = @firstEmployeeId,
                        [UserNameEditingRecord] = N'Department Admin',
                        [EditDate] = @currentDate
                    WHERE [EmployeeTypeID] = @chiefOfPoliceEmployeeTypeId;

                    -- Step 9: Create EmployeeAdmin record linking Employee to DepartmentAdmin AdminType
                    INSERT INTO [dbo].[EmployeeAdmin] ([EmployeeAdminID], [AdminTypeID], [EmployeeID], [CreatedDate], [CreatedUserID], [CreatedBy], [EditedDate], [EditedUserID], [EditedBy])
                    VALUES
                        (@employeeAdminId, @departmentAdminAdminTypeId, @firstEmployeeId, @currentDate, @firstEmployeeId, N'Department Admin', @currentDate, @firstEmployeeId, N'Department Admin');

                    -- Step 10: Update EmployeeStatus to set CreatedUserID/EditedUserID to first Employee
                    UPDATE [dbo].[EmployeeStatus]
                    SET [CreatedUserID] = @firstEmployeeId,
                        [CreatedBy] = N'Department Admin',
                        [EditedUserID] = @firstEmployeeId,
                        [EditedBy] = N'Department Admin'
                    WHERE [EmployeeStatusID] IN (@activeEmployeeStatusId, @inactiveEmployeeStatusId);

                    -- Step 11: Insert remaining EmployeeTypes (for future use)
                    INSERT INTO [dbo].[EmployeesTypes] ([EmployeeTypeID], [DepartmentID], [EmployeeTypeShort], [EmployeeTypeLong], [EmployeeTypeActive], [RecordAddedDateTimeStamp], [AddedByID], [NameOfUserAddingRecord], [UserEditingID], [UserNameEditingRecord], [EditDate])
                    VALUES
                        (@deputyChiefEmployeeTypeId, @departmentId, N'Deputy/Assistant Chief', N'High-level administrators who oversee major bureaus or operations.', 1, @currentDate, @firstEmployeeId, N'Department Admin', @firstEmployeeId, N'Department Admin', @currentDate),
                        (@captainEmployeeTypeId, @departmentId, N'Captain', N'Usually commands a specific precinct or a specialized division (e.g., Narcotics).', 1, @currentDate, @firstEmployeeId, N'Department Admin', @firstEmployeeId, N'Department Admin', @currentDate),
                        (@lieutenantEmployeeTypeId, @departmentId, N'Lieutenant', N'Mid-level managers who supervise several squads and handle administrative duties.', 1, @currentDate, @firstEmployeeId, N'Department Admin', @firstEmployeeId, N'Department Admin', @currentDate),
                        (@sergeantEmployeeTypeId, @departmentId, N'Sergeant', N'First-line supervisors who manage a squad of officers on the street.', 1, @currentDate, @firstEmployeeId, N'Department Admin', @firstEmployeeId, N'Department Admin', @currentDate),
                        (@corporalEmployeeTypeId, @departmentId, N'Corporal', N'A senior officer rank, often serving as a field supervisor or trainer.', 1, @currentDate, @firstEmployeeId, N'Department Admin', @firstEmployeeId, N'Department Admin', @currentDate),
                        (@detectiveEmployeeTypeId, @departmentId, N'Detective', N'An officer focused on investigations rather than patrol; rank may be equal to or higher than an officer.', 1, @currentDate, @firstEmployeeId, N'Department Admin', @firstEmployeeId, N'Department Admin', @currentDate),
                        (@policeOfficerEmployeeTypeId, @departmentId, N'Police Officer', N'The standard entry-level rank responsible for patrol and responding to calls.', 1, @currentDate, @firstEmployeeId, N'Department Admin', @firstEmployeeId, N'Department Admin', @currentDate);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Note: This will delete all the seed data
            // In a production scenario, you might want to be more selective
            
            migrationBuilder.Sql(@"
                DELETE FROM [dbo].[EmployeeAdmin] WHERE [EmployeeID] IN (SELECT [EmployeeID] FROM [dbo].[Employee] WHERE [Username] = N'deptadmin');
                DELETE FROM [dbo].[EmployeesTypes] WHERE [DepartmentID] IN (SELECT [DepartmentID] FROM [dbo].[Department] WHERE [DepartmentCode] = N'DEPT001');
                DELETE FROM [dbo].[Employee] WHERE [Username] = N'deptadmin';
                DELETE FROM [dbo].[EmployeeStatus] WHERE [DepartmentID] IN (SELECT [DepartmentID] FROM [dbo].[Department] WHERE [DepartmentCode] = N'DEPT001');
                DELETE FROM [dbo].[Department] WHERE [DepartmentCode] = N'DEPT001';
                DELETE FROM [dbo].[Admin] WHERE [UserName] = N'superuser';
                DELETE FROM [dbo].[AdminType] WHERE [AdminTypeShort] IN (N'SuperUser', N'CustomerSupport', N'DepartmentAdmin', N'DepartmentAdminStaff', N'RequesterScheduler', N'DepartmentHRStaff', N'DepartmentFiscalStaff', N'DepartmentAuditor');
            ");
        }
    }
}

