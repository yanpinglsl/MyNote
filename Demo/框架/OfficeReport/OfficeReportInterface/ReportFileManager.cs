using System;
using System.Collections.Generic;
using System.Text;
using PecsReport.PluginInterface;
using System.IO;
using PecsReport.CommomNode;
using DBInterfaceCommonLib;
using CET.PecsNodeManage;
using System.Data;
using CSharpDBPlugin;

namespace OfficeReportInterface
{
    /// <summary>
    /// 报表文件管理类
    /// </summary>
    public class ReportFileManager
    {
        /// <summary>
        /// 报表管理类唯一实例
        /// </summary>
        private static ReportFileManager ReportManager = null;

        public static ReportFileManager GetInstance()
        {
            if (ReportManager == null)
                ReportManager = new ReportFileManager();

            return ReportManager;
        }

        /// <summary>
        /// 报表库根节点
        /// </summary>
        private ReportNode reportDepository;

        private ReportFileManager()
        {
            string defaultReportDepository = "Report";
            reportDepository = new ReportNode(null, ReportNodeType.REPORTDEPOT, ReportNodeType.REPORTDEPOT, defaultReportDepository, 3000, DateTime.Now);
        }

        public ReportNode RootReportNode
        {
            get { return reportDepository; }
        }

        /// <summary>
        /// 加载指定报表组节点下的子报表节点
        /// </summary>
        /// <param name="ParentReportNode">指定的报表组节点</param>
        /// <param name="selectReportType">当前选择的报表节点类型</param>
        /// <param name="filterReportType"></param>
        public void LoadReportFiles(ReportNode ParentReportNode, uint[] selectReportType, ReportQueryType[] filterReportType)
        {
            // 清除原来的子报表文件节点
            ParentReportNode.Clear();

            // 不加载报表文件节点的Data字段
            // 只有当根节点是报表库根节点或报表组节点时才执行加载
            ErrorMsg msg = new ErrorMsg();
            SysNode rootNode = PecsNodeManager.PecsNodeInstance.LoadSysReportNodes(selectReportType, ReportNode.DEFAULTNODEVERSION, ref msg);

            if (rootNode != null)
            {
                InitReportNodeTree(rootNode, ParentReportNode, filterReportType);
            }
        }

        /// <summary>
        /// 初始化树形结构
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="ParentReportNode">父节点</param>
        /// <param name="filterType">过滤报表类型，对于时段报表不加载</param>
        private void InitReportNodeTree(SysNode rootNode, ReportNode ParentReportNode, ReportQueryType[] filterReportType)
        {
            uint nodeType = rootNode.NodeType;
            if (nodeType == ReportNodeType.REPORTDEPOT || nodeType == ReportNodeType.REPORTGROUP)
            {
                for (int i = 0; i < rootNode.ChildrenCount; i++)
                {
                    SysNode childNode = (SysNode)rootNode.Find(i);
                    if (childNode == null)
                    {
                        continue;
                    }
                    // 将新节点添加至父节点的子节点列表中
                    ReportNode subNode = new ReportNode(ParentReportNode, childNode.NodeType, childNode.NodeID, childNode.NodeName, ReportNode.DEFAULTNODEVERSION, childNode.NodeUpdateTime);
                    int reportFlag = 0;
                    ReportQueryType queryType = GetReportQueryType(subNode, ref reportFlag);
                    if (childNode.NodeType != ReportNodeType.REPORTGROUP && reportFlag != 1)//只加载数据源报表
                        continue;
                    //由于版本没有用到，目前采用版本号来存储报表类型，避免二次解析
                    subNode.NodeVersion = Convert.ToUInt32(queryType);

                    if (IsQueryInFilterType(queryType, filterReportType))
                    {
                        continue;
                    }

                    ParentReportNode.Add(subNode);
                    // 递归加载PQ监测节点
                    InitReportNodeTree(childNode, subNode, filterReportType);
                }
            }
        }

        private bool IsQueryInFilterType(ReportQueryType queryType, ReportQueryType[] filterReportType)
        {
            for (int i = 0; i < filterReportType.Length; i++)
            {
                if (queryType == filterReportType[i])
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获取报表类型
        /// </summary>
        /// <param name="reportNode"></param>
        /// <returns></returns>
        public static ReportQueryType GetReportQueryType(ReportNode reportNode, ref int reportFlag)
        {
            ReportQueryType reportquerytype = ReportQueryType.ReportDay;
            MemoryStream reportfileStream = new MemoryStream();
            reportfileStream.SetLength(0);

            DataTable resultTable = new DataTable();
            SetupTableProvider.Instance.ReadSetupNodesByNodeTypeID(0, reportNode.NodeType, reportNode.NodeID, false, 1, true, ref resultTable);

            if (resultTable.Rows.Count == 1)
            {
                DataRow dr = resultTable.Rows[0];
                // image类型数据转换为byte[]存储
                if (!Convert.IsDBNull(dr["Data"]))
                {
                    byte[] bytes = (byte[])dr["Data"];
                    // 基于结果数据创建结果数据流对像
                    reportfileStream.Position = 0;
                    reportfileStream.Write(bytes, 0, bytes.Length);
                }
            }
            if (reportfileStream != null && reportfileStream.Length > 0)
            {
                reportfileStream.Position = 0;
                BinaryReader binReader = new BinaryReader(reportfileStream);
                reportFlag = binReader.ReadInt32();
                binReader.ReadInt32();
                binReader.ReadUInt32();

                int tmpQueryType = binReader.ReadInt32();
                if (Enum.IsDefined(typeof(ReportQueryType), tmpQueryType))
                    reportquerytype = (ReportQueryType)tmpQueryType;
            }
            return reportquerytype;
        }

        /// <summary>
        /// 读取并生成指定报表文件节点对应的报表文件独享
        /// </summary>
        /// <param name="reportNode">待读取的报表文件节点</param>
        /// <returns>获取的报表文件对像</returns>
        public CommonReportFile ReadReportFile(ReportNode reportNode)
        {
            MemoryStream reportfileStream = new MemoryStream();
            reportfileStream.SetLength(0);

            DataTable resultTable = new DataTable();
            SetupTableProvider.Instance.ReadSetupNodesByNodeTypeID(0, reportNode.NodeType, reportNode.NodeID, false, 1, true, ref resultTable);

            if (resultTable.Rows.Count == 1)
            {
                DataRow dr = resultTable.Rows[0];
                // image类型数据转换为byte[]存储
                if (!Convert.IsDBNull(dr["Data"]))
                {
                    byte[] bytes = (byte[])dr["Data"];
                    // 基于结果数据创建结果数据流对像
                    reportfileStream.Position = 0;
                    reportfileStream.Write(bytes, 0, bytes.Length);
                }
            }

            CommonReportFile reportfile = null;
            if (reportfileStream != null && reportfileStream.Length > 0)
            {
                reportfile = new CommonReportFile(reportNode, reportNode.NodeType);
                reportfile.LoadReportFromStream(reportfileStream);
                // 关闭流对像
                reportfileStream.Close();
            }

            return reportfile;

        }

        /// <summary>
        /// 指定报表文件的类型和名称获取对应的报表节点ID
        /// </summary>
        /// <param name="reportType">报表节点类型</param>
        /// <param name="reportName">报表节点名称</param>
        /// <param name="saveParentNodeID">父节点ID</param>
        /// <returns></returns>
        public uint GetReportNodeID(uint reportType, string reportName, uint saveParentNodeID)
        {
            uint parentNodeType = ReportNodeType.REPORTGROUP;
            if (saveParentNodeID == ReportNodeType.REPORTDEPOT)
            {
                parentNodeType = ReportNodeType.REPORTDEPOT;
            }
            uint tarReportID = 0;
            int res = SetupTableProvider.Instance.GetSetupNodeID(0, parentNodeType, saveParentNodeID, reportType, reportName, ref tarReportID);

            return tarReportID;
        }

        /// <summary>
        /// 指定报表文件类型，获取当前报表文件的总数
        /// </summary>
        /// <param name="reportType">报表文件类型</param>
        /// <returns></returns>
        public int GetTotalReportFileNum(uint reportType)
        {
            int result = 0;
            if (reportDepository == null)
                return result;

            result = GetReportNodeCount(reportDepository, reportType);
            return result;
        }

        /// <summary>
        /// 重载版本，获取Pecstar类型报表文件
        /// </summary>
        /// <returns></returns>
        public int GetTotalReportFileNum()
        {
            return GetTotalReportFileNum(ReportNodeType.REPORTPECSTAR);
        }

        /// <summary>
        /// 递归获取指定节点下的报表文件节点的个数
        /// </summary>
        /// <param name="rootNode">当前节点</param>
        /// <param name="reportType">节点类型</param>
        /// <returns></returns>
        private int GetReportNodeCount(ReportNode rootNode, uint reportType)
        {
            int result = 0;
            if (rootNode.NodeType == reportType)
                result = 1;
            else
            {
                for (int i = 0; i < rootNode.ChildrenCount; i++)
                {
                    ReportNode subNode = rootNode.Find(i) as ReportNode;
                    if (subNode != null)
                        result += GetReportNodeCount(subNode, reportType);
                }
            }

            return result;
        }

        /// <summary>
        /// 根据指定的报表文件ID，查找对应的报表文件节点
        /// </summary>
        /// <param name="targetReportID">指定的报表文件ID</param>
        /// <returns></returns>
        public ReportNode FindTargetReportNode(uint targetReportID)
        {
            if (targetReportID > 0)
                return FindReportNode(RootReportNode, targetReportID);
            else
                return null;

        }

        /// <summary>
        /// 采用递归方式获取指定ID的报表节点
        /// </summary>
        /// <param name="rootNode">当前遍历的报表节点</param>
        /// <param name="targetReportID">指定报表ID</param>
        /// <returns></returns>
        private ReportNode FindReportNode(ReportNode rootNode, uint targetReportID)
        {
            if (rootNode.NodeID == targetReportID)
            {
                if (rootNode.NodeType != ReportNodeType.REPORTDEPOT && rootNode.NodeType != ReportNodeType.REPORTGROUP)
                    return rootNode;
            }
            else
            {
                for (int i = 0; i < rootNode.ChildrenCount; i++)
                {
                    ReportNode subNode = rootNode.Find(i) as ReportNode;
                    if (subNode != null)
                    {
                        // 直接点继续查找
                        ReportNode tmpNode = FindReportNode(subNode, targetReportID);
                        // 找到目标节点，则返回
                        if (tmpNode != null)
                            return tmpNode;
                    }
                }
            }
            return null;
        }
    }
}
