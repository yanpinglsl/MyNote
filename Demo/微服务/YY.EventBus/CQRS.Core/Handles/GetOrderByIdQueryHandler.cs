using CQRS.Core.Interfaces;
using CQRS.Core.Models;

namespace CQRS.Core.Handles
{
    public class GetOrderByIdQueryHandler : IGetOrderByIdQueryHandler
    {
        public GetOrderByIdResponseModel GetOrderById(GetOrderByIdRequestModel requestModel)
        {
            var orderDetails = new GetOrderByIdResponseModel();
            // 业务逻辑
            return orderDetails;
        }
    }
}
