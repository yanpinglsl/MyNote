using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;
using SerilogDemo.Localization.ErrorCode;

namespace SerilogDemo.Controllers
{
    [Route("api/Sample01")]
    public class Sample01Controller : AbpController
    {
        public ObjectResult Get()
        {
            throw new BusinessException(SampleErrorCodes.ThisIsABusinessException)
                .WithData("msg","这是一个参数化的错误消息");
        }
    }
}
