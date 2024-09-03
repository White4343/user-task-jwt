using Task = UserTaskJWT.Web.Api.Tasks.Task;

namespace UserTaskJWT.Web.Api.Users
{
    public class User
    {
        public Guid Id { get; set; }

        public required string Username { get; set; }

        public required string Email { get; set; }

        public required string PasswordHash { get; set; }

        public required DateTime CreatedAt { get; set; }

        public required DateTime UpdatedAt { get; set; }

        public ICollection<Task> Tasks { get; } = new List<Task>();
    }
}