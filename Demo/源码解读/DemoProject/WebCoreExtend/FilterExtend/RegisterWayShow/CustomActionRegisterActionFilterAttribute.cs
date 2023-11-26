using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreExtend.FilterExtend.RegisterWayShow
{
    /// <summary>
    /// 注册在Action上面
    /// </summary>
    public class CustomActionRegisterActionFilterAttribute : ActionFilterAttribute
    {
        public string Remark = null;

        /// <summary>
        /// Action执行后
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine($"This {nameof(CustomActionRegisterActionFilterAttribute)} OnActionExecuted {this.Order} {this.Remark}");
        }
        /// <summary>
        /// Action执行前
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine($"This {nameof(CustomActionRegisterActionFilterAttribute)} OnActionExecuting {this.Order} {this.Remark}");
        }

        ///// <summary>
        ///// Result执行前
        ///// </summary>
        ///// <param name="context"></param>
        //public override void OnResultExecuting(ResultExecutingContext context)
        //{
        //    Console.WriteLine($"This {nameof(CustomActionRegisterActionFilterAttribute)} OnResultExecuting{this.Order} {this.Remark}");
        //}

        ///// <summary>
        ///// Result执行后
        ///// </summary>
        ///// <param name="context"></param>
        //public override void OnResultExecuted(ResultExecutedContext context)
        //{
        //    Console.WriteLine($"This {nameof(CustomActionRegisterActionFilterAttribute)} OnResultExecuted{this.Order} {this.Remark}");
        //    Console.WriteLine("");
        //}
    }
}
