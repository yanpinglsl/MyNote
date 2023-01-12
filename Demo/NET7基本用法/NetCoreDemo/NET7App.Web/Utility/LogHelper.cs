using NLog.Web;
using NLog;

namespace NET7App.Web.Utility
{
    public static class LogHelper
    {
        /// <summary>
        /// 配置Log4Net
        /// </summary>
        /// <param name="builder"></param>
        public static void Log4NetRegister(this WebApplicationBuilder builder)
        {
            //1、引入Nuget包log4net、Microsoft.Extensions.Logging.Log4Net.AspNetCore
            //  （如果需要将日志写入数据库，则还需要引入System.Data.SqlClient包）
            //2、准备配置文件CfgFile/log4net.Config
            //3.让配置文件生效
            builder.Logging.AddLog4Net("CfgFile/log4net.Config");
            //builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
        }

        /// <summary>
        /// 配置Nlog
        /// </summary>
        /// <param name="builder"></param>
        public static void NLogRegister(this WebApplicationBuilder builder)
        {
            //1、引入Nuget包NLog、NLog.Web.AspNetCore
            //  （如果需要将日志写入数据库，则还需要引入NLog.Database包）
            //2、准备配置文件CfgFile/Nlog.Config
            //3.让配置文件生效
            NLog.LogManager.Setup().LoadConfigurationFromFile("CfgFile/NLog.config").GetCurrentClassLogger();
            //builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            builder.Host.UseNLog();

        }
    }
}
