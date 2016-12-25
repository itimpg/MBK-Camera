using AutoMapper;
using Mbk.Model;
using static Mbk.Helper.Converter;

namespace Mbk.Dal
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<Camera, CameraModel>().ReverseMap();

            CreateMap<HeatMap, HeatMapModel>()
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => ConvertToDateTime($"{src.Date} {src.Time}")))
                .ForMember(dest => dest.Gmt, otp => otp.MapFrom(src => ConvertToTime(src.Gmt)));
            CreateMap<HeatMapModel, HeatMap>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => ToDateString(src.DateTime)))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => ToTimeString(src.DateTime)))
                .ForMember(dest => dest.Gmt, opt => opt.MapFrom(src => ToTimeString(src.Gmt)));

            CreateMap<Counting, CountingModel>()
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => ConvertToDateTime($"{src.Date} {src.Time}")))
                .ForMember(dest => dest.Gmt, otp => otp.MapFrom(src => ConvertToTime(src.Gmt)));
            CreateMap<CountingModel, Counting>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => ToDateString(src.DateTime)))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => ToTimeString(src.DateTime)))
                .ForMember(dest => dest.Gmt, opt => opt.MapFrom(src => ToTimeString(src.Gmt)));

            CreateMap<CountingDetail, CountingDetailModel>().ReverseMap();
        }
    }
}
