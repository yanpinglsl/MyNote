using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.FilterExtend.RegisterWayShow
{
    /// <summary>
    /// 注册控制器
    /// </summary>
    public class CustomControllerRegisterActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine($"This {nameof(CustomControllerRegisterActionFilterAttribute)} OnActionExecuted {this.Order}---{this.GetHashCode()}");
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine($"This {nameof(CustomControllerRegisterActionFilterAttribute)} OnActionExecuting{this.Order}---{this.GetHashCode()}");
        }
        //public override void OnResultExecuting(ResultExecutingContext context)
        //{
        //    Console.WriteLine($"This {nameof(CustomControllerRegisterActionFilterAttribute)} OnResultExecuting{this.Order}");
        //}
        //public override void OnResultExecuted(ResultExecutedContext context)
        //{
        //    Console.WriteLine($"This {nameof(CustomControllerRegisterActionFilterAttribute)} OnResultExecuted{this.Order}");
        //}
    }
}
