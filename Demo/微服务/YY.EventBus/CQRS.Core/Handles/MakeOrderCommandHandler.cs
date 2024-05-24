using CQRS.Core.Interfaces;
using CQRS.Core.Models;

namespace CQRS.Core.Handles
{
    public class MakeOrderCommandHandler : IMakeOrderCommandHandler
    {
        public MakeOrderResponseModel MakeOrder(MakeOrderRequestModel requestModel)
        {
            var result = new MakeOrderResponseModel
            {
                IsSuccess = true,
                OrderId = new Guid("53d26807-ad70-4449-8479-024c54eb2020")
            };

            // 业务逻辑

            return result;
        }
    }
}
