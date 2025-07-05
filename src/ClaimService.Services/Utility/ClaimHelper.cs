using System.Security.Claims;

namespace ClaimService.Services.Utility
{

    public static class UserClaimHelper
    {
        public static string? GetClaimValue(ClaimsPrincipal user, string claimType)
        {
            return user?.Claims?.FirstOrDefault(c => c.Type == claimType)?.Value;
        }

        public static long GetUserId(ClaimsPrincipal user)
        {
            return Convert.ToInt64(GetClaimValue(user, ClaimTypes.NameIdentifier)); // or "sub" if using OIDC
        }

        public static string? GetEmail(ClaimsPrincipal user)
        {
            return GetClaimValue(user, ClaimTypes.Email);
        }

        public static string? GetRole(ClaimsPrincipal user)
        {
            return GetClaimValue(user, ClaimTypes.Role);
        }
    }

}
