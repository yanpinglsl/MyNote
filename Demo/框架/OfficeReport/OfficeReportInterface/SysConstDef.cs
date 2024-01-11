using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using CSharpDBPlugin;
using DBInterfaceCommonLib;
using ErrorCode = DBInterfaceCommonLib.ErrorCode;

namespace OfficeReportInterface
{
    /// <summary>
    /// 用于定义和存储系统公用的常量管理类
    /// </summary>
    public class SysConstDef
    {
        #region 公有成员属性
        /// <summary>
        /// 设置无效数据值的常量定义
        /// </summary>
        public const double NA = -2147483648F;

        /// <summary>
        /// 用于设定从数据库中查询结果的最大返回条数
        /// </summary>
        public const uint DefaultMaxRowCount = 100000000;
        /// <summary>
        /// 默认最多导出6万条
        /// </summary>
        public const uint ExportMaxRowCount = 60000;

        /// <summary>
        /// 缓存文件的保存路径
        /// </summary>
        public static readonly string BufferFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"DataBuffer");

        /// <summary>
        /// 自定义配置文件的保存路径
        /// </summary>
        public static string ProfileDirPath;
        /// <summary>
        /// 图片存储路径
        /// </summary>
        public static readonly string PictureDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pictures");

        /// <summary>
        /// 临时文件夹目录
        /// </summary>
        public static readonly string TempDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CGI", "TempFile");

        /// <summary>
        /// 连接数据库项目名称（初始化时赋值）
        /// </summary>
        public static string ProjectName = string.Empty;

        /// <summary>
        /// 修改事件确认信息对应的权限
        /// </summary>
        public static readonly uint UpdateAckAlarm = 14;

        /// <summary>
        /// 获取项目名称所在缓存目录的路径
        /// </summary>
        /// <returns></returns>
        public static string GetProjectFilePath()
        {
            return Path.Combine(BufferFilePath, ProjectName);
        }

        #endregion

        /// <summary>
        /// 定义私有构造函数，不允许实例化对象
        /// </summary>
        private SysConstDef()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }       

    }  



    /// <summary>
    /// 参数定义
    /// </summary>
    public class DataParameterInfoDef
    {
        /// <summary>
        /// 唯一实例
        /// </summary>
        public readonly static DataParameterInfoDef DataManager = new DataParameterInfoDef();

        /// <summary>
        /// dataType与相关名称的映射关系
        /// </summary>
        private Dictionary<uint, string> dataTypeNameMap = new Dictionary<uint, string>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public EMSErrorMsg Initialize()
        {
            EMSErrorMsg resultMsg = new EMSErrorMsg(true);

            DataTable resultTable = new DataTable();
            int errorCode = DataLogSolutionMapProvider.Instance.ReadDataTypeNameMaps(0,null, ref resultTable);
            if (errorCode != (int)ErrorCode.Success)
            {
                ErrorInfoManager.Instance.WriteDBInterfaceLog(errorCode,"DatalogSolutionMapProvider.ReadDataTypeNameMaps");
                resultMsg = new EMSErrorMsg("DatalogSolutionMapProvider.ReadDataTypeNameMaps", EMSWebErrorCode.databaseError);
            }
            
            foreach (DataRow dr in resultTable.Rows)
            {
                uint dataTypeID = Convert.ToUInt32(dr["DataTypeID"]);
                string dataTypeName = Convert.ToString(dr["DataTypeName"]);
                if (!dataTypeNameMap.ContainsKey(dataTypeID))
                    dataTypeNameMap.Add(dataTypeID, dataTypeName);
            }



            return resultMsg;
        }

        /// <summary>
        /// 获取参数ID与名称的映射关系
        /// </summary>
        /// <param name="dataIDs"></param>
        /// <returns></returns>
        public Dictionary<uint, string> GetDataIDNameMap(List<uint> dataIDs)
        {
            Dictionary<uint, string> resultMap = new Dictionary<uint, string>();

            DataTable resultTable = new DataTable();
            int errorCode = DataLogSolutionMapProvider.Instance.ReadDataIDNameMaps(0,dataIDs.ToArray(), ref resultTable);
            if (errorCode != (int)ErrorCode.Success)
                ErrorInfoManager.Instance.WriteDBInterfaceLog(errorCode,"DatalogSolutionMapProvider.Instance.ReadDataIDNameMaps");

            Dictionary<uint, string> dataIDNameMap = new Dictionary<uint, string>();
            foreach (DataRow dr in resultTable.Rows)
            {
                uint dataID = Convert.ToUInt32(dr["DataID"]);
                string dataName = Convert.ToString(dr["DataName"]);
                if (!dataIDNameMap.ContainsKey(dataID))
                    dataIDNameMap.Add(dataID, dataName);
            }

            foreach (uint dataid in dataIDs)
            {
                string dataName;
                bool hasName = dataIDNameMap.TryGetValue(dataid, out dataName);
                if (!hasName)
                    dataName = string.Empty;
                if (!resultMap.ContainsKey(dataid))
                    resultMap.Add(dataid, dataName);
            }

            return resultMap;
        }

        /// <summary>
        /// 获取DataType相关名称
        /// </summary>
        /// <param name="dataTypeID"></param>
        /// <returns></returns>
        public string GetDataTypeNameByID(uint dataTypeID)
        {
            string dataTypeName;
            bool hasName = dataTypeNameMap.TryGetValue(dataTypeID, out dataTypeName);
            if (!hasName)
                dataTypeName = string.Empty;
            return dataTypeName;
        }

        /// <summary>
        /// 获取参数类型与名称的映射关系
        /// </summary>
        /// <param name="dataTypes"></param>
        /// <returns></returns>
        public Dictionary<uint, string> GetDataTypeNameMap(List<uint> dataTypes)
        {
            Dictionary<uint, string> resultMap = new Dictionary<uint, string>();          

            foreach (uint dataType in dataTypes)
            {
                string dataTypeName;
                bool hasName = dataTypeNameMap.TryGetValue(dataType, out dataTypeName);
                if (!hasName)
                    dataTypeName = string.Empty;
                if (!resultMap.ContainsKey(dataType))
                    resultMap.Add(dataType, dataTypeName);
            }

            return resultMap;
        }

        /// <summary>
        /// 获取回路号与名称的映射关系
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetLogicalDeviceIndexNameMap(List<int> logicIndexList)
        {
            Dictionary<int, string> resultMap = new Dictionary<int, string>();

            foreach (int logicIndex in logicIndexList)
            {
                string logicName = LocalResourceManager.GetInstance().GetString("1089", "Loop ID") + logicIndex.ToString();
                if (!resultMap.ContainsKey(logicIndex))
                    resultMap.Add(logicIndex, logicName);
            }

            return resultMap;
        }

        /// <summary>
        /// 根据私有方案中的ParaType获取相关名称
        /// </summary>
        /// <param name="paraType"></param>
        /// <returns></returns>
        public string GetParaTypeName(int paraType)
        {
            string resultStr = string.Empty;
            Dictionary<int, string> resultMap = GetParaTypeNameMap();
            resultMap.TryGetValue(paraType, out resultStr);
            if (string.IsNullOrWhiteSpace(resultStr))
                resultStr = string.Empty;
            return resultStr;
        }

        /// <summary>
        /// 获取定时记录类型和名称映射关系
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetParaTypeNameMap()
        {
            Dictionary<int, string> resultMap = new Dictionary<int, string>();

            resultMap.Add(1, LocalResourceManager.GetInstance().GetString("1215", "Data Log"));//普通定时记录
            resultMap.Add(2, LocalResourceManager.GetInstance().GetString("1216", "AnalogRealtime Source"));//模拟量实时测点
            resultMap.Add(3, LocalResourceManager.GetInstance().GetString("1217", "Setpoint"));//越限事件
            resultMap.Add(4, LocalResourceManager.GetInstance().GetString("1218", "Digital InRealtime Source"));//开关量实时测点
            resultMap.Add(5, LocalResourceManager.GetInstance().GetString("1219", "High-speed Data Recorder"));//高速定时记录
            
            return resultMap;
        }
    }
}