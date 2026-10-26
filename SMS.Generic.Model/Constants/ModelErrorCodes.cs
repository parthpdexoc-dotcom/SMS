using System.ComponentModel;
using System.Reflection;

namespace SMS.Generic.Models.Constants
{
    public class ModelErrorCodes
    {
        #region Request/Post/PostPosition service errors
        [Description("Approver email not found for the provided RoutingGroupId.")]
        public const string REQUEST_APPROVER_EMAIL_NOT_FOUND = "REQUEST_APPROVER_EMAIL_NOT_FOUND";
        [Description("Request {0} not found.")]
        public const string REQUEST_ENTITY_NOT_FOUND_WITH_ID = "REQUEST_ENTITY_NOT_FOUND_WITH_ID";
        [Description("RoutingGroup {0} not found.")]
        public const string ROUTING_GROUP_NOT_FOUND_WITH_ID = "ROUTING_GROUP_NOT_FOUND_WITH_ID";
        [Description("Employee ID missing.")]
        public const string REQUEST_EMPLOYEE_ID_MISSING = "REQUEST_EMPLOYEE_ID_MISSING";
        [Description("Request not found.")]
        public const string REQUEST_NOT_FOUND_GENERIC = "REQUEST_NOT_FOUND_GENERIC";
        [Description("Request with ID {0} not found.")]
        public const string REQUEST_NOT_FOUND_WITH_ID = "REQUEST_NOT_FOUND_WITH_ID";
        [Description("Parent job with ID {0} not found.")]
        public const string JOB_PARENT_NOT_FOUND = "JOB_PARENT_NOT_FOUND";
        [Description("Cannot change the supervisor job setting while this job has child jobs. Remove or reassign the child jobs first.")]
        public const string JOB_SUPERVISOR_CANNOT_CHANGE_WITH_CHILD_JOBS = "JOB_SUPERVISOR_CANNOT_CHANGE_WITH_CHILD_JOBS";
        [Description("DTOs collection cannot be empty")]
        public const string JOB_POSITION_DTOS_EMPTY = "JOB_POSITION_DTOS_EMPTY";
        [Description("Job position id is required.")]
        public const string JOB_POSITION_ID_REQUIRED = "JOB_POSITION_ID_REQUIRED";
        [Description("Job position with ID {0} not found.")]
        public const string JOB_POSITION_NOT_FOUND_BY_ID = "JOB_POSITION_NOT_FOUND_BY_ID";
        [Description("Parent job position with ID {0} not found.")]
        public const string JOB_POSITION_PARENT_NOT_FOUND_BY_ID = "JOB_POSITION_PARENT_NOT_FOUND_BY_ID";
        [Description("Employee ID not found in token.")]
        public const string JOB_POSITION_EMPLOYEE_ID_NOT_FOUND_IN_TOKEN = "JOB_POSITION_EMPLOYEE_ID_NOT_FOUND_IN_TOKEN";
        [Description("Job position id is required")]
        public const string JOB_POSITION_UPDATE_CALLSIGN_JOB_POSITION_ID_REQUIRED = "JOB_POSITION_UPDATE_CALLSIGN_JOB_POSITION_ID_REQUIRED";
        [Description("fieldName is required")]
        public const string JOB_POSITION_UPDATE_CALLSIGN_FIELD_NAME_REQUIRED = "JOB_POSITION_UPDATE_CALLSIGN_FIELD_NAME_REQUIRED";
        [Description("Invalid fieldName '{0}'.")]
        public const string JOB_POSITION_UPDATE_CALLSIGN_INVALID_FIELD_NAME = "JOB_POSITION_UPDATE_CALLSIGN_INVALID_FIELD_NAME";
        [Description("Property '{0}' was not found on '{1}'.")]
        public const string JOB_POSITION_PROPERTY_NOT_FOUND = "JOB_POSITION_PROPERTY_NOT_FOUND";
        [Description("Property '{0}' on '{1}' is not writable.")]
        public const string JOB_POSITION_PROPERTY_NOT_WRITABLE = "JOB_POSITION_PROPERTY_NOT_WRITABLE";
        [Description("Property '{0}' on '{1}' is not a string.")]
        public const string JOB_POSITION_PROPERTY_NOT_STRING = "JOB_POSITION_PROPERTY_NOT_STRING";
        [Description("DateOfWork is required.")]
        public const string JOB_POSITION_DATE_OF_WORK_REQUIRED = "JOB_POSITION_DATE_OF_WORK_REQUIRED";
        [Description("Invalid job ID.")]
        public const string JOB_POSITION_INVALID_JOB_ID = "JOB_POSITION_INVALID_JOB_ID";
        [Description("Request not found for job.")]
        public const string JOB_POSITION_REQUEST_NOT_FOUND_FOR_JOB = "JOB_POSITION_REQUEST_NOT_FOUND_FOR_JOB";
        [Description("DateOfWork is not valid for this Request.")]
        public const string JOB_POSITION_DATE_OF_WORK_INVALID_FOR_REQUEST = "JOB_POSITION_DATE_OF_WORK_INVALID_FOR_REQUEST";
        [Description("Job with ID {0} not found.")]
        public const string JOB_NOT_FOUND_BY_ID = "JOB_NOT_FOUND_BY_ID";
        [Description("Job {0} does not have an associated request.")]
        public const string JOB_ASSOCIATED_REQUEST_NOT_FOUND = "JOB_ASSOCIATED_REQUEST_NOT_FOUND";
        [Description("No available dates found for this job. All positions may be filled or dates may be in the past.")]
        public const string JOB_POSITION_NO_AVAILABLE_DATES = "JOB_POSITION_NO_AVAILABLE_DATES";
        [Description("Job ID is required")]
        public const string JOB_POSITION_ASSIGN_JOB_ID_REQUIRED = "JOB_POSITION_ASSIGN_JOB_ID_REQUIRED";
        [Description("At least one EmployeeID is required")]
        public const string JOB_POSITION_ASSIGN_EMPLOYEE_IDS_REQUIRED = "JOB_POSITION_ASSIGN_EMPLOYEE_IDS_REQUIRED";
        [Description("At least one valid date is required")]
        public const string JOB_POSITION_ASSIGN_VALID_DATES_REQUIRED = "JOB_POSITION_ASSIGN_VALID_DATES_REQUIRED";
        [Description("Job {0} has invalid total positions value: {1}")]
        public const string JOB_POSITION_INVALID_TOTAL_JOB_POSITIONS = "JOB_POSITION_INVALID_TOTAL_JOB_POSITIONS";
        [Description("This job is already assigned to you.")]
        public const string JOB_ALREADY_ASSIGNED_TO_YOU = "JOB_ALREADY_ASSIGNED_TO_YOU";
        [Description("time ended")]
        public const string JOB_TIME_ENDED = "JOB_TIME_ENDED";
        [Description("No available slots.")]
        public const string JOB_NO_AVAILABLE_SLOTS = "JOB_NO_AVAILABLE_SLOTS";
        [Description("No waiting list slots available for this job.")]
        public const string JOB_WAITING_LIST_FULL = "JOB_WAITING_LIST_FULL";
        [Description("Waiting list is not enabled for this request. Please switch to regular job view.")]
        public const string JOB_WAITING_LIST_NOT_ENABLED_FOR_REQUEST = "JOB_WAITING_LIST_NOT_ENABLED_FOR_REQUEST";
        [Description("Waiting list is only available after all regular job slots are full.")]
        public const string JOB_WAITING_LIST_AVAILABLE_ONLY_WHEN_SLOTS_FULL = "JOB_WAITING_LIST_AVAILABLE_ONLY_WHEN_SLOTS_FULL";
        [Description("Waiting list view is not available for special event requests.")]
        public const string JOB_WAITING_LIST_NOT_SUPPORTED_FOR_SPECIAL_EVENT = "JOB_WAITING_LIST_NOT_SUPPORTED_FOR_SPECIAL_EVENT";
        [Description("This job is not part of the selected special event request.")]
        public const string JOB_NOT_INCLUDED_IN_SELECTED_SPECIAL_EVENT_REQUEST = "JOB_NOT_INCLUDED_IN_SELECTED_SPECIAL_EVENT_REQUEST";
        [Description("{4} has a time conflict with an existing assignment from {2} to {3} (overlap: {0} hours {1} minutes). Please review and confirm to proceed.")]
        public const string JOB_TIME_CONFLICT_WITH_TAKEN_POSITIONS = "JOB_TIME_CONFLICT_WITH_TAKEN_POSITIONS";
        [Description("{3} already has a job assigned on {0} from {1} to {2}. Please choose a different job time or confirm to proceed.")]
        public const string JOB_SAME_TIME_SLOT_ALREADY_TAKEN = "JOB_SAME_TIME_SLOT_ALREADY_TAKEN";
        [Description("Overlap conflict confirmation is not enabled for this department/request policy.")]
        public const string JOB_OVERLAP_CONFLICT_CONFIRMATION_NOT_ENABLED = "JOB_OVERLAP_CONFLICT_CONFIRMATION_NOT_ENABLED";
        [Description("Overlap conflict confirmation is not enabled for this department/request policy. Cannot proceed for {0}.")]
        public const string JOB_OVERLAP_CONFLICT_CONFIRMATION_NOT_ENABLED_FOR_EMPLOYEE = "JOB_OVERLAP_CONFLICT_CONFIRMATION_NOT_ENABLED_FOR_EMPLOYEE";
        [Description("Time conflict. Please review and confirm to proceed.")]
        public const string BULK_ASSIGN_TIME_CONFLICT_DIALOG_DEFAULT = "BULK_ASSIGN_TIME_CONFLICT_DIALOG_DEFAULT";
        [Description("{0} already has a job assigned on {1} from {2} to {3}. Please choose a different job time or confirm to proceed.")]
        public const string BULK_ASSIGN_SAME_TIME_SLOT_EMPLOYEE = "BULK_ASSIGN_SAME_TIME_SLOT_EMPLOYEE";
        [Description("You have outstanding jobs ready to close. Please close them to apply for a new job.")]
        public const string REQUEST_OUTSTANDING_READY_TO_CLOSE_JOB = "REQUEST_OUTSTANDING_READY_TO_CLOSE_JOB";
        [Description("{0} has outstanding jobs ready to close. Please close them to apply the new job.")]
        public const string REQUEST_OUTSTANDING_READY_TO_CLOSE_JOB_FOR_EMPLOYEE = "REQUEST_OUTSTANDING_READY_TO_CLOSE_JOB_FOR_EMPLOYEE";
        [Description("You have applied the maximum number of jobs today")]
        public const string REQUEST_MAX_JOBS_APPLIED_TODAY = "REQUEST_MAX_JOBS_APPLIED_TODAY";
        [Description("{0} has applied the maximum number of jobs today")]
        public const string REQUEST_MAX_JOBS_APPLIED_TODAY_FOR_EMPLOYEE = "REQUEST_MAX_JOBS_APPLIED_TODAY_FOR_EMPLOYEE";
        [Description("Employee is in Different Department.")]
        public const string REQUEST_EMPLOYEE_DIFFERENT_DEPARTMENT = "REQUEST_EMPLOYEE_DIFFERENT_DEPARTMENT";
        [Description("{0} is in a different department.")]
        public const string REQUEST_EMPLOYEE_DIFFERENT_DEPARTMENT_FOR_EMPLOYEE = "REQUEST_EMPLOYEE_DIFFERENT_DEPARTMENT_FOR_EMPLOYEE";
        [Description("{0} does not meet the release criteria for {1} request.")]
        public const string REQUEST_RELEASE_CRITERIA_NOT_MET = "REQUEST_RELEASE_CRITERIA_NOT_MET";
        [Description("No valid dates found.")]
        public const string REQUEST_NO_VALID_DATES_FOUND = "REQUEST_NO_VALID_DATES_FOUND";
        [Description("No open jobs were found for the selected date range.")]
        public const string OPEN_JOBS_DASHBOARD_NO_RECORDS = "OPEN_JOBS_DASHBOARD_NO_RECORDS";
        [Description("Tenure express-interest dashboard is only available for tenure-based requests.")]
        public const string TENURE_EXPRESS_INTEREST_NOT_TENURE_RELEASE = "TENURE_EXPRESS_INTEREST_NOT_TENURE_RELEASE";
        [Description("Employee does not belong to the current department.")]
        public const string TENURE_WAITING_SINGLE_EMPLOYEE_NOT_IN_DEPARTMENT = "TENURE_WAITING_SINGLE_EMPLOYEE_NOT_IN_DEPARTMENT";
        [Description("Request was not found for this department or cannot be loaded for tenure processing.")]
        public const string TENURE_WAITING_SINGLE_EMPLOYEE_REQUEST_CONTEXT_MISSING = "TENURE_WAITING_SINGLE_EMPLOYEE_REQUEST_CONTEXT_MISSING";
        [Description("This employee has no pending tenure waiting row for this request.")]
        public const string TENURE_WAITING_SINGLE_EMPLOYEE_NO_PENDING_ROW = "TENURE_WAITING_SINGLE_EMPLOYEE_NO_PENDING_ROW";
        [Description("{0} does not have required skills for this request")]
        public const string REQUEST_REQUIRED_SKILLS_NOT_MET = "REQUEST_REQUIRED_SKILLS_NOT_MET";
        [Description("{0} id blocked.")]
        public const string REQUEST_JOB_ID_BLOCKED = "REQUEST_JOB_ID_BLOCKED";
        [Description("{0} Restricted By Requester.")]
        public const string REQUEST_EMPLOYEE_RESTRICTED_BY_REQUESTER = "REQUEST_EMPLOYEE_RESTRICTED_BY_REQUESTER";
        [Description("Job ID is required to create a waiting job position.")]
        public const string WAITING_JOB_POSITION_JOB_ID_REQUIRED = "WAITING_JOB_POSITION_JOB_ID_REQUIRED";
        [Description("PostID is required to create WaitingPostPositionDto.")]
        public const string WAITING_POST_POSITION_POST_ID_REQUIRED = "WAITING_POST_POSITION_POST_ID_REQUIRED";
        [Description("This request is not a special job request.")]
        public const string WAITING_POST_POSITION_NOT_SPECIAL_EVENT_REQUEST = "WAITING_POST_POSITION_NOT_SPECIAL_EVENT_REQUEST";
        [Description("You already applied for this special job on the selected date.")]
        public const string WAITING_POST_POSITION_SPECIAL_EVENT_ALREADY_APPLIED = "WAITING_POST_POSITION_SPECIAL_EVENT_ALREADY_APPLIED";
        [Description("There are available positions for this Job. Since this position was cancelled by another employee, available positions will be assigned first. Would you like to take one of the available positions instead?")]
        public const string JOB_CANCEL_PENDING_SLOTS_AVAILABLE = "JOB_CANCEL_PENDING_SLOTS_AVAILABLE";
        [Description("Special job application saved.")]
        public const string SPECIALJOBSUCCESSMESSAGE = "SPECIALJOBSUCCESSMESSAGE";
        [Description("There are {0} positions available. Assignments vary in locations and hours. You are signing up to work at the event, you cannot select a specific job/post assignment. The actual job/post location assignments will be assigned by the Department Administrator closer to the event date. You will be notified of your final assignment at that time.")]
        public const string VIEW_DETAILS_SPECIAL_EVENT_JOB_DETAILS = "VIEW_DETAILS_SPECIAL_EVENT_JOB_DETAILS";
        [Description("Cancellation can only be revoked when the post position status is CancellationPending.")]
        public const string CANCELLATION_ONLY_WHEN_CANCELLATION_PENDING = "CANCELLATION_ONLY_WHEN_CANCELLATION_PENDING";

        [Description("<b>Yes</b>")]
        public const string POST_SCHEDULE_EMAIL_BOLD_YES = "POST_SCHEDULE_EMAIL_BOLD_YES";

        [Description("<b>No</b>")]
        public const string POST_SCHEDULE_EMAIL_BOLD_NO = "POST_SCHEDULE_EMAIL_BOLD_NO";

        [Description("Vehicle Required changed from {0} to {1}. {2}")]
        public const string POST_SCHEDULE_EMAIL_VEHICLE_REQUIRED_CHANGE = "POST_SCHEDULE_EMAIL_VEHICLE_REQUIRED_CHANGE";

        [Description("Vehicle assignment(s) changed from: {0} to: {1}")]
        public const string POST_SCHEDULE_EMAIL_VEHICLE_ASSIGNMENTS_CHANGED = "POST_SCHEDULE_EMAIL_VEHICLE_ASSIGNMENTS_CHANGED";

        [Description("The following vehicle assignment(s) were removed:")]
        public const string POST_SCHEDULE_EMAIL_VEHICLE_REMOVED_SUFFIX = "POST_SCHEDULE_EMAIL_VEHICLE_REMOVED_SUFFIX";

        [Description("The following vehicle(s) are assigned:")]
        public const string POST_SCHEDULE_EMAIL_VEHICLE_ASSIGNED_SUFFIX = "POST_SCHEDULE_EMAIL_VEHICLE_ASSIGNED_SUFFIX";

        [Description("Vehicle assignment(s) unchanged.")]
        public const string POST_SCHEDULE_EMAIL_VEHICLE_ASSIGNMENTS_UNCHANGED = "POST_SCHEDULE_EMAIL_VEHICLE_ASSIGNMENTS_UNCHANGED";
        #endregion

        #region Generic service/repository runtime errors
        [Description("Includes array cannot be empty")]
        public const string GENERIC_INCLUDES_ARRAY_EMPTY = "GENERIC_INCLUDES_ARRAY_EMPTY";
        [Description("Id cannot be empty")]
        public const string GENERIC_ID_CANNOT_BE_EMPTY = "GENERIC_ID_CANNOT_BE_EMPTY";
        [Description("Id cannot be null or empty")]
        public const string GENERIC_ID_NULL_OR_EMPTY = "GENERIC_ID_NULL_OR_EMPTY";
        [Description("Key1 cannot be null or empty")]
        public const string GENERIC_KEY1_NULL_OR_EMPTY = "GENERIC_KEY1_NULL_OR_EMPTY";
        [Description("Key1PropertyName cannot be null or empty")]
        public const string GENERIC_KEY1_PROPERTY_NAME_NULL_OR_EMPTY = "GENERIC_KEY1_PROPERTY_NAME_NULL_OR_EMPTY";
        [Description("Key2 cannot be null or empty")]
        public const string GENERIC_KEY2_NULL_OR_EMPTY = "GENERIC_KEY2_NULL_OR_EMPTY";
        [Description("Key2PropertyName cannot be null or empty")]
        public const string GENERIC_KEY2_PROPERTY_NAME_NULL_OR_EMPTY = "GENERIC_KEY2_PROPERTY_NAME_NULL_OR_EMPTY";
        [Description("Service {0} does not have a GetByIdAsync(Guid) method.")]
        public const string GENERIC_SERVICE_GETBYID_METHOD_MISSING = "GENERIC_SERVICE_GETBYID_METHOD_MISSING";
        [Description("Different department. You cannot edit this department.")]
        public const string GENERIC_DIFFERENT_DEPARTMENT_CANNOT_EDIT = "GENERIC_DIFFERENT_DEPARTMENT_CANNOT_EDIT";
        [Description("Department ID missing from token.")]
        public const string GENERIC_DEPARTMENT_ID_MISSING_FROM_TOKEN = "GENERIC_DEPARTMENT_ID_MISSING_FROM_TOKEN";
        [Description("Employee ID missing from token.")]
        public const string GENERIC_EMPLOYEE_ID_MISSING_FROM_TOKEN = "GENERIC_EMPLOYEE_ID_MISSING_FROM_TOKEN";
        [Description("Error message cannot be null or empty")]
        public const string GENERIC_ERROR_MESSAGE_NULL_OR_EMPTY = "GENERIC_ERROR_MESSAGE_NULL_OR_EMPTY";
        [Description("Cannot specify both isDepartmentScoped and isEmployeeScoped as true. Use isDepartmentAndEmployeeScoped instead.")]
        public const string GENERIC_INVALID_SCOPE_FLAGS = "GENERIC_INVALID_SCOPE_FLAGS";
        [Description("Cannot combine individual scoping with combined scoping.")]
        public const string GENERIC_SCOPE_FLAGS_CONFLICT = "GENERIC_SCOPE_FLAGS_CONFLICT";
        [Description("PropertyName cannot be null or empty")]
        public const string GENERIC_PROPERTY_NAME_NULL_OR_EMPTY = "GENERIC_PROPERTY_NAME_NULL_OR_EMPTY";
        [Description("Value cannot be null")]
        public const string GENERIC_VALUE_NULL = "GENERIC_VALUE_NULL";
        [Description("{0} with ID {1} not found.")]
        public const string GENERIC_ENTITY_WITH_ID_NOT_FOUND = "GENERIC_ENTITY_WITH_ID_NOT_FOUND";
        [Description("Employee information is missing.")]
        public const string GENERIC_EMPLOYEE_INFO_MISSING = "GENERIC_EMPLOYEE_INFO_MISSING";
        [Description("DTOs collection cannot be empty")]
        public const string GENERIC_DTOS_COLLECTION_EMPTY = "GENERIC_DTOS_COLLECTION_EMPTY";
        [Description("OldKey cannot be null or empty")]
        public const string GENERIC_OLD_KEY_NULL_OR_EMPTY = "GENERIC_OLD_KEY_NULL_OR_EMPTY";
        [Description("NewKey cannot be null or empty")]
        public const string GENERIC_NEW_KEY_NULL_OR_EMPTY = "GENERIC_NEW_KEY_NULL_OR_EMPTY";
        [Description("Ids collection cannot be empty")]
        public const string GENERIC_IDS_COLLECTION_EMPTY = "GENERIC_IDS_COLLECTION_EMPTY";
        [Description("ClientDtos collection cannot be empty")]
        public const string GENERIC_CLIENT_DTOS_COLLECTION_EMPTY = "GENERIC_CLIENT_DTOS_COLLECTION_EMPTY";
        [Description("CompositeKeys collection cannot be empty")]
        public const string GENERIC_COMPOSITE_KEYS_COLLECTION_EMPTY = "GENERIC_COMPOSITE_KEYS_COLLECTION_EMPTY";
        [Description("TimeZoneId is required.")]
        public const string GENERIC_TIMEZONE_ID_REQUIRED = "GENERIC_TIMEZONE_ID_REQUIRED";
        [Description("Key property name cannot be null or empty")]
        public const string REPOSITORY_KEY_PROPERTY_NAME_NULL_OR_EMPTY = "REPOSITORY_KEY_PROPERTY_NAME_NULL_OR_EMPTY";
        [Description("Property '{0}' not found on type '{1}'.")]
        public const string REPOSITORY_PROPERTY_NOT_FOUND_ON_TYPE = "REPOSITORY_PROPERTY_NOT_FOUND_ON_TYPE";
        [Description("Entities collection cannot be empty")]
        public const string REPOSITORY_ENTITIES_COLLECTION_EMPTY = "REPOSITORY_ENTITIES_COLLECTION_EMPTY";
        [Description("Key values collection cannot be empty")]
        public const string REPOSITORY_KEY_VALUES_COLLECTION_EMPTY = "REPOSITORY_KEY_VALUES_COLLECTION_EMPTY";
        [Description("No primary key defined for this entity.")]
        public const string REPOSITORY_NO_PRIMARY_KEY_DEFINED = "REPOSITORY_NO_PRIMARY_KEY_DEFINED";
        [Description("The entities collection cannot be empty.")]
        public const string REPOSITORY_ENTITIES_COLLECTION_EMPTY_WITH_ARTICLE = "REPOSITORY_ENTITIES_COLLECTION_EMPTY_WITH_ARTICLE";
        [Description("At least one key property must be specified.")]
        public const string REPOSITORY_AT_LEAST_ONE_KEY_PROPERTY_REQUIRED = "REPOSITORY_AT_LEAST_ONE_KEY_PROPERTY_REQUIRED";
        [Description("Composite keys cannot be null or empty.")]
        public const string REPOSITORY_COMPOSITE_KEYS_NULL_OR_EMPTY = "REPOSITORY_COMPOSITE_KEYS_NULL_OR_EMPTY";
        [Description("JWT SecretKey is not configured.")]
        public const string AUTH_JWT_SECRET_NOT_CONFIGURED = "AUTH_JWT_SECRET_NOT_CONFIGURED";
        [Description("A valid employee id is required.")]
        public const string VALID_EMPLOYEE_ID_REQUIRED = "VALID_EMPLOYEE_ID_REQUIRED";
        [Description("Employee ID not found in token.")]
        public const string EMPLOYEE_ID_NOT_FOUND_IN_TOKEN = "EMPLOYEE_ID_NOT_FOUND_IN_TOKEN";
        [Description("You are not authorized to update this waiting job position.")]
        public const string WAITING_JOB_POSITION_UPDATE_UNAUTHORIZED = "WAITING_JOB_POSITION_UPDATE_UNAUTHORIZED";
        [Description("WaitingPostPosition with ID {0} not found.")]
        public const string WAITING_JOB_POSITION_NOT_FOUND_BY_ID = "WAITING_JOB_POSITION_NOT_FOUND_BY_ID";
        [Description("Id cannot be empty")]
        public const string ID_CANNOT_BE_EMPTY = "ID_CANNOT_BE_EMPTY";
        [Description("ClosedPostPositionId cannot be empty.")]
        public const string CLOSED_JOB_POSITION_ID_CANNOT_BE_EMPTY = "CLOSED_JOB_POSITION_ID_CANNOT_BE_EMPTY";
        [Description("SourceClosedPostPositionId cannot be empty.")]
        public const string SOURCE_CLOSED_JOB_POSITION_ID_CANNOT_BE_EMPTY = "SOURCE_CLOSED_JOB_POSITION_ID_CANNOT_BE_EMPTY";
        [Description("NewClosedPostPositionId cannot be empty.")]
        public const string NEW_CLOSED_JOB_POSITION_ID_CANNOT_BE_EMPTY = "NEW_CLOSED_JOB_POSITION_ID_CANNOT_BE_EMPTY";
        [Description("ParentID is required when IsChild is true.")]
        public const string REQUESTER_PARENT_ID_REQUIRED_WHEN_CHILD = "REQUESTER_PARENT_ID_REQUIRED_WHEN_CHILD";
        [Description("LiquorLicense is required when IsLiquorLicenseRequired is true.")]
        public const string REQUESTER_LIQUOR_LICENSE_REQUIRED = "REQUESTER_LIQUOR_LICENSE_REQUIRED";
        [Description("At least one file is required.")]
        public const string UPLOAD_AT_LEAST_ONE_FILE_REQUIRED = "UPLOAD_AT_LEAST_ONE_FILE_REQUIRED";
        [Description("Parent entity with ID '{0}' not found.")]
        public const string UPLOAD_PARENT_ENTITY_NOT_FOUND = "UPLOAD_PARENT_ENTITY_NOT_FOUND";
        [Description("One or more files are empty.")]
        public const string UPLOAD_FILES_EMPTY = "UPLOAD_FILES_EMPTY";
        [Description("File '{0}' exceeds the maximum allowed size of {1} MB.")]
        public const string UPLOAD_FILE_SIZE_EXCEEDED = "UPLOAD_FILE_SIZE_EXCEEDED";
        [Description("A file of type '.{0}' already exists for this request. Only one file per type is allowed.")]
        public const string UPLOAD_DUPLICATE_FILE_TYPE = "UPLOAD_DUPLICATE_FILE_TYPE";
        [Description("Logo file is required and must not be empty.")]
        public const string UPLOAD_LOGO_REQUIRED = "UPLOAD_LOGO_REQUIRED";
        [Description("Logo exceeds the maximum allowed size of {0} MB.")]
        public const string UPLOAD_LOGO_SIZE_EXCEEDED = "UPLOAD_LOGO_SIZE_EXCEEDED";
        [Description("Logo must be an image (jpg, jpeg, png, gif, webp). Received: {0}.")]
        public const string UPLOAD_LOGO_INVALID_TYPE = "UPLOAD_LOGO_INVALID_TYPE";
        [Description("Profile photo file is required and must not be empty.")]
        public const string UPLOAD_PROFILE_PHOTO_REQUIRED = "UPLOAD_PROFILE_PHOTO_REQUIRED";
        [Description("Photo exceeds the maximum allowed size of {0} MB.")]
        public const string UPLOAD_PHOTO_SIZE_EXCEEDED = "UPLOAD_PHOTO_SIZE_EXCEEDED";
        [Description("Photo must be an image (jpg, jpeg, png, gif, webp). Received: {0}.")]
        public const string UPLOAD_PHOTO_INVALID_TYPE = "UPLOAD_PHOTO_INVALID_TYPE";
        [Description("Request with ID '{0}' not found or has no file folder.")]
        public const string UPLOAD_REQUEST_NOT_FOUND_OR_NO_FOLDER = "UPLOAD_REQUEST_NOT_FOUND_OR_NO_FOLDER";
        [Description("File with ID '{0}' not found for request '{1}'.")]
        public const string UPLOAD_FILE_NOT_FOUND_FOR_REQUEST = "UPLOAD_FILE_NOT_FOUND_FOR_REQUEST";
        [Description("Id must be greater than zero")]
        public const string GENERIC_ID_MUST_BE_GREATER_THAN_ZERO = "GENERIC_ID_MUST_BE_GREATER_THAN_ZERO";
        [Description("ExportID must be greater than zero")]
        public const string GENERIC_EXPORT_ID_MUST_BE_GREATER_THAN_ZERO = "GENERIC_EXPORT_ID_MUST_BE_GREATER_THAN_ZERO";
        [Description("FitStandardID must be greater than zero")]
        public const string GENERIC_FIT_STANDARD_ID_MUST_BE_GREATER_THAN_ZERO = "GENERIC_FIT_STANDARD_ID_MUST_BE_GREATER_THAN_ZERO";
        [Description("Department code, username, and password are required.")]
        public const string AUTH_DEPARTMENT_CODE_USERNAME_PASSWORD_REQUIRED = "AUTH_DEPARTMENT_CODE_USERNAME_PASSWORD_REQUIRED";
        [Description("Department code and username are required.")]
        public const string AUTH_DEPARTMENT_CODE_USERNAME_REQUIRED = "AUTH_DEPARTMENT_CODE_USERNAME_REQUIRED";
        [Description("Invalid department code, username, or password.")]
        public const string AUTH_INVALID_DEPARTMENT_CODE_USERNAME_PASSWORD = "AUTH_INVALID_DEPARTMENT_CODE_USERNAME_PASSWORD";
        [Description("Invalid username or password.")]
        public const string AUTH_INVALID_USERNAME_OR_PASSWORD = "AUTH_INVALID_USERNAME_OR_PASSWORD";
        [Description("You are not allowed to login.")]
        public const string AUTH_LOGIN_NOT_ALLOWED = "AUTH_LOGIN_NOT_ALLOWED";
        [Description("This account can sign in only from the mobile app.")]
        public const string AUTH_WEB_LOGIN_REQUIRES_ADMIN_TYPE = "AUTH_WEB_LOGIN_REQUIRES_ADMIN_TYPE";
        [Description("You were signed in on another device. This session has ended.")]
        public const string SESSION_REPLACED = "SESSION_REPLACED";
        [Description("Unable to sign you in. Please try again.")]
        public const string AUTH_SESSION_START_FAILED = "AUTH_SESSION_START_FAILED";
        [Description("Your session could not be verified. Please sign in again.")]
        public const string AUTH_EMPLOYEE_SESSION_COULD_NOT_BE_VERIFIED = "AUTH_EMPLOYEE_SESSION_COULD_NOT_BE_VERIFIED";
        [Description("Unauthorized. Your session has expired. Please sign in again.")]
        public const string UNAUTHORIZED = "UNAUTHORIZED";
        [Description("Request is required.")]
        public const string AUTH_REQUEST_REQUIRED = "AUTH_REQUEST_REQUIRED";
        [Description("Provide both OTP and NewPassword together.")]
        public const string AUTH_OTP_AND_NEW_PASSWORD_TOGETHER_REQUIRED = "AUTH_OTP_AND_NEW_PASSWORD_TOGETHER_REQUIRED";
        [Description("OTP is required.")]
        public const string AUTH_OTP_REQUIRED = "AUTH_OTP_REQUIRED";
        [Description("Invalid or expired OTP.")]
        public const string AUTH_INVALID_OR_EXPIRED_OTP = "AUTH_INVALID_OR_EXPIRED_OTP";
        [Description("OTP and NewPassword are required.")]
        public const string AUTH_OTP_AND_NEW_PASSWORD_REQUIRED = "AUTH_OTP_AND_NEW_PASSWORD_REQUIRED";
        [Description("Email is required.")]
        public const string AUTH_EMAIL_REQUIRED = "AUTH_EMAIL_REQUIRED";
        [Description("No account found with this username.")]
        public const string AUTH_NO_ACCOUNT_FOUND_WITH_USERNAME = "AUTH_NO_ACCOUNT_FOUND_WITH_USERNAME";
        [Description("Employee ID missing.")]
        public const string WAITING_JOB_POSITION_EMPLOYEE_ID_MISSING = "WAITING_JOB_POSITION_EMPLOYEE_ID_MISSING";
        [Description("Job not found.")]
        public const string WAITING_JOB_POSITION_JOB_NOT_FOUND = "WAITING_JOB_POSITION_JOB_NOT_FOUND";
        [Description("Token is required.")]
        public const string TOKEN_REQUIRED = "TOKEN_REQUIRED";
        [Description("Token has expired.")]
        public const string TOKEN_EXPIRED = "TOKEN_EXPIRED";
        [Description("Token signature is invalid.")]
        public const string TOKEN_SIGNATURE_INVALID = "TOKEN_SIGNATURE_INVALID";
        [Description("OTP has been sent to your email.")]
        public const string AUTH_OTP_SENT_TO_EMAIL = "AUTH_OTP_SENT_TO_EMAIL";
        [Description("OTP verified successfully.")]
        public const string AUTH_OTP_VERIFIED_SUCCESS = "AUTH_OTP_VERIFIED_SUCCESS";
        [Description("Your password has been reset successfully.")]
        public const string AUTH_PASSWORD_RESET_SUCCESS = "AUTH_PASSWORD_RESET_SUCCESS";

        [Description("SMS")]
        public const string JWT_DEFAULT_ISSUER = "JWT_DEFAULT_ISSUER";

        [Description("SMS-Users")]
        public const string JWT_DEFAULT_AUDIENCE = "JWT_DEFAULT_AUDIENCE";

        [Description("Bearer")]
        public const string JWT_ACCESS_TOKEN_TYPE_BEARER = "JWT_ACCESS_TOKEN_TYPE_BEARER";

        [Description("Login was rejected because department code, username, or password was not provided.")]
        public const string LOG_AUTH_LOGIN_CREDENTIALS_EMPTY = "LOG_AUTH_LOGIN_CREDENTIALS_EMPTY";

        [Description("Login was rejected: no employee matched the department code and username.")]
        public const string LOG_AUTH_LOGIN_NO_MATCHING_EMPLOYEE = "LOG_AUTH_LOGIN_NO_MATCHING_EMPLOYEE";

        [Description("Login was rejected: password verification failed.")]
        public const string LOG_AUTH_LOGIN_PASSWORD_VERIFICATION_FAILED = "LOG_AUTH_LOGIN_PASSWORD_VERIFICATION_FAILED";

        [Description("Login was rejected: the account is a system (master) user and cannot sign in through this portal.")]
        public const string LOG_AUTH_LOGIN_MASTER_ACCOUNT_FORBIDDEN = "LOG_AUTH_LOGIN_MASTER_ACCOUNT_FORBIDDEN";

        [Description("Login was rejected: the employee account is not approved for login.")]
        public const string LOG_AUTH_LOGIN_ACCOUNT_NOT_APPROVED = "LOG_AUTH_LOGIN_ACCOUNT_NOT_APPROVED";

        [Description("Login was rejected: web login was blocked because the employee has no admin type.")]
        public const string LOG_AUTH_LOGIN_WEB_WITHOUT_ADMIN_TYPE = "LOG_AUTH_LOGIN_WEB_WITHOUT_ADMIN_TYPE";

        [Description("Department configuration could not be loaded while building the login token; default flags were used.")]
        public const string LOG_AUTH_JWT_DEPT_CONFIG_LOAD_FAILED = "LOG_AUTH_JWT_DEPT_CONFIG_LOAD_FAILED";

        [Description("Available job release tab tier could not be resolved for the login token; the default tier was used.")]
        public const string LOG_AUTH_JWT_RELEASE_TAB_TIER_DEFAULTED = "LOG_AUTH_JWT_RELEASE_TAB_TIER_DEFAULTED";

        [Description("Profile photo presence could not be determined for employee ID {0} while building the login token; the JWT isLogo claim defaults to false.")]
        public const string LOG_AUTH_JWT_IS_LOGO_CHECK_FAILED = "LOG_AUTH_JWT_IS_LOGO_CHECK_FAILED";

        [Description("Employee authenticated successfully.")]
        public const string LOG_AUTH_EMPLOYEE_AUTHENTICATED_SUCCESS = "LOG_AUTH_EMPLOYEE_AUTHENTICATED_SUCCESS";

        [Description("JWT token validation failed.")]
        public const string LOG_AUTH_TOKEN_VALIDATION_FAILED = "LOG_AUTH_TOKEN_VALIDATION_FAILED";

        [Description("Forgot password: no employee account matched the email address provided.")]
        public const string LOG_AUTH_FORGOT_PASSWORD_EMAIL_NOT_FOUND = "LOG_AUTH_FORGOT_PASSWORD_EMAIL_NOT_FOUND";

        [Description("Password reset email was not sent because the employee has no primary or secondary email address.")]
        public const string LOG_EMPLOYEE_PASSWORD_RESET_EMAIL_SKIPPED_NO_RECIPIENT = "LOG_EMPLOYEE_PASSWORD_RESET_EMAIL_SKIPPED_NO_RECIPIENT";

        [Description("A row was skipped during bulk employee import because it failed validation or another error occurred.")]
        public const string LOG_EMPLOYEE_BULK_UPSERT_RECORD_SKIPPED = "LOG_EMPLOYEE_BULK_UPSERT_RECORD_SKIPPED";

        [Description("Bulk employee import produced no valid rows to save after errors were skipped.")]
        public const string LOG_EMPLOYEE_BULK_UPSERT_NO_VALID_RECORDS = "LOG_EMPLOYEE_BULK_UPSERT_NO_VALID_RECORDS";

        [Description("Folder path is required.")]
        public const string EXCEL_FOLDER_PATH_REQUIRED = "EXCEL_FOLDER_PATH_REQUIRED";
        [Description("Department ID cannot be empty.")]
        public const string GENERIC_DEPARTMENT_ID_CANNOT_BE_EMPTY = "GENERIC_DEPARTMENT_ID_CANNOT_BE_EMPTY";
        [Description("At least one DTO is required for bulk upsert.")]
        public const string GENERIC_AT_LEAST_ONE_DTO_REQUIRED_FOR_BULK_UPSERT = "GENERIC_AT_LEAST_ONE_DTO_REQUIRED_FOR_BULK_UPSERT";
        [Description("All DTOs must have the same RequestID.")]
        public const string GENERIC_ALL_DTOS_MUST_HAVE_SAME_REQUEST_ID = "GENERIC_ALL_DTOS_MUST_HAVE_SAME_REQUEST_ID";
        [Description("All DTOs must have the same DepartmentEquipmentID.")]
        public const string GENERIC_ALL_DTOS_MUST_HAVE_SAME_DEPARTMENT_EQUIPMENT_ID = "GENERIC_ALL_DTOS_MUST_HAVE_SAME_DEPARTMENT_EQUIPMENT_ID";
        [Description("You are not allowed to access this routing group information.")]
        public const string ROUTING_GROUP_ACCESS_NOT_ALLOWED = "ROUTING_GROUP_ACCESS_NOT_ALLOWED";
        [Description("You are not allowed to edit this routing group information.")]
        public const string ROUTING_GROUP_EDIT_NOT_ALLOWED = "ROUTING_GROUP_EDIT_NOT_ALLOWED";
        [Description("Job position ID is required.")]
        public const string CLOSED_JOB_POSITION_JOB_POSITION_ID_REQUIRED = "CLOSED_JOB_POSITION_JOB_POSITION_ID_REQUIRED";
        [Description("Work start date is required to create a split job position.")]
        public const string CLOSED_JOB_POSITION_WORK_START_DATE_REQUIRED_FOR_SPLIT = "CLOSED_JOB_POSITION_WORK_START_DATE_REQUIRED_FOR_SPLIT";
        [Description("Work start time is required to create a split job position.")]
        public const string CLOSED_JOB_POSITION_WORK_START_TIME_REQUIRED_FOR_SPLIT = "CLOSED_JOB_POSITION_WORK_START_TIME_REQUIRED_FOR_SPLIT";
        [Description("CreatedUserID or SplitEmployeeID is required to save questionnaire submissions.")]
        public const string CLOSED_JOB_POSITION_CREATED_USER_OR_SPLIT_EMPLOYEE_REQUIRED = "CLOSED_JOB_POSITION_CREATED_USER_OR_SPLIT_EMPLOYEE_REQUIRED";
        [Description("No audit row mapping for {0}.")]
        public const string AUDIT_NO_ROW_MAPPING_FOR_TYPE = "AUDIT_NO_ROW_MAPPING_FOR_TYPE";
        [Description("Employee already exists with same username, badge number, or employee number.")]
        public const string EMPLOYEE_DUPLICATE_BY_USERNAME_BADGE_OR_NUMBER = "EMPLOYEE_DUPLICATE_BY_USERNAME_BADGE_OR_NUMBER";
        #endregion

        #region Admin service / JSON Patch
        [Description("Only 'replace' operations are allowed for Admin PATCH.")]
        public const string ADMIN_PATCH_ONLY_REPLACE_ALLOWED = "ADMIN_PATCH_ONLY_REPLACE_ALLOWED";

        [Description("Invalid JSON Patch document for Admin.")]
        public const string ADMIN_PATCH_INVALID_DOCUMENT = "ADMIN_PATCH_INVALID_DOCUMENT";

        [Description("Patch document is required for Admin PATCH.")]
        public const string REQUIRED_ADMIN_PATCH_DOCUMENT = "REQUIRED_ADMIN_PATCH_DOCUMENT";
        #endregion

        #region Employee service / JSON Patch
        [Description("Only 'replace' operations are allowed for Employee PATCH.")]
        public const string EMPLOYEE_PATCH_ONLY_REPLACE_ALLOWED = "EMPLOYEE_PATCH_ONLY_REPLACE_ALLOWED";

        [Description("Invalid JSON Patch document for Employee.")]
        public const string EMPLOYEE_PATCH_INVALID_DOCUMENT = "EMPLOYEE_PATCH_INVALID_DOCUMENT";

        [Description("Patch document is required for Employee PATCH.")]
        public const string REQUIRED_EMPLOYEE_PATCH_DOCUMENT = "REQUIRED_EMPLOYEE_PATCH_DOCUMENT";

        [Description("You can only submit a location for a job assigned to you.")]
        public const string EMPLOYEE_JOB_LOCATION_NOT_ASSIGNED_TO_YOU = "EMPLOYEE_JOB_LOCATION_NOT_ASSIGNED_TO_YOU";
        [Description("Job position schedule is missing.")]
        public const string EMPLOYEE_JOB_LOCATION_SCHEDULE_MISSING = "EMPLOYEE_JOB_LOCATION_SCHEDULE_MISSING";
        [Description("Location can be submitted starting 30 minutes before the job starts.")]
        public const string EMPLOYEE_JOB_LOCATION_TOO_EARLY = "EMPLOYEE_JOB_LOCATION_TOO_EARLY";
        [Description("Location can no longer be submitted after 30 minutes of job end time.")]
        public const string EMPLOYEE_JOB_LOCATION_SHIFT_ENDED = "EMPLOYEE_JOB_LOCATION_SHIFT_ENDED";
        #endregion

        public static string GetErrorMessage(string errorCode)
        {
            var field =
    typeof(ModelErrorCodes).GetField(errorCode, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
    ?? typeof(ModelValidationErrorCodes).GetField(errorCode, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var descriptionAttributes = field?.GetCustomAttributes(true);
            if (descriptionAttributes?.FirstOrDefault() is DescriptionAttribute descriptionAttribute)
            {
                return descriptionAttribute.Description;
            }
            return string.Empty;
        }

        public static string GetErrorMessage(string errorCode, params object[] args)
        {
            var messageTemplate = GetErrorMessage(errorCode);
            if (args == null || args.Length == 0)
            {
                return messageTemplate;
            }

            try
            {
                return string.Format(messageTemplate, args);
            }
            catch (FormatException)
            {
                return messageTemplate;
            }
        }
    }

    /// <summary>
    /// Message templates for Post Schedule Updated email content built in code.
    /// </summary>
    public static class PostScheduleEmailMessages
    {
        public static string BoldYes => ModelErrorCodes.GetErrorMessage(ModelErrorCodes.POST_SCHEDULE_EMAIL_BOLD_YES);

        public static string BoldNo => ModelErrorCodes.GetErrorMessage(ModelErrorCodes.POST_SCHEDULE_EMAIL_BOLD_NO);

        public static string VehicleRequiredChangeFormat =>
            ModelErrorCodes.GetErrorMessage(ModelErrorCodes.POST_SCHEDULE_EMAIL_VEHICLE_REQUIRED_CHANGE);

        public static string VehicleAssignmentsChangedFormat =>
            ModelErrorCodes.GetErrorMessage(ModelErrorCodes.POST_SCHEDULE_EMAIL_VEHICLE_ASSIGNMENTS_CHANGED);

        public static string VehicleRemovedSuffix =>
            ModelErrorCodes.GetErrorMessage(ModelErrorCodes.POST_SCHEDULE_EMAIL_VEHICLE_REMOVED_SUFFIX);

        public static string VehicleAssignedSuffix =>
            ModelErrorCodes.GetErrorMessage(ModelErrorCodes.POST_SCHEDULE_EMAIL_VEHICLE_ASSIGNED_SUFFIX);

        public static string VehicleAssignmentsUnchanged =>
            ModelErrorCodes.GetErrorMessage(ModelErrorCodes.POST_SCHEDULE_EMAIL_VEHICLE_ASSIGNMENTS_UNCHANGED);
    }
}
