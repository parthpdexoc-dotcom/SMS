using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using System;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping Post entities to DTOs and vice-versa.
    /// </summary>
    public class PostMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostMappingProfile"/> class.
        /// </summary>
        public PostMappingProfile()
        {
            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.PostPositions, opt => opt.MapFrom(src => src.PostPositions != null ? src.PostPositions : new List<PostPosition>()))
                .ForMember(dest => dest.IsEscortActivityType, opt => opt.MapFrom(src =>
                    src.DepartmentActivityType != null
                    && src.DepartmentActivityType.ActivityTypeShort != null
                    && src.DepartmentActivityType.ActivityTypeShort.Equals("Escort", StringComparison.OrdinalIgnoreCase)))
                .ForMember(dest => dest.ReleaseAs, opt => opt.MapFrom(src =>
                    src.Request != null
                        ? src.Request.RequestSchedules
                            .OrderByDescending(s => s.EditedDate ?? s.CreatedDate)
                            .ThenByDescending(s => s.EndDate)
                            .Select(s => (ReleaseAs?)s.ReleaseAs)
                            .FirstOrDefault()
                          ?? ReleaseAs.Open
                        : ReleaseAs.Open))
                .ReverseMap();

            CreateMap<Post, PostDto>()
           .ForMember(dest => dest.IsEscortActivityType, opt => opt.MapFrom(src =>
               src.DepartmentActivityType != null
               && src.DepartmentActivityType.ActivityTypeShort != null
               && src.DepartmentActivityType.ActivityTypeShort.Equals("Escort", StringComparison.OrdinalIgnoreCase)))
           .ForMember(dest => dest.EmployeeRank, opt => opt.MapFrom(src => src.RankEligible && src.EmployeeType != null ? src.EmployeeType.Rank : (int?)null))
           .ForMember(dest => dest.JobRank, opt => opt.MapFrom(src =>
               src.EmployeeType != null
                   ? EmployeeTypeDto.FormatJobRankFromEmployeeTypeShort(
                       src.EmployeeType.EmployeeTypeShort)
                   : null))
           .ForMember(dest => dest.ReleaseAs, opt => opt.MapFrom(src =>
               src.Request != null
                   ? src.Request.RequestSchedules
                       .OrderByDescending(s => s.EditedDate ?? s.CreatedDate)
                       .ThenByDescending(s => s.EndDate)
                       .Select(s => (ReleaseAs?)s.ReleaseAs)
                       .FirstOrDefault()
                     ?? ReleaseAs.Open
                   : ReleaseAs.Open));

            CreateMap<PostDto, PostInRequestListDto>()
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostID!.Value))
                .ForMember(dest => dest.TotalPositions, opt => opt.MapFrom(src => src.TotalPostPositions))
                .ForMember(dest => dest.PostParentId, opt => opt.MapFrom(src => src.PostParentID))
                .ForMember(dest => dest.DefineCallSign, opt => opt.MapFrom(src => src.DefineCallSign));
        }
    }
}

