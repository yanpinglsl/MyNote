using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreExtend.FilterExtend.SimpleExtend
{
   
    /// <summary>
    /// 1  不能同时支持双接口，会失效 IAlwaysRunResultFilter  IAsyncAlwaysRunResultFilter3
    /// 2  测试IAlwaysRunResultFilter问题，即使跳转也一定会执行
    /// </summary>
    public class CustomShowAlwaysRunResultFilterAttribute : Attribute, IAlwaysRunResultFilter, IOrderedFilter
    {
        public int Order { get; set; }
        public string Remark = "";

        public void OnResultExecuting(ResultExecutingContext context)
        {
            //context.HttpContext.Items["AlwasRunResultNow"] = DateTime.Now;
            Console.WriteLine($"This {nameof(CustomShowAlwaysRunResultFilterAttribute)} OnResultExecuting{this.Order} {this.Remark}");
        }
        public void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine($"This {nameof(CustomShowAlwaysRunResultFilterAttribute)} OnResultExecuted{this.Order} {this.Remark}");
        }
    }

    public class CustomAsyncShowAlwaysRunResultFilterAttribute : Attribute, IAsyncAlwaysRunResultFilter, IOrderedFilter
    {
        public int Order { get; set; }
        public string Remark = "";

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            await Task.CompletedTask;
            Console.WriteLine($"This {nameof(CustomAsyncShowAlwaysRunResultFilterAttribute)} OnResultExecutionAsync Begin {this.Order} {this.Remark}");
            await next.Invoke();
            Console.WriteLine($"This {nameof(CustomAsyncShowAlwaysRunResultFilterAttribute)} OnResultExecutionAsync End   {this.Order} {this.Remark}");
        }
    }
}
