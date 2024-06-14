using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.SharedLibraries.Paging;
using YY.Zhihu.UseCases.Interfaces;

namespace YY.Zhihu.Infrastructure.Data
{
    public class DataQueryService(AppDbContext dbContext) : IDataQueryService
    {
        public IQueryable<AppUser> AppUsers => dbContext.AppUsers;

        public IQueryable<FollowQuestion> FollowQuestions => dbContext.FollowQuestions;

        public IQueryable<FollowUser> FollowUsers => dbContext.FollowUsers;

        public IQueryable<Question> Questions => dbContext.Questions;

        public IQueryable<Answer> Answers => dbContext.Answers;

        public IQueryable<AnswerLike> AnswerLikes => dbContext.AnswerLikes;

        public async Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> queryable) where T : class
        {
            return await queryable.FirstOrDefaultAsync();
        }

        public async Task<IList<T>> ToListAsync<T>(IQueryable<T> queryable) where T : class
        {
            return await queryable.ToListAsync();
        }

        public async Task<PagedList<T>> ToPageListAsync<T>(IQueryable<T> queryable, Pagination pagination) where T : class
        {
            var result = await queryable
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
            return new PagedList<T>(result, result.Count, pagination);
        }
    }
}
