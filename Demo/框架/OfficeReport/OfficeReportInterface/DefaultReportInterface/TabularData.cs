using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
//using CET.PecsNodeManage;
using OfficeReportInterface.DefaultReportInterface;


namespace OfficeReportInterface
{
    /// <summary>
    ///TabularData 的摘要说明
    /// </summary>
    public class TabularData
    {
        private static uint source = (uint) RepServFileType.Tabular;
        public TabularData()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        public static DataTable GetTabularData(string deviceIDs, string queryParas, int paraTypeID, DateTime startTime, DateTime endTime)
        {
            List<DeviceDataIDDef> queryParamList = ParaParse(deviceIDs, paraTypeID, queryParas);
            List<List<DataLogOriDef>> tempResult = QueryHistoryTrendDataLogView(startTime, endTime, queryParamList);
            return GetTabularDataResult(queryParamList, tempResult, paraTypeID);
        }

        private static DataTable GetTabularDataResult(List<DeviceDataIDDef> queryParamList, List<List<DataLogOriDef>> tempResult, int paraTypeID)
        {
            return FillDataListToTable(queryParamList, tempResult, paraTypeID);
        }

        private static List<List<DataLogOriDef>> QueryHistoryTrendDataLogView(DateTime startTime, DateTime endTime, List<DeviceDataIDDef> queryParamList)
        {
            return HistoryDataQueryManager.DataManager.QueryHistoryTrendDataLogView(queryParamList, startTime, endTime);
        }

        private static DataTable FillDataListToTable(List<DeviceDataIDDef> queryParamList, List<List<DataLogOriDef>> tempResult, int paraTypeID)
        {
            DataTable resultTable = new DataTable("TabularDT");
            ConstrutTable(resultTable);
            //对于一个DeviceDataIDDef
            for (int i = 0; i < queryParamList.Count; i++)
            {
            //    SysNode deviceNode = PecsNodeManager.PecsNodeInstance.GetDeviceNodeByID(queryParamList[i].DeviceID);
                uint deviceNodeID = queryParamList[i].DeviceID;
            //    if (deviceNode != null)
              //  {
              //      deviceNodeID = deviceNode.NodeID;
             //   }

                string deviceName;//= deviceNode == null ? string.Empty : deviceNode.NodeName;
                if (! NamesManager.GetInstance(source).GetDeviceNameWithLoop(queryParamList[i], out deviceName))
                    deviceName = string.Empty;
               
                string dataName = ReportWebServiceManager.ReportWebManager.GetParaNameByDataID(queryParamList[i].DataID);
                string dataTypeName = DataTypeDefine.GetDataTypeName(queryParamList[i].DataTypeID);
                List<DataLogOriDef> dataLogVaueList = tempResult[i];
                for (int j = 0; j < dataLogVaueList.Count; j++)
                {
                    DataRow dr = resultTable.NewRow();
                    dr["DeviceID"] = deviceNodeID;
                    dr["DeviceName"] = deviceName;
                    dr["DataID"] = queryParamList[i].DataID * 100 + queryParamList[i].LogicalDeviceIndex;
                    dr["DataName"] = dataName + "-loop" + queryParamList[i].LogicalDeviceIndex;

                    dr["DataTypeID"] = queryParamList[i].DataTypeID;
                    dr["DataTypeName"] = dataTypeName;
                    if (paraTypeID == (int)DataIDParaType.HighDataLogType)
                        dr["LogTime"] = dataLogVaueList[j].LogTime.ToString("yyyy-MM-dd HH:mm:ss") + "." + dataLogVaueList[j].LogTime.Millisecond;
                    else
                        dr["LogTime"] = dataLogVaueList[j].LogTime.ToString("yyyy-MM-dd HH:mm:ss");
                    dr["DataValue"] = DataFormatManager.GetFormattedDoubleByDigits(dataLogVaueList[j].DataValue);
                    resultTable.Rows.Add(dr);
                }
            }
            return resultTable;
        }

        private static void ConstrutTable(DataTable resultTable)
        {
            //构建返回结果表
            resultTable.Columns.Add("DeviceID");
            resultTable.Columns.Add("DeviceName");
            resultTable.Columns.Add("DataID");
            resultTable.Columns.Add("DataName");
            resultTable.Columns.Add("DataTypeID");
            resultTable.Columns.Add("DataTypeName");
            resultTable.Columns.Add("LogTime");
            resultTable.Columns.Add("DataValue");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceIDs">旧版本的是 deviceId;deviceId;deviceId  新增nodetype后是 nodeType-deviceId；nodeType-deviceId; </param>
        /// <param name="paraTypeID"></param>
        /// <param name="queryParas"></param>
        /// <returns></returns>
        private static List<DeviceDataIDDef> ParaParse(string deviceIDs, int paraTypeID, string queryParas)
        {
            List<DeviceDataIDDef> queryParamList = new List<DeviceDataIDDef>();
            //获取传入的参数
            List<NodeParam> deviceIDList = NodeParam.GetNodeParamList(deviceIDs);//DataFormatManager.ParseUIntList(deviceIDs, ";");
            //根据传入的DeviceID和DataID和DataTypeID进行组合，形成查询条件列表
            for (int i = 0; i < deviceIDList.Count; i++)
            {
                string[] resultStrs = queryParas.Split(';');
                for (int j = 0; j < resultStrs.Length; j++)
                {
                    string paraStrs = resultStrs[j];
                    List<uint> paraList = DataFormatManager.ParseUIntList(paraStrs, ",");

                    if (paraList.Count > 1)
                    {
                        uint dataID = paraList[0];
                        uint dataTypeID = 0;
                        dataTypeID = paraList[1];
                        int logicalDeviceIndex = Convert.ToInt32(paraList[2]);
                        DeviceDataIDDef newParam = new DeviceDataIDDef(deviceIDList[i].NodeType, deviceIDList[i].NodeID, dataID, paraTypeID, Convert.ToInt32(dataTypeID), logicalDeviceIndex);
                        queryParamList.Add(newParam);
                    }

                }
            }
            return queryParamList;
        }
    }
}
