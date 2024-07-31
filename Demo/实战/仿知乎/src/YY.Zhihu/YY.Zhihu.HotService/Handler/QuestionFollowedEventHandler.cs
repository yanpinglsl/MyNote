using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.AppUserAggerate.Events;
using YY.Zhihu.Domain.QuestionAggerate.Event;
using YY.Zhihu.HotService.Data;

namespace YY.Zhihu.HotService.Handler
{
    public class QuestionFollowedEventHandler(QuestionStatManager manager)
        : INotificationHandler<FollowQuestionAddedEvent>
    {
        public Task Handle(FollowQuestionAddedEvent notification, CancellationToken cancellationToken)
        {
            manager.AddFollowCount(notification.FollowQuestion.QuestionId);
            return Task.CompletedTask;
        }
    }
}
