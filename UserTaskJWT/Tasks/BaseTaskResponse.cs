namespace UserTaskJWT.Web.Api.Tasks
{
    public record BaseTaskResponse(Guid Id, string Title, string? Description, DateTime? DueDate, TaskStatus Status,
        TaskPriority Priority, DateTime CreatedAt, DateTime UpdatedAt, Guid UserId);
}