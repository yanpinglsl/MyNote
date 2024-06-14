using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using YY.Zhihu.SharedLibraries.Domain;
using YY.Zhihu.SharedLibraries.Repositoy;
using YY.Zhihu.SharedLibraries.Specification;

namespace YY.Zhihu.Infrastructure.Data.Repository
{
    public class EFGenericRepository<T>(AppDbContext dbContext)
        : EFReadRepository<T>(dbContext), IGenericRepository<T> where T : class, IEntity
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
