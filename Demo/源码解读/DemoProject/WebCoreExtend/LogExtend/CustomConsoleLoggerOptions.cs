using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCoreExtend.LogExtend
{
    public class CustomConsoleLoggerOptions
    {
        public LogLevel MinLogLevel { get; set; } = LogLevel.Information;

        public ConsoleColor ConsoleColor { get; set; } = ConsoleColor.Black;

        public int EventId { get; set; } = 0;

        public void Init(string message)
        {

        }
    }
}
