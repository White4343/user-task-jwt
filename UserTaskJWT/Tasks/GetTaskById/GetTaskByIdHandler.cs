using System.Security.Claims;
using UserTaskJWT.Web.Api.JwtProviderService;

namespace UserTaskJWT.Web.Api.Tasks.GetTaskById
{
    public class GetTaskByIdHandler(ITaskRepository taskRepository)
    {
        public async Task<BaseTaskResponse> HandleAsync(Guid id, ClaimsPrincipal user, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(user);

            var userId = GetUserId.GetUserIdFromClaims(user);

            var task = await taskRepository.GetTaskAsync(id, cancellationToken).ConfigureAwait(false);

            if (task.UserId.ToString() != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to view this task.");
            }

            var baseTaskResponse = new BaseTaskResponse(task.Id, task.Title, task.Description, task.DueDate, 
                task.Status, task.Priority, task.CreatedAt, task.UpdatedAt, task.UserId);

            return baseTaskResponse;
        }
    }
}