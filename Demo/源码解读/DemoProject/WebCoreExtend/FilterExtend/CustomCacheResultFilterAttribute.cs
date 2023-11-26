using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zhaoxi.AgileFramework.WebCore.FilterExtend
{
    /// <summary>
    /// 客户端缓存
    /// </summary>
    public class CustomCacheResultFilterAttribute : Attribute, IResultFilter, IFilterMetadata, IOrderedFilter
    {
        public int Duration { get; set; }

        public int Order => 0;

        public void OnResultExecuted(ResultExecutedContext context)
        {
            //context.HttpContext.Response.Headers["Cache-Control"] = $"public,max-age={this.Duration}";
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers["Cache-Control"] = $"public,max-age={this.Duration}";
        }
    }
}
