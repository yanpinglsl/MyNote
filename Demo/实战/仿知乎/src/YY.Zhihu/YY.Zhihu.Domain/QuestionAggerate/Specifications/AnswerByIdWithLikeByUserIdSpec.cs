using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.SharedLibraries.Specification;

namespace YY.Zhihu.Domain.QuestionAggerate.Specifications
{
    public class AnswerByIdWithLikeByUserIdSpec:Specification<Answer>
    {
        public AnswerByIdWithLikeByUserIdSpec(int answerId ,int userId) 
        {
            FilterCondition = a => a.Id == answerId;
            AddInclude(a => a.AnswerLikes.Where(l => l.UserId == userId));
        }
    }
}
