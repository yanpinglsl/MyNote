using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DBInterfaceCommonLib;
using System.Net;
using System.Text.RegularExpressions;

namespace OfficeReportInterface
{
    /// <summary>
    /// 同步状态描述
    /// </summary>
    public static class SynStatusStr
    {
        public static string SynChronNotCreate = LocalResourceManager.GetInstance().GetString("1074", "Not created");
        public static string SynChronSucsess = LocalResourceManager.GetInstance().GetString("1075", "Synchronized");
        public static string SynChronUnKnown = LocalResourceManager.GetInstance().GetString("1076", "Unknown Status");
        public static string SynChronConflic = LocalResourceManager.GetInstance().GetString("1077", "Data Conflict");
        public static string SynChronError = LocalResourceManager.GetInstance().GetString("1078", "Synchronization Error");
        public static string SynChroning = LocalResourceManager.GetInstance().GetString("1079", "Synchronizing");
    }

    /// <summary>
    /// 同步状态代码
    /// </summary>
    public static class SynStatus
    {
        public const int SynChronNotCreate = 0;
        public const int SynChronSucsess = 3;
        public const int SynChronFailed = 6;
    }

    /// <summary>
    /// 获取同步状态
    /// </summary>
    public class Synchronic
    {
        ///// <summary>
        ///// 获取同步状态
        ///// </summary>
        ///// <param name="isConfig">配置或者历史</param>
        ///// <param name="synchronicStatus">获取同步状态</param>
        ///// <returns>错误信息</returns>
        //public static string GetSynchronicStatus(bool isConfig, ref string synchronicStatus)
        //{
        //    synchronicStatus = SynStatusStr.SynChronUnKnown;
        //    DataTable synchronStatusDataTable = new DataTable();
        //    string result = GetSynchronStatusDataTable(isConfig, ref synchronStatusDataTable);
        //    if (result != string.Empty)
        //        return result;

        //    DataTable conflicTable = new DataTable();
        //    result = GetConflictTableDataTable(isConfig, ref conflicTable);
        //    if (result != string.Empty)
        //        return result;

        //    synchronicStatus = GetSynChronStatus(conflicTable, synchronStatusDataTable);
        //    return string.Empty;
        //}

        /// <summary>
        /// 获取同步状态
        /// </summary>
        /// <param name="conflicTable">数据冲突列表</param>
        /// <param name="queryResult">同步状态列表</param>
        /// <returns>同步状态</returns>
        private static string GetSynChronStatus(DataTable conflicTable, DataTable synchronStatusDataTable)
        {
            if (synchronStatusDataTable.Rows.Count == 0)
                return SynStatusStr.SynChronNotCreate;

            if (synchronStatusDataTable.Rows[0].ItemArray[0] == DBNull.Value)
                return SynStatusStr.SynChronUnKnown;

            if (synchronStatusDataTable.Rows[0].ItemArray[4] == DBNull.Value)
                return SynStatusStr.SynChronUnKnown;

            int status = Convert.ToInt32(synchronStatusDataTable.Rows[0].ItemArray[4]);
            if (status == SynStatus.SynChronNotCreate)
                return SynStatusStr.SynChronNotCreate;

            if (status == SynStatus.SynChronFailed)
                return SynStatusStr.SynChronError;

            if (conflicTable.Rows.Count > 0)
                return SynStatusStr.SynChronConflic;

            return SynStatusStr.SynChronSucsess;
        }

        ///// <summary>
        ///// 获取冲突的对象列表
        ///// </summary>
        ///// <param name="isConfig">配置或者历史</param>
        ///// <param name="queryResult">冲突的对象列表</param>
        ///// <returns>错误信息</returns>
        //private static string GetConflictTableDataTable(bool isConfig, ref DataTable queryResult)
        //{
        //    int code = 0;
        //    if (isConfig)
        //        code = DBMConfigProvider.Instance.ReadConflictTables(ref queryResult);
        //    else
        //        code = DBMHistoryProvider.Instance.ReadConflictTables(ref queryResult);

        //    if (code != 0)
        //    {
        //        if(isConfig)
        //            ErrorInfoManager.Instance.WriteDBInterfaceLog(code, "DBMConfigProvider.Instance.ReadConflictTables");
        //        else
        //            ErrorInfoManager.Instance.WriteDBInterfaceLog(code, "DBMHistoryProvider.Instance.ReadConflictTables");

        //        return LocalResourceManager.GetInstance().GetString("1231","Database error! Can not get the information!");
        //    }       

        //    return string.Empty;
        //}

        ///// <summary>
        ///// 获取数据库同步状态
        ///// </summary>
        ///// <param name="isConfig">配置或者历史</param>
        ///// <param name="queryResult">数据库同步状态</param>
        ///// <returns>错误信息</returns>
        //private static string GetSynchronStatusDataTable(bool isConfig, ref DataTable queryResult)
        //{
        //    int code = 0;
        //    if (isConfig)
        //        code = DBMConfigProvider.Instance.ReadSynchronStatus(ref queryResult);
        //    else
        //        code = DBMHistoryProvider.Instance.ReadSynchronStatus(ref queryResult);

        //    if (code != 0)
        //    {
        //        if (isConfig)
        //            ErrorInfoManager.Instance.WriteDBInterfaceLog(code, "DBMConfigProvider.Instance.ReadSynchronStatus");
        //        else
        //            ErrorInfoManager.Instance.WriteDBInterfaceLog(code, "DBMHistoryProvider.Instance.ReadSynchronStatus");

        //        return LocalResourceManager.GetInstance().GetString("1231","Database error! Can not get the information!");
        //    }  

        //    return string.Empty;
        //}
    }

    /// <summary>
    /// 判断主用数据库是否为发布数据库
    /// </summary>
    public class SynchronicPublisher
    {
        ///// <summary>
        ///// 判断主用数据库是否为发布数据库
        ///// </summary>
        ///// <param name="isRight">主用数据库为发布数据库</param>
        ///// <returns>错误信息</returns>
        //public static string IsRightPrimaryDataSource(string primaryDataSource, ref bool isRight)
        //{
        //    string publisher = string.Empty;
        //    string result = GetPublisher(true, ref publisher);
        //    if (result != string.Empty)
        //        result = GetPublisher(false, ref publisher);
        //    if (result != string.Empty)
        //        return result;

        //    if (publisher == string.Empty)
        //        return LocalResourceManager.GetInstance().GetString("1080", "Can not give a right reault because of the publisher is empty.");

        //    isRight = true;
        //    if (string.Compare(publisher, ConvertDataSource(primaryDataSource), true) != 0)
        //        isRight = false;

        //    return string.Empty;
        //}

        ///// <summary>
        ///// 获取同步发布数据库
        ///// </summary>
        ///// <param name="nameFlag">配置或者历史</param>
        ///// <param name="publisher">返回发布数据库名称</param>
        ///// <returns>错误信息</returns>
        //private static string GetPublisher(bool isConfig, ref string publisher)
        //{
        //    DataTable queryResult = new DataTable();
        //    string result = GetPublisherDataTable(isConfig, ref queryResult);
        //    if (result != string.Empty)
        //        return result;
        //    if (queryResult.Rows.Count == 0 || queryResult.Rows[0].ItemArray[0] == DBNull.Value)
        //        return string.Empty;
        //    publisher = queryResult.Rows[0].ItemArray[0].ToString();
        //    return string.Empty;
        //}

        ///// <summary>
        ///// 获取同步数据库列表
        ///// </summary>
        ///// <param name="isConfig">配置或者历史</param>
        ///// <param name="queryResult">同步数据库列表</param>
        ///// <returns>错误信息</returns>
        //private static string GetPublisherDataTable(bool isConfig, ref DataTable queryResult)
        //{
        //    int code = 0;
        //    if (isConfig)
        //        code = DBMConfigProvider.Instance.GetPublisher(ref queryResult);
        //    else
        //        code = DBMHistoryProvider.Instance.GetPublisher(ref queryResult);

        //    if (code != 0)
        //    {
        //        if (isConfig)
        //            ErrorInfoManager.Instance.WriteDBInterfaceLog(code, "DBMConfigProvider.Instance.GetPublisher");
        //        else
        //            ErrorInfoManager.Instance.WriteDBInterfaceLog(code, "DBMHistoryProvider.Instance.GetPublisher");

        //        return LocalResourceManager.GetInstance().GetString("1231","Database error! Can not get the information!");
        //    }    

        //    return string.Empty;
        //}

        /// <summary>
        /// 转换数据库服务器名称
        /// </summary>
        /// <param name="dataSource">数据库服务器</param>
        /// <returns>转换得到的服务器名称</returns>
        private static string ConvertDataSource(string dataSource)
        {
            string serverName = dataSource.ToLower();
            int index = dataSource.IndexOf('\\');
            if (index > 0)
                serverName = dataSource.Substring(0, index).ToLower();

            if (serverName == "(local)")
                return dataSource.Replace("(local)", Dns.GetHostName());

            if (serverName == "127.0.0.1")
                return dataSource.Replace("127.0.0.1", Dns.GetHostName());

            if (serverName == ".")
                return Dns.GetHostName();

            if (IsIP(serverName))
                return dataSource.Replace(serverName, Dns.GetHostEntry(serverName).HostName);

            return dataSource;
        }

        /// <summary>
        /// 判读是否为IP地址
        /// </summary>
        /// <param name="ip">输入字符串</param>
        /// <returns>是否为IP地址</returns>
        private static bool IsIP(string ip)
        {
            Match match = Regex.Match(ip, @"(\d+)\.(\d+)\.(\d+)\.(\d+)");
            if (!match.Success)
                return false;

            return true;
        }
    }

}
