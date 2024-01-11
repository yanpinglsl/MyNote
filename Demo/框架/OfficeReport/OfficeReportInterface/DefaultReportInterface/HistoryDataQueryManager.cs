using OfficeReportInterface.DefaultReportInterface.IntelligentSafety;
using OfficeReportInterface.DefaultReportInterface.PowerQualityEventsOnly;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using CET.PecsNodeManage;
using DBInterfaceCommonLib;
using CSharpDBPlugin;
using ErrorCode = DBInterfaceCommonLib.ErrorCode;

namespace OfficeReportInterface
{

    /// <summary>
    /// 故障滤波信息与所查询通道在流中位置集合定义结构
    /// </summary>
    public struct WaveDataParamInfo
    {
        /// <summary>
        /// 故障滤波流
        /// </summary>
        public MemoryStream WaveData;

        /// <summary>
        /// 指定查询通道在流中序号
        /// </summary>
        public int InWaveChannelPos;

        /// <summary>
        /// Initializes a new instance of the WaveDataParamInfo struct
        /// </summary>
        /// <param name="memStream">流</param>
        /// <param name="inPos">序号</param>
        public WaveDataParamInfo(MemoryStream memStream, int inPos)
        {
            this.WaveData = memStream;
            this.InWaveChannelPos = inPos;
        }
    }

    /// <summary>
    /// 临时存储外界波形序号与其流中的数值对应位置结构
    /// </summary>
    public struct WaveChannelIndexAndPosInfo
    {
        /// <summary>
        /// 外部循环查询通道序号
        /// </summary>
        public int OutWaveChannelIndex;

        /// <summary>
        /// 流中通道所在位置
        /// </summary>
        public int InWaveChannelPos;

        /// <summary>
        /// Initializes a new instance of the WaveChannelIndexAndPosInfo struct
        /// </summary>
        /// <param name="outIndex">外部循环查询通道序号</param>
        /// <param name="inPos">流中通道所在位置</param>
        public WaveChannelIndexAndPosInfo(int outIndex, int inPos)
        {
            this.OutWaveChannelIndex = outIndex;
            this.InWaveChannelPos = inPos;
        }
    }

    #region 存储波形cfg和data文件的数据结构

    ///// <summary>
    ///// 波形通道数量信息
    ///// </summary>
    //public struct TTADInfo
    //{
    //    /// <summary>
    //    /// 波形通道总个数
    //    /// </summary>
    //    public int TotalChannelNum;

    //    /// <summary>
    //    /// 模拟量通道个数
    //    /// </summary>
    //    public string AChannelNum;

    //    /// <summary>
    //    /// 开关量通道个数
    //    /// </summary>
    //    public string DChannelNum;
    //}

    ///// <summary>
    ///// 模拟量通道信息，由##A决定有多少行，一行表示一个模拟量通道信息
    ///// </summary>
    //public struct AChannelInfo
    //{
    //    /// <summary>
    //    /// 通道系数；必填；实数；1～32个字符宽度；可使用标准浮点记法
    //    /// </summary>
    //    public double ChannelCoef;

    //    /// <summary>
    //    ///  通道偏移量；必填；实数；1～32个字符宽度；可使用标准浮点数记法
    //    /// （通道转换公式为ax+b， x取自.DAT文件， 单位就是uu）
    //    /// </summary>
    //    public double ChannelOff;
    //}

    ///// <summary>
    ///// 采样率信息
    ///// 用多个采样率，就有多少行“samp,endsamp”；
    ///// 用一个采样率，nrates填0，samp填0，endsamp填data文件中的最后一个采样序号
    ///// </summary>
    //public struct SampInfo
    //{
    //    /// <summary>
    //    /// 采用率，单位Hz，必填；实数；1～32字符宽度；可使用标准浮点数记法
    //    /// </summary>
    //    public double Samp;

    //    /// <summary>
    //    /// 用Samp采样率最后一次采样的序号；必填；整数；1～9999999999
    //    /// </summary>
    //    public int EndSamp;
    //}

    #endregion

    /// <summary>
    /// 历史数据查询分析管理类
    /// </summary>
    public class HistoryDataQueryManager 
    {
        /// <summary>
        /// 历史数据管理类唯一实例
        /// </summary>
        public static readonly HistoryDataQueryManager DataManager = new HistoryDataQueryManager();

        #region 公用辅助成员方法

        /// <summary>
        /// 查询指定节点下多参数-类型数据对象的定时记录数据结果，直接使用DataID作为参数
        /// </summary>
        /// <param name="queryParams">查询参数列表</param>
        /// <param name="startTime">查询起始时间</param>
        /// <param name="endTime">查询结束时间</param>
        /// <returns>查询结果值</returns>
        public List<List<DataLogOriDef>> QueryHistoryTrendDataLogView(List<DeviceDataIDDef> queryParams, DateTime startTime, DateTime endTime)
        {
            List<List<DataLogOriDef>> resultList = new List<List<DataLogOriDef>>();
            if (queryParams == null)
            {
                return resultList;
            }
            foreach(DeviceDataIDDef dataIDDef in queryParams)
            {
                List<DataLogOriDef> tempDT = new List<DataLogOriDef>();
                bool isSucess = DefaultTemplatePublicMethod.ReadDatalogs(startTime, endTime, dataIDDef, ref tempDT);
                if (!isSucess)
                {
                    continue;
                }
                resultList.Add(tempDT);
            }
            return resultList;
            ////创建并初始化结果集
            //resultList = new List<DataLogValueDef>[queryParams.Count];
            //for (int i = 0; i < queryParams.Count; i++)
            //{
            //    resultList[i] = new List<DataLogValueDef>();
            //}

            ////创建用于保存最终查询条件映射关系的缓存字段，保存SourceID-DataIndex数组对象的映射关系
            //Dictionary<uint, DataIDToMeasIDDef[]> sourceIDMap = new Dictionary<uint, DataIDToMeasIDDef[]>();
            ////对查询参数进行合并优化，拥有相同SourceID的DataIndex查询合并为一次查询
            //this.CombineQueryParamWithSourceID(sourceIDMap, queryParams);

            //DataTable tempDT = new DataTable();
            //try
            //{
            //    //基于最终的查询条件进行数据库查询,单SourceID进行查询
            //    foreach (KeyValuePair<uint, DataIDToMeasIDDef[]> kvp in sourceIDMap)
            //    {
            //        //SourceID查询条件
            //        uint sourceIDKey = kvp.Key;
            //        //DataIndex查询条件
            //        DataIDToMeasIDDef[] sourceValue = kvp.Value;

            //        //组合拥有相同SourceID的DataIndex参数
            //        List<int> queryDataIndexList = new List<int>();
            //        uint stationID = 0;
            //        foreach (DataIDToMeasIDDef sourceValueItem in sourceValue)
            //        {
            //            //有效的sourceID和DataIndex,形成DataIndex查询字符串
            //            if (sourceValueItem.SourceID > 0 && sourceValueItem.DataIndex > 0)
            //            {
            //                queryDataIndexList.Add(sourceValueItem.DataIndex);
            //                stationID = sourceValueItem.StationID == stationID ? stationID : sourceValueItem.StationID;
            //            }
            //        }
            //        //SourceID对应单DataIndex采用精确查询，对应多DataIndex采用全部查询,对查询结果进行过滤
            //        int dataIndex = queryDataIndexList.Count == 1 ? queryDataIndexList[0] : 0;
            //        if (queryDataIndexList.Count == 0 || stationID == 0)
            //        {
            //            continue;
            //        }

            //        tempDT.Clear();

            //        int resultCode = DatalogProvider.Instance.ReadDatalogs(DBOperationFlag.either, stationID,
            //            sourceIDKey, dataIndex, startTime, endTime, (int)SysConstDefinition.DefaultMaxRowCount, ref tempDT);

            //        DbgTrace.dout("调用数据库接口，查询定时记录。返回的错误字符串是:{0}；返回的错误码的值是：{1}。", DBInterfaceCommonLib.ErrorQuerier.Instance.GetLastErrorString(), resultCode);
            //        DbgTrace.dout("传入数据库接口的入参是：DBOperationFlag = {0} , stationID = {1} , sourceID = {2} , paraIndex = {3}, startTime = {4} , endTime.AddSeconds(1) = {5} , Convert.ToInt32(SysConstDefinition.DefaultMaxRowCount) = {6} 。", DBOperationFlag.either, stationID,   sourceIDKey, dataIndex, startTime, endTime, (int)SysConstDefinition.DefaultMaxRowCount);

            //        if (resultCode == (int) ErrorCode.Success)
            //        {
            //            ////遍历获取当前查询得到的原始值，将原始值放置于对应的查询结果列表中
            //            foreach (DataRow row in tempDT.Rows)
            //            {
            //                DateTime logtime =
            //                    Convert.ToDateTime(row["LogTime"]).AddMilliseconds(Convert.ToDouble(row["Msec"]));

            //                ////获取当前有效的查询参数，依据该参数进行查询结果的取值
            //                for (int index = 0; index < sourceValue.Length; index++)
            //                {
            //                    DataIDToMeasIDDef dataValueItem = sourceValue[index];
            //                    if (dataValueItem.SourceID > 0 && dataValueItem.DataIndex > 0)
            //                    {
            //                        ////查找定位当前参数对应的最终结果列表
            //                        List<DataLogValueDef> resultDataLogList = resultList[index];

            //                        ////如果是用于查询界面显示，则需要进行缺失处理，如果是导出则无需处理
            //                        ////查找定位当前参数的查询条件对象
            //                        DeviceDataIDDef targetDataIDDef = queryParams[index];

            //                        ////获取对应当前DataIndex的查询数据列名称,依据该名称进行取值
            //                        string queryColName = string.Format("Data{0}", dataValueItem.DataIndex);
            //                        double resultValue = double.NaN;
            //                        if (!Convert.IsDBNull(row[queryColName]))
            //                        {
            //                            double temp;
            //                            if (!Double.TryParse(row[queryColName].ToString(), out temp))
            //                                continue;
            //                            resultValue = temp * dataValueItem.Cofficient;
            //                            //进行无效值判断
            //                            if (SysConstDefinition.NA.CompareTo(resultValue) == 0)
            //                            {
            //                                resultValue = double.NaN;
            //                            }
            //                        }

            //                        resultValue = DataFormatManager.GetFormattedDoubleByDigits(resultValue, 3);
            //                        //进行查询结果的赋值,并将该查询结果对象加入至结果列表中
            //                        DataLogValueDef curDataLogValue = new DataLogValueDef(logtime, resultValue);
            //                        resultDataLogList.Add(curDataLogValue);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    DbgTrace.dout(ex.Message + ex.StackTrace);
            //}
            //finally
            //{
            //    if (tempDT != null)
            //    {
            //        tempDT.Dispose();
            //    }
            //}

            //return resultList;
        }

        public bool QueryOriginalWaveDataOfEvent(uint userID, SagSwellEvent item, int offset, out TotalCombinedWave totalCombinedWave)
        {
            totalCombinedWave = new TotalCombinedWave();
            if (!item.HaveWave)
                return false;
            List<string> comtradeWaveDataList = new List<string>();
            DateTime eventTime;
            if (!DateTime.TryParse(item.EventTime, out eventTime))
                return false;
            List<byte[]> dataFileList=new List<byte[]>();
            if (SafetyDataManager.hasIEMSWeb)
            {
                var result = PowerQualityDataManager.QueryOriginalWaveDataByDuraion(userID, item.DeviceID, item.EventTime, 0, 250, 250);
                if (!result.Success)
                    return false;
                foreach (var wave in result.ResultList)
                {
                    foreach (var oneWave in wave.WaveData)
                    {
                        dataFileList.Add(Convert.FromBase64String(oneWave));     
                    }
                }
            }
            else
            {
                if (!OriginalDataManager.DataManager.QueryOriginalWaveData(item.DeviceID, eventTime, offset, out dataFileList))
                    return false;
            }

            List<OneWaveData> oneWaveDataList = new List<OneWaveData>();
            foreach (var myBytes in dataFileList)
            {
                MemoryStream memStream = new MemoryStream(myBytes);
                OneWaveData oneWaveData;
                ComputeWaveData.GetOneWave(memStream, out oneWaveData);
                oneWaveDataList.Add(oneWaveData);
            }

            ComputeWaveData.GetDataForWave(oneWaveDataList, out totalCombinedWave);
            return true;
        }

        /// <summary>
        /// 根据电流电压排序分离
        /// </summary>
        /// <param name="resultData"></param>
        private void SortWaveDataByVoltAndCurrent(ResultWaveDataView resultData)
        {
            //电流电压分开
            List<int> voltChanelList = new List<int>();
            List<int> currentChanelList = new List<int>();
            for (int i = 0; i < resultData.ChannelNameList.Count; i++)
            {
                if (IsChannelVolt(resultData.ChannelNameList[i]))
                    voltChanelList.Add(i);
                else
                    currentChanelList.Add(i);
            }
            List<double[]> ChannelValList = new List<double[]>();
            List<string> ChannelNameList = new List<string>();
            for (int i = 0; i < voltChanelList.Count; i++)
            {
                ChannelValList.Add(resultData.ChannelValList[voltChanelList[i]]);
                ChannelNameList.Add(resultData.ChannelNameList[voltChanelList[i]]);
            }
            for (int i = 0; i < currentChanelList.Count; i++)
            {
                ChannelValList.Add(resultData.ChannelValList[currentChanelList[i]]);
                ChannelNameList.Add(resultData.ChannelNameList[currentChanelList[i]]);
            }
            resultData.ChannelValList = ChannelValList;
            resultData.ChannelNameList = ChannelNameList;
        }

        /// <summary>
        /// 判断通道是否为电压数据
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public static bool IsChannelVolt(string channelName)
        {
            if (channelName.ToUpper().Contains("U"))
                return true;
            if (channelName.ToUpper().Contains("V"))
                return true;
            return false;
        }

        #endregion

        #region 私有逻辑函数

        ///// <summary>
        ///// 根据查询参数对应获取对应的定时记录查询参数集合，并且按照SourceID进行查询参数合并
        ///// </summary>
        ///// <param name="sourceIDMap">定时记录查询映射集合</param>
        ///// <param name="queryParams">查询参数列表</param>
        //private void CombineQueryParamWithSourceID(Dictionary<uint, DataIDToMeasIDDef[]> sourceIDMap, List<DeviceDataIDDef> queryParams)
        //{
        //    for (int i = 0; i < queryParams.Count; i++)
        //    {
        //        DATALOG_PRIVATE_MAP resultMapDef = DataIDToMeasIDDef.InvalidMapDef;
        //        bool result = ReportWebServiceManager.ReportWebManager.FindDataMapDef(queryParams[i], out resultMapDef);
        //        if (result)
        //        {
        //            //创建映射值对象
        //            DataIDToMeasIDDef[] sourceMapValue = null;
        //            bool exist = sourceIDMap.TryGetValue(resultMapDef.SourceID, out sourceMapValue);
        //            if (exist)
        //            {
        //                //当前SourceID映射键已存在，则获取映射值对象，设置当前查询参数序号位置的映射值项
        //                sourceMapValue[i] = resultMapDef;
        //                //将设置后的映射值更新保存
        //                sourceIDMap[resultMapDef.SourceID] = sourceMapValue;
        //            }
        //            else
        //            {
        //                //当前SourceID映射键不存在，则新建映射值，有多少个参数，就设置多少个映射值项
        //                sourceMapValue = new DataIDToMeasIDDef[queryParams.Count];
        //                sourceMapValue[i] = resultMapDef;
        //                //加入映射关系中
        //                sourceIDMap.Add(resultMapDef.SourceID, sourceMapValue);
        //            }
        //        }
        //    }
        //}              

        ///// <summary>
        ///// 查询指定节点ID的原始事件波形数据集合
        ///// </summary>
        ///// <param name="pqNodeID">节点ID</param>
        ///// <param name="eventTime">事件时间</param>
        ///// <param name="millSec">毫秒标识</param>
        ///// <param name="resultList">结果集</param>
        ///// <returns>是否成功</returns>
        //private bool QueryOriginalWaveDataList(uint deviceID, DateTime eventTime, int millSec, ref List<MemoryStream> resultList)
        //{
        //    if (resultList == null)
        //    {
        //        resultList = new List<MemoryStream>();
        //    }

        //    //获取当前监测节点ID对应的设备节点
        //    PecsDeviceNode curDeviceNode = PecsNodeManager.PecsNodeInstance.GetDeviceNodeByID(deviceID);
        //    //当前设备节点存在
        //    if (curDeviceNode == null)
        //        return false;
        //    DataTable resultDT = new DataTable();
        //    uint stationID = curDeviceNode.ParentNode.ParentNode.NodeID;
        //    uint channelID = curDeviceNode.ParentNode.NodeID;
        //    int logType = 2;
        //    int logHandle = 0;
        //    int maxRowCount = Convert.ToInt32(SysConstDefinition.DefaultMaxRowCount);
        //    int errorCode = PECSDBInterface.OriginalLogProvider.Instance.ReadOriglogsByLogTime(DBOperationFlag.either, stationID, channelID, deviceID, logType, logHandle, eventTime, Convert.ToUInt32(millSec), maxRowCount, ref resultDT);
        //    if (errorCode != (int)ErrorCode.Success)
        //        return false;
        //    int tempLogHandle = 100;
        //    bool isExistLogHandleOver100 = false;
        //    //如果存在多条波形，取loghandle最小的
        //    if (resultDT.Rows.Count > 1)
        //    {
        //        for (int i = 0; i < resultDT.Rows.Count; i++)
        //        {
        //            logHandle = Convert.ToInt32(resultDT.Rows[i]["logHandle"]);
        //            if (logHandle >= 100)
        //                isExistLogHandleOver100 = true;
        //            if (logHandle < tempLogHandle)
        //                tempLogHandle = logHandle;
        //        }
        //    }

        //    for (int i = 0; i < resultDT.Rows.Count; i++)
        //    {
        //        //如果同时存在Loghandle大于100和小于100的，只取大于100的，否则去loghandle最小的
        //        logHandle = Convert.ToInt32(resultDT.Rows[i]["logHandle"]);
        //        if (isExistLogHandleOver100)
        //        {
        //            if (logHandle < 100)
        //                continue;
        //        }
        //        else if (resultDT.Rows.Count>1)//不存在大于100的但是有多条，就取loghandle最小的
        //        {
        //            if (logHandle != tempLogHandle)
        //                continue;
        //        }

        //        byte[] waveData = null;
        //        if (!Convert.IsDBNull(resultDT.Rows[i]["Datafile"]))
        //        {
        //            waveData = (byte[])resultDT.Rows[i]["Datafile"];
        //        }
        //        if (waveData != null)
        //        {
        //            MemoryStream memStream = new MemoryStream(waveData);
        //            resultList.Add(memStream);
        //        }
        //    }
        //    return true;
        //}

        #endregion
    }
}
