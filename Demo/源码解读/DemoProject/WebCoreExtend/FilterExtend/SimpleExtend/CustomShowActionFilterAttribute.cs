using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zhaoxi.AgileFramework.WebCore.FilterExtend
{
    /// <summary>
    /// 展示下Action的Filter
    /// </summary>
    public class CustomShowActionFilterAttribute : Attribute, IActionFilter, IFilterMetadata, IOrderedFilter
    {
        public int Order { get; set; }
        /// <summary>
        /// Action执行后
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine($"This {nameof(CustomShowActionFilterAttribute)} OnActionExecuted{this.Order}");
            //throw new Exception($"This is Eleven's {nameof(CustomShowActionFilterAttribute)} OnActionExecuted  Exception");

            //Console.WriteLine($"访问{nameof(CustomShowActionFilterAttribute)}-OnActionExecuted设置Result");
            //context.Result = new JsonResult(new
            //{
            //    Result = true,
            //    Message = $"访问{nameof(CustomShowActionFilterAttribute)}-OnActionExecuted设置Result"
            //});
        }
        /// <summary>
        /// Action执行前
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            #region 修改参数
            Console.WriteLine($"This {nameof(CustomShowActionFilterAttribute)} OnActionExecuting 修改参数值之前 参数为{JsonConvert.SerializeObject(context.ActionArguments)}");
            if (!context.ActionArguments.ContainsKey("Id") || context.ActionArguments["Id"] == null)
            {
                context.ActionArguments["Id"] = 3;//操作参数--
            }
            Console.WriteLine($"This {nameof(CustomShowActionFilterAttribute)} OnActionExecuting 修改参数值之后 参数为{JsonConvert.SerializeObject(context.ActionArguments)}");
            #endregion

            Console.WriteLine($"This {nameof(CustomShowActionFilterAttribute)} OnActionExecuting{this.Order}");

            //throw new Exception($"This is Eleven's {nameof(CustomShowActionFilterAttribute)} OnActionExecuting  Exception");

            //Console.WriteLine($"访问{nameof(CustomShowActionFilterAttribute)}-OnActionExecuting设置Result");
            //context.Result = new JsonResult(new
            //{
            //    Result = true,
            //    Message = $"访问{nameof(CustomShowActionFilterAttribute)}-OnActionExecuting设置Result"
            //});
        }
    }

    /// <summary>
    /// 异步版本ActionFilter
    /// </summary>
    public class CustomAsyncShowActionFilterAttribute : Attribute, IAsyncActionFilter, IFilterMetadata, IOrderedFilter
    {
        public int Order { get; set; }

        /// <summary>
        /// 包裹了Aciton执行
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Console.WriteLine($"This {nameof(CustomAsyncShowActionFilterAttribute)} OnActionExecutionAsync {this.Order} --Begin");
            ////throw new Exception("This is Eleven's OnActionExecutionAsync  Exception");

            //Console.WriteLine($"访问{nameof(CustomShowActionFilterAttribute)}-OnActionExecutionAsync Part1设置Result");
            //context.Result = new JsonResult(new
            //{
            //    Result = true,
            //    Message = $"访问{nameof(CustomShowActionFilterAttribute)}-OnActionExecutionAsync Part1设置Result"
            //});
            ////后续动作就不会再执行了

            #region 设置Context
            //await next.Invoke();
            //Console.WriteLine($"This {nameof(CustomAsyncShowActionFilterAttribute)} OnActionExecutionAsync {this.Order} --End");

            //Console.WriteLine($"访问{nameof(CustomShowActionFilterAttribute)}-OnActionExecutionAsync Part2设置Result");
            //context.Result = new JsonResult(new
            //{
            //    Result = true,
            //    Message = $"访问{nameof(CustomShowActionFilterAttribute)}-OnActionExecutionAsync Part2设置Result"
            //});//这个是错的，毫无效果，是ActionExecutingContext的结果，已经没有意义
            #endregion

            #region 设置executedResult
            var executedResult = await next.Invoke();
            Console.WriteLine($"This {nameof(CustomAsyncShowActionFilterAttribute)} OnActionExecutionAsync {this.Order} --End");

            Console.WriteLine($"访问{nameof(CustomShowActionFilterAttribute)}-OnActionExecutionAsync Part2设置Result");
            executedResult.Result = new JsonResult(new
            {
                Result = true,
                Message = $"访问{nameof(CustomShowActionFilterAttribute)}-OnActionExecutionAsync Part2设置Result"
            });
            #endregion
        }
    }
}
