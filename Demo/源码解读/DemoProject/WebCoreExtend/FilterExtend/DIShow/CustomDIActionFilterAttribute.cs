using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCoreExtend.FilterExtend.RegisterWayShow;

namespace WebCoreExtend.FilterExtend.DIShow
{
    /// <summary>
    /// 注入是不能标记的
    /// </summary>
    public class CustomAttribute : Attribute
    {
        public const string Name = "Eleven";//常量

        public CustomAttribute(ILogger<CustomAttribute> log)
        {

        }

        public CustomAttribute(string a)
        {
            //DefaultHttpContext.Current
            //httpContextAccessor.HttpContext//线程
        }
    }


    public class CustomDIActionFilterAttribute : ActionFilterAttribute
    {
        public string Remark = null;
        private ILogger<CustomDIActionFilterAttribute> _iLogger = null;

        public CustomDIActionFilterAttribute(ILogger<CustomDIActionFilterAttribute> logger)
        {
            this._iLogger = logger;
        }

        /// <summary>
        /// Action执行后
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            this._iLogger.LogInformation($"This {nameof(CustomDIActionFilterAttribute)} OnActionExecuted {this.Order} {this.Remark} {this.GetHashCode()}");
        }
    }
}
