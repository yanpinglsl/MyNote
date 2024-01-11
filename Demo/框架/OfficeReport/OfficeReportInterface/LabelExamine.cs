using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using PecsReport.CommomNode;
using CET.PecsNodeManage;
using PecsReport.PluginInterface;
using PecsReportQueryManager;

namespace OfficeReportInterface
{
    public class LabelExamine
    {
        private DataSourceLabel sourceLabelExamined = new DataSourceLabel();
        private DataRetrievalLabel retrievalLabelExamined = new DataRetrievalLabel();
        private bool needEndtime = false;
        private bool needSelectDeviceNode = false;

        /// <summary>
        /// 是否需要在查询时指定设备节点
        /// </summary>
        public bool NeedSelectDeviceNode
        {
            get { return needSelectDeviceNode; }
            set { needSelectDeviceNode = value; }
        }

        /// <summary>
        /// 查询时是否需要起止时间（包括时段报表以及reportingservice报表）
        /// </summary>
        public bool NeedEndtime
        {
            get { return this.needEndtime; }
            set { this.needEndtime = value; }
        }

        /// <summary>
        /// 校验数据源标签
        /// </summary>
        /// <param name="dataSourceLabel"></param>
        public LabelExamine(DataSourceLabel dataSourceLabel)
        {
            sourceLabelExamined = dataSourceLabel;
        }

        /// <summary>
        /// 校验引用标签
        /// </summary>
        /// <param name="dataRetrievalLabel"></param>
        public LabelExamine(DataRetrievalLabel dataRetrievalLabel)
        {
            retrievalLabelExamined = dataRetrievalLabel;
        }

        /// <summary>
        /// 数据源标签Name参数的校验
        /// </summary>
        /// <returns></returns>
        public bool DSLNameParmExamine()
        {
            if (sourceLabelExamined.name == "" || sourceLabelExamined.name == null)
                return false;
            return true;
        }

        /// <summary>
        /// 数据源标签引用报表是否存在的校验
        /// </summary>
        /// <returns></returns>
        public bool DSLSourceParmExamine()
        {
            //为reporting service报表时不关联报表源，且需要选择截止时间
            if (sourceLabelExamined.source >= (int)RepServFileType.MinumOfTemplate)
            {
                needEndtime = true;
                return true;
            }
            ReportNode reportNode = ReportFileManager.GetInstance().FindTargetReportNode(sourceLabelExamined.source);
            if (reportNode == null)
                return false;

            int reportFlag = 0;
            int res = (int)ReportFileManager.GetReportQueryType(reportNode, ref reportFlag);
            if (reportNode.NodeType == ReportNodeType.REPORTPECSTAR)   //pecstar类型报表
            {
                //officereport将支持设备节点为空的情况
                if (res > (int)ReportQueryType.ReportPeriod && res <= (int)ReportQueryType.ReportFlexibleTemplate)
                {
                    if (sourceLabelExamined.node == "(0)0")
                        needSelectDeviceNode = true;
                }
            }
            else if (reportNode.NodeType == ReportNodeType.REPORTENGSYS)  //EEM类型报表，只支持综合月报和综合年报
            {
                if (res != 6 && res != 7)
                    return false;
            }

            //依据不同的报表类型加载不同的设置时间
            //若为时段报表
            if (res == (int)ReportQueryType.ReportPeriod || res == (int)ReportQueryType.ReportPeriodTemplate || res == (int)ReportQueryType.ReportFlexibleTemplate)
                needEndtime = true;
            //若为动态报表
            if (res == (int)ReportQueryType.ReportFlexibleTemplate)
            {
                if (sourceLabelExamined.querySpan == 0)
                    needEndtime = true;
            }
            //若为普通报表和模板报表
            if (res >= 0 && res <= 8 && res != 4)
            {
                if (sourceLabelExamined.dateSpan == 0)
                    needEndtime = true;
            }
            return true;
        }

        /// <summary>
        /// 数据源标签dateSpan参数是否大于-1的校验
        /// </summary>
        /// <returns></returns>
        public bool DSLDataSpanParmExamine()
        {
            if (sourceLabelExamined.dateSpan > -1)
                return true;
            return false;
        }

        /// <summary>
        /// 数据源标签的校验
        /// </summary>
        /// <returns></returns>
        public int DSLAllExamine()
        {
            if (!DSLNameParmExamine())
                return 1;
            if (!DSLSourceParmExamine())
                return 2;
            if (!DSLDataSpanParmExamine())
                return 3;
            if (sourceLabelExamined.source >= (int)RepServFileType.MinumOfTemplate)
                return 0;
            DeviceNodeIsValid();
            return 0;
        }

        /// <summary>
        /// 数据源标签中的设备节点参数是否为有效设备节点（无效则查询时动态选择）
        /// </summary>
        /// <returns></returns>
        public void DeviceNodeIsValid()
        {
            ReportNode reportNode = ReportFileManager.GetInstance().FindTargetReportNode(sourceLabelExamined.source);
            if (reportNode.NodeType != ReportNodeType.REPORTPECSTAR)   //非pecstar类型报表
            {
                this.needSelectDeviceNode = false;
                return;
            }
            int reportFlag = 0;
            ReportQueryType reportType = ReportFileManager.GetReportQueryType(reportNode, ref reportFlag);
            if (reportType <= ReportQueryType.ReportPeriod)
                return;
            //校验设备节点
            string NodeIDParam = sourceLabelExamined.node;
            if (string.IsNullOrEmpty(NodeIDParam))
            {
                this.needSelectDeviceNode = true;
                return;
            }
            string[] dataSourceStr = NodeIDParam.Split('(', ')');
            uint NodeType = Convert.ToUInt32(dataSourceStr[1]);
            uint NodeID = Convert.ToUInt32(dataSourceStr[2]);
            SysNode sysNode = PecsNodeManager.PecsNodeInstance.GetNodeByTypeID(NodeType, NodeID);
            if (sysNode == null)
                this.needSelectDeviceNode = true;
        }

        /// <summary>
        /// 引用标签Name参数的校验
        /// </summary>
        /// <param name="dataSourceLabelArray"></param>
        /// <returns></returns>
        public bool DRLNameParmExamine(DataSourceLabel[] dataSourceLabelArray)
        {
            if (retrievalLabelExamined.name != "" && retrievalLabelExamined.name != null)
            {
                //与数据源标签进行比对
                foreach (DataSourceLabel DSL in dataSourceLabelArray)
                {
                    if (DSL.name == retrievalLabelExamined.name)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 引用标签Scope参数的校验
        /// </summary>
        /// <returns></returns>
        public bool DRLScopeParmExamine()
        {
            if (retrievalLabelExamined.scope == "" || retrievalLabelExamined.scope == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 引用标签的校验
        /// </summary>
        /// <param name="dataSourceLabelArray"></param>
        /// <returns></returns>
        public int DRLAllExamine(DataSourceLabel[] dataSourceLabelArray)
        {
            if (!DRLNameParmExamine(dataSourceLabelArray))
                return 1;
            if (!DRLScopeParmExamine())
                return 2;
            return 0;
        }
    }
}
