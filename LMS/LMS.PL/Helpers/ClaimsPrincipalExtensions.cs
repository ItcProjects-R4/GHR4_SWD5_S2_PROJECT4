using System.Security.Claims;

namespace LMS.PL.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetFirstName(this ClaimsPrincipal user)
        {
            return user.FindFirst("FirstName")?.Value ?? user.Identity?.Name ?? "User";
        }
        public static string GetLastName(this ClaimsPrincipal user)
        {
            return user.FindFirst("LastName")?.Value ?? "";
        }
        public static string GetAvatarUrl(this ClaimsPrincipal user)
        {
            return user.FindFirst("AvatarUrl")?.Value ?? "";
        }
    }
}
