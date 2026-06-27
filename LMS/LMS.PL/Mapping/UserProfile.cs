using AutoMapper;
using LMS.Domain.Models;
using LMS.Domain.ViewModels.Account;

namespace LMS.PL.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterViewModel, ApplicationUser>()
                .ForMember(des => des.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(des => des.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<ApplicationUser, ProfileSettingsViewModel>();

            CreateMap<UpdateProfileViewModel, ApplicationUser>()
                .ForMember(dest => dest.Email, opt => opt.Ignore());

        }
    }
}
