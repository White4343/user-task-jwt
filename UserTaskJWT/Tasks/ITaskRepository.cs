namespace UserTaskJWT.Web.Api.Tasks
{
    public interface ITaskRepository
    {
        Task<Task> CreateAsync(Task task, CancellationToken cancellationToken);
        Task<IEnumerable<Task>> GetTasksByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<Task> GetTaskAsync(Guid id, CancellationToken cancellationToken);
        Task<Task> UpdateAsync(Task task, CancellationToken cancellationToken);
        Task<Task> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}