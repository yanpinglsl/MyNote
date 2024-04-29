using DotNetCore.CAP;
using System.Text.Json;

namespace DotNETCAP.Core.Subscriber
{
    public class OrderConsumer : ICapSubscribe
    {
        [CapSubscribe("order.created")]
        public object OrderCreated(JsonElement param)
        {
            var OrderName = param.GetProperty("OrderName").GetString();
            Console.WriteLine($"Order:{OrderName}");
            return new { OrderName = OrderName, IsSuccess = true };
        }
    }
}
