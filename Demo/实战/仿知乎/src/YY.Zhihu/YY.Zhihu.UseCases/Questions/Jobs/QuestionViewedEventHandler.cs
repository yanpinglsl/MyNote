using MediatR;
using YY.Zhihu.Domain.QuestionAggerate.Event;

namespace YY.Zhihu.UseCases.Questions.Jobs
{
    public class QuestionViewedEventHandler(QuestionViewCountService questionViewCountService)
        : INotificationHandler<QuestionViewedEvent>
    {
        public Task Handle(QuestionViewedEvent notification, CancellationToken cancellationToken)
        {
            questionViewCountService.AddViewCount(notification.QuestionId);
            return Task.CompletedTask;
        }
    }

}

