using System;
using System.Collections.Generic;
using System.Data;
using CSharpDBPlugin;

//using CET.PecsNodeManage;
using DBInterfaceCommonLib;

namespace OfficeReportInterface
{
    /// <summary>
    ///定时记录管理类，进行过滤和重组结果集
    /// </summary>
    public class HistoryDataLogManager
    {
        public HistoryDataLogManager()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 唯一实例
        /// </summary>
        public static readonly HistoryDataLogManager DataManager = new HistoryDataLogManager();



        ///// <summary>
        ///// 查询任意时间范围的定时记录，不格式化结果集
        ///// </summary>
        ///// <param name="paramList"></param>
        ///// <param name="queryTime"></param>
        ///// <param name="queryFromBuffer">是否从缓存文件中获取数据，TRUE-从缓存文件获取数据(如果有)，FALSE-从数据库获取数据</param>
        ///// <returns></returns>
        //public List<DataLogValueOriList> GetHistoryDataByDuration(List<DeviceDataParam> paramList, DateTimeParam queryTime, bool queryFromBuffer)
        //{
        //    List<DataLogValueOriList> resultList = null;
        //    //创建并初始化结果集
        //    InitializeResultList(paramList, ref resultList);

        //    //创建用于保存最终查询条件映射关系的缓存字段，保存SourceID-DataIndex数组对象的映射关系
        //    Dictionary<uint, DataIDToMeasIDDef[]> sourceIDMap = new Dictionary<uint, DataIDToMeasIDDef[]>();
        //    //对查询参数进行合并优化，拥有相同SourceID的DataIndex查询合并为一次查询
        //    CombineQueryParamWithSourceID(sourceIDMap, paramList);                       

        //    DataTable tempDT = new DataTable();
        //    try
        //    {
        //        //基于最终的查询条件进行数据库查询,单SourceID进行查询
        //        foreach (KeyValuePair<uint, DataIDToMeasIDDef[]> kvp in sourceIDMap)
        //        {
        //            tempDT.Clear();
        //            //从数据库中获取查询结果集
        //            tempDT = GetDataLogTable(kvp, queryTime, queryFromBuffer);
        //            //添加查询数据到最终结果集中
        //            AddNewValueIntoResultList(tempDT, kvp.Value, resultList);
        //        }
        //    }
        //    finally
        //    {
        //        if (tempDT != null)
        //            tempDT.Dispose();
        //    }

        //    return resultList;
        //}          
        /// <summary>
        /// 查询任意时间范围的定时记录，不格式化结果集
        /// </summary>
        /// <param name="paramList"></param>
        /// <param name="queryTime"></param>
        /// <param name="queryFromBuffer">是否从缓存文件中获取数据，TRUE-从缓存文件获取数据(如果有)，FALSE-从数据库获取数据</param>
        /// <returns></returns>
        public List<DataLogValueOriList> GetHistoryDataByDuration(List<DeviceDataParam> paramList, DateTimeParam queryTime, bool queryFromBuffer)
        {
            List<DataLogValueOriList> resultList = null;
            //创建并初始化结果集
            InitializeResultList(paramList, ref resultList);
            //查询相关设备节点的映射记录
            EMSWebServiceManager.EMSWebManager.FindDataMapDef(paramList);
            try
            {
                for (int i = 0; i < paramList.Count; i++)
                {
                    List<DataLogOriDef> tempDT = new List<DataLogOriDef>();
                    DeviceDataParam deviceDataParam = paramList[i];
                    DATALOG_PRIVATE_MAP datalogNode;
                    if (EMSWebServiceManager.EMSWebManager.deviceDataIDToPrivateMap.TryGetValue(deviceDataParam, out datalogNode))
                    {
                        //从数据库中获取查询结果集
                        tempDT = HistoryDataLogBasic.DataManager.ReadDataLogFromDB2(datalogNode, queryTime, 0);
                        //添加查询数据到最终结果集中
                        AddNewValueIntoResultList(tempDT, datalogNode, resultList[i], queryTime.EndTime);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorInfoManager.Instance.WriteLogMessage(ex.ToString());
            }
            return resultList;
        }

        /// <summary>
        /// 根据参数列表创建并初始化结果集
        /// </summary>
        /// <param name="paramList">参数列表</param>
        /// <param name="resultList">结果集</param>
        private void InitializeResultList(List<DeviceDataParam> paramList, ref List<DataLogValueOriList> resultList)
        {
            resultList = new List<DataLogValueOriList>();
            for (int i = 0; i < paramList.Count; i++)
            {
                List<DataLogOriDef> newDataList = new List<DataLogOriDef>();
                DataLogValueOriList newValueList = new DataLogValueOriList();
                //根据设备获取时区，以分钟为单位
                newValueList.TimeZone = SystemNodeManager.DataManager.FindTimeZoneOffsetByDevice(paramList[i].DeviceID);//先默认为北京时间
                newValueList.DataList = newDataList;
                resultList.Add(newValueList);
            }
        }

        ///// <summary>
        ///// 根据查询参数对应获取对应的定时记录查询参数集合，并且按照SourceID进行查询参数合并
        ///// </summary>
        ///// <param name="sourceIDMap">定时记录查询映射集合</param>
        ///// <param name="queryParams">查询参数列表</param>
        //private void CombineQueryParamWithSourceID(Dictionary<uint, DataIDToMeasIDDef[]> sourceIDMap, List<DeviceDataParam> queryParams)
        //{
        //    for (int i = 0; i < queryParams.Count; i++)
        //    {
        //        DataIDToMeasIDDef resultMapDef = DataIDToMeasIDDef.InvalidMapDef;
        //        bool result = EMSWebServiceManager.EMSWebManager.FindDataMapDef(queryParams[i], out resultMapDef);
        //        if (result)
        //        {
        //            //创建映射值对象
        //            DataIDToMeasIDDef[] sourceMapValue = null;
        //            bool exist = sourceIDMap.TryGetValue(resultMapDef.SourceID, out sourceMapValue);
        //            if (exist)
        //            {
        //                //当前SourceID映射键已存在，则获取映射值对象，设置当前查询参数序号位置的映射值项
        //                sourceMapValue[i] = resultMapDef;
        //                //将设置后的映射值更新保存
        //                sourceIDMap[resultMapDef.SourceID] = sourceMapValue;
        //            }
        //            else
        //            {
        //                //当前SourceID映射键不存在，则新建映射值，有多少个参数，就设置多少个映射值项
        //                sourceMapValue = new DataIDToMeasIDDef[queryParams.Count];
        //                sourceMapValue[i] = resultMapDef;
        //                //加入映射关系中
        //                sourceIDMap.Add(resultMapDef.SourceID, sourceMapValue);
        //            }
        //        }
        //    }
        //}


        ///// <summary>
        ///// 查询定时记录结果集，不从缓存文件中获取数据
        ///// </summary>
        ///// <param name="kvp"></param>
        ///// <param name="queryTime"></param>
        ///// <param name="queryFromBuffer">是否从缓存文件中获取数据，TRUE-从缓存文件获取数据(如果有)，FALSE-从数据库获取数据</param>
        ///// <returns></returns>
        //private DataTable GetDataLogTable(KeyValuePair<uint, DataIDToMeasIDDef[]> kvp, DateTimeParam queryTime, bool queryFromBuffer)
        //{
        //    DataTable resultTable = new DataTable();

        //    //SourceID查询条件
        //    uint sourceIDKey = kvp.Key;
        //    //组合拥有相同SourceID的DataIndex参数                    
        //    uint stationID = GetStationID(kvp.Value);
        //    uint deviceID = GetDeviceID(kvp.Value);
        //    int dataIndex = GetDataIndex(kvp.Value);
        //    DataIDToMeasIDDef newDataIDParam = new DataIDToMeasIDDef(stationID, deviceID, sourceIDKey, dataIndex); 

        //    //对于无效值进行过滤                 
        //    if (stationID == 0 || dataIndex < 0)
        //        return resultTable;

        //    if (queryFromBuffer)
        //        resultTable = HistoryDataLogBasic.DataManager.ReadDataLogs(newDataIDParam, queryTime);
        //    else
        //        resultTable = HistoryDataLogBasic.DataManager.QueryDataLogByDuration(stationID, sourceIDKey, dataIndex, queryTime);        

        //    return resultTable;
        //}


        /// <summary>
        /// 获取厂站ID（等于0时无效）
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        private uint GetStationID(DataIDToMeasIDDef[] sourceValue)
        {
            uint stationID = 0;
            foreach (DataIDToMeasIDDef sourceValueItem in sourceValue)
            {
                //有效的sourceID
                if (sourceValueItem.SourceID > 0 && sourceValueItem.DataIndex > 0)
                {
                    stationID = sourceValueItem.StationID == stationID ? stationID : sourceValueItem.StationID;
                }
            }
            return stationID;
        }

        /// <summary>
        /// 获取设备ID（等于0时无效）
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        private uint GetDeviceID(DataIDToMeasIDDef[] sourceValue)
        {
            uint deviceID = 0;
            foreach (DataIDToMeasIDDef sourceValueItem in sourceValue)
            {
                //有效的deviceID
                if (sourceValueItem.SourceID > 0 && sourceValueItem.DataIndex > 0)
                {
                    deviceID = sourceValueItem.DeviceID == deviceID ? deviceID : sourceValueItem.DeviceID;
                }
            }
            return deviceID;
        }

        /// <summary>
        /// 获取数据列索引（小于0时无效，等于0时表示所有数据列，大于0时表示指定数据列）
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        private int GetDataIndex(DataIDToMeasIDDef[] sourceValue)
        {
            int dataIndex = -1;

            int dataIndexCount = 0;
            int dataIndexTemp = 0;
            foreach (DataIDToMeasIDDef sourceValueItem in sourceValue)
            {
                //有效的sourceID和DataIndex,形成DataIndex查询字符串
                if (sourceValueItem.SourceID > 0 && sourceValueItem.DataIndex > 0)
                {
                    dataIndexCount++;
                    dataIndexTemp = sourceValueItem.DataIndex;
                }
            }

            if (dataIndexCount == 0)
                dataIndex = -1;
            else if (dataIndexCount == 1)
                dataIndex = dataIndexTemp;
            else
                dataIndex = 0;

            return dataIndex;
        }

        ///// <summary>
        ///// 将DataTable中的数据保存到结果列表中，不进行结果集格式化
        ///// </summary>
        ///// <param name="resultTable">缓存表数据集</param>
        ///// <param name="sourceValue"></param>
        ///// <param name="resultList">结果列表</param>
        //private void AddNewValueIntoResultList(DataTable resultTable, DataIDToMeasIDDef[] sourceValue, List<DataLogValueOriList> resultList)
        //{
        //    if (resultTable == null)
        //        return;

        //    //遍历获取当前查询得到的原始值，将原始值放置于对应的查询结果列表中
        //    foreach (DataRow row in resultTable.Rows)
        //    {
        //        DateTime logtime = Convert.ToDateTime(row["LogTime"]);
        //        int msec = Convert.ToInt32(row["Msec"]);
        //        logtime = logtime.AddMilliseconds(msec);
        //        //获取当前有效的查询参数，依据该参数进行查询结果的取值
        //        for (int index = 0; index < sourceValue.Length; index++)
        //        {
        //            DataIDToMeasIDDef dataValueItem = sourceValue[index];
        //            if (dataValueItem.SourceID == 0 || dataValueItem.DataIndex <= 0)
        //                continue;

        //            //查找定位当前参数对应的最终结果列表
        //            List<DataLogOriDef> resultDataLogList = resultList[index].DataList;                   

        //            //将查询结果插入到结果集中
        //            double resultValue = GetValueFromDataRow(row, dataValueItem.DataIndex, dataValueItem.Cofficient, - 1);
        //            DataLogOriDef curDataLogValue = new DataLogOriDef(logtime, resultValue);
        //            resultDataLogList.Add(curDataLogValue);
        //        }
        //    }
        //}

        /// <summary>
        /// TODO 测试
        /// 将DataTable中的数据保存到结果列表中，不进行结果集格式化
        /// </summary>
        /// <param name="resultTable">缓存表数据集</param>
        /// <param name="sourceValue"></param>
        /// <param name="resultList">结果列表</param>
        private void AddNewValueIntoResultList(List<DataLogOriDef> dataList, DATALOG_PRIVATE_MAP datalogNode, DataLogValueOriList valueList, DateTime queryEndTime)
        {
            if (dataList == null || dataList.Count == 0)
                return;

            foreach (var item in dataList)
            {
                //DateTime logtime = Convert.ToDateTime(row["LogTime"]);
                //int msec = Convert.ToInt32(row["Msec"]);
                //logtime = logtime.AddMilliseconds(msec);

                //过滤掉不在查询范围内的数据
                if (item.LogTime > queryEndTime)
                    continue;
                //确保是排好序的，最后一个元素的时刻已经大于或等于这个时刻，就不对了
                if (valueList.DataList.Count >= 1 && valueList.DataList[valueList.DataList.Count - 1].LogTime >= item.LogTime)
                    continue;

                //将查询结果插入到结果集中
                double resultValue = GetValueFromDataRow(item.DataValue, datalogNode.coefficient, -1);
                DataLogOriDef curDataLogValue = new DataLogOriDef(item.LogTime, resultValue);
                valueList.DataList.Add(curDataLogValue);

            }
        }
        ///// <summary>
        ///// 从内存表单行记录中获取定时记录值
        ///// </summary>
        ///// <param name="row">内存表行记录</param>
        ///// <param name="dataIndex">定时记录列索引</param>
        ///// <param name="digiNum">保留小数位，为-1表示按原始数据返回</param>
        ///// <returns>定时记录值</returns>
        //private double GetValueFromDataRow(DataRow row, int dataIndex, double coeficient, int digiNum)
        //{
        //    //获取对应当前DataIndex的查询数据列名称,依据该名称进行取值
        //    string queryColName = String.Format("Data{0}", dataIndex);
        //    double resultValue = double.NaN;
        //    if (!Convert.IsDBNull(row[queryColName]))
        //    {
        //        resultValue = Convert.ToDouble(row[queryColName]);
        //        //进行无效值判断
        //        if (SysConstDef.NA.CompareTo(resultValue) == 0)
        //            resultValue = double.NaN;
        //    }
        //    //乘上乘系数
        //    resultValue = resultValue * coeficient;
        //    //保留指定位数的小数位
        //    if(digiNum>=0)
        //        resultValue = DataFormatManager.GetFormattedDoubleByDigits(resultValue, digiNum);
        //    return resultValue;
        //}

        /// <summary>
        /// TODO 测试
        /// 从内存表单行记录中获取定时记录值
        /// </summary>
        /// <param name="row">内存表行记录</param>
        /// <param name="dataIndex">定时记录列索引</param>
        /// <param name="digiNum">保留小数位，为-1表示按原始数据返回</param>
        /// <returns>定时记录值</returns>
        private double GetValueFromDataRow(double value, double coeficient, int digiNum)
        {
            //获取对应当前DataIndex的查询数据列名称,依据该名称进行取值
            double resultValue = double.NaN;
            if (!Convert.IsDBNull(value))
            {
                resultValue = value;
                //进行无效值判断
                if (SysConstDef.NA.CompareTo(resultValue) == 0)
                    resultValue = double.NaN;
            }
            //乘上乘系数
            resultValue = resultValue * coeficient;
            //保留指定位数的小数位
            if (digiNum >= 0)
                resultValue = DataFormatManager.GetFormattedDoubleByDigits(resultValue, digiNum);
            return resultValue;
        }

        /// <summary>
        /// 获取定时记录时间间隔，以分钟为单位
        /// </summary>
        /// <param name="oriTable"></param>
        /// <returns></returns>
        public int GetDataLogIntervalValue(DataTable oriTable)
        {
            int retValue = 0;
            int rowCount = oriTable.Rows.Count;

            //如果只有一个点，直接返回间隔为0
            if (rowCount < 2)
                return retValue;

            //如果有两个点，则直接连接两个点
            DateTime firstTime = new DateTime();
            DateTime secondTime = new DateTime();
            if (rowCount == 2)
            {
                firstTime = Convert.ToDateTime(oriTable.Rows[0]["logtime"]);
                secondTime = Convert.ToDateTime(oriTable.Rows[1]["logtime"]);
                TimeSpan ts = secondTime - firstTime;
                retValue = Convert.ToInt32(ts.TotalMinutes);
                if ((ts.TotalMinutes - retValue) >= 0.5)
                    retValue += 1;
                return retValue;
            }
            //如果有多个点的时候，则循环两两比较，如果间隔相等则取该间隔            
            for (int i = 0; i < rowCount; i++)
            {
                DateTime thisTime = Convert.ToDateTime(oriTable.Rows[i]["logtime"]);
                if (i == 0)
                {
                    firstTime = thisTime;
                    continue;
                }
                else if (i == 1)
                {
                    secondTime = thisTime;
                    continue;
                }
                else
                {
                    TimeSpan ts1 = secondTime - firstTime;
                    TimeSpan ts2 = thisTime - secondTime;
                    double offsetSeconds = ts1.TotalSeconds - ts2.TotalSeconds;
                    if (offsetSeconds >= -30 && offsetSeconds < 30)
                    {
                        double retrunValue = (ts1.TotalMinutes + ts2.TotalMinutes) / 2;
                        retValue = Convert.ToInt32(retrunValue);
                        if ((retrunValue - Convert.ToDouble(retValue)) >= 0.5)
                            retValue += 1;
                        return retValue;
                    }
                    else
                    {
                        firstTime = secondTime;
                        secondTime = thisTime;
                        continue;
                    }
                }
            }

            return retValue;
        }
  }
}