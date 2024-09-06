using UserTaskJWT.Web.Api.Endpoints;

namespace UserTaskJWT.Web.Api.Tasks.CreateTask
{
    public class CreateTaskEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("tasks", 
                    async (HttpContext httpContext, CreateTaskCommand request, CreateTaskHandler useCase) => 
                        await useCase.HandleAsync(request, httpContext.RequestAborted).ConfigureAwait(false))
                .WithTags("Tasks")
                .WithOpenApi();
        }
    }
}