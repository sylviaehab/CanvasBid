using AutoMapper;
using CanvasBid.Models;
using CanvasBid.DTOS.UserDTOS;
using CanvasBid.DTOS.ArtworkDTOS;
using CanvasBid.DTOS.BidDTOs;

namespace CanvasBid.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User Mappings
        CreateMap<RegisterUserDto, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => MapUserRole(src.Role)))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));
    CreateMap<User, UserResponseDto>();


        // Artwork Mappings
        CreateMap<CreateArtworkDto, Artwork>()
            .ForMember(dest => dest.StartingPrice, opt => opt.MapFrom(src => src.InitialPrice));

        // Bid Mappings
        CreateMap<CreateBidDto, Bid>();
    }

    private static UserRole MapUserRole(string roleString)
    {
        return roleString switch
        {
            "Admin" => UserRole.Admin,
            "Artist" => UserRole.Artist,
            "Buyer" => UserRole.Buyer,
            _ => UserRole.Buyer
        };
    }
}
