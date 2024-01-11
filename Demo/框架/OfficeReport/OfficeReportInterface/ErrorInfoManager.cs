using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DBInterfaceCommonLib;

namespace OfficeReportInterface
{
    /// <summary>
    /// 日志信息管理类
    /// </summary>
    public class ErrorInfoManager
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ErrorInfoManager()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        public static readonly ErrorInfoManager Instance = new ErrorInfoManager();


        /// <summary>
        /// 写入数据库接口相关错误信息
        /// </summary>
        /// <param name="errorInstance"></param>
        /// <returns></returns>
        public bool WriteDBInterfaceLog(int errorCode, string errorInstance)
        {
            string errorStr = ErrorQuerier.Instance.GetLastErrorString();
            string errorMsg = string.Format("ErrorInstance:{0}, ErrorCode:{1}, ErrorMsg:{2}", errorInstance, errorCode, errorStr);
            bool hasWriteLog = WriteLogMessage(errorMsg);
            return hasWriteLog;
        }

        /// <summary>
        /// 写入单条日志信息
        /// </summary>
        /// <param name="logMessage">日志信息</param>
        /// <returns></returns>
        public bool WriteLogMessage(string logMessage)
        {
            string logFileName = GetLogPath();
            string logMsg = String.Format("{0} : {1}\r\n", DateTime.Now.ToString(), logMessage);

            try
            {
                if (!System.IO.File.Exists(logFileName))
                    System.IO.File.WriteAllText(logFileName, logMsg, Encoding.UTF8);
                else
                    System.IO.File.AppendAllText(logFileName, logMsg, Encoding.UTF8);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 写入单条日志信息
        /// </summary>
        /// <param name="errorInstance">错误源</param>
        /// <param name="errorMsg">错误信息</param>
        /// <returns></returns>
        public bool WriteLogMessage(string errorInstance, string errorMsg)
        {
            string logFileName = GetLogPath();
            string logMsg = String.Format("{0} : ErrorInstance:{1}, ErrorMessage:{2}\r\n", DateTime.Now.ToString(), errorInstance, errorMsg);

            try
            {
                if (!System.IO.File.Exists(logFileName))
                    System.IO.File.WriteAllText(logFileName, logMsg, Encoding.UTF8);
                else
                    System.IO.File.AppendAllText(logFileName, logMsg, Encoding.UTF8);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 写入单条日志信息
        /// </summary>
        /// <param name="errorInstance">错误源</param>
        /// <param name="exceptionMsg">错误信息</param>
        /// <returns></returns>
        public bool WriteLogMessage(string errorInstance, Exception exMsg)
        {
            string logFileName = GetLogPath();
            string logMsg = String.Format("{0} : ErrorInstance:{1}, ErrorMessage:{2},\r\n    StackTrace:{3}\r\n", DateTime.Now.ToString(), errorInstance, exMsg.Message, exMsg.StackTrace);

            try
            {
                if (!System.IO.File.Exists(logFileName))
                    System.IO.File.WriteAllText(logFileName, logMsg, Encoding.UTF8);
                else
                    System.IO.File.AppendAllText(logFileName, logMsg, Encoding.UTF8);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 批量写入多条日志信息
        /// </summary>
        /// <param name="logMessages">日志信息列表</param>
        /// <returns></returns>
        public bool WriteMultyLogMessages(List<string> logMessages)
        {
            string logFileName = GetLogPath();
            StreamWriter sw = new StreamWriter(logFileName, true);

            try
            {
                foreach (string logItem in logMessages)
                {
                    string logMsg = String.Format("{0} : {1}", DateTime.Now.ToString(), logItem);
                    sw.WriteLine(logMsg);
                }
                sw.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取日志文件路径
        /// </summary>
        /// <returns></returns>
        private static string GetLogPath()
        {
            string DirectoryPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\')).FullName, "Log");
            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);
            string logFileName = Path.Combine(DirectoryPath, "ErrorMsg.log");
            return logFileName;
        }
    }
}