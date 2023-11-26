using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Zhaoxi.AgileFramework.WebCore.FilterExtend;

namespace WebCoreExtend.FilterExtend.SimpleShow
{
    public class CustomSimpleShowAsyncActionFilterAttribute : Attribute, IAsyncActionFilter, IFilterMetadata, IOrderedFilter
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

            #region 可修改参数
            //Console.WriteLine($"This {nameof(CustomShowActionFilterAttribute)} OnActionExecuting 修改参数值之前 参数为{JsonConvert.SerializeObject(context.ActionArguments)}");
            //if (!context.ActionArguments.ContainsKey("Id") || context.ActionArguments["Id"] == null)
            //{
            //    context.ActionArguments["Id"] = 3;//操作参数--
            //}
            //Console.WriteLine($"This {nameof(CustomShowActionFilterAttribute)} OnActionExecuting 修改参数值之后 参数为{JsonConvert.SerializeObject(context.ActionArguments)}");
            #endregion

            await next.Invoke();//方法的具体执行

            Console.WriteLine($"This {nameof(CustomAsyncShowActionFilterAttribute)} OnActionExecutionAsync {this.Order} --End");

            #region 可修改结果

            #region 错误
            //Console.WriteLine($"访问{nameof(CustomShowActionFilterAttribute)}-OnActionExecutionAsync Part2设置Result");
            //context.Result = new JsonResult(new
            //{
            //    Result = true,
            //    Message = $"访问{nameof(CustomShowActionFilterAttribute)}-OnActionExecutionAsync Part2设置Result"
            //});//这个是错的，毫无效果，是ActionExecutingContext的结果，已经没有意义
            #endregion

            #region 正确，应该是设置executedResult
            //var executedResult = await next.Invoke();//得关闭上面的 next.Invoke();
            //Console.WriteLine($"This {nameof(CustomAsyncShowActionFilterAttribute)} OnActionExecutionAsync {this.Order} --End");

            //Console.WriteLine($"访问{nameof(CustomShowActionFilterAttribute)}-OnActionExecutionAsync Part2设置Result");
            //executedResult.Result = new JsonResult(new
            //{
            //    Result = true,
            //    Message = $"访问{nameof(CustomShowActionFilterAttribute)}-OnActionExecutionAsync Part2设置Result"
            //});
            #endregion
            #endregion
        }
    }
}
