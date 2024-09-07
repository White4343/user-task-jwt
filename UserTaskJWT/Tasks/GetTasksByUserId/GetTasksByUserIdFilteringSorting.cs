using FluentValidation;

namespace UserTaskJWT.Web.Api.Tasks.GetTasksByUserId
{
    public static class GetTasksByUserIdFilteringSorting
    {
        public static IEnumerable<Task> Filter(IEnumerable<Task> tasks, GetTasksByUserIdQuery query)
        {
            ArgumentNullException.ThrowIfNull(tasks);
            ArgumentNullException.ThrowIfNull(query);

            var tasksToFilter = tasks;

            if (query.Priority.HasValue)
            {
                tasksToFilter = tasksToFilter.Where(t => t.Priority == query.Priority.Value);
            }

            if (query.Status.HasValue)
            {
                tasksToFilter = tasksToFilter.Where(t => t.Status == query.Status.Value);
            }

            if (query.DueDate.HasValue)
            {
                tasksToFilter = tasksToFilter.Where(t => t.DueDate == query.DueDate.Value);
            }

            return tasksToFilter;
        }

        public static IEnumerable<Task> Sort(IEnumerable<Task> tasks, GetTasksByUserIdQuery query)
        {
            ArgumentNullException.ThrowIfNull(tasks);
            ArgumentNullException.ThrowIfNull(query);

            var tasksToSort = tasks;

            if (query.DueDateSorting.HasValue)
            {
                tasksToSort = query.DueDateSorting.Value switch
                {
                    Sorting.Asc => tasksToSort.OrderBy(t => t.DueDate),
                    Sorting.Desc => tasksToSort.OrderByDescending(t => t.DueDate),
                    _ => throw new ValidationException("Argument was out of range")
                };
            }

            if (query.PrioritySorting.HasValue)
            {
                tasksToSort = query.PrioritySorting.Value switch
                {
                    Sorting.Asc => tasksToSort.OrderBy(t => t.Priority),
                    Sorting.Desc => tasksToSort.OrderByDescending(t => t.Priority),
                    _ => throw new ValidationException("Argument was out of range")
                };
            }

            return tasksToSort;
        }
    }
}