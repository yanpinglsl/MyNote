using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PecsReportQueryManager;
using PecsReport.CommomNode;
using System.IO;
using PecsReport.PluginInterface;

namespace OfficeReportInterface
{
    /// <summary>
    /// 时段报表数据查询
    /// </summary>
    public class PeriodReportPro
    {
        private uint nodeType;

        public uint NodeType
        {
            get { return nodeType; }
            set { nodeType = value; }
        }
        private uint nodeID;

        public uint NodeID
        {
            get { return nodeID; }
            set { nodeID = value; }
        }
        private DataSourceLabel perRepDataSourceLable;

        public DataSourceLabel PerRepDataSourceLable
        {
            get { return perRepDataSourceLable; }
            set { perRepDataSourceLable = value; }
        }

        public PeriodReportPro(DataSourceLabel perRepDSL)
        {
            perRepDataSourceLable = perRepDSL;
        }

        /// <summary>
        /// 查询时段报表的数据集
        /// </summary>
        /// <returns></returns>
        public DataResults ManagePeriodReport()  
        {
            DataResults interimResult = new DataResults();

            ParameterManager parameterManager = InitalParameterManager();
            List<List<DataStruct>> resultList = QueryManager.queryManager.LoadAndQueryReportFileToDataTable(parameterManager);
            //查询未获得数据，返回空的数据集
            if (resultList != null && resultList.Count != 0)
            {
                List<List<string>> result = DataStructToString(resultList);
                interimResult.name = perRepDataSourceLable.name;
                interimResult.dataMusterNum = 1;
                interimResult.dataMusterParamDiscribtionArray = new DataMusterParamDiscribtion[1];
                interimResult.dataMusterArray = new List<List<string>>[1];
                interimResult.dataMusterArray[0] = result;
                //标识该数据集为动态报表数据集
                interimResult.dataMusterParamDiscribtionArray[0] = DataMusterParamDiscribtion.PeriodReport;
                if (this.perRepDataSourceLable.excelType == FileType.FlexibleType)
                    interimResult.dataMusterParamDiscribtionArray[0] = DataMusterParamDiscribtion.DynamicReport;
            }
            return interimResult;
        }

        private PecsReportQueryManange InitalQueryManange()
        {
            string currentDllPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            PecsReportQueryManange.DllFilePath = currentDllPath;
            PecsReportQueryManange.PluginFilePath = Path.Combine(currentDllPath, @"Plugins\PecsReport\");
            PecsReportQueryManange.DBConnectFilePath = AppDomain.CurrentDomain.BaseDirectory;
            PecsReportQueryManange queryManager = new PecsReportQueryManange();
            queryManager.Initialize();
            return queryManager;
        }

        /// <summary>
        /// 设置报表查询参数
        /// </summary>
        /// <returns></returns>
        private ParameterManager InitalParameterManager()
        {
            ParameterManager parameterManager = new ParameterManager();
            parameterManager.ReportNodeID = perRepDataSourceLable.source;//这里获取报表ID 
            if (perRepDataSourceLable.reportNode != null)
            {
                parameterManager.ReportNodeType = perRepDataSourceLable.reportNode.NodeType; //这里获取报表节点类型 
                parameterManager.StartTime = perRepDataSourceLable.startTime;
                parameterManager.EndTime = perRepDataSourceLable.endTime;
            }
            uint tempNodeType = 0;
            uint tempNodeID = 0;
            if (string.IsNullOrEmpty(perRepDataSourceLable.node))
            {
                tempNodeType = this.nodeType;
                tempNodeID = this.NodeID;
            }
            if (perRepDataSourceLable.node != null)
            {
                string[] dataSourceStr = perRepDataSourceLable.node.Split('(', ')');
                //nodetype
                tempNodeType = Convert.ToUInt32(dataSourceStr[1]);
                tempNodeID = Convert.ToUInt32(dataSourceStr[2]);
                //兼容报表中节点设置为(0)0的情况
                if (tempNodeID == 0)
                {
                    tempNodeType = this.nodeType;
                    tempNodeID = this.NodeID;
                }
            }
            List<NodeTypeIDParam> nodeList = new List<NodeTypeIDParam>();
            NodeTypeIDParam nodeTypeId = new NodeTypeIDParam(tempNodeType, tempNodeID);
            nodeList.Add(nodeTypeId);
            parameterManager.PqNodeList = nodeList;
            parameterManager.TimeType = (int)perRepDataSourceLable.interval;
            return parameterManager;
        }

        /// <summary>
        /// DataStruct中提取string信息
        /// </summary>
        /// <param name="dataStructList"></param>
        /// <returns></returns>
        private List<List<string>> DataStructToString(List<List<DataStruct>> dataStructList)
        {
            int row = dataStructList.Count;
            int column = dataStructList[0].Count;
            List<List<string>> StringStruct = new List<List<string>>();
            for (int i = 0; i < row; i++)
            {
                List<string> tempRowStringStruct = new List<string>();
                for (int j = 0; j < column; j++)
                {
                    tempRowStringStruct.Add(dataStructList[i][j].value);
                }
                StringStruct.Add(tempRowStringStruct);
            }
            return StringStruct;
        }
    }
}
