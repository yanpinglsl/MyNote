namespace YY.Zhihu.SharedLibraries.Paging
{
    public class Pagination(int pageNumber = 1, int pageSize = 10)
    {
        private const int MaxPageSize = 100;

        public int PageNumber { get; } = pageNumber;

        public int PageSize { get; } = pageSize > MaxPageSize ? MaxPageSize : pageSize;
    }
}

