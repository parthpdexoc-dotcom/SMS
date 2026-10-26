using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Services.Mapping;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for <see cref="DepartmentCronJobManagement"/> → <see cref="DepartmentCronScheduleDto"/>;
    /// field expansion and <c>CronExpressionDescription</c> are built in <see cref="DepartmentCronScheduleMapping.ToScheduleDto"/>.
    /// </summary>
    public class DepartmentCronScheduleMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentCronScheduleMappingProfile"/> class.
        /// </summary>
        public DepartmentCronScheduleMappingProfile()
        {
            CreateMap<DepartmentCronJobManagement, DepartmentCronScheduleDto>()
                .ConvertUsing(e => DepartmentCronScheduleMapping.ToScheduleDto(e));
        }
    }
}
