using UserTaskJWT.Web.Api.Endpoints;

namespace UserTaskJWT.Web.Api.Users.Login
{
    public class LoginEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("users/login", 
                    async (HttpContext httpContext, LoginCommand request, LoginHandler useCase) => 
                        await useCase.HandleAsync(request, httpContext.RequestAborted).ConfigureAwait(false))
                .WithTags("Users")
                .WithOpenApi();
        }
    }
}
