using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.Zhihu.Domain.QuestionAggerate.Event;
using YY.Zhihu.HotService.Data;

namespace YY.Zhihu.HotService.Handler
{
    public class AnswerLikeEventHandler(QuestionStatManager manager)
        : INotificationHandler<AnswerLikeEvent>
    {
        public Task Handle(AnswerLikeEvent notification, CancellationToken cancellationToken)
        {
            manager.AddAnswerCount(notification.QuestionId);
            return Task.CompletedTask;
        }
    }
}
