using UserTaskJWT.Web.Api.Users;

namespace UserTaskJWT.Web.Api.Tasks
{
    public class Task
    {
        public Guid Id { get; set; }

        public required string Title { get; set; }; 

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public required TaskStatus Status { get; set; }

        public required TaskPriority Priority { get; set; }

        public required DateTime CreatedAt { get; set; }

        public required DateTime UpdatedAt { get; set; }

        public required Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }

    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }
}
