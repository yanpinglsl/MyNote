using DotNetCore.CAP;
using System.Text.Json;

namespace DotNETCAP.Core.Subscriber
{
    public class OrderPublisher : ICapSubscribe
    {
        [CapSubscribe("order.created.status")]
        public void OrderCreatedStatus(JsonElement param)
        {
            var orderName = param.GetProperty("OrderName").GetString();
            var isSuccess = param.GetProperty("IsSuccess").GetBoolean();

            Console.WriteLine($"{orderName}:{isSuccess}");
        }
    }
}
