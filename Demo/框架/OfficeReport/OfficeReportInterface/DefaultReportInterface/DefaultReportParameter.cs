using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using CET.PecsNodeManage;
using OfficeReportInterface.DefaultReportInterface.EnergyCost;


namespace OfficeReportInterface
{
    /// <summary>
    /// 设备-回路号-参数结构体
    /// </summary>
    public struct LogicalDeviceIndex
    {
        /// <summary>
        /// 节点类型。原本只有设备类型，增加了自定义节点后，可能是自定义节点的类型（自定义根节点、自定义组节点、自定义节点）
        /// </summary>
        public uint NodeType;

        /// <summary>
        /// 原本只有设备类型，这里表示的就是deviceID，增加了自定义节点后，可能是自定义节点的类型的nodeId（自定义根节点、自定义组节点、自定义节点）
        /// </summary>
        public uint DeviceID;
        /// <summary>
        /// 回路号。原本只有设备类型，这里的回路号是只有设备节点才有的回路号，自定义根节点、自定义组节点不存在回路号
        /// </summary>
        public int LogicalIndex;
        /// <summary>
        /// 节点名称。这个是增加了自定义节点标签页后，为了保存自定义节点而增加的属性
        /// </summary>
        public string NodeName;
        /// <summary>
        /// 构造函数（在增加自定义节点树后，deviceID就可能是自定义根节点、自定义组节点、自定义节点的ID，因此必须加上nodeType）
        /// </summary>
        /// <param name="nodeType"></param>
        /// <param name="deviceID"></param>
        /// <param name="nodeName"></param>
        public LogicalDeviceIndex(uint nodeType,uint deviceID,string nodeName)
        {
            this.NodeType = nodeType;
            this.DeviceID = deviceID;
            this.LogicalIndex = 1;
            this.NodeName = nodeName;
        }

        /// <summary>
        /// 构造函数(在没有增加自定义节点树的时候，deviceID就是设备ID)
        /// </summary>
        /// <param name="nodeType"></param>
        /// <param name="deviceID"></param>
        /// <param name="logicalIndex"></param>
        public LogicalDeviceIndex(uint nodeType, uint deviceID, int logicalIndex)
        {
            this.NodeType = nodeType;
            this.DeviceID = deviceID;
            this.LogicalIndex = logicalIndex;
            this.NodeName = string.Empty;
        }
    }

    /// <summary>
    /// 查询参数类，包含所有查询条件信息
    /// </summary>
    public class DefaultReportParameter
    {
        public List<List<uint>> StationChannelDeviceList{ get; set; }

        //EnergyCost的dataId选项
        public int kWh=0;
        public int kvarh = 0;
        public int kVAh = 0;
        public int kWDemand = 0;
        public int kvarDemand = 0;
        public int kVADemand = 0;

        private NewTOUProfile _touProfile = new NewTOUProfile();

        public NewTOUProfile TOUProfile
        {
            get { return _touProfile; }
            set { _touProfile = value; }
        }

        /// <summary>
        /// 是否是ITIC曲线还是SEMI
        /// </summary>
        public bool IsITIC { get; set; }

        /// <summary>
        /// 是否为需量
        /// </summary>
        public bool IsDemand{ get; set; }


        public int Interval{ get; set; }

        public bool IsIncludeTable{ get; set; }
 
        /// <summary>
        /// 设备参数列表
        /// </summary>
        public List<DeviceDataIDDef> DeviceDataIDList{ get; set; }

        /// <summary>
        /// 是否包含警告信息
        /// </summary>
        public bool IsIncludeWarning{ get; set; }
        /// <summary>
        /// 时段类型,日周月年
        /// </summary>
        public int PeriodType{ get; set; }
        /// <summary>
        /// 时间数目，是单个时间还是两个时间
        /// </summary>
        public int CompareNumber{ get; set; }

        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime StartTime{ get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 是否包含平均值
        /// </summary>
        public bool IsIncludeAvg{ get; set; }
        /// <summary>
        /// 是否包含求和
        /// </summary>
        public bool IsIncludeTotal{ get; set; }
        /// <summary>
        /// 时间列表，Energy Period报表中使用
        /// </summary>
        public List<DateTime> DateTimeList  { get; set; }
        /// <summary>
        /// 需要查询的userID
        /// </summary>
        public uint userID { get; set; }
        /// <summary>
        /// 需要查询的userName
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// ITIC/SEMI曲线的ID。
        /// </summary>
        public int curveID { get; set; }

        public DefaultReportParameter()
        {
            DeviceDataIDList = new List<DeviceDataIDDef>();
            DateTimeList = new List<DateTime>();
            IsIncludeWarning = false;
            PeriodType = 0;
            userID = 1;
            userName = "ROOT";
            curveID = 0;
        }
    }
}