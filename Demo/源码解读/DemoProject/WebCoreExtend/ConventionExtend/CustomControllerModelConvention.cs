using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WebCoreExtend.ConventionExtend
{
    public class CustomControllerModelConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            #region show data
            //Console.WriteLine($"***************{controller.ControllerName}");
            //Console.WriteLine($"***************{controller.DisplayName}");
            //foreach (var item in controller.Actions)
            //{
            //    Console.WriteLine($"***************{item.ActionName}");
            //    Console.WriteLine($"***************{item.DisplayName}");
            //    Console.WriteLine($"***************{string.Join(",", item.Attributes.Select(a => a.ToString()))}");
            //    Console.WriteLine($"***************{string.Join(",", item.Parameters.Select(a => a.ParameterName))}");
            //}
            #endregion

            if (controller.ControllerName.Equals("Home"))
            {
                Console.WriteLine("This is CustomControllerModelConvention Apply");
                controller.Filters.Add(new CustomInnerActionFilterAttribute() { Remark= "CustomControllerModelConvention----1" });
                //等同于在控制器上面标记Filter---
            }
        }
    }

    internal class CustomInnerActionFilterAttribute : ActionFilterAttribute
    {
        public string Remark = null;
        /// <summary>
        /// Action执行后
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine($"This {nameof(CustomInnerActionFilterAttribute)} OnActionExecuted {this.Order}---{Remark}");
        }
        /// <summary>
        /// Action执行前
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine($"This {nameof(CustomInnerActionFilterAttribute)} OnActionExecuting{this.Order}---{Remark}");
        }

        /// <summary>
        /// Result执行前
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            Console.WriteLine($"This {nameof(CustomInnerActionFilterAttribute)} OnResultExecuting{this.Order}---{Remark}");
        }

        /// <summary>
        /// Result执行后
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine($"This {nameof(CustomInnerActionFilterAttribute)} OnResultExecuted{this.Order}---{Remark}");
            Console.WriteLine("");
        }
    }
}
