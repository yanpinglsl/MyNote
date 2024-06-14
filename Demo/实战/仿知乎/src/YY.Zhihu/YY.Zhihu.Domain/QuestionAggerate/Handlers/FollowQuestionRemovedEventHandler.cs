using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.AppUserAggerate.Events;
using YY.Zhihu.Domain.QuestionAggerate.Entites;
using YY.Zhihu.SharedLibraries.Repositoy;

namespace YY.Zhihu.Domain.QuestionAggerate.Handlers
{
    public class FollowQuestionRemovedEventHandler(IRepository<Question> questions) : INotificationHandler<FollowQuestionRemovedEvent>
    {
        public async Task Handle(FollowQuestionRemovedEvent notification, CancellationToken cancellationToken)
        {
           var question = await questions.GetByIdAsync(notification.FollowQuestion.Id);
            if(question == null)
                return;
            question.FollowerCount -= 1;
            await questions.SaveChangesAsync(cancellationToken);
        }
    }
}
