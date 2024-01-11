using System;
using System.Collections.Generic;
using CET.PecsNodeManage;
using DBInterfaceCommonLib;
using System.Data;
using OfficeReportInterface.DefaultReportInterface;
using OfficeReportInterface.DefaultReportInterface.CommonluUsed;
using System.Globalization;
using CSharpDBPlugin;
using BasicDataInterface;
using EMSCommonLib;
using Newtonsoft.Json.Linq;


namespace OfficeReportInterface
{
    public enum PeriodType
    {
        Day_Type = 0,
        Week_Type = 1,
        Month_Type = 2,
        Year_Type = 3,
    }

    public class DefaultTemplatePublicMethod
    {

        //public static int ReadDatalogs(DBOperationFlag dbFlag, uint stationID, uint sourceID, int paraIndex, DateTime startTime,
        //    DateTime endTime, int maxRowCount, ref DataTable tempDT, DeviceDataIDDef deviceDataID)
        //{
        //    tempDT = new DataTable();
        //    int errorCode = PECSDBInterface.DatalogProvider.Instance.ReadDatalogs(
        //    DBOperationFlag.either, stationID, sourceID, paraIndex, startTime, endTime,
        //    Convert.ToInt32(SysConstDefinition.DefaultMaxRowCount)
        //    , ref tempDT);
        //    WriteLog(startTime, endTime, stationID, sourceID, paraIndex, errorCode);

        //    if (errorCode == 0)
        //    {//要乘以乘系数。
        //        AddCoefficient(deviceDataID,ref tempDT);
        //    }

        //    return errorCode;
        //}
        public static bool ReadDatalogs(DateTime startTime, DateTime endTime, DeviceDataIDDef deviceDataID, ref List<DataLogOriDef> result)
        {
            DATALOG_PRIVATE_MAP resultMapDef;
            bool hasMap = ReportWebServiceManager.ReportWebManager.FindDataMapDef(deviceDataID, out resultMapDef);
            if (!hasMap)
            {
                result = new List<DataLogOriDef>();
                return true ;
            }
            return ReadDatalogs(resultMapDef, startTime, endTime, deviceDataID,ref result);
        }

        public static bool ReadDatalogs(DATALOG_PRIVATE_MAP datalogNode, DateTime startTime, DateTime endTime, DeviceDataIDDef deviceDataID , ref List<DataLogOriDef> result)
        {
            result = new List<DataLogOriDef>();
            string dataParamStr = string.Format("{0},{1},{2},{3},{4}", datalogNode.deviceID, datalogNode.dataID, datalogNode.dataTypeID, datalogNode.logicalDeviceIndex, datalogNode.paraType);
            string timeParamStr = string.Format("{0},{1}", startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            var datalogResult = BasicDataProvider.GetInstance().GetOriDataLogList(dataParamStr, 0, 0, string.Empty, timeParamStr);
            if (!datalogResult.Success)
            {
                ErrorInfoManager.Instance.WriteLogMessage("ReadDatalogs", datalogResult.ErrorMessage);
                return false;
            }
            if (datalogResult.ResultList.Count > 0)
            {
                foreach (var item in datalogResult.ResultList[0].DataList)
                {
                    result.Add(new DataLogOriDef(DatetimeConvert.ConvertDoubleToDate(item.LogTime), item.DataValue));
                }
                //要乘以乘系数。
                AddCoefficient(deviceDataID, ref result);
            }

            return true;          
        }

        //private static void AddCoefficient(DeviceDataIDDef deviceDataID,ref DataTable tempDT)
        //{
        //    DataIDToMeasIDDef resultMapDef;
        //    bool result = ReportWebServiceManager.ReportWebManager.FindDataMapDef(deviceDataID, out resultMapDef);
        //    if (!result)
        //        return;
            
        //    if (resultMapDef.Cofficient == 1)
        //        return;
            
        //    double coefficient = resultMapDef.Cofficient;
        //    int count = tempDT.Rows.Count;
        //    for (int i = 0; i < count; ++i)
        //    {
        //        DataRow oneRow = tempDT.Rows[i];
        //        double dataValue;
        //        if (!double.TryParse(oneRow[3].ToString(), out dataValue))
        //            continue;
        //        dataValue = dataValue*coefficient;
        //        oneRow[3] = dataValue;
        //    }
        //}

        private static void AddCoefficient(DeviceDataIDDef deviceDataID,ref List<DataLogOriDef> tempDT)
        {
            DATALOG_PRIVATE_MAP  resultMapDef;
            bool result = ReportWebServiceManager.ReportWebManager.FindDataMapDef(deviceDataID, out resultMapDef);
            if (!result)
                return;

            if (resultMapDef.coefficient == 1)
                return;

            double coefficient = resultMapDef.coefficient;
            for (int i = 0; i < tempDT.Count; ++i)
            {
                DataLogOriDef oneRow = tempDT[i];
                oneRow.DataValue *=coefficient;
            }
        }

        private static void WriteLog(DateTime startTime, DateTime endTime, uint stationID, uint sourceID, int paraIndex, int errorCode)
        {
            DbgTrace.dout("调用数据库接口，查询定时记录。返回的错误字符串是:{0}；返回的错误码的值是：{1}。", DBInterfaceCommonLib.ErrorQuerier.Instance.GetLastErrorString(), errorCode);
            DbgTrace.dout("传入数据库接口的入参是：DBOperationFlag = {0} , stationID = {1} , sourceID = {2} , paraIndex = {3}, startTime = {4} , endTime.AddSeconds(1) = {5} , Convert.ToInt32(SysConstDefinition.DefaultMaxRowCount) = {6} 。", DBOperationFlag.either, stationID, sourceID, paraIndex, startTime, endTime.AddSeconds(1), Convert.ToInt32(SysConstDefinition.DefaultMaxRowCount));
        }

        public const double NA = -2147483648F;
        public static void PrintDataTable(DataTable resultDT)
        {
            string oneLine = string.Empty;

            for (int k = 0; k < resultDT.Rows.Count; k++)
            {
                for (int j = 0; j < resultDT.Columns.Count; j++)
                {
                    oneLine = string.Format("{0}  {1}", oneLine, resultDT.Rows[k][j]);
                }
                oneLine = string.Format("{0}\n", oneLine);
            }
            DbgTrace.dout(oneLine);
        }
        /// <summary>
        /// 计算增量累计值电量
        /// </summary>
        /// <param name="value"></param>
        /// <param name="resultTable"></param>
        /// <param name="cellItem"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="item"></param>
        public static double CaculateAcculateValue(DateTime startTime, DateTime endTime, List<DataLogOriDef> resultTable, ref int dataNullNumber)
        {
            double value = double.NaN;
            double tempValue = double.NaN;
            DateTime dt = DateTime.Now;
            List<TimeSpan> tsList = new List<TimeSpan>();
            bool isFirstValue = true;
            DateTime tempStartTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, startTime.Second);
            DateTime tempEndTime = new DateTime(endTime.Year, endTime.Month, endTime.Day, endTime.Hour, endTime.Minute, endTime.Second);

            object keyValue = null;
            DateTime dataStartTime = DateTime.Now;
            DateTime dataEndTime = DateTime.Now;
            for (int i = 0; i < resultTable.Count; i++)
            {
                DateTime logTime = resultTable[i].LogTime;
                if (logTime <= tempStartTime)
                    continue;
                if (logTime > tempEndTime)
                    break;
                if (isFirstValue)
                {
                    dt = logTime;
                    isFirstValue = false;
                    dataStartTime = logTime;
                }
                else
                {
                    TimeSpan ts = dt - logTime;
                    if (!tsList.Contains(ts))
                        tsList.Add(ts);
                    dataEndTime = logTime;
                }
                dt = logTime;

                keyValue = resultTable[i].DataValue;
                if (keyValue == null || Convert.IsDBNull(keyValue))
                {
                    dataNullNumber++;
                    continue;
                }
                if (IsNaN(tempValue))
                {
                    if (!IsNaN(keyValue))
                        tempValue = Convert.ToDouble(keyValue);
                    else
                        dataNullNumber++;
                }
                else
                {
                    if (!IsNaN(keyValue))
                        tempValue += Convert.ToDouble(keyValue);
                    else
                    {
                        dataNullNumber++;
                        continue;

                    }
                }
            }
            if (tsList.Count != 0)
            {
                //如果第一个时间加上时间间隔小于
                if (tempStartTime < dataStartTime.Add(tsList[0]))
                    dataNullNumber++;
                if (tempEndTime >= dataEndTime.Add(-tsList[0]))
                    dataNullNumber++;
            }
            //判断是否存在数据缺失，根据各个数据的时间间隔来判断，如果两个相邻数据的间隔不同，则认为数据存在缺失
            if (tsList.Count > 1)
                dataNullNumber = tsList.Count;//存在数据缺失，标红
            if (!IsNaN(tempValue))
            {
                if (IsNaN(value))
                    value = 0;
                value += tempValue;
            }
            return value;
        }

        public static void AddWarnings(DeviceDataIDDef deviceDataID, string warningInfo, DataTable dataWarning, bool isIncludeWarning,uint source)
        {
            string message = WarningInforManager.Instance.GetWarningMessage(deviceDataID, warningInfo,source);
            AddDataWarning(message, dataWarning, isIncludeWarning);
        }

        public static void AddWarnings(DeviceDataIDDef deviceDataID, WarningKind warningKind, DataTable dataWarning, bool isIncludeWarning, uint source)
        {
            string warningInfo = WarningInforManager.Instance.GetWarningMessage(warningKind, deviceDataID,source);
            AddDataWarning( warningInfo, dataWarning, isIncludeWarning);
        }
        public static void AddWarnings(DeviceDataIDDef deviceDataID, WarningKind warningKind, DateTime time, DataTable dataWarning, bool isIncludeWarning, uint source)
        {
            string warningInfo = WarningInforManager.Instance.GetWarningMessage(warningKind, deviceDataID,time,source);
            AddDataWarning(warningInfo, dataWarning, isIncludeWarning);
        }
        public static void AddWarnings(DeviceDataIDDef deviceDataID, WarningKind warningKind, DateTime time,string tariffIndexName, DataTable dataWarning, bool isIncludeWarning, uint source)
        {
           
            string warningInfo = WarningInforManager.Instance.GetWarningMessage(warningKind, deviceDataID, time,tariffIndexName, source);
            AddDataWarning(warningInfo, dataWarning, isIncludeWarning);
        }

        public static void AddWarnings(DeviceDataIDDef deviceDataID, DeviceDataIDDef deviceDataIDNew, WarningKind warningKind, DataTable dataWarning, bool isIncludeWarning, uint source)
        {
            string warningInfo = WarningInforManager.Instance.GetWarningMessage(warningKind, deviceDataID,deviceDataIDNew, source);
            AddDataWarning(warningInfo, dataWarning, isIncludeWarning);
        }
        /// <summary>
        /// 是否超过当前时刻
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private static bool IsBeyondNow(DateTime dateTime)
        {
            if (dateTime >= DateTime.Now)
                return true;
            return false;
        }

        /// <summary>
        /// 使用查询数据结果中的时间列中的每一行的时刻值，获取对应的原始数据值，放入对应的数据列
        /// </summary>
        /// <param name="tempDT">查询结果集</param>
        /// <param name="resultDT">赋值结果集</param>
        /// <param name="interval">查询间隔</param>
        /// <param name="measDataType">查询测点数据类型</param>
        /// <param name="valIndex">查询的值的序列</param>
        /// <returns>是否成功</returns>
        public static List<DataLogOriDef> GetSameTimeVal(DateTime startTime, DateTime endTime, DeviceDataIDDef deviceDataID, List<DataLogOriDef> tempDT, int interval, ref DateTime dataNullStartTime, ref int dataNullNumber,DateTime realStartTime,bool limitCount)
        {
            List<DataLogOriDef> resultDT = new List<DataLogOriDef>();
            resultDT = tempDT;
            int rowIndex = 0;
            bool hasHappend = false;

            while (startTime <= endTime)
            {
                if (rowIndex >= tempDT.Count)
                {
                    resultDT.Add(new DataLogOriDef(startTime,double.NaN));

                    if (dataNullNumber == 0)
                    {
                        if (hasHappend == false)
                        {
                            if (!IsBeyondNow(startTime))
                            {
                                dataNullStartTime = startTime;
                                hasHappend = true;
                            }
                        }
                        dataNullNumber++;
                    }
                    startTime = startTime.AddSeconds(interval);
                    continue;
                }
                DateTime logtime = tempDT[rowIndex].LogTime;
                if (logtime < startTime)
                {
                    rowIndex++;
                    continue;
                }

                //时间大于等于基准时间，加一行
                DataLogOriDef resultRow = new DataLogOriDef(startTime, double.NaN);
                if (logtime == startTime)
                {
                    resultRow.DataValue =  tempDT[rowIndex].DataValue;
                }
                else
                {
                    if (hasHappend == false)
                    {
                        dataNullStartTime = startTime;
                        hasHappend = true;
                        dataNullNumber++;
                    }
                }
                resultDT.Add(resultRow);

                if (IsNaN(resultRow.DataValue) && dataNullNumber == 0)
                {
                    if (hasHappend == false)
                    {
                        if (!IsBeyondNow(startTime))
                        {
                            dataNullStartTime = startTime;
                            hasHappend = true;
                        }
                    }                   
                    dataNullNumber++;
                }
                startTime = startTime.AddSeconds(interval);
                if (resultDT.Count >= MAX_COUNT &&limitCount)
                    break;
            }
            //判断开头部分是不是有空数据
            if (resultDT.Count > 0 &&
                Convert.ToDateTime(resultDT[0].LogTime).AddSeconds(-interval) >= realStartTime)
            {
                dataNullStartTime = realStartTime;
                hasHappend = true;
                dataNullNumber++;
            }
            return resultDT;
        }

        /// <summary>
        /// 获取数据时间间隔行数
        /// </summary>
        /// <param name="resultDT"></param>
        /// <returns></returns>
        public static int GetInterval(List<DataLogOriDef> resultDT, ref int index, ref DateTime dataStartTime, DeviceDataIDDef deviceDataIDDef, DataTable dataWarning, bool isIncludeWarning,uint source)
        {
            int result = 3600;//默认间隔一小时
            DateTime tempTime = DateTime.MinValue;
            bool hasChanged = false;

            DateTime queryStartTime = dataStartTime;//保留查询的起始时间，用于向前推时间

            bool isFindInterval = false;
            for (int i = 0; i < resultDT.Count; i++)
            {
                if (i == 0)
                    dataStartTime = Convert.ToDateTime(resultDT[i].LogTime);
                TimeSpan ts = Convert.ToDateTime(resultDT[i].LogTime) - tempTime;

                //间隔发生变化，给出提示.小时报表不需要间隔变化的警告信息提示
                if (Math.Round(ts.TotalSeconds) != result && i >= 2 && hasChanged == false && source != (uint)RepServFileType.HourlyUsage)
                {
                    AddWarnings(deviceDataIDDef, WarningKind.DataIntervalChanged, tempTime, dataWarning, isIncludeWarning,source);
                    hasChanged = true;
                }
                if (Math.Round(ts.TotalSeconds) < result)
                {
                    result = Convert.ToInt32(ts.TotalSeconds);
                    isFindInterval = true;
                }
            
                tempTime = Convert.ToDateTime(resultDT[i].LogTime);       

            }
            //第一个参数数据不存在，从第二个开始继续找间隔
            if (!isFindInterval)
                index = -1;
            //--------将起始点移动到离查询的起始时间最近或者查询的点本身。原因：例如查询2018年3月9日0：00 - 2018年3月10日00：00   ，有数据的点是从2018年3月9日11：45开始的，这样11：45之前的点就没有写到表格中，导致显示的曲线的起始点是11：45，实际应该画从2018年3月9日00：00开始的曲线，而不是11：45开始的曲线  这个是IBD的丁玉凤提出的问题，采用这个办法来解决----------------------------------------------
            var sTime = dataStartTime;//从有数据的起始时刻，开始向前倒推，直到查询起始时间
            while (sTime >= queryStartTime)
            {
                sTime = dataStartTime.AddSeconds(-result);//往前推一个间隔，例如减去5分钟
                if (sTime >= queryStartTime) //如果大于或等于起始时间，则将数据起始时刻设置为该时刻
                    dataStartTime = sTime;
            }
            //---------------------------------------------------------
            return result;
        }
        public const int MAX_COUNT = 30000;
        public static void ConstrutDataTable(DateTime startTime, DateTime endTime, DataTable dt, int interval,out List<string> warningMessageList)
        {
            warningMessageList = new List<string>();
            int count = 0;
            while (startTime <= endTime)
            {
                DataRow dr = dt.NewRow();
                dr[0] = startTime.ToString(DataManager.TimeFormatWithoutSecond) + "\r";
                startTime = startTime.AddSeconds(interval);
                dt.Rows.Add(dr);
                ++count;
                if (count >= MAX_COUNT)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 处理翻转
        /// </summary>
        /// <param name="deviceDataID"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double HandleRollOverValue(DeviceDataIDDef deviceDataID, double value)
        {
            double tempValue = double.NaN;
            if (double.IsNaN(value) || value >= 0)
                return value;
            if (deviceDataID.DataID <= 4000000 || deviceDataID.DataID > 5000000)
                return value;
            DataTable resultDT = new DataTable();
            try
            {
                DATALOG_PRIVATE_MAP resultMapDef;
                bool result = ReportWebServiceManager.ReportWebManager.FindDataMapDef(deviceDataID, out resultMapDef);
                if (!result)
                {
                    return value;
                }
                if (double.TryParse(resultMapDef.reserved, out tempValue))
                    return value + tempValue;
          
            }
            finally
            {
                if(resultDT!=null)
                resultDT.Dispose();
            }
            return value;
        }

        /// <summary>
        /// 获取每月最大天数
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetMaxDaysofMonth(DateTime time)
        {
            switch (time.Month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return 31;
                case 4:
                case 6:
                case 9:
                case 11:
                    return 30;
                case 2:
                    if (DateTime.IsLeapYear(time.Year))
                    {
                        return 29;
                    }
                    break;
            }
            return 28;
        }

        /// <summary>
        /// 给表增加列
        /// </summary>
        /// <param name="resultDT"></param>
        /// <param name="columnName"></param>
        public static void AddColumnToTable(DataTable resultDT, string columnName)
        {
            DataColumn column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = columnName;
            column.DefaultValue = "";
            resultDT.Columns.Add(column);
        }

        public static double GetDiffValue(object value1, object value2, DeviceDataIDDef deviceDataID)
        {
            double tempValue1 = double.NaN;
            double tempValue2 = double.NaN;
            if (Convert.IsDBNull(value1) || Convert.IsDBNull(value2))
                return double.NaN;
            if (!double.TryParse(value1.ToString(), out tempValue1))
                return double.NaN;
            if (!double.TryParse(value2.ToString(), out tempValue2))
                return double.NaN;
            return HandleRollOverValue(deviceDataID, Convert.ToDouble(tempValue1 - tempValue2));
        }

        /// <summary>
        /// 根据时间以及报表查询时段类型返回对应的时间字符串
        /// </summary>
        /// <param name="startTime">时间</param>
        /// <param name="periodType">时段类型</param>
        /// <returns></returns>
        public static string CaculateTimeIndexByPeriodType(DateTime startTime, int periodType)
        {
            switch (periodType)
            {
                case (int)PeriodType.Day_Type:
                    {
                        if (startTime.Hour < 9)
                            return "0" + (startTime.Hour + 1) + ":00";
                        else
                            return (startTime.Hour + 1) + ":00";
                    }
                case (int)PeriodType.Week_Type:
                {
                    return DayOfWeekManager.Instance.GetDayOfWeek(startTime);
                }
                 
                case (int)PeriodType.Month_Type:
                    return startTime.ToString("dd");
                case (int)PeriodType.Year_Type:
                    return startTime.ToString("MM");
                default:
                    return "";
            }
        }

        /// <summary>
        /// 根据时段类型反馈对应的时标标识符
        /// </summary>
        /// <param name="periodType"></param>
        /// <returns></returns>
        public static string GetIndexCulumnName(int periodType)
        {
            switch (periodType)
            {
                case (int)PeriodType.Day_Type:
                    return LocalResourceManager.GetInstance().GetString("0012", "Hour of Day");
                case (int)PeriodType.Week_Type:
                    return LocalResourceManager.GetInstance().GetString("0013", "Day of Week");
                case (int)PeriodType.Month_Type:
                    return LocalResourceManager.GetInstance().GetString("0014", "Day of Month");
                default:
                    return LocalResourceManager.GetInstance().GetString("0015", "Month of Year");
            }
        }

        /// <summary>
        /// 根据时段类型反馈对应的时标标识符
        /// </summary>
        /// <param name="periodType"></param>
        /// <returns></returns>
        public static string GetReportTypeNameByPeriodType(int periodType)
        {
            switch (periodType)
            {
                case (int)PeriodType.Day_Type:
                    return LocalResourceManager.GetInstance().GetString("0016", "Daily");
                case (int)PeriodType.Week_Type:
                    return LocalResourceManager.GetInstance().GetString("0017", "Weekly");
                case (int)PeriodType.Month_Type:
                    return LocalResourceManager.GetInstance().GetString("0018", "Monthly");
                default:
                    return LocalResourceManager.GetInstance().GetString("0019", "yearly");
            }
        }

        public static string GetDateTimeFormat(int periodType)
        {
            switch (periodType)
            {
                case (int)PeriodType.Day_Type:
                    return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                case (int)PeriodType.Week_Type:
                    return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                case (int)PeriodType.Month_Type:
                    DateTimeFormatManager dateTimeFormatManager = new DateTimeFormatManager();
                    string format = dateTimeFormatManager.GetYearAndMonthNormal();
                    return format;
                default:
                    DateTimeFormatManager dateTimeFormatManager2 = new DateTimeFormatManager();
                    string format2 = dateTimeFormatManager2.GetYearNormal();
                    return format2;
            }
        }

        public static string GetDateStringColumnName(DateTime time, int periodType)
        {
            switch (periodType)
            {
                case (int)PeriodType.Day_Type:
                    return time.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern) + "\r";
                case (int)PeriodType.Week_Type:
                    return time.Year.ToString() + " " + LocalResourceManager.GetInstance().GetString("0046", "Week") +" "+ ((time.DayOfYear - 1) / 7 + 1);
                case (int)PeriodType.Month_Type:
                    DateTimeFormatManager dateTimeFormatManager = new DateTimeFormatManager();
                    string format = dateTimeFormatManager.GetYearAndMonthNormal();
                    return time.ToString(format);
                default:
                    string format2 = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                    return time.ToString(format2);
            }
        }
        /// <summary>
        /// 解析参数字符串
        /// </summary>
        /// <param name="parameters">deviceId,dataID,logicalDeviceIndex,dataTypeID</param>
        /// <returns></returns>
        public static List<DeviceDataIDDef> ConstructParaList(string parameters)
        {
            if (parameters.Contains(":"))//情况1：如果是Json格式的，就以json方式解析
            {
                return DataFormatManager.Create(FormatType.JsonType).DeserializeObject<List<DeviceDataIDDef>>(parameters);
            }
            //情况2：按照旧的方式解析
            List<DeviceDataIDDef> queryParamList = new List<DeviceDataIDDef>();
            try
            {
                List<string> resultStrs = DataFormatManager.ParseStringList(parameters, ";");
                for (int i = 0; i < resultStrs.Count; i++)
                {
                    string paraStrs = resultStrs[i];
                    List<uint> paraIDs = DataFormatManager.ParseUIntList(paraStrs, ",");
                    if (paraIDs.Count <= 0)
                        continue;
                    uint deviceID = paraIDs[0];
                    uint dataID = SysConstDefinition.DATAIDKWH_IMPORT;
                    int dataTypeID = 1;
                    int logicalDeviceIndex = 1;
                    if (paraIDs.Count > 1)
                        dataID = Convert.ToUInt32(paraIDs[1]);
                    if (paraIDs.Count > 2)
                        logicalDeviceIndex = Convert.ToInt32(paraIDs[2]);
                    if (paraIDs.Count > 3)
                        dataTypeID = Convert.ToInt32(paraIDs[3]);
                    DeviceDataIDDef newParam = new DeviceDataIDDef(SysNodeType.PECSDEVICE_NODE, deviceID, dataID, (int)DataIDParaType.DatalogType, Convert.ToInt32(dataTypeID), logicalDeviceIndex);
                    queryParamList.Add(newParam);
                }
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }
            return queryParamList;
        }

        /// <summary>
        /// 根据报表类型获取时间间隔
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="periodType"></param>
        /// <returns></returns>
        public static DateTime CaculateIntervelEndTimeByPeriodType(DateTime startTime, int periodType)
        {
            switch (periodType)
            {
                case (int)PeriodType.Day_Type:
                    return startTime.AddHours(1);
                case (int)PeriodType.Week_Type:
                    return startTime.AddDays(1);
                case (int)PeriodType.Month_Type:
                    return startTime.AddDays(1);
                case (int)PeriodType.Year_Type:
                    return startTime.AddMonths(1);
                default:
                    return startTime;
            }
        }

        // <summary>
        /// 创建返回的结果集表格
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="periodType"></param>
        /// <param name="intervalType"></param>
        /// <param name="resultDTList"></param>
        /// <param name="resultDT"></param>
        /// <param name="dr2"></param>
        public static DataTable ConstructRealDataTable(DateTime startTime, DateTime endTime, List<DeviceDataIDDef> paraList,uint source)
        {
            DataTable resultDT = new DataTable("Data");
            DataColumn column;
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "TimeStamp";
            resultDT.Columns.Add(column);

            for (int i = 0; i < paraList.Count; i++)
            {
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = i.ToString();
                resultDT.Columns.Add(column);
            }

            DataRow dr = resultDT.NewRow();
            dr[0] = LocalResourceManager.GetInstance().GetString("0026", "Time");
            for (int i = 0; i < paraList.Count; i++)
            {
                PecsDeviceNode deviceNode = PecsNodeManager.PecsNodeInstance.GetDeviceNodeByID(paraList[i].DeviceID);
                if (deviceNode != null)
                    dr[i + 1] = GetDeviceNameByDeviceDataID(paraList[i],source);
            }
            resultDT.Rows.Add(dr);
            dr = resultDT.NewRow();
            dr[0] = LocalResourceManager.GetInstance().GetString("0027", "Parameters");
            NamesManager namesManager = NamesManager.GetInstance(source);
            for (int i = 0; i < paraList.Count; i++)
            {
                string paraNameWithLoop = string.Empty;
                if (namesManager.GetMeasureName(paraList[i], out paraNameWithLoop))
                    dr[i + 1] = paraNameWithLoop;
            }
            resultDT.Rows.Add(dr);
            return resultDT;
        }

        public static DateTime CaculateEndTimeByPeriodType(DateTime startTime, int periodType)
        {
            switch (periodType)
            {
                case (int)PeriodType.Day_Type:
                    return startTime.AddDays(1);
                case (int)PeriodType.Week_Type:
                    return startTime.AddDays(7);
                case (int)PeriodType.Month_Type:
                    return startTime.AddMonths(1);
                case (int)PeriodType.Year_Type:
                    return startTime.AddYears(1);
                default:
                    return startTime;
            }
        }

        public static string GetDeviceNameByDeviceDataID(DeviceDataIDDef deviceDataIDDef,uint source)
        {
            string deviceName;          
            NamesManager namesManager =  NamesManager.GetInstance(source);
            namesManager.GetDeviceNameWithLoop(deviceDataIDDef, out deviceName);
            return deviceName;       
        }

        public static object GetTotalData(object resultValue, object addValue)
        {
            double tempValue1 = double.NaN;
            double tempValue2 = double.NaN;
            if (Convert.IsDBNull(addValue))
                return resultValue;
            if (!double.TryParse(addValue.ToString(), out tempValue1))
                return resultValue;
            if (!double.TryParse(resultValue.ToString(), out tempValue2))
                resultValue = 0;
            return tempValue1 + tempValue2;
        }

        /// <summary>
        /// 使用查询数据结果中的时间列中的每一行的时刻值，获取对应的原始数据值，放入对应的数据列
        /// </summary>
        /// <param name="tempDT">查询结果集</param>
        /// <param name="resultDT">赋值结果集</param>
        /// <param name="interval">查询间隔</param>
        /// <param name="measDataType">查询测点数据类型</param>
        /// <param name="valIndex">查询的值的序列</param>
        /// <returns>是否成功</returns>
        public static List<DataLogOriDef> GetSameTimeVal(DateTime startTime, DateTime endTime, List<DataLogOriDef> tempDT, int periodType)
        {
            List<DataLogOriDef> resultList = new List<DataLogOriDef>();
            int rowIndex = 0;
            DataRow resultRow = null;
            while (startTime <= endTime)
            {
                DataLogOriDef datalog = new DataLogOriDef();
                if (rowIndex >= tempDT.Count)
                {
                    datalog.LogTime = startTime;
                    resultList.Add(datalog);
                    startTime = CaculateIntervelEndTimeByPeriodType(startTime, periodType);
                    continue;
                }
                DateTime logtime = tempDT[rowIndex].LogTime;
                if (logtime < startTime)
                {
                    rowIndex++;
                    continue;
                }
                //时间大于等于基准时间，加一行
                datalog = new DataLogOriDef();
                datalog.LogTime= startTime;
                if (logtime == startTime)
                    datalog.DataValue = tempDT[rowIndex].DataValue;
                resultList.Add(datalog);
                startTime = CaculateIntervelEndTimeByPeriodType(startTime, periodType);
            }
            return resultList;
        }

        private static DataTable ConstructResultTable()
        {
            DataTable resultTable = new DataTable();
            DefaultTemplatePublicMethod.AddColumnToTable(resultTable, "LogTime");
            DefaultTemplatePublicMethod.AddColumnToTable(resultTable, "1");
            DefaultTemplatePublicMethod.AddColumnToTable(resultTable, "2");
            DefaultTemplatePublicMethod.AddColumnToTable(resultTable, "3");
            return resultTable;
        }

        /// <summary>
        /// 返回参数名称
        /// </summary>
        /// <param name="deviceDataId"></param>
        /// <returns></returns>
        public static string GetParanameByParament(DeviceDataIDDef deviceDataId,uint source)
        {
            string dataName;
            NamesManager namesManager =  NamesManager.GetInstance(source);
            namesManager.GetMeasureName(deviceDataId, out dataName);
            return dataName;
        }


        /// <summary>
        /// 每增加一个参数要增加一个结束标志位
        /// </summary>
        /// <param name="resultTempDT"></param>
        public static void AddDataLastRow(DataTable resultTempDT)
        {
            DataRow dr = resultTempDT.NewRow();
            dr[0] = "Next";
            resultTempDT.Rows.Add(dr);
        }

        /// <summary>
        /// 增量计算
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static object GetChangedValue(object value1, object value2)
        {
            double tempValue1 = double.NaN;
            double tempValue2 = double.NaN;
            if (Convert.IsDBNull(value1) || Convert.IsDBNull(value2))
                return "";
            if (!double.TryParse(value1.ToString(), out tempValue1))
                return "";
            if (!double.TryParse(value2.ToString(), out tempValue2))
                return "";
            if (Math.Abs(tempValue1) < 0.0001)
                return 0 + "%";
            return (tempValue2 - tempValue1) / tempValue1 * 100 + "%";
        }

        /// <summary>
        /// 汇总列计算
        /// </summary>
        /// <param name="dr2"></param>
        /// <param name="resultDT"></param>
        public static void AddTotalColumn(DataTable resultDT, int startRow, int startColumn)
        {
            for (int i = startRow; i < resultDT.Rows.Count; i++)
            {
                double columnTotal = double.NaN;
                for (int j = startColumn; j < resultDT.Columns.Count; j++)
                {
                    double tempValue = double.NaN;
                    if (double.TryParse(resultDT.Rows[i][j].ToString(), out tempValue))
                    {
                        if (double.IsNaN(columnTotal))
                            columnTotal = 0;
                        columnTotal += tempValue;
                    }
                }
                if (!double.IsNaN(columnTotal))
                    resultDT.Rows[i][resultDT.Columns.Count - 1] = columnTotal;
            }
        }

        /// <summary>
        /// 汇总行计算
        /// </summary>
        /// <param name="dr2"></param>
        /// <param name="resultDT"></param>
        public static void AddTotalRow(DataTable resultDT, int startNumber)
        {
            DataRow dr = resultDT.NewRow();
            dr[0] = LocalResourceManager.GetInstance().GetString("0081", "Period Total");

            for (int j = 1; j < resultDT.Columns.Count; j++)
            {
                double columnTotal = double.NaN;
                for (int i = startNumber; i < resultDT.Rows.Count; i++)
                {
                    double tempValue = double.NaN;
                    if (double.TryParse(resultDT.Rows[i][j].ToString(), out tempValue))
                    {
                        if (double.IsNaN(columnTotal))
                            columnTotal = 0;
                        columnTotal += tempValue;
                    }
                }
                if (!double.IsNaN(columnTotal))
                    dr[j] = columnTotal;
            }
            resultDT.Rows.Add(dr);
        }

        /// <summary>
        /// 构建警告信息表格
        /// </summary>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        public static DataTable ConstructWarningTable(bool isIncludeWarning)
        {
            DataTable resultDT = new DataTable("warning");
            AddColumnToTable(resultDT, "Message");
            AddColumnToTable(resultDT, "Date");
            if (isIncludeWarning)
            {
                DataRow dr = resultDT.NewRow();
                dr[0] = LocalResourceManager.GetInstance().GetString("0037", "Message");
                dr[1] =  "Date Added";
                resultDT.Rows.Add(dr);
            }
            return resultDT;
        }

        public static void DataWarningNullFromTime(DateTime startTime, DeviceDataIDDef deviceDataID, DataTable dataWarning,
          bool isShowWarning,uint source)
        {
            string message = WarningInforManager.Instance.GetWarningMessage(WarningKind.DataNullStartFromSomeTime,
                deviceDataID, startTime,source);
            AddDataWarning(message, dataWarning, isShowWarning);
        }

        /// <summary>
        /// 警告信息增加行
        /// </summary>
        /// <param name="dataWarning"></param>
        public static void AddDataWarning(string warningMessage, DataTable dataWarning, bool isIncludeWarning)
        {
            if (!isIncludeWarning)
                return;
            DataRow dr = dataWarning.NewRow();
            dr[0] = warningMessage;
            dr[1] =DateTime.Now.ToString(DataManager.GetWarningMessageTimeFormat()) ; 
            dataWarning.Rows.Add(dr);
        }

        /// <summary>
        /// 查询 3/5 分钟间隔日报统计结果
        /// </summary>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="statType">统计类型</param>
        /// <param name="stationID">厂站号</param>
        /// <param name="sourceID">定时记录源号</param>
        /// <param name="paraIndex">参数序号</param>
        /// <param name="resultDT">查询结果集</param>
        /// <returns>是否成功</returns>
        //public static bool QueryDataLogData(DateTime startTime, DateTime endTime, DeviceDataIDDef deviceDataID, int periodType, uint stationID, uint sourceID, int paraIndex, ref DataTable resultDT, ref DataTable dataWarning, bool isShowWarning,uint source)
        //{
        //    string temp = string.Format("QueryDataLogData(DateTime startTime={0}, DateTime endTime={1}, DeviceDataIDDef deviceDataID={2}, int periodType={3}, uint stationID={4}, uint sourceID={5}, int paraIndex={6}, ref DataTable resultDT, ref DataTable dataWarning, bool isShowWarning={7},uint source={8})",startTime,  endTime,  deviceDataID.ToString(),  periodType,  stationID,  sourceID, paraIndex, isShowWarning, source);
        //    DbgTrace.dout(temp);    //根据不同的间隔类型，自动格式化不同的查询起始时间和结束时间
        //    DataTable tempDT = new DataTable();
        //    int errorCode = DefaultTemplatePublicMethod.ReadDatalogs(DBOperationFlag.either, stationID, sourceID, paraIndex, startTime, endTime.AddSeconds(1), Convert.ToInt32(SysConstDefinition.DefaultMaxRowCount), ref tempDT, deviceDataID);

        //    if (errorCode != (int)CSharpDBPlugin.ErrorCode.Success || tempDT.Rows.Count == 0)
        //    {

        //        AddWarnings(deviceDataID, WarningKind.DataIsNull, dataWarning, isShowWarning, source);

        //        return false;
        //    }

        //    //增量类型电度值计算和差值计算不一致
        //    if (deviceDataID.DataID >= 4000429 && deviceDataID.DataID <= 4000433)
        //    {
        //        resultDT = tempDT.Clone();
        //        while (startTime < endTime)//累计值的不需要最后一个时间点的数据
        //        {
        //            DateTime tempEndTime = CaculateIntervelEndTimeByPeriodType(startTime, periodType);
        //            int dataNullNumber = 0;
        //            DataRow resultRow = resultDT.NewRow();
        //            resultRow["logTime"] = startTime;
        //            resultRow[3] = CaculateAcculateValue(startTime, tempEndTime, tempDT, ref dataNullNumber);
        //            resultDT.Rows.Add(resultRow);
        //            startTime = CaculateIntervelEndTimeByPeriodType(startTime, periodType);
        //            DbgTrace.dout("计算累计值：startTime={0}, tempEndTime={1},得到的结果是：{2}.", startTime, tempEndTime, resultRow[3]);
        //        }
        //    }
        //    else
        //    {
        //        //使用查询数据结果中的时间列中的每一行的时刻值，获取对应的原始数据值，放入对应的数据列
        //        resultDT = GetSameTimeVal(startTime, endTime, tempDT, periodType);
        //        int tipTimes = 0;
        //        //对结果集求差值
        //        for (int i = 0; i < resultDT.Rows.Count - 1; i++)
        //        {
        //            var value1 = resultDT.Rows[i + 1][3];
        //            var value2 = resultDT.Rows[i][3];
        //            double tempValue1 = double.NaN;
        //            double tempValue2 = double.NaN;

        //            if (Convert.IsDBNull(value1) )
        //            {
        //                if (tipTimes == 0)
        //                {
        //                    AddWarnings(deviceDataID, WarningKind.DataNullStartFromSomeTime,
        //                        Convert.ToDateTime(resultDT.Rows[i + 1][0]),
        //                        dataWarning,
        //                        isShowWarning, source);
        //                    ++tipTimes;
        //                }
        //            }
        //            if (Convert.IsDBNull(value2))
        //            {
        //                if (tipTimes == 0)
        //                {
        //                    AddWarnings(deviceDataID, WarningKind.DataNullStartFromSomeTime,
        //                        Convert.ToDateTime(resultDT.Rows[i][0]),
        //                        dataWarning,
        //                        isShowWarning, source);
        //                    ++tipTimes;
        //                }
        //            }
        //            if (!double.TryParse(value1.ToString(), out tempValue1))
        //            {
        //                if (tipTimes == 0)
        //                {
        //                    AddWarnings(deviceDataID, WarningKind.DataNullStartFromSomeTime,
        //                        Convert.ToDateTime(resultDT.Rows[i + 1][0]),
        //                        dataWarning,
        //                        isShowWarning, source);
        //                    ++tipTimes;
        //                }
        //            }
        //            if (!double.TryParse(value2.ToString(), out tempValue2))
        //            {
        //                if (tipTimes == 0)
        //                {
        //                    AddWarnings(deviceDataID, WarningKind.DataNullStartFromSomeTime,
        //                        Convert.ToDateTime(resultDT.Rows[i][0]),
        //                        dataWarning,
        //                        isShowWarning, source);
        //                    ++tipTimes;
        //                }
        //            }

        //            var resultTemp=GetDiffValue(resultDT.Rows[i + 1][3], resultDT.Rows[i][3], deviceDataID);
        //            resultDT.Rows[i][3] = resultTemp;

        //            DbgTrace.dout("计算差值：(时刻：{0})(值：{1}) - (时刻：{2})(值：{3}) = {4} .",
        //                resultDT.Rows[i + 1][0], resultDT.Rows[i + 1][3], resultDT.Rows[i][0], resultDT.Rows[i][3], resultTemp);
        //        }
        //        resultDT.Rows.RemoveAt(resultDT.Rows.Count - 1);
        //        DbgTrace.dout("resultDT.Rows.RemoveAt(resultDT.Rows.Count - 1);");
        //    }
        //    return true;
        //}

        public static bool QueryDataLogData(DateTime startTime, DateTime endTime, DeviceDataIDDef deviceDataID, int periodType, ref List<DataLogOriDef> resultList, ref DataTable dataWarning, bool isShowWarning, uint source)
        {
            string temp = string.Format("QueryDataLogData(DateTime startTime={0}, DateTime endTime={1}, DeviceDataIDDef deviceDataID={2}, int periodType={3}, ref List<DataLogOriDef> resultDT, ref DataTable dataWarning, bool isShowWarning={4},uint source={5})", startTime, endTime, deviceDataID.ToString(), periodType,  isShowWarning, source);
            DbgTrace.dout(temp);    //根据不同的间隔类型，自动格式化不同的查询起始时间和结束时间

            List<DataLogOriDef> tempDT = new List<DataLogOriDef>();
            bool isSucess = DefaultTemplatePublicMethod.ReadDatalogs(startTime, endTime.AddSeconds(1), deviceDataID, ref tempDT);
            if (!isSucess)
            {
                AddWarnings(deviceDataID, WarningKind.DataIsNull, dataWarning, isShowWarning, source);
                return false;
            }

            List<DataLogOriDef> resultDT = new List<DataLogOriDef>();
            //增量类型电度值计算和差值计算不一致
            if (deviceDataID.DataID >= 4000429 && deviceDataID.DataID <= 4000433)
            {
                resultDT = tempDT;
                while (startTime < endTime)//累计值的不需要最后一个时间点的数据
                {
                    DateTime tempEndTime = CaculateIntervelEndTimeByPeriodType(startTime, periodType);
                    int dataNullNumber = 0;
                    DataLogOriDef datalog = new DataLogOriDef();
                    datalog.LogTime = startTime;
                    datalog.DataValue = CaculateAcculateValue(startTime, tempEndTime, tempDT, ref dataNullNumber);
                    resultDT.Add(datalog);
                    startTime = CaculateIntervelEndTimeByPeriodType(startTime, periodType);
                    DbgTrace.dout("计算累计值：startTime={0}, tempEndTime={1},得到的结果是：{2}.", startTime, tempEndTime, datalog.DataValue);
                }
            }
            else
            {
                //使用查询数据结果中的时间列中的每一行的时刻值，获取对应的原始数据值，放入对应的数据列
                resultDT = GetSameTimeVal(startTime, endTime, tempDT, periodType);
                int tipTimes = 0;
                //对结果集求差值
                resultList = new List<DataLogOriDef>();
                for (int i = 0; i < resultDT.Count - 1; i++)
                {
                    var value1 = resultDT[i + 1].DataValue;
                    var value2 = resultDT[i].DataValue;
                    double tempValue1 = double.NaN;
                    double tempValue2 = double.NaN;

                    if (Convert.IsDBNull(value1))
                    {
                        if (tipTimes == 0)
                        {
                            AddWarnings(deviceDataID, WarningKind.DataNullStartFromSomeTime,
                                resultDT[i + 1].LogTime,
                                dataWarning,
                                isShowWarning, source);
                            ++tipTimes;
                        }
                    }
                    if (Convert.IsDBNull(value2))
                    {
                        if (tipTimes == 0)
                        {
                            AddWarnings(deviceDataID, WarningKind.DataNullStartFromSomeTime,
                                resultDT[i].LogTime,
                                dataWarning,
                                isShowWarning, source);
                            ++tipTimes;
                        }
                    }
                    if (!double.TryParse(value1.ToString(), out tempValue1))
                    {
                        if (tipTimes == 0)
                        {
                            AddWarnings(deviceDataID, WarningKind.DataNullStartFromSomeTime,
                                resultDT[i + 1].LogTime,
                                dataWarning,
                                isShowWarning, source);
                            ++tipTimes;
                        }
                    }
                    if (!double.TryParse(value2.ToString(), out tempValue2))
                    {
                        if (tipTimes == 0)
                        {
                            AddWarnings(deviceDataID, WarningKind.DataNullStartFromSomeTime,
                                resultDT[i].LogTime,
                                dataWarning,
                                isShowWarning, source);
                            ++tipTimes;
                        }
                    }
                    var resultTemp = GetDiffValue(resultDT[i + 1].DataValue, resultDT[i].DataValue, deviceDataID);
                    resultList.Add(new DataLogOriDef(resultDT[i].LogTime, resultTemp));
                    DbgTrace.dout("计算差值：(时刻：{0})(值：{1}) - (时刻：{2})(值：{3}) = {4} .",
                        resultDT[i + 1].LogTime.ToString(), resultDT[i + 1].DataValue, resultDT[i].LogTime.ToString(), resultDT[i].DataValue, resultTemp);
                }
                DbgTrace.dout("resultDT.Rows.RemoveAt(resultDT.Rows.Count - 1);");
            }
            return true;
        }

        /// <summary>
        /// 增加警告信息
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="dataWarning"></param>
        /// <param name="resultTempDT"></param>
        /// <param name="rowIndex"></param>
        /// <param name="tempDT"></param>
        public static void AddDataNullWarnings(DefaultReportParameter parameter, DataTable dataWarning, DataTable resultTempDT, int rowIndex, List<DataLogOriDef> tempDT,uint source)
        {
            //记录空缺数据的数目
            int dataNullNuber = 0;
            //记录第一条空缺数据的时间
            DateTime dataNullTime = DateTime.Now;
            for (int j = 0; j < tempDT.Count; j++)
            {
                if (IsNaN(tempDT[j].DataValue))
                {
                    resultTempDT.Rows[j + 1][rowIndex + 1] = "";
                    dataNullNuber++;
                    if (dataNullNuber == 1)
                        dataNullTime = Convert.ToDateTime(tempDT[j].LogTime);
                }
                else
                    resultTempDT.Rows[j + 1][rowIndex + 1] = tempDT[j].DataValue;
            }
        }

        public static bool IsNaN(object obj)
        {
            double tempDouble = double.NaN;
            if (!double.TryParse(obj.ToString(), out tempDouble))
                return true;
            if (double.IsNaN(tempDouble))
                return true;
            return false;
        }

        /// <summary>
        /// 计算行列累加数据
        /// </summary>
        /// <param name="resultDT"></param>
        public static void CaculateRowAndColumnTotal(ref DataTable resultDT)
        {
            DataRow dr = resultDT.NewRow();
            dr[0] = LocalResourceManager.GetInstance().GetString("0081", "Period Total");
            double tempTotalValue = double.NaN;
            //计算列累加
            for (int j = 1; j < resultDT.Columns.Count - 1; j++)
            {
                tempTotalValue = double.NaN;
                for (int i = 1; i < resultDT.Rows.Count; i++)
                {
                    double tempValue = double.NaN;
                    if (!double.TryParse(resultDT.Rows[i][j].ToString(), out tempValue))
                        continue;
                    if (double.IsNaN(tempTotalValue))
                        tempTotalValue = 0;
                    tempTotalValue += tempValue;
                }
                if (!double.IsNaN(tempTotalValue))
                {
                    dr[j] = tempTotalValue;
                }
            }
            resultDT.Rows.Add(dr);
            //计算最后一列累加
            for (int i = 0; i < resultDT.Rows.Count; i++)
            {
                tempTotalValue = double.NaN;
                for (int j = 1; j < resultDT.Columns.Count - 1; j++)
                {
                    double tempValue = double.NaN;
                    if (!double.TryParse(resultDT.Rows[i][j].ToString(), out tempValue))
                        continue;
                    if (double.IsNaN(tempTotalValue))
                        tempTotalValue = 0;
                    tempTotalValue += tempValue;
                }
                if (!double.IsNaN(tempTotalValue))
                {
                    resultDT.Rows[i][resultDT.Columns.Count - 1] = tempTotalValue;
                }
            }
        }

        internal static void ConstrutDataTable(DateTime startTime, DateTime endTime, DataTable dt, int interval)
        {
            while (startTime <= endTime)
            {
                DataRow dr = dt.NewRow();
                dr[0] = startTime.ToString(DataManager.TimeFormatWithoutSecond) + "\r";
                startTime = startTime.AddSeconds(interval);
                dt.Rows.Add(dr);
            }
        }
    }
}
