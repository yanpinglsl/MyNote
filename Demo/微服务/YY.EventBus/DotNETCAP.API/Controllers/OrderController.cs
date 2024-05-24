using DotNETCAP.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DotNETCAP.API.Controllers
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
        public IActionResult OrderDetails(int id)
        {
            Guid guid = Guid.NewGuid();
            var response = mediator.Send(new GetOrderByIdRequestModel {OrderId = guid }).Result;
            return Ok(response);
        }
    }
}