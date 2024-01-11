using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using OfficeReportInterface.DefaultReportInterface;
using OfficeReportInterface.DefaultReportInterface.CommonluUsed;
using OfficeReportInterface.DefaultReportInterface.IntelligentSafety;
using CET.PecsNodeManage;
using BasicDataInterface.Models.Response;
using BasicDataInterface;

namespace OfficeReportInterface
{

    /// <summary>
    ///PowerQualityDataManager 的摘要说明
    /// </summary>
    public class PowerQualityDataManager : IDataSheet
    {
        #region 填充数据

        private uint source = (uint) RepServFileType.PowerQuality;
        private int _maxNumberOfEvents;

        public const string _NothingString = EventInformation._NothingString;

        public void SetMaxNumberOfEvents(int maxNumber)
        {
            _maxNumberOfEvents = maxNumber;
        }

        public void SetSource(uint sourceTemp)
        {
            source = sourceTemp;
        }

        public PowerQualityDataManager()
        {
            _maxNumberOfEvents = 30;
            AddDurationDataId();

        }

        private CurveType GetCurveTypeByInput(DefaultReportParameter parameter)
        {
            if (parameter.IsITIC)
                return CurveType.ITIC;
            return CurveType.SEMI100s;
        }

        /// <summary>
        /// 返回Power Quality报表模板所有数据
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public List<DataTable> GetDataLogDatas(DefaultReportParameter parameter)
        {
            List<DataTable> resultDTList = new List<DataTable>();

            try
            {
                //警告信息
                DataTable warningDT = DefaultTemplatePublicMethod.ConstructWarningTable(parameter.IsIncludeWarning);
                //汇总信息
                DataTable summaryDT = ConstructDataSummaryTable(parameter);

                resultDTList.Add(warningDT);
                resultDTList.Add(summaryDT);

                //调用PowerQualityInterface的接口，获取事件的相关数据
                List<SagSwellEvent> actual;
                PowerQualityInterfaceManager.GetInstance().GetPowerQualityEvents(_maxNumberOfEvents, parameter, out actual);

                //如果事件条数超过_maxNumberOfEvents，则给出警告信息提示
                AddWarningForEventCountLimit(parameter.IsIncludeWarning, actual.Count, ref warningDT);
                //只取前面_maxNumberOfEvents条事件
                LimitEventCount(ref actual);

                //获取最坏事件.key是triggerTime，value是该triggerTime的所有最坏事件的eventId
                Dictionary<string, List<uint>> worstCaseDic;
                CurveType curveType = GetCurveTypeByInput(parameter);
                RMSAnalysisManager.GetAllWorstCase(curveType, actual, out worstCaseDic);

                //-----------------------------------填充事件信息的内容--------------------------------------------------------------
                DataTable evernGroupDT;
                int eventNumber;
                int groupNumber;
                Dictionary<string, List<int>> eventTypeDic;
                GetEventResult(actual, worstCaseDic, out evernGroupDT, out groupNumber, out eventNumber, curveType, out eventTypeDic, parameter.IsIncludeAvg);

                //-----------------------------填写ITIC/SEMI曲线数据到evernGroupDT-----------------------------------------------------
                //获取曲线数据
                SARFIChartCurve chartCurve = new SARFIChartCurve(GetCurveType(parameter.IsITIC), parameter.curveID);
                if (SafetyDataManager.hasIEMSWeb)
                {
                    WebGeneralResult<SARFIChartCurve> curveResult = GetSARFIChartFeatureValues(parameter.userID, GetCurveType(parameter.IsITIC), parameter.curveID);
                    chartCurve.CurveName = curveResult.ResultList[0].CurveName;
                    chartCurve.UpLineValues = curveResult.ResultList[0].UpLineValues;
                    chartCurve.DownLineValues = curveResult.ResultList[0].DownLineValues;
                }
                else
                {
                    var curveResult = RMSAnalysisManager.GetSARFIChartFeatureValues(GetCurveType(parameter.IsITIC), parameter.curveID, parameter.userID);
                    chartCurve.CurveName = curveResult[0].CurveName;
                    chartCurve.UpLineValues = curveResult[0].UpLineValues;
                    chartCurve.DownLineValues = curveResult[0].DownLineValues;
                }

                //确保有足够的行
                int lineCountNeeded = chartCurve.UpLineValues.Count;
                if (chartCurve.DownLineValues != null && chartCurve.DownLineValues.Count > lineCountNeeded)
                    lineCountNeeded = chartCurve.DownLineValues.Count;

                lineCountNeeded = lineCountNeeded + 4; // 第一行写 数字的个数 ，第二行写 标题 ，第三行写 例如 UpLineValues 第四行 写 X 或 Y  第5行及后面行 写 值
                if (evernGroupDT.Rows.Count < (lineCountNeeded)) //如果现有的行数不够，则添加行
                {
                    for (int i = 0; i < lineCountNeeded; i++)
                    {
                        DataRow oneRow = evernGroupDT.NewRow();
                        evernGroupDT.Rows.Add(oneRow);
                    }
                }
                //填写曲线数据

                //第一行  写曲线点个数
                evernGroupDT.Rows[0]["UpLineValues_X"] = chartCurve.UpLineValues.Count;
                evernGroupDT.Rows[0]["UpLineValues_Y"] = chartCurve.UpLineValues.Count;
          
                if (chartCurve.DownLineValues != null && chartCurve.DownLineValues.Count > 0)
                {
                    evernGroupDT.Rows[0]["DownLineValues_X"] = chartCurve.DownLineValues.Count;
                    evernGroupDT.Rows[0]["DownLineValues_Y"] = chartCurve.DownLineValues.Count;
                }
                else
                {
                    evernGroupDT.Rows[0]["DownLineValues_X"] = 0;
                    evernGroupDT.Rows[0]["DownLineValues_Y"] = 0;
                }

                //X轴的最大值。如果是ITIC，则取两条曲线的最大横坐标的较大者（向上取整）；如果是SEMI，取一条曲线的最大值（向上取整）
                evernGroupDT.Rows[0]["UpLineValues_X_Line"] = parameter.IsITIC && chartCurve.DownLineValues != null ? Math.Max(Math.Ceiling(GetxLineValue(chartCurve.UpLineValues[chartCurve.UpLineValues.Count - 1].XValue, parameter.IsITIC)), Math.Ceiling(GetxLineValue(chartCurve.DownLineValues[chartCurve.DownLineValues.Count - 1].XValue, parameter.IsITIC))) : Math.Ceiling(GetxLineValue(chartCurve.UpLineValues[chartCurve.UpLineValues.Count - 1].XValue,parameter.IsITIC));
                //X轴的最小值。如果是ITIC，则取两条曲线的最小横坐标的较小者（向下取整）；如果是SEMI，取一条曲线的最小值（向下取整）
                evernGroupDT.Rows[0]["DownLineValues_X_Line"] = parameter.IsITIC && chartCurve.DownLineValues != null ? Math.Min(Math.Floor(GetxLineValue(chartCurve.UpLineValues[0].XValue, parameter.IsITIC)), Math.Floor(GetxLineValue(chartCurve.DownLineValues[0].XValue, parameter.IsITIC))) : Math.Floor(GetxLineValue(chartCurve.UpLineValues[0].XValue,parameter.IsITIC));//X轴的最小值

                //第二行  写标题 例如 Disturbances [1996 CBEMA - ITIC]
                evernGroupDT.Rows[1]["UpLineValues_X"] = string.Format("Disturbances [{0}]", chartCurve.CurveName);
                //Y轴的最大值。如果是ITIC，则是500；如果是SEMI，则是100
                evernGroupDT.Rows[1]["UpLineValues_X_Line"] = parameter.IsITIC ? 500 : 100;
                //Y轴的最小值。
                evernGroupDT.Rows[1]["DownLineValues_X_Line"] = 0;

                //第三行  写 例如 UpLineValues
                evernGroupDT.Rows[2]["UpLineValues_X"] = "UpLineValues_X";
                evernGroupDT.Rows[2]["UpLineValues_Y"] = "UpLineValues_Y";
                evernGroupDT.Rows[2]["DownLineValues_X"] = "DownLineValues_X";
                evernGroupDT.Rows[2]["DownLineValues_Y"] = "DownLineValues_Y";
                //第四行  写 X 或 Y
                evernGroupDT.Rows[3]["UpLineValues_X"] = "X";
                evernGroupDT.Rows[3]["UpLineValues_Y"] = "Y";
                evernGroupDT.Rows[3]["DownLineValues_X"] = "X";
                evernGroupDT.Rows[3]["DownLineValues_Y"] = "Y";

                //第5行及后面行 写 值
                for (int i = 0; i < chartCurve.UpLineValues.Count; i++)
                {
                    var xValue = chartCurve.UpLineValues[i].XValue;
                    evernGroupDT.Rows[4 + i]["UpLineValues_X"] = xValue;
                    evernGroupDT.Rows[4 + i]["UpLineValues_Y"] = chartCurve.UpLineValues[i].YValue;
                    evernGroupDT.Rows[4 + i]["UpLineValues_X_Line"] = GetxLineValue(xValue, parameter.IsITIC); 
             
                }
                if (chartCurve.DownLineValues != null && chartCurve.DownLineValues.Count > 0 &&parameter.IsITIC) //只有ITIC有downLine，SEMI没有DownLine
                {
                    for (int i = 0; i < chartCurve.DownLineValues.Count; i++)
                    {
                        var xValue = chartCurve.DownLineValues[i].XValue;
                        evernGroupDT.Rows[4 + i]["DownLineValues_X"] =xValue;
                        evernGroupDT.Rows[4 + i]["DownLineValues_Y"] = chartCurve.DownLineValues[i].YValue;
                        evernGroupDT.Rows[4 + i]["DownLineValues_X_Line"] = GetxLineValue(xValue, parameter.IsITIC); 
                    }
                }
                //-----------------------------填充汇总信息的数据-----------------------------------------------------------------------------
                FillIncidentsAndEventNumber(groupNumber, eventNumber, ref summaryDT, eventTypeDic);

                //获取波形数据表
                List<DataTable> WaveFormDT;
                if (source == (uint) RepServFileType.PowerQuality)
                    WaveFormDT = GetWaveDataTable(parameter.userID, actual);
                    //Power Quality Events Only不需要获取波形
                else
                    WaveFormDT = new List<DataTable>();
                resultDTList.Add(evernGroupDT);
                resultDTList.AddRange(WaveFormDT);
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }
            return resultDTList;
        }
        /// <summary>
        /// 计算横坐标的值
        /// </summary>
        /// <param name="xValue"></param>
        /// <param name="isITIC"></param>
        /// <returns></returns>
        private static double GetxLineValue(double xValue, bool isITIC)
        {
            return isITIC ? Math.Pow(10, xValue) / 1000 : xValue / 1000;
        }
        #region 填充单元格

        /// <summary>
        /// 添加告警信息
        /// </summary>
        /// <param name="isIncludeWarning"></param>
        /// <param name="count"></param>
        /// <param name="warningDT"></param>
        private void AddWarningForEventCountLimit(bool isIncludeWarning, int count, ref DataTable warningDT)
        {
            if (count > _maxNumberOfEvents && isIncludeWarning)
            {
                DataRow warningDR = warningDT.NewRow();
                string tempFormat = LocalResourceManager.GetInstance() .GetString("0590", "Events more than {0},only show first {0} events.");
                warningDR[0] = string.Format(tempFormat, _maxNumberOfEvents);
                warningDR[1] = DateTime.Now.ToString();
                warningDT.Rows.Add(warningDR);
            }
        }

        private void LimitEventCount(ref List<SagSwellEvent> actual)
        {
            if (_maxNumberOfEvents < actual.Count)
            {
                var actualTemp = new List<SagSwellEvent>();
                for (int i = 0; i < _maxNumberOfEvents; ++i)
                {
                    actualTemp.Add(actual[i]);
                }
                actual = actualTemp;
            }
        }

        private void FillIncidentsAndEventNumber(int groupNumber, int eventNumber, ref DataTable summaryDT, Dictionary<string, List<int>> eventTypeDic)
        {
            summaryDT.Rows[2][0] = LocalResourceManager.GetInstance().GetString("0107", "Total No. of Incidents");
            summaryDT.Rows[2][1] = groupNumber;

            summaryDT.Rows[3][0] = LocalResourceManager.GetInstance().GetString("0108", "Total No. of Events");
            summaryDT.Rows[3][1] = eventNumber;

            summaryDT.Rows[4][0] = LocalResourceManager.GetInstance().GetString("0109", "No. of Sag");
            int numberOfSag = GetNumberOfType(eventTypeDic, LocalResourceManager.GetInstance().GetString("0043", "Sag"));
            summaryDT.Rows[4][1] = numberOfSag;

            summaryDT.Rows[5][0] = LocalResourceManager.GetInstance().GetString("0110", "No. of Swell");
            int numberOfSwell = GetNumberOfType(eventTypeDic, LocalResourceManager.GetInstance().GetString("0049", "Swell"));
            summaryDT.Rows[5][1] = numberOfSwell;

            summaryDT.Rows[6][0] = LocalResourceManager.GetInstance().GetString("0111", "No. of Transient");
            int numberOfTrans = GetNumberOfType(eventTypeDic, LocalResourceManager.GetInstance().GetString("0050", "Transient"));
            summaryDT.Rows[6][1] = numberOfTrans;

            summaryDT.Rows[7][0] = LocalResourceManager.GetInstance().GetString("0600", "No. of Interruptions");
            int numberOfInter = GetNumberOfType(eventTypeDic, LocalResourceManager.GetInstance().GetString("0083", "Interruption"));
            summaryDT.Rows[7][1] = numberOfInter;

            summaryDT.Rows[8][0] = LocalResourceManager.GetInstance().GetString("0601", "No. of Others");
            int numberOfOthers = eventNumber - numberOfSag - numberOfSwell - numberOfTrans - numberOfInter;
            summaryDT.Rows[8][1] = numberOfOthers;
        }

        private static int GetNumberOfType(Dictionary<string, List<int>> eventTypeDic, string eventType)
        {
            List<int> items;
            if (!eventTypeDic.TryGetValue(eventType, out items))
                return 0;
            return items.Count;
        }

        /// <summary>
        /// 用于保存波形的id和name
        /// </summary>
        public struct IDToName
        {
            public string id;
            public string name;

            public IDToName(string id,string name)
            {
                this.id = id;
                this.name = name;
            }
        }

        private List<DataTable> GetWaveDataTable(uint userID,List<SagSwellEvent> eventList)
        {
            const int offset = 250;
            List<DataTable> waveFormDTList = new List<DataTable>();
            for (int i = 0; i < eventList.Count; i++)
            {
                DataTable waveFormDT = new DataTable();
                try
                {
                    waveFormDTList.Add(waveFormDT);
                    var item = eventList[i];
                    //获取数据库的数据
                    TotalCombinedWave totalCombinedWave;
                    HistoryDataQueryManager.DataManager.QueryOriginalWaveDataOfEvent(userID,item, offset, out totalCombinedWave);
                    //获取triggerTime
                    DateTime triggerTime;
                    if (!DateTime.TryParse(item.TriggerTime, out triggerTime))
                        continue;
                    List<IDToName> idNameOoaDic;
                    GetIdNameDic(totalCombinedWave, out idNameOoaDic);
                    ConcstructWaveTable(waveFormDT, totalCombinedWave, i, idNameOoaDic);
                }
                catch (Exception ex)
                {
                    DbgTrace.dout(ex.Message + ex.StackTrace);
                }
            }
            return waveFormDTList;
        }

        private DataTable GetNewEventGroupDt()
        {
            DataTable resultDT = new DataTable("EventGroupDataTable");
            //-----------------用于画事件表格--------------------------------------------------------------
            //设备ID
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("Index", "String"));
            //波形列
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("WaveForm", "String"));
            //设备名称
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("DeviceName", "String"));
            //录波时间
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("RecordTime", "String"));
            //事件类型
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("EventType", "String"));
            //相别
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("Direction", "String"));
            //最大特征值
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("TotalMag", "String"));
            //持续时间
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("Duration", "String"));
            //录波实际时间
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("RecordTimeReal", "String"));
            //录波实际时间
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("MsecReal", "String"));
            //标记是否是最坏事件，如果是最坏事件，填写“IsWorstCase”，否则填写“IsNotWorstCase”
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("WorstCase", "String"));
            //所在的区域，填写A，B，C
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("Region", "String"));
            //打点的时候用到的Mag值，是负数的话会被转换为正数
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("PointMagValue", "String"));
            //显示到打点位置的浮动信息
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("FullInformation", "String"));

            //-----------------用于画ITIC，或者SEMI曲线-----------------------------------------------------------------------
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("UpLineValues_X", "String"));
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("UpLineValues_Y", "String"));

            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("DownLineValues_X", "String"));
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("DownLineValues_Y", "String"));

            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("UpLineValues_X_Line", "String"));//曲线横坐标的显示，ITIC的是 return Math.pow(10, this.value) / 1000; SEMI的是return this.value / 1000; 
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("DownLineValues_X_Line", "String"));
            //---------------------------------------------------------------------------------------
            return resultDT;
        }

        private const string NEXT = "Next";

        /// <summary>
        /// 获取字符串，例如，Incident2 2013-09-12 03:12:52.554 (Worst Case: Event6)
        /// </summary>
        /// <param name="groupIndex"></param>
        /// <param name="eventIndex"></param>
        /// <param name="triggerTime"></param>
        /// <returns></returns>
        private string GetEventWorstCaseString(int groupIndex, int eventIndex, String triggerTime, List<SagSwellEvent> actual, Dictionary<string, List<uint>> worstCaseDic, Dictionary<uint, int> eventIdToIndexDic)
        {
            //获取0596=(Worst Case: {0})  0597=Event {0};
            string worstEventList = GetWorstCaseString(triggerTime, worstCaseDic, eventIdToIndexDic);

            //0593=Incident{0} {1} (Worst Case: Event{2})
            string format = LocalResourceManager.GetInstance() .GetString("0598", "Incident {0} {1} {2}");
            DateTime triggerTimeDateTime;
            string showTriggerTime = triggerTime;
            if (DateTime.TryParse(triggerTime, out triggerTimeDateTime))
            {
                showTriggerTime = triggerTimeDateTime.ToString(DataManager.TimeFormatWithMinisecondForC);
            }
            return string.Format(format, groupIndex, showTriggerTime, worstEventList);
        }

        private static string GetWorstCaseString(String triggerTime, Dictionary<string, List<uint>> worstCaseDic, Dictionary<uint, int> eventIdToIndexDic)
        {
            List<uint> worstList;
            if (!worstCaseDic.TryGetValue(triggerTime, out worstList))
                return string.Empty;
            //Event 1; Event 2; Event 4; Event 6;...
            var result = GetEventNList(eventIdToIndexDic, worstList);
            //(Worst Case: Event 1; Event 2; Event 4; Event 6;...)
            result = GetWorstCaseTotal(result);
            return result;
        }

        private static string GetWorstCaseTotal(string result)
        {
            if (string.IsNullOrEmpty(result.Trim()))
                return string.Empty;
            string worstCaseTipFormat = LocalResourceManager.GetInstance()
                .GetString("0599", "(Worst Case:{0})");
            result = string.Format(worstCaseTipFormat, result);
            return result;
        }

        private static string GetEventNList(Dictionary<uint, int> eventIdToIndexDic, List<uint> worstList)
        {
            string result = string.Empty;
            string eventListStringFormat = LocalResourceManager.GetInstance()
                .GetString("0597", "Event {0};");
            foreach (var item in worstList)
            {
                int index;
                if (!eventIdToIndexDic.TryGetValue(item, out index))
                    continue;
                var tempEventString = string.Format(eventListStringFormat, index);
                result = string.Format("{0} {1}", result, tempEventString);
            }
            result = string.Format("{0} ", result);
            return result;
        }

        private string GetIncidentNString(int groupIndex)
        {
            string format = LocalResourceManager.GetInstance().GetString("0594", "Incident {0}");
            return string.Format(format, groupIndex);
        }

        private string GetWaveFormOrEmptyString(bool hasWave)
        {
            if (hasWave)
                return LocalResourceManager.GetInstance().GetString("0079", "Waveform");
            return EventInformation._NothingString;
        }

        /// <summary>
        /// 获取事件信息的单元格集合
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="worstCaseDic"></param>
        /// <param name="evernGroupDT"></param>
        /// <param name="groupNumber"></param>
        /// <param name="eventNumber"></param>
        /// <param name="curveType"></param>
        /// <param name="eventTypeDic"></param>
        /// <param name="isUseEndTime"></param>
        private void GetEventResult(List<SagSwellEvent> actual, Dictionary<string, List<uint>> worstCaseDic, out DataTable evernGroupDT, out int groupNumber, out int eventNumber, CurveType curveType, out Dictionary<string, List<int>> eventTypeDic, bool isUseEndTime)
        {
            eventTypeDic = new Dictionary<string, List<int>>();
            //分配列空间
            evernGroupDT = GetNewEventGroupDt();
            //添加第一行的Next和yyyy-MM-dd HH:mm:ss.000
            AddEventGroupFirstRow(ref evernGroupDT);
            //key是EventId，value是处于事件顺序中的标号。
            Dictionary<uint, int> eventIdToIndexDic = GetEventIdToIndexDic(actual);

            //依次添加每个分组
            int groupIndex = 0;
            int eventIndex = 0;
            string lastTrigerTime = string.Empty;
            int count = actual.Count;
            for (int i = 0; i < count; ++i)
            {
                var eventItem = actual[i];
                ++eventIndex;
                if (lastTrigerTime != eventItem.TriggerTime)
                {
                    ++groupIndex;
                    lastTrigerTime = eventItem.TriggerTime;

                    //添加一行Incident1 2013-09-15 17:47:29.224(Worst Case: Event1)，Incident 1
                    AddEventGroupOneLine(ref evernGroupDT, groupIndex, eventIndex, lastTrigerTime, actual, worstCaseDic, eventIdToIndexDic);
                    //添加一个空行
                    AddEventGroupEmptyRow(ref evernGroupDT);
                    //添加Event，WF Recording等，这行用于描述列的内容的标题行
                    AddEventGroupTitleLine(ref evernGroupDT);
                }
                //添加一行事件内容
                AddEventGroupOneEvent(ref evernGroupDT, eventIndex, isUseEndTime, eventItem, worstCaseDic, curveType, ref eventTypeDic);
                //如果（是最后一个事件），或者（不是最后一个事件，但是后面一个事件的TraggerTime不相同），则：添加Next行
                AddEventGroupNextRows(ref evernGroupDT, i, actual);
            }
            //传出组的个数和事件的个数
            groupNumber = groupIndex;
            eventNumber = eventIndex;
        }

        private static Dictionary<uint, int> GetEventIdToIndexDic(List<SagSwellEvent> actual)
        {
            //key是EventId，value是处于事件顺序中的标号。
            Dictionary<uint, int> eventIdToIndexDic = new Dictionary<uint, int>();
            int eventIndex = 0;
            foreach (var item in actual)
            {
                ++eventIndex;
                eventIdToIndexDic.Add(item.EventID, eventIndex);
            }
            return eventIdToIndexDic;
        }

        private static void AddEventGroupNextRows(ref DataTable evernGroupDT, int i, List<SagSwellEvent> actual)
        {
            int count = actual.Count;
            //如果是最后一个元素，直接加上Next行
            if (i == (count - 1))
            {
                AddEventGroupNextRow(ref evernGroupDT);
            }
            //如果不是最后一个元素，而下一个元素分组不同
            if (i < (count - 1))
            {
                if (actual[i + 1].TriggerTime != actual[i].TriggerTime)
                {
                    AddEventGroupNextRow(ref evernGroupDT);
                }
            }
        }

        /// <summary>
        /// double值保留3位小数，转换成字符串
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private string GetStingByDouble3Point(double n)
        {
            return CommonUsedFunctionsForWorstCase.GetStingByDouble3Point(n);
        }

        private string GetStringByString(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                return EventInformation._NothingString;
            }
            return description;
        }

        /// <summary>
        /// 添加一行事件的内容
        /// </summary>
        /// <param name="evernGroupDT"></param>
        /// <param name="eventIndex"></param>
        /// <param name="isUseEndTime"></param>
        /// <param name="eventItem"></param>
        /// <param name="worstCaseDic"></param>
        /// <param name="curveType"></param>
        /// <param name="eventTypeDic"></param>
        private void AddEventGroupOneEvent(ref DataTable evernGroupDT, int eventIndex, bool isUseEndTime, SagSwellEvent eventItem, Dictionary<string, List<uint>> worstCaseDic, CurveType curveType, ref Dictionary<string, List<int>> eventTypeDic)
        {
            //-----------------添加事件这行的数据-----------------------------------------------------------------------------------------------------------
            DataRow oneRow = evernGroupDT.NewRow();
            string eventTypeName;
            GetEventTypeName(eventItem.EventCode1, eventItem.FetureInfo.TotalMag, eventItem.EventType, out eventTypeName);
            //-----------------获取节点名称------------------------------------------------------------------------------------------------------------------------------
            string NodeName = string.Empty;
            var node=PecsNodeManager.PecsNodeInstance.GetNodeByTypeID(SysNodeType.PECSDEVICE_NODE, eventItem.DeviceID);//获取设备节点的名称
            if(node!=null)
                NodeName=node.NodeName;
            NodeName = GetStringByString(NodeName);
            //时间是取起始时间还是结束时间
            string time = GetStringByString(eventItem.EventTime);
            if (isUseEndTime)
                time = GetStringByString(eventItem.FetureInfo.FeatureTime);
            string eventType = RMSAnalysisManager.DataManager.GetEventTypeByCode1(eventItem.EventType, eventItem.EventCode1, eventItem.FetureInfo.TotalMag);
            string mag = GetStingByDouble3Point(eventItem.FetureInfo.TotalMag);
            string duration = CommonUsedFunctionsForWorstCase.GetSaved3Or6PointForDurationStr(eventItem.FetureInfo.Duration);

            oneRow["Index"] = eventIndex;
            oneRow["WaveForm"] = GetWaveFormOrEmptyString(eventItem.HaveWave);
            oneRow["DeviceName"] = NodeName;
            oneRow["RecordTime"] = time;

            oneRow["EventType"] = eventType;
            oneRow["Direction"] = GetStringByString(eventItem.FetureInfo.Direction);
            oneRow["TotalMag"] = mag;
            oneRow["Duration"] = duration;
            //是匹配的时间
            oneRow["RecordTimeReal"] = eventItem.FetureInfo.FeatureTime;
            oneRow["MsecReal"] = eventItem.FetureInfo.FeatureTime;

            oneRow["WorstCase"] = GetStringForWorstCase(IsWorstCase(eventItem, worstCaseDic));
            oneRow["Region"] = GetRegionString(eventItem, curveType);
            oneRow["PointMagValue"] = GetPointMagValue(eventItem.FetureInfo.TotalMag);
            oneRow["FullInformation"] = GetFullInformation(NodeName, time, eventType, mag, duration);
            evernGroupDT.Rows.Add(oneRow);
            //统计eventType个数
            AddCountForEventType(eventTypeDic, eventType);
            //---------------添加事件详细信息----------------------------------------------------------------------------------------------------
            List<List<string>> detailForEvent = new List<List<string>>();
            //
            if (SafetyDataManager.hasIEMSWeb)
            {
                detailForEvent = eventItem.Details;
            }
            else
            {
                detailForEvent = GetEventDetals(eventItem);
            }

            for (int j = 0; j < detailForEvent.Count; ++j)//遍历填写每个数据
            {
                var eventDetailRow = detailForEvent[j];
                var cellRow = evernGroupDT.NewRow();//新的一行
                cellRow[0] = string.Empty;//空格
                cellRow[1] = string.Empty;//空格
                for (int i = 0; i < eventDetailRow.Count; i++)//添加一行的数据
                {
                    cellRow[2+i] ="'"+ eventDetailRow[i];  //加上单引号表示是文本，这样显示出来的就是保留三位小数，免得要在VBA中再次设置数据保留小数位数
                }
                cellRow["FullInformation"] = "detail";
                evernGroupDT.Rows.Add(cellRow);
            }
        }

        private static void AddCountForEventType(Dictionary<string, List<int>> eventTypeDic, string eventType)
        {
            List<int> items;
            if (!eventTypeDic.TryGetValue(eventType, out items))
            {
                items = new List<int>();
                eventTypeDic.Add(eventType, items);
            }
            items.Add(1);
        }


        /// <summary>
        /// 显示到浮动信息中的，如果是负数，就要写成
        /// </summary>
        /// <param name="totalMag"></param>
        /// <returns></returns>
        private string GetMagForFloat(string mag)
        {
            return CommonUsedFunctionsForWorstCase.GetMagForFloat(mag);
        }

        private object GetFullInformation(string NodeName, string time, string eventType, string mag, string duration)
        {
            string infor = string.Format(
                "Node: {0}\nTime: {1}\nEvent Type: {2}\nMagnitude (%): {3}\nDuration (ms): {4}",
                NodeName,
                time,
                eventType, GetMagForFloat(mag), duration
                );
            return infor;
        }

        private string GetPointMagValue(double totalMag)
        {
            double mag = CommonUsedFunctionsForWorstCase.GetChangedTotalMagForPoint(totalMag);
            return GetStingByDouble3Point(mag);
        }

        /// <summary>
        /// 返回要使用的打点的图片名称，例如D:\CET\iemsR9\iEMSWeb\-WebService\CGI\DotImage\ASEMI100s.png   中的  ASEMI100s
        /// </summary>
        /// <param name="eventItem"></param>
        /// <param name="curveType"></param>
        /// <returns></returns>
        private string GetRegionString(SagSwellEvent eventItem, CurveType curveType)
        {
            if(SafetyDataManager.hasIEMSWeb)
            {
                var outOfLimit = eventItem.FetureInfo.OutOfLimit;
                if ((int)curveType == (int)CurveType.ITIC) //说明是在ITIC中
                {
                    if (outOfLimit == -1)
                        return LocalResourceManager.GetInstance().GetString("1261", "Others");
                    else if (outOfLimit == 0)
                        return  "A";
                    else if (outOfLimit == 1)
                        return "B";
                    else if (outOfLimit == 2)
                        return "C";
                    else return "Unknown Region";
                }
                else //说明是在SEMI中
                {
                    if (outOfLimit == -1)
                        return LocalResourceManager.GetInstance().GetString("1261", "Others");
                    else if (outOfLimit == 0)
                        return "ASEMI100s";
                    else if (outOfLimit == 1)
                        return  "C";
                    else if (outOfLimit == 2)
                        return "BSEMI100s";
                    else return "Unknown Region";
                }
            }
            else
            {
                string region = _NothingString;
                IWorstCaseManager worstCaseManager;
                if (!RMSAnalysisManager.GetWorstCaseManagerObject(curveType, out worstCaseManager))
                    return region;
                worstCaseManager.GetRegion(eventItem, out region);
                if (curveType == CurveType.SEMI100s)
                    region = string.Format("{0}SEMI100s", region);
                return region;
            }
        }


        private static string GetStringForWorstCase(bool isworstCase)
        {
            if (isworstCase)
                return "IsWorstCase";
            return "IsNotWorstCase";
        }
        /// <summary>
        /// 判断是否是最坏事件
        /// </summary>
        /// <param name="eventItem"></param>
        /// <param name="worstCaseDic"></param>
        /// <returns></returns>
        private static bool IsWorstCase(SagSwellEvent eventItem, Dictionary<string, List<uint>> worstCaseDic)
        {
            if (SafetyDataManager.hasIEMSWeb)
                return eventItem.IsWorstCase;
            List<uint> oneGroup;
            if (!worstCaseDic.TryGetValue(eventItem.TriggerTime, out oneGroup))
                return false;
            if (!oneGroup.Contains(eventItem.EventID))
                return false;
            return true;
        }

        private static void AddEventGroupTitleLine(ref DataTable evernGroupDT)
        {
            DataRow tempRow = evernGroupDT.NewRow();
            tempRow["Index"] = LocalResourceManager.GetInstance().GetString("0064", "Event");
            tempRow["WaveForm"] = LocalResourceManager.GetInstance().GetString("0065", "WF Recording");
            tempRow["DeviceName"] = LocalResourceManager.GetInstance().GetString("0005", "Device");
            tempRow["RecordTime"] = LocalResourceManager.GetInstance().GetString("0026", "Time");
            tempRow["EventType"] = LocalResourceManager.GetInstance().GetString("0067", "Type");
            tempRow["Direction"] = LocalResourceManager.GetInstance().GetString("0592", "Direction");
            tempRow["TotalMag"] = LocalResourceManager.GetInstance().GetString("0069", "Magnitude (%)");
            tempRow["Duration"] = LocalResourceManager.GetInstance().GetString("0071", "Duration (s)");
            evernGroupDT.Rows.Add(tempRow);
        }

        private static void AddEventGroupEmptyRow(ref DataTable evernGroupDT)
        {
            DataRow oneRow = evernGroupDT.NewRow();
            evernGroupDT.Rows.Add(oneRow);
        }

        private static void AddEventGroupNextRow(ref DataTable evernGroupDT)
        {
            DataRow oneRow = evernGroupDT.NewRow();
            oneRow[0] = NEXT;
            evernGroupDT.Rows.Add(oneRow);
        }

        private void AddEventGroupOneLine(ref DataTable evernGroupDT, int groupIndex, int eventIndex, string lastTrigerTime, List<SagSwellEvent> actual, Dictionary<string, List<uint>> worstCaseDic, Dictionary<uint, int> eventIdToIndexDic)
        {
            DataRow oneRow = evernGroupDT.NewRow();
            //添加一组事件分组
            oneRow[0] = GetEventWorstCaseString(groupIndex, eventIndex, lastTrigerTime, actual, worstCaseDic, eventIdToIndexDic);
            oneRow[1] = GetIncidentNString(groupIndex);
            evernGroupDT.Rows.Add(oneRow);
        }

        private static void AddEventGroupFirstRow(ref DataTable evernGroupDT)
        {
            DataRow firstRow = evernGroupDT.NewRow();
            firstRow["Index"] = NEXT;
            firstRow["WaveForm"] = DataManager.TimeFormatWithMinisecondForExcel;
            firstRow["TotalMag"] = DataManager.DataFormat3;
            evernGroupDT.Rows.Add(firstRow);
        }

        private void ConcstructWaveTable(DataTable waveFormDT, TotalCombinedWave totalCombinedWave, int index, List<IDToName> idNameOOADic)
        {
            try
            {
                //添加LogTime列
                AddLogTimeColumn(waveFormDT, totalCombinedWave, index);
                //添加各个通道
                AddIaEctColumns(waveFormDT, totalCombinedWave, idNameOOADic);
                //结束列
                waveFormDT.Columns.Add(PublicFunction.CreateDataTableColumn("finish", "String"));
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }
            try
            {
                if (waveFormDT.Rows.Count > 0)
                    waveFormDT.Rows[0][waveFormDT.Columns.Count - 1] = index + 1;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 判断通道是否为电压数据
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public static bool IsChannelVolt(string channelName)
        {
            if (channelName.ToUpper().Contains("U"))
                return true;
            if (channelName.ToUpper().Contains("V"))
                return true;
            return false;
        }

        private static void SortedDic(Dictionary<string, List<double>> dic, out Dictionary<string, List<double>> sortedDic)
        {
            sortedDic = new Dictionary<string, List<double>>();
            var uDic = new Dictionary<string, List<double>>();
            var otherDic = new Dictionary<string, List<double>>();
            //添加U
            foreach (var item in dic)
            {
                if (IsChannelVolt(item.Key))
                {
                    uDic.Add(item.Key, item.Value);
                }
                else
                {
                    otherDic.Add(item.Key, item.Value);
                }
            }
            foreach (var item in uDic)
            {
                sortedDic.Add(item.Key, item.Value);
            }
            foreach (var item in otherDic)
            {
                sortedDic.Add(item.Key, item.Value);
            }
        }

        private static void GetNameToValueDic(List<IDToName> idNameOOADic, Dictionary<string, List<double>> dic, out Dictionary<string, List<double>> nameToValueDic)
        {
            nameToValueDic = new Dictionary<string, List<double>>();
            //获取key是Ua，Ub等的字典
            foreach (var item in dic)
            {
                string name=item.Key;
                //List<string> nameList=new List<string>();
                //// if (!idNameOOADic.TryGetValue(item.Key, out name))
                ////    name = item.Key;
                //FindValueByKey(idNameOOADic, item.Key, out nameList);
                //if(nameList.Count==0)
                //    name = item.Key;
                //else
                //{
                //    foreach (var tempName in nameList)
                //    {
                //        name = tempName;
                //        if (!nameToValueDic.ContainsKey(name))
                //            break;
                //    }
                //}

                ////if (nameToValueDic.ContainsKey(name))
                ////    continue;
                nameToValueDic.Add(name, item.Value);
            }
        }
        /// <summary>
        /// 根据key查找值,可能存在重复的key，不同的值
        /// </summary>
        /// <param name="idNameOOADic"></param>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static void FindValueByKey(List<IDToName> idNameOOADic,string key,out List<string> name)
        {
            name = new List<string>();
            foreach (var item in idNameOOADic)
            {
                if (key == item.id)
                {
                    name.Add(item.name);
                }
            }
        }

        private static void AddIaEctColumns(DataTable waveFormDT, TotalCombinedWave totalCombinedWave, List<IDToName> idNameOOADic)
        {
            if (totalCombinedWave._CfgDataList.Count == 0)
                return;
            DateTime triggerTimeTemp = totalCombinedWave._CfgDataList[0]._TimeStamps._TriggerTime;
            double secondAndMiniSecond;
            GetSecondAndMiniSecond(totalCombinedWave._CfgDataList[0]._TimeStamps._FistTime, triggerTimeTemp, out secondAndMiniSecond);
            const int startColumn = 0;
            int count = startColumn;
            //获取key是Ua，Ub等的字典
            Dictionary<string, List<double>> nameToValueDic;
            GetNameToValueDic(idNameOOADic, totalCombinedWave._ValueDic, out nameToValueDic);

            //把U放到I的的前面
            Dictionary<string, List<double>> sortedDic;
            SortedDic(nameToValueDic, out sortedDic);

            //添加各个通道的列，例如Ua，Ub，Uc
            foreach (var item in sortedDic)
            {
                //添加一个通道的列
                waveFormDT.Columns.Add(PublicFunction.CreateDataTableColumn(item.Key + waveFormDT.Columns.Count, "String"));
                int columnNum = waveFormDT.Columns.Count - 1;
                if (count == startColumn)
                    waveFormDT.Rows[0][columnNum] = secondAndMiniSecond;
                //添加这列的数据的名字，例如Ua
                waveFormDT.Rows[1][columnNum] = GetSecondItemByDot(item.Key);//item.Key;
                int tempRow = 2;
                //添加这列的各个数据
                foreach (var oneValue in item.Value)
                {
                    waveFormDT.Rows[tempRow][columnNum] = CommonUsedFunctionsForWorstCase.GetStingByDouble3Point(oneValue);
                    ++tempRow;
                }
                ++count;
            }
        }
        /// <summary>
        /// 获取 A1,V1中的V1
        /// </summary>
        /// <param name="stringWithDot"></param>
        /// <returns></returns>
        private static string GetSecondItemByDot(string stringWithDot)
        {
           List<string> result= DataFormatManager.ParseStringList(stringWithDot, ",");
            if (result == null || result.Count < 2)
                return stringWithDot;
            return result[1];
        }

        private static void AddLogTimeColumn(DataTable waveFormDT, TotalCombinedWave totalCombinedWave, int index)
        {
            //添加LogTime列
            waveFormDT.Columns.Add(new DataColumn());
            while (waveFormDT.Rows.Count <= totalCombinedWave._TimeList.Count)
            {
                DataRow dr = waveFormDT.NewRow();
                waveFormDT.Rows.Add(dr);
                dr = waveFormDT.NewRow();
                waveFormDT.Rows.Add(dr);
            }
            int currentColumnCount = waveFormDT.Columns.Count - 1;
            waveFormDT.Rows[0][currentColumnCount] = (index + 1).ToString();
            waveFormDT.Rows[1][currentColumnCount] = "LogTime";

            int row = 2;
            foreach (var timestamp in totalCombinedWave._TimeList)
            {
              //  double secondAndMinSecond;
              //  GetSecondAndMiniSecond(totalCombinedWave._CfgDataList[0]._TimeStamps._FistTime, timestamp, out secondAndMinSecond);
                waveFormDT.Rows[row][currentColumnCount] = timestamp/1000.0; //以秒为单位   //secondAndMinSecond;
                ++row;
            }
        }

        private static void GetSecondAndMiniSecond(DateTime startTime, DateTime time, out double secondAndMinSecond)
        {
            double second = startTime.Second;
            double minisecond = startTime.Millisecond; //这样只能获取到毫秒，后面的微秒部分数据获取不到
            double temp = second*1000.0 + minisecond;
            secondAndMinSecond = temp/1000.0;
            TimeSpan timeSpan = time - startTime;
            secondAndMinSecond = timeSpan.TotalSeconds + secondAndMinSecond;
        }

        private static void GetIdNameDic(TotalCombinedWave totalCombinedWave, out List<IDToName> idNameOOADic)
        {
            idNameOOADic = new List<IDToName>();
            DbgTrace.dout("Total Combined Wave:");
            foreach (var item in totalCombinedWave._CfgDataList)
            {
                DbgTrace.dout("One cgf data:");
                foreach (var ooa in item._AnList)
                {
                    DbgTrace.dout("An=" + ooa._An + " , Ch_id=" + ooa._Ch_id);
                    //if (!idNameOOADic.ContainsKey(ooa._An))
                    //    idNameOOADic.Add(ooa._An, ooa._Ch_id);
                    idNameOOADic.Add(new IDToName(ooa._An, ooa._Ch_id));
                }
                foreach (var ood in item._DnList)
                {
                    DbgTrace.dout("Dn=" + ood._Dn + " , Ch_id=" + ood._Ch_id);
                    //if (!idNameOOADic.ContainsKey(ood._Dn))
                    //    idNameOOADic.Add(ood._Dn, ood._Ch_id);
                    idNameOOADic.Add(new IDToName(ood._Dn, ood._Ch_id));
                }
            }
        }


        public static string GetDateTimeStringByCultrue(DateTime dateTime, int msec)
        {
            dateTime = dateTime.AddMilliseconds(msec);
            return dateTime.ToString(DataManager.TimeFormatWithMinisecondForC) + "\r";
        }

        private static void GetEventTypeName(int code1, double totalMag, int eventType, out string eventTypeName)
        {
            eventTypeName = LocalResourceManager.GetInstance().GetString("0084", "Undefined Type");
            EventTypeEnum eventTypeNameType;
            EventTypeManager.GetEventTypeLikeSwellEnum(code1, totalMag, eventType, out eventTypeNameType);
            switch (eventTypeNameType)
            {
                case EventTypeEnum.Interruption:
                    eventTypeName = LocalResourceManager.GetInstance().GetString("0083", "Interruption");
                    break;
                case EventTypeEnum.Sag:
                    eventTypeName = LocalResourceManager.GetInstance().GetString("0043", "Sag");
                    break;
                case EventTypeEnum.Swell:
                    eventTypeName = LocalResourceManager.GetInstance().GetString("0049", "Swell");
                    break;
                case EventTypeEnum.Transient:
                    eventTypeName = LocalResourceManager.GetInstance().GetString("0050", "Transient");
                    break;
                case EventTypeEnum.UndefinedType:
                    eventTypeName = LocalResourceManager.GetInstance().GetString("0084", "Undefined Type");
                    break;

            }
        }

        private List<uint> m_durationDataIdList = new List<uint>();

        private void AddDurationDataId()
        {
            m_durationDataIdList.Add(SysConstDefinition.DISTURBANCEDURATION);

            m_durationDataIdList.Add(SysConstDefinition.TRANSDURATIONA);

            m_durationDataIdList.Add(SysConstDefinition.TRANSDURATIONB);

            m_durationDataIdList.Add(SysConstDefinition.TRANSDURATIONC);
        }

        /// <summary>
        /// 构建汇总信息表格
        /// </summary>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        private DataTable ConstructDataSummaryTable(DefaultReportParameter parameter)
        {
            DataTable resultDT = new DataTable("Summary");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "Measurements");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "1");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "2");

            //增加参数起始标志行，以便模板VBA进行计算
            DataRow resultRow = resultDT.NewRow();
            resultRow[0] = DataManager.GetStartTimeSting(parameter.StartTime);
            resultRow[1] = DataManager.GetEndTimeString(parameter.EndTime);
            if (parameter.IsITIC)
                resultRow[2] = "ITIC";
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultRow[0] = LocalResourceManager.GetInstance().GetString("0021", "Devices");
            string deviceParaStr = string.Empty;
            for (int i = 0; i < parameter.DeviceDataIDList.Count; i++)
            {
                //if (i == parameter.DeviceDataIDList.Count - 1)
                //    deviceParaStr = deviceParaStr + DefaultTemplatePublicMethod.GetDeviceNameByDeviceDataID(parameter.DeviceDataIDList[i],source);
                //else
                deviceParaStr = deviceParaStr + DefaultTemplatePublicMethod.GetDeviceNameByDeviceDataID(parameter.DeviceDataIDList[i], source) + "; ";
            }
            resultRow[1] = deviceParaStr;
            resultDT.Rows.Add(resultRow);

            //resultRow = resultDT.NewRow();
            //resultRow[0] = string.Empty;//LocalResourceManager.GetInstance().GetString("0106", "Incident Interval");
            //resultRow[1] = string.Empty;//PowerQualityInterfaceManager.GetInstance().GetGroupInterval();//GetInterval(parameter.Interval);
            //resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultDT.Rows.Add(resultRow);

            return resultDT;
        }

        #endregion

        #endregion

        #region 从iemsweb获取数据
        /// <summary>
        /// 根据是否是ITIC获取curveType
        /// 曲线类型，0-不限定曲线类型，1-ITIC曲线，2-SEMI曲线
        /// </summary>
        /// <param name="isITIC"></param>
        /// <returns></returns>
        public static int GetCurveType(bool isITIC)
        {
            return isITIC ? (int)RMSAnalysisManager.ToleranceCurveType.ITICLimit : (int)RMSAnalysisManager.ToleranceCurveType.SEMILimit;
        }
        /// <summary>
        /// 获取各种容忍度标准曲线
        /// </summary>
        /// <param name="curveType">曲线类型，1-ITIC曲线，2-SEMI曲线，不可为0</param>
        /// <param name="curveID">曲线ID, 1-表示默认标准曲线，其他的为自定义标准曲线</param>
        /// <returns></returns>
        public static WebGeneralResult<SARFIChartCurve> GetSARFIChartFeatureValues(uint userID, int curveType, int curveID)
        {
            return BasicDataProvider.GetInstance().GetSARFIChartFeatureValues(SafetyDataManager.GetTokenHashtable(userID), curveType, curveID);
        }

        /// <summary>
        /// 获取各种SARFI曲线的基本信息
        /// </summary>
        /// <param name="curveType">曲线类型，0-不限定曲线类型，1-ITIC曲线，2-SEMI曲线</param>
        /// <param name="curveID">曲线ID, 1-表示默认标准曲线，其他的为自定义标准曲线，为0表示不限定曲线ID</param>
        /// <returns></returns>
        public static WebGeneralResult<SARFIChartInfo> GetSARFIChartGeneralInfo(uint userID, int curveType, int curveID)
        {
            return BasicDataProvider.GetInstance().GetSARFIChartGeneralInfo(SafetyDataManager.GetTokenHashtable(userID), curveType, curveID);
        }

        /// <summary>
        /// 容忍度分析中查询所有暂态事件，合并了QuerySagSwellEventsForMore接口，不获取事件确认信息，添加了LimitType字段
        /// </summary>
        /// <param name="PQNodeParams"></param>
        /// <param name="rmsEventTypes"></param>
        /// <param name="timeParam"></param>
        /// <param name="curveParam"></param>
        /// <param name="pointRange"></param>
        /// <param name="maxRowCount"></param>
        /// <param name="exportType"></param>
        /// <param name="groupInterval"></param>
        /// <returns></returns>
        public static WebGeneralResult<SagSwellEvent> QuerySagSwellEventsForSARFI(uint userID, string PQNodeParams, string rmsEventTypes, string timeParam, string curveParam, string pointRange, int maxRowCount, int exportType, string groupInterval)
        {
            WebGeneralResult<SagSwellEvent> result=new WebGeneralResult<SagSwellEvent>(new List<SagSwellEvent>());
            try
            {
                var resultTemp= BasicDataProvider.GetInstance().QuerySagSwellEventsForSARFI(SafetyDataManager.GetTokenHashtable(userID), PQNodeParams, rmsEventTypes, timeParam, curveParam, pointRange, maxRowCount, exportType, groupInterval);
                if(resultTemp.Success)
                foreach (var item in resultTemp.ResultList)
                {
                    SagSwellEvent oneEvent=new SagSwellEvent();
                    //不需要确认信息，因此这里没有复制过来
                    oneEvent.ChannelID = item.ChannelID;
                    oneEvent.DeviceID = item.DeviceID;
                    oneEvent.EventByte = item.EventByte;
                    oneEvent.EventClass = item.EventClass;
                    oneEvent.EventCode1 = item.EventCode1;
                    oneEvent.EventCode2 = item.EventCode2;
                    oneEvent.EventID = item.EventID;
                    oneEvent.EventTime = item.EventTime;
                    oneEvent.EventType = item.EventType;
                    oneEvent.FetureInfo = new SagSwellFeature();
                    oneEvent.FetureInfo.Direction = item.FetureInfo.Direction;
                    oneEvent.FetureInfo.Duration = item.FetureInfo.Duration;
                    oneEvent.FetureInfo.EventID = item.FetureInfo.EventID;
                    oneEvent.FetureInfo.FeatureTime = item.FetureInfo.FeatureTime;
                    oneEvent.FetureInfo.FeatureValues = item.FetureInfo.FeatureValues;
                    oneEvent.FetureInfo.OutOfLimit = item.FetureInfo.OutOfLimit;
                    oneEvent.FetureInfo.QueryForMoreDatalog = item.FetureInfo.QueryForMoreDatalog;
                    oneEvent.FetureInfo.QueryForMoreFeature = item.FetureInfo.QueryForMoreFeature;
                    oneEvent.FetureInfo.TotalMag = item.FetureInfo.TotalMag;
                    oneEvent.HaveWave = item.HaveWave;
                    oneEvent.LocalZone = item.LocalZone;
                    oneEvent.NominalVoltage = item.NominalVoltage;
                    oneEvent.PQNodeID = item.PQNodeID;
                    oneEvent.PQNodeName = item.PQNodeName;
                    oneEvent.PQNodeType = item.PQNodeType;
                    oneEvent.RMSEventType = item.RMSEventType;
                    oneEvent.StationFlag = item.StationFlag;
                    oneEvent.StationID = item.StationID;
                    oneEvent.TriggerTime = item.TriggerTime;
                    oneEvent.Details = item.Details;
                    oneEvent.IsWorstCase = item.IsWorstCase;
                    result.ResultList.Add(oneEvent);
                }
            }
            catch (Exception ex)
            {
                LogService.LogManager.getInstance().Error(ex.Message + ":" + ex.StackTrace);
                result.Success = false;
                result.ErrorMessage = ex.Message + ex.StackTrace;
            }
            return result;
        }
        /// <summary>
        /// 查询一个设备的波形
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="deviceID"></param>
        /// <param name="waveTime"></param>
        /// <param name="mSec"></param>
        /// <param name="backwardOffset"></param>
        /// <param name="forwardOffset"></param>
        /// <returns></returns>
        public static WebGeneralResult<BasicDataInterface.Models.Response.OriginalWaveInfo> QueryOriginalWaveDataByDuraion(uint userID, uint deviceID, string waveTime, int mSec, int backwardOffset, int forwardOffset)
        {
            try
            {
                return BasicDataProvider.GetInstance().QueryOriginalWaveDataByDuraion(SafetyDataManager.GetTokenHashtable(userID), deviceID, waveTime, mSec, backwardOffset, forwardOffset);
                ;
            }
            catch (Exception ex)
            {
                LogService.LogManager.getInstance().Error(ex.Message + ":" + ex.StackTrace);
            }
            return new WebGeneralResult<BasicDataInterface.Models.Response.OriginalWaveInfo>();
        }

    /// <summary>
        /// 加载自定义节点树，一次性加载所有层级的节点
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static WebGeneralResult<PQMonitorNodeView> LoadAllPQMonitorNodes(uint userId)
        {
            try
            {
                return BasicDataProvider.GetInstance().LoadAllPQMonitorNodes(userId,SafetyDataManager.GetTokenHashtable(userId));
            }
            catch (Exception ex)
            {
                LogService.LogManager.getInstance().Error(ex.Message + ":" + ex.StackTrace);
            }
            return new WebGeneralResult<PQMonitorNodeView>();
        }
        #endregion

        #region 获取一个事件的detail信息
        /// <summary>
        /// 获取一个事件的detail信息，以二维数组方式返回
        /// </summary>
        /// <param name="sagSwellEvent"></param>
        /// <returns></returns>
        public static List<List<string>> GetEventDetals(SagSwellEvent sagSwellEvent)
        {
            var fetureInfo = GetDetailForEvent(sagSwellEvent);
            var oneRow = new List<string>();//存储一行的数据
            var result = new List<List<string>>();//最终的结果数据
            if (sagSwellEvent.EventType == 17)//瞬态事件
            {
                if (sagSwellEvent.EventByte <= 4)
                {
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0355", "V Nominal"));//额定电压
                    oneRow.Add(fetureInfo.NominalVoltage);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0356", "Worst Magnitude"));//总幅值
                    oneRow.Add(fetureInfo.data9);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0357", "Duration"));//持续时间
                    oneRow.Add(fetureInfo.data8);
                    result.Add(oneRow);

                    oneRow = new List<string>();//换行
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0358", "Va Max"));
                    oneRow.Add(fetureInfo.data3);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0359", "Vb Max"));
                    oneRow.Add(fetureInfo.data5);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0360", "Vc Max"));
                    oneRow.Add(fetureInfo.data7);
                    result.Add(oneRow);

                    oneRow = new List<string>();//换行
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0361", "Va Duration"));
                    oneRow.Add(fetureInfo.data2);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0362", "Vb Duration"));
                    oneRow.Add(fetureInfo.data4);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0363", "Vc Duration"));
                    oneRow.Add(fetureInfo.data6);
                    result.Add(oneRow);
                }
                else
                {
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0355", "V Nominal"));//额定电压
                    oneRow.Add(fetureInfo.NominalVoltage);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0356", "Worst Magnitude"));//总幅值
                    oneRow.Add(fetureInfo.data9);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0357", "Duration"));//持续时间
                    oneRow.Add(fetureInfo.data8);
                    result.Add(oneRow);

                    oneRow = new List<string>();//换行
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0364", "Vab Max"));
                    oneRow.Add(fetureInfo.data3);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0365", "Vbc Max"));
                    oneRow.Add(fetureInfo.data5);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0366", "Vca Max"));
                    oneRow.Add(fetureInfo.data7);
                    result.Add(oneRow);

                    oneRow = new List<string>();//换行
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0367", "Vab Duration"));
                    oneRow.Add(fetureInfo.data2);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0368", "Vbc Duration"));
                    oneRow.Add(fetureInfo.data4);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0369", "Vca Duration"));
                    oneRow.Add(fetureInfo.data6);
                    result.Add(oneRow);
                }
            }
            if (sagSwellEvent.EventType == 18) //暂态事件
            {
                if (sagSwellEvent.EventByte <= 4)
                {
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0355", "V Nominal"));//额定电压
                    oneRow.Add(fetureInfo.NominalVoltage);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0356", "Worst Magnitude"));//总幅值
                    oneRow.Add(fetureInfo.data1);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0357", "Duration"));//持续时间
                    oneRow.Add(fetureInfo.data2);
                    result.Add(oneRow);

                    oneRow = new List<string>();//换行
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0370", "Va Min"));
                    oneRow.Add(fetureInfo.data4);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0371", "Vb Min"));
                    oneRow.Add(fetureInfo.data8);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0372", "Vc Min"));
                    oneRow.Add(fetureInfo.data12);
                    result.Add(oneRow);

                    oneRow = new List<string>();//换行
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0373", "Va Max"));
                    oneRow.Add(fetureInfo.data5);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0374", "Vb Max"));
                    oneRow.Add(fetureInfo.data9);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0375", "Vc Max"));
                    oneRow.Add(fetureInfo.data13);
                    result.Add(oneRow);
                }
                else
                {
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0355", "V Nominal"));//额定电压
                    oneRow.Add(fetureInfo.NominalVoltage);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0356", "Worst Magnitude"));//总幅值
                    oneRow.Add(fetureInfo.data1);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0357", "Duration"));//持续时间
                    oneRow.Add(fetureInfo.data2);
                    result.Add(oneRow);

                    oneRow = new List<string>();//换行
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0376", "Vab Min"));
                    oneRow.Add(fetureInfo.data4);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0377", "Vbc Min"));
                    oneRow.Add(fetureInfo.data8);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0378", "Vca Min"));
                    oneRow.Add(fetureInfo.data12);
                    result.Add(oneRow);

                    oneRow = new List<string>();//换行
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0379", "Vab Max"));
                    oneRow.Add(fetureInfo.data5);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0380", "Vbc Max"));
                    oneRow.Add(fetureInfo.data9);
                    oneRow.Add(LocalResourceManager.GetInstance().GetString("0381", "Vca Max"));
                    oneRow.Add(fetureInfo.data13);
                    result.Add(oneRow);
                }
            }
            return result;
        }
        /// <summary>
        /// 返回的数据里面总是保留2位有效小数，但是客户端需要保留3位小数，需要补0
        /// </summary>
        /// <param name="nominalVoltage"></param>
        /// <returns></returns>
        private static string GetNominalVoltage(string nominalVoltage)
        {
            int index = nominalVoltage.IndexOf('.');
            return (index < 0) ? "--" : nominalVoltage.Substring(0, index + 3) + '0' + nominalVoltage.Substring(index + 3, nominalVoltage.Count() - index - 3);
        }
        /// <summary>
        /// 获取用于填写事件详细信息表格的数据。
        /// </summary>
        /// 入参是一个事件的数据
        /// <returns></returns>
        private static FetureInfoResult GetDetailForEvent(SagSwellEvent sagSwellEvent)
        {
            var fetureInfo = new FetureInfoResult { NominalVoltage = sagSwellEvent.NominalVoltage == null ? "--" : GetNominalVoltage(sagSwellEvent.NominalVoltage) };

            if (sagSwellEvent.FetureInfo.FeatureValues == null || sagSwellEvent.FetureInfo.FeatureValues.Count == 0)
            {
                fetureInfo.Duration = "--";
                fetureInfo.TotalMag = "--";
                fetureInfo.data1 = "--";
                fetureInfo.data2 = "--";
                fetureInfo.data3 = "--";
                fetureInfo.data4 = "--";
                fetureInfo.data5 = "--";
                fetureInfo.data6 = "--";
                fetureInfo.data7 = "--";
                fetureInfo.data8 = "--";
                fetureInfo.data9 = "--";
                fetureInfo.data10 = "--";
                fetureInfo.data11 = "--";
                fetureInfo.data12 = "--";
                fetureInfo.data13 = "--";
                fetureInfo.data14 = "--";
                fetureInfo.data15 = "--";
            }
            else if (sagSwellEvent.FetureInfo.FeatureValues.Count == 9 || sagSwellEvent.FetureInfo.FeatureValues.Count == 7) //瞬态事件特征值个数，老版本为7，新版本添加了总幅值和总持续时间，所以变成了9，需要兼容旧版本
            {
                fetureInfo.data1 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[0], 3000021, false, false);
                fetureInfo.data2 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[1], 3000022, true, false);
                fetureInfo.data3 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[2], 3000023, false, false);
                fetureInfo.data4 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[3], 3000024, true, false);
                fetureInfo.data5 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[4], 3000025, false, false);
                fetureInfo.data6 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[5], 3000026, true, false);
                fetureInfo.data7 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[6], 3000027, false, false);
                //先计算data8和data9，此时的Duration和TotalMag是还没有乘以乘系数的
                fetureInfo.data8 = checkIfNaN(sagSwellEvent.FetureInfo.Duration, 3000022, true, false); //带单位的总持续时间
                fetureInfo.data9 = checkIfNaN(sagSwellEvent.FetureInfo.TotalMag, 3000023, false, false); //带单位的总幅值
                //此时再对Duration和TotalMag乘以乘系数就没有问题了
                fetureInfo.Duration = checkIfNaN(sagSwellEvent.FetureInfo.Duration, 3000022, true, true);
                fetureInfo.TotalMag = checkIfNaN(sagSwellEvent.FetureInfo.TotalMag, 3000023, false, true);
            }
            else if (sagSwellEvent.FetureInfo.FeatureValues.Count == 15) //暂态事件特征值
            {
                fetureInfo.Duration = checkIfNaN(sagSwellEvent.FetureInfo.Duration, 3000001, true, true);
                fetureInfo.TotalMag = checkIfNaN(sagSwellEvent.FetureInfo.TotalMag, 3000028, false, true);
                fetureInfo.data1 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[0], 3000028, false, false);
                fetureInfo.data2 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[1], 3000001, true, false);
                fetureInfo.data3 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[2], 3000002, false, false);
                fetureInfo.data4 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[3], 3000003, false, false);
                fetureInfo.data5 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[4], 3000004, false, false);
                fetureInfo.data6 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[5], 3000005, false, false);
                fetureInfo.data7 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[6], 3000006, false, false);
                fetureInfo.data8 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[7], 3000007, false, false);
                fetureInfo.data9 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[8], 3000008, false, false);
                fetureInfo.data10 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[9], 3000009, false, false);
                fetureInfo.data11 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[10], 3000010, false, false);
                fetureInfo.data12 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[11], 3000011, false, false);
                fetureInfo.data13 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[12], 3000012, false, false);
                fetureInfo.data14 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[13], 3000013, false, false);
                fetureInfo.data15 = checkIfNaN(sagSwellEvent.FetureInfo.FeatureValues[14], 3000014, false, false);
            }

            return fetureInfo;
        }
        /// <summary>
        /// 对空数据，持续时间，幅值进行处理
        /// </summary>
        /// <param name="featureValue">double值，传入fetureInfo.FeatureValues[i]</param>
        /// <param name="dataID">dataID</param>
        /// <param name="isDuration">是否是持续时间</param>
        /// <param name="withOutUnit">不带单位，true表示不带单位</param>
        /// <returns></returns>
        private static string checkIfNaN(double featureValue, uint dataID, bool isDuration, bool withOutUnit)
        {
            if (double.IsNaN(featureValue))
                return "--";

            DataUnitDef unitDef = UnitConfigManager.DataManager.FindDataUnit(dataID);
            string unitName = unitDef.UnitName;//单位名称
            double coefficient = unitDef.Coefficient;//乘系数

            featureValue = featureValue * coefficient;//乘以乘系数

            if (isDuration && featureValue < 0.001)
            {
                return GetFormatFeatureValueString(withOutUnit, featureValue, 6, unitName);
            }
            return GetFormatFeatureValueString(withOutUnit, featureValue, 3, unitName);
        }

        /// <summary>
        /// 保留小数位，加上逗号分隔整数部分，加上单位后返回字符串
        /// </summary>
        /// <param name="withOutUnit">true表示不带单位</param>
        /// <param name="featureValue">值</param>
        /// <param name="numberDecimalDigits">保留的小数位数</param>
        /// <param name="unitName">单位名称字符串，用于添加到值后面，作为整体字符串返回</param>
        /// <returns>字符串，值+单位</returns>
        private static string GetFormatFeatureValueString(bool withOutUnit, double featureValue, int numberDecimalDigits, string unitName)
        {
            NumberFormatInfo definedProvider = NumberFormatInfo.CurrentInfo.Clone() as NumberFormatInfo;
            definedProvider.NumberDecimalDigits = numberDecimalDigits;//设置保留小数位
            string featureValueStr = featureValue.ToString("F", definedProvider);//获取保留N位小数的字符串
            featureValueStr = DataFormatManager.GetSplitDoubleStr(featureValueStr);//小数点左边，每隔三位加上逗号
            return withOutUnit ? featureValueStr : (featureValueStr + unitName);//如果不带单位，直接返回；否则返回带单位的字符串
        }

        #endregion


    }

    public struct EventStruct
    {
        public DateTime eventTime;
        public int MillSec;
        public uint deviceID;
        public EventStruct(DateTime eventTime, int MillSec, uint deviceID)
        {
            this.eventTime = eventTime;
            this.MillSec = MillSec;
            this.deviceID = deviceID;
        }
    }

    public struct OrigLogStruct
    {
        private DateTime recordTime;
        private int MillSec;
        private uint deviceID;
        public int comtradeNum;
        public OrigLogStruct(DateTime recordTime, int MillSec, uint deviceID)
        {
            this.recordTime = recordTime;
            this.MillSec = MillSec;
            this.deviceID = deviceID;
            comtradeNum = 1;
        }
    }
}
