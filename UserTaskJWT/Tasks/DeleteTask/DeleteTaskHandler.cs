using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UserTaskJWT.Web.Api.JwtProviderService;

namespace UserTaskJWT.Web.Api.Tasks.DeleteTask
{
    public class DeleteTaskHandler(ITaskRepository taskRepository)
    {
        public async Task<IResult> HandleAsync(Guid id, ClaimsPrincipal user,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(id);

            var userId = GetUserInformation.GetUserIdFromClaims(user);

            ArgumentNullException.ThrowIfNull(userId);

            var task = await taskRepository.GetTaskAsync(id, cancellationToken).ConfigureAwait(false);

            GetUserInformation.CheckUserId(task.UserId, userId);

            await taskRepository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

            return Results.Accepted();
        }
    }
}