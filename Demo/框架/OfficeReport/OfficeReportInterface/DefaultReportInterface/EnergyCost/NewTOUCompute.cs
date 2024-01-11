using System;
using System.Collections.Generic;
using System.Data;
using CET.PecsNodeManage;
using CSharpDBPlugin;
using OfficeReportInterface.DefaultReportInterface.CommonluUsed;
using OfficeReportInterface.DefaultReportInterface.PowerQualityEventsOnly;



namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
    public  class NewTOUCompute
    {
        public void AddWarningsForTariffIndex(DataTable warningDT, NewTOUProfile TOUProfile, bool isIncludeWarning,uint source)
        {
            foreach (var deviceItem in m_emptyDic)
            {
                //如果没有映射方案就不提示了
                DATALOG_PRIVATE_MAP resultMapDef;
                if (!ReportWebServiceManager.ReportWebManager.FindDataMapDef(deviceItem.Key, out resultMapDef))
                    continue;
               
                foreach (var tariffIndexItem in deviceItem.Value)
                {

                    foreach (var timeItem in tariffIndexItem.Value)
                    {
                        string tariffIndexName;
                        if (!GetTariffIndexNameById(tariffIndexItem.Key, TOUProfile, out tariffIndexName))
                            continue;
                        DefaultTemplatePublicMethod.AddWarnings(deviceItem.Key, WarningKind.DataForTariffIndexEmpty, timeItem,tariffIndexName, warningDT, isIncludeWarning, source);


                        //只要第一个元素的时刻即可。
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 获取tariffIndex对应的名称
        /// </summary>
        /// <param name="tariffIndex"></param>
        /// <param name="TOUProfile"></param>
        /// <param name="tariffIndexName"></param>
        /// <returns></returns>
        private bool GetTariffIndexNameById(int tariffIndex, NewTOUProfile TOUProfile, out string tariffIndexName)
        {
            tariffIndexName = string.Empty;
            var tariffProfileList = TOUProfile.tariffProfileList;
            foreach (var item in tariffProfileList)
            {
                if (item.NodeID != tariffIndex)
                    continue;
                tariffIndexName = item.NodeName;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 用来保存查询过的没有得到差值数据的段。key是设备信息，Dictionary<int,List<DateTime>>中的int指int taffiIndex，List<DateTime>表示从这些起始时间开始的费率段的差值计算失败。
        /// </summary>
        private Dictionary<DeviceDataIDDef, Dictionary<int, List<DateTime>>> m_emptyDic =
            new Dictionary<DeviceDataIDDef, Dictionary<int, List<DateTime>>>();
        /// <summary>
        /// 添加一个元素到字典中
        /// </summary>
        /// <param name="deviceDataIDDef"></param>
        /// <param name="startTime"></param>
        /// <param name="taffiIndex"></param>
        private void AddItem(DeviceDataIDDef deviceDataIDDef, DateTime startTime, int taffiIndex)
        {
            if(m_emptyDic==null)
                m_emptyDic = new Dictionary<DeviceDataIDDef, Dictionary<int,List<DateTime>>>();
            Dictionary<int,  List<DateTime>> node;
            if (!m_emptyDic.TryGetValue(deviceDataIDDef, out node))
            {
                node = new Dictionary<int, List<DateTime>>();
                m_emptyDic.Add(deviceDataIDDef, node);
            }
            List<DateTime> timeList;
            if (!node.TryGetValue(taffiIndex, out timeList))
            {
                timeList = new List<DateTime>();
                node.Add(taffiIndex, timeList);
            }
            timeList.Add(startTime);
        }


        /// <summary>
        /// 根据数据类型获取最大值或者差值
        /// </summary>
        /// <param name="value">当前值</param>
        /// <param name="result">返回结果</param>
        /// <param name="dataType">数据类型，最大值或者差值</param>
        /// <returns></returns>
        private double GetDataValueByDataType(ref DateTime tempMaxTime, ref DateTime maxValueTime, double value, double result, StatisticType dataType,ref int countForEnergy)
        {
            if (dataType == StatisticType.StatisticTypeMax)//最大值
            {
                if (!double.Equals(value, double.NaN))
                {
                    if (result < value)
                    {
                        result = value;
                        maxValueTime = tempMaxTime;
                        countForEnergy++;
                    }
                }
            }

            else if (dataType == StatisticType.StatisticTypeTotal)//差值
            {
                if (!double.Equals(value,double.NaN)  )
                {
                    result = result + value;
                    countForEnergy++;
                }
            }
            return result;
        }
        /// <summary>
        /// 获取差值累计
        /// </summary>
        /// <param name="endValue">起始时间值</param>
        /// <param name="startValue">结束时间值</param>
        /// <returns></returns>
        private double GetDiffValue(object endValue, object startValue,DeviceDataIDDef deviceDataID)
        {
            try
            {
                if(endValue==null)
                    return double.NaN;
                if(startValue==null)
                    return double.NaN;
                var result = DefaultTemplatePublicMethod.GetDiffValue(endValue, startValue, deviceDataID);
                double doubleValue;
                if (double.TryParse(result.ToString(), out doubleValue))
                    return doubleValue;
                return double.NaN;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return double.NaN;
            }
       
        }

        /// <summary>
        /// 根据费率段时间对获取单元格的值
        /// </summary>
        /// <param name="dateTimePairList"></param>
        /// <returns></returns>
        private double GetTOUDiffValueByDateTimePairList(DeviceDataIDDef deviceDataIDDef,DateTime startTime, List<DateTimePair> dateTimePairList, SortedDictionary<DateTime, double> dataValueMap,int taffiIndex)
        {
            double value = 0;
            double count = 0;
            foreach (DateTimePair timePair in dateTimePairList)
            {
                //获取单元格其实和结束时间的数据
                DateTime starttime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
                starttime = starttime.AddMinutes(timePair.startTimeMinute);
                DateTime endtime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
                endtime = endtime.AddMinutes(timePair.endTimeMinute);
       
                //求和
                if (deviceDataIDDef.DataID >= 4000429 && deviceDataIDDef.DataID <= 4000433)
                {
                 
                    foreach (var item in dataValueMap)
                    {
                        if (item.Key <= starttime)
                            continue;
                        if (item.Key > endtime)
                            break;
                        if (item.Value == null)
                            continue;
                        double temp;
                        if (!double.TryParse(item.Value.ToString(), out temp))
                            continue;
                        if (temp.Equals(double.NaN))
                            continue;
                      
                        value = value + item.Value;
                        count++;
                    }
                }
                //求差值
                else
                {
                    //分别获取开始和结束时间的数据
                    object startvalue;
                    object endvalue;
                    //deviceDataIDDef.DataID==SysConstDefinition.DATAIDKVARH && timePair.endTimeMinute==900
                    GetStartTime(dataValueMap, starttime, endtime, out startvalue);

                    GetEndTime(dataValueMap, starttime, endtime, out endvalue);

                    var result = GetDiffValue(endvalue, startvalue, deviceDataIDDef);

                    if (result.Equals(double.NaN))
                    {
                        AddItem(deviceDataIDDef, starttime, taffiIndex);
                        continue;
                        //只要其中一段为空，则返回看空，宁可返回空也不返回错误数据
                        //    return double.NaN;
                    }
                    //对于对个时间区间，数值累加
                    value = value + result;
                    count++;
                }
            }
            if(count>0)
              return value;

            return double.NaN;
        }

        private static void GetEndTime(SortedDictionary<DateTime, double> dataValueMap, DateTime starttime, DateTime endtime, out object endvalue)
        {
            endvalue=null;
            foreach (var item in dataValueMap)
            {
                if (item.Value == null)
                    continue;

                if (item.Key < starttime)
                    continue;

                if (item.Key > endtime)
                {
                    break;
                }
                double temp;
                if (!double.TryParse(item.Value.ToString(), out temp))
                    continue;
                if (temp.Equals(double.NaN))
                    continue;
                endvalue = item.Value;
            }
        }

        private static void GetStartTime(SortedDictionary<DateTime, double> dataValueMap, DateTime starttime, DateTime endtime,
           out object startvalue)
        {
            startvalue = null;
            foreach (var item in dataValueMap)
            {
                if (item.Key < starttime)
                    continue;
                if (item.Key > endtime)
                    continue;
                if (item.Value == null)
                    continue;
                double temp;
                if (!double.TryParse(item.Value.ToString(), out temp))
                    continue;
                if (temp.Equals(double.NaN))
                    continue;
                startvalue = item.Value;
                break;
            }
           
        }

        private void GetTimePair(DateTime startTime, DateTimePair timePair,out TimePair startTimeAndEndTime)
        {
            DateTime starttime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
            starttime = starttime.AddMinutes(timePair.startTimeMinute);
            DateTime endtime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
            endtime = endtime.AddMinutes(timePair.endTimeMinute);
            startTimeAndEndTime = new TimePair(starttime, endtime);
        }


        private double GetTOUMaxValueByDateTimePairList(DateTime startTime, List<DateTimePair> dateTimePairList, SortedDictionary<DateTime, double> dataValueMap, ref DateTime maxValueTime)
        {
            double result = double.NaN;
            foreach (DateTimePair timePair in dateTimePairList)
            { //获取单元格起始和结束时间的数据
                TimePair startTimeAndEndTime;
                GetTimePair(startTime, timePair, out startTimeAndEndTime);
                DateTime starttime = startTimeAndEndTime.StartTime;
                DateTime endtime = startTimeAndEndTime.EndTime;
                foreach (var item in dataValueMap)
                {

                    if (item.Key < starttime)
                        continue;
                    if (item.Key >= endtime)
                        break;

                    var startvalue = item.Value;
                    if (startvalue == null)
                        continue;
                    double valueDouble;
                    if (!double.TryParse(startvalue.ToString(), out valueDouble))
                        continue;
                    if (result >= valueDouble)
                        continue;
                    result = valueDouble;
                    maxValueTime = item.Key;
                }
            }
            return result;
        }
        private double GetTOUValueByDateTimePairList(DeviceDataIDDef deviceDataIDDef,DateTime startTime, List<DateTimePair> dateTimePairList, StatisticType dataType, SortedDictionary<DateTime, double> dataValueMap, ref DateTime maxValueTime,int taffiIndex)
        {
            if (dataType == StatisticType.StatisticTypeMax)
            {
                return GetTOUMaxValueByDateTimePairList(startTime, dateTimePairList, dataValueMap, ref maxValueTime);
            }
            else
            {
                return GetTOUDiffValueByDateTimePairList(deviceDataIDDef, startTime, dateTimePairList, dataValueMap, taffiIndex);
            }
        }
        /// <summary>
        /// 根据日时段方案解析该日时段的费率方案
        /// </summary>
        /// <param name="dayProfileIndex"></param>
        /// <param name="dayProfileStruct"></param>
        /// <returns></returns>
        private List<DateTimePair> FindDateTimePairByDayProfileIndex(DateTime startTime, DateTime endTime, int dayProfileIndex, int tariffiIndex, List<DayProfileStruct> dayProfileStruct)
        {
            for (int i = 0; i < dayProfileStruct.Count; i++)
            {
                DayProfileStruct dayProfile = dayProfileStruct[i];
                //根据日时段名称得到在年费率表中的index
                if (dayProfile.periodIndex != dayProfileIndex)
                    continue;
                List<TariffPeriodTime> tariffiPeriodList = dayProfile.periodTime;
                for (int j = 0; j < tariffiPeriodList.Count; j++)
                {
                    //费率时段等于
                    TariffPeriodTime tariffiPeriodTime = tariffiPeriodList[j];
                    //找到日时段下对应的费率时段，返回该费率时段所有时间区间与查询起始时间和结束时间之间的交集
                    if (tariffiPeriodTime.tariffIndex != tariffiIndex)
                        continue;
                    for (int k = 0; k < tariffiPeriodTime.periodTimeList.Count; k++)
                    {
                        DateTimePair timePair = tariffiPeriodTime.periodTimeList[k];
                        DateTime tempTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
                        //如果查询时间的起始时间大于区间时间，则取查询时间
                        DateTime tempStartTime = tempTime.AddMinutes(timePair.startTimeMinute);
                        DateTime tempEndTime = tempTime.AddMinutes(timePair.endTimeMinute);
                        if ((startTime >= tempEndTime) || (endTime <= tempStartTime))
                        {
                            timePair.startTimeMinute = 0;
                            timePair.endTimeMinute = 0;
                        }
                        //查询起始结束时间和TOU起始结束时间取交集
                        else if ((startTime <= tempStartTime) && (endTime < tempEndTime))
                        {
                            timePair.endTimeMinute = endTime.Hour * 60 + endTime.Minute;
                        }
                        else if ((startTime > tempStartTime) && (endTime >= tempEndTime))
                        {
                            timePair.startTimeMinute = startTime.Hour * 60 + startTime.Minute;
                        }
                        else
                        {
                            timePair.startTimeMinute = startTime.Hour * 60 + startTime.Minute;
                            timePair.endTimeMinute = endTime.Hour * 60 + endTime.Minute;
                        }
                    }
                    return tariffiPeriodTime.periodTimeList;
                }
            }
            return new List<DateTimePair>();
        }


        private bool GetUsedDayRate(DateTime startTime,DateTime endTime,NewTOUProfile stationTOUProfile ,out List<int> dayRateUsedList)
        {
            dayRateUsedList = new List<int>();
            if (startTime > endTime)
                return false;
            DateTime startTime2 = GetDate(startTime);
            DateTime  endTime2 = GetDateForEndTime(endTime);
          
            while (startTime2 < endTime2)
            {
                int day = FindPeriodByDateTime(startTime2, stationTOUProfile);
                if(!dayRateUsedList.Contains(day))
                   dayRateUsedList.Add(day);
                startTime2 = startTime2.AddDays(1);
            }
            return true;
        }

        public bool GetUsedTariff(DateTime startTime, DateTime endTime, NewTOUProfile stationTOUProfile, out SortedSet<int> tariffNodeIdUsedList)
        {
            tariffNodeIdUsedList = new SortedSet<int>();
            try
            {
             
                List<int> dayRateUsedList;
                if (!GetUsedDayRate(startTime, endTime, stationTOUProfile, out dayRateUsedList))
                    return false;

                foreach (var item in dayRateUsedList)
                {
                    GetUsedTariffOne(item, stationTOUProfile, ref tariffNodeIdUsedList);
                }
                return true;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
          
        }

        private bool GetUsedTariffOne(int dayIndex, NewTOUProfile stationTOUProfile, ref SortedSet<int> tariffNodeIdUsedList)
        {
            try
            {
                foreach (var item in stationTOUProfile.dayProfileList)
                {
                    if (item.periodIndex != dayIndex)
                        continue;
                    foreach (var node in item.periodTime)
                    {
                        if (!tariffNodeIdUsedList.Contains(node.tariffIndex))
                            tariffNodeIdUsedList.Add(node.tariffIndex);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }


        /// <summary>
        /// 根据查询时间到年费率方案中找到日费率段
        /// </summary>
        /// <param name="queryTime"></param>
        /// <param name="yearProfileList"></param>
        /// <returns></returns>
        private int FindPeriodByDateTime(DateTime queryTime, NewTOUProfile stationTOUProfile)
        {
            // 获取查询时间在当年的第几天
            int dayOfYear = queryTime.DayOfYear;
            //年费率方案的个数
            int yearTouNum = stationTOUProfile.yearProfileList.Count;
            //查找查询时间对应的年方案，默认为最后一年
            EnergyCalenderView tempYearTOUProfile = stationTOUProfile.yearProfileList[yearTouNum - 1];

            //寻找该年对应的年时段
            for (int i = 0; i < yearTouNum; i++)
            {
                if (stationTOUProfile.yearProfileList[i].Year != queryTime.Year)
                    continue;
                    
                tempYearTOUProfile = stationTOUProfile.yearProfileList[i];
                break;
            }
            int result  = (int)tempYearTOUProfile.DailyNodeIDs[dayOfYear - 1];
            return result;
        }

        private double GetDiffOrMaxValueByTime(DeviceDataIDDef deviceDataIDDef, DateTime startTime, DateTime endTime, SortedDictionary<DateTime, double> dataValueMap, int taffiIndex, StatisticType dataType, NewTOUProfile stationTOUProfile, ref DateTime maxValueTime, ref bool IsTrue)
        {
            double result = 0;
            DateTime firstEndTime = DateTime.Now;
            DateTime tempMaxTime = startTime;
            //如果累加的一段数据都没有，就是NaN，不是0
            int countForEnergy = 0;
            while (startTime < endTime)
            {
                //起始、结束时间的值
                //获取查询起始时间和第二天的日时段，由于存在起始时间不是0点，所以要查询两天
                int dayProfileIndex = FindPeriodByDateTime(startTime, stationTOUProfile);
                //费率时段对应的的时间区间序列，由于存在跨天的情况，所以要把时间段分为两部分，起始时间-当天24点，24点-结束时间
                //第一天的结束时间为当天24天
                firstEndTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
                firstEndTime = firstEndTime.AddDays(1);

                List<DateTimePair> dateTimePairList = FindDateTimePairByDayProfileIndex(startTime, firstEndTime, dayProfileIndex, taffiIndex, stationTOUProfile.dayProfileList);
                if (dateTimePairList.Count > 0)
                {
                    double value = GetTOUValueByDateTimePairList(deviceDataIDDef, startTime, dateTimePairList, dataType, dataValueMap, ref tempMaxTime, taffiIndex);
                    result = GetDataValueByDataType(ref tempMaxTime, ref maxValueTime, value, result, dataType, ref countForEnergy);
                }
             
                startTime = startTime.AddDays(1);
            }


            firstEndTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
            firstEndTime = firstEndTime.AddDays(1);
            if (firstEndTime < endTime)
            {
                int nextDayProfileIndex = FindPeriodByDateTime(startTime.AddDays(1), stationTOUProfile);
                //对于非整点，要加上最后一天的非整点时段
                List<DateTimePair> nexDaydateTimePairList = FindDateTimePairByDayProfileIndex(firstEndTime, endTime, nextDayProfileIndex, taffiIndex, stationTOUProfile.dayProfileList);
                double nextDayValue = GetTOUValueByDateTimePairList(deviceDataIDDef, endTime, nexDaydateTimePairList, dataType, dataValueMap, ref tempMaxTime, taffiIndex);
                //两段都不为空，则返回相加
                result = GetDataValueByDataType(ref tempMaxTime, ref maxValueTime, nextDayValue, result, dataType, ref countForEnergy);
            }

            if (countForEnergy == 0 )
            {
                result = double.NaN;
            }

            if (!result.Equals(double.NaN))
                result = DataFormatManager.GetFormattedDoubleByDigits(result, (int)EnergyCostChartDataManager.Retained_decimal_digits);
            return result;
        }

        private double GetMaxOrDiffValueByTime(DeviceDataIDDef deviceDataIDDef, DateTime startTime, DateTime endTime, SortedDictionary<DateTime, double> dataValueMap, int taffiIndex, StatisticType dataType, NewTOUProfile stationTOUProfile, ref DateTime maxValueTime, ref bool IsTrue)
        {

               return GetDiffOrMaxValueByTime(deviceDataIDDef, startTime, endTime, dataValueMap, taffiIndex, dataType,
                    stationTOUProfile, ref maxValueTime, ref IsTrue);
            
        
        }

        private static void GetDataValueMap(List<List<DataLogOriDef>> tempResult, int i, out SortedDictionary<DateTime, double> dataValueMap)
        {
            dataValueMap = new SortedDictionary<DateTime, double>();
            List<DataLogOriDef> dataLogList = tempResult[i];
           
            //由于TOU是15分钟间隔，所以将定时记录中的非15分钟整点间隔提取出来，减少计算量
            double tempValue = 0;
            for (int j = 0; j < dataLogList.Count; j++)
            {
                if (dataValueMap.ContainsKey(dataLogList[j].LogTime))
                    continue;
                dataValueMap.Add(dataLogList[j].LogTime, dataLogList[j].DataValue);
            }
        }

        public bool GetEnergyDemandDataAll(DateTime startTime, DateTime endTime, List<DeviceDataIDDef> queryParamList, NewTOUProfile newTOUProfile, out Dictionary<DeviceWithLoop, Dictionary<EnergyKindWithTariff, DataLogValueDef>> resultDataDic, out  Dictionary<uint, Dictionary<uint, Dictionary<DateTime, double>>> maxDic)
        {
            resultDataDic = new Dictionary<DeviceWithLoop, Dictionary<EnergyKindWithTariff, DataLogValueDef>>();
            maxDic = new Dictionary<uint, Dictionary<uint, Dictionary<DateTime, double>>>();
          
            try
            {
                Dictionary<DeviceDataIDDef, SortedDictionary<DateTime, double>> dataValueMapDic;
                GetDataValue(startTime, endTime, queryParamList, out dataValueMapDic);
                Dictionary<DeviceDataIDDef, SortedDictionary<DateTime,double>> dataValueMapDicEnergy =
                    new Dictionary<DeviceDataIDDef, SortedDictionary<DateTime, double>>();
                Dictionary<DeviceDataIDDef, SortedDictionary<DateTime, double>> dataValueMapDicDemand =
                    new Dictionary<DeviceDataIDDef, SortedDictionary<DateTime, double>>();
                foreach (var item in dataValueMapDic)
                {
                  
                    if (TOUComputer.IsEnergy(item.Key.DataID))
                        dataValueMapDicEnergy.Add(item.Key,item.Value);
                    if (TOUComputer.IsDemand(item.Key.DataID))
                        dataValueMapDicDemand.Add(item.Key,item.Value);
                }

                GetEnergyOrDemandData(dataValueMapDicEnergy,startTime, endTime, newTOUProfile,
                   StatisticType.StatisticTypeTotal, ref resultDataDic);
                GetEnergyOrDemandData(dataValueMapDicDemand,startTime, endTime, newTOUProfile,
                    StatisticType.StatisticTypeMax, ref resultDataDic);

                //获取所有.key是dataId，value中的key是dateTime，value是这个时刻中对应的值
                Dictionary<uint, Dictionary<DateTime, List<double>>> summaryDic = new Dictionary<uint, Dictionary<DateTime, List<double>>>();
                //获取总和中的最大值.key是dataId，value中的key是dateTime，value是这个时刻中对应的值
                GetSummaryDic(dataValueMapDicDemand, out summaryDic);

                //第一个key是dataid，第二个key是某一日的0点时刻值，后面对应的是这一天内的所有时刻的数据
                Dictionary<uint, Dictionary<DateTime, Dictionary<DateTime, List<double>>>> groupByDayDic =
                    new Dictionary<uint, Dictionary<DateTime, Dictionary<DateTime, List<double>>>>();
                //增加一个key，是每一天是一年中的第几天的key，这样可以将每一天分组
                GroupByDay(summaryDic, out groupByDayDic);
                //按时刻所属于的费率段将数据分配到相应的费率段
                //第一个key是dataID，第二个key是tariffIndex，第三个key是时刻所在的日的分组，后面是每日对应的时刻的数据。
                Dictionary<uint, Dictionary<uint, Dictionary<DateTime, List<double>>>> tariffDic;
                GetTariffDic(startTime, endTime, groupByDayDic, newTOUProfile, out tariffDic);
                GetMaxDic(tariffDic, out maxDic);
                return true;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        private void GroupByDay(Dictionary<uint, Dictionary<DateTime, List<double>>> summaryDic,out Dictionary<uint, Dictionary<DateTime, Dictionary<DateTime, List<double>>>> groupByDayDic)
        {
           groupByDayDic =new Dictionary<uint, Dictionary<DateTime, Dictionary<DateTime, List<double>>>>();
            foreach (var dataIDPair in summaryDic)
            {
                uint dataId = dataIDPair.Key;
                if (!groupByDayDic.ContainsKey(dataId))
                    groupByDayDic.Add(dataId, new Dictionary<DateTime, Dictionary<DateTime, List<double>>>());
                foreach (var dateTimeItem in dataIDPair.Value)
                {
                    DateTime currentDateTime = dateTimeItem.Key;
                    DateTime groupDay = GetDate(currentDateTime);
                    var tempNode = groupByDayDic[dataId];
                    if (!tempNode.ContainsKey(groupDay))
                        tempNode.Add(groupDay, new Dictionary<DateTime, List<double>>());
                    var timeNode = tempNode[groupDay];
                    if (!timeNode.ContainsKey(currentDateTime))
                        timeNode.Add(currentDateTime, dateTimeItem.Value);
                }
            }
        }

        private DateTime GetDateForEndTime(DateTime endTime)
        {
            DateTime endTimeLargerArea = GetDate(endTime);
            if (endTimeLargerArea != endTime)
                endTimeLargerArea = endTimeLargerArea.AddDays(1);
            return endTimeLargerArea;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="summaryDic"></param>
        /// <param name="tariffDic">第一个key是dataId，第二个key是例如“峰”的ID，第三个key是时刻值，value是这个时刻所有设备所存在的数据</param>
        private void GetTariffDic(DateTime startTime, DateTime endTime, Dictionary<uint, Dictionary<DateTime, Dictionary<DateTime, List<double>>>> groupByDayDic, NewTOUProfile newTOUProfile, out  Dictionary<uint, Dictionary<uint, Dictionary<DateTime, List<double>>>> tariffDic)
        {
            DateTime startTimeLargerArea = GetDate(startTime);
            DateTime endTimeLargerArea = GetDateForEndTime(endTime);
          
            var tariffiIndex = newTOUProfile.tariffProfileList;
            tariffDic = new  Dictionary<uint, Dictionary<uint,  Dictionary<DateTime, List<double>>>>();


            for (int k = 0; k < tariffiIndex.Count; k++)
            {
                GetOneTariff(startTimeLargerArea, endTimeLargerArea, groupByDayDic, (int)tariffiIndex[k].NodeID, newTOUProfile, ref  tariffDic);
            }
        }
        /// <summary>
        /// 获取这个时间的当日0点
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        private DateTime GetDate(DateTime startTime)
        {
           return  startTime.AddHours(-startTime.Hour)
                   .AddMinutes(-startTime.Minute)
                   .AddSeconds(-startTime.Second)
                   .AddMilliseconds(-startTime.Millisecond);
        }

        private void GetOneTariff(DateTime startTimeLargerArea, DateTime endTimeLargerArea, Dictionary<uint, Dictionary<DateTime, Dictionary<DateTime, List<double>>>> groupByDayDic, int taffiIndex, NewTOUProfile newTOUProfile, ref Dictionary<uint, Dictionary<uint, Dictionary<DateTime, List<double>>>> tariffDic)
        {
            foreach (var dataIdDic in groupByDayDic)
            {
                uint dataId = dataIdDic.Key;
                if (!tariffDic.ContainsKey(dataId))
                    tariffDic.Add(dataId, new Dictionary<uint, Dictionary<DateTime, List<double>>>());
                var tariffItemForOneDataId = tariffDic[dataId];
                if (!tariffItemForOneDataId.ContainsKey((uint) taffiIndex))
                    tariffItemForOneDataId.Add((uint) taffiIndex, new Dictionary<DateTime, List<double>>());
            }


            DateTime startTime = startTimeLargerArea;
            DateTime endTime = endTimeLargerArea;

            while (startTime < endTime)
            {
                //起始、结束时间的值
                //获取查询起始时间和第二天的日时段，由于存在起始时间不是0点，所以要查询两天
                int dayProfileIndex = FindPeriodByDateTime(startTime, newTOUProfile);
                //费率时段对应的的时间区间序列，由于存在跨天的情况，所以要把时间段分为两部分，起始时间-当天24点，24点-结束时间
                //第一天的结束时间为当天24天
                var   firstEndTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0);
                firstEndTime = firstEndTime.AddDays(1);

                List<DateTimePair> dateTimePairList = FindDateTimePairByDayProfileIndex(startTime, firstEndTime, dayProfileIndex, taffiIndex, newTOUProfile.dayProfileList);
                foreach (var item in dateTimePairList )
                {
                    TimePair node;
                    GetTimePair(startTime, item, out node);
                    DateTime startTimeOne=node.StartTime;
                    DateTime endTimeOne = node.EndTime;
                    foreach (var dataIdDic in groupByDayDic)
                    {
                        var groupItem = dataIdDic.Value;
                        Dictionary<DateTime, List<double>> dateTimeDic;
                        DateTime keyDateTime = GetDate(startTime);
                        if (!groupItem.TryGetValue(keyDateTime, out dateTimeDic))
                            continue;
                        foreach (var timeItem in dateTimeDic)
                        {
                          
                            DateTime tempKey = timeItem.Key;
                            if (tempKey < startTimeOne || tempKey >= endTimeOne)
                                continue;
                            if (taffiIndex < 0)
                                continue;

                            var timeDic = tariffDic[dataIdDic.Key][(uint)taffiIndex];
                            timeDic.Add(tempKey, timeItem.Value);
                        }
                    }
                }
                startTime = startTime.AddDays(1);
            }
        }

        private static void GetMaxDic(Dictionary<uint,Dictionary<uint, Dictionary<DateTime, List<double>>>> summaryDic,out Dictionary<uint,Dictionary<uint, Dictionary<DateTime, double>>> maxDic)
        {
            maxDic = new Dictionary<uint,Dictionary<uint, Dictionary<DateTime, double>>>();
            foreach (var DataItem in summaryDic)
            {
                foreach (var tariffItem in DataItem.Value)
                {
                
                    DateTime maxTime = new DateTime();
                    double maxValue = 0;
                    bool hasMax = false;
                    foreach (var dateTimeItem in tariffItem.Value)
                    {
                        double tempTotal = 0;
                        foreach (double point in dateTimeItem.Value)
                        {
                           
                            if (double.IsNaN(point))
                                continue;
                            tempTotal = tempTotal + point;
                        }
                        if (tempTotal <= maxValue)
                            continue;
                     
                        maxTime = dateTimeItem.Key;
                        maxValue = tempTotal;
                        hasMax = true;
                    }
              
                    //无论有没有value，都把key加上
                    if (!maxDic.ContainsKey(DataItem.Key))
                        maxDic.Add(DataItem.Key, new Dictionary<uint, Dictionary<DateTime, double>>());
                    var dateTimeTovalueDic = maxDic[DataItem.Key];
                    if (!dateTimeTovalueDic.ContainsKey(tariffItem.Key))
                        dateTimeTovalueDic.Add(tariffItem.Key, new Dictionary<DateTime, double>());
                    if (!hasMax)
                        continue;
                    var maxKeyPair = dateTimeTovalueDic[tariffItem.Key];
                    maxKeyPair.Add(maxTime, maxValue);
                }
            }
        }

        private static void GetSummaryDic(Dictionary<DeviceDataIDDef, SortedDictionary<DateTime, double>> dataValueMapDicDemand, out Dictionary<uint, Dictionary<DateTime, List<double>>> summaryDic)
        {
            summaryDic = new Dictionary<uint, Dictionary<DateTime, List<double>>>();
            foreach (KeyValuePair<DeviceDataIDDef, SortedDictionary<DateTime, double>> item in dataValueMapDicDemand)
            {
                if (!summaryDic.ContainsKey(item.Key.DataID))
                    summaryDic.Add(item.Key.DataID, new Dictionary<DateTime, List<double>>());
                Dictionary<DateTime, List<double>> dateTimeToValueDic;
                if (!summaryDic.TryGetValue(item.Key.DataID, out dateTimeToValueDic))
                    continue;
                foreach (var oneValue in item.Value)
                {
                    if (!dateTimeToValueDic.ContainsKey(oneValue.Key))
                        dateTimeToValueDic.Add(oneValue.Key, new List<double>());
                    List<double> valueList;
                    if (!dateTimeToValueDic.TryGetValue(oneValue.Key, out valueList))
                        continue;
                    valueList.Add(oneValue.Value);
                }
            }
        }

        private void GetDataValue(DateTime startTime, DateTime endTime, List<DeviceDataIDDef> queryParamList, out Dictionary<DeviceDataIDDef, SortedDictionary<DateTime, double>> dataValueMapDic)
        {
            dataValueMapDic = new Dictionary<DeviceDataIDDef, SortedDictionary<DateTime, double>>();
              var  tempResult = HistoryDataQueryManager.DataManager.QueryHistoryTrendDataLogView(queryParamList, startTime, endTime.AddSeconds(1));
           
            //对于一个DeviceDataIDDef
              int i = 0;
              foreach (var item in queryParamList)
            {
                SysNode deviceNode = PecsNodeManager.PecsNodeInstance.GetDeviceNodeByID(item.DeviceID);
                if (deviceNode == null)
                    continue;
                //定义日报测点数据字典，通过时间索引来查找对应的数据
                SortedDictionary<DateTime, double> dataValueMap;
                GetDataValueMap(tempResult, i, out dataValueMap);
                dataValueMapDic.Add(item, dataValueMap);
                ++i;
            }
           
        }


        /// <summary>
        /// 获取energy&demand报表数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="queryParamList"></param>
        /// <param name="resultDT"></param>
        private void GetEnergyOrDemandData(Dictionary<DeviceDataIDDef, SortedDictionary<DateTime, double>> dataValueMapDic, DateTime startTime, DateTime endTime, NewTOUProfile newTOUProfile, StatisticType dataType, ref Dictionary<DeviceWithLoop, Dictionary<EnergyKindWithTariff, DataLogValueDef>> resultDataDic)
        {
    
            foreach (var item in dataValueMapDic)
            {
                SysNode deviceNode = PecsNodeManager.PecsNodeInstance.GetDeviceNodeByID(item.Key.DeviceID);
                if (deviceNode == null)
                    continue;
                //定义日报测点数据字典，通过时间索引来查找对应的数据
                SortedDictionary<DateTime, double> dataValueMap;
       
                dataValueMap = item.Value;

                uint deviceNodeID = deviceNode.NodeID;
                var tariffiIndex = newTOUProfile.tariffProfileList;
                for (int k = 0; k < tariffiIndex.Count; k++)
                {
                    var deviceWithLoopNode = new DeviceWithLoop(deviceNodeID, item.Key.LogicalDeviceIndex);
                    if (!resultDataDic.ContainsKey(deviceWithLoopNode))
                        resultDataDic.Add(deviceWithLoopNode, new Dictionary<EnergyKindWithTariff, DataLogValueDef>());
                    Dictionary<EnergyKindWithTariff, DataLogValueDef> energyOrDemandDic;
                    if (!resultDataDic.TryGetValue(deviceWithLoopNode, out energyOrDemandDic))
                        continue;
                    var energyWithTariff = new EnergyKindWithTariff(item.Key.DataID, tariffiIndex[k].NodeID, dataType);
                    int tafiffIndex =(int) tariffiIndex[k].NodeID;
                    DateTime maxValueTime = startTime;
                    bool IsTrue = true;
                    double total = GetMaxOrDiffValueByTime(item.Key,startTime, endTime, dataValueMap, (int)tariffiIndex[k].NodeID, dataType, newTOUProfile, ref maxValueTime, ref IsTrue);
                    var datalogNodeOne = new DataLogValueDef(maxValueTime, total);
                    energyOrDemandDic.Add(energyWithTariff, datalogNodeOne);
                }
            }
         
        }

  



    }
}
