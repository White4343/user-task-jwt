using System.Security.Claims;

namespace UserTaskJWT.Web.Api.JwtProviderService
{
    public static class GetUserInformation
    {
        public static string? GetUserIdFromClaims(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }

        public static void CheckUserId(Guid authorUserId, string userId)
        {
            if (authorUserId.ToString() != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to view this task.");
            }
        }
    }
}