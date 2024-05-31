using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.Data;
using YY.Zhihu.SharedLibraries.Domain;

namespace YY.Zhihu.Infrastructure.Data.Repository
{
    public class EFReadRepository<T>(AppDbContext dbContext) : IReadRepository<T>
    where T : class, IAggregateRoot
    {
        public IQueryable<T> GetQueryable()
        {
            return dbContext.Set<T>().AsQueryable();
        }

        public async Task<T?> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
            where TKey : notnull
        {
            return await dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> expression,
            CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<T>().Where(expression).ToListAsync(cancellationToken);
        }

        public async Task<int> GetCountAsync(Expression<Func<T, bool>> expression,
            CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<T>().Where(expression).CountAsync(cancellationToken);
        }
    }
}
