
using System;
using System.Collections.Generic;
using System.Data;
using CET.PecsNodeManage;
using CSharpDBPlugin;
using DBInterfaceCommonLib;
using OfficeReportInterface.DefaultReportInterface.EnergyCost;
using ErrorCode = DBInterfaceCommonLib.ErrorCode;

namespace OfficeReportInterface
{
    /// <summary>
    /// 用于预制报表查询的入参结构
    /// </summary>
    public class ReportQueryParameters
    {

    }

    /// <summary>
    /// 节点类型ID组合
    /// </summary>
    public struct NodeParam
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public uint NodeID;
        /// <summary>
        /// 节点类型
        /// </summary>
        public uint NodeType;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="nType"></param>
        /// <param name="nID"></param>
        public NodeParam(uint nType, uint nID)
        {
            this.NodeType = nType;
            this.NodeID = nID;
        }

        /// <summary>
        /// 构造函数，根据字符串格式类型和ID组合，初始化字段信息
        /// </summary>
        /// <param name="nodeParam">节点类型-节点ID，字符串组合</param>
        /// <param name="separator">分隔字符串</param>
        public NodeParam(string nodeParam, string separator)
        {
            string[] nodeDatas = nodeParam.Split(separator.ToCharArray());
            if (nodeDatas.Length == 2)
            {
                this.NodeType = Convert.ToUInt32(nodeDatas[0]);
                this.NodeID = Convert.ToUInt32(nodeDatas[1]);
            }
            else
            {
                this.NodeType = 0;
                this.NodeID = 0;
            }
        }

        /// <summary>
        /// 根据传入的字符串得到nodeParam的list
        /// </summary>
        /// <param name="nodeParams">旧版本的是  deviceId1;deviceId2;  增加自定义节点后的新版本的是 nodeType-nodeId1;nodeType-nodeId2;</param>
        /// <returns></returns>
        public static List<NodeParam> GetNodeParamList(string nodeParams)
        {
            List<NodeParam> resultList = new List<NodeParam>();
            string[] array = nodeParams.Split(';');
            foreach (string nodeParamStr in array)
            {
                string[] paraArray = nodeParamStr.Split('-');
                uint nodeType = SysNodeType.PECSDEVICE_NODE;
                bool validNodeParam = false;

                uint nodeId;
                var spliter = ',';
                if (paraArray.Length == 2)//有nodetype-nodeId的情况（增加了自定义节点后需要通过nodeType区分）
                {
                    string[] deviceIdWithOtherArrary = paraArray[1].Split(spliter);//例如nodeType-nodeId1,logicalDeviceIndex;nodeType-nodeId2;
                    if (uint.TryParse(paraArray[0], out nodeType) && uint.TryParse(deviceIdWithOtherArrary[0], out nodeId))
                        resultList.Add(new NodeParam(nodeType, nodeId));
                }
                else if (paraArray.Length == 1)//只有deviceID的情况
                {
                    string[] deviceIdWithOtherArrary = paraArray[0].Split(spliter);//例如nodeId1,logicalDeviceIndex;nodeId2;
                    if (uint.TryParse(deviceIdWithOtherArrary[0], out nodeId))
                        resultList.Add(new NodeParam(nodeType, nodeId));
                }
            }
            return resultList;
        }
    }

    /// <summary>
    /// 用以存储定时记录DataLog_Source表数据结构
    /// </summary>
    public struct DataLogSourceData
    {
        public uint sourceID;
        public uint stationID;
        public uint channalID;
        public uint deviceID;
        public int groupHandle;
        public byte dType;
        public int[] paraHandle;
    }

    /// <summary>
    /// 定时记录映射配置的ParaType
    /// </summary>
    public enum DataIDParaType
    {
        /// <summary>
        /// 数据记录
        /// </summary>
        DatalogType = 1,

        /// <summary>
        /// 实时测点
        /// </summary>
        RealTimeType = 2,

        /// <summary>
        /// 越限事件
        /// </summary>
        SetPointType = 3,

        /// <summary>
        /// 开关量实时测点
        /// </summary>
        SwitchReal = 4,

        /// <summary>
        /// 高速定时记录
        /// </summary>
        HighDataLogType = 5,
    }

    /// <summary>
    /// 取点间隔
    /// </summary>
    public enum QueryInterval
    {
        /// <summary>
        /// 秒
        /// </summary>
        QueryByNone = 0,

        /// <summary>
        /// 3分钟
        /// </summary>
        QueryByThreeMinute = 3,

        /// <summary>
        /// 5分钟
        /// </summary>
        QueryByFiveMinute = 5,

        /// <summary>
        /// 15分钟
        /// </summary>
        QueryByQuarter = 15,

        /// <summary>
        /// 时
        /// </summary>
        QueryByHour = 60,

        /// <summary>
        /// 天
        /// </summary>
        QueryByDay = 1440,

        /// <summary>
        /// 月
        /// </summary>
        QueryByMonth = 43200,

        /// <summary>
        /// 年
        /// </summary>
        QueryByYear = 525600,
    }

    /// <summary>
    /// 统计方式类型
    /// </summary>
    public enum StatisticType
    {
        /// <summary>
        /// 时刻值
        /// </summary>
        StatisticTypeLast = 0,

        /// <summary>
        /// 平均值
        /// </summary>
        StatisticTypeAverage = 1,

        /// <summary>
        /// 最大值
        /// </summary>
        StatisticTypeMax = 2,

        /// <summary>
        /// 最小值
        /// </summary>
        StatisticTypeMin = 3,

        /// <summary>
        /// 差值
        /// </summary>
        StatisticTypeTotal = 4,
    }

    /// <summary>
    /// 用于定义和标识电压相别的枚举
    /// </summary>
    public enum VoltagePhaseType
    {
        /// <summary>
        /// 所有电压
        /// </summary>
        VoltagePhaseAll = 0,

        /// <summary>
        /// A相电压
        /// </summary>
        VoltagePhaseA = 1,

        /// <summary>
        /// B相电压
        /// </summary>
        VoltagePhaseB = 2,

        /// <summary>
        /// C相电压
        /// </summary>
        VoltagePhaseC = 3,
    }

    #region
  
    /// <summary>
    /// 费率方案结果
    /// </summary>
    public class TariffProfileStruct
    {
        public string tariffName;
        public int tariffIndex;
        public string tariffUnit;
        public double kWhTariff;
        public double kvarhTariff;
        public double kVAhTariff;
        public double kWDemandTariff;
        public double kvarDemandTariff;
        public double kVADemandTariff;

        public TariffProfileStruct()
        {
            tariffName = "";
            tariffIndex = 0;
            tariffUnit = "$";
            kWhTariff = 1;
            kvarhTariff = 1;
            kVAhTariff = 1;
            kWDemandTariff = 1;
            kvarDemandTariff = 1;
            kVADemandTariff = 1;
        }
    }

    /// <summary>
    /// 解析的TOU方案
    /// </summary>
    public class StationTOUProfile
    {
        //厂站ID
        public uint stationID;
        //判断是新的结构还是老的
        public bool IsNewTou;
        public List<YearTOUProfile> yearProfileList;
        public List<DayProfileStruct> dayProfileList;
        public List<TariffProfileStruct> tariffProfileList;
        //保存当前厂站所有用到的费率段
        public List<int> tariffIndexList;

        public StationTOUProfile()
        {
            yearProfileList = new List<YearTOUProfile>();
            dayProfileList = new List<DayProfileStruct>();
            tariffProfileList = new List<TariffProfileStruct>();
            tariffIndexList = new List<int>();
        }
    }

    /// <summary>
    /// 年费率方案
    /// </summary>
    public class YearTOUProfile
    {
        //年份
        public int year;
        //该年对应的日费率方案
        public List<int> dayProfileList;
        public YearTOUProfile()
        {
            dayProfileList = new List<int>();
        }
    }
    #endregion

    /// <summary>
    /// 用于定义和存储系统公用的常量管理类
    /// </summary>
    public partial class SysConstDefinition
    {
        #region 公有成员属性
        /// <summary>
        /// 设置无效数据值的常量定义
        /// </summary>
        public const double NA = -2147483648F;

        /// <summary>
        /// 用于设定从数据库中查询结果的最大返回条数,使用数据库接口默认条数
        /// </summary>
        public const uint DefaultMaxRowCount = 1000000;

        /// <summary>
        /// 自定义节点
        /// </summary>
        public const uint CUSTOM_NODE = 280039424;

        /// <summary>
        /// 自定义节点组
        /// </summary>
        public const uint CUSTOM_GROUP_NODE = 280035328;

        /// <summary>
        /// 自定义节点组根节点
        /// </summary>
        public const uint CUSTOM_ROOT_NODE = 279969792;

        /// <summary>
        /// 总正向有功电度
        /// </summary>
        public const uint DATAIDKWH_IMPORT = 4000004;
        /// <summary>
        /// 增量正向有功电度
        /// </summary>
        public const uint DATAIDKWH_INCREASEIMPORT = 4000429;
    
        /// <summary>
        /// 总正向无功电度dataId=4000020 
        /// </summary>
        public const uint DATAIDKVARH_IMPORT = 4000020;
        /// <summary>
        /// 间隔无功电度 4000431
        /// </summary>
        public const uint DATAIDKVARH_INCREASEIMPORT = 4000431;



        /// <summary>
        /// 总有功电度
        /// </summary>
        public const uint DATAIDKWH = 4000016;

        /// <summary>
        /// 总无功电度
        /// </summary>
        public const uint DATAIDKVARH = 4000032;

        /// <summary>
        /// 总视在电度
        /// </summary>
        public const uint DATAIDKVAH = 4000048;
        /// <summary>
        /// 总正向视在电度dataId=4000036
        /// </summary>
        public const uint DATAIDKVAH_IMPORT = 4000036;
        /// <summary>
        /// 间隔视在电度
        /// </summary>
        public const uint DATAIDKVAH_INCREASEIMPORT = 4000433;

        // 总有功功率需量
        public const uint DATAIDKW_DEMAND = 7000017;
        // 总无功功率需量
        public const uint DATAIDKVAR_DEMAND = 7000021;
        // 总视在功率需量
        public const uint DATAIDKVA_DEMAND = 7000025;
        // 总有功功率
        public const uint DATAIDKWTOTAL = 2000004;
        ///总无功功率
        public const uint DATAIDKVARTOTAL = 2000008;
        // 总视在功率
        public const uint DATAIDKVATOTAL = 2000012;

        /// <summary>
        /// 瞬变A相持续时间
        /// </summary>
        public const uint TRANSDURATIONA = 3000022;

        /// <summary>
        /// 瞬变A最大幅值
        /// </summary>
        public const uint TRANSTotalMagA = 3000023;

        /// <summary>
        /// 瞬变B相持续时间
        /// </summary>
        public const uint TRANSDURATIONB = 3000024;

        /// <summary>
        /// 瞬变B最大幅值
        /// </summary>
        public const uint TRANSTotalMagB = 3000025;

        /// <summary>
        /// 瞬变C相持续时间
        /// </summary>
        public const uint TRANSDURATIONC = 3000026;

        /// <summary>
        /// 瞬变C最大幅值
        /// </summary>
        public const uint TRANSTotalMagC = 3000027;

        /// <summary>
        ///电压变动持续时间
        /// </summary>
        public const uint DISTURBANCEDURATION= 3000001;    

        /// <summary>
        ///电压变动最大幅值
        /// </summary>
        public const uint DISTURBANCETotalMag = 3000028;       

        /// <summary>
        /// 分时计费方案
        /// </summary>
        public const uint NODETYPE_TOUG = 0x10128000;         

        /// <summary>
        /// 年费率方案节点类型
        /// </summary>
        public const uint NODETYPE_YEARPROFILE = 0x10128310;  
  
        /// <summary>
        /// 日费率方案节点类型
        /// </summary>
        public const uint NODETYPE_DAYPROFILE = 0x10128210;  
   
        /// <summary>
        /// 费率段节点类型
        /// </summary>
        public const uint NODETYPE_TRAFF = 0x10128110;

        /// <summary>
        /// 老版本 TOU结构
        /// </summary>
        public const int OLD_TOUSTRUCT = 1;

        /// <summary>
        /// 新版本TOU结构
        /// </summary>
        public const int NEW_TOUSTRUCT = 2;

        /// <summary>
        /// 瞬变事件类型17
        /// </summary>
        public const int TRANSIENT_TYPE = 17;
        /// <summary>
        /// 电压变动事件类型18
        /// </summary>
        public const int SAG_SWELL_TYPE = 18;

        /// <summary>
        /// 方向事件类型19
        /// </summary>
        public const int SAG_DIRECTION_TYPE = 19;
        /// <summary>
        /// 老版本瞬变事件特征值的个数，不包含总幅值和总持续时间，所以是7个
        /// </summary>
        public const int OLD_TRANSIENT_COUNT = 7;
        /// <summary>
        /// 瞬变事件特征值的个数，由于添加了总幅值和总持续时间，所以变成9个了
        /// </summary>
        public const int TRANSIENT_COUNT = 9;
        /// <summary>
        /// 电压变动事件特征值的个数
        /// </summary>
        public const int SAG_SWELL_COUNT = 15;

        /// <summary>
        /// 暂升Code列表
        /// </summary>
        public static int[] SwellCodes = new int[] { 4, 8, 12, 16, 18, 22 };
        /// <summary>
        /// 暂降Code列表Code1
        /// </summary>
        public static int[] SagCodes = new int[] { 3, 7, 11, 15, 19, 21 };
        /// <summary>
        /// 中断Code列表
        /// </summary>
        public static int[] InteruptCodes = new int[] { 6, 10, 14, 20 };
        /// <summary>
        /// 其他Code列表
        /// </summary>
        public static int[] OtherCodes = new int[] { 0, 1, 2, 5, 9, 13, 17 };

        /// <summary>
        /// 设备节点
        /// </summary>
        public const uint PECSDEVICE_NODE = 269619472;

        /// <summary>
        /// 系统配置节点
        /// </summary>
        public const uint PECSSYSCONFIG_NODE = 269484032;
        #endregion

        #region 私有成员变量

        /// <summary>
        /// 存储波形通道ID与名称映射关系表
        /// </summary>
        private static Dictionary<uint, string> waveChIDAndNameMaps;

        #endregion

        /// <summary>
        /// Initializes a new instance of the SysConstDefinition class
        /// </summary>
        public SysConstDefinition()
        {
            // TODO: 在此处添加构造函数逻辑
        }

        /// <summary>
        /// Gets 获取波形通道ID标识定义参数组
        /// </summary>
        public static Dictionary<uint, string> WaveChIDAndNameMaps
        {
            get { return SysConstDefinition.waveChIDAndNameMaps; }
        }

        /// <summary>
        /// 初始化函数
        /// </summary>
        public static void InitializeSysConstDefinition()
        {
            //波形通道ID标识
            LoadWaveChannelIDAndNameMaps();
        }

        #region 私有成员方法
        /// <summary>
        /// 加载波形查询三相电压和三相电流的通道ID参数
        /// </summary>
        private static void LoadWaveChannelIDAndNameMaps()
        {
            if (waveChIDAndNameMaps == null)
            {
                waveChIDAndNameMaps = new Dictionary<uint, string>();
            }

            DataTable resultDT = new DataTable();
            uint[] waveChannelIDs = new uint[0];
            int errorCode = WaveSolutionMapProvider.Instance.ReadChannelIDNameMaps(waveChannelIDs, ref resultDT);
            if (errorCode == (int)ErrorCode.Success)
            {
                int count = resultDT.Rows.Count;
                waveChannelIDs = new uint[count];
                for (int i = 0; i < count; i++)
                {
                    waveChIDAndNameMaps[Convert.ToUInt32(resultDT.Rows[i]["wavechannelid"])]= Convert.ToString(resultDT.Rows[i]["WAVECHANNELNAME"]);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// 用户相关的常量定义
    /// </summary>
    public class UserNodeType
    {
        /// <summary>
        /// 系统安全节点类型
        /// </summary>
        public const uint SYSTEMSECURITY_NODE = 0x10040000;

        /// <summary>
        /// 系统角色组节点类型
        /// </summary>
        public const uint SYSROLESET_NODE = 0x10048000;

        /// <summary>
        /// 系统角色节点类型
        /// </summary>
        public const uint SYSROLE_NODE = 0x10048100;

        /// <summary>
        /// 用户组配置节点类型
        /// </summary>
        public const uint USERGNODESET_NODE = 0x10042000;

        /// <summary>
        /// 用户组节点类型
        /// </summary>
        public const uint USERG_NODE = 0x10042100;

        /// <summary>
        /// 用户配置节点类型
        /// </summary>
        public const uint USERNODESET_NODE = 0x10041000;

        /// <summary>
        /// 用户节点类型
        /// </summary>
        public const uint USER_NODE = 0x10041100;
        /// <summary>
        /// 权限节点
        /// </summary>
        public const uint AUTHORITY_NODE = 0x10043100;
        /// <summary>
        /// 权限设置节点
        /// </summary>
        public const uint AUTHORITYSET_NODE = 0x10043000;
    }

}
