using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.MiddlewareExtend.SimpleExtend
{
    /// <summary>
    /// 替换默认的middleware工厂---其实源码是抄的
    /// </summary>
    public class SecondNewMiddlewareFactory : IMiddlewareFactory
    {
        private readonly IServiceProvider _iServiceProvider;
        private readonly ILogger _logger;
        public SecondNewMiddlewareFactory(IServiceProvider serviceProvider, ILogger<SecondNewMiddlewareFactory> logger)
        {
            this._iServiceProvider = serviceProvider;
            this._logger = logger;
        }

        public IMiddleware Create(Type middlewareType)
        {
            Console.WriteLine("替换默认Middleware工厂");
            return (IMiddleware)this._iServiceProvider.GetService(middlewareType)!;
        }

        /// <summary>
        /// middleware响应时才生成，尽快释放
        /// </summary>
        /// <param name="middleware"></param>
        public void Release(IMiddleware middleware)
        {
            if (middleware != null)
            {
                (middleware as IDisposable)?.Dispose();
            }
        }
    }
}
