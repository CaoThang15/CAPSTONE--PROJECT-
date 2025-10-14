using AutoMapper;
using SMarket.Business.DTOs;
using SMarket.DataAccess.Models;

namespace SMarket.Business.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();
        }
    }
}