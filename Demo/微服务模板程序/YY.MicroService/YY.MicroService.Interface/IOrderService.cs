
using System.Threading.Tasks;
using YY.MicroService.Model;

namespace YY.MicroService.Interface
{
    public interface IOrderService
    {
        Task<bool> AddOrder(Order order);

        bool AddOrderForAOP(Order order);
    }
}
