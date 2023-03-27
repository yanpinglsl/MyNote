using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WebCoreExtend.ConventionExtend
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class CustomControllerModelConventionAttribute : Attribute, IControllerModelConvention
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
                Console.WriteLine("This is CustomControllerModelConventionAttribute Apply");
                controller.Filters.Add(new CustomInnerActionFilterAttribute() { Remark = "CustomControllerModelConventionAttribute----2" });
                //等同于在控制器上面标记Filter---
            }
        }
    }
}
