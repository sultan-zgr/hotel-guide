using AutoMapper;
using HotelService.Models;
using HotelService.DTOs;

namespace HotelService.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Hotel ↔ DTO
            CreateMap<Hotel, HotelDTO>().ReverseMap();
            CreateMap<Hotel, CreateHotelDTO>().ReverseMap();
            CreateMap<Hotel, UpdateHotelDTO>().ReverseMap();

            // Contact ↔ DTO
            CreateMap<Contact, ContactDTO>().ReverseMap();
            CreateMap<Contact, CreateContactDTO>().ReverseMap();

            // ReportRequest ↔ DTO
            CreateMap<ReportRequest, ReportRequestDTO>().ReverseMap();
            CreateMap<ReportRequest, CreateReportRequestDTO>().ReverseMap();
        }
    }
}
