using CQRS.Core.Interfaces;
using CQRS.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CQRS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IGetOrderByIdQueryHandler getOrderByIdQueryHandler,
               IMakeOrderCommandHandler makeOrderCommandHandler)
           : ControllerBase
    {
        [HttpPost]
        public IActionResult MakeOrder(MakeOrderRequestModel requestModel)
        {
            var response = makeOrderCommandHandler.MakeOrder(requestModel);
            return Ok(response);
        }

        [HttpGet]
        public IActionResult OrderDetails()
        {
            Guid id = Guid.NewGuid();
            var response = getOrderByIdQueryHandler.GetOrderById(new GetOrderByIdRequestModel { OrderId = id });
            return Ok(response);
        }
    }
}
