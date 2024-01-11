using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using BasicDataInterface;
using BasicDataInterface.Models.Response;
using HttpInterfaceCommLib.Lib;
using Newtonsoft.Json;
using OfficeReportInterface.DefaultReportInterface.CommonluUsed;
using OfficeReportInterface.DefaultReportInterface.EnergyCost;
//using CET.PecsNodeManage;

namespace OfficeReportInterface.DefaultReportInterface.IntelligentSafety
{
    public class SafetyDataManager
    {
        #region 查询预制报表数据相关

        /// <summary>
        /// 获取数据的三个dataTable，用于填充到Excel的data这个sheet的三个标签（警告表，汇总表，数据表）处
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static List<DataTable> GetStatisticDatas(DefaultReportParameter parameter)
        {
            WebGeneralResult<StatisticForAlarm> statisticData = null;
            ////---------------------A）获取各个需要的数据，用于后面填充到DataTable------------------------------------------
            ////情况1：如果从OfficeReport.ini文件中读取到，是iemsweb，就从bin旁边的CGI目录下获取json数据结果
            //var iniFile = new INIFile(Path.Combine(DbgTrace.GetAssemblyPath(), "OfficeReport.ini"));
            //var isIemsweb = iniFile.ReadString("ProgramRunningEnvironment", "isIEMSWEB");//是否是iemsweb的运行环境
            //if (string.Equals(isIemsweb.Trim().ToLower(), "true"))
            //{
            //    DbgTrace.dout("Read statistic data of events from json file.");
            //    string filePath = Path.Combine(DbgTrace.GetCurrentParentPath(), "CGI", "TempFile", "StatisticsForAlarm.json");
            //    string statisticDataJson = ReadJsonString(filePath);
            //    statisticData = DataFormatManager.Create(FormatType.JsonType).DeserializeObject<WebGeneralResult<StatisticForAlarm>>(statisticDataJson);
            //    File.Delete(filePath);
            //}
            //else //情况2：如果不是iemsweb，而是桌面版的OfficeReport.exe，就登陆iemsweb，调用iemsweb的接口获取数据
            {
                DbgTrace.dout("Read statistic data of events from emswebservice.");
                int interval = 1; //1.统计间隔，1-5日周月季年，6-小时. 
                //调用iemsweb接口获取数据.入参有待继续完善
                statisticData = GetStatisticsForAlarm(parameter.DeviceDataIDList[0].NodeType + "," + parameter.DeviceDataIDList[0].DeviceID, parameter.StartTime + "," + parameter.EndTime, "3,3,3", "-1;-1", 10, interval, parameter.userID);
            }

            //情况3：如果获取数据失败，就写日志
            if (statisticData == null)
            {
                DbgTrace.dout("GetStatisticsForAlarm failed.");
            }
            //------------------------------填充datatable---------------------------------------------------------------------------
            List<DataTable> resultDTList = new List<DataTable>();
            DataTable warningDT = DefaultTemplatePublicMethod.ConstructWarningTable(parameter.IsIncludeWarning);
            DataTable summaryDT = ConstructDataSummaryTable(statisticData, parameter);
            DataTable resultDT = ConstructRealDataTable(statisticData, parameter);
            //根据传入的参数填充警告表，汇总表，数据表
            FillWarmSummaryDataToTable(parameter, warningDT, resultDT);
            resultDTList.Add(warningDT);
            resultDTList.Add(summaryDT);
            resultDTList.Add(resultDT);
            return resultDTList;
        }

        /// <summary>
        /// 填充警告表
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="warningDt"></param>
        /// <param name="resultDt"></param>
        private static void FillWarmSummaryDataToTable(DefaultReportParameter parameter, DataTable warningDt, DataTable resultDt)
        {
            return;
        }

        /// <summary>
        /// 用于存储有2列数据的结构
        /// </summary>
        private struct ColumnPair
        {
            /// <summary>
            /// 某行第一列的值（横坐标的值）
            /// </summary>
            public string columnFirst;

            /// <summary>
            /// 某行第二列的值（纵坐标的值）
            /// </summary>
            public string columnSecond;

            public ColumnPair(string columnFirst, string columnSecond)
            {
                this.columnFirst = columnFirst;
                this.columnSecond = columnSecond;
            }
        }

        /// <summary>
        /// 添加List的前5行（由于存在4次重复，因此提取到这里）
        /// </summary>
        /// <param name="tip">第一行的数据</param>
        /// <param name="titleName">第三行的数据，用来显示到图标的标题位置</param>
        /// <param name="firstColumnDataRange">横坐标数据的range（指的是data这个sheet中，即将填写横坐标的一系列数据的range，例如“I6：I36”字符串）</param>
        /// <param name="secondColumnDataRange">纵坐标数据的range，例如“J6:J36”</param>
        /// <param name="count">坐标点的个数</param>
        /// <returns></returns>
        private static List<ColumnPair> AddFirst5ItemsToList(string tip, string titleName, string firstColumnDataRange, string secondColumnDataRange, string count)
        {
            List<ColumnPair> list = new List<ColumnPair>();
            //line=1  注释，便于理解，不参与显示到界面
            list.Add(new ColumnPair(tip, string.Empty));
            //2
            list.Add(new ColumnPair(count, count)); //2
            //3
            list.Add(new ColumnPair(titleName, titleName)); //3 例如：总部201栋（总计：874）
            //4
            list.Add(new ColumnPair(firstColumnDataRange, secondColumnDataRange)); //4 例如 I6：I36	J6:J36
            //5
            list.Add(new ColumnPair(string.Empty, string.Empty)); //5
            return list;
        }

        /// <summary>
        /// 计算最后一行的行号（最后一行的行号=起始行的行号+行数-1）
        /// </summary>
        /// <param name="count">行数</param>
        /// <param name="startLine">起始行号</param>
        /// <returns></returns>
        private static int ComputeEndLine(int count, int startLine)
        {
            return startLine + count - 1; //最后一行的行号=起始行的行号+行数-1
        }

        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="deviceDataIdList"></param>
        /// <returns></returns>
        private static DataTable ConstructRealDataTable(WebGeneralResult<StatisticForAlarm> statisticData, DefaultReportParameter parameter)
        {
            var resultDT = new DataTable();
            if (!statisticData.Success)
            {
                DbgTrace.dout("请检查iEMSWeb是否正确部署！");
                return resultDT;
            }

            try
            {
                string titleName = string.Format("{0} (总计：{1})", parameter.DeviceDataIDList[0].NodeName, statisticData.ResultList[0].CustomNodesWithEvent_7[0].AlarmListCount);
                int startLine = 6; //数据从第6行开始往后填，第一列是横坐标的值，第二列是纵坐标的值
                //--------------A)获取List<List<ColumnPair>> resultDataList结果数据--------------------------------------------------------------------------------------------------------------------------------------
                List<List<ColumnPair>> resultDataList = new List<List<ColumnPair>>();
                //--------------A1)增加1~2列的数据  按时间分组-曲线图---------------------------------------------
                int lineCount = statisticData.ResultList[0].GroupByTime_6[0].EventCountList.Count;
                int endLine = ComputeEndLine(lineCount, startLine); //最后一行的行号=起始行的行号+行数-1

                List<ColumnPair> groupByTimeList = AddFirst5ItemsToList("注释：按时间分组-曲线图(不显示到界面上，未做国际化)", titleName, string.Format("I{0}:I{1}", startLine, endLine), string.Format("J{0}:J{1}", startLine, endLine), TransIntToString((uint)lineCount));
                //6~endLine
                for (int i = 0; i < lineCount; ++i)
                {
                    groupByTimeList.Add(new ColumnPair(GetDateStrByDateTime(parameter.StartTime.AddDays(i)), TransIntToString((uint)statisticData.ResultList[0].GroupByTime_6[0].EventCountList[i])));
                }
                resultDataList.Add(groupByTimeList);
                //-----------------A2)增加3~4列的数据  按项目节点分组-曲线图----------------------------------------
                lineCount = statisticData.ResultList[0].GroupByNode_1[0].xAxis_categories.Count;
                endLine = ComputeEndLine(lineCount, startLine);
                List<ColumnPair> groupByNodeList = AddFirst5ItemsToList("注释：按项目节点分组-曲线图(不显示到界面上，未做国际化)", titleName, string.Format("K{0}:K{1}", startLine, endLine), string.Format("L{0}:L{1}", startLine, endLine), TransIntToString((uint)lineCount));
                //6~endLine
                for (int i = 0; i < lineCount; ++i)
                {
                    groupByNodeList.Add(new ColumnPair(statisticData.ResultList[0].GroupByNode_1[0].xAxis_categories[i], TransIntToString((uint)statisticData.ResultList[0].GroupByNode_1[0].series[0].valueList[i])));
                }
                resultDataList.Add(groupByNodeList);
                //-------------------A3)增加5~6 事件类型占比-饼图数据----------------------------------------------------
                lineCount = statisticData.ResultList[0].GroupByEventType_4.Count;
                endLine = ComputeEndLine(lineCount, startLine);
                List<ColumnPair> groupByEventTypeList = AddFirst5ItemsToList("注释：事件类型占比-饼图数据(不显示到界面上，未做国际化)", titleName, string.Format("M{0}:M{1}", startLine, endLine), string.Format("N{0}:N{1}", startLine, endLine), TransIntToString((uint)lineCount));
                //6~endLine
                for (int i = 0; i < lineCount; ++i)
                {
                    string count = TransIntToString((uint)statisticData.ResultList[0].GroupByEventType_4[i].y);
                    double rate = ComputerRate(statisticData.ResultList[0].GroupByEventType_4[i].y, (double)statisticData.ResultList[0].CustomNodesWithEvent_7[0].AlarmListCount);
                    groupByEventTypeList.Add(new ColumnPair(string.Format("{0} ({1}; {2}%)", statisticData.ResultList[0].GroupByEventType_4[i].name, count, rate), string.Format("{0}%", rate)));
                }
                resultDataList.Add(groupByEventTypeList);
                //-------------------A4)增加7~8 未确认已确认-stackBar 数据----------------------------------------------------
                lineCount = statisticData.ResultList[0].GroupByAckState_2[0].xAxis_categories.Count;
                endLine = ComputeEndLine(lineCount, startLine);
                List<ColumnPair> groupByAckStateList = AddFirst5ItemsToList("注释：未确认已确认-stackBar(不显示到界面上，未做国际化)", titleName, string.Format("O{0}:O{1}", startLine, endLine), string.Format("P{0}:P{1}", startLine, endLine), TransIntToString((uint)lineCount));
                //6~endLine
                for (int i = 0; i < lineCount; ++i)
                {
                    groupByAckStateList.Add(new ColumnPair(statisticData.ResultList[0].GroupByAckState_2[0].xAxis_categories[i], TransIntToString((uint)statisticData.ResultList[0].GroupByAckState_2[0].series[0].valueList[i])));
                }
                resultDataList.Add(groupByAckStateList);

                //---------------一B)将resultDataList数据填充到DataTable 共有8列（1~2  按时间分组-曲线图； 3~4 按项目节点分组-曲线图；5~6 事件类型占比-饼图；7~8 未确认已确认-stackBar ）-----------------------------------------------

                AddMultiColumnToTataTable(resultDT, 8);
                //-----------------B1)添加N行空行----------------
                int maxLineCount = 0;
                resultDataList.ForEach(item => { if (item.Count > maxLineCount) maxLineCount = item.Count; }); //看最多需要多少行
                AddMultiEmptyLines(resultDT, 1, maxLineCount); //添加这么多个空行到dataTable
                //-----------------B2)填充dataTable的数据--------
                int column = 0;
                foreach (var list in resultDataList) //遍历数据集合，填充到dataTable
                {
                    for (int row = 0; row < list.Count; ++row) //遍历list，填充dataTable的每行
                    {
                        resultDT.Rows[row][column] = list[row].columnFirst; //横坐标数据
                        resultDT.Rows[row][column + 1] = list[row].columnSecond; //列坐标数据
                    }
                    column = column + 2; //列的下标加2，表示换下一个坐标数据（横坐标数据、列坐标数据）
                }
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }

            return resultDT;
        }

        /// <summary>
        /// 从数字转字符串，显示的是例如 123,456,789，特点：有逗号分隔每三个字符
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static string TransIntToString(uint number)
        {
            string numberFormateStr = "###,###";
            return number > 0 ? number.ToString(numberFormateStr) : "0";
        }

        /// <summary>
        /// 获取最大事件个数的自定义节点（必须是自定义节点的，不能是自定义根节点、组节点）
        /// </summary>
        /// <param name="treeNode">传入整棵树</param>
        /// <param name="deviceWithMaxEventCountList">拥有最大事件个数的自定义节点集合</param>
        private static void GetMaxEventDeviceTreeNodes(CustomNodeView treeNode, out List<CustomNodeView> deviceWithMaxEventCountList)
        {
            deviceWithMaxEventCountList = new List<CustomNodeView>();
            int maxEventNumber = 0;
            GetMaxEventNumber(treeNode, ref maxEventNumber);
            if (maxEventNumber > 0)
                FindTreeNodeByEventCount(treeNode, maxEventNumber, deviceWithMaxEventCountList);
        }

        /// <summary>
        /// 查找事件个数是maxEventNumber的自定义节点集合
        /// </summary>
        /// <param name="treeNode">传入树节点</param>
        /// <param name="maxEventNumber">根据这个值找树节点</param>
        /// <param name="deviceWithMaxEventCountList">找到的自定义节点集合</param>
        private static void FindTreeNodeByEventCount(CustomNodeView treeNode, int maxEventNumber, List<CustomNodeView> deviceWithMaxEventCountList)
        {
            if (treeNode.NodeType == SysConstDefinition.CUSTOM_NODE)
            {
                if (treeNode.AlarmListCount == maxEventNumber)
                {
                    deviceWithMaxEventCountList.Add(treeNode);
                }
            }
            else
            {
                if (treeNode.children != null && treeNode.children.Count > 0)
                    foreach (var child in treeNode.children)
                        FindTreeNodeByEventCount(child, maxEventNumber, deviceWithMaxEventCountList); //递归
            }
        }

        /// <summary>
        /// 获取发生事件个数最多的树节点的事件个数（必须是自定义节点的，不能是自定义根节点、组节点）
        /// </summary>
        /// <param name="treeNode">传入树节点（可能是自定义根节点、自定义组节点、自定义节点）</param>
        /// <param name="maxEventNumber">最大事件个数。其初始值应传入0</param>
        private static void GetMaxEventNumber(CustomNodeView treeNode, ref int maxEventNumber)
        {
            if (treeNode.NodeType == SysConstDefinition.CUSTOM_NODE)
            {
                if (treeNode.AlarmListCount > maxEventNumber)
                {
                    maxEventNumber = treeNode.AlarmListCount;
                }
            }
            else
            {
                if (treeNode.children != null && treeNode.children.Count > 0)
                    foreach (var child in treeNode.children)
                        GetMaxEventNumber(child, ref maxEventNumber); //递归
            }
        }

        /// <summary>
        /// 将多个字符串拼接成一串字符串，使用逗号分隔
        /// </summary>
        /// <param name="strList">多个字符串</param>
        /// <returns>使用逗号分隔的一个字符串</returns>
        private static string GetCombinedStr(List<string> strList)
        {
            if (strList == null || strList.Count == 0)
                return string.Empty;
            if (strList.Count == 1)
                return strList[0];
            string result = strList[0];
            for (int i = 1; i < strList.Count; ++i)
                result = result + ", " + strList[i];
            return result;
        }

        /// <summary>
        /// 给dataTable增加多个列
        /// </summary>
        /// <param name="resultDT"></param>
        /// <param name="columnCount">列的个数</param>
        private static void AddMultiColumnToTataTable(DataTable resultDT, int columnCount)
        {
            for (int i = 1; i <= columnCount; ++i)
            {
                DefaultTemplatePublicMethod.AddColumnToTable(resultDT, i.ToString());
            }
        }

        /// <summary>
        /// 获取日期字符串，例如 3月2日
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        private static string GetDateStrByDateTime(DateTime startTime)
        {
            return startTime.ToString((new CommonluUsed.DateTimeFormatManager()).m_shortDatePattern);
        }

        /// <summary>
        /// 计算事件占比
        /// </summary>
        /// <param name="eventNumber">事件个数</param>
        /// <param name="eventTotalNumber">事件总个数</param>
        /// <returns></returns>
        private static double ComputerRate(double eventNumber, double eventTotalNumber)
        {
            return eventTotalNumber == 0 ? 0 : DataFormatManager.GetFormattedDoubleByDigits(eventNumber / eventTotalNumber * 100, 2); //占比多少
        }

        /// <summary>
        ///// 构建汇总信息表格
        /// </summary>
        /// <param name="statisticData"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static DataTable ConstructDataSummaryTable(WebGeneralResult<StatisticForAlarm> statisticData, DefaultReportParameter parameter)
        {
            DataTable resultDT = new DataTable("Summary");
            try
            {
                //-------------各种事件个数数据---------------------
                // 剩余电流
                string ResidualCurrentNumber = TransIntToString((uint)statisticData.ResultList[0].GroupByEventType_4[0].y);
                //剩余电流报警
                string ResidualCurrentNumber_alarm = TransIntToString((uint)statisticData.ResultList[0].StackedColumn_GroupByEventType_EventClass_8[0].series[1].valueList[0]);
                //剩余电流预警
                string ResidualCurrentNumber_warning = TransIntToString((uint)statisticData.ResultList[0].StackedColumn_GroupByEventType_EventClass_8[0].series[2].valueList[0]);

                // 温度
                string TemperatureNumber = TransIntToString((uint)statisticData.ResultList[0].GroupByEventType_4[1].y);
                //温度报警
                string TemperatureNumber_alarm = TransIntToString((uint)statisticData.ResultList[0].StackedColumn_GroupByEventType_EventClass_8[0].series[1].valueList[1]);
                //温度预警
                string TemperatureNumber_warning = TransIntToString((uint)statisticData.ResultList[0].StackedColumn_GroupByEventType_EventClass_8[0].series[2].valueList[1]);

                // 电量越限
                string SetpointNumber = TransIntToString((uint)statisticData.ResultList[0].GroupByEventType_4[2].y);
                // 探头故障
                string ProbeFailureNumber = TransIntToString((uint)statisticData.ResultList[0].GroupByEventType_4[3].y);
                // 开关量事件
                string DIEventNumber = TransIntToString((uint)statisticData.ResultList[0].GroupByEventType_4[4].y);
                // 装置自检事件
                string DeviceDiagnosticEventNumber = TransIntToString((uint)statisticData.ResultList[0].GroupByEventType_4[5].y);
                //累计电气安全事件次数
                double eventTotalNumber = 0;
                foreach (var item in statisticData.ResultList[0].GroupByEventType_4)
                {
                    eventTotalNumber = eventTotalNumber + item.y;
                }
                string eventTotalNumberStr = TransIntToString((uint)eventTotalNumber);
                //----------哪种事件类型占比最大----------------
                double maxEventNumber_EventType = 0; //某种类型的事件的事件个数，用于记录事件个数最大值
                string name_maxEventNumber_EventType = string.Empty; //事件类型名称，这种类型的事件占比最大
                statisticData.ResultList[0].GroupByEventType_4.ForEach(item =>
                {
                    if (item.y > maxEventNumber_EventType)
                    {
                        name_maxEventNumber_EventType = item.name;
                        maxEventNumber_EventType = item.y;
                    }
                });
                double rate_maxEventNumber_EventType = ComputerRate(maxEventNumber_EventType, eventTotalNumber);
                //----------统计哪天事件最多--------------------
                int maxNumber = -1; //记录最大事件个数
                for (int i = 0; i < statisticData.ResultList[0].GroupByTime_6[0].EventCountList.Count; ++i)
                {
                    if (statisticData.ResultList[0].GroupByTime_6[0].EventCountList[i] > maxNumber)
                    {
                        maxNumber = statisticData.ResultList[0].GroupByTime_6[0].EventCountList[i];
                    }
                }
                List<int> maxEventCountSet = new List<int>(); //用于保存结果集合，找出所有满足其个数是最大事件个数的下标，也就是日期
                for (int i = 0; i < statisticData.ResultList[0].GroupByTime_6[0].EventCountList.Count; ++i)
                {
                    if (statisticData.ResultList[0].GroupByTime_6[0].EventCountList[i] == maxNumber)
                    {
                        maxEventCountSet.Add(i); //记录所有的最大值所在的下标，对应的就是日期
                    }
                }
                List<string> maxEventTimesStrList = new List<string>();
                maxEventCountSet.ForEach(item => maxEventTimesStrList.Add(GetDateStrByDateTime(parameter.StartTime.AddDays(item))));
                string maxEventTimesStr = GetCombinedStr(maxEventTimesStrList); //拼接日期字符串，这个字符串是发生事件最多的日期
                //--------统计哪个设备发生的事件最多---------------
                List<CustomNodeView> deviceWithMaxEventCountList = statisticData.ResultList[0].customNodesWithMaxEventNumber_9;
                List<string> deviceNamesWithMaxEventCountStrList = new List<string>();
                deviceWithMaxEventCountList.ForEach(item => deviceNamesWithMaxEventCountStrList.Add(item.text));
                string deviceNamesWithMaxEventCountStr = GetCombinedStr(deviceNamesWithMaxEventCountStrList); //拼接设备字符串，这是发生事件最多的设备的名称
                //--------已确认、未确认事件个数-------------------
                double ackNumber = statisticData.ResultList[0].GroupByAckState_2[0].series[0].valueList[1]; //已确认事件个数
                double unAckNumber = statisticData.ResultList[0].GroupByAckState_2[0].series[0].valueList[0]; //未确认事件个数
                //--------统计的所有事件类型的字符串---------------
                string allEventTypeStr = GetCombinedStr(statisticData.ResultList[0].StackedColumn_GroupByEventType_EventClass_8[0].xAxis_categories);
                //----------------------B）构造datatTable的两个列---------------------------------------------------------------------
                AddMultiColumnToTataTable(resultDT, 2);
                //----------------------C）填充dataTable的各个行-----------------------------------------------------------------------
                //起始结束时间(Line=1)
                AddOneLine(resultDT, DataManager.GetStartTimeSting(parameter.StartTime), DataManager.GetEndTimeString(parameter.EndTime));
                //空行(Line=2)
                EnergyCostChartDataManager.AddEmptyLine(resultDT);
                //警告信息时间格式(Line=3)
                EnergyCostChartDataManager.AddWarninMessageFormat(resultDT);
                //数据坐标轴时间格式(Line=4)
                AddOneLine(resultDT, DataManager.GetTableTimeFormatForExcel(), "Curve Time");
                //数据格式(Line=5)
                AddOneLine(resultDT, EnergyCostChartDataManager.GetNumberFormatInfo(), string.Empty);
                //空行(Line=6~12)
                AddMultiEmptyLines(resultDT, 6, 12);
                //一、项目概况(Line=13)
                AddOneLine(resultDT, LocalResourceManager.GetInstance().GetString("0612", "一、项目概况"), string.Empty);
                //空行(Line=14)
                EnergyCostChartDataManager.AddEmptyLine(resultDT);
                //客户名称,深圳中电(Line=15)
                AddOneLine(resultDT, LocalResourceManager.GetInstance().GetString("0613", "客户名称"), parameter.DeviceDataIDList[0].NodeName);
                //统计周期，2018/2/9 00:00:00  ~  2018/2/10 00:00:00(Line=16)
                AddOneLine(resultDT, LocalResourceManager.GetInstance().GetString("0614", "统计周期"), string.Format("{0} ~ {1}", parameter.StartTime.ToString(DataManager.GetWarningMessageTimeFormat()), parameter.EndTime.ToString(DataManager.GetWarningMessageTimeFormat())));
                //空行(Line=17)
                EnergyCostChartDataManager.AddEmptyLine(resultDT);
                //监测终端数量(Line=18)     注意：18~24行的123值要使用具体数字替代
                AddOneLine(resultDT, LocalResourceManager.GetInstance().GetString("0615", "监测终端数量"), TransIntToString((uint)statisticData.ResultList[0].AlarmOverview_5[0].monitorNumber));
                //(Line=19)
                AddOneLine(resultDT, LocalResourceManager.GetInstance().GetString("0616", "预警总数"), TransIntToString((uint)statisticData.ResultList[0].AlarmOverview_5[0].warningEventNumber));
                //(Line=20)
                AddOneLine(resultDT, LocalResourceManager.GetInstance().GetString("0617", "报警总数"), TransIntToString((uint)statisticData.ResultList[0].AlarmOverview_5[0].alarmEventNumber));
                //(Line=21)
                AddOneLine(resultDT, LocalResourceManager.GetInstance().GetString("0618", "故障总数"), TransIntToString((uint)statisticData.ResultList[0].AlarmOverview_5[0].faultNumber));
                //(Line=22)
                AddOneLine(resultDT, LocalResourceManager.GetInstance().GetString("0619", "漏电次数"), ResidualCurrentNumber); //1.赵红光说，漏电次数指剩余电流报警次数+剩余电流预警次数；2. GroupByEventType_4按顺序的各个数据分别是：【0"剩余电流"】【1"温度"】【2 "越限事件"】【3"探头故障"】【4 "开关量事件"】【5"设备自检事件"】
                //(Line=23)
                AddOneLine(resultDT, LocalResourceManager.GetInstance().GetString("0620", "超温次数"), TemperatureNumber); //超温次数指温度报警次数
                //(Line=24)
                AddOneLine(resultDT, LocalResourceManager.GetInstance().GetString("0621", "过载次数"), SetpointNumber); //过载次数指电量越限次数
                //空行(Line=25)
                EnergyCostChartDataManager.AddEmptyLine(resultDT);
                //二、电气安全运行统计分析(Line=26)
                AddOneLine(resultDT, LocalResourceManager.GetInstance().GetString("0622", "二、电气安全运行统计分析"), string.Empty);
                //空行(Line=27)
                EnergyCostChartDataManager.AddEmptyLine(resultDT);
                //1）事件分布时间统计(Line=28)
                AddOneLine(resultDT, LocalResourceManager.GetInstance().GetString("0623", "1）事件分布时间统计"), string.Empty);
                //空行(Line=29~41)
                AddMultiEmptyLines(resultDT, 29, 41);
                //2）事件分类统计情况(Line=42)
                AddOneLine(resultDT, LocalResourceManager.GetInstance().GetString("0624", "2）事件分类统计情况"), string.Empty);
                //空行(Line=43~54)
                AddMultiEmptyLines(resultDT, 43, 54);
                //(Line=55)0625=统计周期内，发生电气安全事件共： {0}次，其中：电量越限事件： {1}次；剩余电流报警： {2}次；剩余电流预警： {3}次；温度报警： {4}次；温度预警： {5}次；探头故障： {6}次；装置自检故障： {7}次。
                AddOneLine(resultDT, string.Format(LocalResourceManager.GetInstance().GetString("0625", "统计周期内，发生电气安全事件共： {0}次，其中：剩余电流相关： {1}次（剩余电流报警： {2}次；剩余电流预警： {3}次）；温度相关： {4}次（温度报警： {5}次；温度预警： {6}次）；电量越限事件： {7}次；探头故障： {8}次；开关量事件： {9}次；设备自检事件： {10}次。"), eventTotalNumberStr, ResidualCurrentNumber, ResidualCurrentNumber_alarm, ResidualCurrentNumber_warning, TemperatureNumber, TemperatureNumber_alarm, TemperatureNumber_warning, SetpointNumber, ProbeFailureNumber, DIEventNumber, DeviceDiagnosticEventNumber), string.Empty);
                //空行(Line=56)
                EnergyCostChartDataManager.AddEmptyLine(resultDT);
                //0626=3）事件处理情况统计(Line=57)
                AddOneLine(resultDT, LocalResourceManager.GetInstance().GetString("0626", "3）事件处理情况统计"), string.Empty);
                //空行(Line=58~70)
                AddMultiEmptyLines(resultDT, 58, 70);
                //(Line=71)0627=4）结论及建议
                AddOneLine(resultDT, LocalResourceManager.GetInstance().GetString("0627", "4）结论及建议"), string.Empty);

                //(Line=72)0628=1）{0}，在统计周期内，{1}月{2}日发生报警事件较多，请重点排查和关注当日实际用电和设备使用情况； 其中，发生较多事件的设备回路为：{3}，请予以重点关注，及时排查该回路的电气安全隐患！
                AddOneLine(resultDT, deviceWithMaxEventCountList.Count == 0 ? string.Format(LocalResourceManager.GetInstance().GetString("0632", "1）{0}，在统计周期内，未发生电气安全事件。统计的电气安全事件类型包括：{1}。"), parameter.DeviceDataIDList[0].NodeName, allEventTypeStr) : string.Format(LocalResourceManager.GetInstance().GetString("0628", "1）{0}，在统计周期内，{1}发生报警事件较多，请重点排查和关注当日实际用电和设备使用情况；发生较多事件的设备为：{2}，请予以重点关注，及时排查该回路的电气安全隐患！"), parameter.DeviceDataIDList[0].NodeName, maxEventTimesStr, deviceNamesWithMaxEventCountStr), string.Empty);
                //(Line=73)  0629=2）在统计周期内，发生{0}（事件类型）事件总数：{1}条，占比高达{2}%，请予以重点关注和排查！ 
                AddOneLine(resultDT, deviceWithMaxEventCountList.Count == 0 ? string.Format(LocalResourceManager.GetInstance().GetString("0633", "2）在统计周期内，发生{0}（事件类型）事件总数：{1}条。"), allEventTypeStr, 0) : string.Format(LocalResourceManager.GetInstance().GetString("0629", "2）在统计周期内，发生{0}（事件类型）事件总数：{1}条，占比{2}%，请予以重点关注和排查！"), name_maxEventNumber_EventType, TransIntToString((uint)maxEventNumber_EventType), rate_maxEventNumber_EventType), string.Empty);
                //(Line=74)  0630=3）统计周期内，实际发生事件总数为：{0}条，已确认事件：{1}条，未确认事件：{2}条，事件跟进处理情况评级为：{3}
                AddOneLine(resultDT, string.Format(LocalResourceManager.GetInstance().GetString("0630", "3）统计周期内，实际发生事件总数为：{0}条，已确认事件：{1}条，未确认事件：{2}条，事件跟进处理情况评级为：{3}"), eventTotalNumberStr, TransIntToString((uint)ackNumber), TransIntToString((uint)unAckNumber), GetLevelStr(ackNumber, unAckNumber)), string.Empty);
                //(Line=75)  0631=评级标准：已确认事件/（已确认+未确认）*100%。优秀：≥90%；良好：80%~90%；合格：60%~80%；差：＜60%
                AddOneLine(resultDT, LocalResourceManager.GetInstance().GetString("0631", "评级标准：已确认事件/（已确认+未确认）*100%。优秀：≥90%；良好：80%~90%；合格：60%~80%；差：＜60%"), string.Empty);
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }
            return resultDT;
        }

        /// <summary>
        /// 获得评级。评级标准：已确认事件/（已确认+未确认）*100%。优秀：≥90%；良好：80%~90%；合格：60%~80%；差：＜60%
        /// </summary>
        /// <param name="ackNumber">已确认事件个数</param>
        /// <param name="unAckNumber">未确认事件个数</param>
        /// <returns>
        ///0634=优秀
        ///0635=良好
        ///0636=合格
        ///0637=差</returns>
        private static string GetLevelStr(double ackNumber, double unAckNumber)
        {
            double rate = 100;
            if (((ackNumber + unAckNumber) == 0) || ((rate = (ackNumber / (ackNumber + unAckNumber) * 100)) >= 90))
                return LocalResourceManager.GetInstance().GetString("0634", "优秀");
            else if (rate < 90 && rate >= 80)
                return LocalResourceManager.GetInstance().GetString("0635", "良好");
            else if (rate < 80 && rate >= 60)
                return LocalResourceManager.GetInstance().GetString("0636", "合格");
            else
                return LocalResourceManager.GetInstance().GetString("0637", "差");
        }

        /// <summary>
        /// 给datatable添加一个新行，一共有2列
        /// </summary>
        /// <param name="resultDt"></param>
        /// <param name="columnFirst">第一列的值</param>
        /// <param name="columnSecond">第二列的值</param>
        private static void AddOneLine(DataTable resultDt, string columnFirst, string columnSecond)
        {
            DataRow resultRow = resultDt.NewRow();
            resultRow[0] = columnFirst;
            resultRow[1] = columnSecond;
            resultDt.Rows.Add(resultRow);
        }

        /// <summary>
        /// 给datatable添加多个空的新行，一共有2列
        /// </summary>
        /// <param name="resultDt"></param>
        /// <param name="startLine">大于等于起始行号</param>
        /// <param name="endLine">小于等于结束行号</param>
        private static void AddMultiEmptyLines(DataTable resultDt, int startLine, int endLine)
        {
            for (int i = startLine; i <= endLine; ++i)
            {
                AddOneLine(resultDt, string.Empty, string.Empty);
            }
        }

        #endregion

        #region 调用iemsweb接口相关

        /// <summary>
        /// 标识是否正确部署了iEMSWeb
        /// </summary>
        public static bool hasIEMSWeb = false;

        /// <summary>
        /// 初始化iemsweb，并且登陆iemsweb
        /// </summary>
        /// <param name="userId">传入登录的用户id的值，用于登录</param>
        /// <returns>true是登陆成功；false是登陆失败</returns>
        public static bool InitializeIemsWebServiceAndLogin(uint userId, out BasicDataInterface.Models.Response.UserValidateMsgWithCode loginResult)
        {
            loginResult = null;
            //初始化-----------------------
            LogService.LogManager.getInstance().Initialize(@"log4net.config");
            //先保证WebService初始化完成
            LogService.LogManager.getInstance().Info("访问iemsweb接口：初始化中……");
            UserValidateMsg InitializeServiceResult = InitializeService(userId);
            if (!InitializeServiceResult.Success)
            {
                LogService.LogManager.getInstance().Info("访问iemsweb接口：初始化失败，异常信息：" + InitializeServiceResult.MessageStr);
                return false;
            }
            LogService.LogManager.getInstance().Info("访问iemsweb接口：初始化完成……");
            //登陆-------------------------.
            //固定使用ROOT用户登录，拥有所有权限，由于不知道ROOT用户的密码，所以只能通过userID登录
            string pwd = WebSecurityManager.DataManager.AESEncrypt("");
            loginResult = LoginStandaloneFalse(userId.ToString());
            if (WebSecurityManager.DataManager.AESDecrypt(loginResult.Success).ToUpper() == "FALSE")
            {
                LogService.LogManager.getInstance().Info("访问iemsweb接口：登录失败，异常信息：" + WebSecurityManager.DataManager.AESDecrypt(loginResult.MessageStr));
                return false;
            }

            hasIEMSWeb = true; //标识是否正确部署了iEMSWeb
            return true;
        }
        /// <summary>
        /// 封装登录加密信息
        /// </summary>
        /// <returns></returns>
        //public UserValidateMsgWithCode LoginWithUserID(uint userID)
        //{
        //    UserValidateMsgWithCode resultValue = new UserValidateMsgWithCode();
        //    //var userNodeTemp = UserAuthNodeManager.DataManager.FindUserNodeByTypeID(UserNodeType.USER_NODE, userID);

        //    if (userNodeTemp != null)
        //    {
        //        uint userIDTemp;
        //        UserNode userNode = userNodeTemp as UserNode;
        //        string pwd = WebSecurityManager.DataManager.AESEncrypt(userNode.UserPassword);
        //        resultValue = LoginWithCode(userNode.UserDescription, pwd, out userIDTemp);

        //        bool successBool = true;
        //        string success = WebSecurityManager.DataManager.AESDecrypt(resultValue.Success, ref successBool);
        //        if (success.ToLower() == "true")
        //            resultValue.MessageStr = WebSecurityManager.DataManager.AESEncrypt(userNode.UserDescription);
        //    }
        //    else
        //    {
        //        resultValue.Success = WebSecurityManager.DataManager.AESEncrypt("false");
        //        var temp = "UserId not exists.";
        //        resultValue.MessageStr = WebSecurityManager.DataManager.AESEncrypt(temp);
        //        ErrorInfoManager.Instance.WriteLogMessage(temp);
        //    }
        //    return resultValue;
        //}
        ///// <summary>
        ///// 封装登录加密信息
        ///// </summary>
        ///// <param name="userName"></param>
        ///// <param name="password"></param>
        ///// <param name="clientKey"></param>
        ///// <returns></returns>
        //public UserValidateMsgWithCode LoginWithCode(string userName, string pwdCode, out uint userID)
        //{
        //    UserValidateMsgWithCode resultValue = new UserValidateMsgWithCode();
        //    bool success = true;
        //    string pwd = WebSecurityManager.DataManager.AESDecrypt(pwdCode, ref success);
        //    string resultFlag;
        //    string resultMsg;
        //    userID = 0;
        //    if (!success)
        //    {
        //        resultFlag = "false";
        //        resultMsg = LocalResourceManager.GetInstance().GetString("0271", "User Name or Password Error!");
        //    }
        //    else
        //    {
        //        UserValidMsg validateResult = SysAuthManager.DataManager.Login(userName, pwd);
        //        if (validateResult.Success)
        //        {
        //            if (IsStrongPassword(userName, pwd))
        //            {
        //                resultFlag = "true";
        //                resultMsg = validateResult.UserID.ToString();
        //                userID = validateResult.UserID;
        //                resultValue.Token = WebSecurityManager.DataManager.GetToken(userID); //将当前用户的token返回给客户端
        //                resultValue.IsStrongPwd = true;
        //            }
        //            else
        //            {
        //                resultFlag = "true";
        //                resultMsg = string.Empty;
        //                resultValue.Token = string.Empty;
        //                resultValue.IsStrongPwd = false;
        //                userID = validateResult.UserID;
        //            }
        //        }
        //        else
        //        {
        //            resultFlag = "false";
        //            resultMsg = validateResult.MessageStr;
        //        }
        //    }

        //    resultValue.Success = WebSecurityManager.DataManager.AESEncrypt(resultFlag);
        //    resultValue.MessageStr = WebSecurityManager.DataManager.AESEncrypt(resultMsg);


        //    return resultValue;
        //}

        ///// <summary>
        ///// 验证强密码
        ///// </summary>
        ///// <param name="username"></param>
        ///// <param name="password"></param>
        ///// <returns></returns>
        //public bool IsStrongPassword(string username, string password)
        //{
        //    //密码不能包含用户名
        //    if (password.Contains(username)) return false;
        //    //密码长度应该超过8位
        //    if (password.Length < 8) return false;

        //    //字符统计
        //    int iNum = 0, iLttUpper = 0, iLttLower = 0, iSym = 0;

        //    foreach (char c in password)
        //    {
        //        if (c >= '0' && c <= '9') iNum++;
        //        else if (c >= 'a' && c <= 'z') iLttLower++;
        //        else if (c >= 'A' && c <= 'Z') iLttUpper++;
        //        else iSym++;
        //    }

        //    //密码中应该至少包含一个大写字母，小写字母，数字及其他字符
        //    if (iNum == 0 || iLttUpper == 0 || iLttLower == 0 || iSym == 0) return false;

        //    return true;
        //}
        /// <summary>
        ///  调用接口初始化
        /// </summary>
        /// <param name="poi">poi信息</param>
        /// <param name="perNum">查询的条数</param>
        /// <returns></returns>
        private static UserValidateMsg InitializeService(uint userId)
        {
            try
            {
                return BasicDataProvider.GetInstance().InitializeService(userId);
            }
            catch (Exception ex)
            {
                LogService.LogManager.getInstance().Error(ex.Message + ":" + ex.StackTrace);
            }
            return new UserValidateMsg();
        }

        /// <summary>
        ///  调用接口登录
        /// </summary>
        /// <param name="poi">poi信息</param>
        /// <param name="perNum">查询的条数</param>
        /// <returns></returns>
        private static UserValidateMsgWithCode LoginStandaloneFalse(string innerid)
        {
            try
            {
                return BasicDataProvider.GetInstance().LoginStandaloneFalse(innerid);
            }
            catch (Exception ex)
            {
                LogService.LogManager.getInstance().Error(ex.Message + ":" + ex.StackTrace);
            }
            return new UserValidateMsgWithCode();
        }

        /// <summary>
        /// 加载自定义节点树，一次性加载所有层级的节点
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static WebGeneralResult<CustomNodeView> LoadCustomTree(uint userId)
        {
            try
            {
                return BasicDataProvider.GetInstance().LoadCustomTree(userId, GetTokenHashtable(userId));
            }
            catch (Exception ex)
            {
                LogService.LogManager.getInstance().Error(ex.Message + ":" + ex.StackTrace);
            }
            return new WebGeneralResult<CustomNodeView>();
        }

        /// <summary>
        /// 从iemsweb的接口获取事件统计数据
        /// </summary>
        /// <param name="nodeParams">nodetype,nodeid;nodetype,nodeid</param>
        /// <param name="timeParam"></param>
        /// <param name="filterParam"></param>
        /// <param name="typeClassParam"></param>
        /// <param name="maxRowCount"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        /// <summary>
        /// 1.区域报警事件数量统计  
        /// 2.报警事件处理状态统计
        /// 3.区域报警事件分类统计
        /// 4.报警事件分类占比统计
        /// 5.获取指定用户关联项目的统计信息，查询最近1天，最近7天等固定时间段内的报警事件
        /// 6.按时间分组的统计信息，此时x轴显示的是时间，打点间隔的确定如下所示：
        ///查询1天的数据，打点间隔是小时；
        ///查询1周，1个月的数据，打点间隔是天；
        ///查询1个季度，1年的数据，打点间隔是月；
        ///自定义查询时段：打点间隔是天（在web前端会限制自定义查询时段不能超过1个月）；
        ///  7.包含事件统计数据的节点集合
        /// </summary>
        /// <param name="nodeParams">自定义类型节点（自定义根节点、自定义组节点、自定义节点）列表，类型和ID以逗号间隔，多个节点以分号间隔</param>
        /// <param name="timeParam">始时间-结束时间，以逗号间隔</param>
        /// <param name="maxRowCount">一个节点所包含的alarmList的元素个数，不能超过这个值，因为数据太多会超过Json的限制导致报错</param>
        /// <param name="interval">统计间隔，1-5日周月季年，6-小时</param>
        /// <returns></returns>
        public static WebGeneralResult<StatisticForAlarm> GetStatisticsForAlarm(string nodeParams, string timeParam, string filterParam, string typeClassParam, int maxRowCount, int interval, uint userID)
        {
            try
            {
                return BasicDataProvider.GetInstance().GetStatisticsForAlarm(nodeParams, timeParam, filterParam, typeClassParam, maxRowCount, interval, GetTokenHashtable(userID));
            }
            catch (Exception ex)
            {
                LogService.LogManager.getInstance().Error(ex.Message + ":" + ex.StackTrace);
            }
            return new WebGeneralResult<StatisticForAlarm>();
        }

        /// <summary>
        /// 获取Token信息
        /// </summary>
        /// <returns></returns>
        public static Hashtable GetTokenHashtable(uint userID)
        {
            UserValidateMsgWithCode loginResult;
            if (!SafetyDataManager.InitializeIemsWebServiceAndLogin(userID, out loginResult)) //为了获取token而写的
            {
                return null;
            }
            Hashtable tokenPara = JsonConvert.DeserializeObject<Hashtable>(loginResult.Token);
            return tokenPara;
        }


        #endregion

        #region 解析字符串相关

        /// <summary>
        /// 将list转成字符串（Json格式字符串，便于扩展）
        /// </summary>
        /// <param name="deviceNodeList"></param>
        /// <returns></returns>
        public static string GetJsonStr(List<LogicalDeviceIndex> deviceNodeList)
        {
            return DataFormatManager.Create(FormatType.JsonType).SerializeObject(deviceNodeList);
        }

        /// <summary>
        /// 从Json获取List结果数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<LogicalDeviceIndex> GetListFromJson(string str)
        {
            return DataFormatManager.Create(FormatType.JsonType).DeserializeObject<List<LogicalDeviceIndex>>(str);
        }

        #endregion

        #region 读json文件

        /// <summary>
        /// 读取JSON文件
        /// </summary>
        /// <param name="jsonPath">json文件路径</param>
        /// <returns>json字符串</returns>
        public static string ReadJsonString(string jsonPath)
        {
            if (!File.Exists(jsonPath))
            {
                DbgTrace.dout("配置文件不存在：" + jsonPath);
                return string.Empty;
            }
            return File.ReadAllText(jsonPath, Encoding.Default);
        }

        #endregion

    }

    public class LogService
    {
        public class LogManager
        {
            private static LogManager _logManager = new LogManager();

            public static LogManager getInstance()
            {
                return _logManager;
            }

            public void Error(string s)
            {
                ErrorInfoManager.Instance.WriteLogMessage(s);
            }

            public void Info(string s)
            {
                ErrorInfoManager.Instance.WriteLogMessage(s);
            }

            public void Initialize(string log4netConfig)
            {

            }
        }
    }
}
