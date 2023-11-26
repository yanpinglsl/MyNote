using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.FilterExtend.RegisterWayShow
{
    /// <summary>
    /// 注册全局
    /// </summary>
    public class CustomGlobalRegisterActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine($"This {nameof(CustomGlobalRegisterActionFilterAttribute)} OnActionExecuted{this.Order}---{this.GetHashCode()}");
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine($"This {nameof(CustomGlobalRegisterActionFilterAttribute)} OnActionExecuting{this.Order}---{this.GetHashCode()}");
        }
        //public override void OnResultExecuting(ResultExecutingContext context)
        //{
        //    Console.WriteLine($"This {nameof(CustomGlobalRegisterActionFilterAttribute)} OnResultExecuting{this.Order}");
        //}
        //public override void OnResultExecuted(ResultExecutedContext context)
        //{
        //    Console.WriteLine($"This {nameof(CustomGlobalRegisterActionFilterAttribute)} OnResultExecuted{this.Order}");
        //}
    }
}
