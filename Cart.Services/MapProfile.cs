using AutoMapper;
using Cart.Domain.Entities;
using Cart.Services.DTOs.User;

namespace Cart.Services
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>();
        }
    }
}
