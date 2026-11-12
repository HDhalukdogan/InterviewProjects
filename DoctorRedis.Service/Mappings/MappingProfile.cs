using AutoMapper;
using DoctorRedis.Domain.Entities;
using DoctorRedis.Service.DTOs;

namespace DoctorRedis.Service.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Doctor,DoctorDto>().ReverseMap();
        }
    }
}
