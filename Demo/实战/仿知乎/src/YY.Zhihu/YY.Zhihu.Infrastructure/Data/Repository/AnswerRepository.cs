using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.Interfaces;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.Domain.QuestionAggerate.Specifications;
using YY.Zhihu.SharedLibraries.Repositoy;

namespace YY.Zhihu.Infrastructure.Data.Repository
{
    public class AnswerRepository(AppDbContext dbContext) : EFGenericRepository<Answer>(dbContext), IAnswerRepository
    {
        public async Task<Answer?> GetAnswerByIdWithLikeByUserIdAsync(AnswerByIdWithLikeByUserIdSpec specification, CancellationToken cancellationToken = default)
        {
            return await SpecificationEvaluator.GetQuery(DbSet, specification)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}
