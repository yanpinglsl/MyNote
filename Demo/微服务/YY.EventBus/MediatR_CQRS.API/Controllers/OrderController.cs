using MediatR;
using MediatR_CQRS.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediatR_CQRS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public IActionResult MakeOrder([FromBody] MakeOrderRequestModel requestModel)
        {
            var response = mediator.Send(requestModel).Result;
            return Ok(response);
        }

        [HttpGet]
        public IActionResult OrderDetails()
        {
            Guid guid = Guid.NewGuid();
            var response = mediator.Send(new GetOrderByIdRequestModel {OrderId = guid }).Result;
            return Ok(response);
        }
    }
}