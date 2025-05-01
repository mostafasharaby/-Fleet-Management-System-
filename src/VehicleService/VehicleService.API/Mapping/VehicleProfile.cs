using AutoMapper;
using VehicleService.API.Protos;

namespace VehicleService.API.Mapping
{
    public class VehicleProfile : Profile
    {
        public VehicleProfile()
        {
            CreateMap<Domain.Models.Vehicle, VehicleResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.RegistrationNumber))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.Manufacturer))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
                .ForMember(dest => dest.Vin, opt => opt.MapFrom(src => src.VIN))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (VehicleType)src.Type))
                .ForMember(dest => dest.FuelCapacity, opt => opt.MapFrom(src => src.FuelCapacity))
                .ForMember(dest => dest.CurrentFuelLevel, opt => opt.MapFrom(src => src.CurrentFuelLevel))
                .ForMember(dest => dest.OdometerReading, opt => opt.MapFrom(src => src.OdometerReading))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (VehicleStatus)src.Status))
                .ForMember(dest => dest.AssignedDriverId, opt => opt.MapFrom(src => src.AssignedDriverId.HasValue ? src.AssignedDriverId.Value.ToString() : ""))
                .ForMember(dest => dest.LastKnownLocation, opt => opt.MapFrom(src => src.LastKnownLocation != null ? new VehicleLocation
                {
                    Latitude = src.LastKnownLocation.Latitude,
                    Longitude = src.LastKnownLocation.Longitude,
                    Speed = src.LastKnownLocation.Speed,
                    Heading = src.LastKnownLocation.Heading,
                    Timestamp = new DateTimeOffset(src.LastKnownLocation.Timestamp).ToUnixTimeSeconds()
                } : null))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => new DateTimeOffset(src.CreatedAt).ToUnixTimeSeconds()))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => new DateTimeOffset(src.UpdatedAt).ToUnixTimeSeconds()));


            CreateMap<Domain.Models.VehicleLocation, VehicleLocation>()
                  .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
                .ForMember(dest => dest.Speed, opt => opt.MapFrom(src => src.Speed))
                .ForMember(dest => dest.Heading, opt => opt.MapFrom(src => src.Heading))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => new DateTimeOffset(src.Timestamp).ToUnixTimeSeconds()));
        }
    }
}
