using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.FilterExtend.SimpleShow
{
    /// <summary>
    /// 展示下Action的Filter
    /// </summary>
    public class CustomSimpleShowDoubleActionFilterAttribute : Attribute, IActionFilter, IAsyncActionFilter, IFilterMetadata, IOrderedFilter
    {
        public int Order { get; set; }

        /// <summary>
        /// Action执行前
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine($"This {nameof(CustomSimpleShowDoubleActionFilterAttribute)} OnActionExecuting{this.Order}");

            #region 可修改参数
            //Console.WriteLine($"This {nameof(CustomSimpleShowDoubleActionFilterAttribute)} OnActionExecuting 修改参数值之前 参数为{JsonConvert.SerializeObject(context.ActionArguments)}");
            //if (!context.ActionArguments.ContainsKey("Id") || context.ActionArguments["Id"] == null)
            //{
            //    context.ActionArguments["Id"] = 3;//操作参数--
            //}
            //Console.WriteLine($"This {nameof(CustomSimpleShowDoubleActionFilterAttribute)} OnActionExecuting 修改参数值之后 参数为{JsonConvert.SerializeObject(context.ActionArguments)}");
            #endregion
        }


        /// <summary>
        /// Action执行后---可修改结果
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine($"This {nameof(CustomSimpleShowDoubleActionFilterAttribute)} OnActionExecuted{this.Order}");

            #region 可修改结果
            //Console.WriteLine($"访问{nameof(CustomSimpleShowDoubleActionFilterAttribute)}-OnActionExecuted设置Result");
            //context.Result = new JsonResult(new
            //{
            //    Result = true,
            //    Message = $"访问{nameof(CustomSimpleShowDoubleActionFilterAttribute)}-OnActionExecuted设置Result"
            //});
            #endregion
        }

        /// <summary>
        /// 包裹了Aciton执行
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Console.WriteLine($"This {nameof(CustomSimpleShowDoubleActionFilterAttribute)} OnActionExecutionAsync {this.Order} --Begin");

            #region 可修改参数
            //Console.WriteLine($"This {nameof(CustomSimpleShowDoubleActionFilterAttribute)} OnActionExecuting 修改参数值之前 参数为{JsonConvert.SerializeObject(context.ActionArguments)}");
            //if (!context.ActionArguments.ContainsKey("Id") || context.ActionArguments["Id"] == null)
            //{
            //    context.ActionArguments["Id"] = 3;//操作参数--
            //}
            //Console.WriteLine($"This {nameof(CustomSimpleShowDoubleActionFilterAttribute)} OnActionExecuting 修改参数值之后 参数为{JsonConvert.SerializeObject(context.ActionArguments)}");
            #endregion

            await next.Invoke();
            Console.WriteLine($"This {nameof(CustomSimpleShowDoubleActionFilterAttribute)} OnActionExecutionAsync {this.Order} --End");

            #region 可修改结果

            #region 错误
            //Console.WriteLine($"访问{nameof(CustomSimpleShowDoubleActionFilterAttribute)}-OnActionExecutionAsync Part2设置Result");
            //context.Result = new JsonResult(new
            //{
            //    Result = true,
            //    Message = $"访问{nameof(CustomSimpleShowDoubleActionFilterAttribute)}-OnActionExecutionAsync Part2设置Result"
            //});//这个是错的，毫无效果，是ActionExecutingContext的结果，已经没有意义
            #endregion

            #region 正确，应该是设置executedResult
            //var executedResult = await next.Invoke();//得关闭上面的 next.Invoke();
            //Console.WriteLine($"This {nameof(CustomSimpleShowDoubleActionFilterAttribute)} OnActionExecutionAsync {this.Order} --End");

            //Console.WriteLine($"访问{nameof(CustomSimpleShowDoubleActionFilterAttribute)}-OnActionExecutionAsync Part2设置Result");
            //executedResult.Result = new JsonResult(new
            //{
            //    Result = true,
            //    Message = $"访问{nameof(CustomSimpleShowDoubleActionFilterAttribute)}-OnActionExecutionAsync Part2设置Result"
            //});
            #endregion

            #endregion
        }
    }
}
