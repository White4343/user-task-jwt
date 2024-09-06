using UserTaskJWT.Web.Api.Users;

namespace UserTaskJWT.Web.Api.JwtProviderService
{
    public interface IJwtProvider
    {
        string Generate(User user);
    }
}