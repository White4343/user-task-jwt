using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using UserTaskJWT.Web.Api.Endpoints;

namespace UserTaskJWT.Web.Api.Tasks.CreateTask
{
    public sealed class CreateTaskEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("tasks",
                    async (HttpContext httpContext, CreateTaskCommand request, CreateTaskHandler useCase, 
                            ClaimsPrincipal user) => 
                        await useCase.HandleAsync(request, user, httpContext.RequestAborted).ConfigureAwait(false))
                .WithTags("Tasks")
                .RequireAuthorization()
                .WithOpenApi();
        }
    }
}