using System.Security.Claims;
using FluentValidation;
using UserTaskJWT.Web.Api.JwtProviderService;
using UserTaskJWT.Web.Api.Validation;

namespace UserTaskJWT.Web.Api.Tasks.UpdateTask
{
    public class UpdateTaskHandler(ITaskRepository taskRepository, IValidator<UpdateTaskCommand> updateTaskValidator)
    {
        public async Task<BaseTaskResponse> HandleAsync(Guid id, UpdateTaskCommand command, ClaimsPrincipal user, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(command);

            var validationResult = await updateTaskValidator.ValidateAsync(command, cancellationToken).ConfigureAwait(false);

            CheckValidationResult.IsValidationResultValid(validationResult);

            var userId = GetUserInformation.GetUserIdFromClaims(user);

            ArgumentNullException.ThrowIfNull(userId);

            var task = await taskRepository.GetTaskAsync(id, cancellationToken).ConfigureAwait(false);

            GetUserInformation.CheckUserId(task.UserId, userId);

            var taskToUpdate = new Task
            {
                Id = id,
                Title = command.Title,
                Description = command.Description,
                DueDate = command.DueDate,
                Status = command.Status,
                Priority = command.Priority,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = task.CreatedAt,
                UserId = new Guid(userId)
            };

            await taskRepository.UpdateAsync(taskToUpdate, cancellationToken).ConfigureAwait(false);

            var baseTaskResponse = new BaseTaskResponse(taskToUpdate.Id, taskToUpdate.Title, taskToUpdate.Description, 
                taskToUpdate.DueDate, taskToUpdate.Status, taskToUpdate.Priority, taskToUpdate.CreatedAt, 
                taskToUpdate.UpdatedAt, taskToUpdate.UserId);

            return baseTaskResponse;
        }
    }
}