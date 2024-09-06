namespace UserTaskJWT.Web.Api.Tasks.CreateTask
{
    public class CreateTaskHandler(ITaskRepository taskRepository)
    {
        public async Task<Task> HandleAsync(CreateTaskCommand command, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(command);

            // validate if the command is valid

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
                UserId = Guid.Empty
            };

            await taskRepository.CreateAsync(task, cancellationToken).ConfigureAwait(false);

            return task;
        }
    }
}