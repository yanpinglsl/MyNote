using System;
using System.Collections.Generic;
using OfficeReportInterface.DefaultReportInterface.EnergyCost;
using PecsReport.CommomNode;
using PecsReport.PluginInterface;


namespace OfficeReportInterface
{

    public enum DateOffSetType
    {
        RelativeOffset = 0,
        AbsoluteOffset = 1,
    }

    public enum QuerySpan
    {
        Null = 0,
        Day = 1,
        Week = 2,
        Month = 3,
        Quarter = 4,
        Year = 5, 
    }

    public enum Interval
    {
        Day = 1,
        Week = 2,
        Month = 3,
        Year = 5,
        Minute15 = 6,
        Minute30 = 7,
        Hour_1 = 8,
        Hour_2 = 9,
        Hour_4 = 10,
        Hour_8 = 11,
    }

    /// <summary>
    /// 数据源标签结构体
    /// </summary>
    public struct DataSourceLabel
    {
        public string version;
        public string name;
        /// <summary>
        /// 普通报表为数据源报表的报表ID，reporting service报表为0
        /// </summary>
        public uint source;
        /// <summary>
        /// 时间偏移类型
        /// 标签中不存在(为null)则查询输入日期当日;
        /// 为1表示绝对偏移；
        /// 为0表示相对偏移；
        /// </summary>
        public DateOffSetType dateOffsetType;
        /// <summary>
        /// 代表偏移若干单位时间；
        /// 相对偏移表示偏移yyyy年，mm月，dd日
        /// 绝对偏移表示yyyy-mm-03则查询输入日期当月，从03日开始
        /// yyyy-05-03则查询输入日期当年，从05月开始
        /// 2010-05-01则查询从输入日期当年开始
        /// </summary>
        public string dateOffSet; 
        /// <summary>
        /// 实际查询时间（输入时间+偏移时间）（对静态引用标签为查询时间，对动态引用标签为起始时间）
        /// </summary>
        public DateTime startTime;  
        /// <summary>
        /// 结束时间（只对动态引用标签和查询时段报表源有用）
        /// </summary>
        public DateTime endTime;  
        /// <summary>
        /// 查询的日期跨度
        /// </summary>
        public int dateSpan;
        /// <summary>
        /// 设备节点类型与ID（(Typename)i形式）,Typename包括“根节点/厂站节点/通道节点/设备节点”
        /// </summary>
        public string node;
        /// <summary>
        /// 动态报表查询跨度（动态报表专有，0/1/2/3/4/5分别代表跨度为：不设定（在查询时选择起止时间）/日/周/月/季/年）
        /// </summary>
        public QuerySpan querySpan;
        /// <summary>
        /// 动态报表取点间隔（动态报表专有，日周月季年分别是1 2 3 4 5，1分钟、3分钟、5分钟、10分钟、15分、30分、1小时、2小时、4小时、8小时为6-15）
        /// </summary>
        public Interval interval;
        /// <summary>
        /// 记录excel文件为何种类型的报表，以确定根据DataNum进行叠加的单位
        /// </summary>
        public FileType excelType;  
        /// <summary>
        /// 整个标签字符串
        /// </summary>
        public string entireDSLString;  
        /// <summary>
        /// 报表节点，分为pecstar、EEM、PQ  （nodetype:pecstar272760832,EEM272765184,PQ?）
        /// </summary>
        public ReportNode reportNode;  
        /// <summary>
        /// 报表源类型，与枚举ReportQueryType的0~10对应（由于三种报表的类型是统一到0-10的）
        /// </summary>
        public ReportQueryType reportType; 
        /// <summary>
        /// ReportingService报表的标签信息
        /// </summary>
        public ReportingServiceParms entireRSParms;
        /// <summary>
        /// 是否需要执行动态计算公式
        /// </summary>
        public bool needDynamicFormula;
        /// <summary>
        /// 是否是查询条件
        /// </summary>
        public bool isQueryCondition;
  
    }

    public struct ReportingServiceParms
    {
        public string deviceIDs;
        public uint nodeType;
        public int intervalType;
        public int periodType;
        public int statType;
        public int compareNumber;
        public bool isCludeWarning;
        public bool isCludeDataTable;
        public bool isIncludeTotal;
        public bool isIncludeAvg;
        public bool isItic;
        /// <summary>
        /// ITIC或者SEMI的curveID
        /// </summary>
        public int curveID;

        public bool isDemand;
        public List<DateTime> dateTimeList;
        public NewTOUProfile TouProfile;
        public int kWh;
        public int kvarh;
        public int kVAh;
        public int kWDemand;
        public int kvarDemand;
        public int kVADemand;
    }

 

    /// <summary>
    /// 引用标签结构体
    /// </summary>
    public struct DataRetrievalLabel
    {
        /// <summary>
        /// 标签所在单元格行数（主要用于静态引用标签）
        /// </summary>
        public int rowPosition; 
        /// <summary>
        /// 标签所在单元格列数
        /// </summary>
        public int columnPosition;  
        /// <summary>
        /// 数据集标签和数据集序号
        /// </summary>
        public string name; 
        /// <summary>
        /// 引用数据范围
        /// </summary>
        public string scope; 
        /// <summary>
        /// （Col/Row）行方向或列方面自动复制，默认为行复制
        /// </summary>
        public string direction;  
        /// <summary>
        /// 行或列与前一对象相隔的行（列）数,默认为0
        /// </summary>
        public int space;  
        /// <summary>
        /// 获取子结果集，如“startindex，Count”（startindex表示起始，Length表示共几个）为空则获取全部
        /// </summary>
        public string subDataset;  
        /// <summary>
        /// 标签所在sheet名称
        /// </summary>
        public string sheetName;  
    }

    public enum DataMusterParamDiscribtion
    {
        CommonReport=0,
        PeriodReport=1,
        DynamicReport=2,
        DefaultReport=3,      
    }

    /// <summary>
    /// 查询输出结果
    /// </summary>
    public struct DataResults
    {
        /// <summary>
        /// //查询获得的数据集是否是真实需要的。针对数据源标签OffsetDate出现yyyy-MM-31提出，这时的4-31会偏移至
        /// 5-1，但该数据是不应该显示出来的，标识后在填充生成文件时将填充为空
        /// </summary>
        public bool notRealWanted; 
        /// <summary>
        /// 数据集名称
        /// </summary>
        public string name; 
        /// <summary>
        /// 数据集个数（N）
        /// </summary>
        public int dataMusterNum; 
        /// <summary>
        /// 数据集参数说明数组（N个结构）——行数、列数、ID号、名称
        /// </summary>
        public DataMusterParamDiscribtion[] dataMusterParamDiscribtionArray;  
        /// <summary>
        /// 数据集矩阵（N个矩阵）——内部数据用不定长字符串形式存储
        /// </summary>
        public List<List<string>>[] dataMusterArray;  
    }

    public enum NodeType
    {
        RootNode = 269484032,
        StationNode = 269615104,
        ChannalNode = 269619456,
        DeviceNode = 269619472,
    }

    public enum FileType
    {
        PeriodType = 0,
        DayType = 1,
        WeekType = 2,
        MonthType = 3,
        SeasonType = 4,
        YearType = 5,
        FlexibleType = 7,
        ReportingServiceType = 9,
    }

    public enum RepServFileType
    {
        MinumOfTemplate=EnergyPeriod,
        //Type100ms = 99999990,没有这预制报表，因此删除。
        //EnergyDemand = 99999991,没有这预制报表，因此删除。
        EventHistory = 99999992,
        LoadProfile = 99999993,
        PowerQuality = 99999994,
        //SystemConfig = 99999995,没有这预制报表，因此删除。
        Tabular = 99999996,
        Trend = 99999997,
        SingleUsage = 99999998,
        MultiUsage = 99999999,
        EnergyPeriod = 99999910,
        EnergyCost = 99999911,
        HourlyUsage = 99999912,
        PowerQualityEventsOnly = 99999913,
        /// <summary>
        /// 智慧安全用电运行评估报告
        /// </summary>
        Safety = 99999914,
    }
}
