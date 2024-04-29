using MediatR;
using Microsoft.AspNetCore.Mvc;
using RequestResponse.API.Events;

namespace Zhaoxi.MediatorSample.Notifaction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> MakeOrder()
        {
            Console.WriteLine("用户已下单");
            var makeOrderEvent = new MakeOrderEvent("张三", "三体", 1);
            try
            {
                await mediator.Publish(makeOrderEvent);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
            return Ok();
        }

    }
}