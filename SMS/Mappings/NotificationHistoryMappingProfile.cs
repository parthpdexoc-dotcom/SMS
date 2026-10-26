using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using System.Linq;

namespace SMS.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping notification history entities to list DTOs.
    /// </summary>
    public class NotificationHistoryMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationHistoryMappingProfile"/> class.
        /// </summary>
        public NotificationHistoryMappingProfile()
        {
            CreateMap<NotificationHistory, NotificationHistoryListItemDto>()
                .ForMember(
                    dest => dest.NotificationHistoryId,
                    opt => opt.MapFrom(src => src.NotificationHistoryID))
                .ForMember(
                    dest => dest.SentBy,
                    opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(
                    dest => dest.ToCount,
                    opt => opt.MapFrom(src =>
                        src.Recipients == null
                            ? 0
                            : src.Recipients.Count(r => r.RecipientType == NotificationRecipientType.To)))
                .ForMember(
                    dest => dest.CcCount,
                    opt => opt.MapFrom(src =>
                        src.Recipients == null
                            ? 0
                            : src.Recipients.Count(r => r.RecipientType == NotificationRecipientType.Cc)))
                .ForMember(
                    dest => dest.BccCount,
                    opt => opt.MapFrom(src =>
                        src.Recipients == null
                            ? 0
                            : src.Recipients.Count(r => r.RecipientType == NotificationRecipientType.Bcc)))
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status == NotificationStatus.Sent ? "Sent" : "Failed"));
        }
    }
}
