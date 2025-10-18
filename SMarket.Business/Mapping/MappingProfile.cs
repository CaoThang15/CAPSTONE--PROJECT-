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
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                    $"{src.Address}, {src.Ward}, {src.Province}".Trim(',', ' ')));
        }
    }
}