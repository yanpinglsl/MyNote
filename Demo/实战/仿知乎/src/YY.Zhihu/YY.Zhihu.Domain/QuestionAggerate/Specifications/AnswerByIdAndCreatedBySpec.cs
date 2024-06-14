using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.SharedLibraries.Specification;

namespace YY.Zhihu.Domain.QuestionAggerate.Specifications
{
    public class AnswerByIdAndCreatedBySpec : Specification<Question>
    {
        public AnswerByIdAndCreatedBySpec(int userId,int questionId,int answerId)
        {
            FilterCondition = q => q.Id == questionId;
            AddInclude(q => q.Answers.Where(a => a.Id == answerId && a.CreatedBy == userId));
        }
    }
}
