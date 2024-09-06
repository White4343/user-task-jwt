using System.Security.Claims;
using UserTaskJWT.Web.Api.Endpoints;

namespace UserTaskJWT.Web.Api.Tasks.DeleteTask
{
    public class DeleteTaskEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("tasks/{id}",
                async (HttpContext httpContent, Guid id, DeleteTaskHandler useCase, ClaimsPrincipal user) =>
                    await useCase.HandleAsync(id, user, httpContent.RequestAborted).ConfigureAwait(false))
                .WithTags("Tasks")
                .RequireAuthorization()
                .WithOpenApi();
        }
    }
}