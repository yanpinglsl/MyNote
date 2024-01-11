using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInterfaceCommonLib;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using System.Net;
using CSharpDBPlugin;
using ErrorCode = DBInterfaceCommonLib.ErrorCode;

namespace OfficeReportInterface
{
    public class DatabaseStatus
    {
        public string _projectName = string.Empty;

        public string _primaryDataSource = string.Empty;

        public string _secondaryDataSource = string.Empty;

        /// <summary>
        /// 唯一实例
        /// </summary>
        public readonly static DatabaseStatus DataManager = new DatabaseStatus();

        ///// <summary>
        ///// 获取数据库状态
        ///// </summary>
        ///// <param name="filePath">Database.Config存储路径</param>
        ///// <param name="databaseInfoList">数据库状态列表</param>
        ///// <returns>错误信息，0-正确</returns>
        //public List<DatabaseStatusDef> ReadDatabaseStatus()
        //{
        //    List<DatabaseStatusDef> resultList = new List<DatabaseStatusDef>();

        //    string dataSource = string.Empty;
        //    string result = GetDataSource();
        //    if (result != string.Empty)
        //        _primaryDataSource = _secondaryDataSource = string.Format(LocalResourceManager.GetInstance().GetString("1221", "Error:{0}"), result);

        //    resultList.Add(PrimaryConfigurationDatabase());
        //    resultList.Add(PrimaryHistoricalDatabase());
        //    resultList.Add(AuoExport());

        //    bool hasSecondary = false;
        //    int code = PxDBInfoProvider.Instance.HasSecondary(ref hasSecondary);
        //    if (code != (int)ErrorCode.Success)
        //        ErrorInfoManager.Instance.WriteDBInterfaceLog(code, "PxDBInfoProvider.HasSecondary");

        //    if (!hasSecondary)
        //        return resultList;

        //    resultList.Add(SecondaryConfigurationDatabase());
        //    resultList.Add(SecondaryHistoricalDatabase());
        //    resultList.Add(SyschronicStatus());
        //    resultList.Add(PublisherServer());

        //    return resultList;
        //}

        ///// <summary>
        ///// 备用历史数据库状态
        ///// </summary>
        ///// <param name="databaseInfoList">数据库状态列表</param>
        //private DatabaseStatusDef SecondaryHistoricalDatabase()
        //{
        //    DatabaseStatusDef resultValue = new DatabaseStatusDef((int)DBInfoFlag.Secondary);
        //    List<List<string>> infoList = new List<List<string>>();
        //    ReadOneDatabaseInfo(DBOperationFlag.onlySecondary, false, ref infoList);
        //    resultValue.DBInfos = infoList;
        //    return resultValue;
        //}

        ///// <summary>
        ///// 备用配置数据库状态
        ///// </summary>
        ///// <param name="databaseInfoList">数据库状态列表</param>
        //private DatabaseStatusDef SecondaryConfigurationDatabase()
        //{
        //    DatabaseStatusDef resultValue = new DatabaseStatusDef((int)DBInfoFlag.Secondary);
        //    List<List<string>> infoList = new List<List<string>>();
        //    ReadOneDatabaseInfo(DBOperationFlag.onlySecondary, true, ref infoList);
        //    resultValue.DBInfos = infoList;
        //    return resultValue;
        //}

        ///// <summary>
        ///// 主用历史数据库状态
        ///// </summary>
        ///// <param name="databaseInfoList">数据库状态列表</param>
        //private DatabaseStatusDef PrimaryHistoricalDatabase()
        //{
        //    DatabaseStatusDef resultValue = new DatabaseStatusDef((int)DBInfoFlag.Primary);
        //    List<List<string>> infoList = new List<List<string>>();
        //    ReadOneDatabaseInfo(DBOperationFlag.onlyPrimary, false, ref infoList);
        //    resultValue.DBInfos = infoList;
        //    return resultValue;
        //}

        ///// <summary>
        ///// 主用配置数据库状态
        ///// </summary>
        ///// <param name="databaseInfoList">数据库状态列表</param>
        //private DatabaseStatusDef PrimaryConfigurationDatabase()
        //{
        //    DatabaseStatusDef resultValue = new DatabaseStatusDef((int)DBInfoFlag.Primary);
        //    List<List<string>> infoList = new List<List<string>>();
        //    ReadOneDatabaseInfo(DBOperationFlag.onlyPrimary, true, ref infoList);
        //    resultValue.DBInfos = infoList;
        //    return resultValue;
        //}

        ///// <summary>
        ///// 判断主用数据库是否为发布数据库
        ///// </summary>
        ///// <param name="databaseInfoList">数据库状态列表</param>
        //private DatabaseStatusDef PublisherServer()
        //{
        //    DatabaseStatusDef resultValue = new DatabaseStatusDef((int)DBInfoFlag.Syschronic);
        //    List<List<string>> infoList = new List<List<string>>();

        //    string headLine = LocalResourceManager.GetInstance().GetString("1222", "Publisher");
        //    infoList.Add(new List<string> { headLine });

        //    bool isRight = true;
        //    string isRightStr = LocalResourceManager.GetInstance().GetString("1223", "The main database is a publisher");
        //    string result = SynchronicPublisher.IsRightPrimaryDataSource(_primaryDataSource, ref isRight);
        //    if (result != string.Empty)
        //        isRightStr = string.Format(LocalResourceManager.GetInstance().GetString("1221", "Error:{0}"), result);
        //    else if (!isRight)
        //        isRightStr = string.Format(LocalResourceManager.GetInstance().GetString("1221", "Error:{0}"), LocalResourceManager.GetInstance().GetString("1224", "The main database is not a publisher"));
        //    infoList.Add(new List<string> { isRightStr });

        //    resultValue.DBInfos = infoList;
        //    return resultValue;
        //}

        ///// <summary>
        ///// 同步状态
        ///// </summary>
        ///// <param name="databaseInfoList">数据库状态列表</param>
        //private DatabaseStatusDef SyschronicStatus()
        //{
        //    DatabaseStatusDef resultValue = new DatabaseStatusDef((int)DBInfoFlag.Syschronic);
        //    List<List<string>> infoList = new List<List<string>>();

        //    string headLine = LocalResourceManager.GetInstance().GetString("1225", "Synchronization");
        //    infoList.Add(new List<string> { headLine });

        //    string configStatus = SynStatusStr.SynChronUnKnown;
        //    string result = Synchronic.GetSynchronicStatus(true, ref configStatus);
        //    if (result != string.Empty)
        //        configStatus = string.Format(LocalResourceManager.GetInstance().GetString("1221", "Error:{0}"), result);
        //    string rowHead1 = LocalResourceManager.GetInstance().GetString("1226", "Configuration Database");
        //    infoList.Add(new List<string> { rowHead1, configStatus });

        //    string historyStatus = SynStatusStr.SynChronUnKnown;
        //    result = Synchronic.GetSynchronicStatus(false, ref historyStatus);
        //    if (result != string.Empty)
        //        historyStatus = string.Format(LocalResourceManager.GetInstance().GetString("1221", "Error:{0}"), result);
        //    string rowHead2 = LocalResourceManager.GetInstance().GetString("1227", "Historical Database");
        //    infoList.Add(new List<string> { rowHead2, historyStatus });
        //    resultValue.DBInfos = infoList;

        //    return resultValue;
        //}

        #region 自动导出设置
        /// <summary>
        /// 获取自动导出设置
        /// </summary>
        /// <param name="databaseInfo">数据库状态信息</param>
        private DatabaseStatusDef AuoExport()
        {
            DatabaseStatusDef resultValue = new DatabaseStatusDef((int)DBInfoFlag.AutoExport);
            List<List<string>> infoList = new List<List<string>>();

            infoList.Add(new List<string> { LocalResourceManager.GetInstance().GetString("1228", "Auto-export Setting") });
            bool isUserSet = false;
            string isUserSetStr = LocalResourceManager.GetInstance().GetString("1229", "Disabled");
            string result = ReadAutoExportSet(ref isUserSet);
            if (result != string.Empty)
                isUserSetStr = string.Format(LocalResourceManager.GetInstance().GetString("1221", "Error:{0}"), result);
            else if (isUserSet)
                isUserSetStr = LocalResourceManager.GetInstance().GetString("1230", "Enabled");

            infoList.Add(new List<string> { isUserSetStr });

            resultValue.DBInfos = infoList;
            return resultValue;
        }

        /// <summary>
        /// 获取是否已经设置自动导出
        /// </summary>
        /// <param name="isUserSet">是否设置自动导出</param>
        /// <returns>错误信息</returns>
        private string ReadAutoExportSet(ref bool isUserSet)
        {
            isUserSet = false;
            const uint progID = 100;
            const uint userID = 100;
            const uint dataType = 100;
            DataTable queryResult = new DataTable();
            int code = UserProfileProvider.Instance.ReadUserProfiles(0, Dns.GetHostName(), progID, userID, dataType, ref queryResult);
            if (code != 0)
            {
                ErrorInfoManager.Instance.WriteDBInterfaceLog(code, "UserProfileProvider.ReadUserProfiles");
                return LocalResourceManager.GetInstance().GetString("1231", "Database error! Can not get the information!");
            }

            if (queryResult.Rows.Count == 0 || queryResult.Columns.IndexOf("Data") < 0)
                return string.Empty;

            byte[] data = (byte[])queryResult.Rows[0]["Data"];
            if (data != null)
                isUserSet = true;

            return string.Empty;
        }
        #endregion

        ///// <summary>
        ///// 读取一个数据库的信息
        ///// </summary>
        ///// <param name="dbFlag">主用或者备用</param>
        ///// <param name="isConfig">配置或者历史</param>
        ///// <param name="infoList">数据库状态列表</param>
        //private void ReadOneDatabaseInfo(DBOperationFlag dbFlag, bool isConfig, ref List<List<string>> infoList)
        //{
        //    infoList = new List<List<string>>();

        //    infoList.Add(GetHeadline(dbFlag, isConfig));
        //    GetDatabaseServerName(dbFlag, ref infoList);
        //    bool result = GetDatabseVersion(dbFlag, isConfig, ref infoList);
        //    if (result)
        //    {
        //        GetDatabseUsedSpace(dbFlag, isConfig, ref infoList);
        //        GetDatabaseFreeSpace(dbFlag, isConfig, ref infoList);
        //        GetDatabaseFilePath(dbFlag, isConfig, ref infoList);
        //    }
        //}

        /// <summary>
        /// 获取表头
        /// </summary>
        /// <param name="dbFlag">主用或者备用</param>
        /// <param name="isConfig">配置或者历史</param>
        /// <returns>表头</returns>
        private List<string> GetHeadline(DBOperationFlag dbFlag, bool isConfig)
        {
            List<string> headLines = new List<string>();

            string headLine = string.Empty;
            if (dbFlag == DBOperationFlag.onlyPrimary & isConfig == true)
                headLine = LocalResourceManager.GetInstance().GetString("1232", "Main Configuration Database");
            else if (dbFlag == DBOperationFlag.onlyPrimary & isConfig == false)
                headLine = LocalResourceManager.GetInstance().GetString("1233", "Main Historical Database");
            else if (dbFlag == DBOperationFlag.onlySecondary & isConfig == true)
                headLine = LocalResourceManager.GetInstance().GetString("1234", "Backup Configuration Database");
            else
                headLine = LocalResourceManager.GetInstance().GetString("1235", "Backup Historical Database");
            headLines.Add(headLine);

            return headLines;
        }


        /// <summary>
        /// 获取数据库的服务名称
        /// </summary>
        /// <param name="dbFlag">主用或者备用</param>
        /// <param name="infoList">数据库状态列表</param>
        private void GetDatabaseServerName(DBOperationFlag dbFlag, ref List<List<string>> infoList)
        {
            string dataSource = string.Empty;
            if (dbFlag == DBOperationFlag.onlyPrimary)
                dataSource = _primaryDataSource;
            else
                dataSource = _secondaryDataSource;
            string rowHead = LocalResourceManager.GetInstance().GetString("1236", "Database Server");

            infoList.Add(new List<string> { rowHead, dataSource });
        }

        ///// <summary>
        ///// 获取数据库文件存储路径
        ///// </summary>
        ///// <param name="dbFlag">主用或者备用</param>
        ///// <param name="isConfig">配置或者历史</param>
        ///// <param name="infoList">数据库状态列表</param>
        //private void GetDatabaseFilePath(DBOperationFlag dbFlag, bool isConfig, ref List<List<string>> infoList)
        //{
        //    string filePath = string.Empty;
        //    int code = PxDBInfoProvider.Instance.GetDBFilePath(dbFlag, ParseIsConfig(isConfig), ref filePath);
        //    if (code != 0)
        //        ErrorInfoManager.Instance.WriteDBInterfaceLog(code, "PxDBInfoProvider.GetDBFilePath");
        //    if (filePath == null)
        //        filePath = string.Empty;
        //    string rowLine = LocalResourceManager.GetInstance().GetString("1237", "File Path");

        //    infoList.Add(new List<string> { rowLine, filePath });
        //}

        private ConnectToWhichDb ParseIsConfig(bool isConfig)
        {
            return isConfig ? ConnectToWhichDb.ctwdConfig : ConnectToWhichDb.ctwdData;
        }

        ///// <summary>
        ///// 获取数据库文件所在磁盘剩余空间
        ///// </summary>
        ///// <param name="dbFlag">主用或者备用</param>
        ///// <param name="isConfig">配置或者历史</param>
        ///// <param name="infoList">数据库状态列表</param>
        //private void GetDatabaseFreeSpace(DBOperationFlag dbFlag, bool isConfig, ref List<List<string>> infoList)
        //{
        //    double spaceNotUsed = 0.0;
        //    string spaceNotUsedStr = string.Empty;
        //    int code = PxDBInfoProvider.Instance.GetDBSpaceNotUsed(dbFlag, ParseIsConfig(isConfig), ref spaceNotUsed);
        //    if (code != 0)
        //        ErrorInfoManager.Instance.WriteDBInterfaceLog(code, "PxDBInfoProvider.GetDBSpaceNotUsed");

        //    spaceNotUsed = DataFormatManager.GetFormattedDoubleByDigits(spaceNotUsed, 3);
        //    spaceNotUsedStr = string.Format("{0}MB", DataFormatManager.GetSplitDouble(spaceNotUsed));
        //    string rowLine = LocalResourceManager.GetInstance().GetString("1238", "Disk Space Free");

        //    infoList.Add(new List<string> { rowLine, spaceNotUsedStr });
        //}

        ///// <summary>
        ///// 获取数据库已用空间
        ///// </summary>
        ///// <param name="dbFlag">主用或者备用</param>
        ///// <param name="isConfig">配置或者历史</param>
        ///// <param name="infoList">数据库状态列表</param>
        //private void GetDatabseUsedSpace(DBOperationFlag dbFlag, bool isConfig, ref List<List<string>> infoList)
        //{
        //    double spaceUsed = 0.0;
        //    string spaceUsedStr = string.Empty;
        //    int code = PxDBInfoProvider.Instance.GetDBSpaceUsed(dbFlag, ParseIsConfig(isConfig), ref spaceUsed);
        //    if (code != 0)
        //        ErrorInfoManager.Instance.WriteDBInterfaceLog(code, "PxDBInfoProvider.GetDBSpaceUsed");

        //    spaceUsed = DataFormatManager.GetFormattedDoubleByDigits(spaceUsed, 3);
        //    spaceUsedStr = string.Format("{0}MB", DataFormatManager.GetSplitDouble(spaceUsed));
        //    string rowLine = LocalResourceManager.GetInstance().GetString("1239", "Database Size");
        //    infoList.Add(new List<string> { rowLine, spaceUsedStr });
        //}

        ///// <summary>
        ///// 获取数据库版本信息显示
        ///// </summary>
        ///// <param name="dbFlag">主用或者备用</param>
        ///// <param name="isConfig">配置或者历史</param>
        ///// <param name="infoList">数据库状态列表</param>
        //private bool GetDatabseVersion(DBOperationFlag dbFlag, bool isConfig, ref List<List<string>> infoList)
        //{
        //    bool result = true;
        //    string databaseVersion = string.Empty;
        //    result = GetOneServerDbVersion(dbFlag, isConfig, ref databaseVersion);
        //    if (!result)
        //    {
        //        databaseVersion = string.Format(LocalResourceManager.GetInstance().GetString("1221", "Error:{0}"), LocalResourceManager.GetInstance().GetString("1231", "Database error! Can not get the information!"));
        //        result = false;
        //    }
        //    string rowLine = LocalResourceManager.GetInstance().GetString("1240", "Database Version");

        //    infoList.Add(new List<string> { rowLine, databaseVersion });
        //    return result;
        //}

        #region 获取版本信息
        /// <summary>
        /// programNameList
        /// </summary>
        private static List<string> programNameList = new List<string>();

        /// <summary>
        /// 初始化ProgramName列表
        /// </summary>
        private static void InitProgramNameList()
        {
            programNameList.Clear();
            programNameList.Add("PECSTAR_DB_SQL");
            programNameList.Add("EMS_DB_SQL");
        }

        ///// <summary>
        ///// 获取一个数据库的版本
        ///// </summary>
        ///// <param name="dbFlag">主用或者备用</param>
        ///// <param name="isConfig">配置或者历史</param>
        ///// <param name="programName">programName</param>
        ///// <param name="version">返回版本信息</param>
        ///// <returns>错误信息，0-正确</returns>
        //private bool GetOneServerDbVersion(DBOperationFlag dbFlag, bool isConfig, ref string dbVersion)
        //{
        //    InitProgramNameList();
        //    foreach (string programName in programNameList)
        //    {
        //        string description = string.Empty;
        //        DateTime setupDate = new DateTime();
        //        string version = string.Empty;
        //        int code = SystemBuildProvider.Instance.ReadSystemBuild(dbFlag, ParseIsConfig(isConfig), programName, ref description, ref setupDate, ref version);
        //        if (code != 0)
        //        {
        //            ErrorInfoManager.Instance.WriteDBInterfaceLog(code, "SystemBuildProvider.ReadSystemBuild");
        //            return false;
        //        }

        //        if (programName == programNameList[0])
        //            dbVersion = version;
        //        else
        //            dbVersion = string.Format("Pecstar:{0}/iEMS:{1}", dbVersion, version);
        //    }

        //    return true;
        //}
        # endregion

        #region 获取服务器名称

        /// <summary>
        /// 获取服务器名称
        /// </summary>
        /// <param name="filePath">Database.Config存储路径</param>
        /// <returns>错误信息</returns>
        public string GetDataSource()
        {

            string _primaryConnectionString = string.Empty;
            string _secondaryConnectionString = string.Empty;
            string connFile = Path.Combine(DbgTrace.GetAssemblyPath(), "dbconn.cfg");
            string result = LoadFromFile(connFile, ref _primaryConnectionString, ref _secondaryConnectionString);
            if (result != string.Empty)
                return result;

            _primaryDataSource = GetValue(_primaryConnectionString, "Data Source");
            bool _hasSecondary = (_secondaryConnectionString.Length > 0);
            if (_hasSecondary)
                _secondaryDataSource = GetValue(_secondaryConnectionString, "Data Source");

            _primaryDataSource = GetRealDataSource(_primaryDataSource);
            _secondaryDataSource = GetRealDataSource(_secondaryDataSource);

            return string.Empty;
        }

        /// <summary>
        /// 如果配置的是"(local)"或者"."这类信息时，将其转换成本机的主机名
        /// </summary>
        private string GetRealDataSource(string oriDataSource)
        {
            string resultSource = oriDataSource;
            if (oriDataSource.Trim().ToUpper() == "(LOCAL)" || oriDataSource.Trim() == "." || oriDataSource.Trim() == "127.0.0.1")
                resultSource = Dns.GetHostName();
            if (oriDataSource.Trim().ToUpper().Contains("(LOCAL)\\"))
                resultSource = oriDataSource.Trim().ToUpper().Replace("(LOCAL)", Dns.GetHostName());
            if (oriDataSource.Trim().Contains(".\\"))
                resultSource = oriDataSource.Trim().Replace(".\\", Dns.GetHostName() + "\\");
            if (oriDataSource.Trim().Contains("127.0.0.1\\"))
                resultSource = oriDataSource.Trim().Replace("127.0.0.1\\", Dns.GetHostName() + "\\");
            return resultSource;
        }

        /// <summary>
        /// 从dbconn.cfg文件内加载数据库连接信息
        /// </summary>
        /// <param name="filePath">dbconn文件的路径</param>
        private string LoadFromFile(string filePath, ref string _primaryConnectionString, ref string _secondaryConnectionString)
        {
            FileStream fileStream;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fileStream);
                uint fileFlag = br.ReadUInt32();
                uint fileVersion = br.ReadUInt32();
                //跳过时标,SYSTEMTIME
                fileStream.Position += 16;
                //projectName
                _projectName = ReadOneString(br, fileFlag, fileVersion);
                _primaryConnectionString = ReadOneString(br, fileFlag, fileVersion);
                _secondaryConnectionString = ReadOneString(br, fileFlag, fileVersion);
                br.Close();
                fileStream.Close();
            }
            catch (System.IO.FileNotFoundException ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        /// <summary>
        /// 从指定字符串中提取指定键的值，key和value前后的空格都会被忽略
        /// </summary>
        /// <param name="connectionString">格式为： key1 =value1; key2= value2; key3 = value3;</param>
        /// <param name="key">要查找的键</param>
        /// <returns>该键的值</returns>
        private string GetValue(string connectionString, string key)
        {
            string[] keyValueList = connectionString.Split(new Char[] { ';' });
            string pattern = "^(?<key>[\\w\\s]+)=(?<value>.*)$";
            foreach (string keyValue in keyValueList)
            {
                Match m = Regex.Match(keyValue, pattern);
                if (m.Success)
                {
                    string tempKey = m.Groups["key"].Value;
                    if (string.Compare(key, tempKey.Trim(), true) == 0)
                        return m.Groups["value"].Value;
                }
            }

            return "";
        }

        /// <summary>
        /// 从缓存内读取一个字符串
        /// </summary>
        /// <param name="br">缓存</param>
        /// <param name="fileFlag">解码用的</param>
        /// <param name="fileVersion">解码用的</param>
        /// <returns></returns>
        private string ReadOneString(BinaryReader br, uint fileFlag, uint fileVersion)
        {
            int len = br.ReadInt32();
            byte[] buffer = new byte[len];
            br.Read(buffer, 0, len);
            DecodeBuf(fileFlag, fileVersion, ref buffer, len);
            return ByteArrayToString(buffer);
        }

        /// <summary>
        /// 连接信息解码
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="buf"></param>
        /// <param name="buflen"></param>
        private void DecodeBuf(uint key1, uint key2, ref byte[] buf, int buflen)
        {
            byte[] keybuf = new byte[8];
            byte acc;
            for (int i = 0; i < buflen; i++)
            {
                if (buf[i] != 0x00)
                    buf[i] = (byte)((int)buf[i] - 1);
                else buf[i] = 0xFF;
            }
            for (int i = 0; i < 4; i++)
            {
                keybuf[i] = (byte)key1;
                key1 = key1 >> 8;
            }
            for (int i = 4; i < 8; i++)
            {
                keybuf[i] = (byte)key2;
                key2 = key2 >> 8;
            }
            acc = 0;
            for (int i = 0; i < keybuf.Length; i++)
            {
                acc = (byte)((int)acc + (int)keybuf[i]);
            }
            for (int i = 0; i < buflen; i++)
            {
                buf[i] = (byte)((int)buf[i] ^ (int)acc);
            }
        }

        /// <summary>
        /// 将byte数组转换为字符串
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private string ByteArrayToString(byte[] buffer)
        {
            Encoding enc = Encoding.Default;
            return enc.GetString(buffer);
        }

        /// <summary>
        /// 数据库信息标志
        /// </summary>
        private enum DBInfoFlag
        {
            /// <summary>
            /// 主用数据库
            /// </summary>
            Primary = 1,
            /// <summary>
            /// 备用数据库
            /// </summary>
            Secondary = 2,
            /// <summary>
            /// 数据库同步
            /// </summary>
            Syschronic = 3,
            /// <summary>
            /// 自动导出信息
            /// </summary>
            AutoExport = 4,
        }

        #endregion
    }
}
