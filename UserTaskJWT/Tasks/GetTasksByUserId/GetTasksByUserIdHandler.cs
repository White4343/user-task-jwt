using System.Security.Claims;
using FluentValidation;
using UserTaskJWT.Web.Api.JwtProviderService;
using UserTaskJWT.Web.Api.Validation;

namespace UserTaskJWT.Web.Api.Tasks.GetTasksByUserId
{
    public class GetTasksByUserIdHandler(ITaskRepository taskRepository, IValidator<GetTasksByUserIdQuery> getTaskByUserIdValidator)
    {
        public async Task<PagedList<BaseTaskResponse>> HandleAsync(GetTasksByUserIdQuery query, 
            ClaimsPrincipal user, CancellationToken cancellationToken)
        {
            var validationResults = await getTaskByUserIdValidator.ValidateAsync(query, cancellationToken).ConfigureAwait(false);

            CheckValidationResult.IsValidationResultValid(validationResults);

            var userId = GetUserInformation.GetUserIdFromClaims(user);

            ArgumentNullException.ThrowIfNull(userId);
            ArgumentNullException.ThrowIfNull(query);
            
            var tasks = await taskRepository.GetTasksByUserIdAsync(new Guid(userId), cancellationToken)
                .ConfigureAwait(false);

            tasks = GetTasksByUserIdFilteringSorting.Filter(tasks, query);
            tasks = GetTasksByUserIdFilteringSorting.Sort(tasks, query);

            var baseTaskResponses = tasks.Select(t => new BaseTaskResponse(t.Id, t.Title, 
                t.Description, t.DueDate, t.Status, t.Priority, t.CreatedAt, t.UpdatedAt, t.UserId));

            // pagination
            var taskResponsePages =
                PagedList<BaseTaskResponse>.Create(baseTaskResponses, query.Page, query.PageSize);

            ArgumentNullException.ThrowIfNull(taskResponsePages);

            return taskResponsePages;
        }
    }
}