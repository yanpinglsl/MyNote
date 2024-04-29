using MediatR;
using CQRS.MediatR.Core.Core.Models;

namespace CQRS.MediatR.Core.Handlers
{
    public class MakeOrderCommandHandler(IMediator mediator) : IRequestHandler<MakeOrderRequestModel,MakeOrderResponseModel>
    {

        public async Task<MakeOrderResponseModel> Handle(MakeOrderRequestModel request, CancellationToken cancellationToken)
        {
            var result = new MakeOrderResponseModel
            {
                IsSuccess = true,
                OrderId = new Guid("53d26807-ad70-4449-8479-024c54eb2020")
            };

            //应用可以根据实际情况选择CQRS模式还是请求响应模式，不能一概而论
            // 发送事件给订阅方
            //var makeOrderEvent = new MakeOrderEvent("张三", "三体", 1);
            //await mediator.Publish(makeOrderEvent, cancellationToken);

            return result;
        }
    }
}
