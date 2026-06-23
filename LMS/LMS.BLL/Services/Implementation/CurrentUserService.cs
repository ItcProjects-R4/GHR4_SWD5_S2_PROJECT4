using LMS.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace LMS.BLL.Services.Implementation
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor)
        : ICurrentUserService
    {
        public string UserId
        {
            get
            {
                var id = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(id))
                    throw new Exception("User not found");
                return id;
            }
        }
    }
}