using DotNETCAP.Core.Models;
using MediatR;
namespace DotNETCAP.Core.Handlers
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
