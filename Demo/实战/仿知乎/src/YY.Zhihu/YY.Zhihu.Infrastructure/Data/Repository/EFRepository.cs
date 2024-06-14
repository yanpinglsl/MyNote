using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.SharedLibraries.Domain;
using YY.Zhihu.SharedLibraries.Repositoy;
namespace YY.Zhihu.Infrastructure.Data.Repository
{
    public class EFRepository<T>(AppDbContext dbContext) : EFReadRepository<T>(dbContext), IRepository<T>
        where T : class, IEntity, IAggregateRoot
    {
        public T Add(T entity)
        {
            dbContext.Set<T>().Add(entity);
            return entity;
        }

        public void Update(T entity)
        {
            dbContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            dbContext.Set<T>().Remove(entity);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
