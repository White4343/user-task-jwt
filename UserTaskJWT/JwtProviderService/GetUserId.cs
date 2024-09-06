using System.Security.Claims;

namespace UserTaskJWT.Web.Api.JwtProviderService
{
    public static class GetUserId
    {
        public static string? GetUserIdFromClaims(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }
    }
}