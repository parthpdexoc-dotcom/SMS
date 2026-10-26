using SMS.Generic.Services.Interfaces;
using SMS.Generic.Services.Interfaces.Audit;
using SMS.Generic.Services.Services;
using SMS.Generic.Services.Services.Audit;
using SMS.Generic.Dtos.Interfaces.Audit;
using SMS.Helpers;
using SMS.Mappings;
using SMS.Services;

namespace SMS.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddEmailSender(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmtpSettings>(configuration.GetSection(SmtpSettings.SectionName));
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            return services;
        }

        public static IServiceCollection AddTwilioSms(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TwilioSettings>(configuration.GetSection(TwilioSettings.SectionName));
            services.AddHttpClient<ISmsSender, TwilioSmsSender>();
            return services;
        }

        public static IServiceCollection AddVehicleApprovalLinkBuilder(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SMS.Generic.Common.Helpers.VehicleApprovalEmailSettings>(configuration.GetSection(SMS.Generic.Common.Helpers.VehicleApprovalEmailSettings.SectionName));
            services.AddScoped<IVehicleApprovalLinkBuilder, VehicleApprovalLinkBuilder>();
            return services;
        }

        public static IServiceCollection AddFirebasePushNotification(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IFirebasePushNotificationService, FirebasePushNotificationService>();
            return services;
        }

        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            // Add HTTP Client
            services.AddHttpClient();

            // Add services
            services.AddScoped<IPostDocumentService, PostDocumentService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IEmployeeValidationService, EmployeeValidationService>();
            services.AddScoped<IDivisionService, DivisionService>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IPostPositionService, PostPositionService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<IPlaceholderService, PlaceholderService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IActivityTypeService, ActivityTypeService>();
            services.AddScoped<IEmployeeStatusService, EmployeeStatusService>();
            services.AddScoped<IEmployeeTypeService, EmployeeTypeService>();
            services.AddScoped<IDepartmentActivityTypeService, DepartmentActivityTypeService>();
            services.AddScoped<IEmployeeAdminService, EmployeeAdminService>();
            services.AddScoped<IEstimateService, EstimateService>();
            services.AddScoped<IHRStatusService, HRStatusService>();
            services.AddScoped<IInvoiceEquipmentService, InvoiceEquipmentService>();
            services.AddScoped<IInvoiceEquipmentListService, InvoiceEquipmentListService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IInvoiceDetailService, InvoiceDetailService>();
            services.AddScoped<IPaymentAllocationService, PaymentAllocationService>();
            services.AddScoped<IRequestTypeService, RequestTypeService>();
            services.AddScoped<IPostTypeService, PostTypeService>();
            services.AddScoped<IPtaeoCodeService, PtaeoCodeService>();
            services.AddScoped<IRequesterTypeService, RequesterTypeService>();
            services.AddScoped<IHrEmployeeTypeService, HrEmployeeTypeService>();
            services.AddScoped<IRequestActivityTypeService, RequestActivityTypeService>();
            services.AddScoped<IDepartmentConfigurationService, DepartmentConfigurationService>();
            services.AddScoped<IDepartmentCronJobManagementService, DepartmentCronJobManagementService>();
            services.AddScoped<IBlockDateService, BlockDateService>();
            services.AddScoped<IBlockPostDayService, BlockPostDayService>();
            services.AddScoped<IBusinessTypeService, BusinessTypeService>();
            services.AddScoped<IPostCallSignService, PostCallSignService>();
            services.AddScoped<IDepartmentPolicyService, DepartmentPolicyService>();
            services.AddScoped<ITenureEmployeeStatisticsService, TenureEmployeeStatisticsService>();
            services.AddScoped<IDepartmentAdminService, DepartmentAdminService>();
            services.AddScoped<IDivisionAdminOnDutyService, DivisionAdminOnDutyService>();
            services.AddScoped<IEligibleTypeService, EligibleTypeService>();
            services.AddScoped<IEmailProcessService, EmailProcessService>();
            services.AddScoped<IEmailAccessCodeService, EmailAccessCodeService>();
            services.AddScoped<IEmergencyContactService, EmergencyContactService>();
            services.AddScoped<IEmployeeUploadService, EmployeeUploadService>();
            services.AddScoped<IRequesterConfigurationService, RequesterConfigurationService>();
            services.AddScoped<IExemptedRequesterService, ExemptedRequesterService>();
            services.AddScoped<IExportRosterService, ExportRosterService>();
            services.AddScoped<IExportLogService, ExportLogService>();
            services.AddScoped<IFitStandardsService, FitStandardsService>();
            services.AddScoped<IHrRequestClassService, HrRequestClassService>();
            services.AddScoped<IImageDetailsService, ImageDetailsService>();
            services.AddScoped<IInvoiceAdjustmentService, InvoiceAdjustmentService>();
            services.AddScoped<IEmployeeSkillService, EmployeeSkillService>();
            services.AddScoped<IDepartmentResponseAreaService, DepartmentResponseAreaService>();
            services.AddScoped<IPostsOnDutyService, PostsOnDutyService>();
            services.AddScoped<IEmployeePositionService, EmployeePositionService>();
            services.AddScoped<IRequesterContactTypesService, RequesterContactTypesService>();
            services.AddScoped<IRequesterContactsService, RequesterContactsService>();
            services.AddScoped<IRequesterLocationsService, RequesterLocationsService>();
            services.AddScoped<IRequesterManagerService, RequesterManagerService>();
            services.AddScoped<IRequesterRestrictedListService, RequesterRestrictedListService>();
            services.AddScoped<IEmployeeTenureService, EmployeeTenureService>();
            services.AddScoped<IDepartmentEquipmentService, DepartmentEquipmentService>();
            services.AddScoped<IRequesterRestrictedListService, RequesterRestrictedListService>();
            services.AddScoped<IEmployeeContactsService, EmployeeContactsService>();
            services.AddScoped<IPaymentTypeService, PaymentTypeService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IRefundPaymentService, RefundPaymentService>();
            services.AddScoped<IRefundPaymentAllocationService, RefundPaymentAllocationService>();
            services.AddScoped<IRequesterLedgerService, RequesterLedgerService>();
            services.AddScoped<IHrDivisionService, HrDivisionService>();
            services.AddScoped<IEmployeeContactTypeService, EmployeeContactTypeService>();
            services.AddScoped<ICloseRequestDetailsService, CloseRequestDetailsService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IEmailCommunicationService, EmailCommunicationService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<INotificationHistoryService, NotificationHistoryService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPrerequisiteTypeService, PrerequisiteTypeService>();
            services.AddScoped<IPrerequisiteService, PrerequisiteService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IStateService, StateService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IEquipmentTypeService, EquipmentTypeService>();
            services.AddScoped<IEquipmentService, EquipmentService>();
            services.AddScoped<IAdminTypeService, AdminTypeService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IDepartmentDivisionsService, DepartmentDivisionsService>();
            services.AddScoped<IRequesterService, RequesterService>();
            services.AddScoped<IRequesterAuthorizedService, RequesterAuthorizedService>();
            services.AddScoped<IRequesterAuditReadService, RequesterAuditReadService>();
            services.AddScoped<IPostAuditReadService, PostAuditReadService>();
            services.AddScoped<IRequestAuditReadService, RequestAuditReadService>();
            services.AddScoped<IPostPositionAuditReadService, PostPositionAuditReadService>();
            services.AddScoped<IEmployeeAuditReadService, EmployeeAuditReadService>();
            services.AddScoped<IEmployeeTypeAuditReadService, EmployeeTypeAuditReadService>();
            services.AddScoped<ISkillAuditReadService, SkillAuditReadService>();
            services.AddScoped<IBusinessTypeAuditReadService, BusinessTypeAuditReadService>();
            services.AddScoped<IEmployeeStatusAuditReadService, EmployeeStatusAuditReadService>();
            services.AddScoped<IDepartmentResponseAreaAuditReadService, DepartmentResponseAreaAuditReadService>();
            services.AddScoped<IClosedPostPositionAuditReadService, ClosedPostPositionAuditReadService>();
            services.AddScoped<IClosedPostPositionEquipmentAuditReadService, ClosedPostPositionEquipmentAuditReadService>();
            services.AddScoped<IRequestScheduleAuditReadService, RequestScheduleAuditReadService>();
            services.AddScoped<IRoutingGroupAuditReadService, RoutingGroupAuditReadService>();
            services.AddScoped<IHolidayDayAuditReadService, HolidayDayAuditReadService>();
            services.AddScoped<IDepartmentDivisionsAuditReadService, DepartmentDivisionsAuditReadService>();
            services.AddScoped<IDepartmentActivityTypeAuditReadService, DepartmentActivityTypeAuditReadService>();
            services.AddScoped<IEquipmentTypeAuditReadService, EquipmentTypeAuditReadService>();
            services.AddScoped<IDepartmentEquipmentAuditReadService, DepartmentEquipmentAuditReadService>();
            services.AddScoped<IDepartmentFeeAuditReadService, DepartmentFeeAuditReadService>();
            services.AddScoped<IPayCodeAuditReadService, PayCodeAuditReadService>();
            services.AddScoped<IPaymentTypeAuditReadService, PaymentTypeAuditReadService>();
            services.AddScoped<IPaymentAuditReadService, PaymentAuditReadService>();
            services.AddScoped<IPaymentAllocationAuditReadService, PaymentAllocationAuditReadService>();
            services.AddScoped<IRequesterLedgerAuditReadService, RequesterLedgerAuditReadService>();
            services.AddScoped<IPlaceholderAuditReadService, PlaceholderAuditReadService>();
            services.AddScoped<IEmailTemplateAuditReadService, EmailTemplateAuditReadService>();
            services.AddScoped<IInvoiceAuditReadService, InvoiceAuditReadService>();
            services.AddScoped<IInvoiceDetailAuditReadService, InvoiceDetailAuditReadService>();
            services.AddScoped<IAuditHistoryReadServiceResolver, AuditHistoryReadServiceResolver>();
            services.AddScoped<IAuditHistoryService, AuditHistoryService>();
            services.AddScoped<IPostWorkflowAuditService, PostWorkflowAuditService>();
            services.AddScoped<IPostWorkflowAuditReadService, PostWorkflowAuditReadService>();
            services.AddScoped<IPostPositionFlowAuditService, PostPositionFlowAuditService>();
            services.AddScoped<IPostPositionFlowAuditReadService, PostPositionFlowAuditReadService>();
            services.AddScoped<IRequesterConfigService, RequesterConfigService>();
            services.AddScoped<IPostEquipmentService, PostEquipmentService>();
            services.AddScoped<IPostPositionVehicleService, PostPositionVehicleService>();
            services.AddScoped<IRequestEquipmentService, RequestEquipmentService>();
            services.AddSingleton<IUploadFileStoragePathProvider, UploadFileStoragePathProvider>();
            services.AddScoped<IUploadFileService, UploadFileService>();
            services.AddScoped<IPostNoteService, PostNotesService>();
            services.AddScoped<IRequestNoteService, RequestNotesService>();
            services.AddScoped<IRequestScheduleService, RequestScheduleService>();
            services.AddScoped<IRequestScheduleGenerationService, RequestScheduleGenerationService>();
            services.AddScoped<IDepartmentDefinedFieldsService, DepartmentDefinedFieldsService>();
            services.AddScoped<IDepartmentInvoiceConfigService, DepartmentInvoiceConfigService>();
            services.AddScoped<IDepartmentFeeService, DepartmentFeeService>();
            services.AddScoped<IRoutingGroupService, RoutingGroupServices>();
            services.AddScoped<IVehicleApprovalRoutingService, VehicleApprovalRoutingService>();
            services.AddScoped<IRequestApprovalRoutingService, RequestApprovalRoutingService>();
            services.AddScoped<IPermitApprovalRoutingService, PermitApprovalRoutingService>();
            services.AddScoped<IApprovalDetailService, ApprovalDetailService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IOptionService, OptionService>();
            services.AddScoped<IQuestionnaireService, QuestionnaireService>();
            services.AddScoped<IActivityFormService, ActivityFormService>();
            services.AddScoped<IRequesterQuestionnaireService, RequesterQuestionnaireService>();
            services.AddScoped<IResponseService, ResponseService>();
            services.AddScoped<IAnswersService, AnswersService>();
            services.AddScoped<IDepartmentQuestionnaireService, DepartmentQuestionnaireService>();
            services.AddScoped<IRequesterEmployeeService, RequesterEmployeeService>();
            services.AddScoped<IEmployeeDeviceTokenService, EmployeeDeviceTokenService>();
            services.AddScoped<IEmployeePostLocationService, EmployeePostLocationService>();
            services.AddScoped<IImpersonationAuthService, ImpersonationAuthService>();
            services.AddScoped<IHolidayDayService, HolidayDayService>();
            services.AddScoped<IClosedPostPositionEquipmentService, ClosedPostPositionEquipmentService>();
            services.AddScoped<IClosedPostPositionService, ClosedPostPositionService>();
            services.AddScoped<ICancelledPostPositionService, CancelledPostPositionService>();
            services.AddScoped<IWaitingPostPositionService, WaitingPostPositionService>();
            services.AddScoped<IPayCodeService, PayCodeService>();
            services.AddScoped<IExcelService, ExcelService>();
            services.AddSingleton<IExcelFolderPathProvider, ExcelFolderPathProvider>();
            services.AddSingleton<IJobAuditService, JobAuditService>();
            // Add payroll services
            services.AddScoped<IPayrollDataService, PayrollDataService>();
            services.AddScoped<IPayrollProcessingService, PayrollProcessingService>();
            services.AddScoped<IPayrollNotificationService, PayrollNotificationService>();
            // DOCX payroll export (uncomment together with PayrollProcessingService docx field/ctor and foreach body)
            // services.AddScoped<IPayrollWordDocumentGenerator, PayrollWordDocumentGenerator>();
            return services;
        }

        public static IServiceCollection AddAppAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DocumentMappingProfile),
                                   typeof(DepartmentMappingProfile),
                                   typeof(EmployeeMappingProfile),
                                   typeof(DivisionMappingProfile),
                                   typeof(PositionMappingProfile),
                                   typeof(PostMappingProfile),
                                   typeof(RequestMappingProfile),
                                   typeof(RequesterWorkspaceMappingProfile),
                                   typeof(SkillMappingProfile),
                                   typeof(PlaceholderMappingProfile),
                                   typeof(EmailTemplateMappingProfile),
                                   typeof(ActivityTypeMappingProfile),
                                   typeof(EmployeeStatusMappingProfile),
                                   typeof(EmployeeTypeMappingProfile),
                                   typeof(EmployeeAdminMappingProfile),
                                   typeof(EstimateMappingProfile),
                                   typeof(HRStatusMappingProfile),
                                   typeof(InvoiceEquipmentMappingProfile),
                                   typeof(InvoiceEquipmentListMappingProfile),
                                   typeof(InvoiceMappingProfile),
                                   typeof(InvoiceDetailMappingProfile),
                                   typeof(RequestTypeMappingProfile),
                                   typeof(PostTypeMappingProfile),
                                   typeof(RequesterTypeMappingProfile),
                                   typeof(HrEmployeeTypeMappingProfile),
                                   typeof(RequestActivityTypeMappingProfile),
                                   typeof(DepartmentConfigurationMappingProfile),
                                   typeof(DepartmentCronScheduleMappingProfile),
                                   typeof(BlockDateMappingProfile),
                                   typeof(BlockPostDayMappingProfile),
                                   typeof(BusinessTypeMappingProfile),
                                   typeof(PostCallSignMappingProfile),
                                   typeof(DepartmentPolicyMappingProfile),
                                   typeof(DepartmentAdminMappingProfile),
                                   typeof(DivisionAdminOnDutyMappingProfile),
                                   typeof(EligibleTypeMappingProfile),
                                   typeof(EmailProcessMappingProfile),
                                   typeof(EmailAccessCodeMappingProfile),
                                   typeof(EmergencyContactMappingProfile),
                                   typeof(EmployeeUploadMappingProfile),
                                   typeof(RequesterConfigurationMappingProfile),
                                   typeof(ExemptedRequesterMappingProfile),
                                   typeof(ExportRosterMappingProfile),
                                   typeof(ExportLogMappingProfile),
                                   typeof(FitStandardsMappingProfile),
                                   typeof(HrRequestClassMappingProfile),
                                   typeof(ImageDetailsMappingProfile),
                                   typeof(InvoiceAdjustmentMappingProfile),
                                   typeof(EmployeeSkillMappingProfile),
                                   typeof(DepartmentResponseAreaMappingProfile),
                                   typeof(PostsOnDutyMappingProfile),
                                   typeof(EmployeePositionMappingProfile),
                                   typeof(RequesterContactTypesMappingProfile),
                                   typeof(RequesterContactsMappingProfile),
                                   typeof(RequesterLocationsMappingProfile),
                                   typeof(EmployeeContactsMappingProfile),
                                   typeof(EmployeeContactTypeMappingProfile),
                                   typeof(PaymentTypeMappingProfile),
                                   typeof(PaymentMappingProfile),
                                   typeof(RefundPaymentMappingProfile),
                                   typeof(RefundPaymentAllocationMappingProfile),
                                   typeof(HrDivisionMappingProfile),
                                   typeof(CloseRequestDetailsMappingProfile),
                                   typeof(EmailCommunicationMappingProfile),
                                   typeof(NotificationHistoryMappingProfile),
                                   typeof(LoginMappingProfile),
                                   typeof(RoleMappingProfile),
                                   typeof(PrerequisiteTypeMappingProfile),
                                   typeof(PrerequisiteMappingProfile),
                                    typeof(VehicleMappingProfile),
                                    typeof(ViewDetailsMappingProfile),
                                    typeof(StateMappingProfile),
                                   typeof(UsersMappingProfile),
                                   typeof(EquipmentTypeMappingProfile),
                                   typeof(AdminTypeMappingProfile),
                                   typeof(AdminMappingProfile),
                                   typeof(DepartmentDivisionsMappingProfile),
                                   typeof(EquipmentMappingProfile),
                                   typeof(RequesterMappingProfile),
                                   typeof(RequesterConfigMappingProfile),
                                   typeof(RequesterManagerMappingProfile),
                                   typeof(RequesterRestrictedListMappingProfile),
                                   typeof(RequesterAuthorizedMappingProfile),
                                   typeof(EmployeeTenureMappingProfile),
                                   typeof(DepartmentEquipmentMappingProfile),
                                   typeof(PostPositionMappingProfile),
                                   typeof(PostEquipmentMappingProfile),
                                   typeof(PostPositionVehicleMappingProfile),
                                   typeof(RequestEquipmentMappingProfile),
                                   typeof(UploadFileMappingProfile),
                                   typeof(PostPositionMappingProfile),
                                   typeof(DepartmentDefinedFieldsMappingProfile),
                                   typeof(DepartmentFeeMappingProfile),
                                   typeof(RoutingGroupMappingProfile),
                                   typeof(VehicleApprovalRoutingMappingProfile),
                                   typeof(RequestApprovalRoutingMappingProfile),
                                   typeof(PermitApprovalRoutingMappingProfile),
                                   typeof(ApprovalDetailMappingProfile),
                                   typeof(RequestNotesMappingProfile),
                                   typeof(RequestScheduleMappingProfile),
                                   typeof(RequestScheduleGenerationMappingProfile),
                                   typeof(PostNotesMappingProfile),
                                   typeof(QuestionnaireMappingProfile),
                                   typeof(QuestionMappingProfile),
                                   typeof(RequesterQuestionnaireMappingProfile),
                                   typeof(OptionMappingProfile),
                                   typeof(ResponseMappingProfile),
                                   typeof(AnswersMappingProfile),
                                   typeof(DepartmentQuestionnaireMappingProfile),
                                   typeof(RequesterEmployeeMappingProfile),
                                   typeof(EmployeeDeviceTokenMappingProfile),
                                   typeof(EmployeePostLocationMappingProfile),
                                   typeof(HolidayDayMappingProfile),
                                   typeof(ClosedPostPositionEquipmentMappingProfile),
                                   typeof(ClosedPostPositionMappingProfile),
                                   typeof(PostDocumentMappingProfile),
                                   typeof(CancelledPostPositionMappingProfile),
                                   typeof(WaitingPostPositionMappingProfile),
                                   typeof(PayCodeMappingProfile),
                                   typeof(PaymentAllocationMappingProfile),
                                   typeof(RequesterLedgerMappingProfile));

            return services;
        }
    }
}
