using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping PostPositionVehicle entities to DTOs and vice-versa.
    /// </summary>
    public class PostPositionVehicleMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostPositionVehicleMappingProfile"/> class.
        /// </summary>
        public PostPositionVehicleMappingProfile()
        {
            CreateMap<PostPositionVehicle, PostPositionVehicleDto>()
                .ReverseMap();

            // Clone for split / transfer hand-off (IDs and audit set by caller).
            CreateMap<PostPositionVehicleDto, PostPositionVehicleDto>()
                .ForMember(d => d.PostPositionVehicleID, o => o.Ignore())
                .ForMember(d => d.PostPositionID, o => o.Ignore())
                .ForMember(d => d.CreatedDate, o => o.Ignore())
                .ForMember(d => d.CreatedUserID, o => o.Ignore())
                .ForMember(d => d.CreatedBy, o => o.Ignore())
                .ForMember(d => d.EditedDate, o => o.Ignore())
                .ForMember(d => d.EditedUserID, o => o.Ignore())
                .ForMember(d => d.EditedBy, o => o.Ignore())
                .ForMember(d => d.IsDeleted, o => o.Ignore());

            CreateMap<DepartmentEquipmentDto, PostPositionVehicle>()
                .ForMember(d => d.PostPositionVehicleID, o => o.Ignore())
                .ForMember(d => d.PostPositionID, o => o.Ignore())
                .ForMember(d => d.DepartmentEquipmentID, o => o.MapFrom(s => s.DepartmentEquipmentID))
                .ForMember(d => d.ChargedType, o => o.MapFrom(s => s.ChargedType))
                .ForMember(d => d.Charge, o => o.MapFrom(s => s.Charge))
                .ForMember(d => d.ChargeMileage, o => o.MapFrom(s => s.ChargeMileage))
                .ForMember(d => d.MileageCharge, o => o.MapFrom(s => s.MileageCharge))
                .ForMember(d => d.MileageCondition, o => o.MapFrom(s => s.MileageCondition))
                .ForMember(d => d.EquipmentQuantity, o => o.Ignore())
                .ForMember(d => d.EquipmentPerPostPosition, o => o.Ignore())
                .ForMember(d => d.CreatedDate, o => o.Ignore())
                .ForMember(d => d.CreatedUserID, o => o.Ignore())
                .ForMember(d => d.CreatedBy, o => o.Ignore())
                .ForMember(d => d.EditedDate, o => o.Ignore())
                .ForMember(d => d.EditedUserID, o => o.Ignore())
                .ForMember(d => d.EditedBy, o => o.Ignore())
                .ForMember(d => d.IsDeleted, o => o.Ignore())
                .ForMember(d => d.PostPosition, o => o.Ignore())
                .ForMember(d => d.DepartmentEquipment, o => o.Ignore())
                .ForMember(d => d.CreatedByEmployee, o => o.Ignore())
                .ForMember(d => d.EditedByEmployee, o => o.Ignore());
        }
    }
}
