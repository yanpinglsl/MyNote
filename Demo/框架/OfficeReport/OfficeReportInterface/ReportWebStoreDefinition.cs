using OfficeReportInterface.DefaultReportInterface.EnergyCost;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using CET.PecsNodeManage;

namespace OfficeReportInterface
{


    /// <summary>
    /// 唯一标识指定节点参数对象的数据结构
    /// 设备ID-DataID-DataTypeID唯一定位一个测点或定时记录SourceID
    /// </summary>
    public struct DeviceDataIDDef
    {
        /// <summary>
        /// 节点类型（例如自定义根节点、自定义组节点、自定义节点、设备节点。。。）
        /// </summary>
        public uint NodeType;
        /// <summary>
        /// 监测节点ID
        /// </summary>
        public uint DeviceID;

        /// <summary>
        /// 参数ID
        /// </summary>
        public uint DataID;

        /// <summary>
        /// 定时记录类型ID
        /// </summary>
        public int ParaTypeID;

        /// <summary>
        /// 参数数据类型ID
        /// </summary>
        public int DataTypeID;

        /// <summary>
        /// 参数数据类型ID
        /// </summary>
        public int LogicalDeviceIndex;
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName;

        /// <summary>
        /// Initializes a new instance of the DeviceDataIDDef struct
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="dataID">参数ID</param>
        /// <param name="dataTypeID">参数类型ID</param>
        public DeviceDataIDDef(uint nodeType,uint nodeID, uint dataID, int paraTypeID, int dataTypeID, int logicalDeviceIndex)
        {
            this.NodeType = nodeType;
            this.DeviceID = nodeID;
            this.DataID = dataID;
            this.ParaTypeID = paraTypeID;
            this.DataTypeID = dataTypeID;
            this.LogicalDeviceIndex = logicalDeviceIndex;
            this.NodeName = string.Empty;
        }
        public DeviceDataIDDef( uint nodeID, uint dataID, int paraTypeID, int dataTypeID, int logicalDeviceIndex)
        {
            this.NodeType = SysNodeType.PECSDEVICE_NODE;
            this.DeviceID = nodeID;
            this.DataID = dataID;
            this.ParaTypeID = paraTypeID;
            this.DataTypeID = dataTypeID;
            this.LogicalDeviceIndex = logicalDeviceIndex;
            this.NodeName = string.Empty;
        }

        public override string ToString()
        {
            return DataFormatManager.Create(FormatType.JsonType).SerializeObject(new List<DeviceDataIDDef> { this });
            //return string.Format("DeviceID={0}, DataID={1}, ParaTypeID={2}, DataTypeID={3},LogicalDeviceIndex={4}",DeviceID , DataID, ParaTypeID, DataTypeID,LogicalDeviceIndex);之前是这样写成字符串的，为了参考用来向前兼容，就不删除这行了。
        }
    }

    ///// <summary>
    ///// PQ参数-参数数据类型对应的实时测点ID以及定时记录SourceID映射结构
    ///// 通过该结构可以快速的完成两者的转化定位
    ///// </summary>
    //public struct DataIDToMeasIDDef
    //{
    //    /// <summary>
    //    /// 无效的映射结果对象
    //    /// </summary>
    //    public static DataIDToMeasIDDef InvalidMapDef = new DataIDToMeasIDDef(0, 0, 0, 0);

    //    /// <summary>
    //    /// 厂站ID
    //    /// </summary>
    //    public uint StationID;

    //    /// <summary>
    //    /// 定时记录SourceID
    //    /// </summary>
    //    public uint SourceID;

    //    /// <summary>
    //    /// 定时记录DataIndex
    //    /// </summary>
    //    public int DataIndex;

    //    /// <summary>
    //    /// 定时记录乘除系数
    //    /// </summary>
    //    public double Cofficient;

    //    /// <summary>
    //    /// Initializes a new instance of the DataIDToMeasIDDef struct
    //    /// </summary>
    //    /// <param name="staID">厂站</param>
    //    /// <param name="sourceID">数据源号</param>
    //    /// <param name="dataIndex">参数序号</param>
    //    /// <param name="cofficient">参数序号</param>
    //    public DataIDToMeasIDDef(uint staID, uint sourceID, int dataIndex, double cofficient)
    //    {
    //        this.StationID = staID;
    //        this.SourceID = sourceID;
    //        this.DataIndex = dataIndex;
    //        this.Cofficient = cofficient;
    //    }
    //}

    #region 历史数据分析存储结构集

    ///// <summary>
    ///// 存放每周波采用点数和最后一次采样序号
    ///// </summary>
    //public struct CountPerCycAndEndIndexInfo
    //{
    //    /// <summary>
    //    /// 每周波采样点数
    //    /// </summary>
    //    public int CountPerCycle;

    //    /// <summary>
    //    /// 该采样率下最后一次采用序号
    //    /// </summary>
    //    public int LastIndex;
    //}

    /// <summary>
    /// 用来保存定时记录查询数据记录点的结构定义
    /// </summary>
    public struct DataLogValueDef
    {
        /// <summary>
        /// 定时记录时刻
        /// </summary>
        public DateTime Logtime;

        /// <summary>
        /// 定时记录数值
        /// </summary>
        public double DataValue;

        /// <summary>
        /// Initializes a new instance of the DataLogValueDef struct
        /// </summary>
        /// <param name="logtime">时间</param>
        /// <param name="dataValue">数值</param>
        public DataLogValueDef(DateTime logtime, double dataValue)
        {
            this.Logtime = logtime;
            this.DataValue = dataValue;
        }
    }

    ///// <summary>
    ///// 用于存储波形数据结果的类
    ///// </summary>
    //public class ResultWaveDataView
    //{
    //    /// <summary>
    //    /// 起始时间
    //    /// </summary>
    //    private DateTime startTime;

    //    /// <summary>
    //    /// 触发时间
    //    /// </summary>
    //    private DateTime triggerTime;

    //    /// <summary>
    //    /// 起始时间
    //    /// </summary>
    //    private int numberOfCycle;

    //    public int NumberOfCycle
    //    {
    //        get { return numberOfCycle; }
    //        set { numberOfCycle = value; }
    //    }

    //    /// <summary>
    //    /// 频率
    //    /// </summary>
    //    private double frequency;

    //    /// <summary>
    //    /// 总点数
    //    /// </summary>
    //    private int totalNum;

    //    /// <summary>
    //    /// 每周波采样点数和最后一次采样序号列表
    //    /// </summary>
    //    private List<CountPerCycAndEndIndexInfo> countPerCycleAndIndexList;

    //    /// <summary>
    //    /// 通道名称
    //    /// </summary>
    //    private List<string> channelNameList;

    //    public List<string> ChannelNameList
    //    {
    //        get { return channelNameList; }
    //        set { channelNameList = value; }
    //    }

    //    /// <summary>
    //    /// 通道数据
    //    /// </summary>
    //    private List<double[]> channelValList;

    //    /// <summary>
    //    /// Initializes a new instance of the ResultWaveDataView class
    //    /// </summary>
    //    /// <param name="startTime">起始时间</param>
    //    /// <param name="triggerTime">触发时间</param>
    //    /// <param name="frequency">频率</param>
    //    /// <param name="totalNum">总点数</param>
    //    public ResultWaveDataView(DateTime startTime, DateTime triggerTime, double frequency, int totalNum)
    //    {
    //        this.startTime = startTime;
    //        this.triggerTime = triggerTime;
    //        this.frequency = frequency;
    //        this.totalNum = totalNum;
    //        this.countPerCycleAndIndexList = new List<CountPerCycAndEndIndexInfo>();
    //        this.channelValList = new List<double[]>();
    //        this.channelNameList = new List<string>();
    //    }

    //    #region 公共属性
    //    /// <summary>
    //    /// Gets or sets 起始时间
    //    /// </summary>
    //    public DateTime StartTime
    //    {
    //        get
    //        {
    //            return this.startTime;
    //        }
    //        set
    //        {
    //            this.startTime = value;
    //        }
    //    }

    //    /// <summary>
    //    /// Gets or sets 触发时间
    //    /// </summary>
    //    public DateTime TriggerTime
    //    {
    //        get
    //        {
    //            return this.triggerTime;
    //        }
    //        set
    //        {
    //            this.triggerTime = value;
    //        }
    //    }

    //    /// <summary>
    //    /// Gets or sets 频率
    //    /// </summary>
    //    public double Frequency
    //    {
    //        get
    //        {
    //            return this.frequency;
    //        }
    //        set
    //        {
    //            this.frequency = value;
    //        }
    //    }

    //    /// <summary>
    //    /// Gets or sets 总点数
    //    /// </summary>
    //    public int TotalNum
    //    {
    //        get
    //        {
    //            return this.totalNum;
    //        }
    //        set
    //        {
    //            this.totalNum = value;
    //        }
    //    }

    //    /// <summary>
    //    /// Gets or sets 每周波采样点数和最后一次采样序号列表
    //    /// </summary>
    //    public List<CountPerCycAndEndIndexInfo> CountPerCycleAndIndexList
    //    {
    //        get
    //        {
    //            return this.countPerCycleAndIndexList;
    //        }
    //        set
    //        {
    //            this.countPerCycleAndIndexList = value;
    //        }
    //    }

    //    /// <summary>
    //    /// Gets or sets 通道数据
    //    /// </summary>
    //    public List<double[]> ChannelValList
    //    {
    //        get
    //        {
    //            return this.channelValList;
    //        }

    //        set
    //        {
    //            this.channelValList = value;
    //        }
    //    }
    //    #endregion
    //}
    #endregion


    /// <summary>
    /// 目前定义的参数类型常量
    /// </summary>
    public class DataTypeDefine
    {
        /// <summary>
        /// 实时值
        /// </summary>
        public const int RealTimeVal = 1;

        /// <summary>
        /// 最大值
        /// </summary>
        public const int MaxVal = 2;

        /// <summary>
        /// 最小值
        /// </summary>
        public const int MinVal = 3;

        /// <summary>
        /// 平均值
        /// </summary>
        public const int AvgVal = 4;

        /// <summary>
        /// 95概率值
        /// </summary>
        public const int Pro95Val = 5;

        /// <summary>
        /// 99概率值
        /// </summary>
        public const int Pro99Val = 6;

        /// <summary>
        /// 获取指定查询类型的名称
        /// </summary>
        /// <param name="dataTypeID">参数类型ID</param>
        /// <returns>参数类型字符串</returns>
        public static string GetDataTypeName(int dataTypeID)
        {
            string result = string.Empty;
            switch (dataTypeID)
            {
                case RealTimeVal:
                    result = "RealTimeValue";
                    break;
                case MaxVal:
                    result = "MaxValue";
                    break;
                case MinVal:
                    result = "MinValue";
                    break;
                case AvgVal:
                    result = "AvgValue";
                    break;
                case Pro95Val:
                    result = "Pro95Value";
                    break;
                case Pro99Val:
                    result = "Pro99Value";
                    break;
            }

            return result;
        }
    }
}