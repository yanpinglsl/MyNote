using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreExtend.FilterExtend.SimpleExtend
{
    /// <summary>
    /// 内存缓存的ResourceFilter
    /// </summary>
    public class CustomDictionaryResourceFilterAttribute : Attribute, IResourceFilter, IFilterMetadata, IOrderedFilter
    {
        private static Dictionary<string, IActionResult> CustomCache = new Dictionary<string, IActionResult>();

        public int Order => 0;

        /// <summary>
        /// 发生在其他动作之前，在控制器实例化之前
        /// 拦截请求---管理资源--就是为了缓存！
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            Console.WriteLine($"This is {nameof(CustomDictionaryResourceFilterAttribute) } OnResourceExecuting");
            string key = context.HttpContext.Request.Path;//指定key，有时候应该带上参数

            //throw new Exception($"This is Eleven's {nameof(CustomDictionaryResourceFilterAttribute)} OnResourceExecuting  Exception");

            if (CustomCache.ContainsKey(key))//有缓存，直接返回缓存
            {
                //Console.WriteLine($"This is {nameof(CustomDictionaryResourceFilterAttribute) } OnResourceExecuting ReadCache");
                context.Result = CustomCache[key];//断路器--到Result生成了，但是Result还需要转换成Html
            }
        }
        /// <summary>
        /// 发生在其他动作之后，Result执行之后--ResultFilter执行之后
        /// 那岂不是知道请求相应的结果---把结果可以加入缓存
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            Console.WriteLine($"This is {nameof(CustomDictionaryResourceFilterAttribute) } OnResourceExecuted");
            //这个应该缓存起来
            string key = context.HttpContext.Request.Path;

            //throw new Exception($"This is Eleven's {nameof(CustomDictionaryResourceFilterAttribute)} OnResourceExecuted  Exception");

            if (!CustomCache.ContainsKey(key))
            {
                Console.WriteLine($"This is {nameof(CustomDictionaryResourceFilterAttribute) } OnResourceExecuted AddCache");
                CustomCache.Add(key, context.Result);
                if (context.Result is JsonResult)//Json打印下
                {
                    Console.WriteLine($"context.Result: {Newtonsoft.Json.JsonConvert.SerializeObject(context.Result)}");
                }
            }
        }
    }

    /// <summary>
    /// Redis缓存---设置Order -1  先执行， 没有实现Redis保存数据
    /// </summary>
    public class CustomRedisResourceFilterAttribute : Attribute, IResourceFilter, IFilterMetadata, IOrderedFilter
    {
        private static Dictionary<string, IActionResult> CustomCache = new Dictionary<string, IActionResult>();

        public int Order => -1;

        /// <summary>
        /// 发生在其他动作之前
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            Console.WriteLine($"This is {nameof(CustomRedisResourceFilterAttribute) } OnResourceExecuting");
            string key = context.HttpContext.Request.Path;//指定key，有时候应该带上参数
            if (CustomCache.ContainsKey(key))//有缓存，直接返回缓存
            {
                //context.Result = CustomCache[key];//断路器--到Result生成了，但是Result还需要转换成Html
            }
        }
        /// <summary>
        /// 发生在其他动作之后
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            Console.WriteLine($"This is {nameof(CustomRedisResourceFilterAttribute) } OnResourceExecuted");
            string key = context.HttpContext.Request.Path;

            if (!CustomCache.ContainsKey(key))
            {
                //CustomCache.Add(key, context.Result);//不缓存
            }
        }
    }

    /// <summary>
    /// Redis缓存---设置Order -1  先执行， 没有实现Redis保存数据
    /// </summary>
    public class CustomAsyncDictionaryResourceFilterAttribute : Attribute, IAsyncResourceFilter, IFilterMetadata, IOrderedFilter
    {
        private static Dictionary<string, IActionResult> CustomCache = new Dictionary<string, IActionResult>();

        public int Order => -1;

        /// <summary>
        /// 异步版本
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="next">其实是包裹步骤组成的委托</param>
        /// <returns></returns>
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            Console.WriteLine($"This is {nameof(CustomAsyncDictionaryResourceFilterAttribute) } OnResourceExecutionAsync -Begin");
            string key = context.HttpContext.Request.Path;
            if (CustomCache.ContainsKey(key))
            {
                context.Result = CustomCache[key];
            }
            else
            {
                var result = await next.Invoke();
                if (!CustomCache.ContainsKey(key))
                {
                    //CustomCache.Add(key, context.Result);//不缓存
                }
                Console.WriteLine($"This is {nameof(CustomAsyncDictionaryResourceFilterAttribute) } OnResourceExecutionAsync -End");
            }
            Console.WriteLine("***********************************************");
        }
    }
}
