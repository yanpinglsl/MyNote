using System;
using System.IO;
using System.Text;
using System.Threading;

public class DbgTrace
{
    /// <summary>
    /// 用于标识DbgtraceOral是否被初始化了。如果初始化了，就使用DbgTraceOral（桌面版的OfficeReport）；如果没有，就使用iemsweb使用的ErrorInfoManager
    /// </summary>
    private static bool hasInitialed = false;

    private static string logFileName = string.Format("OfficeReportGenerateService-{0}-{1}-{2}-{3}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour);

    /// <summary>
    /// 设置日志文件的名称（不包含路径）
    /// </summary>
    /// <param name="name"></param>
    public static void SetLogFileName(string name)
    {
        logFileName = string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}.log",name, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour,DateTime.Now.Minute,DateTime.Now.Second); 
    }
   
    /// <summary>
    /// OfficeReport.ini文件的路径
    /// </summary>
    /// <returns></returns>
    public static string GetOfficeReportIniFilePath()
    {
        string name = "OfficeReport.ini";
        string assemblyPath = DbgTrace.GetAssemblyPath();
        string fileName = Path.Combine(assemblyPath, name);
        return fileName;
    }
    /// <summary>  
    /// 获取Assembly的运行路径  
    /// </summary>  
    ///<returns></returns>  
    public static string GetAssemblyPath()
    {
        string _CodeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

        _CodeBase = _CodeBase.Substring(8, _CodeBase.Length - 8); // 8是file:// 的长度  

        string[] arrSection = _CodeBase.Split(new char[] { '/' });

        string _FolderPath = "";
        for (int i = 0; i < arrSection.Length - 1; i++)
        {
            _FolderPath += arrSection[i] + "/";
        }

        return _FolderPath;
    }
    /// <summary>
    /// 获取当前dll所在目录的父目录
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentParentPath()
    {
        string path = GetAssemblyPath();
        DirectoryInfo di = new DirectoryInfo(path);
        DirectoryInfo parentDi = di.Parent;
        return parentDi.FullName;
    }

    /// <summary>
    /// 判断是否是OfficeReport.exe在使用这个OfficeReportInterface.dll
    /// </summary>
    /// <returns></returns>
    public static bool IsOfficeReport()
    {
        string name = logFileName.Substring(0, logFileName.IndexOf("-", System.StringComparison.Ordinal));
        return name == "OfficeReport";
    }

    /// <summary>
    /// 日志信息管理类
    /// </summary>
    private class ErrorInfoManager
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
        /// 写入单条日志信息
        /// </summary>
        /// <param name="logMessage">日志信息</param>
        /// <returns></returns>
        public bool WriteLogMessage(string logMessage)
        {
            //if (hasInitialed)//使用dbgtraceoral写日志
            //{
            //    DbgTraceOral.dout(logMessage);
            //    return true;
            //}

            return WriteLogMessageByErrorInfoManager(logMessage);
        }

        private bool WriteLogMessageByErrorInfoManager(string logMessage)
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
        /// 获取日志文件路径
        /// </summary>
        /// <returns></returns>
        private static string GetLogPath()
        {
            string webAppRootPath = GetCurrentParentPath();
            string DirectoryPath = Path.Combine(webAppRootPath, "Log");
            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);
            string logFileFullName = Path.Combine(DirectoryPath, logFileName);

            if (File.Exists(logFileFullName) && (new FileInfo(logFileFullName)).Length > 1024*1024) //如果文件过长，就使用新的文件写日志
            {
                string name = logFileName.Substring(0, logFileName.IndexOf("-", System.StringComparison.Ordinal));
                var logFileNameTemp = string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}.log", name, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                Interlocked.Exchange(ref logFileName, logFileNameTemp);
                logFileFullName = Path.Combine(DirectoryPath, logFileName);
            }

            return logFileFullName;
        }
    }

    public static void dout(string str, params object[] args)
    {
        try
        {
            if (args == null || args.Length == 0)
                ErrorInfoManager.Instance.WriteLogMessage(str);
            else
            {
                ErrorInfoManager.Instance.WriteLogMessage(string.Format(str, args));
            }
        }
        catch (Exception ex)
        {

        }
    }

    public static void writeInLog(string format)
    {
        ErrorInfoManager.Instance.WriteLogMessage(format);
    }

    public static void doutc(object redOnWhite, string s, string message)
    {
        ErrorInfoManager.Instance.WriteLogMessage(string.Format(s, message));
    }

    public class DbgTraceColor
    {
        public static object RedOnWhite { get; set; }
    }

    public static void doutc(object redOnWhite, string s, string a, string name)
    {
        ErrorInfoManager.Instance.WriteLogMessage(string.Format(s, a,name));
    }
}


