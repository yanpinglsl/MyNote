using YY.Zhihu.SharedLibraries.Paging;

namespace YY.Zhihu.UseCases.Contracts.Common.Interfaces
{
    public interface ICacheService<TValue> where TValue : class
    {
        string Key { get; set; }

        ValueTask<TValue?> GetOrSetByIdAsync(int id, Func<CancellationToken, Task<TValue?>> factory);

        ValueTask<TValue?> GetOrSetByIdAsync(int fid, int sid, Func<CancellationToken, Task<TValue?>> factory);

        ValueTask<PagedList<TValue>?> GetOrSetListByPageAsync(int id, Pagination pagination,
            Func<CancellationToken, Task<PagedList<TValue>?>> factory);

        ValueTask<TValue?> GetOrSetByKeyAsync(Func<CancellationToken, Task<TValue?>> factory);
    }

}
