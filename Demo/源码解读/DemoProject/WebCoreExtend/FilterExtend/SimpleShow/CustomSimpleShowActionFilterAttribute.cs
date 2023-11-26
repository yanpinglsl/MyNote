using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.FilterExtend.SimpleShow
{
    //public class CustomSimpleShowActionFilterAttribute2 : ActionFilterAttribute
    //{ }

    /// <summary>
    /// 展示下Action的Filter
    /// </summary>
    public class CustomSimpleShowActionFilterAttribute : Attribute, IActionFilter, IFilterMetadata, IOrderedFilter
    {
        public int Order { get; set; }

        /// <summary>
        /// Action执行前
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine($"This {nameof(CustomSimpleShowActionFilterAttribute)} OnActionExecuting{this.Order}");
            //获取参数--校验参数--修改参数---

            #region 可修改参数
            //Console.WriteLine($"This {nameof(CustomShowActionFilterAttribute)} OnActionExecuting 修改参数值之前 参数为{JsonConvert.SerializeObject(context.ActionArguments)}");
            //if (!context.ActionArguments.ContainsKey("Id") || context.ActionArguments["Id"] == null)
            //{
            //    context.ActionArguments["Id"] = 3;//操作参数--
            //}
            //Console.WriteLine($"This {nameof(CustomShowActionFilterAttribute)} OnActionExecuting 修改参数值之后 参数为{JsonConvert.SerializeObject(context.ActionArguments)}");
            #endregion
        }

        /// <summary>
        /// Action执行后---可修改结果
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine($"This {nameof(CustomSimpleShowActionFilterAttribute)} OnActionExecuted{this.Order}");

            //获取结果---修改结果
            #region 可修改结果
            //Console.WriteLine($"访问{nameof(CustomShowActionFilterAttribute)}-OnActionExecuted设置Result");
            //context.Result = new JsonResult(new
            //{
            //    Result = true,
            //    Message = $"访问{nameof(CustomShowActionFilterAttribute)}-OnActionExecuted设置Result"
            //});
            #endregion
        }


    }
}
