using System.Security.Claims;
using UserTaskJWT.Web.Api.Endpoints;

namespace UserTaskJWT.Web.Api.Tasks.GetTaskById
{
    public class GetTaskByIdEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("tasks/{id}",
                    async (HttpContext httpContext, Guid id, GetTaskByIdHandler useCase,
                            ClaimsPrincipal user) =>
                        await useCase.HandleAsync(id, user, httpContext.RequestAborted).ConfigureAwait(false))
                .WithTags("Tasks")
                .RequireAuthorization()
                .WithOpenApi();
        }
    }
}