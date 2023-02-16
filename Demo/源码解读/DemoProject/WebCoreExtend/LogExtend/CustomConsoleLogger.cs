using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.LogExtend
{
    public class CustomConsoleLogger : ILogger
    {
        private readonly CustomConsoleLoggerOptions _CustomConsoleLoggerOptions;
        public CustomConsoleLogger(CustomConsoleLoggerOptions options)
        {
            this._CustomConsoleLoggerOptions = options;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;//没有啥需要释放
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= this._CustomConsoleLoggerOptions.MinLogLevel;
            //让上端注册的地方，最终可以影响到组件的行为
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!this.IsEnabled(logLevel))
                return;

            //日志输出到控制台
            Console.BackgroundColor = this._CustomConsoleLoggerOptions.ConsoleColor;

            Console.WriteLine($"Eleven Custom Log: {logLevel} - {eventId.Id} - {formatter(state, exception)}");

        }
    }
}
