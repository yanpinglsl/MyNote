using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using YY.Zhihu.SharedLibraries.Domain;
using YY.Zhihu.SharedLibraries.Repositoy;
using YY.Zhihu.SharedLibraries.Specification;

namespace YY.Zhihu.Infrastructure.Data.Repository
{
    public class EFReadRepository<T>(AppDbContext dbContext) : IReadRepository<T> where T : class, IEntity
    {
        protected readonly DbSet<T> DbSet = dbContext.Set<T>();
        public IQueryable<T> GetQueryable()
        {
            return dbContext.Set<T>().AsQueryable();
        }

        public async Task<T?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
            where TKey : notnull
        {
            return await dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<List<T>> GetListAsync(ISpecification<T>? specification = null, CancellationToken cancellationToken = default)
        {
            return await SpecificationEvaluator.GetQuery(DbSet, specification).ToListAsync(cancellationToken);
        }

        public async Task<T?> GetSingleOrDefaultAsync(ISpecification<T>? specification = null, CancellationToken cancellationToken = default)
        {
            return await SpecificationEvaluator.GetQuery(DbSet, specification).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CountAsync(ISpecification<T>? specification = null, CancellationToken cancellationToken = default)
        {
            return await SpecificationEvaluator.GetQuery(DbSet, specification).CountAsync(cancellationToken);
        }

        public async Task<bool> AnyAsync(ISpecification<T>? specification = null, CancellationToken cancellationToken = default)
        {
            return await SpecificationEvaluator.GetQuery(DbSet, specification).AnyAsync(cancellationToken);
        }
    }
}
