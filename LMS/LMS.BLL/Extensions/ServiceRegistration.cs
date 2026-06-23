using LMS.BLL.Services.Implementation;
using LMS.BLL.Services.Interfaces;
using LMS.DAL.Data;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.BLL.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddBLLServices(this IServiceCollection services)
        {
            
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IInstructorService, InstructorService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
        }
    }
}