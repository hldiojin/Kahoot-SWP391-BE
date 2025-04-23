using AutoMapper;
using Repository.Models; // Assuming your User entity is in this namespace
using static Repository.DTO.RequestDTO; // This gives you access to LoginRequestDTO and RegisterRequestDTO

namespace Repository.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<LoginRequestDTO, UserDTO>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.userName))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.password))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<RegisterRequestDTO, User>() // Mapping directly to your User entity
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password)) // Map to PasswordHash in User
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // Assuming you might want to map from UserDTO back to other DTOs or entities
            CreateMap<UserDTO, LoginRequestDTO>()
                .ForMember(dest => dest.userName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.password, opt => opt.MapFrom(src => src.Password));

            CreateMap<UserDTO, RegisterRequestDTO>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));

            CreateMap<User, UserDTO>(); // Add mapping from User to UserDTO
            CreateMap<UserDTO, User>(); // Add mapping from UserDTO to User
        }
    }
}