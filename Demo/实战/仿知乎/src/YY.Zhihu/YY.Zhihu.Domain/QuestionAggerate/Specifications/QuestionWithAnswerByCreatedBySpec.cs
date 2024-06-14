using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.QuestionAggerate.Entites;

namespace YY.Zhihu.Domain.QuestionAggerate.Specifications
{
    public class QuestionWithAnswerByCreatedBySpec : Specification<Question>
    {
        public QuestionWithAnswerByCreatedBySpec(int userId, int questionId)
        {
            FilterCondition = question => question.CreatedBy.Equals(userId) && question.Id.Equals(questionId);
            AddInclude(q => q.Answers.Take(1));
        }
    }
}
