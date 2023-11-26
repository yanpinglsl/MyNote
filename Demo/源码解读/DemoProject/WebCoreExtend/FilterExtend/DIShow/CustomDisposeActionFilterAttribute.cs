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
    public class CustomDisposeActionFilterAttribute : ActionFilterAttribute, IDisposable
    {
        public string Remark = null;
        private ILogger<CustomDisposeActionFilterAttribute> _iLogger = null;
        private static CustomDisposeActionFilterAttribute _Instance = null;
        private static int _iCounter = 0;
        public CustomDisposeActionFilterAttribute(ILogger<CustomDisposeActionFilterAttribute> logger)
        {
            this._iLogger = logger;
        }

        /// <summary>
        /// Action执行后
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _iCounter++;
            Console.WriteLine($"This {nameof(CustomDisposeActionFilterAttribute)} OnActionExecuted {this.Order}");

            if (_Instance == null)
            {
                _Instance = this;
            }
            else
            {
                Console.WriteLine($"This {nameof(CustomDisposeActionFilterAttribute)} 是同一个实例吗？{object.ReferenceEquals(_Instance, this)} {this.GetHashCode()}");
            }

            this._iLogger.LogInformation($"This {nameof(CustomDisposeActionFilterAttribute)} OnActionExecuted {this.Order} {this.Remark} {this.GetHashCode()}");
        }

        public void Dispose()
        {
            Console.WriteLine($"This {nameof(CustomDisposeActionFilterAttribute)} Dispose {_iCounter}  重置为0");
            _iCounter = 0;
        }
    }
}
