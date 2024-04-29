using MediatR;
using RequestResponse.API.Events;

namespace RequestResponse.API.Handlers.MakeOrder
{
    public class SmsHandler(ILogger<SmsHandler> logger) : INotificationHandler<MakeOrderEvent>
    {
        public Task Handle(MakeOrderEvent makeOrderEvent, CancellationToken cancellationToken)
        {
            Console.WriteLine($"发信处理：这是一个假装下单成功后发送给[{makeOrderEvent.Uid}]的虚拟消息：已购买：{makeOrderEvent.Name}，数量：{makeOrderEvent.Qty}");
            logger.LogInformation($"{nameof(MakeOrderEvent)}：{makeOrderEvent.Uid},{makeOrderEvent.Name}, {makeOrderEvent.Qty}");
            return Task.CompletedTask;
        }
    }
}