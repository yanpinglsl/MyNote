using System;
using System.Collections.Generic;
using System.IO;
using CET.PecsNodeManage;

namespace OfficeReportInterface
{
    #region 数据库维护

    /// <summary>
    /// 数据库状态信息
    /// </summary>
    public struct DatabaseStatusDef
    {
        /// <summary>
        /// 数据库信息标志，1-主用数据库，2-备用数据库，3-同步信息，4-自动导出信息
        /// </summary>
        public int DBFlag;
        /// <summary>
        /// 数据库信息列表
        /// </summary>
        public List<List<string>> DBInfos;
        public DatabaseStatusDef(int dbflag)
        {
            this.DBFlag = dbflag;
            this.DBInfos = new List<List<string>>();
        }
    }

    #endregion
    /// <summary>
    /// 错误信息
    /// </summary>
    public struct EMSErrorMsg
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success;
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrorCode;
        /// <summary>
        /// 错误描述信息
        /// </summary>
        public string ErrorDescription;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage;
        /// <summary>
        /// 错误源
        /// </summary>
        public string ErrorMsgInstance;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isSuccess"></param>
        public EMSErrorMsg(bool isSuccess)
        {
            this.Success = isSuccess;
            this.ErrorDescription = string.Empty;
            this.ErrorMsgInstance = string.Empty;
            if (isSuccess)
                this.ErrorCode = (int)EMSWebErrorCode.nomal;
            else
                this.ErrorCode = (int)EMSWebErrorCode.programError;
            this.ErrorMessage = string.Empty;
        }

        /// <summary>
        /// 构造函数，指定错误类型
        /// </summary>
        /// <param name="errorInstance"></param>
        /// <param name="errorCode"></param>
        public EMSErrorMsg(string errorInstance, EMSWebErrorCode errorCode)
        {
            if (errorCode == EMSWebErrorCode.nomal)
            {
                this.Success = true;
                this.ErrorCode = (int)EMSWebErrorCode.nomal;
            }
            else
            {
                this.Success = false;
                this.ErrorCode = (int)errorCode;
            }
            this.ErrorMessage = string.Empty;
            this.ErrorMsgInstance = errorInstance;
            this.ErrorDescription = string.Empty;
        }

        /// <summary>
        /// 从ErrorMsg结构中获取信息
        /// </summary>
        /// <param name="oriMsg"></param>
        public void SetFromErrorMsg(ErrorMsg oriMsg)
        {
            this.Success = oriMsg.IsSuccess;
            this.ErrorDescription = oriMsg.ErrorDescription;
            this.ErrorMsgInstance = oriMsg.ErrorMsgInstance;
            this.ErrorMessage = oriMsg.ErrorMessage;
            if (!oriMsg.IsSuccess)
                this.ErrorCode = (int)EMSWebErrorCode.programError;
            else
                this.ErrorCode = (int)EMSWebErrorCode.nomal;
        }
    }

    /// <summary>
    /// 起始事件-结束时间组合
    /// </summary>
    public struct DateTimeParam
    {
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime StartTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stime"></param>
        /// <param name="etime"></param>
        public DateTimeParam(DateTime stime, DateTime etime)
        {
            this.StartTime = stime;
            this.EndTime = etime;
        }
        /// <summary>
        /// 构造函数，根据字符串类型时间组合，初始化结构字段
        /// </summary>
        /// <param name="timeParam">字符串类型时间组合，起始时间-结束时间</param>
        /// <param name="separator">间隔符</param>
        public DateTimeParam(string timeParam, string separator)
        {
            string[] times = timeParam.Split(separator.ToCharArray());
            if (times.Length == 2)
            {
                this.StartTime = Convert.ToDateTime(times[0]);
                this.EndTime = Convert.ToDateTime(times[1]);
            }
            else
            {
                this.StartTime = new DateTime(1900, 1, 1);
                this.EndTime = new DateTime(1900, 1, 1);
            }
        }
    }

    /// <summary>
    /// 设备参数信息（包括参数ID、通信设备ID和参数数据类型ID）
    /// </summary>
    public struct DeviceDataParam
    {
        /// <summary>
        /// 通信设备ID
        /// </summary>
        public uint DeviceID;
        /// <summary>
        /// 参数ID
        /// </summary>
        public uint DataID;
        /// <summary>
        /// 参数数据类型ID
        /// </summary>
        public uint DataTypeID;
        /// <summary>
        /// 测点回路号
        /// </summary>
        public int LogicalDeviceIndex;
        /// <summary>
        /// 定时记录类型，1-普通定时记录，5-高速定时记录
        /// </summary>
        public int ParaType;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="dataID"></param>
        /// <param name="dataType"></param>
        public DeviceDataParam(uint deviceID, uint dataID, uint dataType)
        {
            this.DataID = dataID;
            this.DeviceID = deviceID;
            this.DataTypeID = dataType;
            this.LogicalDeviceIndex = 1;
            this.ParaType = 1;
        }

        /// <summary>
        /// 构造函数，根据字符串类型参数组合，初始化相关字段
        /// </summary>
        /// <param name="deviceParamStr">字符串类型参数组合，"deviceID,dataID,dataType,logicIndex,paraType"</param>
        /// <param name="separator">间隔字符</param>
        public DeviceDataParam(string deviceParamStr, string separator)
        {
            string[] deviceParams = deviceParamStr.Split(separator.ToCharArray());
            if (deviceParams.Length == 5)
            {
                this.DeviceID = Convert.ToUInt32(deviceParams[0]);
                this.DataID = Convert.ToUInt32(deviceParams[1]);
                this.DataTypeID = Convert.ToUInt32(deviceParams[2]);
                this.LogicalDeviceIndex = Convert.ToInt32(deviceParams[3]);
                this.ParaType = Convert.ToInt32(deviceParams[4]);
            }
            else
            {
                this.DeviceID = 0;
                this.DataID = 0;
                this.DataTypeID = 0;
                this.LogicalDeviceIndex = 0;
                this.ParaType = 0;
            }
        }
        public DeviceDataParam(uint deviceID, uint dataID, uint dataType, int logicalDeviceIndex, int paraType)
        {
            this.DataID = dataID;
            this.DeviceID = deviceID;
            this.DataTypeID = dataType;
            this.LogicalDeviceIndex = logicalDeviceIndex;
            this.ParaType = paraType;
        }
    }

    public struct DevDataIDType
    {
        /// <summary>
        /// 通信设备ID
        /// </summary>
        public uint DeviceID;
        /// <summary>
        /// 参数ID
        /// </summary>
        public uint DataID;
        /// <summary>
        /// 参数数据类型ID
        /// </summary>
        public uint DataTypeID;

        public DevDataIDType(uint devID, uint dataID, uint dataType)
        {
            this.DeviceID = devID;
            this.DataID = dataID;
            this.DataTypeID = dataType;
        }
    }

    /// <summary>
    /// 厂站通道设备ID
    /// </summary>
    public struct StaChaDevID
    {
        public uint StationID;
        public uint ChannelID;
        public uint DeviceID;
        public StaChaDevID(uint sID, uint cID, uint dID)
        {
            this.StationID = sID;
            this.ChannelID = cID;
            this.DeviceID = dID;
        }
    }

    /// <summary>
    /// 定时记录值结构
    /// </summary>
    public struct DataLogOriDef
    {
        public DateTime LogTime;
        public double DataValue;
        public DataLogOriDef(DateTime logtime, double value)
        {
            this.LogTime = logtime;
            this.DataValue = value;
        }
    }

    /// <summary>
    /// 包含定时记录列表的结构
    /// </summary>
    public struct DataLogValueOriList
    {
        /// <summary>
        /// 时区，用分钟表示
        /// </summary>
        public int TimeZone;
        /// <summary>
        /// 定时记录列表
        /// </summary>
        public List<DataLogOriDef> DataList;
    }

    /// <summary>
    /// EEM参数-参数数据类型对应的实时测点ID以及定时记录SourceID映射结构
    /// 通过该结构可以快速的完成两者的转化定位
    /// </summary>
    public struct DataIDToMeasIDDef
    {
        /// <summary>
        /// 无效的映射结果对象
        /// </summary>
        public static DataIDToMeasIDDef InvalidMapDef = new DataIDToMeasIDDef(0, 0, 0, 0);

        /// <summary>
        /// 厂站ID
        /// </summary>
        public uint StationID;
        /// <summary>
        /// 设备ID
        /// </summary>
        public uint DeviceID;

        /// <summary>
        /// 定时记录SourceID
        /// </summary>
        public uint SourceID;

        /// <summary>
        /// 定时记录DataIndex
        /// </summary>
        public int DataIndex;

        /// <summary>
        /// 定时记录乘除系数
        /// </summary>
        public double Cofficient;

        /// <summary>
        /// 翻转值
        /// </summary>
        public double fullScal;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sourceID"></param>
        /// <param name="dataIndex"></param>
        /// <param name="cofficient"></param>
        public DataIDToMeasIDDef(uint staID, uint devID, uint sourceID, int dataIndex)
        {
            this.StationID = staID;
            this.SourceID = sourceID;
            this.DataIndex = dataIndex;
            this.Cofficient = 1;
            this.DeviceID = devID;
            this.fullScal = 0;
        }
    }



    /// <summary>
    /// 事件确认信息
    /// </summary>
    public struct EventAckInfo
    {
        /// <summary>
        /// 是否被确认
        /// </summary>
        public bool IsAcknowledged;
        /// <summary>
        /// 确认时间
        /// </summary>
        public string AckTime;
        /// <summary>
        /// 确认用户
        /// </summary>
        public string AckUser;
        /// <summary>
        /// 确认信息
        /// </summary>
        public string AckReason;
        /// <summary>
        /// 是否允许被确认，有事件确认权限为TRUE，没有权限则为FALSE
        /// </summary>
        public bool EnabledAck;

        /// <summary>
        /// 确认信息是否可被修改，有事件确认修改权限为TRUE，没有权限则为FALSE
        /// </summary>
        public bool EnabledUpdateAck;

        public EventAckInfo(bool enabledAck, bool enabledUpdateAck, bool isAcked)
        {
            this.EnabledAck = enabledAck;
            this.EnabledUpdateAck = enabledUpdateAck;
            this.IsAcknowledged = isAcked;
            this.AckTime = new DateTime(1900, 1, 1).ToShortDateString();
            this.AckUser = string.Empty;
            this.AckReason = string.Empty;
        }
    }

    /// <summary>
    /// 事件类型结构
    /// </summary>
    public struct EventTypeDef
    {
        /// <summary>
        /// 事件类型的ID
        /// </summary>
        public int EventTypeID;
        /// <summary>
        /// 事件类型的名称
        /// </summary>
        public string text;

        public EventTypeDef(int id, string name)
        {
            this.EventTypeID = id;
            this.text = name;
        }
    }


    /// <summary>
    /// 波形信息
    /// </summary>
    public struct OriginalWaveInfo
    {
        /// <summary>
        /// 波形ID
        /// </summary>
        public uint WaveID;
        /// <summary>
        /// 录波触发时间
        /// </summary>
        public string WaveTime;
        /// <summary>
        /// 厂站ID
        /// </summary>
        public uint StationID;
        /// <summary>
        /// 厂站名称
        /// </summary>
        public string StationName;
        /// <summary>
        /// 通道ID
        /// </summary>
        public uint ChannelID;
        /// <summary>
        /// 通道名称
        /// </summary>
        public string ChannelName;
        /// <summary>
        /// 设备ID
        /// </summary>
        public uint DeviceID;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName;

        /// <summary>
        /// 保存logHandle
        /// </summary>
        public int LogHandle;

        /// <summary>
        /// 波形采样率，参考平台组的命名，用于显示波形列表
        /// </summary>
        public int CountPerCycle;

        /// <summary>
        /// 保存波形数据，用于波形的显示
        /// </summary>
        public List<string> WaveData;
    }

    /// <summary>
    /// 实时数据行定义
    /// </summary>
    public struct RltRowDef
    {
        /// <summary>
        /// 查询项目名称
        /// </summary>
        public string ElectricItem;
        /// <summary>
        /// 实时数据列表
        /// </summary>
        public List<uint> DataIDList;

    }

    /// <summary>
    /// 暂态事件结构
    /// </summary>
    public struct SagSwellEvent
    {
        /// <summary>
        /// 触发时间，用于暂态事件分组，触发时间相同的为一组
        /// </summary>
        public string TriggerTime;
        /// <summary>
        /// 事件ID
        /// </summary>
        public uint EventID;
        /// <summary>
        /// 事件记录时间
        /// </summary>
        public string EventTime;
        /// <summary>
        /// 时区
        /// </summary>
        public int LocalZone;
        /// <summary>
        /// 暂态事件类型
        /// </summary>
        public string RMSEventType;
        /// <summary>
        /// 厂站ID
        /// </summary>
        public uint StationID;
        /// <summary>
        /// 通道ID
        /// </summary>
        public uint ChannelID;
        /// <summary>
        /// 设备ID
        /// </summary>
        public uint DeviceID;
        /// <summary>
        /// 监测节点类型
        /// </summary>
        public uint PQNodeType;
        /// <summary>
        /// 监测节点ID
        /// </summary>
        public uint PQNodeID;
        /// <summary>
        /// 监测节点名称
        /// </summary>
        public string PQNodeName;
        /// <summary>
        /// 厂站标识
        /// </summary>
        public int StationFlag;
        /// <summary>
        /// 事件类型
        /// </summary>
        public int EventType;
        /// <summary>
        /// 事件等级
        /// </summary>
        public int EventClass;
        /// <summary>
        /// 事件Byte
        /// </summary>
        public int EventByte;

        /// <summary>
        /// 事件Code1
        /// </summary>
        public int EventCode1;
        /// <summary>
        /// 事件Code2
        /// </summary>
        public int EventCode2;
        /// <summary>
        /// 是否有波形
        /// </summary>
        public bool HaveWave;

        /// <summary>
        /// 特征值信息
        /// </summary>
        public SagSwellFeature FetureInfo;
        /// <summary>
        /// 事件确认信息
        /// </summary>
        public EventAckInfo AckInfo;

        /// <summary>
        /// 额定电压
        /// </summary>
        public string NominalVoltage;
        /// <summary>
        /// 事件详细信息，紧挨着的两个元素，一个是名称，一个是其值的最终字符串
        /// </summary>
        public List<List<string>> Details;
        /// <summary>
        /// 是否是最坏事件
        /// </summary>
        public bool IsWorstCase;
    }


    /// <summary>
    /// 暂态特征值信息
    /// </summary>
    public struct SagSwellFeature
    {
        /// <summary>
        /// 特征值所在事件ID
        /// </summary>
        public uint EventID;
        /// <summary>
        /// 是否进一步查询特征值信息
        /// </summary>
        public bool QueryForMoreFeature;
        /// <summary>
        /// 是否进一步查询定时记录信息
        /// </summary>
        public bool QueryForMoreDatalog;
        /// <summary>
        /// 特征值对应的时间
        /// </summary>
        public string FeatureTime;
        /// <summary>
        /// 持续时间
        /// </summary>
        public double Duration;
        /// <summary>
        /// 总幅值
        /// </summary>
        public double TotalMag;
        /// <summary>
        /// 特征值信息列表
        /// </summary>
        public List<double> FeatureValues;
        /// <summary>
        /// 是否越限，-1-区域外，0-不越限，1-越上限，2-越下限
        /// </summary>
        public int OutOfLimit;
        /// <summary>
        /// 对应的故障方向事件的描述信息
        /// </summary>
        public string Direction;

        public SagSwellFeature(bool moreFeature, bool moreDatalog)
        {
            this.EventID = 0;
            this.QueryForMoreFeature = moreFeature;
            this.QueryForMoreDatalog = moreDatalog;
            this.Duration = double.NaN;
            this.TotalMag = double.NaN;
            this.FeatureValues = new List<double>();
            this.FeatureTime = new DateTime(1900, 1, 1).ToShortDateString();
            this.OutOfLimit = -1; //默认在区域外
            this.Direction = string.Empty;
        }
    }

    ///// <summary>
    ///// SARFI曲线结构
    ///// </summary>
    //public struct SARFIChartCurve
    //{
    //    /// <summary>
    //    /// 曲线类型，1-ITIC曲线，2-SEMI曲线
    //    /// </summary>
    //    public int CurveType;
    //    /// <summary>
    //    /// 曲线ID，1-表示默认曲线
    //    /// </summary>
    //    public int CurveID;
    //    /// <summary>
    //    /// 曲线名称
    //    /// </summary>
    //    public string CurveName;
    //    /// <summary>
    //    /// 上限曲线
    //    /// </summary>
    //    public List<SARFIChartValue> UpLineValues;
    //    /// <summary>
    //    /// 下限曲线
    //    /// </summary>
    //    public List<SARFIChartValue> DownLineValues;
    //    /// <summary>
    //    /// 构造函数
    //    /// </summary>
    //    /// <param name="curType"></param>
    //    /// <param name="curID"></param>
    //    public SARFIChartCurve(int curType, int curID)
    //    {
    //        this.CurveType = curType;
    //        this.CurveID = curID;
    //        this.CurveName = string.Empty;
    //        this.UpLineValues = new List<SARFIChartValue>();
    //        this.DownLineValues = new List<SARFIChartValue>();
    //    }

    //    /// <summary>
    //    /// 写二进制流
    //    /// </summary>
    //    /// <param name="writer"></param>
    //    public void WriteBuffer(BinaryWriter writer)
    //    {
    //        writer.Write(this.CurveType);
    //        writer.Write(this.CurveID);
    //        writer.Write(this.CurveName);
    //        WriteValues(writer, this.UpLineValues);
    //        WriteValues(writer, this.DownLineValues);
    //    }
    //    /// <summary>
    //    /// 读二进制流
    //    /// </summary>
    //    /// <param name="reader"></param>
    //    public void ReadBuffer(BinaryReader reader)
    //    {
    //        this.CurveType = reader.ReadInt32();
    //        this.CurveID = reader.ReadInt32();
    //        this.CurveName = reader.ReadString();
    //        ReadValues(reader, ref this.UpLineValues);
    //        ReadValues(reader, ref this.DownLineValues);
    //    }

    //    private void WriteValues(BinaryWriter writer, List<SARFIChartValue> lineValues)
    //    {
    //        writer.Write(lineValues.Count);
    //        foreach (SARFIChartValue valueItem in lineValues)
    //        {
    //            writer.Write(valueItem.XValue);
    //            writer.Write(valueItem.YValue);
    //        }
    //    }

    //    private void ReadValues(BinaryReader reader, ref List<SARFIChartValue> lineValues)
    //    {
    //        lineValues = new List<SARFIChartValue>();
    //        int valueCount = reader.ReadInt32();
    //        for (int i = 0; i < valueCount; i++)
    //        {
    //            SARFIChartValue valueItem = new SARFIChartValue();
    //            valueItem.XValue = reader.ReadDouble();
    //            valueItem.YValue = reader.ReadDouble();
    //            lineValues.Add(valueItem);
    //        }
    //    }
    //}

    //public struct SARFIChartValue
    //{
    //    public double XValue;
    //    public double YValue;
    //    public SARFIChartValue(double xval, double yval)
    //    {
    //        this.XValue = xval;
    //        this.YValue = yval;
    //    }
    //}

    //public struct SARFIChartInfo
    //{
    //    public int CurveType;
    //    public int CurveID;
    //    public string CurveName;
    //    public SARFIChartInfo(int curType, int curID)
    //    {
    //        this.CurveType = curType;
    //        this.CurveID = curID;
    //        this.CurveName = string.Empty;
    //    }
    //}

    /// <summary>
    /// SAFI特征值
    /// </summary>
    public struct SarfiRMSValue
    {
        /// <summary>
        /// 厂站ID
        /// </summary>
        public uint StationID;
        /// <summary>
        /// 通道ID
        /// </summary>
        public uint ChannelID;
        /// <summary>
        /// 关联设备ID
        /// </summary>
        public uint DeviceID;
        /// <summary>
        /// 电能质量节点名称
        /// </summary>
        public string PQNodeName;
        /// <summary>
        /// 越上限或者越下限，-1-不在区域范围内，0-不越限，1-越上限，2-越下限
        /// </summary>
        public int LimitType;
        /// <summary>
        /// 特征值类型，0-总特征值，1-A相特征值
        /// 2-B相特征值，3-C相特征值
        /// </summary>
        public int FeatureType;
        /// <summary>
        /// 特征值类型（总特征值、ABC三相特征值）
        /// </summary>
        public string FeatureTypeStr;
        /// <summary>
        /// 事件类型，电压瞬变或者电压变动
        /// </summary>
        public int EventType;
        /// <summary>
        /// 事件类型名称
        /// </summary>
        public string EventTypeStr;
        /// <summary>
        /// 事件时间
        /// </summary>
        public string EventTime;
        /// <summary>
        /// 持续时间
        /// </summary>
        public double Duration;
        /// <summary>
        /// 总幅值
        /// </summary>
        public double TotalMag;
        /// <summary>
        /// 存储所有特征值
        /// </summary>
        public List<double> FeatureValues;
        /// <summary>
        /// 18为暂升,19为暂降等等,需根据此字段匹配起始结束事件
        /// </summary>
        public int Code1;
        /// <summary>
        /// 1为起始事件，2为结束事件
        /// </summary>
        public int Code2;

        public SarfiRMSValue Clone()
        {
            SarfiRMSValue cloneData = new SarfiRMSValue();
            cloneData.StationID = this.StationID;
            cloneData.ChannelID = this.ChannelID;
            cloneData.DeviceID = this.DeviceID;
            cloneData.PQNodeName = this.PQNodeName;
            cloneData.LimitType = this.LimitType;
            cloneData.FeatureType = this.FeatureType;
            cloneData.FeatureTypeStr = this.FeatureTypeStr;
            cloneData.EventType = this.EventType;
            cloneData.EventTypeStr = this.EventTypeStr;
            cloneData.EventTime = this.EventTime;
            cloneData.Duration = this.Duration;
            cloneData.TotalMag = this.TotalMag;
            return cloneData;
        }
    }

    /// <summary>
    /// 用于SARFI特征值波形查找的条件
    /// </summary>
    public struct SARFIWaveHandle
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public uint DeviceID;
        /// <summary>
        /// 事件类型
        /// </summary>
        public uint EventType;
        /// <summary>
        /// 事件时间
        /// </summary>
        public DateTime EventTime;
        /// <summary>
        /// 持续时间
        /// </summary>
        public double Duration;
    }

    /// <summary>
    /// 谐波越限次数统计
    /// </summary>
    public struct HarmonicDataOffLimit
    {
        public string NodeName;
        public List<double> OffLimitTimes;
        public List<double> UpLimitDatas;
        public List<double> LowLimitDatas;
        public bool leaf;
        public List<HarmonicDataOffLimit> Children;
        public HarmonicDataOffLimit(string nodeName)
        {
            this.NodeName = nodeName;
            this.OffLimitTimes = new List<double>();
            this.UpLimitDatas = new List<double>();
            this.LowLimitDatas = new List<double>();
            this.leaf = false;
            this.Children = new List<HarmonicDataOffLimit>();
        }
    }

    /// <summary>
    /// EMSWeb错误码
    /// </summary>
    public enum EMSWebErrorCode
    {
        /// <summary>
        /// 正常
        /// </summary>
        nomal = 0,
        /// <summary>
        /// 令牌错误
        /// </summary>
        tokenError = 1,
        /// <summary>
        /// 权限错误
        /// </summary>
        authError = 2,
        /// <summary>
        /// 数据库错误
        /// </summary>
        databaseError = 3,
        /// <summary>
        /// 程序错误
        /// </summary>
        programError = 4,
        /// <summary>
        /// 初始化错误（可能是未初始化，也可能是超时）
        /// </summary>
        initialError = 5,
        /// <summary>
        /// 等待时间已到
        /// </summary>
        waitTimeOut = 6,
        /// <summary>
        /// 导出错误
        /// </summary>
        exportError = 7,
        /// <summary>
        /// 用于返回WEB显示的错误
        /// </summary>
        returnError = 8,
        /// <summary>
        /// 图片错误
        /// </summary>
        pictureError = 9,
        /// <summary>
        /// 用户锁定
        /// </summary>
        userLock = 10,
        /// <summary>
        /// 用户编辑超时
        /// </summary>
        userEditTimeOut = 11,
        /// <summary>
        /// 节点验证失败
        /// </summary>
        nodeError = 12,
    }

    /// <summary>
    /// 缓存文件状态
    /// </summary>
    public enum BufferFileState
    {
        /// <summary>
        /// 无效
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// 稳定的
        /// </summary>
        Stable = 1,
        /// <summary>
        /// 临时的
        /// </summary>
        Temporary = 2,
    }
    /// <summary>
    /// 特征值信息，用来填写事件的detail栏的数据
    /// </summary>
    public class FetureInfoResult
    {
        /// <summary>
        /// 额定电压
        /// </summary>
        public string NominalVoltage;
        /// <summary>
        /// 持续时间
        /// </summary>
        public string Duration;
        /// <summary>
        /// 总幅值
        /// </summary>
        public string TotalMag;
        /// <summary>
        /// 第1个数据
        /// </summary>
        public string data1;
        /// <summary>
        /// 第2个数据
        /// </summary>
        public string data2;
        /// <summary>
        /// 第3个数据
        /// </summary>
        public string data3;
        /// <summary>
        /// 第4个数据
        /// </summary>
        public string data4;
        /// <summary>
        /// 第5个数据
        /// </summary>
        public string data5;
        /// <summary>
        /// 第6个数据
        /// </summary>
        public string data6;
        /// <summary>
        /// 第7个数据
        /// </summary>
        public string data7;
        /// <summary>
        /// 第8个数据
        /// </summary>
        public string data8;
        /// <summary>
        /// 第9个数据
        /// </summary>
        public string data9;
        /// <summary>
        /// 第10个数据
        /// </summary>
        public string data10;
        /// <summary>
        /// 第11个数据
        /// </summary>
        public string data11;
        /// <summary>
        /// 第12个数据
        /// </summary>
        public string data12;
        /// <summary>
        /// 第13个数据
        /// </summary>
        public string data13;
        /// <summary>
        /// 第14个数据
        /// </summary>
        public string data14;
        /// <summary>
        /// 第15个数据
        /// </summary>
        public string data15;
    }

    public struct DevDataID
    {
        /// <summary>
        /// 通信设备ID
        /// </summary>
        public uint DeviceID;
        /// <summary>
        /// 参数ID
        /// </summary>
        public uint DataID;

        public DevDataID(uint devID, uint dataID)
        {
            this.DeviceID = devID;
            this.DataID = dataID;
        }
    }
       
}
