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
    public class AnswerCreatedEventHandler(QuestionStatManager manager)
        : INotificationHandler<AnswerCreatedEvent>
    {
        public Task Handle(AnswerCreatedEvent notification, CancellationToken cancellationToken)
        {
            manager.AddAnswerCount(notification.QuestionId);
            return Task.CompletedTask;
        }
    }
}
