using AutoMapper;
using DriverService.API.Protos;
using DriverService.Domain.Models;

namespace DriverService.API.Mapping
{
    public class DriverProfile : Profile
    {
        public DriverProfile()
        {
            CreateMap<Driver, DriverResponse>()
                .ForMember(dest => dest.LicenseExpiry, opt => opt.MapFrom(src => new DateTimeOffset(src.LicenseExpiry).ToUnixTimeSeconds()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => new DateTimeOffset(src.CreatedAt).ToUnixTimeSeconds()))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => new DateTimeOffset(src.UpdatedAt).ToUnixTimeSeconds()));

            CreateMap<Driver, DriverResponse>()
              .ForMember(dest => dest.LicenseExpiry, opt => opt.MapFrom(src => new DateTimeOffset(src.LicenseExpiry).ToUnixTimeSeconds()))
              .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => new DateTimeOffset(src.CreatedAt).ToUnixTimeSeconds()))
              .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => new DateTimeOffset(src.UpdatedAt).ToUnixTimeSeconds()))
              .ReverseMap();
        }

    }
}

