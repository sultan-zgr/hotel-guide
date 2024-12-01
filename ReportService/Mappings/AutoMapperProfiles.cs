using AutoMapper;
using ReportService.DTOs;
using ReportService.Models;
using shared.Messaging.Events;

namespace ReportService.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Rapor -> DTO
            CreateMap<Report, ReportDTO>().ReverseMap();
            CreateMap<CreateReportRequestDTO, Report>();
            CreateMap<Report, ReportListDTO>();

            CreateMap<HotelUpdatedEvent, Hotel>().ReverseMap();
            CreateMap<HotelDeletedEvent, Hotel>().ReverseMap();


            //Rapor->ReportRequestEvent
            CreateMap<Report, ReportRequestEvent>()
                .ForMember(dest => dest.ReportId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.RequestedAt, opt => opt.MapFrom(src => src.RequestedAt)).ReverseMap();
        }
    }
}
