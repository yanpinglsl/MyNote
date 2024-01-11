
//using OfficeReportInterface.DefaultReportInterface.EnergyCost;

//namespace OfficeReportInterface
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Data;
//    using System.Linq;
//    using System.Web;
//    //using CET.PecsNodeManage;
//    using DBInterfaceCommonLib;
//    using PECSDBInterface;
//    using PowerQualityInterface;

//    /// <summary>
//    /// TOUReportDataManager 的摘要说明
//    /// </summary>
//    public class TOUReportDataManager
//    {
//        ///// <summary>
//        ///// 唯一数据管理实例对象
//        ///// </summary>
//        //public static readonly TOUReportDataManager DataManager = new TOUReportDataManager();

//        //private TOUReportDataManager()
//        //{

//        //}

//        //private static StationTOUProfile GetTOUProfileByStationID(uint stationID, ref List<int> tariffiIndex)
//        //{
//        //    StationTOUProfile stationTOUProfile = new StationTOUProfile();
//        //    //获取该厂站对应的TOU方案
//        //    for (int j = 0; j < ReportWebServiceManager.stationTOUProfileList.Count; j++)
//        //    {
//        //        if (ReportWebServiceManager.stationTOUProfileList[j].stationID == stationID)
//        //        {
//        //            stationTOUProfile = ReportWebServiceManager.stationTOUProfileList[j];
//        //            tariffiIndex = ReportWebServiceManager.stationTOUProfileList[j].tariffIndexList;
//        //            break;
//        //        }
//        //    }
//        //    return stationTOUProfile;
//        //}

//        ///// <summary>
//        ///// 定时记录类型每日增量报表
//        ///// </summary>
//        ///// <param name="startTime"></param>
//        ///// <param name="endTime"></param>
//        ///// <param name="parameters"></param>
//        ///// <param name="tariffiIndex"></param>
//        ///// <param name="resultDT"></param>
//        ///// <returns></returns>
//        //public void GetTOUDataLogDatas(DateTime startTime, DateTime endTime, string parameters, ref DataTable resultDT)
//        //{
//        //    CreateTOUDTColumns(ref resultDT);
//        //    List<uint> touDataIDList = new List<uint>();
//        //    touDataIDList.Add(SysConstDefinition.DATAIDKWH);
//        //    touDataIDList.Add(SysConstDefinition.DATAIDKVARH);
//        //    touDataIDList.Add(SysConstDefinition.DATAIDKVAH);
//        //    List<DeviceDataIDDef> queryParamList = GetEnergyDemandParaList(parameters, touDataIDList);

//        //    //根据每个设备每个参数获取TOU每日增量表格
//        //    GetTOUDayIntervalResult(startTime, endTime, queryParamList, ref resultDT);
//        //}

//        ///// <summary>
//        ///// 获取energy&demand报表数据
//        ///// </summary>
//        ///// <param name="startTime"></param>
//        ///// <param name="endTime"></param>
//        ///// <param name="queryParamList"></param>
//        ///// <param name="resultDT"></param>
//        //private void GetTOUDayIntervalResult(DateTime startTime, DateTime endTime, List<DeviceDataIDDef> queryParamList, ref DataTable resultDT)
//        //{
//        //    List<DataLogValueDef>[] tempResult = HistoryDataQueryManager.DataManager.QueryHistoryTrendDataLogView(queryParamList, startTime, endTime.AddSeconds(1));

//        //    //对于一个DeviceDataIDDef
//        //    for (int i = 0; i < queryParamList.Count; i++)
//        //    {
//        //        SysNode deviceNode = PecsNodeManager.PecsNodeInstance.GetDeviceNodeByID(queryParamList[i].DeviceID);
//        //        uint deviceNodeID = 0;
//        //        if (deviceNode == null)
//        //            continue;

//        //        deviceNodeID = deviceNode.NodeID;
//        //        //定义日报测点数据字典，通过时间索引来查找对应的数据
//        //        Dictionary<DateTime, object> dataValueMap = new Dictionary<DateTime, object>();
//        //        List<DataLogValueDef> dataLogList = tempResult[i];
//        //        //由于TOU是15分钟间隔，所以将定时记录中的非15分钟整点间隔提取出来，减少计算量
//        //        double tempValue = 0;
//        //        for (int j = 0; j < dataLogList.Count; j++)
//        //        {
//        //            if (tempValue < dataLogList[j].DataValue)
//        //                tempValue = dataLogList[j].DataValue;
//        //            if (dataLogList[j].Logtime.Minute % 15 == 0)
//        //            {
//        //                dataValueMap.Add(dataLogList[j].Logtime, tempValue);
//        //                tempValue = 0;
//        //            }
//        //        }

//        //        string deviceName = deviceNode == null ? string.Empty : deviceNode.NodeName;
//        //        string dataName = ReportWebServiceManager.ReportWebManager.GetParaNameByDataID(queryParamList[i].DataID);
//        //        string dataTypeName = DataTypeDefine.GetDataTypeName(queryParamList[i].DataTypeID);
//        //        uint stationID = deviceNode.ParentNode.ParentNode.NodeID;

//        //        List<int> tariffiIndex = new List<int>(); ;
//        //        StationTOUProfile stationTOUProfile = GetTOUProfileByStationID(stationID, ref tariffiIndex);
//        //        DateTime tempStartTime = startTime;
//        //        while (tempStartTime < endTime)
//        //        {
//        //            for (int k = 0; k < tariffiIndex.Count; k++)
//        //            {
//        //                DataRow dr = resultDT.NewRow();
//        //                dr["DeviceID"] = deviceNodeID;
//        //                dr["DeviceName"] = deviceName;
//        //                dr["DataID"] = queryParamList[i].DataID * 100 + queryParamList[i].LogicalDeviceIndex;
//        //                dr["DataName"] = dataName + "-loop" + queryParamList[i].LogicalDeviceIndex;

//        //                dr["TariffIndex"] = tariffiIndex[k];
//        //                dr["TariffName"] = FindTariffNameByIndex(stationTOUProfile, tariffiIndex[k]);
//        //                dr["LogTime"] = tempStartTime.ToString("yyyy-MM-dd HH:mm:ss");
//        //                DateTime tempTime = new DateTime();
//        //                bool isTrue = false;
//        //                dr["DataValue"] = GetMaxOrDiffValueByTime(tempStartTime, tempStartTime.AddDays(1), dataValueMap, tariffiIndex[k], StatisticType.StatisticTypeTotal, stationTOUProfile, ref tempTime, ref isTrue);
//        //                resultDT.Rows.Add(dr);
//        //            }
//        //            tempStartTime = tempStartTime.AddDays(1);
//        //        }
//        //    }
//        //}

//        #region Energy&Demand Of TOU

//        ///// <summary>
//        ///// 获取TOU方案的电度有关数据
//        ///// </summary>
//        ///// <param name="startTime"></param>
//        ///// <param name="endTime"></param>
//        ///// <param name="parameters"></param>
//        ///// <param name="resultDT"></param>
//        //public void GetEnergyDemandData(DateTime startTime, DateTime endTime, string deviceParameters, ref DataTable resultDT)
//        //{
//        //    CreateEnergyDTColumns(ref resultDT);
//        //    GetEnergyData(startTime, endTime, deviceParameters, ref resultDT);
//        //    GetDemandData(startTime, endTime, deviceParameters, ref resultDT);
//        //}

//        //private void GetEnergyData(DateTime startTime, DateTime endTime, string deviceParameters, ref DataTable resultDT)
//        //{
//        //    List<uint> energyDataIDList = new List<uint>();
//        //    energyDataIDList.Add(SysConstDefinition.DATAIDKWH);
//        //    energyDataIDList.Add(SysConstDefinition.DATAIDKVARH);
//        //    energyDataIDList.Add(SysConstDefinition.DATAIDKVAH);

//        //    List<DeviceDataIDDef> queryParamList = GetEnergyDemandParaList(deviceParameters, energyDataIDList);
//        //    GetEnergyDemandData(startTime, endTime, queryParamList, "Energy", StatisticType.StatisticTypeTotal, ref resultDT);
//        //}

//        //private void GetDemandData(DateTime startTime, DateTime endTime, string deviceParameters, ref DataTable resultDT)
//        //{
//        //    List<uint> energyDataIDList = new List<uint>();
//        //    energyDataIDList.Add(SysConstDefinition.DATAIDKW_DEMAND);
//        //    energyDataIDList.Add(SysConstDefinition.DATAIDKVAR_DEMAND);
//        //    energyDataIDList.Add(SysConstDefinition.DATAIDKVA_DEMAND);

//        //    List<DeviceDataIDDef> queryParamList = GetEnergyDemandParaList(deviceParameters, energyDataIDList);
//        //    GetEnergyDemandData(startTime, endTime, queryParamList, "Demand", StatisticType.StatisticTypeMax, ref resultDT);
//        //}

//        ///// <summary>
//        ///// 获取energy&demand报表数据
//        ///// </summary>
//        ///// <param name="startTime"></param>
//        ///// <param name="endTime"></param>
//        ///// <param name="queryParamList"></param>
//        ///// <param name="resultDT"></param>
//        //private void GetEnergyDemandData(DateTime startTime, DateTime endTime, List<DeviceDataIDDef> queryParamList, string category, StatisticType dataType, ref DataTable resultDT)
//        //{
//        //    List<DataLogValueDef>[] tempResult = HistoryDataQueryManager.DataManager.QueryHistoryTrendDataLogView(queryParamList, startTime, endTime.AddSeconds(1));

//        //    //对于一个DeviceDataIDDef
//        //    for (int i = 0; i < queryParamList.Count; i++)
//        //    {
//        //        SysNode deviceNode = PecsNodeManager.PecsNodeInstance.GetDeviceNodeByID(queryParamList[i].DeviceID);
//        //        if (deviceNode == null)
//        //            continue;
//        //        //定义日报测点数据字典，通过时间索引来查找对应的数据
//        //        Dictionary<DateTime, object> dataValueMap = new Dictionary<DateTime, object>();
//        //        List<DataLogValueDef> dataLogList = tempResult[i];
//        //        //由于TOU是15分钟间隔，所以将定时记录中的非15分钟整点间隔提取出来，减少计算量
//        //        double tempValue = 0;
//        //        for (int j = 0; j < dataLogList.Count; j++)
//        //        {
//        //            if (tempValue < dataLogList[j].DataValue)
//        //                tempValue = dataLogList[j].DataValue;
//        //            if (dataLogList[j].Logtime.Minute % 15 == 0)
//        //            {
//        //                dataValueMap.Add(dataLogList[j].Logtime, tempValue);
//        //                tempValue = 0;
//        //            }
//        //        }

//        //        uint deviceNodeID = deviceNode.NodeID;
//        //        string deviceName = deviceNode == null ? string.Empty : deviceNode.NodeName;
//        //        string dataName = ReportWebServiceManager.ReportWebManager.GetParaNameByDataID(queryParamList[i].DataID);
//        //        string dataTypeName = DataTypeDefine.GetDataTypeName(queryParamList[i].DataTypeID);
//        //        uint stationID = deviceNode.ParentNode.ParentNode.NodeID;

//        //        List<int> tariffiIndex = new List<int>();
//        //        StationTOUProfile stationTOUProfile = GetTOUProfileByStationID(stationID, ref tariffiIndex);

//        //        for (int k = 0; k < tariffiIndex.Count; k++)
//        //        {
//        //            DataRow dr = resultDT.NewRow();
//        //            dr["DeviceID"] = deviceNodeID;
//        //            dr["DeviceName"] = deviceName;
//        //            dr["DataID"] = queryParamList[i].DataID;
//        //            dr["DataName"] = dataName + "-loop" + queryParamList[i].LogicalDeviceIndex;

//        //            int tafiffIndex = tariffiIndex[k];
//        //            dr["TariffIndex"] = tafiffIndex;
//        //            DateTime maxValueTime = startTime;
//        //            dr["TariffName"] = FindTariffNameByIndex(stationTOUProfile, tariffiIndex[k]);
//        //            bool IsTrue = true;
//        //            double total = GetMaxOrDiffValueByTime(startTime, endTime, dataValueMap, tariffiIndex[k], dataType, stationTOUProfile, ref maxValueTime, ref IsTrue);
//        //            dr["IsTrue"] = IsTrue.ToString();
//        //            double cost = GetTOUTariffByDataID(stationTOUProfile.tariffProfileList[tafiffIndex - 1], queryParamList[i].DataID);
//        //            if (cost < 0.001)
//        //                continue;
//        //            dr["LogTime"] = maxValueTime.ToString("yyyy-MM-dd HH:mm:ss");
//        //            if (total < 0.001)//等于0的不显示
//        //                continue;
//        //            dr["Value"] = Math.Abs(total);
//        //            dr["Rate"] = Math.Abs(cost);
//        //            if (total.Equals(double.NaN))
//        //                dr["Tariff"] = 0;
//        //            else
//        //                dr["Tariff"] = DataFormatManager.GetFormattedDoubleByDigits(Math.Abs(total * cost), 3);
//        //            dr["Category"] = category;
//        //            dr["CurrencySymbol"] = stationTOUProfile.tariffProfileList[0].tariffUnit;
//        //            resultDT.Rows.Add(dr);
//        //        }
//        //    }
//        //}

//        ///// <summary>
//        ///// 获取电度相关参数列表
//        ///// </summary>
//        ///// <param name="parameters"></param>
//        ///// <returns></returns>
//        //public List<DeviceDataIDDef> GetEnergyDemandParaList(string parameters, List<uint> dataIDList)
//        //{
//        //    List<DeviceDataIDDef> queryParamList = new List<DeviceDataIDDef>();
//        //    List<string> deviceIDList = DataFormatManager.ParseStringList(parameters, ";");

//        //    for (int i = 0; i < deviceIDList.Count; i++)
//        //    {
//        //        string paraStrs = deviceIDList[i];
//        //        List<uint> paraIDs = DataFormatManager.ParseUIntList(paraStrs, ",");
//        //        List<NodeParam> nodeParamsList = NodeParam.GetNodeParamList(paraStrs);
               
//        //        //uint deviceID = Convert.ToUInt32(paraIDs[0]); 加上了自定义节点后就不能直接这样解析了
//        //        int logicalDeviceIndex = Convert.ToInt32(paraIDs[1]);

//        //        for (int j = 0; j < dataIDList.Count; j++)
//        //        {
//        //            uint dataID = dataIDList[j];
//        //            DeviceDataIDDef newParam = new DeviceDataIDDef(nodeParamsList[0].NodeType, nodeParamsList[0].NodeID, dataID, (int)DataIDParaType.DatalogType, DataTypeDefine.RealTimeVal, logicalDeviceIndex);
//        //            queryParamList.Add(newParam);
//        //        }
//        //    }
//        //    return queryParamList;
//        //}


//        ///// <summary>
//        ///// 根据参数名称名称获取费率
//        ///// </summary>
//        ///// <param name="tariffProfile">费率定义</param>
//        ///// <param name="dataName">参数名称</param>
//        ///// <returns></returns>
//        //private double GetTOUTariffByDataID(TariffProfileStruct tariffProfile, uint dataID)
//        //{
//        //    if (dataID == SysConstDefinition.DATAIDKWH)
//        //        return tariffProfile.kWhTariff;
//        //    else if (dataID == SysConstDefinition.DATAIDKVARH)
//        //        return tariffProfile.kvarhTariff;
//        //    else if (dataID == SysConstDefinition.DATAIDKVAH)
//        //        return tariffProfile.kVAhTariff;
//        //    else if (dataID == SysConstDefinition.DATAIDKW_DEMAND)
//        //        return tariffProfile.kWDemandTariff;
//        //    else if (dataID == SysConstDefinition.DATAIDKVAR_DEMAND)
//        //        return tariffProfile.kvarDemandTariff;
//        //    else
//        //        return tariffProfile.kVADemandTariff;
//        //}

//        #endregion

//        ///// <summary>
//        ///// 获取某个设备dataid对应的日报测点ID
//        ///// </summary>
//        ///// <param name="deviceNode"></param>
//        ///// <returns></returns>
//        //private static Dictionary<uint, uint> GetMeasureIDByDataID(PecsDeviceNode deviceNode)
//        //{
//        //    uint stationID = deviceNode.ParentNode.ParentNode.NodeID;
//        //    uint channelID = deviceNode.ParentNode.NodeID;
//        //    uint deviceID = deviceNode.NodeID;
//        //    //定义dataid，日报测点ID对应的字典
//        //    Dictionary<uint, uint> measureDataIDDic = new Dictionary<uint, uint>();
//        //    Dictionary<uint, uint> dataIDMeasureDic = new Dictionary<uint, uint>();
//        //    DataTable resultTable = new DataTable();
//        //    RealTimeProvider.Instance.ReadRealtimeStamps(stationID, channelID, deviceID, 0, ref resultTable);
//        //    for (int i = 0; i < resultTable.Rows.Count; i++)
//        //    {
//        //        uint dataID = 0;
//        //        uint measureID = 0;
//        //        if (Convert.IsDBNull(resultTable.Rows[i]["dataid"]))
//        //            continue;
//        //        if (Convert.IsDBNull(resultTable.Rows[i]["measureID"]))
//        //            continue;
//        //        measureID = Convert.ToUInt32(resultTable.Rows[i]["measureID"]);
//        //        dataID = Convert.ToUInt32(resultTable.Rows[i]["dataid"]);

//        //        for (int j = 0; j < deviceNode.DayRptNodeNum; j++)
//        //        {
//        //            PecsDayRptMeasNode dayRptNode = deviceNode.GetDayRptNode(j);
//        //            if (dayRptNode.MeasID == measureID)
//        //            {
//        //                measureDataIDDic.Add(dataID, dayRptNode.NodeID);
//        //                dataIDMeasureDic.Add(dayRptNode.NodeID, dataID);
//        //                break;
//        //            }
//        //        }
//        //    }
//        //    return measureDataIDDic;
//        //}

//        //private double GetMaxOrDiffValueByTime(DateTime startTime, DateTime endTime, Dictionary<DateTime, object> dataValueMap, int taffiIndex, StatisticType dataType, StationTOUProfile stationTOUProfile, ref DateTime maxValueTime, ref bool IsTrue)
//        //{
//        //    double result = 0;
//        //    DateTime firstEndTime = DateTime.Now;
//        //    DateTime tempMaxTime = startTime;
//        //    while (startTime < endTime)
//        //    {
//        //        //起始、结束时间的值
//        //        //获取查询起始时间和第二天的日时段，由于存在起始时间不是0点，所以要查询两天
//        //        int dayProfileIndex = FindPeriodByDateTime(startTime, stationTOUProfile);
//        //        //费率时段对应的的时间区间序列，由于存在跨天的情况，所以要把时间段分为两部分，起始时间-当天24点，24点-结束时间
//        //        //第一天的结束时间为当天24天
//        //        firstEndTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
//        //        firstEndTime = firstEndTime.AddDays(1);

//        //        List<DateTimePair> dateTimePairList = FindDateTimePairByDayProfileIndex(startTime, firstEndTime, dayProfileIndex, taffiIndex, stationTOUProfile.dayProfileList);
//        //        double value = GetTOUValueByDateTimePairList(startTime, dateTimePairList, dataType, dataValueMap, ref tempMaxTime, ref IsTrue);
//        //        result = GetDataValueByDataType(ref tempMaxTime, ref maxValueTime, value, result, dataType);
//        //        startTime = startTime.AddDays(1);
//        //    }

//        //    int nextDayProfileIndex = FindPeriodByDateTime(startTime.AddDays(1), stationTOUProfile);
//        //    firstEndTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
//        //    firstEndTime = firstEndTime.AddDays(1);
//        //    if (firstEndTime < endTime)
//        //    {
//        //        //对于非整点，要加上最后一天的非整点时段
//        //        List<DateTimePair> nexDaydateTimePairList = FindDateTimePairByDayProfileIndex(firstEndTime, endTime, nextDayProfileIndex, taffiIndex, stationTOUProfile.dayProfileList);
//        //        double nextDayValue = GetTOUValueByDateTimePairList(endTime, nexDaydateTimePairList, dataType, dataValueMap, ref tempMaxTime, ref IsTrue);
//        //        //两段都不为空，则返回相加
//        //        result = GetDataValueByDataType(ref tempMaxTime, ref maxValueTime, nextDayValue, result, dataType);
//        //    }
//        //    result = DataFormatManager.GetFormattedDoubleByDigits(result, 3);
//        //    return result;
//        //}

//        ///// <summary>
//        ///// 根据数据类型获取最大值或者差值
//        ///// </summary>
//        ///// <param name="value">当前值</param>
//        ///// <param name="result">返回结果</param>
//        ///// <param name="dataType">数据类型，最大值或者差值</param>
//        ///// <returns></returns>
//        //private double GetDataValueByDataType(ref DateTime tempMaxTime, ref DateTime maxValueTime, double value, double result, StatisticType dataType)
//        //{
//        //    if (dataType == StatisticType.StatisticTypeMax)//最大值
//        //    {
//        //        if (value != double.NaN)
//        //        {
//        //            if (result < value)
//        //            {
//        //                result = value;
//        //                maxValueTime = tempMaxTime;
//        //            }
//        //        }
//        //    }
//        //    else if (dataType == StatisticType.StatisticTypeTotal)//差值
//        //    {
//        //        if (value != double.NaN)
//        //        {
//        //            result = result + value;
//        //        }
//        //    }
//        //    return result;
//        //}

//        //private double GetTOUValueByDateTimePairList(DateTime startTime, List<DateTimePair> dateTimePairList, StatisticType dataType, Dictionary<DateTime, object> dataValueMap, ref DateTime maxValueTime, ref bool IsTrue)
//        //{
//        //    if (dataType == StatisticType.StatisticTypeMax)
//        //    {
//        //        return GetTOUMaxValueByDateTimePairList(startTime, dateTimePairList, dataValueMap, ref maxValueTime);
//        //    }
//        //    else
//        //    {
//        //        return GetTOUDiffValueByDateTimePairList(startTime, dateTimePairList, dataValueMap, ref IsTrue);
//        //    }
//        //}

//        ///// <summary>
//        ///// 根据费率段时间对获取单元格的值
//        ///// </summary>
//        ///// <param name="dateTimePairList"></param>
//        ///// <returns></returns>
//        //private double GetTOUDiffValueByDateTimePairList(DateTime startTime, List<DateTimePair> dateTimePairList, Dictionary<DateTime, object> dataValueMap, ref bool IsTrue)
//        //{
//        //    double value = 0;
//        //    foreach (DateTimePair timePair in dateTimePairList)
//        //    {
//        //        //获取单元格其实和结束时间的数据
//        //        DateTime starttime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
//        //        starttime = starttime.AddMinutes(timePair.startTimeMinute);
//        //        DateTime endtime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
//        //        endtime = endtime.AddMinutes(timePair.endTimeMinute);
//        //        //分别获取开始和结束时间的数据
//        //        object startvalue = null;
//        //        object endvalue = null;
//        //        dataValueMap.TryGetValue(starttime, out startvalue);
//        //        dataValueMap.TryGetValue(endtime, out endvalue);
//        //        DateTime tempStartTime = starttime;
//        //        DateTime tempEndTime = endtime;
//        //        while (startvalue == null)
//        //        {
//        //            IsTrue = false;
//        //            tempStartTime = tempStartTime.AddMinutes(15);
//        //            if (tempStartTime >= endtime)
//        //                break;
//        //            dataValueMap.TryGetValue(tempStartTime, out startvalue);
//        //        }
//        //        while (endvalue == null)
//        //        {
//        //            IsTrue = false;
//        //            tempEndTime = tempEndTime.AddMinutes(-15);
//        //            if (tempEndTime <= starttime)
//        //                break;
//        //            dataValueMap.TryGetValue(tempEndTime, out endvalue);
//        //        }
//        //        if (GetDiffValue(endvalue, startvalue).Equals(double.NaN))
//        //        {
//        //            //只要其中一段为空，则返回看空，宁可返回空也不返回错误数据
//        //            return 0;
//        //        }
//        //        else
//        //        {
//        //            //对于对个时间区间，数值累加
//        //            value = value + GetDiffValue(endvalue, startvalue);
//        //        }
//        //    }
//        //    return value;
//        //}

//        //private double GetTOUMaxValueByDateTimePairList(DateTime startTime, List<DateTimePair> dateTimePairList, Dictionary<DateTime, object> dataValueMap, ref DateTime maxValueTime)
//        //{
//        //    double result = 0;
//        //    foreach (DateTimePair timePair in dateTimePairList)
//        //    { //获取单元格其实和结束时间的数据
//        //        DateTime starttime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
//        //        starttime = starttime.AddMinutes(timePair.startTimeMinute);
//        //        DateTime endtime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
//        //        endtime = endtime.AddMinutes(timePair.endTimeMinute);
//        //        while (starttime < endtime)
//        //        {
//        //            object startvalue = null;
//        //            dataValueMap.TryGetValue(starttime, out startvalue);
//        //            if (startvalue != null)
//        //            {
//        //                if (result < Convert.ToDouble(startvalue))
//        //                {
//        //                    result = Convert.ToDouble(startvalue);
//        //                    maxValueTime = starttime;
//        //                }
//        //            }
//        //            starttime = starttime.AddMinutes(15);
//        //        }
//        //    }
//        //    return result;
//        //}

//        ///// <summary>
//        ///// 获取差值累计
//        ///// </summary>
//        ///// <param name="endValue">起始时间值</param>
//        ///// <param name="startValue">结束时间值</param>
//        ///// <returns></returns>
//        //private double GetDiffValue(object endValue, object startValue)
//        //{
//        //    try
//        //    {
//        //        if (endValue == null || startValue == null)
//        //            return 0;
//        //        return Convert.ToDouble(endValue) - Convert.ToDouble(startValue);
//        //    }
//        //    catch
//        //    {
//        //        return 0;
//        //    }
//        //}

//        ///// <summary>
//        ///// 根据查询时间到年费率方案中找到日费率段
//        ///// </summary>
//        ///// <param name="queryTime"></param>
//        ///// <param name="yearProfileList"></param>
//        ///// <returns></returns>
//        //private int FindPeriodByDateTime(DateTime queryTime, StationTOUProfile stationTOUProfile)
//        //{
//        //    // 获取查询时间在当年的第几天
//        //    int dayOfYear = queryTime.DayOfYear;
//        //    //年费率方案的个数
//        //    int yearTouNum = stationTOUProfile.yearProfileList.Count;
//        //    //查找查询时间对应的年方案，默认为最后一年
//        //    YearTOUProfile tempYearTOUProfile = stationTOUProfile.yearProfileList[yearTouNum - 1];
//        //    //老的方案
//        //    if (!stationTOUProfile.IsNewTou)
//        //    {
//        //        //偶数年
//        //        if (queryTime.Year % 2 == 0)
//        //        {
//        //            tempYearTOUProfile = stationTOUProfile.yearProfileList[0];
//        //        }
//        //    }
//        //    else
//        //    {
//        //        //寻找该年对应的年时段
//        //        for (int i = 0; i < yearTouNum; i++)
//        //        {
//        //            if (stationTOUProfile.yearProfileList[i].year == queryTime.Year)
//        //                tempYearTOUProfile = stationTOUProfile.yearProfileList[i];
//        //        }
//        //    }
//        //    //从3月1号开始，为了保持平闰年一致，3月1号从60开始
//        //    if (dayOfYear <= 59 || DateTime.IsLeapYear(queryTime.Year))
//        //    {
//        //        return tempYearTOUProfile.dayProfileList[dayOfYear - 1] + 1;
//        //    }
//        //    else
//        //    {
//        //        return tempYearTOUProfile.dayProfileList[dayOfYear] + 1;
//        //    }
//        //}

//        ///// <summary>
//        ///// 根据费率时段序号查找费率时段名称
//        ///// </summary>
//        ///// <param name="stationID">厂站ID</param>
//        ///// <param name="tariffIndex">费率段序号</param>
//        ///// <returns></returns>
//        //private string FindTariffNameByIndex(StationTOUProfile touProfile, int tariffIndex)
//        //{
//        //    string resultTariffName = string.Empty;
//        //    for (int i = 0; i < touProfile.tariffProfileList.Count; i++)
//        //    {
//        //        if (touProfile.tariffProfileList[i].tariffIndex == tariffIndex)
//        //            return touProfile.tariffProfileList[i].tariffName;
//        //    }
//        //    return resultTariffName;
//        //}

//        ///// <summary>
//        ///// 根据日时段方案解析该日时段的费率方案
//        ///// </summary>
//        ///// <param name="dayProfileIndex"></param>
//        ///// <param name="dayProfileStruct"></param>
//        ///// <returns></returns>
//        //private List<DateTimePair> FindDateTimePairByDayProfileIndex(DateTime startTime, DateTime endTime, int dayProfileIndex, int tariffiIndex, List<DayProfileStruct> dayProfileStruct)
//        //{
//        //    for (int i = 0; i < dayProfileStruct.Count; i++)
//        //    {
//        //        DayProfileStruct dayProfile = dayProfileStruct[i];
//        //        //根据日时段名称得到在年费率表中的index
//        //        if (dayProfile.periodIndex != dayProfileIndex)
//        //            continue;
//        //        List<TariffPeriodTime> tariffiPeriodList = dayProfile.periodTime;
//        //        for (int j = 0; j < tariffiPeriodList.Count; j++)
//        //        {
//        //            //费率时段等于
//        //            TariffPeriodTime tariffiPeriodTime = tariffiPeriodList[j];
//        //            //找到日时段下对应的费率时段，返回该费率时段所有时间区间与查询起始时间和结束时间之间的交集
//        //            if (tariffiPeriodTime.tariffIndex != tariffiIndex)
//        //                continue;
//        //            for (int k = 0; k < tariffiPeriodTime.periodTimeList.Count; k++)
//        //            {
//        //                DateTimePair timePair = tariffiPeriodTime.periodTimeList[k];
//        //                DateTime tempTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
//        //                //如果查询时间的起始时间大于区间时间，则取查询时间
//        //                DateTime tempStartTime = tempTime.AddMinutes(timePair.startTimeMinute);
//        //                DateTime tempEndTime = tempTime.AddMinutes(timePair.endTimeMinute);
//        //                if ((startTime >= tempEndTime) || (endTime <= tempStartTime))
//        //                {
//        //                    timePair.startTimeMinute = 0;
//        //                    timePair.endTimeMinute = 0;
//        //                }
//        //                //查询起始结束时间和TOU起始结束时间取交集
//        //                else if ((startTime <= tempStartTime) && (endTime < tempEndTime))
//        //                {
//        //                    timePair.endTimeMinute = endTime.Hour * 60 + endTime.Minute;
//        //                }
//        //                else if ((startTime > tempStartTime) && (endTime >= tempEndTime))
//        //                {
//        //                    timePair.startTimeMinute = startTime.Hour * 60 + startTime.Minute;
//        //                }
//        //                else
//        //                {
//        //                    timePair.startTimeMinute = startTime.Hour * 60 + startTime.Minute;
//        //                    timePair.endTimeMinute = endTime.Hour * 60 + endTime.Minute;
//        //                }
//        //            }
//        //            return tariffiPeriodTime.periodTimeList;
//        //        }
//        //    }
//        //    return new List<DateTimePair>();
//        //}

//        ///// <summary>
//        ///// 构造每日增量报表列
//        ///// </summary>
//        ///// <param name="resultDT"></param>
//        //private void CreateTOUDTColumns(ref DataTable resultDT)
//        //{
//        //    //设备ID
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("DeviceID", "UInt32"));
//        //    //设备名称
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("DeviceName", "String"));
//        //    //参数ID 
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("DataID", "String"));
//        //    //参数名称
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("DataName", "String"));
//        //    //费率时段ID 
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("TariffIndex", "String"));
//        //    //费率时段名称
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("TariffName", "String"));
//        //    //时间
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("LogTime", "String"));
//        //    //数据
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("DataValue", "Double"));
//        //}

//        ///// <summary>
//        ///// 电能报报表
//        ///// </summary>
//        ///// <param name="resultDT"></param>
//        //private void CreateEnergyDTColumns(ref DataTable resultDT)
//        //{
//        //    //设备ID
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("DeviceID", "UInt32"));
//        //    //设备名称
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("DeviceName", "String"));
//        //    //参数ID 
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("DataID", "String"));
//        //    //参数名称
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("DataName", "String"));
//        //    //费率时段ID 
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("TariffIndex", "String"));
//        //    //费率时段名称
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("TariffName", "String"));
//        //    //时间
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("LogTime", "String"));
//        //    //总量
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("Value", "Double"));
//        //    //是否为精确数字
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("IsTrue", "String"));
//        //    //费率
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("Rate", "Double"));
//        //    //费用
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("Tariff", "Double"));
//        //    //种类，Energy or Demand
//        //    resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("Category", "String"));
//        //    DataColumn column = new DataColumn();
//        //    column.DataType = System.Type.GetType("System.String");
//        //    column.ColumnName = "CurrencySymbol";
//        //    column.DefaultValue = "$";
//        //    resultDT.Columns.Add(column);
//        //}
//    }
//}