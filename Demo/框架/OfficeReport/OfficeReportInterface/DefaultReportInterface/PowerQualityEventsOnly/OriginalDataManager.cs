using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using CET.PecsNodeManage;
using CSharpDBPlugin;
using DBInterfaceCommonLib;
using ErrorCode = DBInterfaceCommonLib.ErrorCode;

namespace OfficeReportInterface
{

    /// <summary>
    ///原始波形记录管理类
    /// </summary>
    public class OriginalDataManager
    {
        public OriginalDataManager()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 事件记录管理类唯一实例
        /// </summary>
        public static readonly OriginalDataManager DataManager = new OriginalDataManager();

        /// <summary>
        /// 已被删除的设备缓存信息（用于波形查找）
        /// </summary>
        public Dictionary<uint, StaChaDevID> DeletedDeviceMap = new Dictionary<uint, StaChaDevID>();
        /// <summary>
        /// 查询波形
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="waveTime"></param>
        /// <param name="offset">如果为0则精确查找</param>
        /// <param name="dataFileList"></param>
        /// <returns></returns>
        public bool QueryOriginalWaveData(uint deviceID, DateTime waveTime, int offset, out List<byte[]> dataFileList)
        {
            dataFileList = new List<byte[]>();
            uint stationID = SystemNodeManager.DataManager.FindStationID(deviceID);
            uint channelID = SystemNodeManager.DataManager.FindChannelID(deviceID);
            //在节点中没有找到厂站和通道，则在被删除的设备信息中找
            if (stationID == 0 || channelID == 0)
            {
                StaChaDevID stadev;
                bool exists = DeletedDeviceMap.TryGetValue(deviceID, out stadev);
                if (exists)
                {
                    stationID = stadev.StationID;
                    channelID = stadev.ChannelID;
                }
                else
                    return false;
            }

            if (offset == 0)
               return  QueryOriginalWaveDataByTime(stationID, channelID, deviceID, waveTime,out dataFileList);
            else
                return QueryOriginalWaveDataByDuraion(stationID, channelID, deviceID, waveTime, offset,out dataFileList);
        }
        /// <summary>
        /// 查询波形
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="waveTime"></param>
        /// <param name="offset">如果为0则精确查找</param>
        /// <returns></returns>
        //public List<OriginalWaveInfo> QueryOriginalWaveData(uint deviceID, DateTime waveTime, int offset)
        //{
        //    uint stationID = SystemNodeManager.DataManager.FindStationID(deviceID);
        //    uint channelID = SystemNodeManager.DataManager.FindChannelID(deviceID);
        //    //在节点中没有找到厂站和通道，则在被删除的设备信息中找
        //    if (stationID == 0 || channelID == 0)
        //    {
        //        StaChaDevID stadev;
        //        bool exists = DeletedDeviceMap.TryGetValue(deviceID, out stadev);
        //        if (exists)
        //        {
        //            stationID = stadev.StationID;
        //            channelID = stadev.ChannelID;
        //        }
        //        else
        //            return null;
        //    }

        //    if (offset == 0)
        //        return QueryOriginalWaveDataByTime(stationID, channelID, deviceID, waveTime);
        //    else
        //        return QueryOriginalWaveDataByDuraion(stationID, channelID, deviceID, waveTime, offset);
        //}
       
             /// <summary>
        /// 查询指定节点ID的原始事件波形数据集合，指定查询时刻和偏差毫秒数，由于存储过程中已经对波形记录进行了升序排列，所以在这里就不需要进行排序了
        /// </summary>   
        /// <param name="deviceID">设备</param>
        /// <param name="waveTime">波形触发时间（包括毫秒数）</param>  
        /// <param name="offset">偏移毫秒数，左右偏移</param>
        /// <returns></returns>
        public bool QueryOriginalWaveDataByDuraion(uint stationID, uint channelID, uint deviceID, DateTime waveTime, int offset, out List<byte[]> dataFileList)
        {
            dataFileList = new List<byte[]>();  

            DataTable resultDT = new DataTable();
            //先进行精确匹配，如果匹配不到再进行偏移匹配
            QueryOriginalWaveDataByTime(stationID, channelID, deviceID, waveTime, out dataFileList);
            if (dataFileList.Count != 0)
                return true;

            //根据偏差生成查询起始与结束时刻
            DateTime startTime = waveTime.AddMilliseconds(Convert.ToDouble(-1*offset));
            DateTime endTime = waveTime.AddMilliseconds(Convert.ToDouble(offset));
            int startMsec = startTime.Millisecond;
            int endMsec = endTime.Millisecond;
            try
            {
                int resultCode = WformLogProvider.Instance.ReadWformLog(0, stationID, channelID, deviceID, 2, 0, startTime, startMsec, endTime, endMsec, true, Convert.ToInt32(SysConstDef.DefaultMaxRowCount), ref resultDT);
                if (resultCode == (int)ErrorCode.Success)
                {
                    FormatComtradeData(resultDT,out dataFileList);
                    return true;
                }
                else
                {
                    ErrorInfoManager.Instance.WriteDBInterfaceLog(resultCode, "OriginalLogProvider2.Instance.ReadOriglogs");
                    return false;
                }       
            }
            finally
            {
                if (resultDT != null)
                    resultDT.Dispose();
            }
        }
        /// <summary>
        /// 查询指定节点ID的原始事件波形数据集合，指定查询时刻和偏差毫秒数，由于存储过程中已经对波形记录进行了升序排列，所以在这里就不需要进行排序了
        /// </summary>   
        /// <param name="deviceID">设备</param>
        /// <param name="waveTime">波形触发时间（包括毫秒数）</param>  
        /// <param name="offset">偏移毫秒数，左右偏移</param>
        /// <returns></returns>
        //public List<OriginalWaveInfo> QueryOriginalWaveDataByDuraion(uint stationID, uint channelID, uint deviceID, DateTime waveTime, int offset)
        //{
        //    List<OriginalWaveInfo> resultList = new List<OriginalWaveInfo>();           

        //    DataTable resultDT = new DataTable();
        //    //先进行精确匹配，如果匹配不到再进行偏移匹配
        //    resultList = QueryOriginalWaveDataByTime(stationID, channelID, deviceID, waveTime);
        //    if (resultList.Count != 0)
        //        return resultList;

        //    //根据偏差生成查询起始与结束时刻
        //    DateTime startTime = waveTime.AddMilliseconds(Convert.ToDouble(-1*offset));
        //    DateTime endTime = waveTime.AddMilliseconds(Convert.ToDouble(offset));
        //    int startMsec = startTime.Millisecond;
        //    int endMsec = endTime.Millisecond;
        //    try
        //    {
        //        int resultCode = OriginalLogProvider2.Instance.ReadOriglogs(DBOperationFlag.either, stationID, channelID, deviceID, 2, 0, startTime, startMsec, endTime, endMsec, true, Convert.ToInt32(SysConstDef.DefaultMaxRowCount), ref resultDT);
        //        if (resultCode == (int)ErrorCode.Success)
        //            resultList = FormatComtradeData(resultDT);
        //        else
        //            ErrorInfoManager.Instance.WriteDBInterfaceLog(resultCode, "OriginalLogProvider2.Instance.ReadOriglogs");
        //    }
        //    finally
        //    {
        //        if (resultDT != null)
        //            resultDT.Dispose();
        //    }

        //    return resultList;
        //}
       
               /// <summary>
        /// 根据设备ID和波形时间精确查找波形记录
        /// </summary>    
        /// <param name="deviceID">设备</param>
        /// <param name="waveTime">波形时间</param>
        /// <param name="mSec">波形时间毫秒数</param>
        /// <returns></returns>
        public bool QueryOriginalWaveDataByTime(uint stationID, uint channelID, uint deviceID, DateTime waveTime, out List<byte[]> dataFileList)
        {
            DataTable resultDT = new DataTable();
            dataFileList = new List<byte[]>();
            try
            {
                int resultCode = WformLogProvider.Instance.ReadWformLogByLogTime(0, stationID, channelID, deviceID, 2, 0, waveTime, waveTime.Millisecond, Convert.ToInt32(SysConstDef.DefaultMaxRowCount), ref resultDT);
                if (resultCode != (int)CSharpDBPlugin.ErrorCode.Success)
                {
                    ErrorInfoManager.Instance.WriteDBInterfaceLog(resultCode, "OriginalLogProvider2.Instance.ReadOriglogs");
                    return false;

                }
                else
                {
                    FormatComtradeData(resultDT, out dataFileList);
                    return true;
                }
            }
            finally
            {
                if (resultDT != null)
                    resultDT.Dispose();
            }
        }
        /// <summary>
        /// 根据设备ID和波形时间精确查找波形记录
        /// </summary>    
        /// <param name="deviceID">设备</param>
        /// <param name="waveTime">波形时间</param>
        /// <param name="mSec">波形时间毫秒数</param>
        /// <returns></returns>
        //public List<OriginalWaveInfo> QueryOriginalWaveDataByTime(uint stationID, uint channelID, uint deviceID, DateTime waveTime)
        //{
        //    List<OriginalWaveInfo> resultList = new List<OriginalWaveInfo>();       

        //    DataTable resultDT = new DataTable();
        //    try
        //    {
        //        int resultCode = OriginalLogProvider.Instance.ReadOriglogsByLogTime(DBOperationFlag.either, stationID, channelID, deviceID, 2, 0, waveTime, (uint)waveTime.Millisecond, Convert.ToInt32(SysConstDef.DefaultMaxRowCount), ref resultDT);
        //        if (resultCode == (int)ErrorCode.Success)
        //            resultList = FormatComtradeData(resultDT);
        //        else
        //            ErrorInfoManager.Instance.WriteDBInterfaceLog(resultCode, "OriginalLogProvider2.Instance.ReadOriglogs");
        //    }
        //    finally
        //    {
        //        if (resultDT != null)
        //            resultDT.Dispose();
        //    }

        //    return resultList;
        //}
         

               /// <summary>
        /// 将符合条件的波形记录的二进制数据进行格式化后传给客户端
        /// </summary>
        /// <param name="resultDT"></param>
        /// <returns></returns>
        private void FormatComtradeData(DataTable resultDT, out List<byte[]> dataFileList)
        {
            dataFileList = new List<byte[]>();
            //同一触发时间，logHandle大于100的才合并，其它情况不合并
            List<OriginalWaveInfo> result = new List<OriginalWaveInfo>();
            foreach (DataRow row in resultDT.Rows)
            {
                if (Convert.IsDBNull(row["Datafile"]))
                    continue;
                byte[] dataFile = (byte[])row["Datafile"];
                dataFileList.Add(dataFile);
            }
        }

        /// <summary>
        /// 将符合条件的波形记录的二进制数据进行格式化后传给客户端
        /// </summary>
        /// <param name="resultDT"></param>
        /// <returns></returns>
        //private List<OriginalWaveInfo> FormatComtradeData(DataTable resultDT)
        //{
        //    //同一触发时间，logHandle大于100的才合并，其它情况不合并
        //    List<OriginalWaveInfo> result = new List<OriginalWaveInfo>();
        //    foreach (DataRow row in resultDT.Rows)
        //    {
        //        if (Convert.IsDBNull(row["Datafile"]))
        //            continue;

        //        byte[] dataFile = (byte[])row["Datafile"];

        //        ResultWaveDataView resultData = new ResultWaveDataView(DateTime.MinValue, DateTime.MinValue, 0, 0);
        //        //解析comtrade波形文件
        //        MemoryStream memStream = new MemoryStream(dataFile);
        //        List<string> comtradeWaveDataList = WaveDataDecodeManager.ParseWaveData(memStream);
        //        WaveDataDecodeManager.GetQueryOriginalWaveData(resultData, comtradeWaveDataList);

        //        // 进行Base64编码，将二进制数据转换为字符串，这样可以在XML进行传输
        //        String waveStr = Convert.ToBase64String(dataFile);
        //        OriginalWaveInfo resultItem = new OriginalWaveInfo();
        //        DateTime triggerTime = Convert.ToDateTime(row["LogTime"]);
        //        triggerTime = triggerTime.AddMilliseconds(Convert.ToInt32(row["Msec"]));
        //        resultItem.WaveID = Convert.ToUInt32(row["ID"]); //保存起来，用于客户端根据ID判断是否该波形已显示过
        //        resultItem.WaveTime = DataFormatManager.GetFormatTimeString(triggerTime);
        //        //如果有波形采样率则赋值，否则为0，客户端根据返回值进行显示，如果是0则显示为--
        //        resultItem.CountPerCycle = resultData.CountPerCycleAndIndexList.Count == 0 ? 0 : resultData.CountPerCycleAndIndexList[0].CountPerCycle;

        //        resultItem.LogHandle = Convert.ToInt32(row["logHandle"]);
        //        OriginalWaveInfo targetWave = new OriginalWaveInfo();
        //        //如果同一触发时间的波形记录已存在，且两者的logHandle都大于100，则合并
        //        if (IsTimeExist(result, resultItem.WaveTime, ref targetWave) && targetWave.LogHandle > 100 && resultItem.LogHandle > 100)
        //        {
        //            targetWave.WaveData.Add(waveStr);
        //        }
        //        else //其它情况不合并
        //        {                    
        //            resultItem.WaveData = new List<string>();
        //            resultItem.WaveData.Add(waveStr);
        //            result.Add(resultItem);
        //        }
        //    }
        //    return result;
        //}

        /// <summary>
        /// 判断是否已存在同一触发时间的波形记录，如果存在则返回true
        /// </summary>
        /// <param name="result"></param>
        /// <param name="waveTime"></param>
        /// <returns></returns>
        private bool IsTimeExist(List<OriginalWaveInfo> result, string waveTime, ref OriginalWaveInfo targetWave)
        {
            foreach (var item in result)
            {
                if (item.WaveTime == waveTime && item.LogHandle > 100)
                {
                    targetWave = item;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 查询获取指定波形通道参数ID的波形映射名称，可查询全部参数的结果
        /// </summary>
        /// <param name="PQNodeID">监测节点ID</param>
        /// <param name="waveChannelIDList">波形通道ID列表，如果为空则表示查询所有</param>
        /// <returns></returns>
        public List<DataIDDef> GetWaveChannelCfgNameByNodeID(uint stationID, uint channelID, uint deviceID, List<uint> waveChannelIDList)
        {
            List<DataIDDef> resultList = new List<DataIDDef>();
            if (waveChannelIDList.Count == 0)
            {
                for (int i = 0; i < PecsNodeManager.PecsNodeInstance.WaveIDNum; i++)
                    waveChannelIDList.Add(PecsNodeManager.PecsNodeInstance.GetWaveChannelDef(i).DataID);
            }

            PecsDeviceNode targetNode = PecsNodeManager.PecsNodeInstance.GetDeviceNodeByID(stationID, channelID, deviceID);
            if (targetNode == null)
                return resultList;

            int waveMapID = targetNode.WaveMapID;
            foreach (uint waveID in waveChannelIDList)
            {
                string cfgName = PecsNodeManager.PecsNodeInstance.GetWaveMapResult(waveMapID, waveID);
                DataIDDef newWaveNameItem = new DataIDDef(waveID, cfgName);
                resultList.Add(newWaveNameItem);
            }

            return resultList;
        }

        /// <summary>
        /// 获取厂站通道设备与波形记录相关时间的映射关系
        /// </summary>
        /// <param name="staChaDevID">查询厂站通道设备ID，厂站ID不能为0，其他为0时表示该参数不参与查询</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="offset">允许的偏移时间，毫秒</param>
        /// <returns></returns>
        public Dictionary<StaChaDevID, List<DateTime>> GetComtradMap(StaChaDevID staChaDevID, DateTimeParam timeParam, int offset)
        {
            Dictionary<StaChaDevID, List<DateTime>> idComtradMap = new Dictionary<StaChaDevID, List<DateTime>>();
            if (staChaDevID.StationID == 0)
                return idComtradMap;

            DateTime startTimeForQuery = timeParam.StartTime.AddMilliseconds(Convert.ToDouble(-1*offset));
            DateTime endTimeForQuery = timeParam.EndTime.AddMilliseconds(Convert.ToDouble(offset));
            DataTable resultDT = new DataTable();
            int errorCode = WformLogProvider.Instance.ReadWformLog(0, staChaDevID.StationID, staChaDevID.ChannelID, staChaDevID.DeviceID, 2, 0, startTimeForQuery, startTimeForQuery.Millisecond, endTimeForQuery, endTimeForQuery.Millisecond, false, 0, ref resultDT);
            if (errorCode != (int)ErrorCode.Success)
                ErrorInfoManager.Instance.WriteDBInterfaceLog(errorCode, "OriginalLogProvider2.Instance.ReadOriglogs");
            idComtradMap = ConcreateComtradMap(resultDT);            
            return idComtradMap;
        }        

        /// <summary>
        /// 获取厂站通道设备与波形记录相关时间的映射关系（根据设备列表查找）
        /// </summary>
        /// <param name="deviceIDs">设备ID列表</param>
        /// <param name="timeParam">起始时间-结束时间</param>
        /// <param name="offset">允许的偏移时间，毫秒</param>
        /// <returns></returns>
        public Dictionary<StaChaDevID, List<DateTime>> GetComtradMap(List<NodeDataParam> nodes, DateTimeParam timeParam, int offset)
        {
          

            DateTime startTimeForQuery = timeParam.StartTime.AddMilliseconds(Convert.ToDouble(-1 * offset));
            DateTime endTimeForQuery = timeParam.EndTime.AddMilliseconds(Convert.ToDouble(offset));
            //DataTable resultDT = new DataTable();
            ////NodeDataParam[] nodes, int logType, int logHandle, DateTime startTime, DateTime endTime, bool includeData, int maxRowCount, ref DataTable queryResult)
            //EMSWebServiceProvider.Instance.QueryOriDatasByNodes(nodes.ToArray(), 2, 0, startTimeForQuery, endTimeForQuery, false, 0, ref resultDT);
            
            
            
           
            //idComtradMap = ConcreateComtradMap(resultDT);

            Dictionary<StaChaDevID, List<DateTime>> idComtradMap;
            WaveManager waveManager = new WaveManager();
            waveManager.GetAllWave(nodes, startTimeForQuery, endTimeForQuery, out idComtradMap);
            return idComtradMap;
        }      

        /// <summary>
        /// 在查询的时间范围内是否有波形记录
        /// </summary>
        /// <param name="commuID">厂站通道设备ID</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="offset">偏移毫秒数</param>
        /// <param name="idComtradMap">波形记录映射关系</param>
        /// <returns>有波形返回TRUE,否则返回False</returns>
        public bool HasComtradeFile(StaChaDevID commuID, DateTime startTime, int offset, Dictionary<StaChaDevID, List<DateTime>> idComtradMap)
        {
            bool hasComtradFile = false;         

            List<DateTime> comtradTimeList;
            bool hasComtradTime = idComtradMap.TryGetValue(commuID, out comtradTimeList);
            if (hasComtradTime)
            {
                DateTime myStart = startTime.AddMilliseconds(Convert.ToDouble(-1*offset));
                DateTime myEnd = startTime.AddMilliseconds(Convert.ToDouble(offset));
                foreach (DateTime timeItem in comtradTimeList)
                {
                    if (timeItem >= myStart && timeItem <= myEnd)
                    {
                        hasComtradFile = true;
                        break;
                    }
                }
            }
            return hasComtradFile;
        }

        /// <summary>
        /// 根据结果集组建波形映射关系
        /// </summary>
        /// <param name="resultDT"></param>
        /// <returns></returns>
        private Dictionary<StaChaDevID, List<DateTime>> ConcreateComtradMap(DataTable resultDT)
        {
            Dictionary<StaChaDevID, List<DateTime>> idComtradMap = new Dictionary<StaChaDevID, List<DateTime>>();
            foreach (DataRow dr in resultDT.Rows)
            {
                StaChaDevID commuIDItem = new StaChaDevID();
                commuIDItem.StationID = Convert.ToUInt32(dr["StationID"]);
                commuIDItem.ChannelID = Convert.ToUInt32(dr["ChannelID"]);
                commuIDItem.DeviceID = Convert.ToUInt32(dr["DeviceID"]);

                DateTime logtime = Convert.ToDateTime(dr["Logtime"]);
                logtime = logtime.AddMilliseconds(Convert.ToDouble(dr["MSec"]));

                if (!idComtradMap.ContainsKey(commuIDItem))
                    idComtradMap[commuIDItem] = new List<DateTime>();

                idComtradMap[commuIDItem].Add(logtime);
            }
            return idComtradMap;
        }
    }
}