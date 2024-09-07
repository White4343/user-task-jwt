using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UserTaskJWT.Web.Api.Endpoints;

namespace UserTaskJWT.Web.Api.Tasks.GetTasksByUserId
{
    public class GetTasksByUserIdEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/tasks",
                    async (HttpContext httpContent, [AsParameters] GetTasksByUserIdQuery query, GetTasksByUserIdHandler useCase,
                            ClaimsPrincipal user) =>
                        await useCase.HandleAsync(query, user, httpContent.RequestAborted).ConfigureAwait(false))
                .WithTags("Tasks")
                .RequireAuthorization()
                .WithOpenApi();
        }
    }
}