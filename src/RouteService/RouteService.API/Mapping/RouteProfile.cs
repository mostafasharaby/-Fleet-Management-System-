using AutoMapper;
using RouteService.API.Protos;
using RouteService.Domain.Enums;
using RouteService.Domain.Models;

namespace RouteService.API.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Domain.Models.Route, RouteMessage>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.VehicleId.ToString()))
                .ForMember(dest => dest.DriverId, opt => opt.MapFrom(src => src.DriverId.ToString()))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString("o")))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.HasValue ? src.EndTime.Value.ToString("o") : ""))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString().ToUpper()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString("o")))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.HasValue ? src.UpdatedAt.Value.ToString("o") : ""))
                .ForMember(dest => dest.EstimatedDurationMinutes, opt => opt.MapFrom(src => (int)src.EstimatedDuration.TotalMinutes));

            CreateMap<RouteMessage, Domain.Models.Route>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => Guid.Parse(src.VehicleId)))
                .ForMember(dest => dest.DriverId, opt => opt.MapFrom(src => Guid.Parse(src.DriverId)))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => DateTime.Parse(src.StartTime)))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.EndTime) ? (DateTime?)null : DateTime.Parse(src.EndTime)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<Protos.RouteStatus>(src.Status, true)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Parse(src.CreatedAt)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.UpdatedAt) ? (DateTime?)null : DateTime.Parse(src.UpdatedAt)))
                .ForMember(dest => dest.EstimatedDuration, opt => opt.MapFrom(src => TimeSpan.FromMinutes(src.EstimatedDurationMinutes)))
                .ConstructUsing((src, ctx) =>
                {
                    var route = new Domain.Models.Route(
                        src.Name,
                        Guid.Parse(src.VehicleId),
                        Guid.Parse(src.DriverId),
                        DateTime.Parse(src.StartTime),
                        new List<RouteStop>() // Empty list 
                    );
                    return route;
                });

            CreateMap<RouteStop, RouteStopMessage>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.PlannedArrivalTime, opt => opt.MapFrom(src => src.PlannedArrivalTime.ToString("o")))
                .ForMember(dest => dest.ActualArrivalTime, opt => opt.MapFrom(src => src.ActualArrivalTime.HasValue ? src.ActualArrivalTime.Value.ToString("o") : ""))
                .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => src.DepartureTime.HasValue ? src.DepartureTime.Value.ToString("o") : ""))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (StopStatus)(int)src.Status));

            CreateMap<RouteStopMessage, RouteStop>()
                .ConstructUsing((src, ctx) =>
                {
                    return new RouteStop(
                        src.SequenceNumber,
                        src.Name,
                        src.Address,
                        src.Latitude,
                        src.Longitude,
                        DateTime.Parse(src.PlannedArrivalTime),
                        src.EstimatedDurationMinutes,
                        src.Notes
                    );
                });

            CreateMap<RouteMessage, RouteResponse>()
                .ForMember(dest => dest.Route, opt => opt.MapFrom(src => src));

            CreateMap<Domain.Models.Route, RouteResponse>()
                .ForMember(dest => dest.Route, opt => opt.MapFrom(src => src));

            CreateMap<RouteStopMessage, RouteStopResponse>()
                .ForMember(dest => dest.Stop, opt => opt.MapFrom(src => src));

            CreateMap<RouteStop, RouteStopResponse>()
                .ForMember(dest => dest.Stop, opt => opt.MapFrom(src => src));
        }
    }
}

