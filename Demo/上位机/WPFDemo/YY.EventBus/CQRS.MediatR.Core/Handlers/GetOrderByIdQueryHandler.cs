using MediatR;
using CQRS.MediatR.Core.Core.Models;

namespace CQRS.MediatR.Core.Handlers
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
