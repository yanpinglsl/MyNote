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
    public class FollowQuestionAddedEventHandler(IRepository<Question> questions) : INotificationHandler<FollowQuestionAddedEvent>
    {
        public async Task Handle(FollowQuestionAddedEvent notification, CancellationToken cancellationToken)
        {
           var question = await questions.GetByIdAsync(notification.FollowQuestion.Id);
            if(question == null)
                return;
            question.FollowerCount += 1;
            await questions.SaveChangesAsync(cancellationToken);
        }
    }
}
