using YY.Zhihu.Domain.AppUserAggerate.Entites;

namespace YY.Zhihu.Domain.AppUserAggerate.Specifications
{
    public class FollowQuestionByIdSpec : Specification<AppUser>
    {
        public FollowQuestionByIdSpec(int userId, int questionId)
        {
            FilterCondition = user => user.Id.Equals(userId);

            AddInclude(user => user.FollowQuestions.Where(fq => fq.QuestionId.Equals(questionId)));
        }
    }

}