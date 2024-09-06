using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserTaskJWT.Web.Api.Data;

namespace UserTaskJWT.Web.Api.Tasks
{
    public class TaskRepository(AppDbContext context, ILogger<TaskRepository> logger) : ITaskRepository
    {
        public async Task<Task> CreateAsync(Task task, CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(task);
                ArgumentNullException.ThrowIfNull(context.Tasks);

                await context.Tasks.AddAsync(task, cancellationToken).ConfigureAwait(false);
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                logger.LogInformation("Task created: {Task} by user {UserId}", task, task.UserId);

                return task;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IEnumerable<Task>> GetTasksByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(userId);
                ArgumentNullException.ThrowIfNull(context.Tasks);

                var tasks = await context.Tasks.Where(t => t.UserId == userId).ToListAsync(cancellationToken).ConfigureAwait(false);

                return tasks;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<Task> GetTaskAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(id);
                ArgumentNullException.ThrowIfNull(context.Tasks);

                var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
                    .ConfigureAwait(false);

                ArgumentNullException.ThrowIfNull(task);

                return task;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<Task> UpdateAsync(Task task, CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(task);
                ArgumentNullException.ThrowIfNull(context.Tasks);

                context.Tasks.Update(task);
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                logger.LogInformation("Task updated: {Task} by user {UserId}", task, task.UserId);

                return task;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<Task> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(id);
                ArgumentNullException.ThrowIfNull(context.Tasks);

                var task = await GetTaskAsync(id, cancellationToken).ConfigureAwait(false);

                context.Tasks.Remove(task);
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                logger.LogInformation("Task deleted: {Task} by user {UserId}", task, task.UserId);

                return task;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
