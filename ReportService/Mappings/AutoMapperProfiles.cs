using AutoMapper;
using ReportService.DTOs;
using ReportService.Models;

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
        }
    }

}
