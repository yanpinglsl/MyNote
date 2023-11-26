using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCoreExtend.CommonModels;
using WebCoreExtend.WebCore;

namespace WebCoreExtend.FilterExtend.SimpleExtend
{
    
    /// <summary>
    /// 异常处理Filter  需要IOC注入
    /// </summary>
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        #region Identity
        private readonly ILogger<CustomExceptionFilterAttribute> _logger;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger
            , IModelMetadataProvider modelMetadataProvider)
        {
            this._modelMetadataProvider = modelMetadataProvider;
            this._logger = logger;
            Console.WriteLine($"*************This {nameof(CustomExceptionFilterAttribute)} Init***************");
        }
        #endregion

        /// <summary>
        /// 异常发生，但是没有处理时
        /// 异常之后得写日志
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            Console.WriteLine($"*************This {nameof(CustomExceptionFilterAttribute)} OnException {this.Order}");
            if (!context.ExceptionHandled)
            {
                #region 日志
                string url = context.HttpContext.Request.Path.Value;
                string actionName = context.ActionDescriptor.DisplayName;
                var logModel = new LogModel()
                {
                    OriginalClassName = "",
                    OriginalMethodName = actionName,
                    Remark = $"来源于{nameof(CustomExceptionFilterAttribute)}.{nameof(OnException)}"
                };
                this._logger.LogError(context.Exception, $"OnException：{url}----->actionName={actionName}  Message={context.Exception.Message}", JsonConvert.SerializeObject(logModel));
                #endregion

                if (context.HttpContext.Request.IsAjaxRequest())//header看看是不是XMLHttpRequest
                {
                    context.Result = new JsonResult(new AjaxResult()
                    {
                        Result = false,
                        Message = context.Exception.Message
                    });//中断式---请求到这里结束了，不再继续Action
                }
                else
                {
                    var result = new ViewResult { ViewName = "~/Views/Shared/Error.cshtml" };
                    result.ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);
                    result.ViewData.Add("Exception", context.Exception);
                    context.Result = result;
                }

                context.ExceptionHandled = true;
            }
        }
    }

    public class CustomSyncExceptionFilterAttribute : Attribute, IExceptionFilter, IFilterMetadata, IOrderedFilter
    {
        public int Order => 5;//先执行

        /// <summary>
        /// 只日志 不处理
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            Console.WriteLine($"This {nameof(CustomSyncExceptionFilterAttribute)} OnException {this.Order}");
            throw new Exception($"This is Eleven's {nameof(CustomSyncExceptionFilterAttribute)} OnException抛出  Exception");


            if (!context.ExceptionHandled)
            {
                Console.WriteLine($"This {nameof(CustomSyncExceptionFilterAttribute)} OnException {this.Order}  -Inside");
                //context.ExceptionHandled = true;//不指定
            }
        }
    }

    public class CustomAsyncExceptionFilterAttribute : Attribute, IAsyncExceptionFilter, IFilterMetadata, IOrderedFilter
    {
        public int Order => 10;//先执行

        /// <summary>
        /// 只日志 不处理
        /// </summary>
        /// <param name="context"></param>
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            await Task.CompletedTask;
            Console.WriteLine($"This {nameof(CustomAsyncExceptionFilterAttribute)} OnExceptionAsync {this.Order}");
            if (!context.ExceptionHandled)
            {
                Console.WriteLine($"This {nameof(CustomAsyncExceptionFilterAttribute)} OnExceptionAsync {this.Order}  -Inside");

                //context.ExceptionHandled = true;//不指定
            }
        }
    }
}
