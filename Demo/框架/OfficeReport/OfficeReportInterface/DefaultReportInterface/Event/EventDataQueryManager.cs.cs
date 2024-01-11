using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
//using CET.PecsNodeManage;
using OfficeReportInterface.DefaultReportInterface.CommonluUsed;
using DBInterfaceCommonLib;
using System.Globalization;
using CET.PecsNodeManage;
using DBInterfaceModels;
using BasicDataInterface.Models.Response;

namespace OfficeReportInterface.DefaultReportInterface
{
    public class EventDataQueryManager : IDataSheet
    {
        private const int Max_Events_Count=1000; 

        public List<DataTable> GetDataLogDatas(DefaultReportParameter parameter)
        {
            List<DataTable> resultDTList = new List<DataTable>();
            int[] eventClass = GetEventClass(parameter);
            DataTable warningDT = DefaultTemplatePublicMethod.ConstructWarningTable(parameter.IsIncludeWarning);
            DataTable summaryDT = ConstructDataSummaryTable(parameter);
            DataTable resultStationDT = QueryDeviceEventData(parameter, eventClass, 1, warningDT);
            DataTable resultChannelDT = QueryDeviceEventData(parameter, eventClass, 2, warningDT);
            DataTable resultDeviceDT = QueryDeviceEventData(parameter, eventClass, 3, warningDT);
            resultDTList.Add(warningDT);
            resultDTList.Add(summaryDT);
            resultDTList.Add(resultStationDT);
            resultDTList.Add(resultChannelDT);
            resultDTList.Add(resultDeviceDT);
            return resultDTList;
        }

        private DataTable ConstructDataSummaryTable(DefaultReportParameter parameter)
        {
            DataTable resultDT = new DataTable("Summary");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "1");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "2");
            DataRow resultRow = resultDT.NewRow();
    
            resultRow[0] = DataManager.GetStartTimeSting(parameter.StartTime);
            resultRow[1] = DataManager.GetEndTimeString(parameter.EndTime);
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultRow[0] = GetSourceNameStr(parameter,1);
            if (parameter.StationChannelDeviceList[0].Count > 0)
                resultRow[1] = "station";
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultRow[0] = GetSourceNameStr(parameter, 2);
            if (parameter.StationChannelDeviceList[1].Count > 0)
                resultRow[1] = "channel";
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultRow[0] = GetSourceNameStr(parameter, 3);
            if (parameter.StationChannelDeviceList[2].Count > 0)
                resultRow[1] = "station";
            resultDT.Rows.Add(resultRow);
         
            resultRow = resultDT.NewRow();
            resultRow[0] = DataManager.TimeFormatWithMinisecondForExcel;
            resultDT.Rows.Add(resultRow);
            return resultDT;
        }

        private string GetSourceNameStr(DefaultReportParameter parameter, int type)
        {
            string dataSource = string.Empty;
            uint nodeType;
            if (type == 1)
                nodeType = PecsNodeType.PECSSTATION_NODE;
            else if (type == 2)
                nodeType = PecsNodeType.PECSCHANNEL_NODE;
            else
                nodeType = PecsNodeType.PECSDEVICE_NODE;
            for (int i = 0; i < parameter.StationChannelDeviceList[type - 1].Count; i++)
            {
                SysNode node = PecsNodeManager.PecsNodeInstance.GetNodeByTypeID(nodeType, parameter.StationChannelDeviceList[type - 1][i]);

                //if (i == parameter.StationChannelDeviceList[type - 1].Count-1)
                //    dataSource = dataSource + node.NodeName ;
                //else
                {
                    dataSource = dataSource + node.NodeName + "; ";
                }
                
            }

            if (dataSource.Length > 0)
                dataSource = dataSource.Substring(0, dataSource.Length - 1);
            return dataSource;
        }

        private int[] GetEventClass(DefaultReportParameter parameter)
        {
            List<int> eventClassList = new List<int>();
            if ((parameter.Interval & 0x01) != 0)
                eventClassList.Add(1);
            if ((parameter.Interval & 0x10) != 0)
                eventClassList.Add(2);
            if ((parameter.Interval & 0x100) != 0)
                eventClassList.Add(3);
            if ((parameter.Interval & 0x1000) != 0)
                eventClassList.Add(0);
            return eventClassList.ToArray();
        }

        /// <summary>
        /// 查询系统历史事件数据
        /// </summary>
        /// <param name="deviceIDs">设备</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>结果集</returns>
        public DataTable QueryDeviceEventData(DefaultReportParameter parameter, int[] eventClasses, int type,DataTable warningDT)
        {
            uint nodeType;
            DataTable eventHistoryDT = new DataTable("EventHistoryDT");
            if (type == 1)
            {
                this.CreateStationEventColumns(ref eventHistoryDT);
                nodeType = PecsNodeType.PECSSTATION_NODE;
            }
            else if (type == 2)
            {
                this.CreateChannelEventColumns(ref eventHistoryDT);
                nodeType = PecsNodeType.PECSCHANNEL_NODE;
            }
            else
            {
                this.CreateDeviceEventColumns(ref eventHistoryDT);
                nodeType = PecsNodeType.PECSDEVICE_NODE;
            }
            List<NODE_TYPE_ID> nodeTypeIDList = new List<NODE_TYPE_ID>();
            for (int i = 0; i < parameter.StationChannelDeviceList[type - 1].Count; i++)
            {
                NODE_TYPE_ID nodeTypeID = new NODE_TYPE_ID();
                nodeTypeID.nodeType = nodeType;
                nodeTypeID.nodeID = parameter.StationChannelDeviceList[type - 1][i];
                nodeTypeIDList.Add(nodeTypeID);
            }
            if (nodeTypeIDList.Count == 0)
                return eventHistoryDT;
            var returnReslut = PD_TB_06_Manager.ReadEventLogsByMultiStaChnDev(nodeTypeIDList, parameter.StartTime, parameter.EndTime, new int[] { }, eventClasses);
             if (!returnReslut.Success)
            {
                DbgTrace.dout("GetAllEvents:{0}", returnReslut.ErrorMessage);
                return eventHistoryDT;
            }
            List<EventLogByDevResponse> resultDT = new List<EventLogByDevResponse>();
            //string cc = DBInterfaceCommonLib.ErrorQuerier.Instance.GetLastErrorString();
            if (resultDT.Count == 0)
            {
                if (!parameter.IsIncludeWarning)
                    return eventHistoryDT;
                DataRow dr = warningDT.NewRow();
                if (type == 1)
                    dr[0] = string.Format("{0} {1}", LocalResourceManager.GetInstance().GetString("0165", "Station Event:"), LocalResourceManager.GetInstance().GetString("0058", "data is Null!"));
                else if (type == 2)
                    dr[0] = string.Format("{0} {1}", LocalResourceManager.GetInstance().GetString("0166", "Channel Event:"), LocalResourceManager.GetInstance().GetString("0058", "data is Null!"));
                else if (type == 3)
                    dr[0] = string.Format("{0} {1}", LocalResourceManager.GetInstance().GetString("0167", "Device Event:"), LocalResourceManager.GetInstance().GetString("0058", "data is Null!"));

                dr[1] = DateTime.Now.ToString(DataManager.GetWarningMessageTimeFormat());
                warningDT.Rows.Add(dr);
            }
            if (resultDT.Count >= Max_Events_Count && parameter.IsIncludeWarning)
            {
                string formatTemp=LocalResourceManager.GetInstance().GetString("0591", "There are more than {0} events. The report only displays the latest {0} events.");

                string warningInfo = string.Format(formatTemp, Max_Events_Count);
                DataRow warningDR = warningDT.NewRow();       
                if (type == 1)
                    warningInfo = string.Format("{0} {1}", LocalResourceManager.GetInstance().GetString("0165", "Station Event:"), warningInfo);
                else if (type == 2)
                    warningInfo = string.Format("{0} {1}", LocalResourceManager.GetInstance().GetString("0166", "Channel Event:"), warningInfo);
                else if (type == 3)
                    warningInfo = string.Format("{0} {1}", LocalResourceManager.GetInstance().GetString("0167", "Device Event:"), warningInfo);
                warningDR[0]=warningInfo;             
                warningDT.Rows.Add(warningDR);
            }
            for (int i = 0; i < resultDT.Count; i++)
            {
                if (i >= Max_Events_Count)
                    break;
                AddEventTableHeadRow(ref eventHistoryDT, type);
                uint stationID = Convert.ToUInt32(resultDT[i].StationID);
                uint channelID = Convert.ToUInt32(resultDT[i].ChannelID);
                uint deviceID = Convert.ToUInt32(resultDT[i].DeviceID);
                string stationName = PecsNodeManager.PecsNodeInstance.GetStationName(stationID);
                string channelName = PecsNodeManager.PecsNodeInstance.GetChannelName(stationID, channelID);
                string deviceName = PecsNodeManager.PecsNodeInstance.GetDeviceName(stationID, channelID, deviceID);
                AddEventTableHeadRow(ref eventHistoryDT, type);

                DataRow row = eventHistoryDT.NewRow();
                row["Index"] = eventHistoryDT.Rows.Count;
                int msec = Convert.ToInt32(resultDT[i].EventTime.Split('.')[1]);
                DateTime logTime = Convert.ToDateTime(resultDT[i].EventTime.Split('.')[0]);
                row["LogTime"] = PowerQualityDataManager.GetDateTimeStringByCultrue(logTime, msec);
                row["Description"] = Convert.ToString(resultDT[i].Description);
                row["Station"] = stationName;
                if (type > 1)
                    row["Channel"] = channelName;
                if (type > 2)
                    row["Device"] = deviceName;

                int eventClass = Convert.ToInt32(resultDT[i].EventClass);
                if (!eventClasses.Contains(eventClass))
                    continue;
                string eventClassStr = string.Empty;
                bool success = ReportWebServiceManager.ReportWebManager.EventClassIDNameMap.TryGetValue(eventClass, out eventClassStr);
                row["EventClass"] = eventClassStr;

                int eventType = Convert.ToInt32(resultDT[i].EventType);
                string eventTypeStr = string.Empty;
                success = ReportWebServiceManager.ReportWebManager.EventTypeIDNameMap.TryGetValue(eventType, out eventTypeStr);
                row["EventType"] = eventTypeStr;
                eventHistoryDT.Rows.Add(row);
            }
            return eventHistoryDT;
        }


        /// <summary>
        /// 创建添加历史事件列表列
        /// </summary>
        /// <param name="eventHistoryDT">事件结果集</param>
        private void CreateDeviceEventColumns(ref DataTable eventHistoryDT)
        {
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "Index");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "LogTime");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "Station");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "Channel");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "Device");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "EventClass");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "EventType");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "Description");
        }

        /// <summary>
        /// 创建添加历史事件列表列
        /// </summary>
        /// <param name="eventHistoryDT">事件结果集</param>
        private void CreateChannelEventColumns(ref DataTable eventHistoryDT)
        {
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "Index");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "LogTime");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "Station");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "Channel");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "EventClass");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "EventType");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "Description");
        }

        /// <summary>
        /// 创建添加历史事件列表列
        /// </summary>
        /// <param name="eventHistoryDT">事件结果集</param>
        private void CreateStationEventColumns(ref DataTable eventHistoryDT)
        {
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "Index");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "LogTime");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "Station");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "EventClass");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "EventType");
            DefaultTemplatePublicMethod.AddColumnToTable(eventHistoryDT, "Description");
        }

        private void AddEventTableHeadRow(ref DataTable eventHistoryDT, int type)
        {
            if (eventHistoryDT.Rows.Count == 0)
            {
                DataRow row = eventHistoryDT.NewRow();
                row["Index"] = LocalResourceManager.GetInstance().GetString("0157", "No.");
                row["LogTime"] = LocalResourceManager.GetInstance().GetString("0158", "TimeStamp"); 
                row["Station"] = LocalResourceManager.GetInstance().GetString("0159", "Station"); 
                if (type > 1)
                    row["Channel"] = LocalResourceManager.GetInstance().GetString("0160", "Channel"); 
                if (type > 2)
                    row["Device"] = LocalResourceManager.GetInstance().GetString("0161", "Device"); 
                row["EventClass"] = LocalResourceManager.GetInstance().GetString("0162", "EventClass"); 
                row["EventType"] = LocalResourceManager.GetInstance().GetString("0163", "EventType"); 
                row["Description"] = LocalResourceManager.GetInstance().GetString("0164", "Description");
                eventHistoryDT.Rows.Add(row);
            }
        }
    }
}
