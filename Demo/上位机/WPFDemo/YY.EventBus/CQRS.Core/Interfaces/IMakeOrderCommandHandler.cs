using CQRS.Core.Models;

namespace CQRS.Core.Interfaces
{
    public interface IMakeOrderCommandHandler
    {
        MakeOrderResponseModel MakeOrder(MakeOrderRequestModel requestModel);
    }
}
