using AutoMapper;
using Mbk.Model;
using System;
using System.Globalization;

namespace Mbk.Dal
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<Camera, CameraModel>().ReverseMap();

            CreateMap<HeatMap, HeatMapModel>()
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src =>
                     DateTime.ParseExact($"{src.Date} {src.Time}", "yyyy-MM-dd HH:MM:ss", CultureInfo.InvariantCulture)));
            CreateMap<HeatMapModel, HeatMap>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.DateTime.Date.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.DateTime.Date.ToString("HH:MM:ss")));

            CreateMap<Counting, CountingModel>()
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src =>
                     DateTime.ParseExact($"{src.Date} {src.Time}", "yyyy-MM-dd HH:MM:ss", CultureInfo.InvariantCulture)));
            CreateMap<CountingModel, Counting>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.DateTime.Date.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.DateTime.Date.ToString("HH:MM:ss")));
        }
    }
}
