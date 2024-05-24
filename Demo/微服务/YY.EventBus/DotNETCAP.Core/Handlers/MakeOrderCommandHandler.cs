using DotNETCAP.Core.Models;
using DotNetCore.CAP;
using MediatR;

namespace DotNETCAP.Core.Handlers
{
    public class MakeOrderCommandHandler(ICapPublisher capPublisher) : IRequestHandler<MakeOrderRequestModel,MakeOrderResponseModel>
    {

        public async Task<MakeOrderResponseModel> Handle(MakeOrderRequestModel request, CancellationToken cancellationToken)
        {
            var result = new MakeOrderResponseModel
            {
                IsSuccess = true,
                OrderId = new Guid("53d26807-ad70-4449-8479-024c54eb2020")
            };

            // 发送事件
            await capPublisher.PublishAsync("order.created", new { OrderName = "三体" }, "order.created.status",
                cancellationToken: cancellationToken);

            return result;
        }
    }
}
