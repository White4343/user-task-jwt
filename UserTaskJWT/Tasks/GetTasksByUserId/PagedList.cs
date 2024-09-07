using Microsoft.EntityFrameworkCore;
#pragma warning disable CA1000
#pragma warning disable CA1002

namespace UserTaskJWT.Web.Api.Tasks.GetTasksByUserId
{
    public class PagedList<T>
    {
        private PagedList(List<T> items, int page, int pageSize, int totalCount)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public List<T> Items { get; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public bool HasNextPage => Page * PageSize < TotalCount;

        public bool HasPreviousPage => Page > 1;

        public static PagedList<T> Create(IEnumerable<T> query, int page, int pageSize)
        {
            var enumerable = query.ToList();
            var totalCount = enumerable.Count;
            
            var items = enumerable.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedList<T>(items, page, pageSize, totalCount);
        }
    }
}