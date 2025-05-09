using AutoMapper;
using Okala.Domain.AggregateRoots;
using OKala.Application.Dto;

namespace OKala.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<WeatherData, WeatherDataDto>()
                .ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => string.Format("{0} °C", src.Main.Temp)))
                .ForMember(dest => dest.Humidity, opt => opt.MapFrom(src => src.Main.Humidity+" %"))
                .ForMember(dest => dest.WindSpeed, opt => opt.MapFrom(src => src.Wind.Speed))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Coord.Lat))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Coord.Lon))
                .ReverseMap();

            CreateMap<PollutionData, WeatherDataDto>()
                .ForMember(dest => dest.AirQualityIndex, opt => opt.MapFrom(src => src.List.FirstOrDefault().Main.AQI))
                .ForMember(dest => dest.Pollutants, opt => opt.MapFrom(src => src.List.FirstOrDefault().Components))
                .ReverseMap();

            CreateMap<Pollutants, PollutionData.Polluts.Component>()
                .ForMember(dest => dest.PM2_5, opt => opt.MapFrom(src => src.PM25))
                .ReverseMap();
        }
    }
}
