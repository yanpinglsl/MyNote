
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using CET.PecsNodeManage;
using DBInterfaceCommonLib;
using OfficeReportInterface.DefaultReportInterface.PowerQualityEventsOnly;
using System.Text;
using OfficeReportInterface.DefaultReportInterface.EnergyCost;
using CSharpDBPlugin;

namespace OfficeReportInterface
{
  

    /// <summary>
    /// 唯一标识指定节点波形通道参数的数据结构
    /// </summary>
    public struct DeviceWaveIDDef
    {
        /// <summary>
        /// 监测节点ID
        /// </summary>
        public uint DeviceID;

        /// <summary>
        /// 波形通道参数ID
        /// </summary>
        public uint WaveChannelID;

        /// <summary>
        /// Initializes a new instance of the PQWaveIDDef struct
        /// </summary>
        /// <param name="pqNodeID">监测节点ID</param>
        /// <param name="waveID">波形通道ID</param>
        public DeviceWaveIDDef(uint deviceID, uint waveID)
        {
            this.DeviceID = deviceID;
            this.WaveChannelID = waveID;
        }
    }

    /// <summary>
    /// PQWebService的后台公用管理类
    /// 主要用于节点初始化、节点映射缓存等操作
    /// </summary>
    public class ReportWebServiceManager
    {
        /// <summary>
        /// 全局唯一单例
        /// </summary>
        public static readonly ReportWebServiceManager ReportWebManager = new ReportWebServiceManager();

        /// <summary>
        /// 当前Web应用程序的绝对物理路径
        /// </summary>
        private static string webAppRootPath = string.Empty;

        /// <summary>
        /// 数据库连接文件路径
        /// </summary>
        private static string curDBSourceInfo = string.Empty;

        /// <summary>
        /// 用于缓存PQ监测节点-DataID-DataType至实时测点-定时记录SourceID的映射关系集合
        /// </summary>
        private Dictionary<DeviceDataIDDef, DataIDToMeasIDDef> deviceDataIDToMeasIDMap = new Dictionary<DeviceDataIDDef, DataIDToMeasIDDef>();

        /// <summary>
        /// 获取系统配置的参数dataid-dataName映射表
        /// </summary>
        private List<DataIDDef> dataIDDefList;

        ///// <summary>
        ///// 获取系统所有设备信息
        ///// </summary>
        //private List<PecsDeviceNode> deviceNodeList;

        /// <summary>
        /// 事件等级
        /// </summary>
        private Dictionary<int, string> eventClassIDNameMap;

        /// <summary>
        /// 事件类型
        /// </summary>
        private Dictionary<int, string> eventTypeIDNameMap;

        /// <summary>
        /// 用于缓存PQ监测节点-WaveChannelID至波形Cfg文件通道名称的映射关系集合
        /// </summary>
        private Dictionary<DeviceWaveIDDef, string> pqWaveIdToCfgNameMap = new Dictionary<DeviceWaveIDDef, string>();

        ////全局公用TOU方案
        //public static List<StationTOUProfile> stationTOUProfileList = new List<StationTOUProfile>();

        #region 成员对象
        /// <summary>
        /// 是否初始化成功
        /// </summary>
        private bool initialSuccess;

        #endregion

        /// <summary>
        /// Initializes a new instance of the ReportWebServiceManager class
        /// </summary>
        private ReportWebServiceManager()
        {
        }

        /// <summary>
        /// 获取日报测点起始ID和所在列
        /// </summary>
        /// <param name="rptMeasID"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static uint GetDayMeasStartIDByMeasID(uint rptMeasID, ref int valueIndex)
        {
            uint dayMeasStartID = 0;
            valueIndex = Convert.ToInt32(rptMeasID % 16);
            if (valueIndex == 0)
            {
                dayMeasStartID = (rptMeasID / 16 - 1) * 16 + 1;
                valueIndex = 16;
            }
            else
            {
                dayMeasStartID = (rptMeasID / 16) * 16 + 1;
            }
            return dayMeasStartID;
        }

        #region 公共成员属性
        /// <summary>
        /// Gets or sets 当前Web应用程序的绝对物理路径
        /// </summary>
        public static string WebAppRootPath
        {
            get
            {
                return webAppRootPath;
            }

            set
            {
                webAppRootPath = value;
            }
        }

        /// <summary>
        /// Gets or sets 数据库连接文件路径
        /// </summary>
        public static string CurDBSourceInfo
        {
            get
            {
                return curDBSourceInfo;
            }

            set
            {
                curDBSourceInfo = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether 初始化是否成功
        /// </summary>
        public bool InitialSuccess
        {
            get { return this.initialSuccess; }
        }

        ///// <summary>
        ///// Gets 获取系统配置的参数dataid-dataName映射表
        ///// </summary>
        //public List<DataIDDef> DataIDDefList
        //{
        //    get { return this.dataIDDefList; }
        //}

        ///// <summary>
        ///// Gets 设备节点列表
        ///// </summary>
        //public List<PecsDeviceNode> DeviceNodeList
        //{
        //    get { return this.deviceNodeList; }
        //}

        /// <summary>
        /// Gets 事件等级ID与名称对照表
        /// </summary>
        public Dictionary<int, string> EventClassIDNameMap
        {
            get { return this.eventClassIDNameMap; }
        }

        /// <summary>
        /// Gets 事件类型ID与名称对照表
        /// </summary>
        public Dictionary<int, string> EventTypeIDNameMap
        {
            get { return this.eventTypeIDNameMap; }
        }

        #endregion

        #region 公共成员方法
        /// <summary>
        /// WebService后台的全局初始化函数
        /// 初始化数据库连接，加载节点以及解析节点映射关系等
        /// </summary>
        /// <returns>初始化结果</returns>
        public ErrorMsg InitializeWebService()
        {
            ErrorMsg resultMsg = new ErrorMsg(true);

            //初始化Web所引用的数据库接口的连接信息初始化
            resultMsg = this.InitializeDBConnetionData();
            if (!resultMsg.IsSuccess)
            {
                return resultMsg;
            }
            //首先初始化系统常量定义
            SysConstDefinition.InitializeSysConstDefinition();
            if (!PecsNodeManager.PecsNodeInstance.InitSuccess)
            {
                DbgTrace.dout("初始化PecsNodeManager.");
                DateTime initStartTime = DateTime.Now;
                //初始化PecsStar系统节点管理DLL并加载系统通信设置节点
                //resultMsg = PecsNodeManager.PecsNodeInstance.InitializeNodeManager(0);//初始化PECSDBInterface.DBModuleInterface.Instance.Initialize
                //DbgTrace.dout("PecsNodeManager.PecsNodeInstance.InitializeNodeManager耗时:" + (DateTime.Now - initStartTime).TotalSeconds + "秒");
                //if (!resultMsg.IsSuccess)
                //{
                //    return resultMsg;
                //}
                initStartTime = DateTime.Now;
                resultMsg = PecsNodeManager.PecsNodeInstance.InitialConfigNodes(true);
                DbgTrace.dout("PecsNodeManager.PecsNodeInstance.InitialConfigNodes耗时:" + (DateTime.Now - initStartTime).TotalSeconds + "秒");
                if (!resultMsg.IsSuccess)
                {
                    return resultMsg;
                }
                //initStartTime = DateTime.Now;
                ////初始化TOU方案
                //for (int i = 0; i < SystemMeasPointGNode.SystemConfigNode.ChildrenCount; i++)
                //{
                //    try
                //    {
                //        stationTOUProfileList.Add(ParseTOUByStation(SystemMeasPointGNode.SystemConfigNode.Find(i)));
                //    }
                //    catch (Exception exc)
                //    {
                //        resultMsg.ErrorMessage = exc.Message;
                //    }
                //}
                //DbgTrace.dout("初始化TOU方案耗时:" + (DateTime.Now - initStartTime).TotalSeconds + "秒");
                initStartTime = DateTime.Now;

                resultMsg = PecsNodeManager.PecsNodeInstance.InitialStationNode();
                DbgTrace.dout("初始化加载定时记录配置分组对应关系耗时:" + (DateTime.Now - initStartTime).TotalSeconds + "秒");
                initStartTime = DateTime.Now;
                //初始化加载定时记录映射和波形映射关系
                if (resultMsg.IsSuccess)
                {
                    EMSWebServiceManager.EMSWebManager.InitializeWebService();
                    //初始化事件等级和类型
                    this.InitEventClass();
                    this.InitEventType();
                    //加载DataID-DataName对应
                    this.dataIDDefList = PecsNodeManager.PecsNodeInstance.GetMappedDataIDListByDataType(0, 0);

                    UnitConfigManager.DataManager.Initialize();//PQreport用到

                    ////初始化TOU方案
                    //for (int i = 0; i < SystemMeasPointGNode.SystemConfigNode.ChildrenCount; i++)
                    //{
                    //    stationTOUProfileList.Add(ParseTOUByStation(SystemMeasPointGNode.SystemConfigNode.Find(i)));
                    //}
                    DbgTrace.dout("初始化加载定时记录映射和波形映射关系耗时:" + (DateTime.Now - initStartTime).TotalSeconds + "秒");
                }
            }

            //初始化成功
            this.initialSuccess = resultMsg.IsSuccess;
            return resultMsg;
        }

        /// <summary>
        /// 根据指定的参数对象快速找到映射关系结果
        /// </summary>
        /// <param name="param">参数对象</param>
        /// <param name="resultMap">映射结果对象</param>
        /// <returns>是否查到结果</returns>
        public bool FindDataMapDef(DeviceDataIDDef param, out DATALOG_PRIVATE_MAP resultMap)
        {
            //bool result = false;
            //resultMap = DataIDToMeasIDDef.InvalidMapDef;

            ////从缓存字典中快速查找定位
            //result = this.deviceDataIDToMeasIDMap.TryGetValue(param, out resultMap) ;    这里保留之前的代码做对照。改为使用Redis缓存，由于这里key的结构不一样，多了nodeName等内容，为了复用代码，做了key结构的转换
            //return result;
           return EMSWebServiceManager.EMSWebManager.FindDataMapDef(new DeviceDataParam(param.DeviceID, param.DataID, (uint) param.DataTypeID, param.LogicalDeviceIndex, param.ParaTypeID), out resultMap);
        }

        /// <summary>
        /// 根据dataid获取电能质量参数的paraName
        /// </summary>
        /// <param name="dataID">参数ID</param>
        /// <returns>参数名称</returns>
        public string GetParaNameByDataID(uint dataID)
        {
            for (int i = 0; i < PecsNodeManager.PecsNodeInstance.DataIDNum; i++)
            {
                DataIDDef dataIDDef = PecsNodeManager.PecsNodeInstance.GetDataIDDef(i);
                if (dataIDDef.DataID == dataID)
                {
                    return dataIDDef.DataName;
                }
            }

            return string.Empty;
        }

        ///// <summary>
        ///// 返回所有参数ID和名称列表
        ///// </summary>
        ///// <returns>结果集</returns>
        //public DataTable GetDataIDNameData()
        //{
        //    DataTable dataIDNameDT = new DataTable("DataIDNameDT");

        //    DataColumn column;
        //    DataRow row;

        //    column = new DataColumn();
        //    column.DataType = System.Type.GetType("System.Int32");
        //    column.ColumnName = "DataID";
        //    column.Unique = true;
        //    column.ReadOnly = true;
        //    dataIDNameDT.Columns.Add(column);

        //    column = new DataColumn();
        //    column.DataType = System.Type.GetType("System.String");
        //    column.ColumnName = "DataName";
        //    dataIDNameDT.Columns.Add(column);

        //    for (int i = 0; i < this.dataIDDefList.Count; i++)
        //    {
        //        row = dataIDNameDT.NewRow();
        //        row["DataID"] = this.dataIDDefList[i].DataID;
        //        row["DataName"] = this.dataIDDefList[i].DataName;
        //        dataIDNameDT.Rows.Add(row);
        //    }

        //    return dataIDNameDT;
        //}

        ///// <summary>
        ///// 返回所有设备ID和名称列表
        ///// </summary>
        ///// <returns>结果集</returns>
        //public DataTable GetAllDeviceIDNameData()
        //{
        //    DataTable deviceIDNameDT = new DataTable("DeviceIDNameDT");

        //    DataColumn column;
        //    DataRow row;

        //    column = new DataColumn();
        //    column.DataType = System.Type.GetType("System.Int32");
        //    column.ColumnName = "DeviceID";
        //    column.Unique = true;
        //    column.ReadOnly = true;
        //    deviceIDNameDT.Columns.Add(column);

        //    column = new DataColumn();
        //    column.DataType = System.Type.GetType("System.String");
        //    column.ColumnName = "DeviceName";
        //    deviceIDNameDT.Columns.Add(column);

        //    for (int i = 0; i < this.deviceNodeList.Count; i++)
        //    {
        //        row = deviceIDNameDT.NewRow();
        //        row["DeviceID"] = this.deviceNodeList[i].NodeID;
        //        row["DeviceName"] = this.deviceNodeList[i].NodeName;
        //        deviceIDNameDT.Rows.Add(row);
        //    }

        //    return deviceIDNameDT;
        //}

        /// <summary>
        /// 返回所有厂站ID和名称列表
        /// </summary>
        /// <returns>结果集</returns>
        public DataTable GetStationIDNameData()
        {
            DataTable stationIDNameDT = new DataTable("StationIDNameDT");

            DataColumn column;
            DataRow row;

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "StationID";
            column.Unique = true;
            column.ReadOnly = true;
            stationIDNameDT.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "StationName";
            stationIDNameDT.Columns.Add(column);

            for (int i = 0; i < PecsNodeManager.PecsNodeInstance.StationNum; i++)
            {
                row = stationIDNameDT.NewRow();
                row["StationID"] = PecsNodeManager.PecsNodeInstance.GetStationID(i);
                uint stationID = PecsNodeManager.PecsNodeInstance.GetStationID(i);
                row["StationName"] = PecsNodeManager.PecsNodeInstance.GetStationName(stationID);
                stationIDNameDT.Rows.Add(row);
            }

            return stationIDNameDT;
        }

        /// <summary>
        /// 更加厂站ID获取通道ID名称列表
        /// </summary>
        /// <param name="stationIDs">厂站ID</param>
        /// <returns>结果集</returns>
        public DataTable GetChannelIDNameByStations(string stationIDs)
        {
            DataTable channelIDNameDT = new DataTable("ChannelIDNameDT");
            List<uint> stationIDList = DataFormatManager.ParseUIntList(stationIDs, ";");
            DataColumn column;
            DataRow row;

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "ChannelID";
            column.Unique = true;
            column.ReadOnly = true;
            channelIDNameDT.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "ChannelName";
            channelIDNameDT.Columns.Add(column);

            for (int i = 0; i < stationIDList.Count; i++)
            {
                PecsStationNode stationNode = SystemMeasPointGNode.SystemConfigNode.Find(SysNodeType.PECSSTATION_NODE, stationIDList[i]) as PecsStationNode;
                SysNode communicationNode = stationNode.Find(SysNodeType.PECSCOMMUNI_NODE, stationNode.NodeID);

                for (int j = 0; j < communicationNode.ChildrenCount; j++)
                {
                    SysNode channelNode = communicationNode.Find(j);
                    if (channelNode.NodeType == SysNodeType.PECSCHANNEL_NODE)
                    {
                        row = channelIDNameDT.NewRow();
                        row["ChannelID"] = channelNode.NodeID;
                        row["ChannelName"] = channelNode.NodeName;
                        channelIDNameDT.Rows.Add(row);
                    }
                }
            }

            return channelIDNameDT;
        }

        /// <summary>
        /// 根据厂站通道获取设备节点
        /// </summary>
        /// <param name="stationIDs">厂站ID</param>
        /// <param name="channelIDs">通道ID</param>
        /// <returns>结果集</returns>
        public DataTable GetDeviceIDNameByStationsAndChannels(string stationIDs, string channelIDs)
        {
            DataTable deviceIDNameDT = new DataTable("DeviceIDNameDT");
            List<uint> stationIDList = DataFormatManager.ParseUIntList(stationIDs, ";");
            List<uint> channelIDList = DataFormatManager.ParseUIntList(channelIDs, ";");
            DataColumn column;
            DataRow row;

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "DeviceID";
            column.Unique = true;
            column.ReadOnly = true;
            deviceIDNameDT.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "DeviceName";
            deviceIDNameDT.Columns.Add(column);

            for (int i = 0; i < stationIDList.Count; i++)
            {
                PecsStationNode stationNode = SystemMeasPointGNode.SystemConfigNode.Find(SysNodeType.PECSSTATION_NODE, stationIDList[i]) as PecsStationNode;
                SysNode communicationNode = stationNode.Find(SysNodeType.PECSCOMMUNI_NODE, stationNode.NodeID);

                for (int j = 0; j < channelIDList.Count; j++)
                {
                    SysNode channelNode = communicationNode.Find(SysNodeType.PECSCHANNEL_NODE, channelIDList[j]);
                    if (channelNode.NodeType == SysNodeType.PECSCHANNEL_NODE)
                    {
                        for (int l = 0; l < channelNode.ChildrenCount; l++)
                        {
                            PecsDeviceNode deviceNode = channelNode.Find(l) as PecsDeviceNode;
                            row = deviceIDNameDT.NewRow();
                            row["DeviceID"] = deviceNode.NodeID;
                            row["DeviceName"] = deviceNode.NodeName;
                            deviceIDNameDT.Rows.Add(row);
                        }
                    }
                }
            }

            return deviceIDNameDT;
        }

        ///// <summary>
        ///// 根据指定的波形参数对象快速找到波形cfg文件中对应的名称
        ///// </summary>
        ///// <param name="param">波形参数对象</param>
        ///// <param name="resultName">Cfg名称</param>
        ///// <returns>是否找到</returns>
        //public bool FindWaveChannelMapDef(DeviceWaveIDDef param, out string resultName)
        //{
        //    bool result = false;
        //    resultName = string.Empty;

        //    //从缓存字典中快速查找定位
        //    result = this.pqWaveIdToCfgNameMap.TryGetValue(param, out resultName);

        //    return result;
        //}

        #endregion

        #region 私有成员方法
        public ErrorMsg InitiaPluginDll()
        {
            IDBResourceFactory factory = DBInterfaceCommonLib.DBReasourceFactory.Instance();
            IGlobalInformation globalInfo = DBInterfaceCommonLib.GlobalInformation.Instance();
            PostgreSQLDBConnectionManagerPlugin.Plugin plugin1 = new PostgreSQLDBConnectionManagerPlugin.Plugin();
            plugin1.Register(factory, globalInfo);
            SQLDBConnectionManagerPlugin.Plugin plugin2 = new SQLDBConnectionManagerPlugin.Plugin();
            plugin2.Register(factory, globalInfo);
            //PostgreSQLEMSDBInterfacePlugin.Plugin plugin3 = new PostgreSQLEMSDBInterfacePlugin.Plugin();
            //plugin3.Register(factory, globalInfo);
            //SQLEMSDBInterfacePlugin.Plugin plugin4 = new SQLEMSDBInterfacePlugin.Plugin();
            //plugin4.Register(factory, globalInfo);

            ErrorMsg resultMsg = new ErrorMsg(true);
            int resultCode = DBPlugin.Instance.Initialize();//CurDBSourceInfo
            DBPlugin.Instance.SetDBConnPath(CurDBSourceInfo);
            if (resultCode != (int)CSharpDBPlugin.ErrorCode.Success)
            {
                resultMsg.IsSuccess = false;
                resultMsg.ErrorMsgInstance = "InitializePECSDBConnetion";
                resultMsg.ErrorMessage = CSharpDBPlugin.ErrorQuerier.Instance.GetLastErrorString();
                return resultMsg;
            }
            return resultMsg;
            // 2022/08/01 修改
            //PostgreSQLPECSDBInterfacePlugin.Plugin plugin5 = new PostgreSQLPECSDBInterfacePlugin.Plugin();
            //plugin5.Register(factory, globalInfo);
            //SQLPECSDBInterfacePlugin.Plugin plugin6 = new SQLPECSDBInterfacePlugin.Plugin();
            //plugin6.Register(factory, globalInfo);
        }
        /// <summary>
        /// 初始化数据库连接信息
        /// </summary>
        /// <returns>错误信息</returns>
        private ErrorMsg InitializeDBConnetionData()
        {
            ErrorMsg resultMsg = new ErrorMsg(true);
            resultMsg = InitiaPluginDll();
            if (!resultMsg.IsSuccess)
            {
                return resultMsg;
            }

            uint maxPoolSize = 20;
            int resultCode = DBServerInfoProvider.Instance.SetMaxPoolSize(maxPoolSize);
            if (resultCode != (int)CSharpDBPlugin.ErrorCode.Success)
            {
                resultMsg.IsSuccess = false;
                resultMsg.ErrorMsgInstance = "InitializeSetMaxPoolSize";
                resultMsg.ErrorMessage = CSharpDBPlugin.ErrorQuerier.Instance.GetLastErrorString();
                return resultMsg;
            }
            return resultMsg;
        }

        /// <summary>
        /// 加载和初始化PQ监测节点-电量参数-参数类型与设备测点ID以及定时记录SourceID的对应关系
        /// </summary>
        /// <param name="parentNode">父节点</param>
        private void LoadDataIDToMeasureIDMap(SysNode parentNode)
        {
            //遍历所有的监测节点
            for (int i = 0; i < parentNode.ChildrenCount; i++)
            {
                SysNode subNode = parentNode.Find(i);
                switch (subNode.NodeType)
                {
                    case PecsNodeType.PECSCOMMUNI_NODE:
                    case PecsNodeType.PECSCHANNEL_NODE:
                    case PecsNodeType.PECSSTATION_NODE:
                        //递归加载解析子节点
                        this.LoadDataIDToMeasureIDMap(subNode);
                        break;
                    case PecsNodeType.PECSDEVICE_NODE:
                        PecsDeviceNode subPecsNode = subNode as PecsDeviceNode;
                        ////加载系统所有设备信息
                        //if (this.deviceNodeList == null)
                        //{
                        //    this.deviceNodeList = new List<PecsDeviceNode>();
                        //}

                        //this.deviceNodeList.Add(subPecsNode);
                        this.ExecuteDataIDMapByNode(subPecsNode);
                        //this.ExecuteWaveChannelIDMapByNode(subPecsNode);
                        break;
                }
            }
        }

        /// <summary>
        /// 为指定的设备节点进行DataID映射解析
        /// </summary>
        /// <param name="curDevNode">PQ节点对象</param>
        private void ExecuteDataIDMapByNode(PecsDeviceNode curDevNode)
        {
            if (curDevNode != null)
            {
                //获取当前设备的厂站通道ID
                uint channelID = curDevNode.ParentNode.NodeID;
                uint stationID = curDevNode.ParentNode.ParentNodeID;

                //直接查询获取当前定时记录映射方案对应的所有映射记录列表 待确认
                //var mapDataHandleList = PecsNodeManager.PecsNodeInstance.GetDataLogPrivateMapByDeviceID(curDevNode.NodeID);
                var mapDataHandleList = PecsNodeManager.PecsNodeInstance.GetMappedDataHandleListByDeviceID(curDevNode.NodeID);
                foreach (var curDataHandle in mapDataHandleList)
                {
                    //reporting service只对回路1参数有效
                    //if (curDataHandle.logicalDeviceIndex != 1)
                    //    continue;
                    //只针对定时记录映射进行过滤
                    DeviceDataIDDef curPQParam = new DeviceDataIDDef(curDevNode.NodeType,curDevNode.NodeID, curDataHandle.DataID, (int)curDataHandle.ParaTypeID, (int)curDataHandle.DataTypeID, (int)curDataHandle.logicalDeviceIndex);
                    DataIDToMeasIDDef resultMapDef = DataIDToMeasIDDef.InvalidMapDef;
                    //执行参数映射解析
                    bool result = this.GetDataIDMapResultWithParaHandle(stationID, channelID, curDevNode.NodeID, (int)curDataHandle.ParaHandle, curDataHandle.Cofficient, out resultMapDef);
                    if (result)
                    {
                        this.deviceDataIDToMeasIDMap[curPQParam] = resultMapDef;
                    }
                }
            }
        }

        ///// <summary>
        ///// 为指定的PQ监测节点进行波形通道映射解析
        ///// </summary>
        ///// <param name="pqNode">PQ节点对象</param>
        //private void ExecuteWaveChannelIDMapByNode(PecsDeviceNode deviceNode)
        //{
        //    //获取当前设备的波形映射方案ID
        //    int waveMapID = deviceNode.WaveMapID;

        //    //遍历所有的WaveChannelID参数，创建PQWaveIDDef对象
        //    for (int i = 0; i < PecsNodeManager.PecsNodeInstance.WaveIDNum; i++)
        //    {
        //        DataIDDef curParam = PecsNodeManager.PecsNodeInstance.GetWaveChannelDef(i);
        //        //创建PQ波形参数对象
        //        DeviceWaveIDDef curPQParam = new DeviceWaveIDDef(deviceNode.NodeID, curParam.DataID);
        //        //根据映射方案和波形参数获取对应的波形Cfg名称
        //        string resultCfgName = PecsNodeManager.PecsNodeInstance.GetWaveMapResult(waveMapID, curParam.DataID);
        //        if (!string.IsNullOrEmpty(resultCfgName))
        //        {
        //            this.pqWaveIdToCfgNameMap[curPQParam] = resultCfgName;
        //        }
        //    }
        //}

        /// <summary>
        /// 基于参数ID对象获取对应测点及定时记录映射关系结果
        /// </summary>
        /// <param name="stationID">厂站ID</param>
        /// <param name="channelID">通道ID</param>
        /// <param name="deviceID">设备ID</param>
        /// <param name="paraHandle">参数句柄</param>
        /// <param name="cofficient">参数系数</param>
        /// <param name="resultDef">映射结果</param>
        /// <returns>是否成功</returns>
        private bool GetDataIDMapResultWithParaHandle(uint stationID, uint channelID, uint deviceID, int paraHandle, double cofficient, out DataIDToMeasIDDef resultDef)
        {
            bool result = false;
            resultDef = DataIDToMeasIDDef.InvalidMapDef;
            resultDef.StationID = stationID;
            resultDef.Cofficient = cofficient;

            if (paraHandle >= 0)
            {
                //根据句柄参数获取定时记录的SourceID和DataIndex
                //DataSourceDef resDataSource = PecsNodeManager.PecsNodeInstance.GetDataLogSourceAndIndex(stationID, deviceID, paraHandle);
                //if (!DataSourceDef.InvalidDataSource.Equals(resDataSource))
                //{
                //    resultDef.SourceID = resDataSource.SourceID;
                //    resultDef.DataIndex = resDataSource.DataIndex;
                //    result = true;
                //}
            }

            return result;
        }

        /// <summary>
        /// 初始化事件等级
        /// </summary>
        private void InitEventClass()
        {
            this.eventClassIDNameMap = new Dictionary<int, string>();
            this.eventClassIDNameMap.Add(-1, LocalResourceManager.GetInstance().GetString("0152", "All Class"));
            this.eventClassIDNameMap.Add(0, LocalResourceManager.GetInstance().GetString("0153", "Others"));
            this.eventClassIDNameMap.Add(1, LocalResourceManager.GetInstance().GetString("0154", "Fault"));
            this.eventClassIDNameMap.Add(2, LocalResourceManager.GetInstance().GetString("0155", "Alarm"));
            this.eventClassIDNameMap.Add(3, LocalResourceManager.GetInstance().GetString("0156", "Normal"));
        }

        /// <summary>
        /// 初始化事件类型
        /// </summary>
        private void InitEventType()
        {
            //初始化事件类型            
            this.eventTypeIDNameMap = new Dictionary<int, string>();
            this.eventTypeIDNameMap.Add(-1, LocalResourceManager.GetInstance().GetString("0122", "All Type"));
            this.eventTypeIDNameMap.Add(1, LocalResourceManager.GetInstance().GetString("0123", "Devices Exceed Limit"));
            this.eventTypeIDNameMap.Add(2, LocalResourceManager.GetInstance().GetString("0124", "Devices Exceed Limit Return"));
            this.eventTypeIDNameMap.Add(3, LocalResourceManager.GetInstance().GetString("0125", "Digital In ON"));
            this.eventTypeIDNameMap.Add(4, LocalResourceManager.GetInstance().GetString("0126", "Digital In OFF"));
            this.eventTypeIDNameMap.Add(5, LocalResourceManager.GetInstance().GetString("0127", "Soft Record Exceed Limit"));
            this.eventTypeIDNameMap.Add(6, LocalResourceManager.GetInstance().GetString("0128", "Soft Record Exceed Limit Return"));
            this.eventTypeIDNameMap.Add(7, LocalResourceManager.GetInstance().GetString("0129", "Device Self Check"));
            this.eventTypeIDNameMap.Add(8, LocalResourceManager.GetInstance().GetString("0130", "Control and Adjust"));
            this.eventTypeIDNameMap.Add(9, LocalResourceManager.GetInstance().GetString("0131", "System Config/Parameter"));
            this.eventTypeIDNameMap.Add(10, LocalResourceManager.GetInstance().GetString("0132", "Device Trigger"));
            this.eventTypeIDNameMap.Add(11, LocalResourceManager.GetInstance().GetString("0133", "Device Start/Quit"));
            this.eventTypeIDNameMap.Add(12, LocalResourceManager.GetInstance().GetString("0134", "Site Start/Quit"));
            this.eventTypeIDNameMap.Add(13, LocalResourceManager.GetInstance().GetString("0135", "Communication Station Start/Quit"));
            this.eventTypeIDNameMap.Add(14, LocalResourceManager.GetInstance().GetString("0136", "Edit Data"));
            this.eventTypeIDNameMap.Add(15, LocalResourceManager.GetInstance().GetString("0137", "Communication Fail and Resume"));
            this.eventTypeIDNameMap.Add(16, LocalResourceManager.GetInstance().GetString("0138", "Max./Min. SetPoint"));
            this.eventTypeIDNameMap.Add(17, LocalResourceManager.GetInstance().GetString("0139", "Transient"));
            this.eventTypeIDNameMap.Add(18, LocalResourceManager.GetInstance().GetString("0140", "Voltage Variation"));
            this.eventTypeIDNameMap.Add(19, LocalResourceManager.GetInstance().GetString("0141", "Voltage Variation Fault Judged"));
            this.eventTypeIDNameMap.Add(20, LocalResourceManager.GetInstance().GetString("0142", "SEL Event"));
            this.eventTypeIDNameMap.Add(21, LocalResourceManager.GetInstance().GetString("0143", "Communication Events"));
            this.eventTypeIDNameMap.Add(22, LocalResourceManager.GetInstance().GetString("0144", "Database Events"));
            this.eventTypeIDNameMap.Add(23, LocalResourceManager.GetInstance().GetString("0145", "PQDIF File Events"));
            this.eventTypeIDNameMap.Add(24, LocalResourceManager.GetInstance().GetString("0146", "PQDIF Command Events"));
            this.eventTypeIDNameMap.Add(25, LocalResourceManager.GetInstance().GetString("0147", "Communication Port Related"));
            this.eventTypeIDNameMap.Add(26, LocalResourceManager.GetInstance().GetString("0148", "Command process (transfered by Dserver)"));
            this.eventTypeIDNameMap.Add(27, LocalResourceManager.GetInstance().GetString("0149", "Load dll Events"));
            this.eventTypeIDNameMap.Add(28, LocalResourceManager.GetInstance().GetString("0150", "Supplementary Data Finished"));
            this.eventTypeIDNameMap.Add(29, LocalResourceManager.GetInstance().GetString("0151", "Device Event(Recorded by soft)"));
        }


        #region TOU方案解析函数

        /// <summary>
        /// 每个厂站都有TOU方案，遍历厂站查找TOU方案
        /// </summary>
        /// <param name="parentNode">父节点</param>
        private StationTOUProfile ParseTOUByStation(SysNode subNode)
        {
            //加载TOU相关节点
            uint[] nodeTypes = new uint[] { SysConstDefinition.NODETYPE_YEARPROFILE, SysConstDefinition.NODETYPE_DAYPROFILE, SysConstDefinition.NODETYPE_TRAFF };
            DataTable resultTable = new DataTable();
            SetupTableProvider.Instance.ReadSetupNodesByParentNodeTypeID(0,subNode.NodeType, subNode.NodeID, nodeTypes, 0, true, ref resultTable);
            StationTOUProfile stationTouPrifile = new StationTOUProfile();
            stationTouPrifile.stationID = subNode.NodeID;
            for (int j = 0; j < resultTable.Rows.Count; j++)
            {
                DataRow dr = resultTable.Rows[j];
                //解析年费率方案
                if (Convert.ToUInt32(dr["nodetype"]) == SysConstDefinition.NODETYPE_YEARPROFILE)
                {
                    if (Convert.IsDBNull(dr["Data"]))
                    {
                        continue;
                    }
                    byte[] bytes = (byte[])dr["Data"];
                    //加载年TOU方案
                    ParseYearTOUProfile(bytes, stationTouPrifile);
                }
                //解析日费率方案
                else if (Convert.ToUInt32(dr["nodetype"]) == SysConstDefinition.NODETYPE_DAYPROFILE)
                {
                    if (Convert.IsDBNull(dr["Data"]))
                    {
                        continue;
                    }
                    //加载日TOU方案
                    ParseDayTOUProfile(dr, stationTouPrifile);
                }
                else if (Convert.ToUInt32(dr["nodetype"]) == SysConstDefinition.NODETYPE_TRAFF)//费率段方案
                {
                    ParseTariffTOUProfile(stationTouPrifile, dr);
                }
            }
            return stationTouPrifile;
        }

        private void ParseTariffTOUProfile(StationTOUProfile stationTouPrifile, DataRow dr)
        {
            TariffProfileStruct tariffProfile = new TariffProfileStruct();
            tariffProfile.tariffName = dr["nodeName"].ToString();
            tariffProfile.tariffIndex = Convert.ToInt32(dr["nodeid"]);
            if (Convert.IsDBNull(dr["Data"]))
            {
                return;
            }
            byte[] bytes = (byte[])dr["Data"];
            MemoryStream userAuthorityStream = new MemoryStream();
            userAuthorityStream.Write(bytes, 0, bytes.Length);
            BinaryReader binReader = new BinaryReader(userAuthorityStream);
            userAuthorityStream.Position = 0;
            tariffProfile.tariffUnit = Encoding.Default.GetString(binReader.ReadBytes(10)).Trim().Replace("\0", "");

            tariffProfile.kWhTariff = binReader.ReadDouble();
            tariffProfile.kvarhTariff = binReader.ReadDouble();
            tariffProfile.kVAhTariff = binReader.ReadDouble();
            tariffProfile.kWDemandTariff = binReader.ReadDouble();
            tariffProfile.kvarDemandTariff = binReader.ReadDouble();
            tariffProfile.kVADemandTariff = binReader.ReadDouble();

            stationTouPrifile.tariffProfileList.Add(tariffProfile);
        }

        /// <summary>
        /// 解析年费率方案
        /// </summary>
        private void ParseYearTOUProfile(byte[] yearTouStream, StationTOUProfile stationTouPrifile)
        {
            MemoryStream userAuthorityStream = new MemoryStream();
            userAuthorityStream.Write(yearTouStream, 0, yearTouStream.Length);
            BinaryReader binReader = new BinaryReader(userAuthorityStream);

            userAuthorityStream.Position = 0;
            int version = binReader.ReadInt32();
            //判断是否为老的TOU方案
            if (version != SysConstDefinition.OLD_TOUSTRUCT)
                stationTouPrifile.IsNewTou = true;

            //读取TOU年方案的数目
            int yearNum = binReader.ReadInt32();
            for (int i = 0; i < yearNum; i++)
            {
                YearTOUProfile yeaTouProfile = new YearTOUProfile();
                yeaTouProfile.year = binReader.ReadInt32();
                for (int k = 0; k < 366; k++)
                {
                    yeaTouProfile.dayProfileList.Add(binReader.ReadByte());
                }
                //增加年费率方案
                stationTouPrifile.yearProfileList.Add(yeaTouProfile);
            }
        }

        /// <summary>
        /// 解析日费率方案
        /// </summary>
        /// <param name="yearTouStream"></param>
        /// <param name="stationTouPrifile"></param>
        private void ParseDayTOUProfile(DataRow dr, StationTOUProfile stationTouPrifile)
        {
            //日时段名称
            string periodName = Convert.ToString(dr["nodename"]);
            int periodIndex = Convert.ToInt32(dr["nodeid"]);
            byte[] dayTouStream = (byte[])dr["Data"];
            DayProfileStruct dayPrifile = new DayProfileStruct();
            dayPrifile.periodName = periodName;
            dayPrifile.periodIndex = periodIndex;
            DateTime indexTime = Convert.ToDateTime("00:00:00");
            int periodNum = dayTouStream[0];
            //如果第一个时间段不是从0点开始，则把0点到当前时段累加到最后一个时间段
            if (dayTouStream[1] != 0)
            {
                TariffPeriodTime dayProfilePeriod = new TariffPeriodTime();
                dayProfilePeriod.tariffIndex = dayTouStream[periodNum * 2];
                DateTimePair timePair = new DateTimePair();
                timePair.startTimeMinute = 0;
                timePair.endTimeMinute = Convert.ToUInt16(dayTouStream[1] * 15);
                dayProfilePeriod.periodTimeList.Add(timePair);
                dayPrifile.periodTime.Add(dayProfilePeriod);
            }
            //加载日时段的时间区间序列
            for (int l = 1; l <= periodNum; l++)
            {
                bool find = false;
                foreach (TariffPeriodTime tariffiPeriod in dayPrifile.periodTime)
                {
                    //如果当前日时段已经定义了时间，则时间对累加
                    if (tariffiPeriod.tariffIndex == dayTouStream[l * 2])
                    {
                        DateTimePair timePair = new DateTimePair();
                        //每15分钟为一个时间区间
                        timePair.startTimeMinute = Convert.ToUInt16(dayTouStream[l * 2 - 1] * 15);
                        //最后一个时段为24点
                        if (l == periodNum)
                            timePair.endTimeMinute = 1440;
                        else
                            timePair.endTimeMinute = Convert.ToUInt16(dayTouStream[l * 2 + 1] * 15);

                        tariffiPeriod.periodTimeList.Add(timePair);
                        find = true;
                    }
                }
                //没有，则新建
                if (!find)
                {
                    if (dayTouStream[l * 2] != 0)
                    {
                        TariffPeriodTime dayProfilePeriod = new TariffPeriodTime();
                        dayProfilePeriod.tariffIndex = dayTouStream[l * 2];
                        //当前费率段不在厂站费率段之内，则添加
                        if (!stationTouPrifile.tariffIndexList.Contains(dayProfilePeriod.tariffIndex))
                            stationTouPrifile.tariffIndexList.Add(dayProfilePeriod.tariffIndex);
                        DateTimePair timePair = new DateTimePair();
                        timePair.startTimeMinute = Convert.ToUInt16(dayTouStream[l * 2 - 1] * 15);
                        //最后一个时段为24点
                        if (l == periodNum)
                            timePair.endTimeMinute = 1440;
                        else
                            timePair.endTimeMinute = Convert.ToUInt16(dayTouStream[l * 2 + 1] * 15);
                        dayProfilePeriod.periodTimeList.Add(timePair);
                        dayPrifile.periodTime.Add(dayProfilePeriod);
                    }
                }
            }
            stationTouPrifile.dayProfileList.Add(dayPrifile);
        }
        #endregion

        #endregion

    }

}
