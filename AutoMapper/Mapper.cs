using AutoMapper;
using BackTareas.Models;
using BackTareas.Models.DTO;

namespace BackTareas.AutoMapper
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Work, WorkWithUser>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
            CreateMap<Work, WorkDTO>().ReverseMap();
            CreateMap<WorkCreationDTO,Work>();
            CreateMap<WorkUpdateDTO,Work>();

            CreateMap<User,UserWithWorksDTO>()
                .ForMember(dest => dest.Works, opt => opt.MapFrom(src => src.Works));
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<UserCreationDTO,User>();
            CreateMap<UserUpdateDTO,User>();
            CreateMap<UserLoginDTO, User>();
        }
    }
}
