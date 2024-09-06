namespace UserTaskJWT.Web.Api.Tasks.CreateTask
{
    public record CreateTaskCommand(string Title, string? Description, DateTime? DueDate,
        TaskStatus Status, TaskPriority Priority);
}