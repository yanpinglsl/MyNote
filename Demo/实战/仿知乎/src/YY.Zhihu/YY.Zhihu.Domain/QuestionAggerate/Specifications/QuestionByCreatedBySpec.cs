using YY.Zhihu.Domain.AppUserAggerate.Entites;
using YY.Zhihu.Domain.QuestionAggerate.Entites;

namespace YY.Zhihu.Domain.AppUserAggerate.Specifications
{
    public class QuestionByCreatedBySpec : Specification<Question>
    {
        public QuestionByCreatedBySpec(int userId, int questionId)
        {
            FilterCondition = question => question.CreatedBy.Equals(userId) && question.Id.Equals(questionId);
        }
    }

}