using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YY.MicroService.Interface;
using YY.MicroService.Model;

namespace YY.MicroService.OrderServiceInstance.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrderService orderService;
        public OrderController(IOrderService _orderService)
        {
            orderService = _orderService;
        }
        [HttpGet]
        public async Task<object> AddOrder()
        {
            Order order = new Order
            {
                OrderId = 1,
                ProductId = 10,
                UserId = 1,
                Amount = 10
            };
            var result = await orderService.AddOrder(order);
            return new 
            {
                Result = result ? true : false,
                Message = result ? "新增成功" : "新增失败",
                StatusCode = result ? 30000 : -9999
            };
        }
        [HttpGet]
        public object AddAOPOrder()
        {
            Order order = new Order
            {
                OrderId = 1,
                ProductId = 10,
                UserId = 1,
                Amount = 10
            };
            var result = orderService.AddOrderForAOP(order);
            return new
            {
                Result = result ? true : false,
                Message = result ? "新增成功" : "新增失败",
                StatusCode = result ? 30000 : -9999
            };

        }
    }
}
