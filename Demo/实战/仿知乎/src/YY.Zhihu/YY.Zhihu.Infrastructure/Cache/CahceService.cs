using YY.Zhihu.SharedLibraries.Paging;
using YY.Zhihu.UseCases.Contracts.Common.Interfaces;
using ZiggyCreatures.Caching.Fusion;

namespace YY.Zhihu.Infrastructure.Cache
{
    public class CacheService<TValue>(IFusionCache cache) : ICacheService<TValue> where TValue : class
    {
        public string Key { get; set; } = typeof(TValue).ToString();

        public async ValueTask<TValue?> GetOrSetByIdAsync(int id, Func<CancellationToken, Task<TValue?>> factory)
        {
            return await cache.GetOrSetAsync<TValue?>($"{Key}:{id}", factory);
        }

        public async ValueTask<TValue?> GetOrSetByIdAsync(int fid, int sid, Func<CancellationToken, Task<TValue?>> factory)
        {
            return await cache.GetOrSetAsync<TValue?>($"{Key}:{fid}:{sid}", factory);
        }

        public async ValueTask<PagedList<TValue>?> GetOrSetListByPageAsync(int id, Pagination pagination,
            Func<CancellationToken, Task<PagedList<TValue>?>> factory)
        {
            var key = $"{Key}:{id}:{pagination.PageNumber}-{pagination.PageSize}";

            return await cache.GetOrSetAsync<PagedList<TValue>?>(key, factory);
        }

        public async ValueTask<TValue?> GetOrSetByKeyAsync(Func<CancellationToken, Task<TValue?>> factory)
        {
            return await cache.GetOrSetAsync<TValue?>(Key, factory);
        }
    }
}
