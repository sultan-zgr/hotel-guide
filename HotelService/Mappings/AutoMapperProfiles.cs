using AutoMapper;
using HotelService.Models;
using HotelService.DTOs.HotelDTOs;
using HotelService.DTOs.ContactDTOs;

namespace HotelService.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Hotel, HotelDTO>()
                .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => src.Contacts));

            CreateMap<CreateHotelDTO, Hotel>();
            CreateMap<UpdateHotelDTO, Hotel>();

            CreateMap<Contact, ContactDTO>();
            CreateMap<CreateContactDTO, Contact>();

            CreateMap<Hotel, HotelAddedEvent>();
            CreateMap<Hotel, HotelUpdatedEvent>();
        }
    }
}