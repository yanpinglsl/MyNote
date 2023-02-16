using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.LogExtend
{
    public static class CustomConsoleLoggerExtensions
    {
        public static ILoggingBuilder AddCustomLogger(this ILoggingBuilder builder)
        {
            //还可能有其他的IOC注册

            CustomConsoleLoggerOptions options = new CustomConsoleLoggerOptions()
            {
                MinLogLevel = LogLevel.Warning,
                ConsoleColor = ConsoleColor.Green,
            };
            return builder.AddCustomLogger(options);
        }

        public static ILoggingBuilder AddCustomLogger(this ILoggingBuilder builder, CustomConsoleLoggerOptions options)
        {
            builder.AddProvider(new CustomConsoleLoggerProvider(options));
            return builder;
        }

        public static ILoggingBuilder AddCustomLogger(this ILoggingBuilder builder, Action<CustomConsoleLoggerOptions> configure)
        {
            CustomConsoleLoggerOptions options = new CustomConsoleLoggerOptions()
            {
                MinLogLevel = LogLevel.Warning,
                ConsoleColor = ConsoleColor.Green,
            };
            configure.Invoke(options);

            return builder.AddCustomLogger(options);
        }
    }
}
