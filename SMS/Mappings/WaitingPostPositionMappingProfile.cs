using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping TenureWaitingPosition entities to DTOs and vice-versa.
    /// </summary>
    public class WaitingPostPositionMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitingPostPositionMappingProfile"/> class.
        /// </summary>
        public WaitingPostPositionMappingProfile()
        {
            CreateMap<WaitingPostPosition, WaitingPostPositionDto>()
                // Ensure AutoMapper uses parameterless ctor (DTO also has an assignment ctor)
                .ConstructUsing(_ => new WaitingPostPositionDto())
                .ForMember(dest => dest.WaitingPostPositionID, opt => opt.MapFrom(src => src.WaitingPostPositionID))
                .ForMember(dest => dest.PostID, opt => opt.MapFrom(src => src.PostID))
                .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => src.RequestID))
                .ForMember(dest => dest.EmployeeID, opt => opt.MapFrom(src => src.EmployeeID))
                .ForMember(dest => dest.AssignedEmployeeName, opt => opt.MapFrom(src =>
                    src.Employee != null
                        ? $"{src.Employee.FirstName} {src.Employee.LastName}".Trim()
                        : null))
                .ForMember(dest => dest.DateOfWork, opt => opt.MapFrom(src => src.DateOfWork))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.HasAppliedInTenure, opt => opt.MapFrom(src => src.HasAppliedInTenure))
                .ForMember(dest => dest.PostPositionStatus, opt => opt.MapFrom(src => src.PostPositionStatus))
                .ForMember(dest => dest.ProcessingStatus, opt => opt.MapFrom(src => src.ProcessingStatus))
                .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => src.StatusCode))
                .ForMember(dest => dest.ResultMessage, opt => opt.MapFrom(src => src.ResultMessage))
                .ForMember(dest => dest.ProcessedAt, opt => opt.MapFrom(src => src.ProcessedAt))
                .ForMember(dest => dest.EquipmentRequired, opt => opt.MapFrom(src => src.EquipmentRequired))
                .ForMember(dest => dest.CallSign, opt => opt.MapFrom(src =>
                    src.PostCallSign != null ? src.PostCallSign.CallSign : src.CallSign))
                .ForMember(dest => dest.PostCallSignID, opt => opt.MapFrom(src => src.PostCallSignID))
                .ForMember(dest => dest.WaitingPostPositionPayRate, opt => opt.MapFrom(src => src.WaitingPostPositionPayRate))
                .ForMember(dest => dest.IsPremiumPaidEvent, opt => opt.MapFrom(src => src.IsPremiumPaidEvent))
                .ForMember(dest => dest.PostDepartmentDefinedFieldValue1, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostDepartmentDefinedValue1 : null))
                .ForMember(dest => dest.PostDepartmentDefinedFieldValue2, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostDepartmentDefinedValue2 : null))
                .ForMember(dest => dest.PostDepartmentDefinedFieldValue3, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostDepartmentDefinedValue3 : null))
                .ForMember(dest => dest.PostDepartmentDefinedFieldValue4, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostDepartmentDefinedValue4 : null))
                .ForMember(dest => dest.PostDepartmentDefinedFieldValue5, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostDepartmentDefinedValue5 : null))
                .ForMember(dest => dest.PostDepartmentDefinedFieldValue6, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostDepartmentDefinedValue6 : null))
                .ForMember(dest => dest.PostDepartmentDefinedFieldValue7, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostDepartmentDefinedValue7 : null))
                .ForMember(dest => dest.PostDepartmentDefinedFieldValue8, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostDepartmentDefinedValue8 : null))
                .ForMember(dest => dest.PostDepartmentDefinedFieldValue9, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostDepartmentDefinedValue9 : null))
                .ForMember(dest => dest.PostDepartmentDefinedFieldValue10, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostDepartmentDefinedValue10 : null))
                // Schedule/display fields still derived from related Post when loaded
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostStartTime : default))
                .ForMember(dest => dest.LengthOfWork, opt => opt.MapFrom(src => src.Post != null ? src.Post.PostLength : 0))
                .ForMember(dest => dest.Request, opt => opt.MapFrom(src => src.Request ?? (src.Post != null ? src.Post.Request : null)))
                .ForMember(dest => dest.Post, opt => opt.MapFrom(src => src.Post));

            CreateMap<WaitingPostPositionDto, WaitingPostPosition>()
                .ForMember(dest => dest.WaitingPostPositionID, opt => opt.MapFrom(src => src.WaitingPostPositionID ?? Guid.Empty))
                .ForMember(dest => dest.PostID, opt => opt.MapFrom(src => src.PostID))
                .ForMember(dest => dest.RequestID, opt => opt.MapFrom(src => src.RequestId))
                .ForMember(dest => dest.EmployeeID, opt => opt.MapFrom(src => src.EmployeeID))
                .ForMember(dest => dest.DateOfWork, opt => opt.MapFrom(src => src.DateOfWork))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.HasAppliedInTenure, opt => opt.MapFrom(src => src.HasAppliedInTenure))
                .ForMember(dest => dest.PostPositionStatus, opt => opt.MapFrom(src => src.PostPositionStatus))
                .ForMember(dest => dest.ProcessingStatus, opt => opt.MapFrom(src => src.ProcessingStatus))
                .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => src.StatusCode))
                .ForMember(dest => dest.ResultMessage, opt => opt.MapFrom(src => src.ResultMessage))
                .ForMember(dest => dest.ProcessedAt, opt => opt.MapFrom(src => src.ProcessedAt))
                .ForMember(dest => dest.EquipmentRequired, opt => opt.MapFrom(src => src.EquipmentRequired))
                .ForMember(dest => dest.CallSign, opt => opt.MapFrom(src => src.CallSign))
                .ForMember(dest => dest.PostCallSignID, opt => opt.MapFrom(src => src.PostCallSignID))
                .ForMember(dest => dest.WaitingPostPositionPayRate, opt => opt.MapFrom(src => src.WaitingPostPositionPayRate))
                .ForMember(dest => dest.IsPremiumPaidEvent, opt => opt.MapFrom(src => src.IsPremiumPaidEvent))

                // Navigation is resolved by EF; don't map incoming DTO graph.
                .ForMember(dest => dest.Post, opt => opt.Ignore())
                .ForMember(dest => dest.Request, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.PostCallSign, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByEmployee, opt => opt.Ignore())
                .ForMember(dest => dest.EditedByEmployee, opt => opt.Ignore());

            CreateMap<WaitingPostPosition, WaitingPostPositionResponseDto>()
                .IncludeBase<WaitingPostPosition, WaitingPostPositionDto>()
                .ForMember(dest => dest.BadgeNumber, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.BadgeNumber : null))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FirstName : null))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.LastName : null))
                .ForMember(dest => dest.PrimaryPhone, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.PrimaryPhone : null))
                .ForMember(dest => dest.PrimaryEmail, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.PrimaryEmail : null))
                .ForMember(dest => dest.EmployeeTypeName, opt => opt.MapFrom(src => src.Employee != null && src.Employee.EmployeeType != null ? src.Employee.EmployeeType.EmployeeTypeShort : null))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Employee != null && src.Employee.Department != null ? src.Employee.Department.DepartmentName : null))
                .ForMember(dest => dest.EmployeeStatusName, opt => opt.MapFrom(src => src.Employee != null && src.Employee.Status != null ? src.Employee.Status.StatusShort : null))
                .ForMember(dest => dest.Post, opt => opt.Ignore());
        }
    }
}
