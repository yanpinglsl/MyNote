using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YY.Zhihu.SharedLibraries.Result;
using IResult = YY.Zhihu.SharedLibraries.Result.IResult;

namespace YY.Zhihu.Application.Infrastructure
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        public ISender Sender => HttpContext.RequestServices.GetRequiredService<ISender>();

        [NonAction]
        public IActionResult ReturnResult(IResult result)
        {
            switch (result.Status)
            {
                case ResultStatus.Ok:
                    {
                        var value = result.GetValue();
                        return value is null ? NoContent() : Ok(value);
                    }
                case ResultStatus.Error:
                    return result.Errors is null ? BadRequest() : BadRequest(new { errors = result.Errors });

                case ResultStatus.NotFound:
                    return result.Errors is null ? NotFound() : NotFound(new { errors = result.Errors });

                case ResultStatus.Invalid:
                    return result.Errors is null ? BadRequest() : BadRequest(new { errors = result.Errors });

                case ResultStatus.Forbidden:
                    return StatusCode(403);

                case ResultStatus.Unauthorized:
                    return Unauthorized();

                default:
                    return BadRequest();
            }
        }
    }
}
