using MediatR;
using MediatR_Mediator.API.Events;

namespace MediatR_Mediator.API.Handlers.MakeOrder
{
    public class StockHandler(ILogger<StockHandler> logger) : INotificationHandler<MakeOrderEvent>
    {
        public Task Handle(MakeOrderEvent makeOrderEvent, CancellationToken cancellationToken)
        {
            Console.WriteLine($"库存处理：[{makeOrderEvent.Name}]的库存数量减少{makeOrderEvent.Qty}");
            logger.LogInformation($"{nameof(MakeOrderEvent)}：{makeOrderEvent.Uid},{makeOrderEvent.Name},{makeOrderEvent.Qty}");
            return Task.CompletedTask;
        }
    }
}
