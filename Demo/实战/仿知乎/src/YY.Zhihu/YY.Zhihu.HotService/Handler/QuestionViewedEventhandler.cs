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
    public class QuestionViewedEventhandler(QuestionStatManager manager)
        : INotificationHandler<QuestionViewedEvent>
    {
        public Task Handle(QuestionViewedEvent notification, CancellationToken cancellationToken)
        {
            manager.AddViewCount(notification.QuestionId);
            return Task.CompletedTask;
        }
    }
}
