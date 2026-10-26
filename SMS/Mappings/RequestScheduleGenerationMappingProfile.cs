using AutoMapper;
using Newtonsoft.Json;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Mappings
{
    public class RequestScheduleGenerationMappingProfile : Profile
    {
        public RequestScheduleGenerationMappingProfile()
        {
            CreateMap<RequestScheduleGeneration, RequestScheduleGenerationDto>()
                .ForMember(d => d.ReleaseBeforeDaysOfWeek, o => o.MapFrom(s => FromJson(s.ReleaseBeforeDaysOfWeek)))
                .ForMember(d => d.ReleaseBeforeDaysOfMonth, o => o.MapFrom(s => FromJson(s.ReleaseBeforeDaysOfMonth)));

            CreateMap<RequestScheduleGenerationDto, RequestScheduleGeneration>()
                .ForMember(d => d.Request, o => o.Ignore())
                .ForMember(d => d.ReleaseBeforeDaysOfWeek, o => o.MapFrom(s => ToJson(s.ReleaseBeforeDaysOfWeek)))
                .ForMember(d => d.ReleaseBeforeDaysOfMonth, o => o.MapFrom(s => ToJson(s.ReleaseBeforeDaysOfMonth)));
        }

        private static string? ToJson(List<int>? values) =>
            values is { Count: > 0 } ? JsonConvert.SerializeObject(values) : null;

        private static List<int>? FromJson(string? json) =>
            string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<List<int>>(json);
    }
}
