using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using BasicDataInterface.Models.Response;
using CET.PecsNodeManage;
using CSharpDBPlugin;
using DBInterfaceCommonLib;
using ErrorCode = DBInterfaceCommonLib.ErrorCode;
using OfficeReportInterface.DefaultReportInterface.IntelligentSafety;

namespace OfficeReportInterface
{
    /// <summary>
    ///暂态分析管理类
    /// </summary>
    public class RMSAnalysisManager
    {
        public RMSAnalysisManager()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 唯一实例
        /// </summary>
        public static readonly RMSAnalysisManager DataManager = new RMSAnalysisManager();
        /// <summary>
        /// 瞬变事件类型
        /// </summary>
        private const uint TRANSIENT_TYPE = 17;
        /// <summary>
        /// 电压变动事件类型
        /// </summary>
        public const uint SAG_SWELL_TYPE = 18;
        /// <summary>
        /// 老版本瞬变事件特征值的个数，不包含总幅值和总持续时间，所以是7个
        /// </summary>
        private const int OLD_TRANSIENT_COUNT = 7;
        /// <summary>
        /// 瞬变事件特征值的个数，由于添加了总幅值和总持续时间，所以变成9个了
        /// </summary>
        private const int TRANSIENT_COUNT = 9;
        /// <summary>
        /// 电压变动事件特征值的个数
        /// </summary>
        private const int SAG_SWELL_COUNT = 15;
        /// <summary>
        /// 电压变动起始事件
        /// </summary>
        private const int START_EVENT_CODE = 1;
        /// <summary>
        /// 电压变动结束事件
        /// </summary>
        private const int END_EVENT_CODE = 2;
        /// <summary>
        /// 暂升Code列表
        /// </summary>
        private static int[] SwellCodes = new int[] { 4, 8, 12, 16, 18, 22 };
        /// <summary>
        /// 暂降Code列表
        /// </summary>
        private static int[] SagCodes = new int[] { 3, 7, 11, 15, 19, 21 };
        /// <summary>
        /// 中断Code列表
        /// </summary>
        private static int[] InteruptCodes = new int[] { 6, 10, 14, 20 };
        /// <summary>
        /// 其他Code列表
        /// </summary>
        private static int[] OtherCodes = new int[] { 0, 1, 2, 5, 9, 13, 17 };

        /// <summary>
        /// 查询波形的默认偏移量为正负250毫秒
        /// </summary>
        private const int WAVE_OFFSET = 250;
        /// <summary>
        /// 短时电压变动Code列表
        /// </summary>
        private static int[] ShortDurationRMSVariationCodes = new int[] { 1, 2, 5, 9 };
        /// <summary>
        /// 长时电压变动Code列表
        /// </summary>
        private static int[] LongDurationRMSVariationCodes = new int[] { 13 };
        /// <summary>
        /// 缓存用户对应的SARFI曲线列表
        /// </summary>
        private Dictionary<uint, List<SARFIChartCurve>> userChartMap;

        /// <summary>
        /// SEMI-100s标准的curveID
        /// </summary>
        private const int SEMI100S_CURVE_ID = Int32.MaxValue;

        private List<uint> GetDeviceIds(List<NodeParam> nodeParams, uint userID)
        {
            List<uint> deviceIdList = new List<uint>();
            foreach (NodeParam nodeParam in nodeParams)
            {
                deviceIdList.Add(nodeParam.NodeID);
            }
            return deviceIdList;
        }

        private static SortedDictionary<DateTime, List<EventInformation>> GetSortedVariation17And18StartEventDic(Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18StartEventDic)
        {
            SortedDictionary<DateTime, List<EventInformation>> variation17and18StartEventDic =
                new SortedDictionary<DateTime, List<EventInformation>>();
            foreach (var item in _Variation18StartEventDic)
            {
                foreach (var valueItem in item.Value)
                {
                    foreach (var listItem in valueItem.Value)
                    {
                        List<EventInformation> tempList;
                        if (!variation17and18StartEventDic.TryGetValue(listItem.FullTime, out tempList))
                        {
                            tempList = new List<EventInformation>();
                            variation17and18StartEventDic.Add(listItem.FullTime, tempList);
                        }
                        tempList.Add(listItem);
                    }
                }
            }
            return variation17and18StartEventDic;
        }
        /// <summary>
        /// 获取所有的最坏事件。key是triggerTime，value是所有的最坏事件的eventId
        /// </summary>
        /// <param name="allEventsList"></param>
        /// <returns></returns>
        public static bool GetAllWorstCase(CurveType curveType,List<SagSwellEvent> allEventsList, out     Dictionary<string, List<uint>> worstCaseDic)
        {
            Dictionary<string, List<SagSwellEvent>> groupedDic = GetGroupedEvents(allEventsList);
            worstCaseDic=new Dictionary<string, List<uint>>();
            
            //分别确认每组的最坏事件
            foreach (var item in groupedDic)
            {
                List<SagSwellEvent> oneGroupList=item.Value;
                List<uint> eventIdList;
                GetWorstCaseForOneGroup(curveType,oneGroupList, out eventIdList);
                worstCaseDic.Add(item.Key, eventIdList);
            }
            return true;
        }

        /// <summary>
        /// 传入一组的所有事件，得出这组中的最坏事件
        /// </summary>
        /// <param name="curveType"></param>
        /// <param name="oneGroupList"></param>
        /// <param name="eventIdList"></param>
        /// <returns></returns>
        private static bool GetWorstCaseForOneGroup(CurveType curveType, List<SagSwellEvent> oneGroupList, out List<uint> eventIdList)
        {
            eventIdList = new List<uint>();

            if (SafetyDataManager.hasIEMSWeb)
            {
                foreach (var sagSwellEvent in oneGroupList)
                {
                    if (sagSwellEvent.IsWorstCase)
                        eventIdList.Add(sagSwellEvent.EventID);
                }
            }
            else
            {
                IWorstCaseManager worstCaseManager;
                if (!GetWorstCaseManagerObject(curveType, out worstCaseManager))
                    return false;
                List<SagSwellEvent> worsetCaseList = new List<SagSwellEvent>();
                worstCaseManager.GetWorstCase(oneGroupList, out worsetCaseList);

                AddEventIdList(eventIdList, worsetCaseList);
            }
            return true;
        }

        private static void AddEventIdList(List<uint> eventIdList, List<SagSwellEvent> worsetCaseList)
        {
            foreach (var item in worsetCaseList)
            {
                eventIdList.Add(item.EventID);
            }
        }

        public static bool GetWorstCaseManagerObject(CurveType curveType, out   IWorstCaseManager worstCaseManager)
        {
            worstCaseManager = new WorstCaseITICManager();
            if (curveType == CurveType.ITIC)
            {
                worstCaseManager = new WorstCaseITICManager();
                return true;
            }

            if (curveType == CurveType.SEMI100s)
            {
                worstCaseManager = new WorstCaseSEMI100sManager();
                return true;
            }
            return false;

        }


        /// <summary>
        /// 由List的事件转换为Dic保存的事件，key是TriggerTime
        /// </summary>
        /// <param name="allEventsList"></param>
        /// <returns>key是triggerTime，value是该triggerTime对应的所有事件</returns>
        private static Dictionary<string, List<SagSwellEvent>> GetGroupedEvents(List<SagSwellEvent> allEventsList)
        {
            Dictionary<string, List<SagSwellEvent>> resultDic=new Dictionary<string, List<SagSwellEvent>>();
            foreach (var item in allEventsList)
            {
                List<SagSwellEvent> oneGroupList;
                if (!resultDic.TryGetValue(item.TriggerTime, out oneGroupList))
                {
                    oneGroupList=new List<SagSwellEvent>();
                    resultDic.Add(item.TriggerTime,oneGroupList);
                }
                oneGroupList.Add(item);
            }
            return resultDic;
        }



        private static List<EventInformation> GetStartEventDt(SortedDictionary<DateTime, List<EventInformation>> variation17and18StartEventDic)
        {
            List<EventInformation> startEventDt = new List<EventInformation>();

            foreach (var item in variation17and18StartEventDic)
            {
                foreach (var valueItem in item.Value)
                {
                    startEventDt.Add(valueItem);
                }
            }
            //从大到小的顺序
            startEventDt.Reverse();
            return startEventDt;
        }

        private List<SagSwellEvent> GetSortedList(List<SagSwellEvent> eventList)
        {

            SortedDictionary<string, List<SagSwellEvent>> sortedDic = new SortedDictionary<string, List<SagSwellEvent>>();
            foreach (var item in eventList)
            {
                var fullTime = item.EventTime;
                List<SagSwellEvent> valueList;
                if (!sortedDic.TryGetValue(fullTime, out valueList))
                {
                    valueList = new List<SagSwellEvent>();
                    sortedDic.Add(fullTime, valueList);
                }
                valueList.Add(item);
            }
            List<SagSwellEvent> eventListResult = new List<SagSwellEvent>();
            foreach (var itemkey in sortedDic)
            {
                foreach (SagSwellEvent itemvalue in itemkey.Value)
                {
                    eventListResult.Add(itemvalue);
                }
            }
            List<SagSwellEvent> eventListResultMaxToMin = new List<SagSwellEvent>();
            int count = eventListResult.Count;
            for (int i = count - 1; i >= 0; --i)
            {
                eventListResultMaxToMin.Add(eventListResult[i]);
            }

            return eventListResultMaxToMin;
        }


        /// <summary>
        /// 对暂态事件进行分组
        /// </summary>
        /// <param name="eventList"></param>
        /// <returns></returns>
        private List<SagSwellEvent> GroupSagSwellEvent(List<SagSwellEvent> eventList, string groupInterval)
        {
            //先排序，从大到小
            eventList = GetSortedList(eventList);

            double[] groupIntervalInt = GetGroupInterval(groupInterval);

            List<SagSwellEvent> resultList = new List<SagSwellEvent>();
            DateTime triggerTime = DateTime.MinValue; //此字段用于遍历每条暂态事件，进行暂态事件分组
            for (int i = eventList.Count - 1; i >= 0; i--)
            {
                SagSwellEvent oneEvent = eventList[i];
                DateTime eventTime = Convert.ToDateTime(eventList[i].EventTime);
                if (triggerTime == DateTime.MinValue) //表明是新一组的时间，将触发时间设为
                {
                    triggerTime = eventTime;
                    oneEvent.TriggerTime = eventList[i].EventTime;
                }
                else //如果不为最小时刻，则需判断下一个事件与前一个事件的时间差是否超过1秒，并判断与组内第一条事件时间差是否超过3秒，如果超过了则将其编到下一组
                {
                    TimeSpan lastEventTS = eventTime - Convert.ToDateTime(eventList[i + 1].EventTime); //保存与上一条事件的时间差
                    TimeSpan firstEventTS = eventTime - triggerTime; //保存与组内第一条事件的时间差
                    if (lastEventTS.TotalSeconds <= groupIntervalInt[0] && firstEventTS.TotalSeconds <= groupIntervalInt[1]) //如果与上一条事件差不超过1秒，与组内第一条事件差不超过3秒，则编为同一组
                        oneEvent.TriggerTime = DataFormatManager.GetFormatTimeString(triggerTime);
                    else //否则编为下一组 
                    {
                        triggerTime = eventTime;
                        oneEvent.TriggerTime = eventList[i].EventTime;
                    }
                }
                resultList.Add(oneEvent);
            }
            resultList.Reverse(); //按时间降序排列
            return resultList;
        }

        /// <summary>
        /// 获取分组算法，默认与前一条事件时间差超过1秒，或与组内第一条事件时间差超过3秒则将当前事件编为下一组
        /// </summary>
        /// <param name="groupInterval"></param>
        /// <returns></returns>
        private double[] GetGroupInterval(string groupInterval)
        {
            double[] result = { 1, 3 };
            try
            {
                string[] tmpStr = groupInterval.Split(',');
                if (tmpStr.Length == 2)
                {
                    result[0] = Convert.ToDouble(tmpStr[0]);
                    result[1] = Convert.ToDouble(tmpStr[1]);
                }
            }
            catch (Exception ex)
            {
                ErrorInfoManager.Instance.WriteLogMessage("GetGroupInterval", ex);
            }
            return result;
        }

        /// <summary>
        /// 针对未找到结束事件的情况，调用存储过程获取，多次调用会存在性能问题
        /// </summary>
        /// <param name="sarfiValues"></param>
        /// <param name="sagSwellEvent"></param>
        /// <returns></returns>
        private SagSwellFeature QueryForMoreFeature(List<SARFIChartCurve> sarfiValues, SagSwellEvent sagSwellEvent, List<double> pointRangeList)
        {
            //uint deviceID = sagSwellEvent.DeviceID;
            //uint stationID = SystemNodeManager.DataManager.FindStationID(deviceID);
            //uint channelID = SystemNodeManager.DataManager.FindChannelID(deviceID);
            //DateTime startTime = Convert.ToDateTime(sagSwellEvent.EventTime);
            //DataTable queryResult = new DataTable();
            //int errorCode = EMSWebServiceProvider.Instance.QueryRMSNextEvent(DBOperationFlag.either, startTime, stationID, channelID, deviceID, sagSwellEvent.EventType, (int)sagSwellEvent.EventByte, sagSwellEvent.EventCode1, ref queryResult);
            //if (errorCode != (int)ErrorCode.Success)
            //    ErrorInfoManager.Instance.WriteDBInterfaceLog(errorCode, "EMSWebServiceProvider.QueryRMSNextEvent");

            SagSwellFeature featureInfo = new SagSwellFeature(false, false);
            //if (queryResult.Rows.Count == 1 && Convert.ToInt32(queryResult.Rows[0]["Code2"]) == 2)
            //    featureInfo = GetSagSwellFeatureFromDataRow(startTime, queryResult.Rows[0], sarfiValues, pointRangeList);
            return featureInfo;
        }

        /// <summary>
        /// 根据code1获取事件类型
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="eventCode1"></param>
        /// <param name="totalMag"></param>
        /// <returns></returns>
        public string GetEventTypeByCode1(int eventType, int eventCode1, double totalMag)
        {
            if (eventType == 17) return LocalResourceManager.GetInstance().GetString("1281", "Transient");
            else
            {
                if (SagCodes.Contains(eventCode1))
                { //Sag
                    return LocalResourceManager.GetInstance().GetString("1282", "Sag");
                }
                else if (SwellCodes.Contains(eventCode1))
                { //Swell
                    return LocalResourceManager.GetInstance().GetString("1283", "Swell");
                }
                else if (InteruptCodes.Contains(eventCode1))
                { //Interruption
                    return LocalResourceManager.GetInstance().GetString("1284", "Interruption");
                }
                else
                { //无法判断的情况根据幅值判断，如果是0.1到1则是暂降，大于1为暂升，小于0.1为中断
                    if (totalMag < 10)
                        return LocalResourceManager.GetInstance().GetString("1284", "Interruption");
                    else if (10 <= totalMag && totalMag < 100)
                        return LocalResourceManager.GetInstance().GetString("1282", "Sag");
                    else if (totalMag > 100)
                        return LocalResourceManager.GetInstance().GetString("1283", "Swell");
                    else
                    {
                        if (ShortDurationRMSVariationCodes.Contains(eventCode1))
                            return LocalResourceManager.GetInstance().GetString("1352", "Short Duration RMS Variation");
                        else if (LongDurationRMSVariationCodes.Contains(eventCode1))
                            return LocalResourceManager.GetInstance().GetString("1353", "Long Duration RMS Variation");
                        else
                            return LocalResourceManager.GetInstance().GetString("1354", "RMS Variation");
                    }
                }
            }
        }

        /// <summary>
        /// 根据特征值中的OutOfLimit字段以及选中的容忍度标准判断事件标记点的区域
        /// </summary>
        /// <param name="outOfLimit"></param>
        /// <param name="curveType"></param>
        /// <returns></returns>
        public string GetRegionTypeByID(int outOfLimit, uint curveType)
        {
            if (curveType == 1) //说明是在ITIC中
            {
                if (outOfLimit == -1)
                    return LocalResourceManager.GetInstance().GetString("1261", "Others");
                else if (outOfLimit == 0)
                    return LocalResourceManager.GetInstance().GetString("1262", "Region A");
                else if (outOfLimit == 1)
                    return LocalResourceManager.GetInstance().GetString("1263", "Region B");
                else if (outOfLimit == 2)
                    return LocalResourceManager.GetInstance().GetString("1264", "Region C");
                else return "Unknown Region";
            }
            else //说明是在SEMI中
            {
                if (outOfLimit == -1)
                    return LocalResourceManager.GetInstance().GetString("1261", "Others");
                else if (outOfLimit == 0)
                    return LocalResourceManager.GetInstance().GetString("1262", "Region A");
                else if (outOfLimit == 1)
                    return LocalResourceManager.GetInstance().GetString("1264", "Region C");
                else if (outOfLimit == 2)
                    return LocalResourceManager.GetInstance().GetString("1263", "Region B");
                else return "Unknown Region";
            }
        }

        /// <summary>
        /// 写入特征值到数据库中的相应事件中
        /// </summary>
        /// <param name="features">特征值列表</param>
        /// <param name="eventID">事件ID</param>
        /// <param name="eventTime">事件时间</param>
        /// <returns></returns>
        private EMSErrorMsg WriteRMSEventStr(List<double> features, uint eventID, DateTime eventTime)
        {
            EMSErrorMsg resultMsg = new EMSErrorMsg(true);
            string eventStr = string.Empty;
            for (int i = 0; i < features.Count; i++)
            {
                string feaStr = string.Empty;
                if (!double.IsNaN(features[i]))
                    feaStr = features[i].ToString();
                if (i == 0)
                    eventStr = feaStr;
                else
                    eventStr += ";" + feaStr;
            }

            int errorCode = EMSWebServiceProvider.Instance.WriteRMSEventStr(eventID, eventTime.Year, eventStr);
            if (errorCode != (int)ErrorCode.Success)
                ErrorInfoManager.Instance.WriteDBInterfaceLog(errorCode, "WriteEventStr");

            return resultMsg;

        }

        /// <summary>
        /// 根据暂态事件类型获取类型名称
        /// </summary>
        /// <param name="rmsType">暂态事件类型</param>
        /// <returns></returns>
        public string GetRMSEventTypeName(int rmsType)
        {
            RMSEventType rmsEventType = (RMSEventType)rmsType;
            switch (rmsEventType)
            {
                case RMSEventType.Transient:
                    return LocalResourceManager.GetInstance().GetString("1257", "Transient");
                case RMSEventType.Variation:
                    return LocalResourceManager.GetInstance().GetString("1256", "Voltage Variation");
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取事件统计总数
        /// </summary>
        /// <param name="eventCounts"></param>
        /// <returns></returns>
        private int GetTotalValue(int[] eventCounts)
        {
            int totalCount = 0;
            foreach (int itemCount in eventCounts)
                totalCount += itemCount;
            return totalCount;
        }

        /// <summary>
        /// 获取事件统计值数组
        /// </summary>
        /// <param name="rmsEventTypes">暂态事件类型列表</param>
        /// <param name="nodeDeviceIDsMap">PQ节点与关联设备的映射关系</param>
        /// <param name="deviceCountsMap">设备与相关事件统计值的映射关系</param>
        /// <param name="pqNode"></param>
        /// <returns></returns>
        private int[] GetEventCountsArray(List<int> rmsEventTypes, List<uint> deviceIDs, Dictionary<uint, int[]> deviceCountsMap)
        {
            int[] eventCounts = new int[rmsEventTypes.Count];
            foreach (uint deviceID in deviceIDs)
            {
                int[] counts;
                bool hasValue = deviceCountsMap.TryGetValue(deviceID, out counts);
                if (!hasValue || counts.Length != eventCounts.Length)
                    continue;

                for (int i = 0; i < eventCounts.Length; i++)
                    eventCounts[i] += counts[i];
            }
            return eventCounts;
        }

        /// <summary>
        /// 获取设备ID与相关事件统计值的的映射关系，事件统计值顺序与输入的暂态事件类型对应
        /// </summary>
        /// <param name="rmsEventTypes">暂态事件类型</param>
        /// <param name="queryResult">数据库查询结果</param>
        /// <returns></returns>
        private Dictionary<uint, int[]> GetDeviceCountsMap(List<int> rmsEventTypes, DataTable queryResult)
        {
            Dictionary<uint, int[]> deviceCountsMap = new Dictionary<uint, int[]>();
            foreach (DataRow dr in queryResult.Rows)
            {
                uint deviceID = Convert.ToUInt32(dr["DeviceID"]);
                int eventType = Convert.ToInt32(dr["EventType"]);
                int count = Convert.ToInt32(dr["EventCount"]);

                int[] typeCounts;
                bool hasValue = deviceCountsMap.TryGetValue(deviceID, out typeCounts);
                if (!hasValue)
                    typeCounts = new int[rmsEventTypes.Count];

                int rmsType = (int)GetRmsTypeByEventTypeCode(eventType);
                int index = rmsEventTypes.IndexOf(rmsType);
                typeCounts[index] += count;
                deviceCountsMap[deviceID] = typeCounts;
            }
            return deviceCountsMap;
        }

        /// <summary>
        /// 获取所有关联的设备ID列表
        /// </summary>
        /// <param name="nodeDeviceIDsMap"></param>
        /// <returns></returns>
        private List<uint> GetDeviceIDListByMap(Dictionary<SysNode, List<uint>> nodeDeviceIDsMap)
        {
            List<uint> deviceIDList = new List<uint>();
            foreach (List<uint> listItem in nodeDeviceIDsMap.Values)
            {
                foreach (uint deviceID in listItem)
                {
                    if (!deviceIDList.Contains(deviceID))
                        deviceIDList.Add(deviceID);
                }
            }
            return deviceIDList;
        }

        /// <summary>
        /// 从特征值字符串中获取特征值列表
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="dr">事件行</param>
        /// <param name="featureList">返回特征值列表</param>
        /// <returns>是否解析成功</returns>
        private bool GetFeatureListFromStr(int eventType, DataRow dr, ref List<double> featureList)
        {
            bool result = false;
            featureList = new List<double>();
            //找到特征值字符串     
            string eventStr = string.Empty;
            if (!Convert.IsDBNull(dr["EveStr1"]))
                eventStr = Convert.ToString(dr["EveStr1"]);

            List<string> featureStrs = DataFormatManager.ParseStringList(eventStr, ";");
            if (eventType == (int)SAG_SWELL_TYPE)
            {
                //总特征幅值(%);持续时间(ms);基准电压值(V);A相最小幅值(%);A相最大幅值(%);A相平均幅值(%);A相能量(%);B相最小幅值(%);B相最大幅值(%);
                //B相平均幅值(%);B相能量(%);C相最小幅值(%);C相最大幅值(%);C相平均幅值(%);C相能量(%)
                if (featureStrs.Count != SAG_SWELL_COUNT)
                    return result;
            }
            else if (eventType == (int)TRANSIENT_TYPE)
            {
                //瞬变基准电压;瞬变A相电压持续时间;瞬变A相电压最大幅值(%);瞬变B相电压持续时间;
                //瞬变B相电压最大幅值(%);瞬变C相电压持续时间;瞬变C相电压最大幅值(%)
                if (featureStrs.Count != TRANSIENT_COUNT && featureStrs.Count != OLD_TRANSIENT_COUNT)
                    return result;
            }
            else
                return result;

            try
            {
                //默认为false，特征值中只要有一个不为空就说明是true，不再需要从定时记录表中获取数据
                result = false;
                foreach (string itemStr in featureStrs)
                {
                    if (string.IsNullOrWhiteSpace(itemStr))
                        featureList.Add(double.NaN);
                    else
                    {
                        featureList.Add(Convert.ToDouble(itemStr));
                        result = true;
                    }
                }
            }
            catch
            {
                result = false;
            }

            //针对ION7650等装置没有总特征幅值的情况，在此处进行计算
            if (eventType == (int)SAG_SWELL_TYPE && featureList.Count == SAG_SWELL_COUNT)
                featureList[0] = GetTotalMagFromFeatures(featureList);

            return result;
        }
        /// <summary>
        /// 从特征值字符串中获取特征值列表
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="dr">事件行</param>
        /// <param name="featureList">返回特征值列表</param>
        /// <returns>是否解析成功</returns>
        private bool GetFeatureListFromStr(int eventType, EventInformation dr, ref List<double> featureList)
        {
            bool result = false;
            featureList = new List<double>();
            //找到特征值字符串     
            //string eventStr = string.Empty;
            //if (!Convert.IsDBNull(dr["EveStr1"]))
            //    eventStr = Convert.ToString(dr["EveStr1"]);
            string eventStr = dr.EveStr1;

            List<string> featureStrs = DataFormatManager.ParseStringList(eventStr, ";");
            if (eventType == (int)SAG_SWELL_TYPE)
            {
                //总特征幅值(%);持续时间(ms);基准电压值(V);A相最小幅值(%);A相最大幅值(%);A相平均幅值(%);A相能量(%);B相最小幅值(%);B相最大幅值(%);
                //B相平均幅值(%);B相能量(%);C相最小幅值(%);C相最大幅值(%);C相平均幅值(%);C相能量(%)
                if (featureStrs.Count != SAG_SWELL_COUNT)
                    return result;
            }
            else if (eventType == (int)TRANSIENT_TYPE)
            {
                //瞬变基准电压;瞬变A相电压持续时间;瞬变A相电压最大幅值(%);瞬变B相电压持续时间;
                //瞬变B相电压最大幅值(%);瞬变C相电压持续时间;瞬变C相电压最大幅值(%)
                if (featureStrs.Count != TRANSIENT_COUNT && featureStrs.Count != OLD_TRANSIENT_COUNT)
                    return result;
            }
            else
                return result;

            try
            {
                //默认为false，特征值中只要有一个不为空就说明是true，不再需要从定时记录表中获取数据
                result = false;
                foreach (string itemStr in featureStrs)
                {
                    if (string.IsNullOrWhiteSpace(itemStr))
                        featureList.Add(double.NaN);
                    else
                    {
                        featureList.Add(Convert.ToDouble(itemStr));
                        result = true;
                    }
                }
            }
            catch
            {
                result = false;
            }

            //针对ION7650等装置没有总特征幅值的情况，在此处进行计算
            if (eventType == (int)SAG_SWELL_TYPE && featureList.Count == SAG_SWELL_COUNT)
                featureList[0] = GetTotalMagFromFeatures(featureList);

            return result;
        }

        private List<double> GetFeatureList(List<DataLogValueOriList> datalogList, List<int> indexes, int eventType)
        {
            List<double> featureList = new List<double>();
            if (datalogList.Count != indexes.Count)
                return featureList;

            for (int i = 0; i < datalogList.Count; i++)
            {
                if (indexes[i] < datalogList[i].DataList.Count && indexes[i] >= 0)
                    featureList.Add(datalogList[i].DataList[indexes[i]].DataValue);
                else
                    featureList.Add(double.NaN);
            }

            //针对ION7650等装置没有总特征幅值的情况，在此处进行计算
            if (eventType == (int)SAG_SWELL_TYPE && featureList.Count == SAG_SWELL_COUNT)
                featureList[0] = GetTotalMagFromFeatures(featureList);

            return featureList;
        }

        /// <summary>
        /// 获取幅值偏离最大的幅值所在的索引位置
        /// </summary>
        /// <param name="mags"></param>
        /// <returns></returns>
        private int GetMaxMagIndex(double[] mags)
        {
            List<double> dvalList = new List<double>();
            foreach (double mag in mags)
            {
                double val = GetMagDValue(mag);
                dvalList.Add(val);
            }

            int maxIndex = 0;
            double maxVal = 0;
            for (int i = 0; i < dvalList.Count; i++)
            {
                if (double.IsNaN(dvalList[i]))
                    continue;
                if (maxVal < dvalList[i])
                {
                    maxVal = dvalList[i];
                    maxIndex = i;
                }
            }
            return maxIndex;
        }

        /// <summary>
        /// 获取幅值偏离程度
        /// </summary>
        /// <param name="mag"></param>
        /// <returns></returns>
        private double GetMagDValue(double mag)
        {
            double val = mag - 100;
            if (val < 0)
                val = -1 * val;
            return val;
        }

        /// <summary>
        /// 查找目标定时记录所在的索引位置
        /// </summary>
        /// <param name="featureTime"></param>
        /// <param name="datalogList"></param>
        /// <returns></returns>
        private List<int> GetValueIndexList(int eventType, DateTime featureTime, List<DataLogValueOriList> datalogList)
        {
            List<int> indexList = new List<int>();

            foreach (DataLogValueOriList dataList in datalogList)
            {
                int index = -1;
                double totalms = double.MaxValue;
                for (int i = 0; i < dataList.DataList.Count; i++)
                {
                    TimeSpan ts = dataList.DataList[i].LogTime - featureTime;

                    //经过与黄俊沟通，瞬变特征值定时记录时标应与事件时间一致
                    if (eventType == (int)TRANSIENT_TYPE && ts.TotalMilliseconds == 0)
                    {
                        index = i;
                        break;
                    }

                    //经过与黄俊沟通，电压变动特征值定时记录时标与结束事件时间偏差在20ms（左右）内，取最接近事件时间的那个数据                                            
                    if (eventType == (int)SAG_SWELL_TYPE && Math.Abs(ts.TotalMilliseconds) <= 20)
                    {
                        if (Math.Abs(ts.TotalMilliseconds) < totalms)
                        {
                            index = i;
                            totalms = Math.Abs(ts.TotalMilliseconds);
                        }
                    }
                }
                indexList.Add(index);
            }
            return indexList;
        }

        /// <summary>
        /// 获取暂态事件类型名称
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="eventCode"></param>
        /// <returns></returns>
        private string GetEventTypeName(int eventType)
        {
            string resultStr = string.Empty;

            RMSEventType rmsType = GetRmsTypeByEventTypeCode(eventType);
            resultStr = GetRMSEventTypeName((int)rmsType);

            return resultStr;
        }

        /// <summary>
        /// 获取匹配的事件行，要求为比输入的起始时间大的下一个事件
        /// </summary>
        /// <param name="startEventTime">起始事件时间</param>
        /// <param name="rowList">行结果集</param>
        /// <returns></returns>
        private DataRow GetFormatEventRow(DateTime startEventTime, List<DataRow> rowList)
        {
            DataRow tempdr = null;
            for (int i = rowList.Count - 1; i >= 0; i--)
            {
                DateTime endEventTime = GetDateTimeFromEventRow(rowList[i]);
                if (endEventTime > startEventTime)
                {
                    tempdr = rowList[i];
                    break;
                }
            }
            return tempdr;
        }

        /// <summary>
        /// 获取匹配的事件行，要求为比输入的起始时间大的下一个事件
        /// </summary>
        /// <param name="startEventTime">起始事件时间</param>
        /// <param name="rowList">行结果集</param>
        /// <returns></returns>
        private EventInformation GetFormatEventRow(DateTime startEventTime, List<EventInformation> rowList)
        {
            EventInformation tempdr = null;
            for (int i = rowList.Count - 1; i >= 0; i--)
            {
                DateTime endEventTime = GetDateTimeFromEventRow(rowList[i]);
                if (endEventTime > startEventTime)
                {
                    tempdr = rowList[i];
                    break;
                }
            }
            return tempdr;
        }

        /// <summary>
        /// 从事件结果行中获取事件时间（包括毫秒数）
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private DateTime GetDateTimeFromEventRow(DataRow item)
        {
            DateTime eventTime = Convert.ToDateTime(item["EventTime"]);
            eventTime = eventTime.AddMilliseconds(Convert.ToInt32(item["Msec"]));
            return eventTime;
        }

        /// <summary>
        /// 从事件结果行中获取事件时间（包括毫秒数）
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private DateTime GetDateTimeFromEventRow(EventInformation item)
        {
            return item.FullTime;
        }

        private Dictionary<SagSwellHandle, List<DataRow>> GetStartEndEventMap(DataTable queryResult)
        {
            Dictionary<SagSwellHandle, List<DataRow>> startEndEventMap = new Dictionary<SagSwellHandle, List<DataRow>>();
            foreach (DataRow dr in queryResult.Rows)
            {
                if (Convert.ToUInt32(dr["EventType"]) == TRANSIENT_TYPE)
                    continue;

                SagSwellHandle sagSwellItem = new SagSwellHandle(Convert.ToUInt32(dr["DeviceID"]));
                sagSwellItem.stationFlag = Convert.ToByte(dr["StationFlag"]);
                sagSwellItem.eventType = Convert.ToInt32(dr["EventType"]);
                sagSwellItem.eventByte = Convert.ToInt32(dr["EventByte"]);
                sagSwellItem.code1 = Convert.ToInt32(dr["Code1"]);

                List<DataRow> rowList;
                bool hasRows = startEndEventMap.TryGetValue(sagSwellItem, out rowList);
                if (!hasRows)
                    rowList = new List<DataRow>();
                rowList.Add(dr);
                startEndEventMap[sagSwellItem] = rowList;
            }
            return startEndEventMap;
        }

        private DataTable QuerySagSwellEndEvent(DataTable rmsDt, List<uint> deviceIDs, List<int> eventCodes)
        {
            DateTime rmsStartTime = GetRecordStartTime(rmsDt, (int)SAG_SWELL_TYPE);
            DateTime rmsEndTime = GetRecordEndTime(rmsDt, (int)SAG_SWELL_TYPE);

            rmsEndTime = rmsEndTime.AddHours(1);//结束时间扩大一小时
            int startOrEndEvent = 2; //只查询结束事件
            int queryType = (int)SAG_SWELL_TYPE;//只查询电压变动事件
            int maxRowCount = 0;//不限制查询结果记录条数
            DataTable queryResult = new DataTable();
            int errorCode = EMSWebServiceProvider.Instance.QueryRMSEventLogs(DBOperationFlag.either, deviceIDs.ToArray(), rmsStartTime, rmsEndTime, queryType, eventCodes.ToArray(), startOrEndEvent, 0, maxRowCount, ref queryResult);
            if (errorCode != (int)ErrorCode.Success)
                ErrorInfoManager.Instance.WriteDBInterfaceLog(errorCode, "EMSWebServiceProvider.QueryRMSEventLogs");
            return queryResult;
        }

        /// <summary>
        /// 查询结果集中相应事件类型的结束时间
        /// </summary>
        /// <param name="rmsDt"></param>
        /// <param name="eventType">事件类型，为0表示不限制</param>
        /// <returns></returns>
        private static DateTime GetRecordEndTime(DataTable rmsDt, int eventType)
        {
            DateTime rmsEndTime = DataFormatManager.GetMinDateTime();
            for (int i = 0; i < rmsDt.Rows.Count; i++)
            {
                if (Convert.ToInt32(rmsDt.Rows[i]["EventType"]) == eventType || eventType == 0)
                {
                    rmsEndTime = Convert.ToDateTime(rmsDt.Rows[i]["EventTime"]);
                    rmsEndTime = rmsEndTime.AddMilliseconds(Convert.ToInt32(rmsDt.Rows[i]["Msec"]));
                    break;
                }
            }
            return rmsEndTime;
        }

        /// <summary>
        /// 查询结果集中相应事件类型的起始时间
        /// </summary>
        /// <param name="rmsDt"></param>
        /// <param name="eventType">事件类型，为0表示不限制</param>
        /// <returns></returns>
        private DateTime GetRecordStartTime(DataTable rmsDt, int eventType)
        {
            DateTime rmsStartTime = DataFormatManager.GetMinDateTime();
            for (int i = rmsDt.Rows.Count - 1; i > 0; i--)
            {
                if (Convert.ToInt32(rmsDt.Rows[i]["EventType"]) == eventType || eventType == 0)
                {
                    rmsStartTime = Convert.ToDateTime(rmsDt.Rows[i]["EventTime"]);
                    rmsStartTime = rmsStartTime.AddMilliseconds(Convert.ToInt32(rmsDt.Rows[i]["Msec"]));
                    break;
                }
            }
            return rmsStartTime;
        }

        /// <summary>
        /// 根据暂态事件类型，转换成查询的Code类型
        /// </summary>
        /// <param name="rmsEventTypes">暂态事件类型列表</param>
        /// <returns></returns>
        private List<int> GetQueryEventCodes(List<int> rmsEventTypes)
        {
            List<int> eventCodes = new List<int>();
            foreach (int rmsType in rmsEventTypes)
            {
                switch (rmsType)
                {
                    case (int)RMSEventType.Variation:
                        eventCodes.AddRange(SagCodes);
                        eventCodes.AddRange(SwellCodes);
                        eventCodes.AddRange(InteruptCodes);
                        eventCodes.AddRange(OtherCodes);
                        break;
                }
            }
            eventCodes.Sort();
            return eventCodes;
        }

        /// <summary>
        /// 转换查询事件类型，17-瞬变，18-电压变动，0-所有暂态事件
        /// </summary>
        /// <param name="rmsEventTypes">原始事件列表</param>
        /// <returns></returns>
        private int GetQueryEventType(List<int> rmsEventTypes)
        {
            int queryType = 0;

            if (rmsEventTypes.Count == 1 && rmsEventTypes[0] == (int)RMSEventType.Transient)
                queryType = (int)TRANSIENT_TYPE;
            if (rmsEventTypes.Count == 1 && rmsEventTypes[0] == (int)RMSEventType.Variation)
                queryType = (int)SAG_SWELL_TYPE;

            return queryType;
        }

        /// <summary>
        /// 根据事件类型和事件Code转换为暂态事件类型
        /// </summary>
        /// <param name="eventType">事件类型</param>     
        /// <returns>暂态事件类型</returns>
        private RMSEventType GetRmsTypeByEventTypeCode(int eventType)
        {
            RMSEventType resultType = RMSEventType.Variation;

            if (eventType == (int)TRANSIENT_TYPE)
                resultType = RMSEventType.Transient;

            if (eventType == (int)SAG_SWELL_TYPE)
                resultType = RMSEventType.Variation;

            return resultType;
        }

        /// <summary>
        /// 获取指定类型特征值曲线的特征值数据列表
        /// </summary>
        /// <param name="curveType">待加载的曲线类型,1-ITIC, 2-SEMI, 不可为0</param>
        /// <param name="curveID">曲线ID，1-表示默认曲线，不可为0</param>
        /// <param name="userID">当前用户ID</param>
        /// <returns>特征值曲线</returns>
        public static List<SARFIChartCurve> GetSARFIChartFeatureValues(int curveType, int curveID, uint userID)
        {
            List<SARFIChartCurve> resultList = new List<SARFIChartCurve>();

            List<SARFIChartCurve> tempList = GetUserSarfiChartList(userID);
            foreach (SARFIChartCurve curveItem in tempList)
            {
                if (curveItem.CurveType == curveType && curveItem.CurveID == curveID)
                    resultList.Add(curveItem);
            }

            return resultList;
        }

        /// <summary>
        /// 判断暂态事件是否有波形
        /// </summary>
        /// <param name="oriList">波形查找相关条件列表</param>
        /// <returns></returns>
        public List<bool> JudgeSARFIValueWave(List<SARFIWaveHandle> oriList)
        {
            List<bool> resultList = new List<bool>();

            //电压变动左右偏移270毫秒
            int sagSwellOffset = 270;
            //瞬变左右偏移250ms
            int transientOffset = 250;

            List<NodeDataParam> nodes = new List<NodeDataParam>();
            DateTime queryStartTime = DateTime.MaxValue;
            DateTime queryEndTime = DateTime.MinValue;
            foreach (SARFIWaveHandle oriItem in oriList)
            {
                NodeDataParam node = new NodeDataParam(PecsNodeType.PECSDEVICE_NODE, oriItem.DeviceID);
                nodes.Add(node);
                DateTime queryWaveTime;
                if (oriItem.EventType == SAG_SWELL_TYPE)
                {
                    //特征值时间-持续时间-20ms，左右偏移270ms
                    //（这是因为起始事件时间通过，特征值时间-持续时间加减20ms范围找到，波形再通过与起始事件左右偏移270ms范围找到）
                    queryWaveTime = oriItem.EventTime.AddMilliseconds(-1 * oriItem.Duration);
                }
                else
                    queryWaveTime = oriItem.EventTime;
                if (queryWaveTime < queryStartTime)
                    queryStartTime = queryWaveTime;
                if (queryWaveTime > queryEndTime)
                    queryEndTime = queryWaveTime;
            }

            //设备与时间列表的映射关系
            Dictionary<StaChaDevID, List<DateTime>> idWaveTimeMap = OriginalDataManager.DataManager.GetComtradMap(nodes, new DateTimeParam(queryStartTime, queryEndTime), sagSwellOffset);

            //判断是否有波形
            foreach (SARFIWaveHandle oriItem in oriList)
            {
                bool result = false;
                uint stationID = SystemNodeManager.DataManager.FindStationID(oriItem.DeviceID);
                uint channelID = SystemNodeManager.DataManager.FindChannelID(oriItem.DeviceID);
                StaChaDevID staItem = new StaChaDevID(stationID, channelID, oriItem.DeviceID);
                DateTime waveTime;
                if (oriItem.EventType == SAG_SWELL_TYPE)
                    waveTime = oriItem.EventTime.AddMilliseconds(-1 * oriItem.Duration);
                else
                    waveTime = oriItem.EventTime;
                List<DateTime> timeList;
                bool exists = idWaveTimeMap.TryGetValue(staItem, out timeList);
                if (exists && timeList != null)
                {
                    foreach (DateTime timeItem in timeList)
                    {
                        DateTime waveStartTime;
                        DateTime waveEndTime;
                        if (oriItem.EventType == SAG_SWELL_TYPE)
                        {
                            waveStartTime = waveTime.AddMilliseconds(-1 * sagSwellOffset);
                            waveEndTime = waveTime.AddMilliseconds(sagSwellOffset);
                        }
                        else
                        {
                            waveStartTime = waveTime.AddMilliseconds(-1 * transientOffset);
                            waveEndTime = waveTime.AddMilliseconds(transientOffset);
                        }
                        if (timeItem >= waveStartTime && timeItem <= waveEndTime)
                        {
                            result = true;
                            break;
                        }
                    }
                }
                resultList.Add(result);
            }

            return resultList;
        }

        /// <summary>
        /// 获取SARFI曲线基本信息
        /// </summary>
        /// <param name="curveType">曲线类型，0-不限定曲线类型，1-ITIC，2-SEMI</param>
        /// <param name="curveID">曲线ID，0-不限定曲线ID，1-表示默认曲线</param>
        /// <param name="userID">当前用户ID</param>
        /// <returns></returns>
        public List<SARFIChartInfo> GetSARFIChartGeneralInfo(int curveType, int curveID, uint userID)
        {
            List<SARFIChartInfo> resultList = new List<SARFIChartInfo>();

            List<SARFIChartCurve> tempList = GetUserSarfiChartList(userID);
            foreach (SARFIChartCurve chartItem in tempList)
            {
                if (curveType != chartItem.CurveType && curveType != 0)
                    continue;
                if (curveID != chartItem.CurveID && curveID != 0)
                    continue;
                SARFIChartInfo resultItem = new SARFIChartInfo();
                resultItem.CurveType = chartItem.CurveType;
                resultItem.CurveID = chartItem.CurveID;
                resultItem.CurveName = chartItem.CurveName;
                resultList.Add(resultItem);
            }

            return resultList;
        }


        /// <summary>
        /// 从定时记录中匹配指定数据
        /// </summary>
        /// <param name="logTime"></param>
        /// <param name="oriDataLogs"></param>
        /// <returns></returns>
        private double GetFormitTimeDataValue(DateTime logTime, List<DataLogOriDef> oriDataLogs)
        {
            double resultValue = double.NaN;

            for (int i = 0; i < oriDataLogs.Count; i++)
            {
                if (logTime == oriDataLogs[i].LogTime)
                {
                    resultValue = oriDataLogs[i].DataValue;
                    break;
                }
            }

            return resultValue;
        }

        /// <summary>
        /// 获取SARFI用来查询定时记录的参数信息
        /// </summary>
        /// <param name="deviceIDs"></param>
        /// <param name="dataIDs"></param>
        /// <returns></returns>
        private List<DeviceDataParam> GetRMSDataParams(List<uint> deviceIDs, List<uint> dataIDs)
        {
            List<DeviceDataParam> dataParams = new List<DeviceDataParam>();
            foreach (uint deviceID in deviceIDs)
            {
                List<DATALOG_PRIVATE_MAP> dataMaps = FindRMSDataMaps(deviceID);
                foreach (uint dataID in dataIDs)
                {
                    //这里的DataTypeID默认为1，因为有些现场的配置可能为2,3,4等，所以进行了进一步的判定
                    DeviceDataParam param = new DeviceDataParam(deviceID, dataID, 1);
                    foreach (DATALOG_PRIVATE_MAP map in dataMaps)
                    {
                        if (map.deviceID == deviceID && map.dataID == dataID)
                        {
                            param = new DeviceDataParam(deviceID, dataID, map.dataTypeID);
                            param.LogicalDeviceIndex = (int)map.logicalDeviceIndex;
                            param.ParaType = (int)map.paraType;
                            break;
                        }
                    }
                    dataParams.Add(param);
                }
            }
            return dataParams;
        }

        /// <summary>
        /// 查找设备相关的暂态映射关系
        /// </summary>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        private List<DATALOG_PRIVATE_MAP> FindRMSDataMaps(uint deviceID)
        {
            List<DATALOG_PRIVATE_MAP> resultList = new List<DATALOG_PRIVATE_MAP>();
            List<DATALOG_PRIVATE_MAP> dataMaps = EMSWebServiceManager.EMSWebManager.GetDatalogPrivateMapByID(deviceID);
            foreach (DATALOG_PRIVATE_MAP map in dataMaps)
            {
                //只获取暂态参数类型
                if (map.dataID > 3000000 && map.dataID < 4000000)
                    resultList.Add(map);
            }
            return resultList;
        }


        /// <summary>
        /// 判断特征值越限类型
        /// </summary>
        /// <param name="sarfiCurve">标准曲线</param>
        /// <param name="duration">持续时间（对数值）</param>
        /// <param name="totalMag">幅值</param>
        /// <returns>返回越限类型，-1表示不在区域范围内，0-不越限，1-越上限，2-越下限</returns>
        public int GetRMSLimitTypesByPointRange(List<SARFIChartCurve> sarfiCurve, double duration, double totalMag, List<double> pointRange)
        {
            if (double.IsNaN(duration) || double.IsNaN(totalMag))
                return (int)SARFILimitType.OutOfRange;

            int result = (int)SARFILimitType.Normal;
            totalMag = GetChangedTotalMagForPoint(totalMag); //瞬变事件幅值有可能是负数
            if (pointRange.Count != 0) //说明是在容忍度界面，需要判断是否在容忍度区域外
            {
                int curveType = sarfiCurve[0].CurveType;

                //标记不在界面范围的点
                double x_dur = duration;
                if (curveType == (uint)ToleranceCurveType.ITICLimit)
                    x_dur = Math.Log10(duration);
                if (x_dur < pointRange[0])
                    result = (int)SARFILimitType.OutOfRange;
                if (x_dur > pointRange[1])
                    result = (int)SARFILimitType.OutOfRange;
                if (totalMag < pointRange[2])
                    result = (int)SARFILimitType.OutOfRange;
                if (totalMag > pointRange[3])
                    result = (int)SARFILimitType.OutOfRange;
            }

            if (result == (int)SARFILimitType.OutOfRange)
                return result;

            result = GetRMSLimitTypes(sarfiCurve, duration, totalMag);

            return result;
        }
        /// <summary>
        /// 判断特征值越限类型
        /// </summary>
        /// <param name="sarfiCurve">标准曲线</param>
        /// <param name="duration">持续时间（对数值）</param>
        /// <param name="totalMag">幅值</param>
        /// <returns>返回越限类型，0-不越限，1-越上限，2-越下限</returns>
        private int GetRMSLimitTypes(List<SARFIChartCurve> sarfiCurve, double duration, double totalMag)
        {
            if (double.IsNaN(duration) || double.IsNaN(totalMag))
                return (int)SARFILimitType.OutOfRange;

            int result = (int)SARFILimitType.Normal;
            totalMag = GetChangedTotalMagForPoint(totalMag); //瞬变事件幅值有可能是负数

            if (sarfiCurve.Count != 1)
                return result;

            if (sarfiCurve[0].UpLineValues.Count == 0)
                return result;

            int curveType = sarfiCurve[0].CurveType;

            if (curveType == (int)ToleranceCurveType.ITICLimit)
            {
                //判断是否越上限
                result = OverITICUpCurve(sarfiCurve, duration, totalMag, result, curveType);

                //如果有下限曲线，并且数值点非越上限，则进一步进行越下限判断
                result = UnderITICDownCurve(sarfiCurve, duration, totalMag, result, curveType);
            }

            if (curveType == (int)ToleranceCurveType.SEMILimit)
            {
                //判断是否越下限
                result = UnderSEMICurve(sarfiCurve, duration, totalMag, result);
            }

            return result;
        }
      

        /// <summary>
        /// 判断是否越SEMI下限
        /// </summary>
        /// <param name="sarfiCurve"></param>
        /// <param name="duration"></param>
        /// <param name="totalMag"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private int UnderSEMICurve(List<SARFIChartCurve> sarfiCurve, double duration, double totalMag, int result)
        {
            for (int i = 0; i < sarfiCurve[0].UpLineValues.Count - 1; i++)
            {
                SARFIChartValue point1 = sarfiCurve[0].UpLineValues[i];
                SARFIChartValue point2 = sarfiCurve[0].UpLineValues[i + 1];
                double x_val = duration;
                double y_val;
                bool exist = GetYValueByXValue(point1, point2, x_val, out y_val);
                if (exist)
                {
                    if (y_val == double.MaxValue)
                    {
                        if (totalMag < point1.YValue && totalMag < point2.YValue)
                            result = (int)SARFILimitType.BelowLimit;
                    }
                    else if (totalMag < y_val)
                        result = (int)SARFILimitType.BelowLimit;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 判断是否越ITIC下限
        /// </summary>
        /// <param name="sarfiCurve"></param>
        /// <param name="duration"></param>
        /// <param name="totalMag"></param>
        /// <param name="result"></param>
        /// <param name="curveType"></param>
        /// <returns></returns>
        private int UnderITICDownCurve(List<SARFIChartCurve> sarfiCurve, double duration, double totalMag, int result, int curveType)
        {
            if (sarfiCurve[0].DownLineValues != null && result == (int)SARFILimitType.Normal)
            {
                for (int i = 0; i < sarfiCurve[0].DownLineValues.Count - 1; i++)
                {
                    SARFIChartValue point1 = sarfiCurve[0].DownLineValues[i];
                    SARFIChartValue point2 = sarfiCurve[0].DownLineValues[i + 1];
                    double x_val = duration;
                    if (curveType == (int)ToleranceCurveType.ITICLimit)
                        x_val = Math.Log10(duration);
                    double y_val;
                    bool exist = GetYValueByXValue(point1, point2, x_val, out y_val);
                    if (exist)
                    {
                        if (y_val == double.MaxValue)
                        {
                            //垂直直线
                            if (totalMag < point1.YValue && totalMag < point2.YValue)
                                result = (int)SARFILimitType.BelowLimit;
                        }
                        else if (totalMag < y_val)
                            result = (int)SARFILimitType.BelowLimit;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 判断是否越ITIC上限
        /// </summary>
        /// <param name="sarfiCurve"></param>
        /// <param name="duration"></param>
        /// <param name="totalMag"></param>
        /// <param name="result"></param>
        /// <param name="curveType"></param>
        /// <returns></returns>
        private int OverITICUpCurve(List<SARFIChartCurve> sarfiCurve, double duration, double totalMag, int result, int curveType)
        {
            for (int i = 0; i < sarfiCurve[0].UpLineValues.Count - 1; i++)
            {
                SARFIChartValue point1 = sarfiCurve[0].UpLineValues[i];
                SARFIChartValue point2 = sarfiCurve[0].UpLineValues[i + 1];
                double x_val = duration;
                if (curveType == (int)ToleranceCurveType.ITICLimit)
                    x_val = Math.Log10(duration);
                double y_val;
                bool exist = GetYValueByXValue(point1, point2, x_val, out y_val);
                if (exist)
                {
                    if (y_val == double.MaxValue)
                    {
                        if (totalMag > point1.YValue && totalMag > point2.YValue)
                            result = (int)SARFILimitType.OverLimit;
                    }
                    else if (totalMag > y_val)
                        result = (int)SARFILimitType.OverLimit;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 根据输入的两个点构成的线段，以及新的X坐标值，获取Y坐标值
        /// </summary>
        /// <param name="point1">第一个坐标点(X轴值比第二个坐标X轴值小)</param>
        /// <param name="point2">第二个坐标点</param>
        /// <param name="x_value">X轴值</param>
        /// <param name="y_value">输出，Y轴值</param>
        /// <returns>是否找到值</returns>
        private bool GetYValueByXValue(SARFIChartValue point1, SARFIChartValue point2, double x_value, out double y_value)
        {
            bool result = true;

            //如果为垂直直线，那么返回最大值
            if (point1.XValue == point2.XValue && point1.XValue == x_value)
            {
                y_value = double.MaxValue;
                return true;
            }

            //如果不在线段范围内，则查找失败
            if (x_value < point1.XValue || x_value > point2.XValue)
            {
                y_value = double.NaN;
                return false;
            }

            // 计算斜率    
            double k = (point1.YValue - point2.YValue) / (point1.XValue - point2.XValue);
            // 根据斜率，计算y坐标    
            y_value = k * (x_value - point1.XValue) + point1.YValue;

            return result;
        }

        ///// <summary>
        ///// 将曲线信息写入到二进制流中
        ///// </summary>
        ///// <param name="curveList"></param>
        ///// <returns></returns>
        //private byte[] WriteSarfiChartCurveIntoStream(List<SARFIChartCurve> curveList)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    BinaryWriter writer = new BinaryWriter(ms);
        //    writer.Write(curveList.Count);
        //    foreach (SARFIChartCurve curveItem in curveList)
        //        curveItem.WriteBuffer(writer);
        //    return ms.ToArray();
        //}

     

        /// <summary>
        /// 根据暂态事件类型查找相关定时记录的参数ID列表
        /// </summary>
        /// <param name="eventType">事件类型，为0表示查询所有暂态参数</param>
        /// <returns></returns>
        private List<uint> GetRMSDataIDs(uint eventType)
        {
            List<uint> resultList = new List<uint>();

            if (eventType == SAG_SWELL_TYPE || eventType == 0)
            {
                resultList.Add(3000028);//总特征幅值(%)
                resultList.Add(3000001);//电压变动持续时间
                resultList.Add(3000002);//电压变动基准电压值
                resultList.Add(3000003);//电压变动A相最小幅值(%)
                resultList.Add(3000004);//电压变动A相最大幅值(%)
                resultList.Add(3000005);//电压变动A相平均幅值(%)
                resultList.Add(3000006);//电压变动A相能量(%)
                resultList.Add(3000007);//电压变动B相最小幅值(%)
                resultList.Add(3000008);//电压变动B相最大幅值(%)
                resultList.Add(3000009);//电压变动B相平均幅值(%)
                resultList.Add(3000010);//电压变动B相能量(%)
                resultList.Add(3000011);//电压变动C相最小幅值(%)
                resultList.Add(3000012);//电压变动C相最大幅值(%)
                resultList.Add(3000013);//电压变动C相平均幅值(%)
                resultList.Add(3000014);//电压变动C相能量(%)                
            }

            if (eventType == TRANSIENT_TYPE || eventType == 0)
            {
                resultList.Add(3000021);//瞬变基准电压
                resultList.Add(3000022);//瞬变A相电压持续时间
                resultList.Add(3000023);//瞬变A相电压最大幅值(%)
                resultList.Add(3000024);//瞬变B相电压持续时间
                resultList.Add(3000025);//瞬变B相电压最大幅值(%)
                resultList.Add(3000026);//瞬变C相电压持续时间
                resultList.Add(3000027);//瞬变C相电压最大幅值(%)
                resultList.Add(3000029);//瞬变总特征幅值(%)
                resultList.Add(3000030);//瞬变总持续时间(ms)
            }
            return resultList;
        }

        /// <summary>
        /// 加载默认Sarfi特征曲线特征值数据
        /// </summary>
        /// <param name="curType">特征曲线类型，0-不限定曲线类型，1-ITIC，2-SEMI</param>
        /// <returns></returns>
        private static List<SARFIChartCurve> LoadDefaultSarfiFeatureData(int curType)
        {
            List<SARFIChartCurve> resultList = new List<SARFIChartCurve>();

            int defaultCurID = 1;//默认的ID            
            if (curType == 0 || curType == (int)ToleranceCurveType.ITICLimit)
            {
                SARFIChartCurve ITICCurve = new SARFIChartCurve((int)ToleranceCurveType.ITICLimit, defaultCurID);
                ITICCurve.CurveName = "ITIC";
                //上限曲线
                ITICCurve.UpLineValues.Add(new SARFIChartValue(-0.398, 500));
                ITICCurve.UpLineValues.Add(new SARFIChartValue(0, 200));
                ITICCurve.UpLineValues.Add(new SARFIChartValue(0.477, 140));
                ITICCurve.UpLineValues.Add(new SARFIChartValue(0.477, 120));
                ITICCurve.UpLineValues.Add(new SARFIChartValue(1.30, 120));
                ITICCurve.UpLineValues.Add(new SARFIChartValue(2.699, 120));
                ITICCurve.UpLineValues.Add(new SARFIChartValue(2.699, 110));
                ITICCurve.UpLineValues.Add(new SARFIChartValue(4, 110));
                ITICCurve.UpLineValues.Add(new SARFIChartValue(5, 110));
                //下限曲线
                ITICCurve.DownLineValues.Add(new SARFIChartValue(1.30, 0));
                ITICCurve.DownLineValues.Add(new SARFIChartValue(1.30, 70));
                ITICCurve.DownLineValues.Add(new SARFIChartValue(2.699, 70));
                ITICCurve.DownLineValues.Add(new SARFIChartValue(2.699, 80));
                ITICCurve.DownLineValues.Add(new SARFIChartValue(4, 80));
                ITICCurve.DownLineValues.Add(new SARFIChartValue(4, 90));
                ITICCurve.DownLineValues.Add(new SARFIChartValue(5, 90));
                resultList.Add(ITICCurve);
            }

            if (curType == 0 || curType == (int)ToleranceCurveType.SEMILimit)
            {
                SARFIChartCurve SEMICurve = new SARFIChartCurve((int)ToleranceCurveType.SEMILimit, defaultCurID);
                SEMICurve.CurveName = "SEMI-1s";
                SEMICurve.UpLineValues.Add(new SARFIChartValue(50, 50));
                SEMICurve.UpLineValues.Add(new SARFIChartValue(200, 50));
                SEMICurve.UpLineValues.Add(new SARFIChartValue(200, 70));
                SEMICurve.UpLineValues.Add(new SARFIChartValue(500, 70));
                SEMICurve.UpLineValues.Add(new SARFIChartValue(500, 80));
                SEMICurve.UpLineValues.Add(new SARFIChartValue(1000, 80));
                resultList.Add(SEMICurve);

                SARFIChartCurve SEMI100sCurve = new SARFIChartCurve((int)ToleranceCurveType.SEMILimit, SEMI100S_CURVE_ID);
                SEMI100sCurve.CurveName = "SEMI-100s";
                SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(10, 0));
                SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(20, 0));
                SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(20, 50));
                SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(200, 50));
                SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(200, 70));
                SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(500, 70));
                SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(500, 80));
                SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(10000, 80));
                SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(10000, 90));
                SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(100000, 90));
                resultList.Add(SEMI100sCurve);
            }

            return resultList;
        }


        /// <summary>
        /// 获取用户匹配的曲线列表（iems特有）
        /// </summary>        
        /// <param name="userID">当前用户ID</param>
        /// <returns></returns>
        private static List<SARFIChartCurve> GetUserSarfiChartList(uint userID)
        {
            // List<SARFIChartCurve> tempList;
            //bool hasVal = userChartMap.TryGetValue(userID, out tempList);
            List<SARFIChartCurve> defaultSarfiCurves = LoadDefaultSarfiFeatureData(0);

            List<SARFIChartCurve> resultList = defaultSarfiCurves;
            //if (tempList == null)
            //    return resultList;

            //foreach (SARFIChartCurve curve in tempList)
            //{
            //    if (ExistInUserSarfiChartList(defaultSarfiCurves, curve))
            //        continue;
            //    resultList.Add(curve);
            //}

            return resultList;
        }



        /// <summary>
        /// 判断是否有波形
        /// </summary>
        /// <param name="idComtradMap"></param>
        /// <param name="eventHandle"></param>
        /// <returns></returns>
        private bool HasEventComtrade(Dictionary<StaChaDevID, List<DateTime>> idComtradMap, EventInformation     eventHandle)
        {
            StaChaDevID staDev = new StaChaDevID((uint)eventHandle.StationId, (uint)eventHandle.ChannelId, (uint)eventHandle.DeviceId);
            bool hasComtrade = OriginalDataManager.DataManager.HasComtradeFile(staDev, eventHandle.FullTime, WAVE_OFFSET, idComtradMap);
            return hasComtrade;
        }

        /// <summary>
        /// 针对瞬变事件电压幅值有可能为负数的情况进行处理，如果为负数，则先取绝对值，后加上100才能在ITIC中进行打点
        /// </summary>
        /// <param name="totalMag"></param>
        /// <returns></returns>
        private double GetChangedTotalMagForPoint(double totalMag)
        {
            if (totalMag >= 0)
                return totalMag;

            return Math.Abs(totalMag) + 100;
        }
        /// <summary>
        /// 添加了3000028和3000029到数据库15表中，分别对应瞬变事件的总幅值和总持续时间
        /// </summary>
        /// <param name="featureList"></param>
        /// <returns></returns>
        private double[] GetTransientDurMag(List<double> featureList)
        {
            double duration = double.NaN, totalMag = double.NaN;
            if (featureList.Count == 9)
            {
                totalMag = featureList[7];
                duration = featureList[8];
            }

            List<int> durationIndexes = new List<int>(); //存储持续时间对应的索引
            if (double.IsNaN(totalMag)) //先判断幅值是否为空，如果为空需要取三相里面的绝对值最大值，持续时间该相对应的持续时间
            {
                double totalMagCompareValue = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (GetChangedTotalMagForPoint(featureList[i * 2 + 2]) >= totalMagCompareValue)
                    {
                        if (GetChangedTotalMagForPoint(featureList[i * 2 + 2]) == totalMagCompareValue) //如果是等于的话需要将索引添加进去
                            durationIndexes.Add(i);

                        if (GetChangedTotalMagForPoint(featureList[i * 2 + 2]) > totalMagCompareValue) //如果是大于的话则清除原来的索引，并加入当前的索引
                        {
                            durationIndexes.Clear();
                            durationIndexes.Add(i);
                        }

                        totalMag = featureList[i * 2 + 2];
                        totalMagCompareValue = GetChangedTotalMagForPoint(featureList[i * 2 + 2]);
                    }
                }
            }
            if (double.IsNaN(duration))
            {
                double durationCompareValue = 0;
                for (int i = 0; i < 3; i++)
                {
                    //如果索引不在幅值对应的索引里面则直接跳过，如果对应的索引为空说明featureList[7]不为空，但是featureList[8]为空，此时需要从三相中获取持续时间最大值
                    if (durationIndexes.Count != 0 && !durationIndexes.Contains(i))
                        continue;

                    //持续时间不可能为负数，所以这里Math.Abs没有必要改成GetChangedTotalMagForPoint
                    if (Math.Abs(featureList[i * 2 + 1]) >= durationCompareValue) //针对所有特征值都为0的情况，需要用>=
                    {
                        duration = featureList[i * 2 + 1];
                        durationCompareValue = Math.Abs(featureList[i * 2 + 1]);
                    }
                }
            }
            return new double[] { duration, totalMag };
        }

        /// <summary>
        ///     /// <summary>
        /// 查询暂态事件信息
        /// </summary>
        /// <param name="nodeParam">电能质量节点类型-节点ID</param>
        /// <param name="rmsEventType">暂态事件类型，1-瞬变、2-电压变动</param>
        /// <param name="timeParam">时间组合，起始时间-结束时间</param>     
        /// <param name="maxRowCount">返回记录最大条数</param>
        /// <returns></returns>
        /// </summary>
        /// <param name="nodeParams"></param>
        /// <param name="rmsEventTypes"></param>
        /// <param name="timeParam"></param>
        /// <param name="eventID"></param>
        /// <param name="curveTypeID">curveTypeID这个是ITIC，SEMI曲线的Type+ID.
        /// curveTypeID这个是ITIC，SEMI曲线的Type+ID，其中Type为曲线类型,1-ITIC, 2-SEMI, 不可为0;
        /// iEMSWeb里面因为可以自定义曲线，所以会有ID不等于1的情况;OfficeReport里面不需要考虑自定义曲线,OfficeReport里面应该只有默认曲线,所以ID传1就可以了</param>
        /// <param name="userID"></param>
        /// <param name="maxRowCount"></param>
        /// <param name="pointRange"></param>
        /// <param name="groupInterval"></param>
        /// <returns></returns>
        public List<SagSwellEvent> QuerySagSwellEvents(List<NodeParam> nodeParams, List<int> rmsEventTypes, DateTimeParam timeParam, uint eventID, NodeParam curveTypeID, uint userID, int maxRowCount, string pointRange, string groupInterval)
        {
            List<SagSwellEvent> resultList = new List<SagSwellEvent>();
            //从数据库查找数据            
            List<uint> deviceIDs = GetDeviceIds(nodeParams, userID);
            //仿照上面的来写
            Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18StartEventDic;
            Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18EndEventDic;
            Dictionary<int, List<EventInformation>> _Direction19Dic;
            PdTb06AllDataManager pd = new PdTb06AllDataManager();
            if (!pd.GetAllEvents(deviceIDs, timeParam.StartTime, timeParam.EndTime, rmsEventTypes, maxRowCount, out _Variation18StartEventDic, out _Variation18EndEventDic, out _Direction19Dic))
                return resultList;

            //从小到大的顺序
            var variation17And18StartEventDic = GetSortedVariation17And18StartEventDic(_Variation18StartEventDic);
            //从大到小排序
            var startEventDt = GetStartEventDt(variation17And18StartEventDic);
            DateTimeParam queryTimeParam = EventLogManager.DataManager.GetRecordEventTimeParam(startEventDt);
            Dictionary<StaChaDevID, List<DateTime>> idComtradMap;
            WaveManager waveManager = new WaveManager();
            waveManager.GetAllWave(nodeParams, queryTimeParam.StartTime, queryTimeParam.EndTime, out idComtradMap);
            //获取Sarfi曲线
            List<SARFIChartCurve> sarfiValues = GetSARFIChartFeatureValues((int) curveTypeID.NodeType, (int) curveTypeID.NodeID, userID);
            //获取容忍度区域，Sarfi曲线
            List<double> pointRangeList = new List<double>();
            if (pointRange != string.Empty)
            {
                string[] pointStrList = pointRange.Split(',');
                foreach (string pointStr in pointStrList)
                    pointRangeList.Add(Convert.ToDouble(pointStr));
            }

            foreach (var oneDevice in _Variation18StartEventDic)
            {
                var keyDevice = oneDevice.Key;
                NodeParam pqNode = new NodeParam((uint) SysConstDefinition.PECSDEVICE_NODE, (uint) keyDevice);
                var sysNode = PecsNodeManager.PecsNodeInstance.GetNodeByTypeID(pqNode.NodeType, pqNode.NodeID);
                string nodeName = sysNode.NodeName;

                var valueDevice = oneDevice.Value;
                foreach (var handle in valueDevice)
                {
                    var sagHanleValue = handle.Value;
                    foreach (var eventHandle in sagHanleValue)
                    {
                        SagSwellEvent resultItem = new SagSwellEvent();
                        resultItem.EventID = (uint) eventHandle.ID;
                        resultItem.DeviceID = (uint) eventHandle.DeviceId;
                        resultItem.PQNodeType = pqNode.NodeType;
                        resultItem.PQNodeID = pqNode.NodeID;
                        resultItem.PQNodeName = nodeName;
                        resultItem.EventByte = eventHandle.EventByte;
                        resultItem.EventClass = eventHandle.EventClass;
                        resultItem.EventCode1 = eventHandle.Code1;
                        resultItem.EventCode2 = eventHandle.Code2;
                        resultItem.EventTime = DataFormatManager.GetFormatTimeString(eventHandle.FullTime);
                        resultItem.EventType = eventHandle.EventType;
                        resultItem.LocalZone = SystemNodeManager.DataManager.FindTimeZoneOffsetByStation((uint) eventHandle.StationId);
                        resultItem.RMSEventType = GetEventTypeName(resultItem.EventType);
                        resultItem.StationID = (uint) eventHandle.StationId;
                        resultItem.ChannelID = (uint) eventHandle.ChannelId;
                        resultItem.StationFlag = eventHandle.StationFlag;
                        resultItem.HaveWave = HasEventComtrade(idComtradMap, eventHandle);
                        resultItem.FetureInfo = GetRmsFeatureInfo(_Variation18StartEventDic, _Variation18EndEventDic, _Direction19Dic, eventHandle, sarfiValues, pointRangeList);
                        resultList.Add(resultItem);
                    }
                }
            }
            EndTimeManager.GetInstance().SetLastEventTime(DateTime.MinValue);

            return GroupSagSwellEvent(resultList, groupInterval);
        }

        /// <summary>
        /// 获取暂态事件的特征值信息
        /// </summary>
        /// <param name="startEventMap"></param>
        /// <param name="endEventMap"></param>
        /// <param name="dr"></param>
        /// <param name="eventHandle"></param>
        /// <returns></returns>
        private SagSwellFeature GetRmsFeatureInfo(Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18StartEventDic, Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18EndEventDic, Dictionary<int, List<EventInformation>> _Direction19Dic, EventInformation eventHandle, List<SARFIChartCurve> sarfiValues, List<double> pointRangeList)
        {
            SagSwellFeature featureInfo = new SagSwellFeature(false, false);
            try
            {
                //结束事件匹配                
                if (eventHandle.EventType == (int)SAG_SWELL_TYPE)
                    GetSagSwellFeatrueInfo(_Variation18StartEventDic, _Variation18EndEventDic, _Direction19Dic, eventHandle, sarfiValues, pointRangeList, out featureInfo);
                if (eventHandle.EventType == (int)TRANSIENT_TYPE)
                    GetTransientFeatureInfo(eventHandle, sarfiValues, pointRangeList, out featureInfo);
                return featureInfo;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }
            return featureInfo;
        }

        /// <summary>
        /// 获取暂态特征值信息（瞬变事件）
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="pointRangeList">如果是容忍度分析界面需要一次性查询出特征值</param>
        /// <returns></returns>
        private bool GetTransientFeatureInfo(EventInformation dr, List<SARFIChartCurve> sarfiValues, List<double> pointRangeList, out  SagSwellFeature featureInfo)
        {
            featureInfo = new SagSwellFeature(false, false);
            featureInfo.EventID = (uint)dr.ID;
            featureInfo.FeatureTime = DataFormatManager.GetFormatTimeString(GetDateTimeFromEventRow(dr));

            List<double> featureList = new List<double>();
            bool success = GetFeatureListFromStr((int)TRANSIENT_TYPE, dr, ref featureList);
            featureInfo.FeatureValues = featureList;
            if (success)
            {
                double[] durAndMag = GetTransientDurMag(featureList);
                featureInfo.Duration = durAndMag[0];
                featureInfo.TotalMag = durAndMag[1];
                featureInfo.OutOfLimit = GetRMSLimitTypesByPointRange(sarfiValues, featureInfo.Duration, featureInfo.TotalMag, pointRangeList);
            }
            else
            {
                //featureInfo.QueryForMoreDatalog = true; //不再分两次获取特征值，因为在客户端加载会产生性能问题
                SagSwellEvent sagSwellEvent = new SagSwellEvent();
                sagSwellEvent.DeviceID = (uint)dr.DeviceId;

                sagSwellEvent.EventTime = dr.FullTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                sagSwellEvent.EventType = dr.EventType;
                sagSwellEvent.EventByte = dr.EventByte;
                sagSwellEvent.EventCode1 = dr.Code1;
                featureInfo = QueryForMoreDatalog(sarfiValues, sagSwellEvent, featureInfo);
                if (featureInfo.FeatureValues.Count != 0)
                {
                    //返写特征值到数据库表
                    WriteRMSEventStr(featureInfo.FeatureValues, featureInfo.EventID, Convert.ToDateTime(featureInfo.FeatureTime));
                    featureInfo.QueryForMoreDatalog = false;
                    if (pointRangeList.Count == 4) //如果是容忍度分析界面中，需要一次性查询出特征值数据
                        featureInfo.OutOfLimit = GetRMSLimitTypesByPointRange(sarfiValues, featureInfo.Duration, featureInfo.TotalMag, pointRangeList);
                    else //如果是暂态事件界面，需将pointRangeList设为空，因为无法获取chart范围
                        featureInfo.OutOfLimit = GetRMSLimitTypesByPointRange(sarfiValues, featureInfo.Duration, featureInfo.TotalMag, new List<double>());
                }
            }

            return true;
        }

        /// <summary>
        /// 针对未找到特征值的情况，需要从定时记录中查询特征值
        /// </summary>
        /// <param name="sarfiValues"></param>
        /// <param name="sagSwellEvent"></param>
        /// <param name="featureItem"></param>
        /// <returns></returns>
        private SagSwellFeature QueryForMoreDatalog(List<SARFIChartCurve> sarfiValues, SagSwellEvent sagSwellEvent, SagSwellFeature featureItem)
        {
            List<uint> dataIDs = GetRMSDataIDs((uint)sagSwellEvent.EventType);
            List<uint> deviceIDs = new List<uint>();
            deviceIDs.Add(sagSwellEvent.DeviceID);
            List<DeviceDataParam> dataParamList = GetRMSDataParams(deviceIDs, dataIDs);

            DateTime featureTime = Convert.ToDateTime(featureItem.FeatureTime);
            //取前后1秒的查询范围
            DateTimeParam timeParam = new DateTimeParam(featureTime.AddSeconds(-1), featureTime.AddSeconds(1));
            List<DataLogValueOriList> datalogList = HistoryDataLogManager.DataManager.GetHistoryDataByDuration(dataParamList, timeParam, false);

            SagSwellFeature featureInfo = new SagSwellFeature(false, false);
            featureInfo.EventID = featureItem.EventID; //必须添加，否则在写入特征值时会失败
            List<int> indexes = GetValueIndexList(sagSwellEvent.EventType, featureTime, datalogList);

            featureInfo.FeatureTime = DataFormatManager.GetFormatTimeString(featureTime);
            featureInfo.FeatureValues = GetFeatureList(datalogList, indexes, sagSwellEvent.EventType);
            if (sagSwellEvent.EventType == (uint)SAG_SWELL_TYPE)
            {
                if (featureInfo.FeatureValues.Count == SAG_SWELL_COUNT)
                {
                    featureInfo.TotalMag = featureInfo.FeatureValues[0];
                    featureInfo.Duration = featureInfo.FeatureValues[1];
                }
            }
            else
            {
                double[] durAndMag = GetTransientDurMag(featureInfo.FeatureValues);
                featureInfo.Duration = durAndMag[0];
                featureInfo.TotalMag = durAndMag[1];
            }
            featureInfo.OutOfLimit = GetRMSLimitTypes(sarfiValues, featureInfo.Duration, featureInfo.TotalMag);
            return featureInfo;
        }

        /// <summary>
        /// 获取暂态特征值信息（电压变动事件）
        /// </summary>
        /// <param name="startEventMap"></param>
        /// <param name="endEventMap"></param>
        /// <param name="dr"></param>
        /// <param name="eventHandle"></param>
        /// <param name="forSarfi">如果是容忍度分析界面需要一次性查询出特征值</param>
        /// <returns></returns>
        private bool GetSagSwellFeatrueInfo(Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18StartEventDic,
            Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18EndEventDic, Dictionary<int, List<EventInformation>> _Direction19Dic,
            EventInformation eventHandle, List<SARFIChartCurve> sarfiValues, List<double> pointRangeList, out SagSwellFeature featureInfo)
        {
            featureInfo = new SagSwellFeature(false, false);
            SagHandle sagHandle = new SagHandle(eventHandle.StationId, eventHandle.ChannelId, eventHandle.DeviceId, eventHandle.EventByte, eventHandle.EventType, eventHandle.StationFlag, eventHandle.Code1);
            DateTime startEventTime = eventHandle.FullTime;
            Dictionary<SagHandle, List<EventInformation>> oneDeviceStartEventMap;
            if (!_Variation18StartEventDic.TryGetValue(eventHandle.DeviceId, out oneDeviceStartEventMap))
                return false;

            EventInformation startNextdr = null;
            List<EventInformation> startRowList;
            bool hasStartRow = oneDeviceStartEventMap.TryGetValue(sagHandle, out startRowList);
            if (hasStartRow)
            {
                //找到相同条件起始事件的下一个起始事件
                startNextdr = GetFormatEventRow(startEventTime, startRowList);
            }

            EventInformation endTempdr = null;
            List<EventInformation> endRowList;
            Dictionary<SagHandle, List<EventInformation>> oneDeviceEndEventMap;
            if (_Variation18EndEventDic.TryGetValue(eventHandle.DeviceId, out oneDeviceEndEventMap))
            {
                if (oneDeviceEndEventMap.TryGetValue(sagHandle, out endRowList))
                {
                    //找到比起始事件时间大的结束事件
                    endTempdr = GetFormatEventRow(startEventTime, endRowList);
                }
            }
            EventInformation enddr = null;
            if (startNextdr != null && endTempdr != null)
            {
                //找到下一个起始事件和结束事件的情况
                featureInfo.QueryForMoreFeature = false;
                DateTime startNextTime = GetDateTimeFromEventRow(startNextdr);
                DateTime endTempTime = GetDateTimeFromEventRow(endTempdr);
                if (endTempTime < startNextTime) //如果结束事件时间小于下一个起始事件时间，说明找到了结束事件
                {
                    enddr = endTempdr;
                    featureInfo = GetSagSwellFeatureFromDataRow(startEventTime, enddr, sarfiValues, pointRangeList);
                }

                //与结束事件无关了，只需要判断方向事件大于等于起始事件时间，小于下一个起始事件时间即可
                GetRMSDirectionInfo(_Direction19Dic, eventHandle, ref featureInfo, startNextTime);
            }
            else if (startNextdr == null && endTempdr != null)
            {
                //找到结束事件而没有找到起始事件
                featureInfo.QueryForMoreFeature = false;
                enddr = endTempdr;
                featureInfo = GetSagSwellFeatureFromDataRow(startEventTime, enddr, sarfiValues, pointRangeList);
                //此时故障方向事件时间只需要>=起始事件即可匹配
                GetRMSDirectionInfo(_Direction19Dic, eventHandle, ref featureInfo, DateTime.MaxValue);
            }
            else if (startNextdr == null && endTempdr == null) //说明既没有下一个起始事件也没有结束事件，此时故障方向事件>=起始事件即可匹配
            {
                //不再分布获取特征值了，因为这样在客户端加载会产生性能问题
                //featureInfo.QueryForMoreFeature = true;
                featureInfo.QueryForMoreFeature = false;
                SagSwellEvent sagSwellEvent = new SagSwellEvent();
                sagSwellEvent.DeviceID = (uint)eventHandle.DeviceId;
                sagSwellEvent.EventTime = eventHandle.EventTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                sagSwellEvent.EventType = eventHandle.EventType;
                sagSwellEvent.EventByte = eventHandle.EventByte;
                sagSwellEvent.EventCode1 = eventHandle.Code1;
                if (pointRangeList.Count == 4) //容忍度分析中需要一次性查出特征值数据
                    featureInfo = QueryForMoreFeature(sarfiValues, sagSwellEvent, pointRangeList);
                else //电压暂态事件中传入的pointRangeList需为空
                    featureInfo = QueryForMoreFeature(sarfiValues, sagSwellEvent, new List<double>());
                //此时故障方向事件时间只需要>=起始事件即可匹配
                GetRMSDirectionInfo(_Direction19Dic, eventHandle, ref featureInfo, DateTime.MaxValue);
            }
            else
            {
                //其他情况下，表示已找到了下一个起始事件，但没有结束事件，说明结束事件丢失，
                featureInfo.QueryForMoreFeature = false;
                DateTime startNextTime = GetDateTimeFromEventRow(startNextdr);
                //此时故障方向事件时间需要>=起始事件，<下一条起始事件
                GetRMSDirectionInfo(_Direction19Dic, eventHandle, ref featureInfo, startNextTime);
            }
            return true;
        }

        /// <summary>
        /// 从数据行中取出事件特征值信息
        /// </summary>
        /// <param name="enddr"></param>
        /// <param name="forSarfi">容忍度分析中需要一次性查询出特征值</param>
        /// <returns></returns>
        private SagSwellFeature GetSagSwellFeatureFromDataRow(DateTime startEventTime, EventInformation enddr, List<SARFIChartCurve> sarfiValues, List<double> pointRangeList)
        {
            SagSwellFeature featureInfo = new SagSwellFeature(false, false);
            DateTime endEventTime = GetDateTimeFromEventRow(enddr);
            List<double> featureList = new List<double>();
            bool success = GetFeatureListFromStr((int)SAG_SWELL_TYPE, enddr, ref featureList);
            featureInfo.EventID = (uint)enddr.ID;
            featureInfo.FeatureTime = DataFormatManager.GetFormatTimeString(endEventTime);
            if (!success)
            {
                //featureInfo.QueryForMoreDatalog = true; //不再分两次获取特征值，因为在客户端加载会产生性能问题
                SagSwellEvent sagSwellEvent = new SagSwellEvent();
                sagSwellEvent.DeviceID = (uint)enddr.DeviceId;
                // DateTime eventTime = Convert.ToDateTime(enddr["EventTime"]);
                sagSwellEvent.EventTime = enddr.FullTime.ToString("yyyy-MM-dd HH:mm:ss.fff"); //eventTime.AddMilliseconds(Convert.ToInt32(enddr["Msec"])).ToString("yyyy-MM-dd HH:mm:ss.fff");
                sagSwellEvent.EventType = enddr.EventType;
                sagSwellEvent.EventByte = enddr.EventByte;
                sagSwellEvent.EventCode1 = enddr.Code1;
                featureInfo = QueryForMoreDatalog(sarfiValues, sagSwellEvent, featureInfo);
                if (featureInfo.FeatureValues.Count != 0)
                {
                    //返写特征值到数据库表
                    WriteRMSEventStr(featureInfo.FeatureValues, featureInfo.EventID, Convert.ToDateTime(featureInfo.FeatureTime));
                    featureInfo.QueryForMoreDatalog = false;
                    if (pointRangeList.Count == 4) //如果是容忍度分析界面中，需要一次性查询出特征值数据
                        featureInfo.OutOfLimit = GetRMSLimitTypesByPointRange(sarfiValues, featureInfo.Duration, featureInfo.TotalMag, pointRangeList);
                    else //如果是暂态事件界面，需将pointRangeList设为空，因为无法获取chart范围
                        featureInfo.OutOfLimit = GetRMSLimitTypesByPointRange(sarfiValues, featureInfo.Duration, featureInfo.TotalMag, new List<double>());
                }
            }
            else
            {
                featureInfo.QueryForMoreDatalog = false;
                featureInfo.TotalMag = featureList[0];
                featureInfo.Duration = featureList[1];
                featureInfo.OutOfLimit = GetRMSLimitTypesByPointRange(sarfiValues, featureInfo.Duration, featureInfo.TotalMag, pointRangeList);
                featureInfo.FeatureValues = featureList;
            }
            return featureInfo;
        }


        /// <summary>
        /// 匹配故障方向事件
        /// </summary>
        /// <param name="directionEventMap"></param>
        /// <param name="eventHandle"></param>
        /// <param name="featureInfo"></param>
        /// <param name="nextStartEventTime"></param>
        private void GetRMSDirectionInfo(Dictionary<int, List<EventInformation>> directionEventMap, EventInformation eventHandle, ref SagSwellFeature featureInfo, DateTime nextStartEventTime)
        {
            if (!SagCodes.Contains(eventHandle.Code1)) //只针对暂降事件判断故障方向，因为只有暂降事件才有方向事件；
                return;

            DateTime directionEventTime = DateTime.MinValue;
            EventInformation nextDirectionEventRow = GetNextDirectionEvent(eventHandle, directionEventMap, ref directionEventTime);
            if (nextDirectionEventRow != null && directionEventTime < nextStartEventTime) //说明匹配成功，不会等于下一个起始事件的时间，所以去掉了等号
                featureInfo.Direction = nextDirectionEventRow.Description;
        }

        /// <summary>
        /// 获取下一个故障方向事件
        /// </summary>
        /// <param name="eventHandle"></param>
        /// <param name="directionEventMap"></param>
        /// <param name="nextDirectionEventTime"></param>
        /// <returns></returns>
        private EventInformation GetNextDirectionEvent(EventInformation eventHandle, Dictionary<int, List<EventInformation>> directionEventMap, ref DateTime nextDirectionEventTime)
        {
            List<EventInformation> rowList;

            if (!directionEventMap.TryGetValue(eventHandle.DeviceId, out rowList))
                return null;

            for (int i = rowList.Count - 1; i >= 0; i--) //逆序遍历，因为事件是降序排列的，需要获取起始事件下一个的方向事件，也可能等于起始事件时间
            {
                DateTime directionEventTime = GetDateTimeFromEventRow(rowList[i]);
                if (directionEventTime >= eventHandle.EventTime)
                {
                    nextDirectionEventTime = directionEventTime;
                    return rowList[i];
                }
            }
            return null;
        }



        
        /// <summary>
        /// 从特征值列表中获取总电压幅值
        /// </summary>
        /// <param name="featureList"></param>
        /// <returns></returns>
        private double GetTotalMagFromFeatures(List<double> featureList)
        {
            double resultValue = double.NaN;

            if (featureList.Count != SAG_SWELL_COUNT)
                return resultValue;
            //如果获取到的总电压不为空，则直接返回
            if (!double.IsNaN(featureList[0]))
            {
                resultValue = featureList[0];
                return resultValue;
            }

            //从A相最小幅值(%);A相最大幅值(%);B相最小幅值(%);B相最大幅值(%);C相最小幅值(%);C相最大幅值(%);获取最严重的幅值，作为总幅值。
            double compareValue = 0;
            if (IsMaxMag(featureList[3], ref compareValue))
                resultValue = featureList[3];
            if (IsMaxMag(featureList[4], ref compareValue))
                resultValue = featureList[4];
            if (IsMaxMag(featureList[7], ref compareValue))
                resultValue = featureList[7];
            if (IsMaxMag(featureList[8], ref compareValue))
                resultValue = featureList[8];
            if (IsMaxMag(featureList[11], ref compareValue))
                resultValue = featureList[11];
            if (IsMaxMag(featureList[12], ref compareValue))
                resultValue = featureList[12];

            return resultValue;
        }

        private bool IsMaxMag(double featureValue, ref double compareValue)
        {
            bool result = false;
            if (!double.IsNaN(featureValue) && Math.Abs(100 - featureValue) > compareValue)
            {
                compareValue = Math.Abs(100 - featureValue);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 容忍度曲线类型
        /// </summary>
        public   enum ToleranceCurveType
        {
            /// <summary>
            /// ITIC曲线
            /// </summary>
            ITICLimit = 1,

            /// <summary>
            /// SEMI特征曲线
            /// </summary>
            SEMILimit = 2,
        }

        /// <summary>
        /// 越限类型
        /// </summary>
        public enum SARFILimitType
        {
            /// <summary>
            /// 错误
            /// </summary>
            OutOfRange = -1,
            /// <summary>
            /// 正常
            /// </summary>
            Normal = 0,
            /// <summary>
            /// 越上限
            /// </summary>
            OverLimit = 1,
            /// <summary>
            /// 越下限
            /// </summary>
            BelowLimit = 2,
        }
    }



    /// <summary>
    /// 用于辅助查询暂态事件
    /// </summary>
    public struct SagSwellHandle
    {
        public uint deviceID;
        public byte stationFlag;
        public int eventType;
        public int eventByte;
        public int code1;

        public SagSwellHandle(uint deviceID)
        {
            this.deviceID = deviceID;
            this.stationFlag = 0;
            this.eventByte = 0;
            this.eventType = (int)RMSAnalysisManager.SAG_SWELL_TYPE;
            this.code1 = -1;
        }
    }

    /// <summary>
    /// 暂态事件类型
    /// </summary>
    public enum RMSEventType
    {
        /// <summary>
        /// 瞬变
        /// </summary>
        Transient = 1,
        /// <summary>
        /// 电压变动
        /// </summary>
        Variation = 2,
    }

}

