using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.LogExtend
{
    public class CustomConsoleLoggerProvider : ILoggerProvider
    {
        private CustomConsoleLoggerOptions _CustomConsoleLoggerOptions = null;
        public CustomConsoleLoggerProvider(CustomConsoleLoggerOptions options)
        {
            this._CustomConsoleLoggerOptions = options;
        }


        public ILogger CreateLogger(string categoryName)
        {
            this._CustomConsoleLoggerOptions.Init("12345");

            return new CustomConsoleLogger(this._CustomConsoleLoggerOptions);
        }

        public void Dispose()
        {

        }
    }
}
