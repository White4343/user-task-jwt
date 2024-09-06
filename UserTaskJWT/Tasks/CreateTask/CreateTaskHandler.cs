using System.Security.Claims;
using FluentValidation;

namespace UserTaskJWT.Web.Api.Tasks.CreateTask
{
    public class CreateTaskHandler(ITaskRepository taskRepository, IValidator<CreateTaskCommand> createTaskValidator)
    {
        public async Task<Task> HandleAsync(CreateTaskCommand command, ClaimsPrincipal user, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(user);

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            ArgumentNullException.ThrowIfNull(userId);

            var validationResult = await createTaskValidator.ValidateAsync(command, cancellationToken).ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

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

            return task;
        }
    }
}