using AutoMapper;
using TelemetryService.Application.DTOs;
using TelemetryService.Domain.Models;

namespace TelemetryService.Application.Mapping
{
    public class TelemetryProfile : Profile
    {
        public TelemetryProfile()
        {
            CreateMap<TelemetryData, TelemetryDataDto>().ReverseMap();
            CreateMap<AlertThreshold, AlertThresholdDto>().ReverseMap();
        }
    }
}
