using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PecsReportQueryManager;
using System.IO;

namespace OfficeReportInterface
{
    public class QueryManager
    {
        public static PecsReportQueryManange queryManager = null;

        /// <summary>
        /// 查询前初始化动态报表和时段报表接口  
        /// </summary>
        public static void InitalQueryManange(bool IsWeb)
        {
            string currentDllPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            InitalQueryManange(IsWeb, currentDllPath);
            //PecsReportQueryManange.DllFilePath = currentDllPath;
            //if (IsWeb)
            //    PecsReportQueryManange.PluginFilePath = Path.Combine(currentDllPath, @"bin\Plugins\PecsReport\");
            //else
            //    PecsReportQueryManange.PluginFilePath = Path.Combine(currentDllPath, @"Plugins\PecsReport\");
            //PecsReportQueryManange.DBConnectFilePath = AppDomain.CurrentDomain.BaseDirectory;
            //queryManager = new PecsReportQueryManange();
            //queryManager.Initialize();
        }

        /// <summary>
        /// 查询前初始化动态报表和时段报表接口  
        /// </summary>
        public static void InitalQueryManange(bool IsWeb,string exePath)
        {
            string currentDllPath = exePath;
            PecsReportQueryManange.DllFilePath = currentDllPath;
            if (IsWeb)
                PecsReportQueryManange.PluginFilePath = Path.Combine(currentDllPath, @"bin\Plugins\PecsReport\");
            else
                PecsReportQueryManange.PluginFilePath = Path.Combine(currentDllPath, @"Plugins\PecsReport\");
            PecsReportQueryManange.DBConnectFilePath = AppDomain.CurrentDomain.BaseDirectory;
            queryManager = new PecsReportQueryManange();
            queryManager.Initialize();
        }
    }
}
