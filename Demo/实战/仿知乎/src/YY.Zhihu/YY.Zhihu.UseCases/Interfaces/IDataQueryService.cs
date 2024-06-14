using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.SharedLibraries.Paging;

namespace YY.Zhihu.UseCases.Interfaces
{
    public interface IDataQueryService
    {
        public IQueryable<AppUser> AppUsers { get; }

        public IQueryable<FollowQuestion> FollowQuestions { get; }

        public IQueryable<FollowUser> FollowUsers { get; }

        public IQueryable<Question> Questions { get; }

        public IQueryable<Answer> Answers { get; }

        public IQueryable<AnswerLike> AnswerLikes { get; }

        Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> queryable) where T : class;

        Task<IList<T>> ToListAsync<T>(IQueryable<T> queryable) where T : class;

        Task<PagedList<T>> ToPageListAsync<T>(IQueryable<T> queryable, Pagination pagination) where T : class;
    }
}
