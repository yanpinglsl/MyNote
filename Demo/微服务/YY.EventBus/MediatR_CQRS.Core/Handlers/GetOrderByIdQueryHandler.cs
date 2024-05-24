using MediatR;
using MediatR_CQRS.Core.Models;

namespace MediatR_CQRS.Core.Handlers
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdRequestModel, GetOrderByIdResponseModel>
    {

        public Task<GetOrderByIdResponseModel> Handle(GetOrderByIdRequestModel request, CancellationToken cancellationToken)
        {
            var orderDetails = new GetOrderByIdResponseModel()
            {
                OrderId = request.OrderId,
                OrderName = "测试订单"
            };
            // 业务逻辑
            return Task.FromResult(orderDetails);
        }
    }
}
