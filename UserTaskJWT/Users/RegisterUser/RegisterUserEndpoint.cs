using UserTaskJWT.Web.Api.Endpoints;

namespace UserTaskJWT.Web.Api.Users.RegisterUser
{
    public sealed class RegisterUserEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("users/register",
                    async (HttpContext httpContext, RegisterUserCommand request, RegisterUserHandler useCase) =>
                        await useCase.HandleAsync(request, httpContext.RequestAborted).ConfigureAwait(false))
                .WithTags("Users")
                .WithOpenApi();
        }
    }
}