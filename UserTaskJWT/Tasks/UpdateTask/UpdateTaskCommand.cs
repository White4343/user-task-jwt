namespace UserTaskJWT.Web.Api.Tasks.UpdateTask
{
    public record UpdateTaskCommand(string Title, string? Description, DateTime? DueDate, TaskStatus Status,
        TaskPriority Priority);
}