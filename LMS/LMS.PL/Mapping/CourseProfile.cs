using AutoMapper;
using LMS.Domain.Models;
using LMS.Domain.ViewModels.Shared;
using LMS.Domain.ViewModels.Student.CourseDetails;  
namespace LMS.PL.Mapping
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            
            CreateMap<Course, CourseViewModel>()
                .ForMember(dest => dest.Modules,
                    opt => opt.MapFrom(src => src.Modules))
                .ReverseMap();

           
            CreateMap<Module, ModuleViewModel>()
                .ForMember(dest => dest.Contents,  
                    opt => opt.MapFrom(src => src.Contents))
                .ReverseMap();

            
            CreateMap<Content, ContentViewModel>()
                .ReverseMap();
        }
    }
}