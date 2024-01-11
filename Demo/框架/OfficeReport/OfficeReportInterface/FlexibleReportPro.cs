using System;
using System.Collections.Generic;

using CET.PecsNodeManage;
using PecsReportQueryManager;

namespace OfficeReportInterface
{
    /// <summary>
    /// 动态报表数据查询
    /// </summary>
    class FlexibleReportPro
    {
        private static FlexibleReportPro flexibleReportPro = null;
        private DataSourceLabel flexRepDateSourceLable;
        private DataResults interimResult = new DataResults();
        private PecsReportQueryManange queryManager;

        public DataSourceLabel FlexRepDateSourceLable
        {
            get { return flexRepDateSourceLable; }
            set { flexRepDateSourceLable = value; }
        }

        public static FlexibleReportPro GetInstance()
        {
            if (flexibleReportPro == null)
                flexibleReportPro = new FlexibleReportPro();
            return flexibleReportPro;
        }

        ///// <summary>
        ///// 查询动态报表的数据集
        ///// </summary>
        ///// <param name="starttime"></param>
        ///// <returns></returns>
        //public DataResults ManageFlexibleTemplate2()
        //{
        //    ParameterManager parameterManager = InitalParameterManager();

        //    List<List<DataStruct>> result = queryManager.LoadAndQueryReportFileToDataTable(parameterManager);
        //    List<List<string>> realResult = new List<List<string>>();
        //    for (int i = 0; i < result.Count; i++)
        //    {
        //        List<string> row = new List<string>();
        //        for (int j = 0; j < result[i].Count; j++)
        //        {
        //            row.Add(result[i][j].value);
        //        }
        //        realResult.Add(row);
        //    }
        //    //查询未获得数据，返回空的数据集
        //    if (realResult == null || realResult.Count == 0)
        //        return interimResult;

        //    interimResult.name = flexrepdatesourcelable.name;
        //    interimResult.dataMusterNum = 1;
        //    interimResult.dataMusterParamDiscribtionArray = new DataMusterParamDiscribtion[1];
        //    interimResult.dataMusterArray = new List<List<string>>[1];

        //    interimResult.dataMusterArray[0] = realResult;
        //    //标识该数据集为动态报表数据集
        //    interimResult.dataMusterParamDiscribtionArray[0] = DataMusterParamDiscribtion.DynamicReport;

        //    return interimResult;
        //}

        /// <summary>
        /// 查询动态报表的数据集
        /// </summary>
        /// <param name="starttime"></param>
        /// <returns></returns>
        public DataResults ManageFlexibleTemplate(DateTime startTime, DateTime endTime, bool needDeviceNode, uint sysNodeType, uint sysNodeID,DataSourceLabel dataSourceLabel, ref List<Command> commandList)
        {
            uint nodeType;
            uint nodeID;
            string nodeIDParam = flexRepDateSourceLable.node;
            if (string.IsNullOrEmpty(nodeIDParam) && needDeviceNode)
            {
                nodeType = sysNodeType;
                nodeID = sysNodeID;
            }
            else
            {
                string[] dataSourceStr = nodeIDParam.Split('(', ')');
                nodeType = Convert.ToUInt32(dataSourceStr[1]);
                nodeID = Convert.ToUInt32(dataSourceStr[2]);
                SysNode sysNode = PecsNodeManager.PecsNodeInstance.GetNodeByTypeID(nodeType, nodeID);
                if (sysNode == null && needDeviceNode)
                {
                    nodeType = sysNodeType;
                    nodeID = sysNodeID;
                }
            }

            if (this.flexRepDateSourceLable.excelType == FileType.FlexibleType)
                this.flexRepDateSourceLable.excelType = FileType.DayType;
            List<List<string>> realResult = new List<List<string>>();
            PecReportCacheManager.ReportCacheManager cacheManager = new PecReportCacheManager.ReportCacheManager();
            realResult = cacheManager.GetReportResultByNodeID(this.flexRepDateSourceLable.source, nodeType, nodeID, startTime, (int)this.flexRepDateSourceLable.excelType);

            //查询未获得数据，返回空的数据集
            if (realResult == null || realResult.Count == 0)
            {
                Command commandInfo = new Command(dataSourceLabel.source, nodeType, nodeID, startTime);
                if (!commandList.Contains(commandInfo))
                    commandList.Add(commandInfo);
            }

            interimResult.name = flexRepDateSourceLable.name;
            interimResult.dataMusterNum = 1;
            interimResult.dataMusterParamDiscribtionArray = new DataMusterParamDiscribtion[1];
            interimResult.dataMusterArray = new List<List<string>>[1];

            interimResult.dataMusterArray[0] = realResult;
            //标识该数据集为动态报表数据集
         
            interimResult.dataMusterParamDiscribtionArray[0] = DataMusterParamDiscribtion.DynamicReport;

            return interimResult;
        }
    }
}
