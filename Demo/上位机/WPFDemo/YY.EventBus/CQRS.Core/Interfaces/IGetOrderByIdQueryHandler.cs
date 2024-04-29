using CQRS.Core.Models;

namespace CQRS.Core.Interfaces
{
    public interface IGetOrderByIdQueryHandler
    {
        GetOrderByIdResponseModel GetOrderById(GetOrderByIdRequestModel requestModel);
    }
}
