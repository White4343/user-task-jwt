using System.Security.Claims;
using FluentValidation;
using UserTaskJWT.Web.Api.JwtProviderService;
using UserTaskJWT.Web.Api.Validation;

namespace UserTaskJWT.Web.Api.Tasks.CreateTask
{
    public class CreateTaskHandler(ITaskRepository taskRepository, IValidator<CreateTaskCommand> createTaskValidator)
    {
        public async Task<BaseTaskResponse> HandleAsync(CreateTaskCommand command, ClaimsPrincipal user, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(user);

            var userId = GetUserInformation.GetUserIdFromClaims(user);

            ArgumentNullException.ThrowIfNull(userId);

            var validationResult = await createTaskValidator.ValidateAsync(command, cancellationToken).ConfigureAwait(false);

            CheckValidationResult.IsValidationResultValid(validationResult);

            var task = new Task
            {
                Id = Guid.NewGuid(),
                Title = command.Title,
                Description = command.Description,
                DueDate = command.DueDate,
                Status = command.Status,
                Priority = command.Priority,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = new Guid(userId)
            };

            await taskRepository.CreateAsync(task, cancellationToken).ConfigureAwait(false);

            var createdTask = new BaseTaskResponse(task.Id, task.Title, task.Description, task.DueDate, task.Status, task.Priority, task.CreatedAt, task.UpdatedAt, task.UserId);

            return createdTask;
        }
    }
}