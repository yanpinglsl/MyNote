using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCoreExtend.CommonModels;

namespace Zhaoxi.AgileFramework.WebCore.FilterExtend
{
    /// <summary>
    /// 展示下Result的Filter
    /// </summary>
    public class CustomShowResultFilterAttribute : Attribute, IResultFilter, IFilterMetadata, IOrderedFilter
    {
        public int Order { get; set; }
        public string Remark = "";

        public void OnResultExecuting(ResultExecutingContext context)
        {
            Console.WriteLine($"This {nameof(CustomShowResultFilterAttribute)} OnResultExecuting {this.Order} {this.Remark}");

            context.Cancel = true;

            //throw new Exception($"This is Eleven's {nameof(CustomShowResultFilterAttribute)} OnResultExecuting  Exception");
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine($"This {nameof(CustomShowResultFilterAttribute)} OnResultExecuted {this.Order} {this.Remark}");

            //context.Canceled = true;

            //throw new Exception($"This is Eleven's {nameof(CustomShowResultFilterAttribute)} OnResultExecuted  Exception");

            #region 异常处理
            if (context.Exception != null || context.ExceptionDispatchInfo != null)
            {
                if (!context.ExceptionHandled)
                {
                    #region 日志
                    string url = context.HttpContext.Request.Path.Value;
                    string actionName = context.ActionDescriptor.DisplayName;
                    var logModel = new LogModel()
                    {
                        OriginalClassName = "",
                        OriginalMethodName = actionName,
                        Remark = $"来源于{nameof(CustomShowResultFilterAttribute)}.{nameof(OnResultExecuted)}"
                    };
                    Console.WriteLine( $"{nameof(CustomShowResultFilterAttribute)} OnResultExecuted处理异常：{url}----->actionName={actionName}  Message={context.Exception.Message}", JsonConvert.SerializeObject(logModel));
                    #endregion
                    //context.Result是只读的

                    //context.ExceptionHandled = true;//让异常被吞掉，返回就是个空的200
                }
            }
            #endregion


        }
    }


    /// <summary>
    /// 异步版本ResultFilter
    /// </summary>
    public class CustomAsyncShowResultFilterAttribute : Attribute, IAsyncResultFilter, IFilterMetadata, IOrderedFilter
    {
        public int Order { get; set; }
        public string Remark = "";

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            Console.WriteLine($"This {nameof(CustomAsyncShowResultFilterAttribute)} OnResultExecutionAsync {this.Order} {this.Remark} --Begin");

            await next.Invoke();

            Console.WriteLine($"This {nameof(CustomAsyncShowResultFilterAttribute)} OnResultExecutionAsync {this.Order} {this.Remark} --End");
        }
    }
}
