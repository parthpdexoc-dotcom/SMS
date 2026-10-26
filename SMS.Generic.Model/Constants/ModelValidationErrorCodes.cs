using System.ComponentModel;

namespace SMS.Generic.Models.Constants
{
    public class ModelValidationErrorCodes : ModelErrorCodes
    {
        [Description("Request is invalid")]
        public const string INVALID_REQUEST = "INVALID_REQUEST";

        #region AdminTypeDto
        [Description("AdminTypeShort is required")]
        public const string REQUIRED_ADMIN_TYPE_SHORT = "REQUIRED_ADMIN_TYPE_SHORT";

        [Description("AdminTypeShort cannot exceed 100 characters")]
        public const string MAXIMUM_LENGTH_ADMIN_TYPE_SHORT = "MAXIMUM_LENGTH_ADMIN_TYPE_SHORT";

        [Description("AdminTypeLong is required")]
        public const string REQUIRED_ADMIN_TYPE_LONG = "REQUIRED_ADMIN_TYPE_LONG";

        [Description("AdminTypeLong cannot exceed 255 characters")]
        public const string MAXIMUM_LENGTH_ADMIN_TYPE_LONG = "MAXIMUM_LENGTH_ADMIN_TYPE_LONG";

        [Description("AdminTypeShort already exists")]
        public const string DUPLICATE_ADMIN_TYPE_SHORT = "DUPLICATE_ADMIN_TYPE_SHORT";

        [Description("Status is required")]
        public const string REQUIRED_STATUS = "REQUIRED_STATUS";

        [Description("Status cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_STATUS = "MAXIMUM_LENGTH_STATUS";
        #endregion

        #region LoginDto,AdminForgotPasswordDto,AdminDto
        [Description("Admin type is required")]
        public const string REQUIRED_ADMIN_TYPE_ID = "REQUIRED_ADMIN_TYPE_ID";


        [Description("First name cannot exceed 255 characters")]
        public const string MAXIMUM_LENGTH_FIRST_NAME = "MAXIMUM_LENGTH_FIRST_NAME";


        [Description("Last name cannot exceed 255 characters")]
        public const string MAXIMUM_LENGTH_LAST_NAME = "MAXIMUM_LENGTH_LAST_NAME";

        [Description("Username is required")]
        public const string REQUIRED_USERNAME = "REQUIRED_USERNAME";

        [Description("Username cannot exceed {0} characters")]
        public const string MAXIMUM_LENGTH_USERNAME = "MAXIMUM_LENGTH_USERNAME";

        [Description("Password is required")]
        public const string REQUIRED_PASSWORD = "REQUIRED_PASSWORD";

        [Description("Password must be at least 6 characters")]
        public const string MINIMUM_LENGTH_PASSWORD = "MINIMUM_LENGTH_PASSWORD";

        [Description("Password cannot exceed 100 characters")]
        public const string MAXIMUM_LENGTH_PASSWORD = "MAXIMUM_LENGTH_PASSWORD";

        [Description("New password is required")]
        public const string REQUIRED_NEW_PASSWORD = "REQUIRED_NEW_PASSWORD";

        [Description("Password must be at least 6 characters")]
        public const string MINIMUM_LENGTH_NEW_PASSWORD = "MINIMUM_LENGTH_NEW_PASSWORD";
        #endregion

        #region BusinessTypeDto
        [Description("BusinessTypeShort is required")]
        public const string REQUIRED_BUSINESS_TYPE_SHORT = "REQUIRED_BUSINESS_TYPE_SHORT";

        [Description("BusinessTypeShort cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_BUSINESS_TYPE_SHORT = "MAXIMUM_LENGTH_BUSINESS_TYPE_SHORT";

        [Description("BusinessTypeLong is required")]
        public const string REQUIRED_BUSINESS_TYPE_LONG = "REQUIRED_BUSINESS_TYPE_LONG";

        [Description("BusinessTypeLong cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_BUSINESS_TYPE_LONG = "MAXIMUM_LENGTH_BUSINESS_TYPE_LONG";

        [Description("BusinessTypeShort already exists in this department")]
        public const string DUPLICATE_BUSINESS_TYPE_SHORT = "DUPLICATE_BUSINESS_TYPE_SHORT";
        #endregion

        #region PaymentTypeDto
        [Description("Payment type name is required")]
        public const string REQUIRED_PAYMENT_TYPE_NAME = "REQUIRED_PAYMENT_TYPE_NAME";

        [Description("Payment type name cannot exceed 100 characters")]
        public const string MAXIMUM_LENGTH_PAYMENT_TYPE_NAME = "MAXIMUM_LENGTH_PAYMENT_TYPE_NAME";

        [Description("Payment type name already exists in this department")]
        public const string DUPLICATE_PAYMENT_TYPE_NAME = "DUPLICATE_PAYMENT_TYPE_NAME";

        [Description("\"{0}\" is already the default payment type. Only one default payment type is allowed.")]
        public const string DUPLICATE_PAYMENT_TYPE_DEFAULT = "DUPLICATE_PAYMENT_TYPE_DEFAULT";
        #endregion

        #region PayCodeDto
        [Description("PayCodeValue is required")]
        public const string REQUIRED_PAY_CODE_VALUE = "REQUIRED_PAY_CODE_VALUE";

        [Description("PayCodeValue cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_PAY_CODE_VALUE = "MAXIMUM_LENGTH_PAY_CODE_VALUE";

        [Description("PayCodeDescription is required")]
        public const string REQUIRED_PAY_CODE_DESCRIPTION = "REQUIRED_PAY_CODE_DESCRIPTION";

        [Description("PayCodeDescription cannot exceed 1000 characters")]
        public const string MAXIMUM_LENGTH_PAY_CODE_DESCRIPTION = "MAXIMUM_LENGTH_PAY_CODE_DESCRIPTION";

        [Description("PayCodeValue already exists in this department")]
        public const string DUPLICATE_PAY_CODE_VALUE = "DUPLICATE_PAY_CODE_VALUE";
        #endregion

        #region PaymentDto
        [Description("Payment was not found or has been deleted.")]
        public const string PAYMENT_NOT_FOUND_OR_DELETED = "PAYMENT_NOT_FOUND_OR_DELETED";

        [Description("This payment has already been voided.")]
        public const string PAYMENT_ALREADY_VOIDED = "PAYMENT_ALREADY_VOIDED";

        [Description("At least one invoice is required to allocate payment.")]
        public const string REQUIRED_PAYMENT_ALLOCATE_INVOICES = "REQUIRED_PAYMENT_ALLOCATE_INVOICES";

        [Description("The total allocation amount exceeds the remaining balance on this payment.")]
        public const string PAYMENT_ALLOCATION_EXCEEDS_REMAINING_BALANCE = "PAYMENT_ALLOCATION_EXCEEDS_REMAINING_BALANCE";

        [Description("Payment requester does not match the invoice requester.")]
        public const string PAYMENT_INVOICE_REQUESTER_MISMATCH = "PAYMENT_INVOICE_REQUESTER_MISMATCH";

        [Description("The new payment amount is less than the previously applied payment amount to invoices. You must adjust the previously applied payment now to balance your payment on the invoices or cancel your action. DO YOU WISH TO CONTINUE?")]
        public const string PAYMENT_AMOUNT_REQUIRES_ALLOCATION_ADJUSTMENT = "PAYMENT_AMOUNT_REQUIRES_ALLOCATION_ADJUSTMENT";

        [Description("The payment amount must be at least {0:N2}. That amount has already been refunded from this payment.")]
        public const string PAYMENT_AMOUNT_LESS_THAN_REFUNDED = "PAYMENT_AMOUNT_LESS_THAN_REFUNDED";

        [Description("The payment amount must be at least {0:N2} to cover {1:N2} applied to invoices and {2:N2} already refunded.")]
        public const string PAYMENT_AMOUNT_LESS_THAN_APPLIED_AND_REFUNDED = "PAYMENT_AMOUNT_LESS_THAN_APPLIED_AND_REFUNDED";

        [Description("Total invoice allocation of {0:N2} plus refunded amount of {1:N2} exceeds the payment amount of {2:N2}. The maximum available for invoice allocation is {3:N2}. Reduce allocation amounts to continue.")]
        public const string PAYMENT_ALLOCATION_EXCEEDS_PAYMENT_AMOUNT = "PAYMENT_ALLOCATION_EXCEEDS_PAYMENT_AMOUNT";

        [Description("The payment allocation does not belong to this payment.")]
        public const string PAYMENT_ALLOCATION_NOT_BELONG_TO_PAYMENT = "PAYMENT_ALLOCATION_NOT_BELONG_TO_PAYMENT";

        [Description("No changes were detected. Update the payment amount, date, reference, type, or allocation amounts before saving.")]
        public const string PAYMENT_ADJUST_NO_CHANGES = "PAYMENT_ADJUST_NO_CHANGES";

        [Description("Refund amount must be greater than zero.")]
        public const string INVALID_REFUND_PAYMENT_AMOUNT = "INVALID_REFUND_PAYMENT_AMOUNT";

        [Description("At least one payment is required for a refund.")]
        public const string REQUIRED_REFUND_PAYMENT_ALLOCATIONS = "REQUIRED_REFUND_PAYMENT_ALLOCATIONS";

        [Description("Refund payment allocations total {0:N2} does not match the refund amount {1:N2}.")]
        public const string REFUND_PAYMENT_TOTAL_MISMATCH = "REFUND_PAYMENT_TOTAL_MISMATCH";

        [Description("Payment {0}: only {1:N2} is available to refund.")]
        public const string REFUND_PAYMENT_EXCEEDS_UNUSED_BALANCE = "REFUND_PAYMENT_EXCEEDS_UNUSED_BALANCE";

        [Description("Payment cannot be deleted. Active allocations remain on {0}.")]
        public const string PAYMENT_DELETE_BLOCKED_HAS_ALLOCATIONS = "PAYMENT_DELETE_BLOCKED_HAS_ALLOCATIONS";

        [Description("Enter a reason to delete the payment.")]
        public const string PAYMENT_DELETE_REQUIRES_REASON = "PAYMENT_DELETE_REQUIRES_REASON";

        [Description("Enter a reason to delete the refund.")]
        public const string REFUND_PAYMENT_DELETE_REQUIRES_REASON = "REFUND_PAYMENT_DELETE_REQUIRES_REASON";

        [Description("Refund payment was not found or has been deleted.")]
        public const string REFUND_PAYMENT_NOT_FOUND_OR_DELETED = "REFUND_PAYMENT_NOT_FOUND_OR_DELETED";

        [Description("Cannot reduce refund by {0:N2} because the current refund amount is only {1:N2}.")]
        public const string REFUND_PAYMENT_RELEASE_EXCEEDS_AMOUNT = "REFUND_PAYMENT_RELEASE_EXCEEDS_AMOUNT";
        #endregion

        #region PaymentAllocationDto
        [Description("InvoicesID is required")]
        public const string REQUIRED_PAYMENT_ALLOCATION_INVOICE_ID = "REQUIRED_PAYMENT_ALLOCATION_INVOICE_ID";

        [Description("PaymentID is required")]
        public const string REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID = "REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID";

        [Description("AmountApplied must be greater than zero")]
        public const string INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED = "INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED";

        [Description("No active payment allocation was found for this invoice and payment.")]
        public const string PAYMENT_ALLOCATION_NOT_FOUND_FOR_INVOICE = "PAYMENT_ALLOCATION_NOT_FOUND_FOR_INVOICE";

        [Description("This payment allocation has already been voided.")]
        public const string PAYMENT_ALLOCATION_ALREADY_VOIDED = "PAYMENT_ALLOCATION_ALREADY_VOIDED";

        [Description("No payment allocation exists for this invoice and payment; a credit entry cannot be added.")]
        public const string PAYMENT_ALLOCATION_CREDIT_REQUIRES_EXISTING_ALLOCATIONS = "PAYMENT_ALLOCATION_CREDIT_REQUIRES_EXISTING_ALLOCATIONS";

        [Description("A reason is required when discarding a payment allocation.")]
        public const string PAYMENT_ALLOCATION_DISCARD_REQUIRES_REASON = "PAYMENT_ALLOCATION_DISCARD_REQUIRES_REASON";

        [Description("Refund payment allocation not found or already deleted.")]
        public const string REFUND_PAYMENT_ALLOCATION_NOT_FOUND_OR_DELETED = "REFUND_PAYMENT_ALLOCATION_NOT_FOUND_OR_DELETED";

        [Description("This refund payment allocation has already been voided.")]
        public const string REFUND_PAYMENT_ALLOCATION_ALREADY_VOIDED = "REFUND_PAYMENT_ALLOCATION_ALREADY_VOIDED";

        [Description("A reason is required when discarding a refund payment allocation.")]
        public const string REFUND_PAYMENT_ALLOCATION_DISCARD_REQUIRES_REASON = "REFUND_PAYMENT_ALLOCATION_DISCARD_REQUIRES_REASON";

        [Description("Deleting this refund allocation would leave no remaining refunded amount on refund {0}. Please delete refund {0} instead.")]
        public const string REFUND_PAYMENT_ALLOCATION_VOID_WOULD_ZERO_REFUND = "REFUND_PAYMENT_ALLOCATION_VOID_WOULD_ZERO_REFUND";
        #endregion

        #region PolicyDto
        [Description("PostsTakenPerDay must be greater than or equal to 0")]
        public const string POLICY_JOBS_TAKEN_PER_DAY_INVALID = "POLICY_JOBS_TAKEN_PER_DAY_INVALID";

        [Description("HoursPerDayWorked must be greater than or equal to 0")]
        public const string POLICY_HOURS_PER_DAY_INVALID = "POLICY_HOURS_PER_DAY_INVALID";

        [Description("HoursPerWeekWorked must be greater than or equal to 0")]
        public const string POLICY_HOURS_PER_WEEK_INVALID = "POLICY_HOURS_PER_WEEK_INVALID";

        [Description("HoursPerMonthWorked must be greater than or equal to 0")]
        public const string POLICY_HOURS_PER_MONTH_INVALID = "POLICY_HOURS_PER_MONTH_INVALID";

        [Description("NumberOfEmployeesBeforeSupervisorRequire must be greater than or equal to 0")]
        public const string POLICY_NUMBER_OF_EMPLOYEES_BEFORE_SUPERVISOR_INVALID = "POLICY_NUMBER_OF_EMPLOYEES_BEFORE_SUPERVISOR_INVALID";

        [Description("MinHoursPaid must be greater than or equal to 0 when provided")]
        public const string POLICY_MIN_HOURS_PAID_INVALID = "POLICY_MIN_HOURS_PAID_INVALID";
        #endregion

        #region DepartmentPolicyDto
        [Description("DepartmentID is required")]
        public const string REQUIRED_DEPARTMENT_POLICY_DEPARTMENT_ID = "REQUIRED_DEPARTMENT_POLICY_DEPARTMENT_ID";

        [Description("PostsTakenPerDay must be greater than or equal to 0")]
        public const string DEPARTMENT_POLICY_JOBS_TAKEN_PER_DAY_INVALID = "DEPARTMENT_POLICY_JOBS_TAKEN_PER_DAY_INVALID";

        [Description("HoursPerDayWorked must be greater than or equal to 0")]
        public const string DEPARTMENT_POLICY_HOURS_PER_DAY_INVALID = "DEPARTMENT_POLICY_HOURS_PER_DAY_INVALID";

        [Description("HoursPerWeekWorked must be greater than or equal to 0")]
        public const string DEPARTMENT_POLICY_HOURS_PER_WEEK_INVALID = "DEPARTMENT_POLICY_HOURS_PER_WEEK_INVALID";

        [Description("HoursPerMonthWorked must be greater than or equal to 0")]
        public const string DEPARTMENT_POLICY_HOURS_PER_MONTH_INVALID = "DEPARTMENT_POLICY_HOURS_PER_MONTH_INVALID";

        [Description("DeptOpenPostsTakenPerDay must be greater than or equal to 0")]
        public const string DEPARTMENT_POLICY_DEPT_OPEN_JOBS_TAKEN_PER_DAY_INVALID = "DEPARTMENT_POLICY_DEPT_OPEN_JOBS_TAKEN_PER_DAY_INVALID";

        [Description("TenurePostsAllowedPerWeek must be greater than or equal to 0")]
        public const string DEPARTMENT_POLICY_TENURE_JOBS_ALLOWED_PER_WEEK_INVALID = "DEPARTMENT_POLICY_TENURE_JOBS_ALLOWED_PER_WEEK_INVALID";

        [Description("TenurePostsAllowedPerDay must be greater than or equal to 0")]
        public const string DEPARTMENT_POLICY_TENURE_JOBS_ALLOWED_PER_DAY_INVALID = "DEPARTMENT_POLICY_TENURE_JOBS_ALLOWED_PER_DAY_INVALID";

        [Description("DeptOpenPostsTakenPerWeek must be greater than or equal to 0")]
        public const string DEPARTMENT_POLICY_DEPT_OPEN_JOBS_TAKEN_PER_WEEK_INVALID = "DEPARTMENT_POLICY_DEPT_OPEN_JOBS_TAKEN_PER_WEEK_INVALID";

        [Description("Previewable must be greater than or equal to 0 when provided")]
        public const string DEPARTMENT_POLICY_PREVIEWABLE_INVALID = "DEPARTMENT_POLICY_PREVIEWABLE_INVALID";

        [Description("PreviewableHours must be greater than or equal to 0 when provided")]
        public const string DEPARTMENT_POLICY_PREVIEWABLE_HOURS_INVALID = "DEPARTMENT_POLICY_PREVIEWABLE_HOURS_INVALID";

        [Description("Tenure24HourLimit must be greater than or equal to 0 when provided")]
        public const string DEPARTMENT_POLICY_TENURE_24_HOUR_LIMIT_INVALID = "DEPARTMENT_POLICY_TENURE_24_HOUR_LIMIT_INVALID";

        [Description("Tenure24HourLimitPosts must be greater than or equal to 0 when provided")]
        public const string DEPARTMENT_POLICY_TENURE_24_HOUR_LIMIT_JOBS_INVALID = "DEPARTMENT_POLICY_TENURE_24_HOUR_LIMIT_JOBS_INVALID";

        [Description("MinHoursPaid must be greater than or equal to 0")]
        public const string DEPARTMENT_POLICY_MIN_HOURS_PAID_INVALID = "DEPARTMENT_POLICY_MIN_HOURS_PAID_INVALID";

        [Description("UsesTenureJobsForWeekRule must be greater than or equal to 0 when provided")]
        public const string DEPARTMENT_POLICY_USES_TENURE_JOBS_FOR_WEEK_RULE_INVALID = "DEPARTMENT_POLICY_USES_TENURE_JOBS_FOR_WEEK_RULE_INVALID";

        [Description("Position24HourQuota must be greater than or equal to 0 when provided")]
        public const string DEPARTMENT_POLICY_POSITION_24_HOUR_QUOTA_INVALID = "DEPARTMENT_POLICY_POSITION_24_HOUR_QUOTA_INVALID";

        [Description("PositionExempt24HourQuota must be greater than or equal to 0 when provided")]
        public const string DEPARTMENT_POLICY_POSITION_EXEMPT_24_HOUR_QUOTA_INVALID = "DEPARTMENT_POLICY_POSITION_EXEMPT_24_HOUR_QUOTA_INVALID";

        [Description("Managed24HourQuota must be greater than or equal to 0 when provided")]
        public const string DEPARTMENT_POLICY_MANAGED_24_HOUR_QUOTA_INVALID = "DEPARTMENT_POLICY_MANAGED_24_HOUR_QUOTA_INVALID";

        [Description("PostToSupervisor must be greater than or equal to 0 when provided")]
        public const string DEPARTMENT_POLICY_JOB_TO_SUPERVISOR_INVALID = "DEPARTMENT_POLICY_JOB_TO_SUPERVISOR_INVALID";

        [Description("OverlapJobConflictConfirmation is required")]
        public const string REQUIRED_DEPARTMENT_POLICY_OVERLAP_JOB_CONFLICT_CONFIRMATION = "REQUIRED_DEPARTMENT_POLICY_OVERLAP_JOB_CONFLICT_CONFIRMATION";

        [Description("SupervisorsCanViewLowerEmpTypeJobs is required")]
        public const string REQUIRED_DEPARTMENT_POLICY_SUPERVISORS_CAN_VIEW_LOWER_EMP_TYPE_JOBS = "REQUIRED_DEPARTMENT_POLICY_SUPERVISORS_CAN_VIEW_LOWER_EMP_TYPE_JOBS";

        [Description("SupervisorsCanViewLowerEmpTypeJobsCount must be greater than or equal to 0")]
        public const string DEPARTMENT_POLICY_SUPERVISORS_CAN_VIEW_LOWER_EMP_TYPE_JOBS_COUNT_INVALID = "DEPARTMENT_POLICY_SUPERVISORS_CAN_VIEW_LOWER_EMP_TYPE_JOBS_COUNT_INVALID";

        [Description("NumberOfEmployeesBeforeSupervisorRequired must be greater than or equal to 0")]
        public const string DEPARTMENT_POLICY_NUMBER_OF_EMPLOYEES_BEFORE_SUPERVISOR_INVALID = "DEPARTMENT_POLICY_NUMBER_OF_EMPLOYEES_BEFORE_SUPERVISOR_INVALID";
        #endregion

        #region DepartmentDto
        [Description("DepartmentCode already exists")]
        public const string DUPLICATE_DEPARTMENT_CODE = "DUPLICATE_DEPARTMENT_CODE";

        [Description("DepartmentName already exists")]
        public const string DUPLICATE_DEPARTMENT_NAME = "DUPLICATE_DEPARTMENT_NAME";

        [Description("Department folder name already exists")]
        public const string DUPLICATE_EMPLOYEE_DATA_FOLDER_NAME = "DUPLICATE_EMPLOYEE_DATA_FOLDER_NAME";
        #endregion

        #region PostDocumentDto
        [Description("This job document already exists")]
        public const string DUPLICATE_JOB_DOCUMENT = "DUPLICATE_JOB_DOCUMENT";

        [Description("Documentname is already exists")]
        public const string DUPLICATE_JOB_DOCUMENT_NAME = "DUPLICATE_JOB_DOCUMENT_NAME";

        [Description("Document name is required")]
        public const string REQUIRED_DOCUMENT_NAME = "REQUIRED_DOCUMENT_NAME";

        [Description("Document name exceeds maximum allowed length")]
        public const string MAXIMUM_LENGTH_DOCUMENT_NAME = "MAXIMUM_LENGTH_DOCUMENT_NAME";

        [Description("Document path is required")]
        public const string REQUIRED_DOCUMENT_PATH = "REQUIRED_DOCUMENT_PATH";

        [Description("Document path exceeds maximum allowed length")]
        public const string MAXIMUM_LENGTH_DOCUMENT_PATH = "MAXIMUM_LENGTH_DOCUMENT_PATH";
        #endregion

        #region SkillDto
        [Description("SkillShort is required")]
        public const string REQUIRED_SKILL_SHORT = "REQUIRED_SKILL_SHORT";

        [Description("SkillShort cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_SKILL_SHORT = "MAXIMUM_LENGTH_SKILL_SHORT";

        [Description("SkillLong cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_SKILL_LONG = "MAXIMUM_LENGTH_SKILL_LONG";

        [Description("SkillShort already exists in this department")]
        public const string DUPLICATE_SKILL_SHORT = "DUPLICATE_SKILL_SHORT";
        #endregion

        #region DepartmentActivityTypeDto
        [Description("ActivityTypeShort already exists in this department")]
        public const string DUPLICATE_ACTIVITY_TYPE_SHORT = "DUPLICATE_ACTIVITY_TYPE_SHORT";
        #endregion

        #region DepartmentDivisionsDto
        [Description("DivisionShort already exists in this department")]
        public const string DUPLICATE_DIVISION_SHORT = "DUPLICATE_DIVISION_SHORT";
        #endregion

        #region DepartmentEquipmentDto
        [Description("EquipementShort already exists in this department")]
        public const string DUPLICATE_EQUIPMENT_SHORT = "DUPLICATE_EQUIPMENT_SHORT";
        #endregion

        #region EmployeeTypeDto
        [Description("EmployeeTypeShort already exists")]
        public const string DUPLICATE_EMPLOYEE_TYPE_SHORT = "DUPLICATE_EMPLOYEE_TYPE_SHORT";
        #endregion

        #region EmployeeStatusDto
        [Description("StatusShort already exists")]
        public const string DUPLICATE_STATUS_SHORT = "DUPLICATE_STATUS_SHORT";
        #endregion


        #region EquipmentTypeDto
        [Description("EquipementTypeShort already exists")]
        public const string DUPLICATE_EQUIPMENT_TYPE_SHORT = "DUPLICATE_EQUIPMENT_TYPE_SHORT";
        #endregion

        #region EquipmentDto
        [Description("EquipementShort already exists in this department")]
        public const string DUPLICATE_EQUIPMENT_SHORT_DEPARTMENT = "DUPLICATE_EQUIPMENT_SHORT_DEPARTMENT";
        #endregion

        #region PostDto
        [Description("Job short code already exists in this department")]
        public const string DUPLICATE_JOB_SHORT = "DUPLICATE_JOB_SHORT";
        #endregion

        #region PostEquipmentDto
        [Description("Job equipment already exists for this job")]
        public const string DUPLICATE_JOB_EQUIPMENT = "DUPLICATE_JOB_EQUIPMENT";
        #endregion

        #region PostPositionDto
        [Description("Job position already exists for this job")]
        public const string DUPLICATE_JOB_POSITION = "DUPLICATE_JOB_POSITION";
        #endregion

        #region RequestDto
        [Description("Request already exists")]
        public const string DUPLICATE_REQUEST = "DUPLICATE_REQUEST";

        [Description("Not allowed to schedule a job with a suspended employee")]
        public const string REQUEST_SUSPENDED_EMPLOYEE_NOT_ALLOWED_TO_SCHEDULE = "REQUEST_SUSPENDED_EMPLOYEE_NOT_ALLOWED_TO_SCHEDULE";
        [Description("Not allowed to schedule a job with suspended employee {0}")]
        public const string REQUEST_SUSPENDED_EMPLOYEE_NOT_ALLOWED_TO_SCHEDULE_FOR_EMPLOYEE = "REQUEST_SUSPENDED_EMPLOYEE_NOT_ALLOWED_TO_SCHEDULE_FOR_EMPLOYEE";

        [Description("Assigned employees exist on the current request date. Review impacted assignments before changing to a future date.")]
        public const string REQUEST_ONE_TIME_DATE_CHANGE_ASSIGNMENTS_WARNING = "REQUEST_ONE_TIME_DATE_CHANGE_ASSIGNMENTS_WARNING";

        [Description("One or more selected post positions are invalid for request date transfer.")]
        public const string REQUEST_ONE_TIME_DATE_CHANGE_INVALID_TRANSFER_POST_POSITION_IDS = "REQUEST_ONE_TIME_DATE_CHANGE_INVALID_TRANSFER_POST_POSITION_IDS";
        #endregion

        #region RequestService
        [Description("Recipient email is required.")]
        public const string RECIPIENT_EMAIL_REQUIRED = "Recipient email is required.";

        [Description("Request with ID '{0}' not found.")]
        public const string REQUEST_NOT_FOUND = "Request with ID '{0}' not found.";

        [Description("A request must have at least one schedule.")]
        public const string REQUEST_MUST_HAVE_AT_LEAST_ONE_SCHEDULE = "REQUEST_MUST_HAVE_AT_LEAST_ONE_SCHEDULE";
        #endregion

        #region RequestEquipmentDto
        [Description("RequestEquipment already exists")]
        public const string DUPLICATE_REQUEST_EQUIPMENT = "DUPLICATE_REQUEST_EQUIPMENT";
        #endregion

        #region RequesterDto
        [Description("Requester already exists")]
        public const string DUPLICATE_REQUESTER = "DUPLICATE_REQUESTER";
        #endregion

        #region RequesterAuthorizedDto
        [Description("RequesterAuthorized already exists")]
        public const string DUPLICATE_REQUESTER_AUTHORIZED = "DUPLICATE_REQUESTER_AUTHORIZED";
        #endregion

        #region RequesterManagerDto
        [Description("RequesterManager already exists")]
        public const string DUPLICATE_REQUESTER_MANAGER = "DUPLICATE_REQUESTER_MANAGER";
        #endregion

        #region RequesterDeleteValidation
        [Description("Cannot delete Requester because it is associated with existing Requests.")]
        public const string REQUESTER_HAS_REQUESTS_CANNOT_DELETE = "REQUESTER_HAS_REQUESTS_CANNOT_DELETE";
        #endregion

        #region RequesterRestrictedListDto
        [Description("RequesterRestrictedList already exists")]
        public const string DUPLICATE_REQUESTER_RESTRICTED_LIST = "DUPLICATE_REQUESTER_RESTRICTED_LIST";
        #endregion

        #region DepartmentResponseAreaDto
        [Description("DepartmentResponseAreaShort already exists in this department")]
        public const string DUPLICATE_RESPONSE_AREA_SHORT = "DUPLICATE_RESPONSE_AREA_SHORT";
        #endregion

        #region EmployeeAdminDto
        [Description("Employee is already assigned this admin type")]
        public const string DUPLICATE_EMPLOYEE_ADMIN_ASSIGNMENT = "DUPLICATE_EMPLOYEE_ADMIN_ASSIGNMENT";
        #endregion

        #region PostEquipmentDto
        [Description("Equipment is already assigned to this job")]
        public const string DUPLICATE_JOB_EQUIPMENT_ASSIGNMENT = "DUPLICATE_JOB_EQUIPMENT_ASSIGNMENT";
        #endregion

        #region PostPositionDto
        [Description("Employee is already assigned to this job on this date")]
        public const string DUPLICATE_JOB_POSITION_ASSIGNMENT = "DUPLICATE_JOB_POSITION_ASSIGNMENT";

        [Description("Placeholder is already assigned to this job on this date")]
        public const string DUPLICATE_PLACEHOLDER_JOB_POSITION_ASSIGNMENT = "DUPLICATE_PLACEHOLDER_JOB_POSITION_ASSIGNMENT";

        [Description("{0} is already assigned to this job on this date")]
        public const string DUPLICATE_JOB_POSITION_ASSIGNMENT_FOR_EMPLOYEE = "DUPLICATE_JOB_POSITION_ASSIGNMENT_FOR_EMPLOYEE";
        #endregion

        #region RequestEquipmentDto
        [Description("Equipment is already requested for this request")]
        public const string DUPLICATE_REQUEST_EQUIPMENT_ASSIGNMENT = "DUPLICATE_REQUEST_EQUIPMENT_ASSIGNMENT";
        #endregion

        #region DepartmentEquipmentDto
        [Description("Equipment type is already configured for this department")]
        public const string DUPLICATE_DEPARTMENT_EQUIPMENT_TYPE = "DUPLICATE_DEPARTMENT_EQUIPMENT_TYPE";
        #endregion

        #region EmployeeTenureDto
        [Description("Employee already has a tenure record for this employee type in this department")]
        public const string DUPLICATE_EMPLOYEE_TENURE = "DUPLICATE_EMPLOYEE_TENURE";

        [Description("Historical records cannot be updated. Only current records can be modified.")]
        public const string CANNOT_UPDATE_HISTORICAL_RECORD = "CANNOT_UPDATE_HISTORICAL_RECORD";

        [Description("EmployeeTypeID cannot be changed on existing records. Promotions must be created as new records.")]
        public const string CANNOT_UPDATE_EMPLOYEE_TYPE_ID = "CANNOT_UPDATE_EMPLOYEE_TYPE_ID";

        [Description("Promotion date must be greater than the last promotion date.")]
        public const string PROMOTION_DATE_MUST_BE_GREATER = "PROMOTION_DATE_MUST_BE_GREATER";

        [Description("DateOfPromotion must be greater than or equal to DateOfHire.")]
        public const string PROMOTION_DATE_MUST_BE_GREATER_OR_EQUAL_TO_HIRE_DATE = "PROMOTION_DATE_MUST_BE_GREATER_OR_EQUAL_TO_HIRE_DATE";

        [Description("Duplicate promotion detected. An employee cannot be promoted to the same type on the same date.")]
        public const string DUPLICATE_PROMOTION = "DUPLICATE_PROMOTION";

        [Description("Promotion date is required when creating a promotion.")]
        public const string PROMOTION_DATE_REQUIRED = "PROMOTION_DATE_REQUIRED";

        [Description("Only one current record is allowed per employee.")]
        public const string MULTIPLE_CURRENT_RECORDS = "MULTIPLE_CURRENT_RECORDS";

        [Description("DateOfHire cannot be changed. It must remain the same as the first hire date.")]
        public const string DATE_OF_HIRE_CANNOT_BE_CHANGED = "DATE_OF_HIRE_CANNOT_BE_CHANGED";

        [Description("The employee does not belong to the specified department.")]
        public const string EMPLOYEE_DOES_NOT_BELONG_TO_DEPARTMENT = "EMPLOYEE_DOES_NOT_BELONG_TO_DEPARTMENT";

        [Description("Selected employee type is not available.")]
        public const string EMPLOYEE_TYPE_NOT_AVAILABLE = "EMPLOYEE_TYPE_NOT_AVAILABLE";

        [Description("Employee can only be promoted to a higher rank.")]
        public const string EMPLOYEE_CAN_ONLY_BE_PROMOTED_TO_HIGHER_RANK = "EMPLOYEE_CAN_ONLY_BE_PROMOTED_TO_HIGHER_RANK";
        #endregion

        #region RequesterManagerDto
        [Description("Employee is already assigned as manager for this requester")]
        public const string DUPLICATE_REQUESTER_MANAGER_ASSIGNMENT = "DUPLICATE_REQUESTER_MANAGER_ASSIGNMENT";

        [Description("Cannot delete manager. First delete all employees assigned to this manager, then delete the manager.")]
        public const string REQUESTER_MANAGER_HAS_EMPLOYEES_CANNOT_DELETE = "REQUESTER_MANAGER_HAS_EMPLOYEES_CANNOT_DELETE";
        #endregion

        #region RequesterRestrictedListDto
        [Description("Employee is already restricted from working for this requester")]
        public const string DUPLICATE_REQUESTER_RESTRICTION = "DUPLICATE_REQUESTER_RESTRICTION";

        [Description("Expired records cannot be edited")]
        public const string CANNOT_EDIT_EXPIRED_REQUESTER_RESTRICTION = "CANNOT_EDIT_EXPIRED_REQUESTER_RESTRICTION";
        #endregion

        #region DepartmentActivityTypeDto
        [Description("ActivityTypeShort is required")]
        public const string REQUIRED_ACTIVITY_TYPE_SHORT = "REQUIRED_ACTIVITY_TYPE_SHORT";

        [Description("ActivityTypeShort cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_ACTIVITY_TYPE_SHORT = "MAXIMUM_LENGTH_ACTIVITY_TYPE_SHORT";

        [Description("ActivityTypeLong is required")]
        public const string REQUIRED_ACTIVITY_TYPE_LONG = "REQUIRED_ACTIVITY_TYPE_LONG";

        [Description("ActivityTypeLong cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_ACTIVITY_TYPE_LONG = "MAXIMUM_LENGTH_ACTIVITY_TYPE_LONG";
        #endregion

        #region EmployeeTypeDto
        [Description("EmployeeTypeShort is required")]
        public const string REQUIRED_EMPLOYEE_TYPE_SHORT = "REQUIRED_EMPLOYEE_TYPE_SHORT";

        [Description("EmployeeTypeShort cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_TYPE_SHORT = "MAXIMUM_LENGTH_EMPLOYEE_TYPE_SHORT";

        [Description("EmployeeTypeLong is required")]
        public const string REQUIRED_EMPLOYEE_TYPE_LONG = "REQUIRED_EMPLOYEE_TYPE_LONG";

        [Description("EmployeeTypeLong cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_TYPE_LONG = "MAXIMUM_LENGTH_EMPLOYEE_TYPE_LONG";

        [Description("Rank already exists. Each employee type must have a unique rank.")]
        public const string DUPLICATE_EMPLOYEE_TYPE_RANK = "DUPLICATE_EMPLOYEE_TYPE_RANK";
        #endregion

        #region EquipmentTypeDto
        [Description("EquipementTypeShort is required")]
        public const string REQUIRED_EQUIPMENT_TYPE_SHORT = "REQUIRED_EQUIPMENT_TYPE_SHORT";

        [Description("EquipementTypeShort cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_EQUIPMENT_TYPE_SHORT = "MAXIMUM_LENGTH_EQUIPMENT_TYPE_SHORT";

        [Description("EquipementTypeLong is required")]
        public const string REQUIRED_EQUIPMENT_TYPE_LONG = "REQUIRED_EQUIPMENT_TYPE_LONG";

        [Description("EquipementTypeLong cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_EQUIPMENT_TYPE_LONG = "MAXIMUM_LENGTH_EQUIPMENT_TYPE_LONG";
        #endregion

        #region EmployeeStatusDto
        [Description("StatusShort is required")]
        public const string REQUIRED_STATUS_SHORT = "REQUIRED_STATUS_SHORT";

        [Description("StatusShort cannot exceed 100 characters")]
        public const string MAXIMUM_LENGTH_STATUS_SHORT = "MAXIMUM_LENGTH_STATUS_SHORT";

        [Description("StatusLong is required")]
        public const string REQUIRED_STATUS_LONG = "REQUIRED_STATUS_LONG";

        [Description("StatusLong cannot exceed 255 characters")]
        public const string MAXIMUM_LENGTH_STATUS_LONG = "MAXIMUM_LENGTH_STATUS_LONG";
        #endregion

        #region DepartmentDivisionsDto
        [Description("DivisionShort is required")]
        public const string REQUIRED_DIVISION_SHORT = "REQUIRED_DIVISION_SHORT";

        [Description("DivisionShort cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_DIVISION_SHORT = "MAXIMUM_LENGTH_DIVISION_SHORT";

        [Description("DivisionLong is required")]
        public const string REQUIRED_DIVISION_LONG = "REQUIRED_DIVISION_LONG";

        [Description("DivisionLong cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_DIVISION_LONG = "MAXIMUM_LENGTH_DIVISION_LONG";
        #endregion

        #region DepartmentResponseAreaDto
        [Description("DepartmentResponseAreaShort is required")]
        public const string REQUIRED_RESPONSE_AREA_SHORT = "REQUIRED_RESPONSE_AREA_SHORT";

        [Description("DepartmentResponseAreaShort cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_RESPONSE_AREA_SHORT = "MAXIMUM_LENGTH_RESPONSE_AREA_SHORT";

        [Description("DepartmentResponseAreaLong is required")]
        public const string REQUIRED_RESPONSE_AREA_LONG = "REQUIRED_RESPONSE_AREA_LONG";

        [Description("DepartmentResponseAreaLong cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_RESPONSE_AREA_LONG = "MAXIMUM_LENGTH_RESPONSE_AREA_LONG";
        #endregion

        #region EquipmentDto
        [Description("EquipementTypeID is required")]
        public const string REQUIRED_EQUIPMENT_TYPE_ID = "REQUIRED_EQUIPMENT_TYPE_ID";

        [Description("EquipementShort is required")]
        public const string REQUIRED_EQUIPMENT_SHORT = "REQUIRED_EQUIPMENT_SHORT";

        [Description("EquipementShort cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_EQUIPMENT_SHORT = "MAXIMUM_LENGTH_EQUIPMENT_SHORT";

        [Description("EquipementLong is required")]
        public const string REQUIRED_EQUIPMENT_LONG = "REQUIRED_EQUIPMENT_LONG";

        [Description("EquipementLong cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_EQUIPMENT_LONG = "MAXIMUM_LENGTH_EQUIPMENT_LONG";
        #endregion

        #region RequesterManagerDto
        [Description("RequesterID is required")]
        public const string REQUIRED_REQUESTER_ID = "REQUIRED_REQUESTER_ID";
        #endregion

        #region RequesterAuthorizedDto
        [Description("FirstName is required")]
        public const string REQUIRED_REQUESTER_AUTHORIZED_FIRST_NAME = "REQUIRED_REQUESTER_AUTHORIZED_FIRST_NAME";

        [Description("FirstName cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_FIRST_NAME = "MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_FIRST_NAME";

        [Description("LastName is required")]
        public const string REQUIRED_REQUESTER_AUTHORIZED_LAST_NAME = "REQUIRED_REQUESTER_AUTHORIZED_LAST_NAME";

        [Description("LastName cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_LAST_NAME = "MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_LAST_NAME";

        [Description("PrimaryPhone is required")]
        public const string REQUIRED_REQUESTER_AUTHORIZED_PRIMARY_PHONE = "REQUIRED_REQUESTER_AUTHORIZED_PRIMARY_PHONE";

        [Description("PrimaryPhone cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_PRIMARY_PHONE = "MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_PRIMARY_PHONE";

        [Description("PPIsCell cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_PP_IS_CELL = "MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_PP_IS_CELL";

        [Description("SecondaryPhone cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_SECONDARY_PHONE = "MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_SECONDARY_PHONE";

        [Description("SPIsCellPhone cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_SP_IS_CELL_PHONE = "MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_SP_IS_CELL_PHONE";

        [Description("PrimaryEmail is required")]
        public const string REQUIRED_REQUESTER_AUTHORIZED_PRIMARY_EMAIL = "REQUIRED_REQUESTER_AUTHORIZED_PRIMARY_EMAIL";

        [Description("PrimaryEmail cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_PRIMARY_EMAIL = "MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_PRIMARY_EMAIL";

        [Description("SecondaryEmail cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_SECONDARY_EMAIL = "MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_SECONDARY_EMAIL";

        [Description("Primary phone number must be unique")]
        public const string DUPLICATE_REQUESTER_AUTHORIZED_PRIMARY_PHONE = "DUPLICATE_REQUESTER_AUTHORIZED_PRIMARY_PHONE";

        [Description("Primary email must be unique")]
        public const string DUPLICATE_REQUESTER_AUTHORIZED_PRIMARY_EMAIL = "DUPLICATE_REQUESTER_AUTHORIZED_PRIMARY_EMAIL";

        [Description("Primary phone and secondary phone cannot be the same")]
        public const string PRIMARY_AND_SECONDARY_PHONE_CANNOT_BE_SAME = "PRIMARY_AND_SECONDARY_PHONE_CANNOT_BE_SAME";

        [Description("Primary email and secondary email cannot be the same")]
        public const string PRIMARY_AND_SECONDARY_EMAIL_CANNOT_BE_SAME = "PRIMARY_AND_SECONDARY_EMAIL_CANNOT_BE_SAME";
        #endregion

        #region EmployeeTenureDto
        [Description("EmployeeTypeID is required")]
        public const string REQUIRED_EMPLOYEE_TYPE_ID = "REQUIRED_EMPLOYEE_TYPE_ID";
        #endregion

        #region RoleDto
        [Description("Name is required")]
        public const string REQUIRED_ROLE_NAME = "REQUIRED_ROLE_NAME";
        #endregion

        #region RequestersDto
        [Description("DepartmentDefinedNumber is required")]
        public const string REQUIRED_DEPARTMENT_DEFINED_NUMBER = "REQUIRED_DEPARTMENT_DEFINED_NUMBER";

        [Description("DepartmentDefinedNumber cannot exceed 25 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_DEFINED_NUMBER = "MAXIMUM_LENGTH_DEPARTMENT_DEFINED_NUMBER";

        [Description("BusinessTypeID is required")]
        public const string REQUIRED_BUSINESS_TYPE_ID = "REQUIRED_BUSINESS_TYPE_ID";

        [Description("Name is required")]
        public const string REQUIRED_REQUESTER_NAME = "REQUIRED_REQUESTER_NAME";

        [Description("Name cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_NAME = "MAXIMUM_LENGTH_REQUESTER_NAME";

        [Description("Address is required")]
        public const string REQUIRED_REQUESTER_ADDRESS = "REQUIRED_REQUESTER_ADDRESS";

        [Description("Address cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_ADDRESS = "MAXIMUM_LENGTH_REQUESTER_ADDRESS";

        [Description("City is required")]
        public const string REQUIRED_REQUESTER_CITY = "REQUIRED_REQUESTER_CITY";

        [Description("City cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_CITY = "MAXIMUM_LENGTH_REQUESTER_CITY";

        [Description("State is required")]
        public const string REQUIRED_REQUESTER_STATE = "REQUIRED_REQUESTER_STATE";

        [Description("State cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_STATE = "MAXIMUM_LENGTH_REQUESTER_STATE";

        [Description("Zip is required")]
        public const string REQUIRED_REQUESTER_ZIP = "REQUIRED_REQUESTER_ZIP";

        [Description("Zip cannot exceed 10 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_ZIP = "MAXIMUM_LENGTH_REQUESTER_ZIP";

        [Description("Email is required")]
        public const string REQUIRED_REQUESTER_EMAIL = "REQUIRED_REQUESTER_EMAIL";

        [Description("Email cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_EMAIL = "MAXIMUM_LENGTH_REQUESTER_EMAIL";

        [Description("Phone is required")]
        public const string REQUIRED_REQUESTER_PHONE = "REQUIRED_REQUESTER_PHONE";

        [Description("Phone cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_PHONE = "MAXIMUM_LENGTH_REQUESTER_PHONE";
        #endregion

        #region DepartmentConfigurationDto
        [Description("DeptConfigID is required")]
        public const string REQUIRED_DEPARTMENT_CONFIGURATION_DEPT_CONFIG_ID = "REQUIRED_DEPARTMENT_CONFIGURATION_DEPT_CONFIG_ID";

        [Description("DepartmentID is required")]
        public const string REQUIRED_DEPARTMENT_CONFIGURATION_DEPARTMENT_ID = "REQUIRED_DEPARTMENT_CONFIGURATION_DEPARTMENT_ID";

        [Description("Department Home Location is required when Enable Employee Location is enabled")]
        public const string REQUIRED_DEPARTMENT_CONFIGURATION_DEPARTMENT_HOME_LOCATION = "REQUIRED_DEPARTMENT_CONFIGURATION_DEPARTMENT_HOME_LOCATION";
        #endregion

        #region DepartmentFeeDto
        [Description("DepartmentFee DepartmentID is required")]
        public const string REQUIRED_DEPARTMENT_FEE_DEPARTMENT_ID = "REQUIRED_DEPARTMENT_FEE_DEPARTMENT_ID";

        [Description("DepartmentFee FeeShort is required")]
        public const string REQUIRED_DEPARTMENT_FEE_FEE_SHORT = "REQUIRED_DEPARTMENT_FEE_FEE_SHORT";

        [Description("DepartmentFee FeeShort cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_FEE_FEE_SHORT = "MAXIMUM_LENGTH_DEPARTMENT_FEE_FEE_SHORT";

        [Description("DepartmentFee FeeLong cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_FEE_FEE_LONG = "MAXIMUM_LENGTH_DEPARTMENT_FEE_FEE_LONG";

        [Description("DepartmentFee FeeType must be a valid enum value")]
        public const string INVALID_DEPARTMENT_FEE_FEE_TYPE = "INVALID_DEPARTMENT_FEE_FEE_TYPE";
        #endregion

        #region Payroll
        [Description("Payroll processing completed successfully")]
        public const string PAYROLL_PROCESSING_SUCCESS = "PAYROLL_PROCESSING_SUCCESS";

        [Description("Payroll processing failed")]
        public const string PAYROLL_PROCESSING_FAILED = "PAYROLL_PROCESSING_FAILED";

        [Description("No payroll records found for the specified date range")]
        public const string PAYROLL_NO_RECORDS_FOUND = "PAYROLL_NO_RECORDS_FOUND";

        [Description("FTP connection test: SUCCESS")]
        public const string FTP_CONNECTION_SUCCESS = "FTP_CONNECTION_SUCCESS";

        [Description("FTP connection test: FAILED")]
        public const string FTP_CONNECTION_FAILED = "FTP_CONNECTION_FAILED";

        [Description("SFTP connection test: SUCCESS")]
        public const string SFTP_CONNECTION_SUCCESS = "SFTP_CONNECTION_SUCCESS";

        [Description("SFTP connection test: FAILED")]
        public const string SFTP_CONNECTION_FAILED = "SFTP_CONNECTION_FAILED";

        [Description("Data processing test: SUCCESS - Retrieved {0} records")]
        public const string DATA_PROCESSING_TEST_SUCCESS = "DATA_PROCESSING_TEST_SUCCESS";

        [Description("Data processing test: FAILED - {0}")]
        public const string DATA_PROCESSING_TEST_FAILED = "DATA_PROCESSING_TEST_FAILED";

        [Description("Internal server error during payroll processing")]
        public const string PAYROLL_INTERNAL_SERVER_ERROR = "PAYROLL_INTERNAL_SERVER_ERROR";

        [Description("Invalid date format. Use yyyy-MM-dd format.")]
        public const string PAYROLL_INVALID_DATE_FORMAT = "PAYROLL_INVALID_DATE_FORMAT";
        #endregion

        #region DepartmentFee
        [Description("DepartmentFee DefaultFee must be greater than or equal to 0")]
        public const string INVALID_DEPARTMENT_FEE_DEFAULT_FEE = "INVALID_DEPARTMENT_FEE_DEFAULT_FEE";

        [Description("DepartmentFee AdditionalFee must be greater than or equal to 0")]
        public const string INVALID_DEPARTMENT_FEE_ADDITIONAL_FEE = "INVALID_DEPARTMENT_FEE_ADDITIONAL_FEE";

        [Description("DepartmentFee ReimburseTo must be a valid enum value")]
        public const string INVALID_DEPARTMENT_FEE_REIMBURSE_TO = "INVALID_DEPARTMENT_FEE_REIMBURSE_TO";

        [Description("A fee with the same FeeShort already exists for this department")]
        public const string DUPLICATE_DEPARTMENT_FEE_FEE_SHORT = "DUPLICATE_DEPARTMENT_FEE_FEE_SHORT";

        [Description("DefaultFeeConditions is mandatory when IsAdditionalFees is true")]
        public const string REQUIRED_DEPARTMENT_FEE_DEFAULT_FEE_CONDITIONS_WHEN_ADDITIONAL_FEES = "REQUIRED_DEPARTMENT_FEE_DEFAULT_FEE_CONDITIONS_WHEN_ADDITIONAL_FEES";

        [Description("DepartmentFee EffectiveStartDate is required")]
        public const string REQUIRED_DEPARTMENT_FEE_EFFECTIVE_START_DATE = "REQUIRED_DEPARTMENT_FEE_EFFECTIVE_START_DATE";

        [Description("DepartmentFee EffectiveEndDate must be on or after EffectiveStartDate")]
        public const string INVALID_DEPARTMENT_FEE_EFFECTIVE_DATE_RANGE = "INVALID_DEPARTMENT_FEE_EFFECTIVE_DATE_RANGE";
        #endregion

        #region DepartmentEquipmentDto
        [Description("EquipmentTypeID is required")]
        public const string REQUIRED_DEPARTMENT_EQUIPMENT_EQUIPMENT_TYPE_ID = "REQUIRED_DEPARTMENT_EQUIPMENT_EQUIPMENT_TYPE_ID";

        [Description("ChargedType is required")]
        public const string REQUIRED_DEPARTMENT_EQUIPMENT_CHARGED_TYPE = "REQUIRED_DEPARTMENT_EQUIPMENT_CHARGED_TYPE";

        [Description("Charge is required")]
        public const string REQUIRED_DEPARTMENT_EQUIPMENT_CHARGE = "REQUIRED_DEPARTMENT_EQUIPMENT_CHARGE";

        [Description("ChargeMileage is required")]
        public const string REQUIRED_DEPARTMENT_EQUIPMENT_CHARGE_MILEAGE = "REQUIRED_DEPARTMENT_EQUIPMENT_CHARGE_MILEAGE";

        [Description("MileageCharge is required")]
        public const string REQUIRED_DEPARTMENT_EQUIPMENT_MILEAGE_CHARGE = "REQUIRED_DEPARTMENT_EQUIPMENT_MILEAGE_CHARGE";

        [Description("MileageCondition is required")]
        public const string REQUIRED_DEPARTMENT_EQUIPMENT_MILEAGE_CONDITION = "REQUIRED_DEPARTMENT_EQUIPMENT_MILEAGE_CONDITION";

        [Description("EquipmentStatus is required")]
        public const string REQUIRED_DEPARTMENT_EQUIPMENT_EQUIPMENT_STATUS = "REQUIRED_DEPARTMENT_EQUIPMENT_EQUIPMENT_STATUS";

        [Description("RequiresApproval is required")]
        public const string REQUIRED_DEPARTMENT_EQUIPMENT_REQUIRES_APPROVAL = "REQUIRED_DEPARTMENT_EQUIPMENT_REQUIRES_APPROVAL";

        [Description("RequiresUtilizationRecorded is required")]
        public const string REQUIRED_DEPARTMENT_EQUIPMENT_REQUIRES_UTILIZATION_RECORDED = "REQUIRED_DEPARTMENT_EQUIPMENT_REQUIRES_UTILIZATION_RECORDED";
        #endregion

        #region EmployeeAdminDto
        [Description("AdminTypeID is required")]
        public const string REQUIRED_EMPLOYEE_ADMIN_ADMIN_TYPE_ID = "REQUIRED_EMPLOYEE_ADMIN_ADMIN_TYPE_ID";

        [Description("EmployeeID is required")]
        public const string REQUIRED_EMPLOYEE_ADMIN_EMPLOYEE_ID = "REQUIRED_EMPLOYEE_ADMIN_EMPLOYEE_ID";

        [Description("CreatedBy cannot exceed 255 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_ADMIN_CREATED_BY = "MAXIMUM_LENGTH_EMPLOYEE_ADMIN_CREATED_BY";

        [Description("EditedBy cannot exceed 255 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_ADMIN_EDITED_BY = "MAXIMUM_LENGTH_EMPLOYEE_ADMIN_EDITED_BY";
        #endregion

        #region EmployeeDto
        [Description("DepartmentID is required")]
        public const string REQUIRED_EMPLOYEE_DEPARTMENT_ID = "REQUIRED_EMPLOYEE_DEPARTMENT_ID";

        [Description("StatusID is required")]
        public const string REQUIRED_EMPLOYEE_STATUS_ID = "REQUIRED_EMPLOYEE_STATUS_ID";

        [Description("Username is required")]
        public const string REQUIRED_EMPLOYEE_USERNAME = "REQUIRED_EMPLOYEE_USERNAME";

        [Description("Username cannot exceed 25 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_USERNAME = "MAXIMUM_LENGTH_EMPLOYEE_USERNAME";

        [Description("Password is required")]
        public const string REQUIRED_EMPLOYEE_PASSWORD = "REQUIRED_EMPLOYEE_PASSWORD";

        [Description("Password cannot exceed 60 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_PASSWORD = "MAXIMUM_LENGTH_EMPLOYEE_PASSWORD";

        [Description("BadgeNumber cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_BADGE_NUMBER = "MAXIMUM_LENGTH_EMPLOYEE_BADGE_NUMBER";

        [Description("EmployeeNumber cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_NUMBER = "MAXIMUM_LENGTH_EMPLOYEE_NUMBER";

        #region PlaceholderDto
        [Description("This badge number is already used by another placeholder")]
        public const string DUPLICATE_PLACEHOLDER_EMPLOYEE_NUMBER = "DUPLICATE_PLACEHOLDER_EMPLOYEE_NUMBER";

        [Description("Cannot archived this placeholder as it has some assignment history")]
        public const string PLACEHOLDER_HAS_ASSIGNMENT_HISTORY = "PLACEHOLDER_HAS_ASSIGNMENT_HISTORY";

        [Description("An archived placeholder cannot be unarchived")]
        public const string PLACEHOLDER_CANNOT_BE_UNARCHIVED = "PLACEHOLDER_CANNOT_BE_UNARCHIVED";

        [Description("Placeholder status updated")]
        public const string PLACEHOLDER_STATUS_UPDATED = "PLACEHOLDER_STATUS_UPDATED";
        #endregion

        [Description("FirstName cannot exceed 100 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_FIRST_NAME = "MAXIMUM_LENGTH_EMPLOYEE_FIRST_NAME";

        [Description("LastName cannot exceed 100 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_LAST_NAME = "MAXIMUM_LENGTH_EMPLOYEE_LAST_NAME";

        [Description("PrimaryPhone cannot exceed 25 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_PRIMARY_PHONE = "MAXIMUM_LENGTH_EMPLOYEE_PRIMARY_PHONE";

        [Description("SecondaryPhone cannot exceed 25 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_SECONDARY_PHONE = "MAXIMUM_LENGTH_EMPLOYEE_SECONDARY_PHONE";

        [Description("SMS telephone number is required when SMS consent is checked")]
        public const string REQUIRED_EMPLOYEE_SMS_TELEPHONE_NUMBER = "REQUIRED_EMPLOYEE_SMS_TELEPHONE_NUMBER";

        [Description("SMS telephone number must include the country code")]
        public const string INVALID_EMPLOYEE_SMS_TELEPHONE_NUMBER = "INVALID_EMPLOYEE_SMS_TELEPHONE_NUMBER";

        [Description("PrimaryEmail cannot exceed 150 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_PRIMARY_EMAIL = "MAXIMUM_LENGTH_EMPLOYEE_PRIMARY_EMAIL";

        [Description("SecondaryEmail cannot exceed 150 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_SECONDARY_EMAIL = "MAXIMUM_LENGTH_EMPLOYEE_SECONDARY_EMAIL";

        [Description("CreatedBy cannot exceed 255 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_CREATED_BY = "MAXIMUM_LENGTH_EMPLOYEE_CREATED_BY";

        [Description("EditedBy cannot exceed 255 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_EDITED_BY = "MAXIMUM_LENGTH_EMPLOYEE_EDITED_BY";
        #endregion

        #region EmployeeService eligibility and split
        [Description("You are not allowed to split with yourself.")]
        public const string EMPLOYEE_SPLIT_WITH_SELF_NOT_ALLOWED = "EMPLOYEE_SPLIT_WITH_SELF_NOT_ALLOWED";

        [Description("This employee is not eligible for this job because they have no skills on file.")]
        public const string EMPLOYEE_JOB_ELIGIBILITY_MISSING_SKILLS = "EMPLOYEE_JOB_ELIGIBILITY_MISSING_SKILLS";

        [Description("This employee is not eligible for this job because they are missing one or more required skills.")]
        public const string EMPLOYEE_JOB_ELIGIBILITY_MISSING_REQUIRED_SKILLS = "EMPLOYEE_JOB_ELIGIBILITY_MISSING_REQUIRED_SKILLS";

        [Description("This employee is not eligible for this job because their rank (employee type) is missing or invalid.")]
        public const string EMPLOYEE_JOB_ELIGIBILITY_MISSING_RANK = "EMPLOYEE_JOB_ELIGIBILITY_MISSING_RANK";

        [Description("This employee is not eligible for this job because their rank is too low. Required minimum rank: {0}. Employee rank: {1}.")]
        public const string EMPLOYEE_JOB_ELIGIBILITY_INSUFFICIENT_RANK = "EMPLOYEE_JOB_ELIGIBILITY_INSUFFICIENT_RANK";

        [Description("Bulk cancel of {0} scheduled job position(s) failed.")]
        public const string EMPLOYEE_SUSPENDED_BULK_CANCEL_SCHEDULED_JOB_POSITIONS_FAILED = "EMPLOYEE_SUSPENDED_BULK_CANCEL_SCHEDULED_JOB_POSITIONS_FAILED";
        #endregion

        #region RequesterConfigDto
        [Description("RequesterID is required")]
        public const string REQUIRED_REQUESTER_CONFIG_REQUESTER_ID = "REQUIRED_REQUESTER_CONFIG_REQUESTER_ID";

        [Description("RequesterID must be a valid GUID")]
        public const string INVALID_REQUESTER_CONFIG_REQUESTER_ID_FORMAT = "INVALID_REQUESTER_CONFIG_REQUESTER_ID_FORMAT";

        [Description("RequesterID does not exist in the database")]
        public const string REQUESTER_CONFIG_REQUESTER_ID_NOT_FOUND = "REQUESTER_CONFIG_REQUESTER_ID_NOT_FOUND";

        [Description("DefinedSkills cannot exceed 255 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_CONFIG_DEFINED_SKILLS = "MAXIMUM_LENGTH_REQUESTER_CONFIG_DEFINED_SKILLS";

        [Description("DefineRequesterManager cannot exceed 255 characters")]
        public const string MAXIMUM_LENGTH_REQUESTER_CONFIG_DEFINE_REQUESTER_MANAGER = "MAXIMUM_LENGTH_REQUESTER_CONFIG_DEFINE_REQUESTER_MANAGER";
        #endregion

        #region RequesterConfigurationDto
        [Description("RequesterID is required")]
        public const string REQUIRED_REQUESTER_CONFIGURATION_REQUESTER_ID = "REQUIRED_REQUESTER_CONFIGURATION_REQUESTER_ID";

        [Description("Open24Hr is required")]
        public const string REQUIRED_REQUESTER_CONFIGURATION_OPEN24_HR = "REQUIRED_REQUESTER_CONFIGURATION_OPEN24_HR";

        [Description("Sba24Hr is required")]
        public const string REQUIRED_REQUESTER_CONFIGURATION_SBA24_HR = "REQUIRED_REQUESTER_CONFIGURATION_SBA24_HR";

        [Description("HourPerDay is required")]
        public const string REQUIRED_REQUESTER_CONFIGURATION_HOUR_PER_DAY = "REQUIRED_REQUESTER_CONFIGURATION_HOUR_PER_DAY";

        [Description("HourPerWeek is required")]
        public const string REQUIRED_REQUESTER_CONFIGURATION_HOUR_PER_WEEK = "REQUIRED_REQUESTER_CONFIGURATION_HOUR_PER_WEEK";

        [Description("SupervisorOverride is required")]
        public const string REQUIRED_REQUESTER_CONFIGURATION_SUPERVISOR_OVERRIDE = "REQUIRED_REQUESTER_CONFIGURATION_SUPERVISOR_OVERRIDE";

        [Description("RequesterDefault is required")]
        public const string REQUIRED_REQUESTER_CONFIGURATION_REQUESTER_DEFAULT = "REQUIRED_REQUESTER_CONFIGURATION_REQUESTER_DEFAULT";

        [Description("DepartmentID is required")]
        public const string REQUIRED_REQUESTER_CONFIGURATION_DEPARTMENT_ID = "REQUIRED_REQUESTER_CONFIGURATION_DEPARTMENT_ID";
        #endregion

        #region RequesterRestrictedListDto
        [Description("RequesterID is required")]
        public const string REQUIRED_REQUESTER_RESTRICTED_LIST_REQUESTER_ID = "REQUIRED_REQUESTER_RESTRICTED_LIST_REQUESTER_ID";

        [Description("EffectiveStartDate is required")]
        public const string REQUIRED_REQUESTER_RESTRICTED_LIST_EFFECTIVE_START_DATE = "REQUIRED_REQUESTER_RESTRICTED_LIST_EFFECTIVE_START_DATE";

        [Description("EffectiveEndDate is required")]
        public const string REQUIRED_REQUESTER_RESTRICTED_LIST_EFFECTIVE_END_DATE = "REQUIRED_REQUESTER_RESTRICTED_LIST_EFFECTIVE_END_DATE";
        #endregion

        #region DepartmentDto
        [Description("DepartmentCode cannot exceed 15 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_CODE = "MAXIMUM_LENGTH_DEPARTMENT_CODE";

        [Description("DepartmentName cannot exceed 75 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_NAME = "MAXIMUM_LENGTH_DEPARTMENT_NAME";

        [Description("Address cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_ADDRESS = "MAXIMUM_LENGTH_DEPARTMENT_ADDRESS";

        [Description("City cannot exceed 40 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_CITY = "MAXIMUM_LENGTH_DEPARTMENT_CITY";

        [Description("State cannot exceed 30 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_STATE = "MAXIMUM_LENGTH_DEPARTMENT_STATE";

        [Description("Zip cannot exceed 12 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_ZIP = "MAXIMUM_LENGTH_DEPARTMENT_ZIP";

        [Description("FirstName cannot exceed 15 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_FIRST_NAME = "MAXIMUM_LENGTH_DEPARTMENT_FIRST_NAME";

        [Description("LastName cannot exceed 20 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_LAST_NAME = "MAXIMUM_LENGTH_DEPARTMENT_LAST_NAME";

        [Description("Phone cannot exceed 25 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_PHONE = "MAXIMUM_LENGTH_DEPARTMENT_PHONE";

        [Description("Fax cannot exceed 25 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_FAX = "MAXIMUM_LENGTH_DEPARTMENT_FAX";

        [Description("Email cannot exceed 70 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_EMAIL = "MAXIMUM_LENGTH_DEPARTMENT_EMAIL";

        [Description("Domain cannot exceed 60 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_DOMAIN = "MAXIMUM_LENGTH_DEPARTMENT_DOMAIN";

        [Description("NameOfUserAddingRecord cannot exceed 255 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_NAME_OF_USER_ADDING_RECORD = "MAXIMUM_LENGTH_DEPARTMENT_NAME_OF_USER_ADDING_RECORD";

        [Description("UserNameEditingRecord cannot exceed 255 characters")]
        public const string MAXIMUM_LENGTH_DEPARTMENT_USER_NAME_EDITING_RECORD = "MAXIMUM_LENGTH_DEPARTMENT_USER_NAME_EDITING_RECORD";
        #endregion

        #region DivisionDto
        [Description("DivisionName cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_DIVISION_NAME = "MAXIMUM_LENGTH_DIVISION_NAME";

        [Description("DivisionDescription cannot exceed 255 characters")]
        public const string MAXIMUM_LENGTH_DIVISION_DESCRIPTION = "MAXIMUM_LENGTH_DIVISION_DESCRIPTION";

        [Description("Contact cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_DIVISION_CONTACT = "MAXIMUM_LENGTH_DIVISION_CONTACT";

        [Description("Phone cannot exceed 25 characters")]
        public const string MAXIMUM_LENGTH_DIVISION_PHONE = "MAXIMUM_LENGTH_DIVISION_PHONE";

        [Description("PvtLine cannot exceed 15 characters")]
        public const string MAXIMUM_LENGTH_DIVISION_PVT_LINE = "MAXIMUM_LENGTH_DIVISION_PVT_LINE";

        [Description("Email cannot exceed 35 characters")]
        public const string MAXIMUM_LENGTH_DIVISION_EMAIL = "MAXIMUM_LENGTH_DIVISION_EMAIL";

        [Description("Invalid email format")]
        public const string INVALID_DIVISION_EMAIL_FORMAT = "INVALID_DIVISION_EMAIL_FORMAT";

        [Description("NmBilling cannot exceed 20 characters")]
        public const string MAXIMUM_LENGTH_DIVISION_NM_BILLING = "MAXIMUM_LENGTH_DIVISION_NM_BILLING";
        #endregion

        #region Common Phone Validation

        [Description("Phone number must be 10 exactly digits")]
        public const string INVALID_PHONE_FORMAT = "INVALID_PHONE_FORMAT";

        #endregion

        #region EmployeePositionDto
        [Description("Title cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_POSITION_TITLE = "MAXIMUM_LENGTH_EMPLOYEE_POSITION_TITLE";
        #endregion

        #region EmployeeSkillDto
        [Description("Title cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_SKILL_TITLE = "MAXIMUM_LENGTH_EMPLOYEE_SKILL_TITLE";

        [Description("Description cannot exceed 255 characters")]
        public const string MAXIMUM_LENGTH_EMPLOYEE_SKILL_DESCRIPTION = "MAXIMUM_LENGTH_EMPLOYEE_SKILL_DESCRIPTION";
        #endregion

        #region PostDto

        [Description("DepartmentID is required")]
        public const string REQUIRED_JOB_DEPARTMENT_ID = "REQUIRED_JOB_DEPARTMENT_ID";

        [Description("Job title is required")]
        public const string REQUIRED_JOB_TITLE = "REQUIRED_JOB_TITLE";

        [Description("Job title cannot exceed 60 characters")]
        public const string MAXIMUM_LENGTH_JOB_TITLE = "MAXIMUM_LENGTH_JOB_TITLE";

        [Description("Job description cannot exceed 1000 characters")]
        public const string MAXIMUM_LENGTH_JOB_DESCRIPTION = "MAXIMUM_LENGTH_JOB_DESCRIPTION";

        [Description("Total job positions must be greater than zero")]
        public const string INVALID_TOTAL_JOB_POSITIONS = "INVALID_TOTAL_JOB_POSITIONS";

        [Description("Job date is required")]
        public const string REQUIRED_JOB_DATE = "REQUIRED_JOB_DATE";

        [Description("Job start time is required")]
        public const string REQUIRED_JOB_START_TIME = "REQUIRED_JOB_START_TIME";

        [Description("Job length must be greater than zero")]
        public const string INVALID_JOB_LENGTH = "INVALID_JOB_LENGTH";

        [Description("Job length cannot exceed 24 hours")]
        public const string JOB_LENGTH_EXCEEDS_24_HOURS = "JOB_LENGTH_EXCEEDS_24_HOURS";

        [Description("Job start time must be in 24-hour format")]
        public const string INVALID_JOB_START_TIME_FORMAT = "INVALID_JOB_START_TIME_FORMAT";

        [Description("Job end time is required")]
        public const string REQUIRED_JOB_END_TIME = "REQUIRED_JOB_END_TIME";

        [Description("Job end time must be in 24-hour format")]
        public const string INVALID_JOB_END_TIME_FORMAT = "INVALID_JOB_END_TIME_FORMAT";

        [Description("Job notes cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_JOB_NOTES = "MAXIMUM_LENGTH_JOB_NOTES";

        [Description("Job location cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_JOB_LOCATION = "MAXIMUM_LENGTH_JOB_LOCATION";

        [Description("Department defined value cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE = "MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE";

        [Description("Job pay rate cannot be negative")]
        public const string INVALID_JOB_PAY_RATE = "INVALID_JOB_PAY_RATE";

        [Description("Job premium rate cannot be negative")]
        public const string INVALID_JOB_PREMIUM_RATE = "INVALID_JOB_PREMIUM_RATE";

        [Description("Scheduled post positions exist for this post and have assigned employees. Review impacted assignments before saving time or minimum-hours changes.")]
        public const string JOB_SCHEDULED_ASSIGNMENTS_IMPACT_WARNING = "JOB_SCHEDULED_ASSIGNMENTS_IMPACT_WARNING";

        [Description("Cannot decrease total Job Positions because one or more call signs are already set for this Job. Select which call signs to remove and confirm.")]
        public const string JOB_CANNOT_DECREASE_TOTAL_POST_POSITIONS_WITH_ACTIVITY = "JOB_CANNOT_DECREASE_TOTAL_POST_POSITIONS_WITH_ACTIVITY";

        #endregion
        #region PostPositionDto

        [Description("Job ID is required")]
        public const string REQUIRED_JOB_ID = "REQUIRED_JOB_ID";

        [Description("EmployeeId is required")]
        public const string REQUIRED_EMPLOYEE_ID = "REQUIRED_EMPLOYEE_ID";

        [Description("DateOfWork is required")]
        public const string REQUIRED_DATE_OF_WORK = "REQUIRED_DATE_OF_WORK";

        [Description("StartTime is required")]
        public const string REQUIRED_START_TIME = "REQUIRED_START_TIME";

        [Description("LengthOfWork cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_LENGTH_OF_WORK = "MAXIMUM_LENGTH_LENGTH_OF_WORK";

        [Description("CallSign cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_CALL_SIGN = "MAXIMUM_LENGTH_CALL_SIGN";

        [Description("numberOfDays must be greater than 0.")]
        public const string NUMBER_OF_DAYS_MUST_BE_GREATER_THAN_ZERO = "NUMBER_OF_DAYS_MUST_BE_GREATER_THAN_ZERO";

        #endregion

        #region PostsOnDutyDto
        [Description("AdminOnDutyID cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_JOBS_ON_DUTY_ADMIN_ON_DUTY_ID = "MAXIMUM_LENGTH_JOBS_ON_DUTY_ADMIN_ON_DUTY_ID";

        [Description("UnitName cannot exceed 35 characters")]
        public const string MAXIMUM_LENGTH_JOBS_ON_DUTY_UNIT_NAME = "MAXIMUM_LENGTH_JOBS_ON_DUTY_UNIT_NAME";

        [Description("Job name cannot exceed 45 characters")]
        public const string MAXIMUM_LENGTH_JOBS_ON_DUTY_JOB_NAME = "MAXIMUM_LENGTH_JOBS_ON_DUTY_JOB_NAME";

        [Description("StartDate cannot exceed 12 characters")]
        public const string MAXIMUM_LENGTH_JOBS_ON_DUTY_START_DATE = "MAXIMUM_LENGTH_JOBS_ON_DUTY_START_DATE";

        [Description("ListOrder cannot exceed 250 characters")]
        public const string MAXIMUM_LENGTH_JOBS_ON_DUTY_LIST_ORDER = "MAXIMUM_LENGTH_JOBS_ON_DUTY_LIST_ORDER";
        #endregion

        #region PostTypeDto
        [Description("Title cannot exceed 35 characters")]
        public const string MAXIMUM_LENGTH_JOB_TYPE_TITLE = "MAXIMUM_LENGTH_JOB_TYPE_TITLE";

        [Description("Description cannot exceed 255 characters")]
        public const string MAXIMUM_LENGTH_JOB_TYPE_DESCRIPTION = "MAXIMUM_LENGTH_JOB_TYPE_DESCRIPTION";
        #endregion

        #region RequestDto
        [Description("StartDate is required")]
        public const string REQUIRED_REQUEST_START_DATE = "REQUIRED_REQUEST_START_DATE";

        [Description("EndDate is required")]
        public const string REQUIRED_REQUEST_END_DATE = "REQUIRED_REQUEST_END_DATE";

        [Description("EndDate cannot be less than StartDate")]
        public const string END_DATE_CANNOT_BE_LESS_THAN_START_DATE =
            "END_DATE_CANNOT_BE_LESS_THAN_START_DATE";

        [Description("For one-time requests, StartDate and EndDate must be the same")]
        public const string ONETIME_DATES_MUST_BE_SAME =
            "ONETIME_DATES_MUST_BE_SAME";

        [Description("Minimum one day gap is required between StartDate and EndDate")]
        public const string MINIMUM_ONE_DAY_GAP_REQUIRED =
            "MINIMUM_ONE_DAY_GAP_REQUIRED";

        [Description("For weekly requests, at least one weekday must be selected")]
        public const string WEEKDAYS_REQUIRED_FOR_WEEKLY =
            "WEEKDAYS_REQUIRED_FOR_WEEKLY";

        [Description("For non weekly requests, weekday must be empty")]
        public const string WEEKDAYS_NOT_REQUIRED_FOR_NON_WEEKLY =
            "WEEKDAYS_NOT_REQUIRED_FOR_NON_WEEKLY";

        [Description("For non monthly requests, monthday must be empty")]
        public const string MONTHDAYS_NOT_REQUIRED_FOR_NON_MONTHLY =
            "MONTHDAYS_NOT_REQUIRED_FOR_NON_MONTHLY";

        [Description("For non customDates requests, customDates must be empty")]
        public const string CUSTOMDATES_NOT_REQUIRED_FOR_NON_CUSTOMDATES =
           "CUSTOMDATES_NOT_REQUIRED_FOR_NON_CUSTOMDATES";

        [Description("Minimum one week gap is required between StartDate and EndDate")]
        public const string MINIMUM_ONE_WEEK_GAP_REQUIRED =
            "MINIMUM_ONE_WEEK_GAP_REQUIRED";

        [Description("Minimum one month gap is required between StartDate and EndDate")]
        public const string MINIMUM_ONE_MONTH_GAP_REQUIRED =
            "MINIMUM_ONE_MONTH_GAP_REQUIRED";
        [Description("FirstName cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_FIRST_NAME = "MAXIMUM_LENGTH_REQUEST_FIRST_NAME";

        [Description("LastName cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_LAST_NAME = "MAXIMUM_LENGTH_REQUEST_LAST_NAME";

        [Description("PrimaryPhone cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_PRIMARY_PHONE = "MAXIMUM_LENGTH_REQUEST_PRIMARY_PHONE";

        [Description("CellPhone cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_CELL_PHONE = "MAXIMUM_LENGTH_REQUEST_CELL_PHONE";

        [Description("SecondaryPhone cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_SECONDARY_PHONE = "MAXIMUM_LENGTH_REQUEST_SECONDARY_PHONE";

        [Description("AlternateCellPhone cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_ALTERNATE_CELL_PHONE = "MAXIMUM_LENGTH_REQUEST_ALTERNATE_CELL_PHONE";

        [Description("PrimaryEmail cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_PRIMARY_EMAIL = "MAXIMUM_LENGTH_REQUEST_PRIMARY_EMAIL";

        [Description("PrimaryEmail must be a valid email address")]
        public const string INVALID_REQUEST_PRIMARY_EMAIL_FORMAT = "INVALID_REQUEST_PRIMARY_EMAIL_FORMAT";

        [Description("SecondaryEmail cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_SECONDARY_EMAIL = "MAXIMUM_LENGTH_REQUEST_SECONDARY_EMAIL";

        [Description("SecondaryEmail must be a valid email address")]
        public const string INVALID_REQUEST_SECONDARY_EMAIL_FORMAT = "INVALID_REQUEST_SECONDARY_EMAIL_FORMAT";

        [Description("Title cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_TITLE = "MAXIMUM_LENGTH_REQUEST_TITLE";

        [Description("Location cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_LOCATION = "MAXIMUM_LENGTH_REQUEST_LOCATION";

        [Description("Address is required")]
        public const string REQUIRED_REQUEST_ADDRESS = "REQUIRED_REQUEST_ADDRESS";

        [Description("City is required")]
        public const string REQUIRED_REQUEST_CITY = "REQUIRED_REQUEST_CITY";

        [Description("State is required")]
        public const string REQUIRED_REQUEST_STATE = "REQUIRED_REQUEST_STATE";

        [Description("Zip is required")]
        public const string REQUIRED_REQUEST_ZIP = "REQUIRED_REQUEST_ZIP";

        [Description("Address cannot exceed 250 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_ADDRESS = "MAXIMUM_LENGTH_REQUEST_ADDRESS";

        [Description("City cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_CITY = "MAXIMUM_LENGTH_REQUEST_CITY";

        [Description("State cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_STATE = "MAXIMUM_LENGTH_REQUEST_STATE";

        [Description("Zip cannot exceed 10 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_ZIP = "MAXIMUM_LENGTH_REQUEST_ZIP";

        [Description("DepartmentDefinedFieldValue1 cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_1 = "MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_1";

        [Description("DepartmentDefinedFieldValue2 cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_2 = "MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_2";

        [Description("DepartmentDefinedFieldValue3 cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_3 = "MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_3";

        [Description("DepartmentDefinedFieldValue4 cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_4 = "MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_4";

        [Description("DepartmentDefinedFieldValue5 cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_5 = "MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_5";

        [Description("DepartmentDefinedFieldValue6 cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_6 = "MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_6";

        [Description("DepartmentDefinedFieldValue7 cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_7 = "MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_7";

        [Description("DepartmentDefinedFieldValue8 cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_8 = "MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_8";

        [Description("DepartmentDefinedFieldValue9 cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_9 = "MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_9";

        [Description("DepartmentDefinedFieldValue10 cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_10 = "MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_10";

        [Description("Requires vehicle at every job is required when RequiresVehicle is true")]
        public const string REQUIRES_VEHICLE_AT_EACH_JOB_REQUIRED = "REQUIRES_VEHICLE_AT_EACH_JOB_REQUIRED";

        [Description("NumberOfVehicles must be greater than zero when RequiresVehicle is true")]
        public const string NUMBER_OF_VEHICLES_REQUIRED = "NUMBER_OF_VEHICLES_REQUIRED";

        [Description("VehicleApproved is required when RequiresVehicle is true")]
        public const string VEHICLE_APPROVED_REQUIRED = "VEHICLE_APPROVED_REQUIRED";

        [Description("Requires vehicle at every job must not be specified when RequiresVehicle is false")]
        public const string REQUIRES_VEHICLE_AT_EACH_JOB_NOT_ALLOWED = "REQUIRES_VEHICLE_AT_EACH_JOB_NOT_ALLOWED";

        [Description("VehicleApproved must not be specified when RequiresVehicle is false")]
        public const string VEHICLE_APPROVED_NOT_ALLOWED = "VEHICLE_APPROVED_NOT_ALLOWED";

        [Description("Requires equipment at each job is required when RequiresEquipment is true")]
        public const string REQUIRES_EQUIPMENT_AT_EACH_JOB_REQUIRED = "REQUIRES_EQUIPMENT_AT_EACH_JOB_REQUIRED";

        [Description("NumberOfEquipment must be greater than zero when RequiresEquipment is true")]
        public const string NUMBER_OF_EQUIPMENT_REQUIRED = "NUMBER_OF_EQUIPMENT_REQUIRED";

        [Description("Requires equipment at each job must not be specified when RequiresEquipment is false")]
        public const string REQUIRES_EQUIPMENT_AT_EACH_JOB_NOT_ALLOWED = "REQUIRES_EQUIPMENT_AT_EACH_JOB_NOT_ALLOWED";

        [Description("You are changing the Request to a Premium Event where future assignments have already been assigned. Do you want those assignments updated to reflect the updated Pay Rate?")]
        public const string REQUEST_PREMIUM_EVENT_ENABLE_FUTURE_ASSIGNMENT_CONFIRM = "REQUEST_PREMIUM_EVENT_ENABLE_FUTURE_ASSIGNMENT_CONFIRM";

        [Description("You are changing the Request from a Premium Event where future assignments have already been assigned. Do you want those assignments updated to reflect the updated Pay Rate?")]
        public const string REQUEST_PREMIUM_EVENT_DISABLE_FUTURE_ASSIGNMENT_CONFIRM = "REQUEST_PREMIUM_EVENT_DISABLE_FUTURE_ASSIGNMENT_CONFIRM";

        [Description("Changing the request dates will also adjust vehicle and equipment effective start and end dates to match the new request dates. Do you want to continue?")]
        public const string REQUEST_DATE_CHANGE_EQUIPMENT_DATES_CONFIRM = "REQUEST_DATE_CHANGE_EQUIPMENT_DATES_CONFIRM";

        #endregion

        #region RequestScheduleDto
        [Description("The request start and end dates are invalid.")]
        public const string INVALID_REQUEST_SCHEDULE_REQUEST_DATE_RANGE = "INVALID_REQUEST_SCHEDULE_REQUEST_DATE_RANGE";

        [Description("A schedule already exists for this request that overlaps the generated dates.")]
        public const string DUPLICATE_REQUEST_SCHEDULE = "DUPLICATE_REQUEST_SCHEDULE";

        [Description("Generate Schedule is not enabled for this department.")]
        public const string REQUEST_SCHEDULE_GENERATE_NOT_ENABLED = "REQUEST_SCHEDULE_GENERATE_NOT_ENABLED";

        [Description("Generate Schedule is not available for daily or one-time requests.")]
        public const string REQUEST_SCHEDULE_GENERATE_DAILY_NOT_ALLOWED = "REQUEST_SCHEDULE_GENERATE_DAILY_NOT_ALLOWED";

        [Description("Select at least one day to generate schedules.")]
        public const string REQUEST_SCHEDULE_GENERATE_DAYS_REQUIRED = "REQUEST_SCHEDULE_GENERATE_DAYS_REQUIRED";

        [Description("No schedules could be generated for the selected configuration.")]
        public const string REQUEST_SCHEDULE_GENERATE_NO_OCCURRENCES = "REQUEST_SCHEDULE_GENERATE_NO_OCCURRENCES";

        [Description("The schedule day plus the processing hours lands on the work day. That happens every week, so tenure schedules were not created. Change the schedule day or the processing hours.")]
        public const string REQUEST_SCHEDULE_TENURE_NOTICE = "REQUEST_SCHEDULE_TENURE_NOTICE";

        [Description("The first days are too close for tenure, so they were released now as Open. Tenure schedules were not created because the schedule day plus the processing hours lands on the work day every week. Change the schedule day or the processing hours.")]
        public const string REQUEST_SCHEDULE_TENURE_NOTICE_OPEN = "REQUEST_SCHEDULE_TENURE_NOTICE_OPEN";
        #endregion

        #region RequestEquipmentDto
        [Description("RequestID is required")]
        public const string REQUIRED_REQUEST_EQUIPMENT_REQUEST_ID = "REQUIRED_REQUEST_EQUIPMENT_REQUEST_ID";

        [Description("DepartmentEquipmentID is required")]
        public const string REQUIRED_REQUEST_EQUIPMENT_DEPARTMENT_EQUIPMENT_ID = "REQUIRED_REQUEST_EQUIPMENT_DEPARTMENT_EQUIPMENT_ID";

        [Description("EquipmentQuantity must be greater than zero")]
        public const string INVALID_REQUEST_EQUIPMENT_QUANTITY = "INVALID_REQUEST_EQUIPMENT_QUANTITY";
        #endregion

        #region PostEquipmentDto
        [Description("Job ID is required")]
        public const string REQUIRED_JOB_EQUIPMENT_JOB_ID = "REQUIRED_JOB_EQUIPMENT_JOB_ID";

        [Description("DepartmentEquipmentID is required")]
        public const string REQUIRED_JOB_EQUIPMENT_DEPARTMENT_EQUIPMENT_ID = "REQUIRED_JOB_EQUIPMENT_DEPARTMENT_EQUIPMENT_ID";

        [Description("EquipmentQuantity must be greater than zero")]
        public const string INVALID_JOB_EQUIPMENT_QUANTITY = "INVALID_JOB_EQUIPMENT_QUANTITY";
        #endregion

        #region PostPositionVehicleDto
        [Description("PostPositionID is required")]
        public const string REQUIRED_POST_POSITION_VEHICLE_POST_POSITION_ID = "REQUIRED_POST_POSITION_VEHICLE_POST_POSITION_ID";

        [Description("DepartmentEquipmentID is required")]
        public const string REQUIRED_POST_POSITION_VEHICLE_DEPARTMENT_EQUIPMENT_ID = "REQUIRED_POST_POSITION_VEHICLE_DEPARTMENT_EQUIPMENT_ID";

        [Description("Duplicate post position vehicle assignment")]
        public const string DUPLICATE_POST_POSITION_VEHICLE_ASSIGNMENT = "DUPLICATE_POST_POSITION_VEHICLE_ASSIGNMENT";

        [Description("Charge must be greater than or equal to zero")]
        public const string INVALID_POST_POSITION_VEHICLE_CHARGE = "INVALID_POST_POSITION_VEHICLE_CHARGE";

        [Description("Mileage charge must be greater than or equal to zero")]
        public const string INVALID_POST_POSITION_VEHICLE_MILEAGE_CHARGE = "INVALID_POST_POSITION_VEHICLE_MILEAGE_CHARGE";
        #endregion

        #region RoutingGroupDto
        [Description("DepartmentID is required")]
        public const string REQUIRED_ROUTING_GROUP_DEPARTMENT_ID = "REQUIRED_ROUTING_GROUP_DEPARTMENT_ID";

        [Description("EmpIDs is required")]
        public const string REQUIRED_ROUTING_GROUP_EMP_IDS = "REQUIRED_ROUTING_GROUP_EMP_IDS";

        [Description("Approval type is required.")]
        public const string REQUIRED_ROUTING_GROUP_APPROVAL_TYPE = "Approval type is required.";

        [Description("ApproverLong cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_ROUTING_GROUP_APPROVER_LONG = "MAXIMUM_LENGTH_ROUTING_GROUP_APPROVER_LONG";

        [Description("ApproverShort cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_ROUTING_GROUP_APPROVER_SHORT = "MAXIMUM_LENGTH_ROUTING_GROUP_APPROVER_SHORT";

        [Description("ApproverEmail cannot exceed 150 characters")]
        public const string MAXIMUM_LENGTH_ROUTING_GROUP_APPROVER_EMAIL = "MAXIMUM_LENGTH_ROUTING_GROUP_APPROVER_EMAIL";

        [Description("Invalid email format")]
        public const string INVALID_ROUTING_GROUP_APPROVER_EMAIL_FORMAT = "INVALID_ROUTING_GROUP_APPROVER_EMAIL_FORMAT";

        [Description("This department already has a routing group for approval type '{0}'. Each department can have only one per approval type.")]
        public const string ROUTING_GROUP_APPROVAL_TYPE_ALREADY_EXISTS = "This department already has a routing group for approval type '{0}'. Each department can have only one per approval type.";
        #endregion

        #region RequestNotesDto
        [Description("RequestID is required")]
        public const string REQUIRED_REQUEST_NOTES_REQUEST_ID = "REQUIRED_REQUEST_NOTES_REQUEST_ID";

        [Description("Notes cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_NOTES = "MAXIMUM_LENGTH_REQUEST_NOTES";

        [Description("Comments cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_REQUEST_COMMENTS = "MAXIMUM_LENGTH_REQUEST_COMMENTS";

        [Description("Request note with the same content already exists for this request")]
        public const string DUPLICATE_REQUEST_NOTES = "DUPLICATE_REQUEST_NOTES";
        #endregion

        #region PostNotesDto
        [Description("Job ID is required")]
        public const string REQUIRED_JOB_NOTES_JOB_ID = "REQUIRED_JOB_NOTES_JOB_ID";

        [Description("Notes cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_JOB_NOTES_NOTES = "MAXIMUM_LENGTH_JOB_NOTES_NOTES";

        [Description("Comments cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_JOB_NOTES_COMMENTS = "MAXIMUM_LENGTH_JOB_NOTES_COMMENTS";

        [Description("Job note with the same content already exists for this job")]
        public const string DUPLICATE_JOB_NOTES = "DUPLICATE_JOB_NOTES";
        #endregion

        #region BlockDateDto
        [Description("StartDateTime is required")]
        public const string REQUIRED_BLOCK_DATE_START_DATE_TIME = "REQUIRED_BLOCK_DATE_START_DATE_TIME";

        [Description("EndDateTime is required")]
        public const string REQUIRED_BLOCK_DATE_END_DATE_TIME = "REQUIRED_BLOCK_DATE_END_DATE_TIME";

        [Description("EndDateTime cannot be less than or equal to StartDateTime")]
        public const string END_DATE_TIME_CANNOT_BE_LESS_THAN_OR_EQUAL_START_DATE_TIME = "END_DATE_TIME_CANNOT_BE_LESS_THAN_OR_EQUAL_START_DATE_TIME";

        [Description("Job ID cannot be empty when provided")]
        public const string INVALID_BLOCK_DATE_JOB_ID = "INVALID_BLOCK_DATE_JOB_ID";

        [Description("RequestID cannot be empty when provided")]
        public const string INVALID_BLOCK_DATE_REQUEST_ID = "INVALID_BLOCK_DATE_REQUEST_ID";

        [Description("Block date with the same RequestID, Job ID, StartDateTime, and EndDateTime already exists")]
        public const string DUPLICATE_BLOCK_DATE = "DUPLICATE_BLOCK_DATE";

        [Description("A block date already exists for this job: {0}")]
        public const string OVERLAPPING_BLOCK_DATE_FOR_JOB_ID = "OVERLAPPING_BLOCK_DATE_FOR_JOB_ID";

        [Description("A block date already exists for this Request: {0}")]
        public const string OVERLAPPING_BLOCK_DATE_FOR_REQUEST_ID = "OVERLAPPING_BLOCK_DATE_FOR_REQUEST_ID";
        #endregion

        #region QuestionnaireDto
        [Description("Title is required")]
        public const string REQUIRED_QUESTIONNAIRE_TITLE = "REQUIRED_QUESTIONNAIRE_TITLE";

        [Description("Title cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_QUESTIONNAIRE_TITLE = "MAXIMUM_LENGTH_QUESTIONNAIRE_TITLE";

        [Description("IsActive is required")]
        public const string REQUIRED_QUESTIONNAIRE_IS_ACTIVE = "REQUIRED_QUESTIONNAIRE_IS_ACTIVE";

        [Description("QuestionnaireType is required")]
        public const string REQUIRED_QUESTIONNAIRE_TYPE = "REQUIRED_QUESTIONNAIRE_TYPE";

        [Description("Description cannot exceed 1000 characters")]
        public const string MAXIMUM_LENGTH_QUESTIONNAIRE_DESCRIPTION = "MAXIMUM_LENGTH_QUESTIONNAIRE_DESCRIPTION";

        [Description("A questionnaire with this title already exists")]
        public const string DUPLICATE_QUESTIONNAIRE_TITLE = "DUPLICATE_QUESTIONNAIRE_TITLE";

        [Description("CreatedUserID is required")]
        public const string REQUIRED_QUESTIONNAIRE_CREATED_USER_ID = "REQUIRED_QUESTIONNAIRE_CREATED_USER_ID";

        [Description("CreatedBy is required")]
        public const string REQUIRED_QUESTIONNAIRE_CREATED_BY = "REQUIRED_QUESTIONNAIRE_CREATED_BY";

        [Description("CreatedBy cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_QUESTIONNAIRE_CREATED_BY = "MAXIMUM_LENGTH_QUESTIONNAIRE_CREATED_BY";

        [Description("CreatedDate is required")]
        public const string REQUIRED_QUESTIONNAIRE_CREATED_DATE = "REQUIRED_QUESTIONNAIRE_CREATED_DATE";

        [Description("QuestionnaireId is required")]
        public const string REQUIRED_QUESTIONNAIRE_ID = "REQUIRED_QUESTIONNAIRE_ID";

        [Description("A job questionnaire with this Job ID and QuestionnaireId combination already exists")]
        public const string DUPLICATE_JOB_QUESTIONNAIRE = "DUPLICATE_JOB_QUESTIONNAIRE";

        [Description("A RequesterQuestionnaire with this RequesterId and QuestionnaireId combination already exists")]
        public const string DUPLICATE_REQUESTER_QUESTIONNAIRE = "DUPLICATE_REQUESTER_QUESTIONNAIRE";
        #endregion

        #region ActivityFormService
        [Description("Questionnaire cannot be null")]
        public const string QUESTIONNAIRE_CANNOT_BE_NULL = "Questionnaire cannot be null";

        [Description("Failed to retrieve the upserted questionnaire form")]
        public const string FAILED_TO_RETRIEVE_UPSERTED_QUESTIONNAIRE_FORM = "Failed to retrieve the upserted questionnaire form";

        [Description("A form with this title already exists in your department. Please use a different title.")]
        public const string DUPLICATE_QUESTIONNAIRE_TITLE_IN_DEPARTMENT = "DUPLICATE_QUESTIONNAIRE_TITLE_IN_DEPARTMENT";
        #endregion

        #region QuestionDto
        [Description("QuestionText is required")]
        public const string REQUIRED_QUESTION_QUESTION_TEXT = "REQUIRED_QUESTION_QUESTION_TEXT";

        [Description("QuestionnaireId is required")]
        public const string REQUIRED_QUESTION_QUESTIONNAIRE_ID = "REQUIRED_QUESTION_QUESTIONNAIRE_ID";

        [Description("DisplayOrder must be greater than or equal to 1")]
        public const string INVALID_QUESTION_DISPLAY_ORDER = "INVALID_QUESTION_DISPLAY_ORDER";

        [Description("QuestionType must be a valid enum value")]
        public const string INVALID_QUESTION_QUESTION_TYPE = "INVALID_QUESTION_QUESTION_TYPE";

        [Description("QuestionId is required")]
        public const string REQUIRED_QUESTION_QUESTION_ID = "REQUIRED_QUESTION_QUESTION_ID";

        [Description("A question with the same QuestionText already exists for this questionnaire")]
        public const string DUPLICATE_QUESTION_QUESTION_TEXT = "DUPLICATE_QUESTION_QUESTION_TEXT";

        [Description("A question with the same DisplayOrder already exists for this questionnaire")]
        public const string DUPLICATE_QUESTION_DISPLAY_ORDER = "DUPLICATE_QUESTION_DISPLAY_ORDER";
        #endregion

        #region OptionDto
        [Description("QuestionId is required")]
        public const string REQUIRED_QUESTION_ID = "REQUIRED_QUESTION_ID";

        [Description("OptionText cannot exceed 500 characters")]
        public const string MAXIMUM_LENGTH_OPTION_TEXT = "MAXIMUM_LENGTH_OPTION_TEXT";

        [Description("DisplayOrder must be greater than or equal to 1")]
        public const string INVALID_OPTION_DISPLAY_ORDER = "INVALID_OPTION_DISPLAY_ORDER";

        [Description("An option with the same OptionText and DisplayOrder already exists for this question")]
        public const string DUPLICATE_OPTION = "DUPLICATE_OPTION";

        [Description("An option with the same DisplayOrder already exists for this question")]
        public const string DUPLICATE_OPTION_DISPLAY_ORDER = "DUPLICATE_OPTION_DISPLAY_ORDER";
        #endregion

        #region ResponseDto
        [Description("SubmittedBy is required")]
        public const string REQUIRED_RESPONSE_SUBMITTED_BY = "REQUIRED_RESPONSE_SUBMITTED_BY";

        [Description("SubmittedBy must not be empty")]
        public const string INVALID_RESPONSE_SUBMITTED_BY = "INVALID_RESPONSE_SUBMITTED_BY";

        [Description("SubmitterEmail is required")]
        public const string REQUIRED_RESPONSE_SUBMITTER_EMAIL = "REQUIRED_RESPONSE_SUBMITTER_EMAIL";

        [Description("SubmitterEmail must be a valid email address")]
        public const string INVALID_RESPONSE_SUBMITTER_EMAIL_FORMAT = "INVALID_RESPONSE_SUBMITTER_EMAIL_FORMAT";

        [Description("SubmitterEmail cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_RESPONSE_SUBMITTER_EMAIL = "MAXIMUM_LENGTH_RESPONSE_SUBMITTER_EMAIL";

        [Description("A response with the same QuestionnaireId, JobPositionId, and SubmittedBy already exists")]
        public const string DUPLICATE_RESPONSE = "DUPLICATE_RESPONSE";

        [Description("A response with the same QuestionnaireId, JobPositionId, and SubmitterEmail already exists")]
        public const string DUPLICATE_RESPONSE_SUBMITTER_EMAIL = "DUPLICATE_RESPONSE_SUBMITTER_EMAIL";
        #endregion

        #region AnswerDto
        [Description("ResponseId is required")]
        public const string REQUIRED_ANSWER_RESPONSE_ID = "REQUIRED_ANSWER_RESPONSE_ID";

        [Description("ResponseId must not be empty")]
        public const string INVALID_ANSWER_RESPONSE_ID = "INVALID_ANSWER_RESPONSE_ID";

        [Description("QuestionId is required")]
        public const string REQUIRED_ANSWER_QUESTION_ID = "REQUIRED_ANSWER_QUESTION_ID";

        [Description("QuestionId must not be empty")]
        public const string INVALID_ANSWER_QUESTION_ID = "INVALID_ANSWER_QUESTION_ID";

        [Description("Answer is required")]
        public const string REQUIRED_ANSWER_ANSWER = "REQUIRED_ANSWER_ANSWER";

        [Description("Answer must be valid JSON")]
        public const string INVALID_ANSWER_JSON_FORMAT = "INVALID_ANSWER_JSON_FORMAT";

        [Description("An answer with the same ResponseId, QuestionId, and Answer already exists")]
        public const string DUPLICATE_ANSWER = "DUPLICATE_ANSWER";
        #endregion

        #region DepartmentQuestionnaireDto
        [Description("DepartmentID is required")]
        public const string REQUIRED_DEPARTMENT_QUESTIONNAIRE_DEPARTMENT_ID = "REQUIRED_DEPARTMENT_QUESTIONNAIRE_DEPARTMENT_ID";

        [Description("QuestionnaireId is required")]
        public const string REQUIRED_DEPARTMENT_QUESTIONNAIRE_QUESTIONNAIRE_ID = "REQUIRED_DEPARTMENT_QUESTIONNAIRE_QUESTIONNAIRE_ID";

        [Description("DepartmentID cannot be an empty GUID")]
        public const string INVALID_DEPARTMENT_QUESTIONNAIRE_DEPARTMENT_ID = "INVALID_DEPARTMENT_QUESTIONNAIRE_DEPARTMENT_ID";

        [Description("QuestionnaireId cannot be an empty GUID")]
        public const string INVALID_DEPARTMENT_QUESTIONNAIRE_QUESTIONNAIRE_ID = "INVALID_DEPARTMENT_QUESTIONNAIRE_QUESTIONNAIRE_ID";

        [Description("A department questionnaire with this DepartmentID and QuestionnaireId combination already exists")]
        public const string DUPLICATE_DEPARTMENT_QUESTIONNAIRE = "DUPLICATE_DEPARTMENT_QUESTIONNAIRE";

        [Description("Only questionnaires with a source questionnaire can be assigned to a department.")]
        public const string QUESTIONNAIRE_MUST_HAVE_SOURCE_QUESTIONNAIRE_ID = "QUESTIONNAIRE_MUST_HAVE_SOURCE_QUESTIONNAIRE_ID";
        #endregion

        #region HolidayDayDto
        [Description("Holiday name already exists in this department")]
        public const string DUPLICATE_HOLIDAY_NAME = "DUPLICATE_HOLIDAY_NAME";
        #endregion

        #region ClosedPostPositionEquipmentDto
        [Description("ClosedPostPositionID is required")]
        public const string REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_CLOSED_JOB_POSITION_ID = "REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_CLOSED_JOB_POSITION_ID";

        [Description("DepartmentEquipmentID is required")]
        public const string REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_DEPARTMENT_EQUIPMENT_ID = "REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_DEPARTMENT_EQUIPMENT_ID";

        [Description("EmployeeID is required")]
        public const string REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_EMPLOYEE_ID = "REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_EMPLOYEE_ID";

        [Description("VehicleOwnedBy is required when equipment used is a vehicle")]
        public const string REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_VEHICLE_OWNED_BY = "REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_VEHICLE_OWNED_BY";

        [Description("StartMileage is required when equipment used is a vehicle")]
        public const string REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_START_MILEAGE = "REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_START_MILEAGE";

        [Description("EndMileage is required when equipment used is a vehicle")]
        public const string REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_END_MILEAGE = "REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_END_MILEAGE";

        [Description("End Mileage must be higher than Start Mileage.")]
        public const string INVALID_CLOSED_JOB_POSITION_EQUIPMENT_END_MILEAGE = "INVALID_CLOSED_JOB_POSITION_EQUIPMENT_END_MILEAGE";

        [Description("NoWorkReason cannot exceed 500 characters")]
        public const string MAXIMUM_LENGTH_CLOSED_JOB_POSITION_EQUIPMENT_NO_WORK_REASON = "MAXIMUM_LENGTH_CLOSED_JOB_POSITION_EQUIPMENT_NO_WORK_REASON";

        [Description("VehNumber cannot exceed 50 characters")]
        public const string MAXIMUM_LENGTH_CLOSED_JOB_POSITION_EQUIPMENT_VEH_NUMBER = "MAXIMUM_LENGTH_CLOSED_JOB_POSITION_EQUIPMENT_VEH_NUMBER";

        [Description("HoursWorked must be greater than or equal to zero")]
        public const string INVALID_CLOSED_JOB_POSITION_EQUIPMENT_HOURS_WORKED = "INVALID_CLOSED_JOB_POSITION_EQUIPMENT_HOURS_WORKED";

        [Description("VehicleOwnedBy must be a valid enum value (City, Employee, Other)")]
        public const string INVALID_CLOSED_JOB_POSITION_EQUIPMENT_VEHICLE_OWNED_BY = "INVALID_CLOSED_JOB_POSITION_EQUIPMENT_VEHICLE_OWNED_BY";

        [Description("BreakTime must be greater than or equal to zero")]
        public const string INVALID_CLOSED_JOB_POSITION_EQUIPMENT_BREAK_TIME = "INVALID_CLOSED_JOB_POSITION_EQUIPMENT_BREAK_TIME";

        [Description("UsedEquipmentQuantity must be greater than or equal to zero")]
        public const string INVALID_CLOSED_JOB_POSITION_EQUIPMENT_USED_QUANTITY = "INVALID_CLOSED_JOB_POSITION_EQUIPMENT_USED_QUANTITY";

        [Description("EquipmentUsedHours must be greater than or equal to zero")]
        public const string INVALID_CLOSED_JOB_POSITION_EQUIPMENT_EQUIPMENT_USED_HOURS = "INVALID_CLOSED_JOB_POSITION_EQUIPMENT_EQUIPMENT_USED_HOURS";

        [Description("PaidHours must be greater than or equal to zero")]
        public const string INVALID_CLOSED_JOB_POSITION_EQUIPMENT_PAID_HOURS = "INVALID_CLOSED_JOB_POSITION_EQUIPMENT_PAID_HOURS";

        [Description("ReimburseTo must be a valid enum value (City, Employee)")]
        public const string INVALID_CLOSED_JOB_POSITION_EQUIPMENT_REIMBURSE_TO = "INVALID_CLOSED_JOB_POSITION_EQUIPMENT_REIMBURSE_TO";

        [Description("PayRate must be greater than or equal to zero")]
        public const string INVALID_CLOSED_JOB_POSITION_EQUIPMENT_PAY_RATE = "INVALID_CLOSED_JOB_POSITION_EQUIPMENT_PAY_RATE";

        [Description("ReimbursePayRate must be greater than or equal to zero")]
        public const string INVALID_CLOSED_JOB_POSITION_EQUIPMENT_REIMBURSE_PAY_RATE = "INVALID_CLOSED_JOB_POSITION_EQUIPMENT_REIMBURSE_PAY_RATE";

        [Description("StartMileage must be greater than or equal to zero")]
        public const string INVALID_CLOSED_JOB_POSITION_EQUIPMENT_START_MILEAGE = "INVALID_CLOSED_JOB_POSITION_EQUIPMENT_START_MILEAGE";

        [Description("EndMileage must be greater than or equal to zero")]
        public const string INVALID_CLOSED_JOB_POSITION_EQUIPMENT_END_MILEAGE_VALUE = "INVALID_CLOSED_JOB_POSITION_EQUIPMENT_END_MILEAGE_VALUE";

        [Description("Closed job position does not belong to the current employee")]
        public const string CLOSED_JOB_POSITION_EQUIPMENT_CLOSED_JOB_POSITION_NOT_OWNED_BY_EMPLOYEE = "CLOSED_JOB_POSITION_EQUIPMENT_CLOSED_JOB_POSITION_NOT_OWNED_BY_EMPLOYEE";

        [Description("Job ID must match the job ID of the closed job position")]
        public const string CLOSED_JOB_POSITION_EQUIPMENT_JOB_ID_MISMATCH = "CLOSED_JOB_POSITION_EQUIPMENT_JOB_ID_MISMATCH";

        [Description("Equipment is already assigned to this closed job position")]
        public const string DUPLICATE_CLOSED_JOB_POSITION_EQUIPMENT = "DUPLICATE_CLOSED_JOB_POSITION_EQUIPMENT";

        [Description("Closed job position not found")]
        public const string CLOSED_JOB_POSITION_EQUIPMENT_CLOSED_JOB_POSITION_NOT_FOUND = "CLOSED_JOB_POSITION_EQUIPMENT_CLOSED_JOB_POSITION_NOT_FOUND";

        [Description("Employee ID missing from token")]
        public const string EMPLOYEE_ID_MISSING_FROM_TOKEN = "EMPLOYEE_ID_MISSING_FROM_TOKEN";
        #endregion

        #region ClosedPostPositionDto
        [Description("ClosedPostPositionID is required")]
        public const string REQUIRED_CLOSED_JOB_POSITION_ID = "REQUIRED_CLOSED_JOB_POSITION_ID";

        [Description("Job position ID is required")]
        public const string REQUIRED_CLOSED_JOB_POSITION_JOB_POSITION_ID = "REQUIRED_CLOSED_JOB_POSITION_JOB_POSITION_ID";

        [Description("A closed job position already exists for this job, position, and employee")]
        public const string DUPLICATE_CLOSED_JOB_POSITION = "DUPLICATE_CLOSED_JOB_POSITION";

        [Description("Job position ID does not belong to the specified job ID")]
        public const string INVALID_CLOSED_JOB_POSITION_JOB_AND_JOB_POSITION = "INVALID_CLOSED_JOB_POSITION_JOB_AND_JOB_POSITION";

        [Description("Job ID is required")]
        public const string REQUIRED_CLOSED_JOB_POSITION_JOB_ID = "REQUIRED_CLOSED_JOB_POSITION_JOB_ID";

        [Description("HoursWorked must be greater than or equal to zero")]
        public const string INVALID_CLOSED_JOB_POSITION_HOURS_WORKED = "INVALID_CLOSED_JOB_POSITION_HOURS_WORKED";

        [Description("MinimumHoursDue must be greater than or equal to zero when IsMinimumDue is true")]
        public const string INVALID_CLOSED_JOB_POSITION_MINIMUM_HOURS_DUE = "INVALID_CLOSED_JOB_POSITION_MINIMUM_HOURS_DUE";

        [Description("PayRate must be greater than or equal to zero")]
        public const string INVALID_CLOSED_JOB_POSITION_PAY_RATE = "INVALID_CLOSED_JOB_POSITION_PAY_RATE";

        [Description("ReimbursePayRate must be greater than or equal to zero")]
        public const string INVALID_CLOSED_JOB_POSITION_REIMBURSE_PAY_RATE = "INVALID_CLOSED_JOB_POSITION_REIMBURSE_PAY_RATE";

        [Description("CloseDateTime must be on or after WorkStartDate when both are present")]
        public const string CLOSED_JOB_POSITION_CLOSE_DATE_TIME_BEFORE_WORK_START = "CLOSED_JOB_POSITION_CLOSE_DATE_TIME_BEFORE_WORK_START";

        [Description("EmployeeId is required for split. Provide splitEmployeeID in the payload.")]
        public const string REQUIRED_CLOSED_JOB_POSITION_SPLIT_EMPLOYEE_ID = "REQUIRED_CLOSED_JOB_POSITION_SPLIT_EMPLOYEE_ID";

        [Description("Job is not available for split.")]
        public const string INVALID_CLOSED_JOB_POSITION_SPLIT_JOB_UNAVAILABLE = "INVALID_CLOSED_JOB_POSITION_SPLIT_JOB_UNAVAILABLE";

        [Description("Date of work is required for split.")]
        public const string REQUIRED_CLOSED_JOB_POSITION_SPLIT_DATE_OF_WORK = "REQUIRED_CLOSED_JOB_POSITION_SPLIT_DATE_OF_WORK";

        [Description("Start time is required for split.")]
        public const string REQUIRED_CLOSED_JOB_POSITION_SPLIT_START_TIME = "REQUIRED_CLOSED_JOB_POSITION_SPLIT_START_TIME";

        [Description("Not allowed to split with Suspended employe")]
        public const string CLOSED_JOB_POSITION_SPLIT_NOT_ALLOWED_SUSPENDED_EMPLOYEE = "CLOSED_JOB_POSITION_SPLIT_NOT_ALLOWED_SUSPENDED_EMPLOYEE";

        [Description("QuestionaireResponseID is required when QuestionaireRequired is true")]
        public const string REQUIRED_CLOSED_JOB_POSITION_QUESTIONAIRE_RESPONSE_ID = "REQUIRED_CLOSED_JOB_POSITION_QUESTIONAIRE_RESPONSE_ID";

        [Description("Comment cannot exceed maximum length")]
        public const string MAXIMUM_LENGTH_CLOSED_JOB_POSITION_COMMENT = "MAXIMUM_LENGTH_CLOSED_JOB_POSITION_COMMENT";

        [Description("CloseBy cannot exceed maximum length")]
        public const string MAXIMUM_LENGTH_CLOSED_JOB_POSITION_CLOSE_BY = "MAXIMUM_LENGTH_CLOSED_JOB_POSITION_CLOSE_BY";

        [Description("ReimburseStatus cannot exceed maximum length")]
        public const string MAXIMUM_LENGTH_CLOSED_JOB_POSITION_REIMBURSE_STATUS = "MAXIMUM_LENGTH_CLOSED_JOB_POSITION_REIMBURSE_STATUS";

        [Description("Split cannot exceed maximum length")]
        public const string MAXIMUM_LENGTH_CLOSED_JOB_POSITION_SPLIT = "MAXIMUM_LENGTH_CLOSED_JOB_POSITION_SPLIT";

        [Description("Parent closed job position not found")]
        public const string CLOSED_JOB_POSITION_PARENT_NOT_FOUND = "CLOSED_JOB_POSITION_PARENT_NOT_FOUND";

        [Description("Job position not found")]
        public const string CLOSED_JOB_POSITION_JOB_POSITION_NOT_FOUND = "CLOSED_JOB_POSITION_JOB_POSITION_NOT_FOUND";

        [Description("Job position is already unclosed")]
        public const string CLOSED_JOB_POSITION_ALREADY_UNCLOSED = "CLOSED_JOB_POSITION_ALREADY_UNCLOSED";
        #endregion

        #region CancelledPostPositionDto
        [Description("Job ID is required")]
        public const string REQUIRED_CANCELLED_JOB_POSITION_JOB_ID = "REQUIRED_CANCELLED_JOB_POSITION_JOB_ID";

        [Description("Job position ID is required")]
        public const string REQUIRED_CANCELLED_JOB_POSITION_JOB_POSITION_ID = "REQUIRED_CANCELLED_JOB_POSITION_JOB_POSITION_ID";

        [Description("Job position does not belong to the specified job")]
        public const string INVALID_CANCELLED_JOB_POSITION_JOB_AND_JOB_POSITION = "INVALID_CANCELLED_JOB_POSITION_JOB_AND_JOB_POSITION";

        [Description("A cancellation already exists for this job, job position, and employee")]
        public const string DUPLICATE_CANCELLED_JOB_POSITION = "DUPLICATE_CANCELLED_JOB_POSITION";

        [Description("Cancellation reason is required.")]
        public const string REQUIRED_CANCELLED_JOB_POSITION_REASON = "REQUIRED_CANCELLED_JOB_POSITION_REASON";

        [Description("QuestionaireResponseID is required when QuestionaireRequired is true")]
        public const string REQUIRED_CANCELLED_JOB_POSITION_QUESTIONAIRE_RESPONSE_ID = "REQUIRED_CANCELLED_JOB_POSITION_QUESTIONAIRE_RESPONSE_ID";

        [Description("WorkedHours must be greater than or equal to zero")]
        public const string INVALID_CANCELLED_JOB_POSITION_WORKED_HOURS = "INVALID_CANCELLED_JOB_POSITION_WORKED_HOURS";

        [Description("RequiredHours must be greater than or equal to zero")]
        public const string INVALID_CANCELLED_JOB_POSITION_REQUIRED_HOURS = "INVALID_CANCELLED_JOB_POSITION_REQUIRED_HOURS";

        [Description("Mileage must be greater than or equal to zero")]
        public const string INVALID_CANCELLED_JOB_POSITION_MILEAGE = "INVALID_CANCELLED_JOB_POSITION_MILEAGE";

        [Description("PayRate must be greater than or equal to zero")]
        public const string INVALID_CANCELLED_JOB_POSITION_PAY_RATE = "INVALID_CANCELLED_JOB_POSITION_PAY_RATE";

        [Description("ReimbursePayRate must be greater than or equal to zero")]
        public const string INVALID_CANCELLED_JOB_POSITION_REIMBURSE_PAY_RATE = "INVALID_CANCELLED_JOB_POSITION_REIMBURSE_PAY_RATE";

        [Description("CancelledDateTime cannot be in the future")]
        public const string INVALID_CANCELLED_JOB_POSITION_CANCELLED_DATE_TIME_FUTURE = "INVALID_CANCELLED_JOB_POSITION_CANCELLED_DATE_TIME_FUTURE";

        [Description("CancelledBy cannot exceed maximum length")]
        public const string MAXIMUM_LENGTH_CANCELLED_JOB_POSITION_CANCELLED_BY = "MAXIMUM_LENGTH_CANCELLED_JOB_POSITION_CANCELLED_BY";

        [Description("IPAddressCancelledBy cannot exceed maximum length")]
        public const string MAXIMUM_LENGTH_CANCELLED_JOB_POSITION_IP_ADDRESS_CANCELLED_BY = "MAXIMUM_LENGTH_CANCELLED_JOB_POSITION_IP_ADDRESS_CANCELLED_BY";

        [Description("ReimburseStatus cannot exceed maximum length")]
        public const string MAXIMUM_LENGTH_CANCELLED_JOB_POSITION_REIMBURSE_STATUS = "MAXIMUM_LENGTH_CANCELLED_JOB_POSITION_REIMBURSE_STATUS";

        [Description("You can only update your own cancelled job position record")]
        public const string CANCELLED_JOB_POSITION_UPDATE_OWN_RECORD_ONLY = "CANCELLED_JOB_POSITION_UPDATE_OWN_RECORD_ONLY";
        #endregion

        #region DepartmentAdminDto
        [Description("DepartmentID is required")]
        public const string REQUIRED_DEPARTMENT_ID = "REQUIRED_DEPARTMENT_ID";

        [Description("AddedByID is required")]
        public const string REQUIRED_ADDED_BY_ID = "REQUIRED_ADDED_BY_ID";

        [Description("EditDate is required")]
        public const string REQUIRED_EDIT_DATE = "REQUIRED_EDIT_DATE";

        [Description("NameOfUserAddingRecord cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_NAME_OF_USER_ADDING_RECORD = "MAXIMUM_LENGTH_NAME_OF_USER_ADDING_RECORD";

        [Description("UserNameEditingRecord cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_USER_NAME_EDITING_RECORD = "MAXIMUM_LENGTH_USER_NAME_EDITING_RECORD";

        [Description("Employee is already assigned as department admin for this department")]
        public const string DUPLICATE_DEPARTMENT_ADMIN = "DUPLICATE_DEPARTMENT_ADMIN";
        #endregion

        #region EmailTemplateDto
        [Description("Email template title is required")]
        public const string REQUIRED_EMAIL_TEMPLATE_TITLE = "REQUIRED_EMAIL_TEMPLATE_TITLE";

        [Description("Email template title cannot exceed 200 characters")]
        public const string MAXIMUM_LENGTH_EMAIL_TEMPLATE_TITLE = "MAXIMUM_LENGTH_EMAIL_TEMPLATE_TITLE";

        [Description("Email template subject cannot exceed 500 characters")]
        public const string MAXIMUM_LENGTH_EMAIL_TEMPLATE_SUBJECT = "MAXIMUM_LENGTH_EMAIL_TEMPLATE_SUBJECT";

        [Description("An email template with this title already exists")]
        public const string DUPLICATE_EMAIL_TEMPLATE_TITLE = "DUPLICATE_EMAIL_TEMPLATE_TITLE";
        #endregion

        #region SendMailRequestDto
        [Description("Email type is required")]
        public const string REQUIRED_SEND_MAIL_EMAIL_TYPE = "REQUIRED_SEND_MAIL_EMAIL_TYPE";

        [Description("At least one recipient email is required")]
        public const string REQUIRED_SEND_MAIL_TO_EMAILS = "REQUIRED_SEND_MAIL_TO_EMAILS";

        [Description("Invalid character in email address")]
        public const string INVALID_SEND_MAIL_EMAIL_FORMAT = "INVALID_SEND_MAIL_EMAIL_FORMAT";

        [Description("Recipient email cannot exceed 256 characters")]
        public const string MAXIMUM_LENGTH_SEND_MAIL_EMAIL = "MAXIMUM_LENGTH_SEND_MAIL_EMAIL";

        [Description("Recipient email cannot be empty")]
        public const string SEND_MAIL_EMAIL_CANNOT_BE_EMPTY = "SEND_MAIL_EMAIL_CANNOT_BE_EMPTY";
        #endregion

        #region Notification
        [Description("Request body is required.")]
        public const string REQUIRED_NOTIFICATION_REQUEST_BODY = "REQUIRED_NOTIFICATION_REQUEST_BODY";

        [Description("Subject is required.")]
        public const string REQUIRED_NOTIFICATION_SUBJECT = "REQUIRED_NOTIFICATION_SUBJECT";

        [Description("Body is required.")]
        public const string REQUIRED_NOTIFICATION_BODY = "REQUIRED_NOTIFICATION_BODY";

        [Description("At least one recipient in 'to' field is required.")]
        public const string REQUIRED_NOTIFICATION_TO_RECIPIENTS = "REQUIRED_NOTIFICATION_TO_RECIPIENTS";

        [Description("No valid email addresses provided in 'to'.")]
        public const string INVALID_NOTIFICATION_TO_RECIPIENTS = "INVALID_NOTIFICATION_TO_RECIPIENTS";

        [Description("Exactly one of requestId, postId, or postPositionId must be provided.")]
        public const string REQUIRED_NOTIFICATION_SINGLE_CONTEXT_ID = "REQUIRED_NOTIFICATION_SINGLE_CONTEXT_ID";

        [Description("Post position with ID {0} was not found.")]
        public const string NOTIFICATION_POST_POSITION_NOT_FOUND = "NOTIFICATION_POST_POSITION_NOT_FOUND";

        [Description("Unable to send email. Placeholder positions do not have an email address.")]
        public const string NOTIFICATION_POST_POSITION_IS_PLACEHOLDER = "NOTIFICATION_POST_POSITION_IS_PLACEHOLDER";

        [Description("Unable to send email. This position has no assigned employee.")]
        public const string NOTIFICATION_POST_POSITION_NO_EMPLOYEE = "NOTIFICATION_POST_POSITION_NO_EMPLOYEE";

        [Description("Post with ID {0} was not found.")]
        public const string NOTIFICATION_POST_NOT_FOUND = "NOTIFICATION_POST_NOT_FOUND";

        [Description("Request with ID {0} was not found.")]
        public const string NOTIFICATION_REQUEST_NOT_FOUND = "NOTIFICATION_REQUEST_NOT_FOUND";

        [Description("requestId and requesterId are required when attachmentFileIds is provided.")]
        public const string REQUIRED_NOTIFICATION_ATTACHMENT_CONTEXT = "REQUIRED_NOTIFICATION_ATTACHMENT_CONTEXT";

        [Description("Attachment with ID {0} was not found for the given request.")]
        public const string NOTIFICATION_ATTACHMENT_NOT_FOUND = "NOTIFICATION_ATTACHMENT_NOT_FOUND";

        [Description("Notification history with ID {0} was not found.")]
        public const string NOTIFICATION_HISTORY_NOT_FOUND = "NOTIFICATION_HISTORY_NOT_FOUND";
        #endregion

        #region Generic delete
        [Description("Please delete dependent records before deleting {0}.")]
        public const string DELETE_DEPENDENT_RECORDS_FIRST = "DELETE_DEPENDENT_RECORDS_FIRST";
        #endregion

        #region Exception / constraint detection (string literals for DbUpdateException handling)
        /// <summary>Exception type name used to detect EF Core update failures.</summary>
        public const string DbUpdateExceptionTypeName = "DbUpdateException";
        /// <summary>Message substring indicating a foreign key violation (case-insensitive).</summary>
        public const string ForeignKeyMessageSubstring = "foreign key";
        /// <summary>Message substring indicating a REFERENCE constraint violation (case-insensitive).</summary>
        public const string ReferenceConstraintMessageSubstring = "REFERENCE constraint";
        /// <summary>Default display name for department entity when name is empty.</summary>
        public const string DefaultDepartmentEntityName = "department";
        #endregion

        #region ApprovalDetail
        [Description("The routing group is not assigned to this request.")]
        public const string VEHICLE_APPROVAL_ROUTING_GROUP_NOT_ASSIGNED = "VEHICLE_APPROVAL_ROUTING_GROUP_NOT_ASSIGNED";

        [Description("Employee ID missing from token.")]
        public const string VEHICLE_APPROVAL_EMPLOYEE_ID_MISSING = "VEHICLE_APPROVAL_EMPLOYEE_ID_MISSING";

        [Description("You are not a member of this approval group.")]
        public const string VEHICLE_APPROVAL_NOT_MEMBER_OF_GROUP = "VEHICLE_APPROVAL_NOT_MEMBER_OF_GROUP";
        #endregion

        #region PermitApprovalRoutingDto
        [Description("Request ID is required")]
        public const string REQUIRED_PERMIT_APPROVAL_ROUTING_REQUEST_ID = "REQUIRED_PERMIT_APPROVAL_ROUTING_REQUEST_ID";

        [Description("Routing Group ID is required")]
        public const string REQUIRED_PERMIT_APPROVAL_ROUTING_ROUTING_GROUP_ID = "REQUIRED_PERMIT_APPROVAL_ROUTING_ROUTING_GROUP_ID";
        #endregion

        #region Invoice
        [Description("No invoice data found for the given criteria.")]
        public const string NO_INVOICE_DATA_FOUND = "NO_INVOICE_DATA_FOUND";

        [Description("Request '{0}' (Project number: {1}) ({2} - {3}) : No employees have taken any job.")]
        public const string NO_EMPLOYEES_JOB_IN_REQUEST = "NO_EMPLOYEES_JOB_IN_REQUEST";

        [Description("Request '{0}' (Project number: {1}) ({2} - {3}) : {4} closed the job with 0 hours.")]
        public const string EMPLOYEE_CLOSED_JOB_WITH_ZERO_HOURS = "EMPLOYEE_CLOSED_JOB_WITH_ZERO_HOURS";

        [Description("Request '{0}' (Project number: {1}) ({2} - {3}) : The few jobs are not closed.")]
        public const string JOB_NOT_CLOSED_FOR_DATE_RANGE = "JOB_NOT_CLOSED_FOR_DATE_RANGE";

        [Description("Request '{0} ({1} - {2})' : Invoice already exists for this date range.")]
        public const string INVOICE_ALREADY_EXISTS_FOR_DATE_RANGE = "INVOICE_ALREADY_EXISTS_FOR_DATE_RANGE";

        [Description("Request '{0} ({1} - {2})': An invoice cannot be created with a $0.00 total. Please review job hours, pay rates, and fees before generating.")]
        public const string INVOICE_TOTAL_MUST_BE_GREATER_THAN_ZERO = "INVOICE_TOTAL_MUST_BE_GREATER_THAN_ZERO";

        [Description("Invoice was not found or has been deleted.")]
        public const string INVOICE_NOT_FOUND_OR_DELETED = "INVOICE_NOT_FOUND_OR_DELETED";

        [Description("Invoice has no balance due.")]
        public const string INVOICE_NO_BALANCE_DUE = "INVOICE_NO_BALANCE_DUE";

        [Description("Requester does not match the invoice requester.")]
        public const string INVOICE_PAYMENT_REQUESTER_MISMATCH = "INVOICE_PAYMENT_REQUESTER_MISMATCH";

        [Description("RequesterID is required.")]
        public const string REQUIRED_INVOICE_PAYMENT_REQUESTER_ID = "REQUIRED_INVOICE_PAYMENT_REQUESTER_ID";

        [Description("Amount must be zero or greater.")]
        public const string INVALID_INVOICE_PAYMENT_AMOUNT = "INVALID_INVOICE_PAYMENT_AMOUNT";

        [Description("Amount is required and must be greater than zero.")]
        public const string REQUIRED_INVOICE_PAYMENT_AMOUNT = "REQUIRED_INVOICE_PAYMENT_AMOUNT";

        [Description("Settle up amount must be greater than zero.")]
        public const string INVALID_INVOICE_SETTLE_UP_AMOUNT = "INVALID_INVOICE_SETTLE_UP_AMOUNT";

        [Description("Settle up amount or payment amount is required when settling up balance.")]
        public const string REQUIRED_INVOICE_SETTLE_UP_AMOUNT = "REQUIRED_INVOICE_SETTLE_UP_AMOUNT";

        [Description("The allocation amount cannot exceed the invoice balance due.")]
        public const string INVOICE_PAYMENT_EXCEEDS_BALANCE_DUE = "INVOICE_PAYMENT_EXCEEDS_BALANCE_DUE";

        [Description("Payments totaling {0} have already been applied to this invoice. The new invoice total will be {1}, which is {2} less than the amount paid. Assign refund amounts in the table below for a combined total of {2} before you can continue.")]
        public const string INVOICE_UNCLOSE_REQUIRES_REFUND = "INVOICE_UNCLOSE_REQUIRES_REFUND";

        [Description("This action cannot be completed because invoice no. {0} would total $0.00. Delete invoice {0} first, then try again.")]
        public const string INVOICE_UNCLOSE_ZERO_TOTAL_REQUIRES_DELETE = "INVOICE_UNCLOSE_ZERO_TOTAL_REQUIRES_DELETE";

        [Description("Refund allocations total {0:N2} does not match the required refund amount {1:N2}.")]
        public const string INVOICE_REFUND_TOTAL_MISMATCH = "INVOICE_REFUND_TOTAL_MISMATCH";

        [Description("Refund amount {0:N2} exceeds net paid {1:N2} on this invoice for the selected payment.")]
        public const string INVOICE_REFUND_EXCEEDS_PAID_ON_PAYMENT = "INVOICE_REFUND_EXCEEDS_PAID_ON_PAYMENT";
        #endregion

        #region Transfer / Bulk Assign
        [Description("Request context is not available.")]
        public const string REQUEST_CONTEXT_NOT_AVAILABLE = "REQUEST_CONTEXT_NOT_AVAILABLE";

        [Description("Source job position was not found.")]
        public const string TRANSFER_SOURCE_JOB_POSITION_NOT_FOUND = "TRANSFER_SOURCE_JOB_POSITION_NOT_FOUND";

        [Description("Source job position does not belong to the requested job.")]
        public const string TRANSFER_SOURCE_JOB_MISMATCH = "TRANSFER_SOURCE_JOB_MISMATCH";

        [Description("Source job position date does not match dateOfWork.")]
        public const string TRANSFER_SOURCE_DATE_MISMATCH = "TRANSFER_SOURCE_DATE_MISMATCH";

        [Description("Source job position is not in a state that allows transfer.")]
        public const string TRANSFER_SOURCE_STATUS_INVALID = "TRANSFER_SOURCE_STATUS_INVALID";

        [Description("Transfer target must be a different employee than the current assignee.")]
        public const string TRANSFER_TARGET_SAME_AS_SOURCE = "TRANSFER_TARGET_SAME_AS_SOURCE";

        [Description("Time conflict requires confirmation.")]
        public const string TRANSFER_TIME_CONFLICT_REQUIRES_CONFIRMATION = "TRANSFER_TIME_CONFLICT_REQUIRES_CONFIRMATION";

        [Description("Assignment is not available for this employee.")]
        public const string TRANSFER_ASSIGNMENT_UNAVAILABLE = "TRANSFER_ASSIGNMENT_UNAVAILABLE";

        [Description("Job not found.")]
        public const string TRANSFER_JOB_NOT_FOUND = "TRANSFER_JOB_NOT_FOUND";

        [Description("Could not update source job position.")]
        public const string TRANSFER_SOURCE_UPDATE_FAILED = "TRANSFER_SOURCE_UPDATE_FAILED";

        [Description("{0}: job assigned successfully")]
        public const string TRANSFER_ASSIGN_SUCCESS = "TRANSFER_ASSIGN_SUCCESS";

        [Description("Source job position id is required when isTransfer is true.")]
        public const string TRANSFER_SOURCE_JOB_POSITION_REQUIRED = "TRANSFER_SOURCE_JOB_POSITION_REQUIRED";

        [Description("Transfer requires exactly one employee in employeeIDs.")]
        public const string TRANSFER_REQUIRES_SINGLE_EMPLOYEE = "TRANSFER_REQUIRES_SINGLE_EMPLOYEE";
        #endregion


    }
}
