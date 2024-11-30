using AutoMapper;
using ReportService.DTOs;
using ReportService.Models;

namespace ReportService.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Model -> DTO
            CreateMap<Report, ReportDTO>().ReverseMap();

            // DTO -> Model
            CreateMap<CreateReportRequestDTO, Report>();
        }
    }
}
