using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.MiddlewareExtend.StandardMiddlewareExtend
{
    public class BrowserCheckService : IBrowserCheck
    {
        private readonly BrowserFilterOptions _BrowserFilterOptions;
        private readonly ILogger _logger;
        public BrowserCheckService(ILogger<BrowserCheckService> logger, IOptions<BrowserFilterOptions> options)
        {
            this._logger = logger;
            this._BrowserFilterOptions = options.Value;
        }

        public Tuple<bool, string> CheckBrowser(HttpContext httpContext)
        {
            Console.WriteLine($"EnableChorme={this._BrowserFilterOptions.EnableChorme}");
            Console.WriteLine($"EnableEdge={this._BrowserFilterOptions.EnableEdge}");
            Console.WriteLine($"EnableFirefox={this._BrowserFilterOptions.EnableFirefox}");
            Console.WriteLine($"EnableIE={this._BrowserFilterOptions.EnableIE}");
            
            if (httpContext.Request.Headers["User-Agent"].ToString().Contains("Edg/") && !this._BrowserFilterOptions.EnableEdge)
            {
                Console.WriteLine($"{nameof(BrowserFilterMiddleware)} Refuse Edge,Choose other one<br/>");
                return Tuple.Create(false, $"{nameof(BrowserFilterMiddleware)} Refuse Edge,Choose other one<br/>");
            }
            else
            {
                return Tuple.Create(true, $"{nameof(BrowserFilterMiddleware)} ok");
            }
        }
        //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36
        //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36 Edg/91.0.864.64
    }
}
