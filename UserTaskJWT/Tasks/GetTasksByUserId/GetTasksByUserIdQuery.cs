namespace UserTaskJWT.Web.Api.Tasks.GetTasksByUserId
{
    public record GetTasksByUserIdQuery(TaskPriority? Priority, TaskStatus? Status, DateTime? DueDate, 
        Sorting? DueDateSorting, Sorting? PrioritySorting, int Page, int PageSize);
    
    public enum Sorting
    {
        Asc,
        Desc
    }
}