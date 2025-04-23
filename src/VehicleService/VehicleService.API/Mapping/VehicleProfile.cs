using AutoMapper;

namespace VehicleService.API.Mapping
{
    public class VehicleProfile : Profile
    {
        public VehicleProfile()
        {
            CreateMap<Domain.Models.Vehicle, VehicleResponse>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (VehicleType)src.Type))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (VehicleStatus)src.Status))
                .ForMember(dest => dest.FuelCapacity, opt => opt.MapFrom(src => src.FuelCapacity))
                .ForMember(dest => dest.CurrentFuelLevel, opt => opt.MapFrom(src => src.CurrentFuelLevel))
                .ForMember(dest => dest.OdometerReading, opt => opt.MapFrom(src => src.OdometerReading));
        }
    }
}
