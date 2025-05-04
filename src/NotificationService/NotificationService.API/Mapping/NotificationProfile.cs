using AutoMapper;
using NotificationService.API.Protos;
using NotificationService.Domain.Models;

namespace NotificationService.API.Mapping
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<NotificationTemplate, CreateTemplateRequest>()
           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
           .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
           .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (Domain.Enums.NotificationType)src.Type))
           .ForMember(dest => dest.TitleTemplate, opt => opt.MapFrom(src => src.TitleTemplate ?? string.Empty))
           .ForMember(dest => dest.BodyTemplate, opt => opt.MapFrom(src => src.BodyTemplate ?? string.Empty))
           .ForMember(dest => dest.DefaultMetadata, opt => opt.MapFrom(src => src.DefaultMetadata ?? new Dictionary<string, string>()));
        }
    }
}
