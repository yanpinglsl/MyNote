using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using BasicDataInterface;
using CSharpDBPlugin;
using DBInterfaceCommonLib;
using EMSCommonLib;

namespace OfficeReportInterface
{
    /// <summary>
    ///定时记录基础类，进行数据库查询或者缓存查询等操作
    /// </summary>
    public class HistoryDataLogBasic
    {
        /// <summary>
        /// 唯一实例
        /// </summary>
        public static readonly HistoryDataLogBasic DataManager = new HistoryDataLogBasic();

        /// <summary>
        /// 定时记录根目录名称
        /// </summary>
        public const string DIR_DATALOG = "DataLog";        

        /// <summary>
        /// 事件二进制文件版本号
        /// </summary>
        private static readonly int FileVersion = 3601;

        /// <summary>
        /// 临时文件有效期，以分钟为单位
        /// </summary>
        private static readonly int UsefulLife = 60;

        /// <summary>
        /// 正常查询文件夹名称
        /// </summary>
        private const string DIR_NORMALDATA = "NormalData";

        /// <summary>
        /// 定时记录查询条件，用于文件写入
        /// </summary>
        private DataLogQueryCondition queryConditForWrite;

        /// <summary>
        /// 用于写入的表数据
        /// </summary>
        private DataTable dataTableForWrite;

        /// <summary>
        /// 磁盘剩余空间百分比
        /// </summary>
        private static double DriveFreeSpacePercent = -1;
        /// <summary>
        /// 磁盘空间百分比基于此值，警告
        /// </summary>
        private static double DriveFreeSpacePercentAlert = 10;
        /// <summary>
        /// 磁盘空间百分比基于此值，禁止写入缓存文件
        /// </summary>
        private static double DriverFreeSpacePercentForbid = 5;
              

        ///// <summary>
        ///// 从数据库或者缓存文件读取定时记录
        ///// </summary>
        ///// <param name="stationID">厂站ID</param>
        ///// <param name="sourceID">数据源ID</param>
        ///// <param name="dataIndex">定时记录列索引</param>
        ///// <param name="timeParam">时间段</param>    
        ///// <param name="interval">时间间隔，1-5日周月季年</param>
        ///// <returns></returns>
        //public DataTable ReadDataLogs(DataIDToMeasIDDef dataIDParam, DateTimeParam timeParam)
        //{
        //    DataTable resultDt = new DataTable();            

        //    //每个循环只对一个文件或者一个月的数据进行操作
        //    List<DateTimeParam> timeParamList = GetQueryTimeParamList(timeParam);    
        //    foreach (DateTimeParam timeItem in timeParamList)
        //    {
        //        ReadDatalogsByMonth(dataIDParam, timeItem, ref resultDt);
        //    }   

        //    return resultDt;
        //}

        /// <summary>
        /// 合并DataTable的结构和数据
        /// （排除了大小写不一致造成的影响）
        /// </summary>
        /// <param name="oriTable">初始表</param>
        /// <param name="mergedTable">被合并的表</param>
        /// <returns></returns>
        public DataTable GetMergeDataTable(DataTable oriTable, DataTable mergedTable)
        {          
            if (oriTable.Columns.Count == 0)
                return mergedTable;
            if (mergedTable.Columns.Count == 0)
                return oriTable;

            List<string> mergedColums = new List<string>();
            foreach (DataColumn dataColum in mergedTable.Columns)            
                mergedColums.Add(dataColum.ColumnName.ToUpper());

            foreach (DataColumn dataColum in oriTable.Columns)
            {
                if (mergedColums.Contains(dataColum.ColumnName.ToUpper()))
                    mergedTable.Columns[dataColum.ColumnName].ColumnName = dataColum.ColumnName;
            }

            oriTable.Merge(mergedTable);
            return oriTable;
        }

        //private void ReadDatalogsByMonth(DataIDToMeasIDDef dataIDParam, DateTimeParam timeItem, ref DataTable resultDt)
        //{
        //    DataLogQueryCondition queryCondit = new DataLogQueryCondition(dataIDParam.StationID, dataIDParam.DeviceID, dataIDParam.SourceID, dataIDParam.DataIndex);
        //    queryCondit.timeParam = timeItem;
        //    queryCondit.fileBasePath = GetNormalDataDirPath();

        //    //先读缓存文件
        //    DataTable fileDt = new DataTable();
        //    ReadFileMsg readFileMsg = ReadFile(queryCondit, ref fileDt);
        //    resultDt = GetMergeDataTable(resultDt, fileDt);           

        //    //如果文件中读取到所有的数据，那么不再从数据库中查找
        //    if (readFileMsg.isSuccess && readFileMsg.endTime == timeItem.EndTime)
        //        return;

        //    //从数据库中找到剩余的定时记录数据
        //    DataTable dbDt = ReadDataLogByCompareWithFile(dataIDParam, timeItem, fileDt, readFileMsg);
        //    resultDt = GetMergeDataTable(resultDt, dbDt);            

        //    //检查磁盘空间，如果磁盘空间不够则不写入缓存文件
        //    if (CheckDriveFreeSpaceForbid())
        //    {
        //        //开一个线程将未写入文件的数据进行缓存
        //        //WriteFile(queryCondit);
        //        this.queryConditForWrite = queryCondit;
        //        Thread thWrite = new Thread(new ThreadStart(WriteFileThread));
        //        thWrite.Start();
        //    }
        //}

        /// <summary>
        /// 检查磁盘空间是否禁止再写入数据，True-允许写入，False-禁止写入
        /// </summary>
        /// <returns></returns>
        public bool CheckDriveFreeSpaceForbid()
        {            
            if(DriveFreeSpacePercent<0)
                GetDriveFreePercent();

            if (DriveFreeSpacePercent < DriverFreeSpacePercentForbid)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 查看磁盘剩余空间百分比
        /// </summary>
        /// <returns></returns>
        private double GetDriveFreePercent()
        {
            double resultValue = 0;            
            DriveInfo driver = GetDriveInfo(AppDomain.CurrentDomain.BaseDirectory);
            if (driver == null)
                return resultValue;

            if (driver.TotalSize>0)
                resultValue = (double)driver.AvailableFreeSpace / (double)driver.TotalSize * 100;
            DriveFreeSpacePercent = resultValue;

            return resultValue;
        }

        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        private DriveInfo GetDriveInfo(string dirPath)
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (!drive.IsReady)
                    continue;
                if (drive.RootDirectory.Name == dir.Root.Name)
                    return drive;
            }
            return null;
        }


        /// <summary>
        /// 获取用于查询的时间组合，按月添加相关元素
        /// </summary>
        /// <param name="timeParam"></param>
        /// <returns></returns>
        public List<DateTimeParam> GetQueryTimeParamList(DateTimeParam timeParam)
        {
            List<DateTimeParam> timeParamList = new List<DateTimeParam>();
            DateTime monthTime = new DateTime(timeParam.StartTime.Year, timeParam.StartTime.Month, 1);
            while (monthTime<timeParam.EndTime)
            {
                DateTime startTime;
                if (monthTime.Year == timeParam.StartTime.Year && monthTime.Month == timeParam.StartTime.Month)
                    startTime = timeParam.StartTime;
                else
                    startTime = monthTime;

                DateTime endTime;
                if (monthTime.Year == timeParam.EndTime.Year && monthTime.Month == timeParam.EndTime.Month)
                    endTime = timeParam.EndTime;
                else
                    endTime = monthTime.AddMonths(1);

                DateTimeParam timeItem = new DateTimeParam(startTime, endTime);
                timeParamList.Add(timeItem);
                monthTime = monthTime.AddMonths(1);
            }
            return timeParamList;
        }

        ///// <summary>
        ///// 根据与从文件获取的数据对比，从数据库中获取剩余数据
        ///// </summary>
        ///// <param name="dataIDParam">参数集合</param>
        ///// <param name="timeItem">查询起始时间和结束时间</param>
        ///// <param name="fileDt">从文件中获取的数据集</param>
        ///// <param name="readFileMsg">读取文件的信息</param>
        ///// <returns></returns>
        //private DataTable ReadDataLogByCompareWithFile(DataIDToMeasIDDef dataIDParam, DateTimeParam timeItem, DataTable fileDt, ReadFileMsg readFileMsg)
        //{
        //    //从数据库中获取未读到的数据
        //    DateTimeParam queryDBTime = new DateTimeParam(DateTime.MinValue, DateTime.MinValue);
        //    if (!readFileMsg.isSuccess)
        //        queryDBTime = timeItem;

        //    if (readFileMsg.isSuccess && readFileMsg.endTime < timeItem.EndTime)
        //    {
        //        if (timeItem.StartTime <= readFileMsg.endTime)
        //            queryDBTime = new DateTimeParam(readFileMsg.endTime, timeItem.EndTime);
        //        else
        //            queryDBTime = timeItem;
        //    }

        //    DataTable dbDt = ReadDataLogFromDB(dataIDParam, queryDBTime);

        //    //找到最后一条记录的时间
        //    DateTime endRowTime = GetEndRowTime(fileDt);

        //    //找到所有重复的索引
        //    List<int> removeIndexes = GetRemoveIndexList(dbDt, endRowTime);

        //    //从后往前删
        //    foreach (int rowIndex in removeIndexes)
        //        dbDt.Rows.RemoveAt(removeIndexes.Count - rowIndex - 1);
        //    return dbDt;
        //}

        /// <summary>
        /// 获取需要移除的行的索引列表
        /// </summary>
        /// <param name="dbDt"></param>
        /// <param name="endRowTime"></param>
        /// <returns></returns>
        private List<int> GetRemoveIndexList(DataTable dbDt, DateTime endRowTime)
        {
            List<int> removeIndexes = new List<int>();
            for (int i = 0; i < dbDt.Rows.Count; i++)
            {
                DateTime rowTime = Convert.ToDateTime(dbDt.Rows[i]["LogTime"]);
                rowTime = rowTime.AddMilliseconds(Convert.ToInt32(dbDt.Rows[i]["Msec"]));
                if (rowTime > endRowTime)
                    break;
                removeIndexes.Add(i);
            }
            return removeIndexes;
        }

        /// <summary>
        /// 获取数据集最后一条记录时间（包括毫秒数）
        /// </summary>
        /// <param name="dataResult"></param>
        /// <returns></returns>
        private DateTime GetEndRowTime(DataTable dataResult)
        {
            int fileRowCount = dataResult.Rows.Count;
            DateTime endRowTime = DateTime.MinValue;
            if (fileRowCount > 0)
            {
                endRowTime = Convert.ToDateTime(dataResult.Rows[fileRowCount - 1]["LogTime"]);
                endRowTime = endRowTime.AddMilliseconds(Convert.ToInt32(dataResult.Rows[fileRowCount - 1]["Msec"]));
            }
            return endRowTime;
        }

        ///// <summary>
        ///// 从数据库中读取定时记录值
        ///// </summary>
        ///// <param name="dataIDParam"></param>
        ///// <param name="timeParam"></param>
        ///// <returns></returns>
        //private DataTable ReadDataLogFromDB(DataIDToMeasIDDef dataIDParam, DateTimeParam timeParam)
        //{
        //    DataTable dbDt = new DataTable();
        //    int errorCode = DatalogProvider.Instance.ReadDatalogs(DBOperationFlag.either, dataIDParam.StationID, dataIDParam.SourceID, dataIDParam.DataIndex, timeParam.StartTime, timeParam.EndTime, Convert.ToInt32(SysConstDef.DefaultMaxRowCount), ref dbDt);
        //    if (errorCode != (int)ErrorCode.Success)            
        //        ErrorInfoManager.Instance.WriteDBInterfaceLog(errorCode, "DatalogProvider.ReadDatalogs");
        //    return dbDt;
        //}

        private struct ReadFileMsg
        {
            /// <summary>
            /// 是否读取正确
            /// </summary>
            public bool isSuccess;
            /// <summary>
            /// 读到记录的结束时间
            /// </summary>
            public DateTime endTime;

            public ReadFileMsg(bool success)
            {
                this.isSuccess = success;
                this.endTime = DateTime.MinValue;
            }
        }

        /// <summary>
        /// 获取正常数据目录全路径
        /// </summary>
        /// <returns></returns>
        private string GetNormalDataDirPath()
        {
            return Path.Combine(SysConstDef.GetProjectFilePath(), DIR_DATALOG, DIR_NORMALDATA);
        }

        /// <summary>
        /// TODO 测试
        /// 获取定时记录
        /// </summary>
        /// <param name="datalogNode"></param>
        /// <param name="timeParam"></param>
        /// <param name="pointInterval"></param>
        /// <returns></returns>
        public List<DataLogOriDef> ReadDataLogFromDB2(DATALOG_PRIVATE_MAP datalogNode, DateTimeParam timeParam, int pointInterval)
        {
            List<DataLogOriDef> result = new List<DataLogOriDef>();
            string dataParamStr = string.Format("{0},{1},{2},{3},{4}", datalogNode.deviceID, datalogNode.dataID, datalogNode.dataTypeID, datalogNode.logicalDeviceIndex, datalogNode.paraType);
            string timeParamStr = string.Format("{0},{1}", timeParam.StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), timeParam.EndTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            var datalogResult = BasicDataProvider.GetInstance().GetOriDataLogList(dataParamStr, 0, pointInterval, string.Empty, timeParamStr);
            if (!datalogResult.Success)
            {
                ErrorInfoManager.Instance.WriteLogMessage("ReadDataLogFromDB2", datalogResult.ErrorMessage);
                return result;
            }
            if (datalogResult.ResultList.Count > 0)
            {
                foreach (var item in datalogResult.ResultList[0].DataList)
                {
                    result.Add(new DataLogOriDef(DatetimeConvert.ConvertDoubleToDate(item.LogTime), item.DataValue));
                }
            }
            return result;
        }


        ///// <summary>
        ///// 根据任意时间段查询定时记录，不保存缓存文件
        ///// </summary>
        ///// <param name="stationID">厂站ID</param>
        ///// <param name="sourceID">数据源ID</param>
        ///// <param name="dataIndex">数据列索引</param>
        ///// <param name="timeParam">起始时间和结束时间</param>
        ///// <returns>定时记录结果集</returns>
        //public DataTable QueryDataLogByDuration(uint stationID, uint sourceID, int dataIndex, DateTimeParam timeParam)
        //{
        //    DataTable resultDt = new DataTable();
        //    int errorCode = DatalogProvider.Instance.ReadDatalogs(DBOperationFlag.either, stationID, sourceID, dataIndex, timeParam.StartTime, timeParam.EndTime, Convert.ToInt32(SysConstDef.DefaultMaxRowCount), ref resultDt);
        //    if (errorCode != (int)ErrorCode.Success)            
        //        ErrorInfoManager.Instance.WriteDBInterfaceLog(errorCode,"DatalogProvider.ReadDatalogs");

        //    return resultDt;
        //}

        /// <summary>
        /// 读二进制文件
        /// </summary>
        /// <param name="queryCondit">查询条件</param>
        /// <param name="queryResult">查询结果集</param>
        /// <returns>返回查询结果状态和最后一条记录的时间</returns>
        private ReadFileMsg ReadFile(DataLogQueryCondition queryCondit, ref DataTable queryResult)
        {
            ReadFileMsg resultMsg = new ReadFileMsg(false);

            string filePath = GetBufferFilePath(queryCondit);
            if (!File.Exists(filePath))
                return resultMsg;
           
            FileStream fstream = null;
            BinaryReader reader = null;
            try
            {
                fstream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                reader = new BinaryReader(fstream);
                //读取文件头，与当前查询条件进行对比
                DataLogFileHead fileHead = new DataLogFileHead();
                fileHead.ReadFile(reader);
                bool matchedHead = MatchHead(queryCondit, fileHead);
                if (!matchedHead)
                    return resultMsg;

                if (fileHead.fileState == (int)BufferFileState.Invalid)
                    resultMsg.isSuccess = false;
                else if (fileHead.fileState == (int)BufferFileState.Stable)
                {
                    resultMsg.isSuccess = ReadDataLogFromFile(reader, queryCondit, ref queryResult);
                    resultMsg.endTime = queryCondit.timeParam.EndTime;
                }
                else if (fileHead.fileState == (int)BufferFileState.Temporary)
                    resultMsg = ReadDataLogFromTempFile(reader, queryCondit, fileHead, ref queryResult);
            }
            catch (Exception ex)
            {
                resultMsg.isSuccess = false;
                ErrorInfoManager.Instance.WriteLogMessage("HistoryDataLogBasic.ReadFile", ex);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (fstream != null)
                    fstream.Close();
            }         

            return resultMsg;
        }

        /// <summary>
        /// 从临时文件中读取定时记录
        /// </summary>
        /// <param name="queryCondit"></param>
        /// <param name="fileHead"></param>
        /// <param name="reader"></param>
        /// <param name="queryResult"></param>
        /// <returns></returns>
        private ReadFileMsg ReadDataLogFromTempFile(BinaryReader reader,  DataLogQueryCondition queryCondit, DataLogFileHead fileHead, ref DataTable queryResult)
        {
            ReadFileMsg resultMsg = new ReadFileMsg(false);
            if (queryCondit.timeParam.EndTime <= fileHead.timeParam.EndTime)
            {
                //查询时段在文件已有数据的时段内
                resultMsg.isSuccess = ReadDataLogFromFile(reader, queryCondit, ref queryResult);
                resultMsg.endTime = queryCondit.timeParam.EndTime;
            }
            else
            {
                //查询结束时间比文件最后一条记录时间大，说明可能还有数据需要从数据库查询
                //这时返回文件结束时间，用于之后作为查询数据库的起始时间
                DataLogQueryCondition newQueryCondit = queryCondit;
                newQueryCondit.timeParam.EndTime = fileHead.timeParam.EndTime.AddDays(1).Date;
                resultMsg.isSuccess = ReadDataLogFromFile(reader, newQueryCondit, ref queryResult);
                resultMsg.endTime = fileHead.timeParam.EndTime;
            }
            return resultMsg;
        }

        /// <summary>
        /// 获取缓存文件路径
        /// </summary>
        /// <param name="queryCondit">查询条件</param>
        /// <param name="fileBasePath">文件基础路径</param>
        /// <param name="interval">1-5日周月季年</param>
        /// <returns></returns>
        private string GetBufferFilePath(DataLogQueryCondition queryCondit)
        {
            string dirRelativePath = string.Format(@"\{0}\{1}\{2}\", queryCondit.deviceID, queryCondit.timeParam.StartTime.Year, queryCondit.timeParam.StartTime.Month);
            string dirPath = queryCondit.fileBasePath + dirRelativePath;
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            string fileName = string.Format("{0}.dat", queryCondit.sourceID);
            string filePath = Path.Combine(dirPath, fileName);
            return filePath;            
        }

        /// <summary>
        /// 对比查询信息是否与定时记录文件头信息吻合
        /// </summary>
        /// <param name="queryCondit">定时记录查询信息</param>
        /// <param name="laterHead">定时记录头部信息</param>
        /// <returns></returns>
        private bool MatchHead(DataLogQueryCondition queryCondit, DataLogFileHead fileHead)
        {
            if (queryCondit.sourceID != fileHead.sourceID)
                return false;
            if (queryCondit.timeParam.StartTime < fileHead.timeParam.StartTime)
                return false;

            DateTime monthEndTime = new DateTime(queryCondit.timeParam.StartTime.Year, queryCondit.timeParam.StartTime.Month, 1).AddMonths(1);
            if (queryCondit.timeParam.EndTime > monthEndTime)
                return false;
            if (queryCondit.timeParam.StartTime > queryCondit.timeParam.EndTime)
                return false;
            return true;
        }

        ///// <summary>
        ///// 写入定时记录文件，用于线程写入
        ///// </summary>
        //private void WriteFileThread()
        //{
        //    WriteFile(this.queryConditForWrite);
        //}

        ///// <summary>
        ///// 写二进制文件
        ///// </summary>
        ///// <param name="queryResult">定时记录查询结果集</param>
        ///// <param name="queryCondition">查询条件</param>
        ///// <returns></returns>
        //private EMSErrorMsg WriteFile(DataLogQueryCondition queryCondit)
        //{
        //    EMSErrorMsg resultMsg = new EMSErrorMsg(true);

        //    string filePath = GetBufferFilePath(queryCondit);
        //    bool exists = File.Exists(filePath);    

        //    if (!exists)                
        //        resultMsg.Success = CreateAndWriteNewFile(queryCondit, filePath);                
        //    else
        //        resultMsg.Success = UpdateDataLogFile(queryCondit, filePath);
            
        //    return resultMsg;
        //}

        ///// <summary>
        ///// 更新定时记录文件
        ///// </summary>
        ///// <param name="queryCondit"></param>
        ///// <param name="filePath"></param>
        ///// <returns></returns>
        //private bool UpdateDataLogFile(DataLogQueryCondition queryCondit, string filePath)
        //{
        //    bool result = true;
        //    //获取文件头部信息和索引信息
        //    List<long> dataIndexes;
        //    DataLogFileHead oriHead = ReadDataLogFileHead(filePath, out dataIndexes);

        //    //如果版本号不正确或者文件状态无效，那么重建文件
        //    if (oriHead.version != FileVersion || oriHead.fileState == (short)BufferFileState.Invalid)
        //    {
        //        result = CreateAndWriteNewFile(queryCondit, filePath);
        //        return result;
        //    }

        //    //如果文件状态已经为稳定，那么直接返回，不修改文件
        //    if (oriHead.fileState == (short)BufferFileState.Stable)
        //        return true;

        //    //如果当前POI时间与文件中的POI时间一致，那么说明数据没有更新，所以不修改文件
        //    DateTime newPoiTime = GetDataLogUpdateTime(queryCondit.sourceID);
        //    if (oriHead.fileState == (short)BufferFileState.Temporary && oriHead.updateTime == newPoiTime)
        //        return true;

        //    //从数据库查找未缓存的数据
        //    DataTable queryResult = GetNewDataLogFromDB(queryCondit, oriHead);

        //    //更新头部信息以及定时记录数据
        //    result = UpdateDataLogAndHead(filePath, dataIndexes, oriHead, newPoiTime, queryResult);

        //    return result;
        //}

        ///// <summary>
        ///// 更新头部信息以及定时记录数据
        ///// </summary>
        ///// <param name="filePath">文件路径</param>
        ///// <param name="dataIndexes">索引列表</param>
        ///// <param name="oriHead">读取到的文件头信息</param>
        ///// <param name="newPoiTime">最新POI时标</param>
        ///// <param name="queryResult">需要缓存的数据集</param>
        ///// <returns></returns>
        //private bool UpdateDataLogAndHead(string filePath, List<long> dataIndexes, DataLogFileHead oriHead, DateTime newPoiTime, DataTable queryResult)
        //{
        //    bool result = true;
            
        //    FileStream fstream = null;
        //    BinaryWriter writer = null;
        //    try
        //    {
        //        fstream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
        //        writer = new BinaryWriter(fstream);
        //        //更新头部信息              
        //        DataLogFileHead newHead = new DataLogFileHead();
        //        newHead.version = FileVersion;
        //        newHead.sourceID = oriHead.sourceID;
        //        newHead.fileTime = DateTime.Now;
        //        newHead.timeParam = GetNewTimeParam(oriHead, newPoiTime, queryResult);
        //        newHead.updateTime = newPoiTime;
        //        newHead.fileState = GetFileState(oriHead.timeParam.StartTime, newPoiTime);
        //        newHead.WriteFile(writer);

        //        //更新数据内容
        //        DateTime startDate = oriHead.timeParam.EndTime.Date;
        //        long startPosition = dataIndexes[startDate.Day - 1];
        //        UpdateDataLog(writer, startDate, queryResult, startPosition);
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        ErrorInfoManager.Instance.WriteLogMessage("UpdateDataLogAndHead", ex);
        //    }
        //    finally
        //    {
        //        if(writer!=null)
        //            writer.Close();
        //        if(fstream!=null)
        //            fstream.Close();
        //    }
           
        //    return result;
        //}

        ///// <summary>
        ///// 读文件头信息，并返回索引信息列表
        ///// </summary>
        ///// <param name="filePath">文件路径</param>
        ///// <param name="dataIndexes">索引信息列表</param>
        ///// <returns></returns>
        //private DataLogFileHead ReadDataLogFileHead(string filePath, out List<long> dataIndexes)
        //{
        //    DataLogFileHead oriHead = new DataLogFileHead();
        //    dataIndexes = new List<long>();

        //    FileStream fstream = null;
        //    BinaryReader reader = null;              
        //    try
        //    {
        //        //先读取文件头
        //        fstream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        //        reader = new BinaryReader(fstream);
        //        oriHead = new DataLogFileHead();
        //        oriHead.ReadFile(reader);

        //        //读索引                
        //        int totalDays = reader.ReadInt32();
        //        for (int i = 0; i < totalDays; i++)
        //            dataIndexes.Add(reader.ReadInt64());               
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorInfoManager.Instance.WriteLogMessage("ReadDataLogFileHead", ex);
        //    }
        //    finally
        //    {
        //        if (reader != null)
        //            reader.Close();
        //        if (fstream != null)
        //            fstream.Close();
        //    }

        //    return oriHead;
        //}

        ///// <summary>
        ///// 从数据库中查找未缓存的数据
        ///// </summary>
        ///// <param name="queryCondit"></param>
        ///// <param name="oriHead"></param>
        ///// <returns></returns>
        //private DataTable GetNewDataLogFromDB(DataLogQueryCondition queryCondit, DataLogFileHead oriHead)
        //{
        //    DataIDToMeasIDDef dataIDParams = new DataIDToMeasIDDef(queryCondit.stationID, queryCondit.deviceID, queryCondit.sourceID, 0);
        //    DateTime stime = oriHead.timeParam.EndTime.Date;//格式化为日期格式         
        //    DateTime etime = new DateTime(stime.Year, stime.Month, 1).AddMonths(1);//结束时间格式化为下月1日
        //    DateTimeParam timeParam = new DateTimeParam(stime, etime);
        //    DataTable queryResult = ReadDataLogFromDB(dataIDParams, timeParam);           
        //    return queryResult;
        //}

        ///// <summary>
        ///// 获取新的缓存文件数据时间段信息
        ///// </summary>
        ///// <param name="oriHead"></param>
        ///// <param name="newPoiTime"></param>
        ///// <param name="queryResult"></param>
        ///// <returns></returns>
        //private DateTimeParam GetNewTimeParam(DataLogFileHead oriHead, DateTime newPoiTime, DataTable queryResult)
        //{
        //    DateTime stimeFile = oriHead.timeParam.StartTime;
        //    DateTime etimeFile = oriHead.timeParam.EndTime;
        //    short fileState = GetFileState(stimeFile, newPoiTime);
        //    if (fileState == (short)BufferFileState.Stable)
        //        etimeFile = stimeFile.AddMonths(1);
        //    else if (fileState == (short)BufferFileState.Temporary)
        //    {
        //        DateTime recordTime = GetEndRowTime(queryResult);
        //        if (recordTime != DateTime.MinValue)
        //            etimeFile = recordTime;
        //    }
        //    DateTimeParam newTimeParam = new DateTimeParam(stimeFile, etimeFile);
        //    return newTimeParam;
        //}

        ///// <summary>
        ///// 创建新的缓存文件，并写入缓存数据
        ///// </summary>
        ///// <param name="queryCondit"></param>
        ///// <param name="filePath"></param>
        ///// <returns></returns>
        //private bool CreateAndWriteNewFile(DataLogQueryCondition queryCondit, string filePath)
        //{
        //    bool result = true;          
                        
        //    //查找一个月时段的数据
        //    DataIDToMeasIDDef dataIDParams = new DataIDToMeasIDDef(queryCondit.stationID, queryCondit.deviceID, queryCondit.sourceID, 0);
        //    DateTime stime = new DateTime(queryCondit.timeParam.StartTime.Year, queryCondit.timeParam.StartTime.Month, 1);
        //    DateTimeParam timeParam = new DateTimeParam(stime, stime.AddMonths(1));
        //    DataTable queryResult = ReadDataLogFromDB(dataIDParams, timeParam);
                        
        //    FileStream fstream = null;
        //    BinaryWriter writer = null;          
        //    try
        //    {
        //        fstream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
        //        writer = new BinaryWriter(fstream);
        //        WriteFileHead(writer, queryCondit, queryResult);
        //        WriteDataLog(writer, stime, queryResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        ErrorInfoManager.Instance.WriteLogMessage("CreateAndWriteNewFile", ex);
        //    }
        //    finally
        //    {
        //        if(writer!=null)
        //            writer.Close();
        //        if(fstream!=null)
        //            fstream.Close();
        //    }           
        
        //    return result;
        //}

        ///// <summary>
        ///// 写文件头
        ///// </summary>
        ///// <param name="writer"></param>
        ///// <param name="queryCondit"></param>
        //private void WriteFileHead(BinaryWriter writer, DataLogQueryCondition queryCondit, DataTable queryResult)
        //{
        //    DateTime stime = new DateTime(queryCondit.timeParam.StartTime.Year,queryCondit.timeParam.StartTime.Month,1);
        //    DateTime etime = stime;
        //    DateTime updateTime = GetDataLogUpdateTime(queryCondit.sourceID);
        //    short fileState = GetFileState(new DateTimeParam(stime, stime.AddMonths(1)), updateTime);
        //    if (fileState == (short)BufferFileState.Stable)
        //        etime = stime.AddMonths(1);
        //    else if (fileState == (short)BufferFileState.Temporary)
        //        etime = GetEndRowTime(queryResult);

        //    DateTimeParam timeParam = new DateTimeParam(stime,etime);
        //    DataLogFileHead headInfo = new DataLogFileHead(queryCondit.sourceID, timeParam);            
        //    headInfo.updateTime = updateTime;
        //    headInfo.fileState = fileState;            
        //    headInfo.WriteFile(writer);           
        //}

        ///// <summary>
        ///// 写定时记录到二进制流
        ///// </summary>
        ///// <returns></returns>
        //private void WriteDataLog(BinaryWriter writer, DateTime startDate, DataTable queryResult)
        //{
        //    //初始化结果集结构
        //    List<DataLogListParam> dataParamList = InitializeDataParamList(startDate);

        //    //重组查询结果            
        //    RebuildDataResult(startDate, queryResult, ref dataParamList);

        //    //写入索引的初始值
        //    WriteInitialIndex(writer, startDate);

        //    //将所有有数据的数据集顺序写到缓存文件中
        //    List<long> positionList = WriteTotalValueList(writer, dataParamList);

        //    //改写索引位置
        //    UpdateIndexPosition(writer, startDate, positionList);

        //}

        ///// <summary>
        ///// 更新定时记录缓存
        ///// </summary>
        ///// <param name="writer">二进制写入</param>
        ///// <param name="startDate">起始日期</param>
        ///// <param name="queryResult">数据库查询结果集</param>
        ///// <param name="startPositon">要修改的起始位置</param>
        //private void UpdateDataLog(BinaryWriter writer, DateTime startDate, DataTable queryResult, long startPositon)
        //{
        //    //初始化结果集结构
        //    List<DataLogListParam> dataParamList = InitializeDataParamList(startDate);

        //    //重组查询结果            
        //    RebuildDataResult(startDate, queryResult, ref dataParamList);

        //    //将二进制流位置置于要修改的位置，进行二进制文件修改
        //    writer.BaseStream.Position = startPositon;
        //    List<long> positionList = WriteTotalValueList(writer, dataParamList);

        //    //改写索引位置
        //    UpdateIndexPosition(writer, startDate, positionList);
        //}

        ///// <summary>
        ///// 改写索引位置信息
        ///// </summary>
        ///// <param name="writer"></param>
        ///// <param name="startDate"></param>
        ///// <param name="positionList"></param>
        //private void UpdateIndexPosition(BinaryWriter writer, DateTime startDate, List<long> positionList)
        //{
        //    writer.BaseStream.Position = DataLogFileHead.EndPosition + 4 + (startDate.Day - 1) * 8;
        //    foreach (long pos in positionList)
        //        writer.Write(pos);
        //}

        ///// <summary>
        ///// 将所有有数据的数据集顺序写到缓存文件中
        ///// 返回数据位置索引信息
        ///// </summary>
        ///// <param name="writer"></param>
        ///// <param name="dataParamList"></param>
        ///// <returns></returns>
        //private List<long> WriteTotalValueList(BinaryWriter writer, List<DataLogListParam> dataParamList)
        //{
        //    //从后往前找到有数据的截止索引
        //    int valueIndex = FindEndDataValueIndex(dataParamList);

        //    List<long> positionList = new List<long>();
        //    //将所有有数据的数据集顺序写到缓存文件中
        //    for (int i = 0; i <= valueIndex; i++)
        //    {
        //        positionList.Add(writer.BaseStream.Position);
        //        DataLogListParam paramItem = dataParamList[i];
        //        //将定时记录时间写入到缓存文件中
        //        writer.Write(paramItem.logtimeList.Count);
        //        foreach (DateTime logtime in paramItem.logtimeList)
        //            writer.Write(logtime.Ticks);

        //        //将所有列的定时记录值循环写入缓存文件中
        //        for (int colIndex =1; colIndex<=16; colIndex++)
        //        {
        //            List<double> valueList;
        //            paramItem.colIndexValueMap.TryGetValue(colIndex, out valueList);
        //            if (valueList == null)
        //                valueList = new List<double>();
        //            writer.Write(valueList.Count);
        //            foreach (double value in valueList)
        //                writer.Write(value);
        //        }
        //    }
        //    return positionList;
        //}

        ///// <summary>
        ///// 从后往前找到有数据的截止索引
        ///// </summary>
        ///// <param name="dataParamList"></param>
        ///// <returns></returns>
        //private int FindEndDataValueIndex(List<DataLogListParam> dataParamList)
        //{
        //    int valueIndex = dataParamList.Count - 1;
        //    for (int i = dataParamList.Count - 1; i > 0; i--)
        //    {
        //        if (dataParamList[i].logtimeList.Count > 0)
        //        {
        //            valueIndex = i;
        //            break;
        //        }
        //    }
        //    return valueIndex;
        //}

        ///// <summary>
        ///// 写入索引的初始值（默认为0）
        ///// </summary>
        ///// <param name="writer"></param>
        ///// <param name="startDate"></param>
        //private void WriteInitialIndex(BinaryWriter writer, DateTime startDate)
        //{
        //    int totalDays = GetTotalDaysOfMonth(startDate);
        //    //写入当月的总天数
        //    writer.Write(totalDays);
        //    //写各天数据缓存起始位置，先初始化为0            
        //    for (int i = 0; i < totalDays; i++)
        //        writer.Write((long)0);
        //}

        /// <summary>
        ///// 重新组建结果集到目标结构中
        ///// </summary>
        ///// <param name="startDate">查询起始日期</param>
        ///// <param name="queryResult">查询结果集</param>
        ///// <param name="dataParamList">目标结构集</param>
        //private void RebuildDataResult(DateTime startDate, DataTable queryResult, ref List<DataLogListParam> dataParamList)
        //{
        //    //List<int> colIndexes = GetColumIndexesFromDataTable(queryResult);
        //    foreach (DataRow dr in queryResult.Rows)
        //    {
        //        DateTime logtime = Convert.ToDateTime(dr["LogTime"]);
        //        int msec = Convert.ToInt32(dr["Msec"]);
        //        logtime = logtime.AddMilliseconds(msec);
        //        dataParamList[logtime.Day - startDate.Day].logtimeList.Add(logtime);

        //        for (int colIndex=1;colIndex<=16;colIndex++)
        //        {                   
        //            string colName = string.Format("Data{0}", colIndex);
        //            //如果没有这个列则写入空值
        //            if (!queryResult.Columns.Contains(colName))
        //            {
        //                dataParamList[logtime.Day - startDate.Day].colIndexValueMap[colIndex] = new List<double>();
        //                continue;
        //            }

        //            double dataValue = double.NaN;                   
        //            if (!Convert.IsDBNull(dr[colName]))
        //                dataValue = Convert.ToDouble(dr[colName]);
        //            List<double> dataList;
        //            dataParamList[logtime.Day - startDate.Day].colIndexValueMap.TryGetValue(colIndex, out dataList);
        //            if (dataList == null)
        //                dataList = new List<double>();
        //            dataList.Add(dataValue);
        //            dataParamList[logtime.Day - startDate.Day].colIndexValueMap[colIndex] = dataList;
        //        }
        //    }
        //}

 
        ///// <summary>
        ///// 根据查询起始时间初始化结果集结构
        ///// </summary>
        ///// <param name="startDate"></param>
        ///// <returns></returns>
        //private List<DataLogListParam> InitializeDataParamList(DateTime startDate)
        //{
        //    List<DataLogListParam> dataParamList = new List<DataLogListParam>();
        //    int startDay = startDate.Day;
        //    int totalDays = GetTotalDaysOfMonth(startDate);
        //    for (int i = startDay - 1; i < totalDays; i++)
        //    {
        //        DataLogListParam paramItem = new DataLogListParam();
        //        paramItem.logtimeList = new List<DateTime>();
        //        paramItem.colIndexValueMap = new Dictionary<int, List<double>>();
        //        dataParamList.Add(paramItem);
        //    }
        //    return dataParamList;
        //}      

        /// <summary>
        /// 从二进制流读取定时记录
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="queryCondit">查询条件</param>
        /// <param name="queryResult">查询结果集</param>
        private bool ReadDataLogFromFile(BinaryReader reader, DataLogQueryCondition queryCondit, ref DataTable resultDt)
        {
            bool result = true;
            try
            {
                //从二进制文件读取定时记录到目标结果集中
                DataLogArrayParam dataLogArray = ReadDataLogArray(reader, queryCondit);

                //构造缓存表结构
                ConstructDataLogTable(queryCondit.colIndex, resultDt);

                //添加定时记录到缓存表结构中
                AddDataLogToDataTable(queryCondit, dataLogArray, ref resultDt);
            }
            catch(Exception ex)
            {
                ErrorInfoManager.Instance.WriteLogMessage("ReadDataLogFromFile", ex);
            }

            return result;
        }

  
        /// <summary>
        /// 添加定时记录到缓存表结构中
        /// </summary>
        /// <param name="queryCondit"></param>
        /// <param name="dataLogArray"></param>
        /// <param name="resultDt"></param>
        private void AddDataLogToDataTable(DataLogQueryCondition queryCondit, DataLogArrayParam dataLogArray, ref DataTable resultDt)
        {
            //添加到结果集
            for (int i = 0; i < dataLogArray.logTimeList.Count; i++)
            {
                DateTime logTime = dataLogArray.logTimeList[i];
                if (logTime < queryCondit.timeParam.StartTime)
                    continue;
                if (logTime >= queryCondit.timeParam.EndTime)
                    continue;

                DataRow dr = resultDt.NewRow();
                int msec = logTime.Millisecond;
                dr["SourceID"] = queryCondit.sourceID;
                dr["LogTime"] = logTime.AddMilliseconds(-1 * msec);
                dr["Msec"] = logTime.Millisecond;

                if (queryCondit.colIndex == 0)
                {
                    for (int colIndex = 0; colIndex < 16; colIndex++)
                    {
                        string colName = string.Format("Data{0}", colIndex + 1);
                        dr[colName] = dataLogArray.valueArray[colIndex][i];
                    }
                }
                else
                {
                    string colName = string.Format("Data{0}", queryCondit.colIndex);
                    dr[colName] = dataLogArray.valueArray[queryCondit.colIndex - 1][i];
                }

                resultDt.Rows.Add(dr);
            }
        }

        /// <summary>
        /// 组建结果集缓存表结构
        /// </summary>
        /// <param name="colIndex">列索引</param>
        /// <param name="resultDt"></param>
        private void ConstructDataLogTable(int colIndex, DataTable resultDt)
        {
            resultDt.Columns.Add("SourceID", typeof(int));
            resultDt.Columns.Add("LogTime", typeof(DateTime));
            
            resultDt.Columns.Add("Msec", typeof(Int16));
            if (colIndex > 0)
            {
                string colName = string.Format("Data{0}", colIndex);
                resultDt.Columns.Add(colName, typeof(double));
            }
            else
            {
                for (int i = 0; i < 16; i++)
                {
                    string colName = string.Format("Data{0}", i + 1);
                    resultDt.Columns.Add(colName, typeof(double));
                }
            }        
        }

        /// <summary>
        /// 读取定时记录到目标结果集中
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="queryCondit"></param>
        /// <param name="startDay"></param>
        /// <param name="endDay"></param>
        /// <returns></returns>
        private DataLogArrayParam ReadDataLogArray(BinaryReader reader, DataLogQueryCondition queryCondit)
        {
            //判断查询天数
            int startDay = queryCondit.timeParam.StartTime.Day;
            int endDay = GetEndDay(queryCondit);

            //读索引     
            List<long> indexList = ReadValueIndexPositions(reader);

            //读数据
            DataLogArrayParam dataLogArray = new DataLogArrayParam(new List<DateTime>(), new List<double>[16]);
            for (int queryDay = startDay; queryDay <= endDay; queryDay++)
            {
                long valuePos = indexList[queryDay - 1];
                //如果索引位置为0，说明没有数据
                if (valuePos == 0)
                    continue;
                //从查询日所在位置开始读起
                reader.BaseStream.Position = valuePos;

                //获取结果集到相关列表中              
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                    dataLogArray.logTimeList.Add(new DateTime(reader.ReadInt64()));

                if (queryCondit.colIndex == 0)
                {
                    //读取所有的列
                    for (int i = 1; i <= 16; i++)
                        AddColValues(reader, i, ref dataLogArray.valueArray);
                }
                else
                {
                    //跳转到需要查询的列位置
                    for (int i = 1; i < queryCondit.colIndex; i++)
                    {
                        int colCount = reader.ReadInt32();
                        reader.BaseStream.Position = reader.BaseStream.Position  + colCount * 8;                       
                    }
                    AddColValues(reader, queryCondit.colIndex, ref dataLogArray.valueArray);
                }
            }
            return dataLogArray;
        }       

        /// <summary>
        /// 添加一列数据到结果列表中
        /// </summary>
        /// <param name="reader">二进制读取</param>
        /// <param name="valueArray">数据结果列表</param>
        /// <param name="colIndex">列索引</param>
        private void AddColValues(BinaryReader reader, int colIndex, ref List<double>[] valueArray)
        {
            List<double> valueList = ReadColValueList(reader);
            if (valueArray[colIndex - 1] == null)
                valueArray[colIndex - 1] = new List<double>();
            valueArray[colIndex - 1].AddRange(valueList);
        }

        /// <summary>
        /// 读取所有数据的索引位置
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private List<long> ReadValueIndexPositions(BinaryReader reader)
        {
            List<long> indexList = new List<long>();
            int totalDays = reader.ReadInt32();
            for (int i = 0; i < totalDays; i++)
                indexList.Add(reader.ReadInt64());
            return indexList;
        }

        /// <summary>
        /// 获取查找的结束天
        /// </summary>
        /// <param name="queryCondit"></param>
        /// <returns></returns>
        private int GetEndDay(DataLogQueryCondition queryCondit)
        {
            int endDay;
            DateTime etime = queryCondit.timeParam.EndTime;
            if (etime == etime.Date)
                endDay = etime.AddDays(-1).Day;
            else
                endDay = etime.Day;
            return endDay;
        }

        /// <summary>
        /// 读取单列数据
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private List<double> ReadColValueList(BinaryReader reader)
        {
            List<double> valueList = new List<double>();
            int colValueCount = reader.ReadInt32();            
            for (int i = 0; i < colValueCount; i++)
                valueList.Add(reader.ReadDouble());
            return valueList;
        }

        /// <summary>
        /// 获取查询时间所在月份的总天数
        /// </summary>
        /// <param name="recordTime">查询时间</param>
        /// <returns></returns>
        private int GetTotalDaysOfMonth(DateTime recordTime)
        {
            DateTime formatDate = new DateTime(recordTime.Year,recordTime.Month,1);
            return formatDate.AddMonths(1).AddDays(-1).Day;
        }

        ///// <summary>
        ///// 读取临时定时记录缓存文件
        ///// </summary>
        ///// <param name="reader">二进制读取</param>
        ///// <param name="fileHead">已读取的文件头信息</param>
        ///// <param name="colIndex">定时记录列索引</param>
        ///// <param name="updateTime">文件中保存的定时记录更新时标</param>
        ///// <param name="queryResult">查询结果</param>
        ///// <returns>读取文件是否成功</returns>
        //private bool ReadTemporaryDatalogFile(BinaryReader reader, DataLogFileHead fileHead, int colIndex, ref DataTable queryResult)
        //{
        //    bool result = true;

        //    //bool readDataFile = true;
        //    ////文件在有效时间范围内，或者在有效时间范围外但是数据库的时标未更新，都从缓存文件中读取
        //    //if (DateTime.Now > fileHead.fileTime.AddMinutes(UsefulLife))
        //    //{
        //    //    if (fileHead.updateTime != GetDataLogUpdateTime(fileHead.sourceID))
        //    //        readDataFile = false;
        //    //}

        //    //if (readDataFile)
        //    //    result = ReadDataLogFromFile(reader, colIndex, ref queryResult);
        //    //else
        //    //    result = false;
        //    return result;
        //}

        ///// <summary>
        ///// 获取定时记录更新事件
        ///// </summary>
        ///// <param name="headInfo"></param>
        ///// <returns></returns>
        //private DateTime GetDataLogUpdateTime(uint sourceID)
        //{
        //    DateTime updateTime = DateTime.MinValue;
        //    DataTable datalogUpdateDt = new DataTable();
        //    int errorCode = DatalogProvider.Instance.ReadDatalogUpdateTimestamps(DBOperationFlag.either, new uint[] { sourceID }, ref datalogUpdateDt);
        //    if (errorCode != (int)ErrorCode.Success)
        //        ErrorInfoManager.Instance.WriteDBInterfaceLog(errorCode, "DatalogProvider.ReadDatalogUpdateTimestamps");

        //    if (datalogUpdateDt.Rows.Count == 1)
        //    {
        //        updateTime = Convert.ToDateTime(datalogUpdateDt.Rows[0]["UpdateTime"]);
        //        updateTime = updateTime.AddMilliseconds(Convert.ToInt32(datalogUpdateDt.Rows[0]["Msec"]));
        //    }

        //    return updateTime;
        //}

        /// <summary>
        /// 获取文件状态，0-无效，1-稳定，2-临时
        /// </summary>
        /// <param name="timeParam">起始时间-结束时间</param>
        /// <param name="updateTime">定时记录更新时间</param>
        /// <returns>文件状态</returns>
        private short GetFileState(DateTimeParam timeParam, DateTime updateTime)
        {
            short result = 0;            

            if (updateTime < timeParam.StartTime)
                result = (short)BufferFileState.Invalid;

            if (updateTime >= timeParam.StartTime && updateTime < timeParam.EndTime)
                result = (short)BufferFileState.Temporary;

            if (updateTime >= timeParam.EndTime)
                result = (short)BufferFileState.Stable;

            return result;
        }

        /// <summary>
        /// 获取文件状态，根据查询事件所在的月时段作为判断文件状态的依据
        /// </summary>
        /// <param name="oriTime">查询时间</param>
        /// <param name="updateTime">定时记录POI时间</param>
        /// <returns></returns>
        private short GetFileState(DateTime oriTime, DateTime updateTime)
        {            
            DateTime startTime = new DateTime(oriTime.Year, oriTime.Month, 1);
            DateTime endTime = startTime.AddMonths(1);
            DateTimeParam timeParam = new DateTimeParam(startTime, endTime);

            return GetFileState(timeParam,updateTime);
        }

        /// <summary>
        /// 定时记录缓存文件头部信息
        /// </summary>
        private struct DataLogFileHead
        {
            /// <summary>
            /// 文件版本
            /// </summary>
            public int version;
            /// <summary>
            /// 文件创建时间
            /// </summary>
            public DateTime fileTime;
            /// <summary>
            /// 文件状态，0-无效，1-稳定，2-临时
            /// </summary>
            public short fileState;
            /// <summary>
            /// 定时记录POI时标
            /// </summary>
            public DateTime updateTime;
            /// <summary>
            /// 记录源ID
            /// </summary>
            public uint sourceID;
            /// <summary>
            /// 起始时间-结束时间
            /// </summary>
            public DateTimeParam timeParam;
            /// <summary>
            /// 文件头结束位置
            /// </summary>
            public static long EndPosition = 4 + 8 + 2 + 8 + 4 + 8 + 8;
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="sourceID"></param>
            /// <param name="timeParam"></param>
            public DataLogFileHead(uint sourceID, DateTimeParam timeParam)
            {
                this.version = FileVersion;
                this.sourceID = sourceID;
                this.timeParam = timeParam;
                this.fileTime = DateTime.Now;
                this.updateTime = DateTime.MinValue;
                this.fileState = 0;
            }

            /// <summary>
            /// 写二进制文件
            /// </summary>
            /// <param name="writer"></param>
            public void WriteFile(BinaryWriter writer)
            {
                writer.Write(this.version);
                writer.Write(this.fileTime.Ticks);
                writer.Write(this.fileState);
                writer.Write(this.updateTime.Ticks);   
                writer.Write(this.sourceID);
                writer.Write(this.timeParam.StartTime.Ticks);
                writer.Write(this.timeParam.EndTime.Ticks);       
            }

            /// <summary>
            /// 读二进制文件
            /// </summary>
            public void ReadFile(BinaryReader reader)
            {
                this.version = reader.ReadInt32();
                this.fileTime = new DateTime(reader.ReadInt64());
                this.fileState = reader.ReadInt16();
                this.updateTime = new DateTime(reader.ReadInt64());
                this.sourceID = reader.ReadUInt32();
                DateTime startTime = new DateTime(reader.ReadInt64());
                DateTime endTime = new DateTime(reader.ReadInt64());
                this.timeParam = new DateTimeParam(startTime, endTime);                
            }
        }

        /// <summary>
        /// 定时记录查询条件
        /// </summary>
        private struct DataLogQueryCondition
        {
            /// <summary>
            /// 厂站ID
            /// </summary>
            public uint stationID;
            /// <summary>
            /// 设备ID
            /// </summary>
            public uint deviceID;
            /// <summary>
            /// 记录源ID
            /// </summary>
            public uint sourceID;        
            /// <summary>
            /// 定时记录列索引
            /// </summary>
            public int colIndex;
            /// <summary>
            /// 文件基础路径
            /// </summary>
            public string fileBasePath;
            /// <summary>
            /// 起始时间-结束时间
            /// </summary>
            public DateTimeParam timeParam;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="sourceID"></param>
            /// <param name="timeParam"></param>
            /// <param name="colIndex"></param>
            public DataLogQueryCondition(uint staID, uint devID, uint sourceID, int colIndex)
            {
                this.stationID = staID;
                this.deviceID = devID;
                this.sourceID = sourceID;
                this.timeParam = new DateTimeParam();
                this.colIndex = colIndex;
                this.fileBasePath = string.Empty;                
            }
        }

        /// <summary>
        /// 定时记录元素列表组合
        /// </summary>
        private struct DataLogListParam
        {
            public List<DateTime> logtimeList;
            public Dictionary<int, List<double>> colIndexValueMap;
        }

        /// <summary>
        /// 定时记录元素列表组合
        /// </summary>
        private struct DataLogArrayParam
        {
            public List<DateTime> logTimeList;
            public List<double>[] valueArray;
            public DataLogArrayParam(List<DateTime> logtimeList, List<double>[] valueArray)
            {
                this.logTimeList = logtimeList;
                this.valueArray = valueArray;
            }
        }
    }
}