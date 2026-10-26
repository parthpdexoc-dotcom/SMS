using FluentValidation;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class RequestValidator : AbstractValidator<RequestDto>
    {
        public RequestValidator()
        {
            RuleFor(x => x.RequesterID)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_ID);

            RuleFor(x => x.FirstName)
            .MaximumLength(50)
            .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_FIRST_NAME)
            .When(x => !string.IsNullOrEmpty(x.FirstName));

            RuleFor(x => x)
            .Custom((model, context) =>
            {
                var start = model.StartDate;
                var end = model.EndDate;

                // Common rule: EndDate >= StartDate
                if (end.DayNumber < start.DayNumber)
                {
                    context.AddFailure(
                        nameof(model.EndDate),
                        ModelValidationErrorCodes.END_DATE_CANNOT_BE_LESS_THAN_START_DATE
                    );
                    return;
                }

                switch (model.RequestType.RequestType)
                {
                    case RequestTypes.OneTime:
                        if (start != end)
                        {
                            context.AddFailure(
                                nameof(model.EndDate),
                                ModelValidationErrorCodes.ONETIME_DATES_MUST_BE_SAME
                            );
                        }
                        if (model.RequestType.DaysOfWeek != null)
                        {
                            context.AddFailure(
                                "RequestType.DaysOfWeek",
                                ModelValidationErrorCodes.WEEKDAYS_NOT_REQUIRED_FOR_NON_WEEKLY
                            );
                        }
                        if (model.RequestType.DaysOfMonth != null)
                        {
                            context.AddFailure(
                                "RequestType.DaysOfMonth",
                                ModelValidationErrorCodes.MONTHDAYS_NOT_REQUIRED_FOR_NON_MONTHLY
                            );
                        }
                        break;

                    case RequestTypes.Daily:
                        if ((end.DayNumber - start.DayNumber) < 1)
                        {
                            context.AddFailure(
                                nameof(model.EndDate),
                                ModelValidationErrorCodes.MINIMUM_ONE_DAY_GAP_REQUIRED
                            );
                        }
                        if (model.RequestType.DaysOfWeek != null)
                        {
                            context.AddFailure(
                                "RequestType.DaysOfWeek",
                                ModelValidationErrorCodes.WEEKDAYS_NOT_REQUIRED_FOR_NON_WEEKLY
                            );
                        }
                        if (model.RequestType.DaysOfMonth != null)
                        {
                            context.AddFailure(
                                "RequestType.DaysOfMonth",
                                ModelValidationErrorCodes.MONTHDAYS_NOT_REQUIRED_FOR_NON_MONTHLY
                            );
                        }
                        break;

                    case RequestTypes.Weekly:
                        if (model.RequestType.DaysOfWeek == null || !model.RequestType.DaysOfWeek.Any())
                        {
                            context.AddFailure(
                                "RequestType.DaysOfWeek",
                                ModelValidationErrorCodes.WEEKDAYS_REQUIRED_FOR_WEEKLY
                            );
                        }

                        if ((end.DayNumber - start.DayNumber) < 7)
                        {
                            context.AddFailure(
                                nameof(model.EndDate),
                                ModelValidationErrorCodes.MINIMUM_ONE_WEEK_GAP_REQUIRED
                            );
                        }
                        if (model.RequestType.DaysOfMonth != null)
                        {
                            context.AddFailure(
                                "RequestType.DaysOfMonth",
                                ModelValidationErrorCodes.MONTHDAYS_NOT_REQUIRED_FOR_NON_MONTHLY
                            );
                        }
                        break;

                    case RequestTypes.Monthly:
                        if (start.AddMonths(1) > end)
                        {
                            context.AddFailure(
                                nameof(model.EndDate),
                                ModelValidationErrorCodes.MINIMUM_ONE_MONTH_GAP_REQUIRED
                            );
                        }
                        if (model.RequestType.DaysOfWeek != null)
                        {
                            // DaysOfWeek IS required for Pattern mode (e.g. First Monday), so we shouldn't fail if it exists.
                            // But for strict validation, we might check MonthlyMode. 
                            // For now, let's allow it or check if mode is ByDate and it is present?
                            // The original code failed if WeekDays was present for Monthly. 
                            // Meaning legacy Monthly only supported 'MonthDays'.
                            // BUT new Monthly supports ByPattern which uses DaysOfWeek.
                            // So we should NOT fail if DaysOfWeek exists, UNLESS mode is ByDate?
                            // Let's stick to the original logic for now but adapted, assuming 'DaysOfMonth' is the primary for 'ByDate'.
                            // If user is doing custom recurrence or advanced monthly, they might need DaysOfWeek.
                            
                            // Original legacy logic:
                            /*
                            if (model.WeekDays != null) { fail }
                            */
                            
                            // New logic: RequestTypeDto allows MonthlyMode.
                            if (model.RequestType.MonthlyMode == MonthlyMode.ByDate && model.RequestType.DaysOfWeek != null)
                            {
                                 context.AddFailure(
                                    "RequestType.DaysOfWeek",
                                    ModelValidationErrorCodes.WEEKDAYS_NOT_REQUIRED_FOR_NON_WEEKLY
                                );
                            }
                        }
                        break;
                    case RequestTypes.Custom:
                        if ((end.DayNumber - start.DayNumber) < 1)
                        {
                            context.AddFailure(
                                nameof(model.EndDate),
                                ModelValidationErrorCodes.MINIMUM_ONE_DAY_GAP_REQUIRED
                            );
                        }
                        break;
                }
            });


            RuleFor(x => x.LastName)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_LAST_NAME)
                .When(x => !string.IsNullOrEmpty(x.LastName));

            RuleFor(x => x.PrimaryPhone)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_PRIMARY_PHONE)
                .Must(SMS.Generic.Common.Helpers.PhoneNumberHelper.HasValidPhoneDigits)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PHONE_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.PrimaryPhone));

            RuleFor(x => x.CellPhone)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_CELL_PHONE)
                .Must(SMS.Generic.Common.Helpers.PhoneNumberHelper.HasValidPhoneDigits)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PHONE_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.CellPhone));

            RuleFor(x => x.SecondaryPhone)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_SECONDARY_PHONE)
                .Must(SMS.Generic.Common.Helpers.PhoneNumberHelper.HasValidPhoneDigits)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PHONE_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.SecondaryPhone));

            RuleFor(x => x.AlternateCellPhone)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_ALTERNATE_CELL_PHONE)
                .Must(SMS.Generic.Common.Helpers.PhoneNumberHelper.HasValidPhoneDigits)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PHONE_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.AlternateCellPhone));

            RuleFor(x => x.PrimaryEmail)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_PRIMARY_EMAIL)
                .Must(EmailAddressHelper.IsValidEmailAddress)
                .WithMessage(ModelValidationErrorCodes.INVALID_REQUEST_PRIMARY_EMAIL_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.PrimaryEmail));

            RuleFor(x => x.SecondaryEmail)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_SECONDARY_EMAIL)
                .Must(EmailAddressHelper.IsValidEmailAddress)
                .WithMessage(ModelValidationErrorCodes.INVALID_REQUEST_SECONDARY_EMAIL_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.SecondaryEmail));

            RuleFor(x => x.Title)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_TITLE)
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Location)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_LOCATION)
                .When(x => !string.IsNullOrEmpty(x.Location));

            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_REQUEST_ADDRESS)
                .MaximumLength(250)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_ADDRESS);

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_REQUEST_CITY)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_CITY);

            RuleFor(x => x.State)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_REQUEST_STATE)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_STATE);

            RuleFor(x => x.Zip)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_REQUEST_ZIP)
                .MaximumLength(10)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_ZIP);

            RuleFor(x => x.DepartmentDefinedFieldValue1)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_1)
                .When(x => !string.IsNullOrEmpty(x.DepartmentDefinedFieldValue1));

            RuleFor(x => x.DepartmentDefinedFieldValue2)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_2)
                .When(x => !string.IsNullOrEmpty(x.DepartmentDefinedFieldValue2));

            RuleFor(x => x.DepartmentDefinedFieldValue3)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_3)
                .When(x => !string.IsNullOrEmpty(x.DepartmentDefinedFieldValue3));

            RuleFor(x => x.DepartmentDefinedFieldValue4)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_4)
                .When(x => !string.IsNullOrEmpty(x.DepartmentDefinedFieldValue4));

            RuleFor(x => x.DepartmentDefinedFieldValue5)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_5)
                .When(x => !string.IsNullOrEmpty(x.DepartmentDefinedFieldValue5));

            RuleFor(x => x.DepartmentDefinedFieldValue6)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_6)
                .When(x => !string.IsNullOrEmpty(x.DepartmentDefinedFieldValue6));

            RuleFor(x => x.DepartmentDefinedFieldValue7)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_7)
                .When(x => !string.IsNullOrEmpty(x.DepartmentDefinedFieldValue7));

            RuleFor(x => x.DepartmentDefinedFieldValue8)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_8)
                .When(x => !string.IsNullOrEmpty(x.DepartmentDefinedFieldValue8));

            RuleFor(x => x.DepartmentDefinedFieldValue9)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_9)
                .When(x => !string.IsNullOrEmpty(x.DepartmentDefinedFieldValue9));

            RuleFor(x => x.DepartmentDefinedFieldValue10)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_DEPARTMENT_DEFINED_FIELD_VALUE_10)
                .When(x => !string.IsNullOrEmpty(x.DepartmentDefinedFieldValue10));

            // ===== VEHICLE VALIDATION =====
            //When(x => x.RequiresVehicle == true, () =>
            //{
            //    RuleFor(x => x.RequiresVehicleAtEveryPost)
            //        .NotNull()
            //        .WithMessage(ModelValidationErrorCodes.REQUIRES_VEHICLE_AT_EACH_JOB_REQUIRED);

            //    RuleFor(x => x.NumberOfVehicles)
            //        .GreaterThan(0)
            //        .WithMessage(ModelValidationErrorCodes.NUMBER_OF_VEHICLES_REQUIRED);

            //    RuleFor(x => x.VehicleApproved)
            //        .NotNull()
            //        .WithMessage(ModelValidationErrorCodes.VEHICLE_APPROVED_REQUIRED);
            //});

            //When(x => x.RequiresVehicle == false || x.RequiresVehicle == null, () =>
            //{
            //    RuleFor(x => x.RequiresVehicleAtEveryPost)
            //        .Null()
            //        .WithMessage(ModelValidationErrorCodes.REQUIRES_VEHICLE_AT_EACH_JOB_NOT_ALLOWED);

            //    RuleFor(x => x.VehicleApproved)
            //        .Null()
            //        .WithMessage(ModelValidationErrorCodes.VEHICLE_APPROVED_NOT_ALLOWED);
            //});


            //// ===== EQUIPMENT VALIDATION =====
            //When(x => x.RequiresEquipment == true, () =>
            //{
            //    RuleFor(x => x.RequiresEquipmentAtEachPost)
            //        .NotNull()
            //        .WithMessage(ModelValidationErrorCodes.REQUIRES_EQUIPMENT_AT_EACH_JOB_REQUIRED);

            //    RuleFor(x => x.NumberOfEquipment)
            //        .GreaterThan(0)
            //        .WithMessage(ModelValidationErrorCodes.NUMBER_OF_EQUIPMENT_REQUIRED);
            //});

            //When(x => x.RequiresEquipment == false || x.RequiresEquipment == null, () =>
            //{
            //    RuleFor(x => x.RequiresEquipmentAtEachPost)
            //        .Null()
            //        .WithMessage(ModelValidationErrorCodes.REQUIRES_EQUIPMENT_AT_EACH_JOB_NOT_ALLOWED);
            //});

            // ===== SKILL VALIDATION =====
            When(x => x.IsSkillRequired == true, () =>
            {
                RuleFor(x => x.SkillIDs)
                    .Must(list =>
                        list != null &&
                        list.Any() &&
                        list.All(id => id != Guid.Empty))
                    .WithMessage("At least one valid SkillID is required when IsSkillRequired is true.");
            });

            When(x => x.IsSkillRequired == false || x.IsSkillRequired == null, () =>
            {
                RuleFor(x => x.SkillIDs)
                    .Must(list => list == null || list.Count == 0)
                    .WithMessage("SkillIDs must be null or have a count of 0 when IsSkillRequired is not true.");
            });
        }
    }
}

