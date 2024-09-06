using System.Security.Claims;
using UserTaskJWT.Web.Api.Endpoints;

namespace UserTaskJWT.Web.Api.Tasks.UpdateTask
{
    public class UpdateTaskEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("/tasks/{id}", 
                async (HttpContext httpContent, Guid id, UpdateTaskCommand command, UpdateTaskHandler useCase, 
                        ClaimsPrincipal user) => 
                    await useCase.HandleAsync(id, command, user, httpContent.RequestAborted).ConfigureAwait(false))
                .WithTags("Tasks")
                .RequireAuthorization()
                .WithOpenApi();
        }
    }
}