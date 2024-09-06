namespace UserTaskJWT.Web.Api.JwtProviderService
{
    public class JwtOptions
    {
        public required string Issuer { get; init; } = "UserTaskJWT.Web.Api";

        public required string Audience { get; init; } = "UserTaskJWT.Web.Api";

        public required string SecretKey { get; init; } 
            = "UserTaskJWT.Web.ApiUserTaskJWT.Web.ApiUserTaskJWT.Web.ApiUserTaskJWT.Web.Api";
    }
}