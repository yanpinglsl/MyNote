using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreExtend.FilterExtend.SimpleExtend
{
    /// <summary>
    /// 注册在Action上面
    /// </summary>
    public class CustomActionOrderFilterAttribute : ActionFilterAttribute
    {
        public string Remark = null;

        private static CustomActionOrderFilterAttribute _Instance = null;
        /// <summary>
        /// Action执行后
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine($"This {nameof(CustomActionOrderFilterAttribute)} OnActionExecuted {this.Order}");
        }
        /// <summary>
        /// Action执行前
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_Instance == null)
            {
                _Instance = this;//保存第一次的实例
            }
            Console.WriteLine($"This {nameof(CustomActionOrderFilterAttribute)} OnActionExecuting{this.Order}---{_Instance == this} ----{Remark}");//第一次就是true--第二次true就表明缓存 否则就没缓存
        }

        /// <summary>
        /// Result执行前
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            Console.WriteLine($"This {nameof(CustomActionOrderFilterAttribute)} OnResultExecuting{this.Order}");
        }

        /// <summary>
        /// Result执行后
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine($"This {nameof(CustomActionOrderFilterAttribute)} OnResultExecuted{this.Order}");
            Console.WriteLine("");
        }
    }

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
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            Console.WriteLine($"This {nameof(CustomControllerRegisterActionFilterAttribute)} OnResultExecuting{this.Order}");
        }
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine($"This {nameof(CustomControllerRegisterActionFilterAttribute)} OnResultExecuted{this.Order}");
        }
    }

    /// <summary>
    /// 注册全局
    /// </summary>
    public class CustomGlobal1RegisterActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine($"This {nameof(CustomGlobal1RegisterActionFilterAttribute)} OnActionExecuted{this.Order}---{this.GetHashCode()}");
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine($"This {nameof(CustomGlobal1RegisterActionFilterAttribute)} OnActionExecuting{this.Order}---{this.GetHashCode()}");
        }
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            Console.WriteLine($"This {nameof(CustomGlobal1RegisterActionFilterAttribute)} OnResultExecuting{this.Order}");
        }
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine($"This {nameof(CustomGlobal1RegisterActionFilterAttribute)} OnResultExecuted{this.Order}");
        }
    }
}
