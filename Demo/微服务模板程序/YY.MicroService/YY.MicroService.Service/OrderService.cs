
using System;
using System.Threading.Tasks;
using YY.MicroService.Interface;
using YY.MicroService.Model;

namespace YY.MicroService.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUserService UserService;

        public OrderService(IUserService UserService)
        {
            this.UserService = UserService;
        }

        public async Task<bool> AddOrder(Order order)
        {
            Console.WriteLine("================================================================");
            // 根据订单的用户ID获取用户信息
            Console.WriteLine("=============>新增订单信息");
            Console.WriteLine("=============>远程调用用户微服务");
            // 非AOP调用
            var User =await UserService.GetById(order.UserId);
            Console.WriteLine($"用户信息: name={User.Name},id={User.Id}");
            if (User == null)
            {
                return false;
            }

            return true;
        }

        public bool AddOrderForAOP(Order order)
        {
            Console.WriteLine("====================================================");
            // 根据订单的用户ID获取用户信息
            Console.WriteLine("AOP=============>新增订单信息");
            // AOP调用
            var User = UserService.AOPGetById(order.UserId);
            Console.WriteLine($"用户信息: name={User.Name},id={User.Id}");
            if (User == null)
            {
                return false;
            }

            return true;
        }
    }
}
