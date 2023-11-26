using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCoreExtend.CommonModels;

namespace Zhaoxi.AgileFramework.WebCore.FilterExtend
{
    public class LogActionFilterAttribute : ActionFilterAttribute
    {
        private ILogger<LogActionFilterAttribute> _logger = null;
        public LogActionFilterAttribute(ILogger<LogActionFilterAttribute> logger)
        {
            this._logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string url = context.HttpContext.Request.Path.Value;
            string argument = JsonConvert.SerializeObject(context.ActionArguments);

            string controllerName = context.Controller.GetType().FullName;
            string actionName = context.ActionDescriptor.DisplayName;

            LogModel logModel = new LogModel()
            {
                OriginalClassName = controllerName,
                OriginalMethodName = actionName,
                Remark = $"来源于{nameof(LogActionFilterAttribute)}.{nameof(OnActionExecuting)}"
            };

            //this._logger.LogInformation($"url={url}---argument={argument}",new object[] { JsonConvert.SerializeObject(logModel) } );
            this._logger.LogInformation($"url={url}---argument={argument}");
        }
    }
}
