using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.Domain.QuestionAggerate.Specifications;
using YY.Zhihu.SharedLibraries.Repositoy;
using YY.Zhihu.SharedLibraries.Specification;

namespace YY.Zhihu.Domain.Interfaces
{
    public interface IAnswerRepository : IGenericRepository<Answer>
    {
        Task<Answer?> GetAnswerByIdWithLikeByUserIdAsync(AnswerByIdWithLikeByUserIdSpec specification,
            CancellationToken cancellationToken = default);
    }
}
