using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using BasicDataInterface;
using BasicDataInterface.Models.Query;
using BasicDataInterface.Models.Response;
using CET.PecsNodeManage;
using CSharpDBPlugin;
using DBInterfaceCommonLib;
using DBInterfaceModels;
using Newtonsoft.Json;
using ServiceStack.Web;
using ErrorCode = DBInterfaceCommonLib.ErrorCode;
using ErrorQuerier = DBInterfaceCommonLib.ErrorQuerier;

namespace OfficeReportInterface
{
    /// <summary>
    /// EMSWebService的后台公用管理类
    /// 主要用于节点初始化、节点映射缓存等操作
    /// </summary>
    public class EMSWebServiceManager
    {
        /// <summary>
        /// 全局唯一单例
        /// </summary>
        public static readonly EMSWebServiceManager EMSWebManager = new EMSWebServiceManager();

        #region 成员对象

        ///// <summary>
        ///// 通信设备和参数ID-DataID-DataType至实时测点-定时记录SourceID的映射关系集合
        ///// </summary>
        //private Dictionary<DeviceDataParam, DataIDToMeasIDDef> deviceDataIDToMeasIDMap;

        /// <summary>
        /// 通信设备和参数ID-DataID-DataType至DATALOG_PRIVATE_MAP的映射关系集合，用于PecCore4.0的定时记录数据的查询
        /// </summary>
        public Dictionary<DeviceDataParam, DATALOG_PRIVATE_MAP> deviceDataIDToPrivateMap = new Dictionary<DeviceDataParam, DATALOG_PRIVATE_MAP>();

        /// <summary>
        /// 设备ID与定时记录私有映射关系表记录的字典
        /// </summary>
        private Dictionary<uint, List<DATALOG_PRIVATE_MAP>> deviceDatalogPrivateValueMap = new Dictionary<uint, List<DATALOG_PRIVATE_MAP>>();

        /// <summary>
        /// 基本参数到所有参数列表的映射关系
        /// </summary>
        private Dictionary<DevDataIDType, List<DeviceDataParam>> devDataToAllDataMap = new Dictionary<DevDataIDType, List<DeviceDataParam>>();
        /// <summary>
        /// 基本参数到所有参数列表的映射关系
        /// </summary>
        private Dictionary<DevDataID, List<DeviceDataParam>> devDataOnlyToAllDataMap = new Dictionary<DevDataID, List<DeviceDataParam>>();
        /// <summary>
        /// 保存所有sourceID需要更新的设备的映射，针对web建的设备的定时记录sourceID需要front运行一段时间后才会写入sourceID的情况，程序检测sourceID自动更新
        /// </summary>
        public Dictionary<StaChaDevID, StaChaDevID> sourceIDNeedUpdateDevices = new Dictionary<StaChaDevID, StaChaDevID>();

        #endregion

        #region 接口函数

        /// <summary>
        /// WebService后台的全局初始化函数
        /// 初始化数据库连接，加载节点以及解析节点映射关系等
        /// </summary>
        /// <returns>初始化结果</returns>
        public EMSErrorMsg InitializeWebService()
        {
            EMSErrorMsg resultMsg = new EMSErrorMsg(true);

            //初始化加载定时记录映射            
            //deviceDataIDToMeasIDMap = new Dictionary<DeviceDataParam, DataIDToMeasIDDef>();
            deviceDataIDToPrivateMap = new Dictionary<DeviceDataParam, DATALOG_PRIVATE_MAP>();
            deviceDatalogPrivateValueMap = new Dictionary<uint, List<DATALOG_PRIVATE_MAP>>();
            devDataToAllDataMap = new Dictionary<DevDataIDType, List<DeviceDataParam>>();
            //List<StaChaDevID> updatedDevices = new List<StaChaDevID>();
            //resultMsg = LoadDataIDToMeasureIDMap(ref updatedDevices);
            //if (!resultMsg.Success)
            //{
            //    WriteInitialLog(resultMsg, "LoadDataIDToMeasureIDMap");
            //    return resultMsg;
            //}
            SystemNodeManager.DataManager.GetStationTimeZoneInfo();
            return resultMsg;
        }

        ///// <summary>
        ///// 初始化参数信息-sourceid的映射关系
        ///// </summary>
        ///// <param name="sysNode"></param>
        //private EMSErrorMsg LoadDataIDToMeasureIDMap(ref List<StaChaDevID> updatedDevices)
        //{
        //    EMSErrorMsg resultValue = new EMSErrorMsg(true);
        //    //try
        //    //{
        //    //    resultValue = ExecuteDataIDMapByDeviceID(ref updatedDevices);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    resultValue.Success = false;
        //    //    resultValue.ErrorMsgInstance = "LoadDataIDToMeasureIDMap";
        //    //    resultValue.ErrorMessage = ex.Message;
        //    //}
        //    return resultValue;
        //}

        /// <summary>
        /// 将错误信息写入日志文件中
        /// </summary>
        /// <param name="resultMsg"></param>
        private void WriteInitialLog(EMSErrorMsg resultMsg, string errorInstance)
        {
            if (string.IsNullOrEmpty(resultMsg.ErrorMsgInstance))
                resultMsg.ErrorMsgInstance = errorInstance;
            string errorStr = string.Format("ErrorInstance:{0}, ErrorMsg:{1}", resultMsg.ErrorMsgInstance, resultMsg.ErrorMessage);
            ErrorInfoManager.Instance.WriteLogMessage(errorStr);
        }

        public void GetDicFromRedis<R, T>(string key, ref Dictionary<R, T> dic)
        {
            dic = new Dictionary<R, T>();
            List<string> values = RedisWrap.GetAllItemsFromList(key);
            foreach (var value in values)
            {
                KeyValuePair<R, T> kvp = JsonConvert.DeserializeObject<KeyValuePair<R, T>>(value);
                dic[kvp.Key] = kvp.Value;
            }
        }

        public void SaveDicToRedis<R, T>(string key, Dictionary<R, T> dic, TimeSpan slidingExpiration)
        {
            List<string> values = new List<string>();
            foreach (var kvp in dic)
            {
                values.Add(JsonConvert.SerializeObject(kvp));
            }
            RedisWrap.AddRangeToList(key, values, slidingExpiration);
        }

        /// <summary>
        /// 获取当前所有设备的更新时间，用于与Redis缓存里面的更新时间进行对比，判断是否有更新
        /// </summary>
        /// <returns></returns>
        private static Dictionary<StaChaDevID, DateTime> GetDeviceUpdateTimeDic()
        {
            Dictionary<StaChaDevID, DateTime> dic = new Dictionary<StaChaDevID, DateTime>();
            //逐个厂站获取所有设备节点
            for (int i = 0; i < PecsNodeManager.PecsNodeInstance.StationNum; i++)
            {
                PecsStationNode stationNode = (PecsStationNode) SystemMeasPointGNode.SystemConfigNode.Find(i);
                if (stationNode.NodeID == 0)
                    continue;

                foreach (var communicationNode in stationNode.GetChildrenNodes())
                {
                    if (communicationNode.NodeType != PecsNodeType.PECSCOMMUNI_NODE)
                        continue;

                    foreach (var channelNode in communicationNode.GetChildrenNodes())
                    {
                        if (channelNode.NodeID == 0) //必须判断，否则会出现通道ID为0的情况，在长江证券数据库中出现过，暂不清楚原因
                            continue;
                        foreach (var deviceNode in channelNode.GetChildrenNodes())
                        {
                            dic[new StaChaDevID(stationNode.NodeID, channelNode.NodeID, deviceNode.NodeID)] = deviceNode.NodeUpdateTime;
                        }
                    }
                }
            }
            return dic;
        }

        ///// <summary>
        ///// 获取有配置更新的设备
        ///// </summary>
        ///// <returns></returns>
        //public List<StaChaDevID> GetUpdatedDevices()
        //{
        //    List<StaChaDevID> result = new List<StaChaDevID>();
        //    if (!RedisWrap.Exists(RedisWrap.key_DeviceUpdateTime))
        //        return result;

        //    Dictionary<StaChaDevID, DateTime> dicFromPecsNodeManager = GetDeviceUpdateTimeDic();
        //    Dictionary<StaChaDevID, DateTime> dicFromRedis = new Dictionary<StaChaDevID, DateTime>();
        //    EMSWebServiceManager.EMSWebManager.GetDicFromRedis<StaChaDevID, DateTime>(RedisWrap.key_DeviceUpdateTime, ref dicFromRedis);

        //    foreach (var kvp in dicFromPecsNodeManager)
        //    {
        //        if (dicFromRedis.ContainsKey(kvp.Key))
        //        {
        //            if (kvp.Value != dicFromRedis[kvp.Key])
        //                result.Add(kvp.Key);
        //        }
        //        else //说明是新增的设备，需要生成缓存
        //            result.Add(kvp.Key);
        //    }
        //    if (result.Count != 0) //说明有配置更新，将设备配置更新时间写入redis缓存，在下次加载的时候对比更新时间，如果有配置更新则只针对有更新的设备进行更新，并更新到redis中 
        //        EMSWebServiceManager.EMSWebManager.SaveDicToRedis(RedisWrap.key_DeviceUpdateTime, dicFromPecsNodeManager, TimeSpan.MaxValue);

        //    return result;
        //}

        ///// <summary>
        ///// 为指定的通信设备进行DataID映射解析
        ///// </summary>
        ///// <param name="dataIDParams">参数列表</param>
        //private EMSErrorMsg ExecuteDataIDMapByDeviceID(ref List<StaChaDevID> updatedDevices)
        //{
        //    EMSErrorMsg resultValue = new EMSErrorMsg(true);
        //    string Key_deviceDatalogPrivateValueMap = RedisWrap.Key_deviceDatalogPrivateValueMap;
        //    string Key_devDataToAllDataMap = RedisWrap.Key_devDataToAllDataMap;
        //    string Key_deviceDataIDToMeasIDMap = RedisWrap.Key_deviceDataIDToMeasIDMap;
        //    if (RedisWrap.Exists(Key_deviceDatalogPrivateValueMap) && RedisWrap.Exists(Key_devDataToAllDataMap) && RedisWrap.Exists(Key_deviceDataIDToMeasIDMap))
        //    {
        //        GetDicFromRedis<uint, List<DATALOG_PRIVATE_MAP>>(Key_deviceDatalogPrivateValueMap, ref deviceDatalogPrivateValueMap);
        //        GetDicFromRedis<DevDataIDType, List<DeviceDataParam>>(Key_devDataToAllDataMap, ref devDataToAllDataMap);
        //        GetDicFromRedis<DeviceDataParam, DataIDToMeasIDDef>(Key_deviceDataIDToMeasIDMap, ref deviceDataIDToMeasIDMap);

        //        //检查定时记录映射方案是否有更新
        //        updatedDevices = GetUpdatedDevices();
        //        if (updatedDevices.Count != 0) //
        //        {
        //            foreach (var staChaDevId in updatedDevices)
        //            {
        //                //先删除旧的映射方案
        //                deviceDatalogPrivateValueMap = deviceDatalogPrivateValueMap.Where(kvp => (kvp.Key != staChaDevId.DeviceID)).ToDictionary(a => a.Key, a => a.Value);
        //                devDataToAllDataMap = devDataToAllDataMap.Where(kvp => (kvp.Key.DeviceID != staChaDevId.DeviceID)).ToDictionary(a => a.Key, a => a.Value);
        //                deviceDataIDToMeasIDMap = deviceDataIDToMeasIDMap.Where(kvp => (kvp.Key.DeviceID != staChaDevId.DeviceID)).ToDictionary(a => a.Key, a => a.Value);
        //                //再增加新的映射方案

        //                Dictionary<uint, List<DATALOG_PRIVATE_MAP>> deviceDatalogPrivateValueMap1 = new Dictionary<uint, List<DATALOG_PRIVATE_MAP>>();
        //                Dictionary<DevDataIDType, List<DeviceDataParam>> devDataToAllDataMap1 = new Dictionary<DevDataIDType, List<DeviceDataParam>>();
        //                Dictionary<DeviceDataParam, DataIDToMeasIDDef> deviceDataIDToMeasIDMap1 = new Dictionary<DeviceDataParam, DataIDToMeasIDDef>();
        //                resultValue = ReadDatalogPrivateMap(staChaDevId.StationID, staChaDevId.ChannelID, staChaDevId.DeviceID, ref deviceDatalogPrivateValueMap1, ref devDataToAllDataMap1, ref deviceDataIDToMeasIDMap1);
        //                if (!resultValue.Success)
        //                    return resultValue;

        //                deviceDatalogPrivateValueMap = deviceDatalogPrivateValueMap.Union(deviceDatalogPrivateValueMap1).ToDictionary(a => a.Key, a => a.Value);
        //                devDataToAllDataMap = devDataToAllDataMap.Union(devDataToAllDataMap1).ToDictionary(a => a.Key, a => a.Value);
        //                deviceDataIDToMeasIDMap = deviceDataIDToMeasIDMap.Union(deviceDataIDToMeasIDMap1).ToDictionary(a => a.Key, a => a.Value);
        //            }
        //            //将新的映射方案缓存到redis
        //            SaveDicToRedis(Key_deviceDatalogPrivateValueMap, deviceDatalogPrivateValueMap, TimeSpan.MaxValue);
        //            SaveDicToRedis(Key_devDataToAllDataMap, devDataToAllDataMap, TimeSpan.MaxValue);
        //            SaveDicToRedis(Key_deviceDataIDToMeasIDMap, deviceDataIDToMeasIDMap, TimeSpan.MaxValue);
        //        }
        //        return resultValue;
        //    }

        //    for (int i = 0; i < SystemMeasPointGNode.SystemConfigNode.ChildrenCount; i++) //只能按厂站读取，stationID传0的话会报参数错误
        //    {
        //        SysNode sysnode = SystemMeasPointGNode.SystemConfigNode.Find(i);
        //        resultValue = ReadDatalogPrivateMap(sysnode.NodeID, 0, 0, ref deviceDatalogPrivateValueMap, ref devDataToAllDataMap, ref deviceDataIDToMeasIDMap);
        //        if (!resultValue.Success)
        //            return resultValue;
        //    }

        //    SaveDicToRedis(Key_deviceDatalogPrivateValueMap, deviceDatalogPrivateValueMap, TimeSpan.MaxValue);
        //    SaveDicToRedis(Key_devDataToAllDataMap, devDataToAllDataMap, TimeSpan.MaxValue);
        //    SaveDicToRedis(Key_deviceDataIDToMeasIDMap, deviceDataIDToMeasIDMap, TimeSpan.MaxValue);

        //    return resultValue;
        //}

        //private EMSErrorMsg ReadDatalogPrivateMap(uint stationID, uint channelID, uint deviceID, ref Dictionary<uint, List<DATALOG_PRIVATE_MAP>> deviceDatalogPrivateValueMap, ref Dictionary<DevDataIDType, List<DeviceDataParam>> devDataToAllDataMap, ref Dictionary<DeviceDataParam, DataIDToMeasIDDef> deviceDataIDToMeasIDMap)
        //{
        //    EMSErrorMsg resultValue = new EMSErrorMsg(true);
        //    DataTable queryResult = new DataTable();
        //    //仅加载定时记录1种类型，和PecTrend保持一致；不需要加载越限参数，因为越限参数是需要配置虚拟定时记录才有的；
        //    int errorCode = DatalogPrivateMapProvider.Instance.Read(stationID, channelID, deviceID, 0, 1, ref queryResult);
        //    if (errorCode != (int) ErrorCode.Success)
        //    {
        //        resultValue.Success = false;
        //        resultValue.ErrorMsgInstance = "DatalogPrivateMapProvider.Instance.Read";
        //        resultValue.ErrorMessage = ErrorQuerier.Instance.GetLastErrorString();
        //        return resultValue;
        //    }

        //    foreach (DataRow dr in queryResult.Rows)
        //    {
        //        DATALOG_PRIVATE_MAP privateMapItem = new DATALOG_PRIVATE_MAP();
        //        privateMapItem.stationID = Convert.ToUInt32(dr["StationID"]);
        //        privateMapItem.channelID = Convert.ToUInt32(dr["ChannelID"]);
        //        privateMapItem.deviceID = Convert.ToUInt32(dr["DeviceID"]);
        //        privateMapItem.paraHandle = Convert.ToInt32(dr["ParaHandle"]);
        //        privateMapItem.coefficient = Convert.ToDouble(dr["Coefficient"]);
        //        privateMapItem.dataTypeID = Convert.ToUInt32(dr["DataTypeID"]);
        //        privateMapItem.dataID = Convert.ToUInt32(dr["DataID"]);
        //        privateMapItem.dataIDFlag = Convert.ToByte(dr["DataIDFlag"]);
        //        privateMapItem.paraName = Convert.ToString(dr["ParaName"]);
        //        privateMapItem.paraType = Convert.ToInt32(dr["ParaType"]);
        //        privateMapItem.logicalDeviceIndex = Convert.ToInt32(dr["LogicalDeviceIndex"]);
        //        //电度相关的参数默认为999999999，其他参数默认为0
        //        double fullScal = 0;
        //        if (privateMapItem.dataID > 4000000 && privateMapItem.dataID < 5000000)
        //            fullScal = 999999999;
        //        if (!Convert.IsDBNull(dr["Reserved"]))
        //        {
        //            string reserved = Convert.ToString(dr["Reserved"]);
        //            privateMapItem.reserved = Convert.ToString(dr["Reserved"]);

        //            bool success = double.TryParse(reserved, out fullScal);
        //            if (!success)
        //            {
        //                if (privateMapItem.dataID > 4000000 && privateMapItem.dataID < 5000000)
        //                    fullScal = 999999999;
        //                else
        //                    fullScal = 0;
        //            }
        //        }
        //        //填入deviceDatalogPrivateValueMap
        //        List<DATALOG_PRIVATE_MAP> valueList;
        //        bool hasValueList = deviceDatalogPrivateValueMap.TryGetValue(privateMapItem.deviceID, out valueList);
        //        if (!hasValueList)
        //            valueList = new List<DATALOG_PRIVATE_MAP>();

        //        valueList.Add(privateMapItem);
        //        deviceDatalogPrivateValueMap[privateMapItem.deviceID] = valueList;

        //        //填入devDataParam
        //        DevDataIDType devData = new DevDataIDType(privateMapItem.deviceID, privateMapItem.dataID, privateMapItem.dataTypeID);
        //        DeviceDataParam devDataParam = new DeviceDataParam(privateMapItem.deviceID, privateMapItem.dataID, privateMapItem.dataTypeID, privateMapItem.logicalDeviceIndex, privateMapItem.paraType);

        //        List<DeviceDataParam> devDataParams;
        //        bool hasList = this.devDataToAllDataMap.TryGetValue(devData, out devDataParams);
        //        if (!hasList)
        //            devDataParams = new List<DeviceDataParam>();

        //        devDataParams.Add(devDataParam);
        //        this.devDataToAllDataMap[devData] = devDataParams;

        //        //填入deviceDataIDToMeasIDMap
        //        DataIDToMeasIDDef resultMapDef = DataIDToMeasIDDef.InvalidMapDef;
        //        bool parseSuccess = GetDataIDMapResultWithParaHandle(privateMapItem.stationID, privateMapItem.channelID, privateMapItem.deviceID, privateMapItem.paraHandle, privateMapItem.coefficient, fullScal, out resultMapDef);
        //        if (parseSuccess)
        //            deviceDataIDToMeasIDMap[devDataParam] = resultMapDef;
        //    }
        //    return resultValue;
        //}


        /// <summary>
        /// 添加基础参数到所有参数列表的映射关系
        /// </summary>
        /// <param name="dr"></param>
        private void AddDevDataToAllDataMap(DataRow dr)
        {
            DevDataIDType devData = new DevDataIDType(Convert.ToUInt32(dr["DeviceID"]), Convert.ToUInt32(dr["DataID"]), Convert.ToUInt32(dr["DataTypeID"]));
            DeviceDataParam devDataParam = new DeviceDataParam(devData.DeviceID, devData.DataID, devData.DataTypeID);
            devDataParam.LogicalDeviceIndex = Convert.ToInt32(dr["LogicalDeviceIndex"]);
            devDataParam.ParaType = Convert.ToInt32(dr["ParaType"]);

            List<DeviceDataParam> devDataParams;
            bool hasList = this.devDataToAllDataMap.TryGetValue(devData, out devDataParams);
            if (!hasList)
                devDataParams = new List<DeviceDataParam>();
            //仅加载定时记录这种数据类型
            if (devDataParam.ParaType == 1)
            {
                devDataParams.Add(devDataParam);
                this.devDataToAllDataMap[devData] = devDataParams;
            }
        }

        ///// <summary>
        ///// 往字典中添加记录
        ///// </summary>
        ///// <param name="dr"></param>
        //private void AddDeviceDatalogPrivateValueMap(DataRow dr)
        //{
        //    uint deviceID = Convert.ToUInt32(dr["DeviceID"]);
        //    List<DATALOG_PRIVATE_MAP> valueList;
        //    bool hasValueList = deviceDatalogPrivateValueMap.TryGetValue(deviceID, out valueList);
        //    if (!hasValueList)
        //        valueList = new List<DATALOG_PRIVATE_MAP>();
        //    DATALOG_PRIVATE_MAP privateMapItem = new DATALOG_PRIVATE_MAP();
        //    privateMapItem.stationID = Convert.ToUInt32(dr["StationID"]);
        //    privateMapItem.channelID = Convert.ToUInt32(dr["ChannelID"]);
        //    privateMapItem.deviceID = Convert.ToUInt32(dr["DeviceID"]);
        //    privateMapItem.paraHandle = Convert.ToInt32(dr["ParaHandle"]);
        //    privateMapItem.coefficient = Convert.ToDouble(dr["Coefficient"]);
        //    privateMapItem.dataTypeID = Convert.ToUInt32(dr["DataTypeID"]);
        //    privateMapItem.dataID = Convert.ToUInt32(dr["DataID"]);
        //    privateMapItem.dataIDFlag = Convert.ToByte(dr["DataIDFlag"]);
        //    privateMapItem.paraName = Convert.ToString(dr["ParaName"]);
        //    privateMapItem.paraType = Convert.ToInt32(dr["ParaType"]);
        //    privateMapItem.logicalDeviceIndex = Convert.ToInt32(dr["LogicalDeviceIndex"]);
        //    if (!Convert.IsDBNull(dr["Reserved"]))
        //        privateMapItem.reserved = Convert.ToString(dr["Reserved"]);
        //    //仅加载定时记录一种类型
        //    if (privateMapItem.paraType == 1)
        //    {
        //        valueList.Add(privateMapItem);
        //        deviceDatalogPrivateValueMap[deviceID] = valueList;
        //    }
        //}

        ///// <summary>
        ///// 往字典中添加记录
        ///// </summary>
        ///// <param name="dr"></param>
        //private void AddDeviceDataIDToMeasIDMap(DataRow dr)
        //{
        //    uint stationID = Convert.ToUInt32(dr["StationID"]);
        //    uint channelID = Convert.ToUInt32(dr["ChannelID"]);
        //    uint deviceID = Convert.ToUInt32(dr["DeviceID"]);
        //    int paraHandle = Convert.ToInt32(dr["ParaHandle"]);
        //    double cofficient = Convert.ToDouble(dr["Coefficient"]);
        //    uint dataTypeID = Convert.ToUInt32(dr["DataTypeID"]);
        //    int logicalDeviceIndex = Convert.ToInt32(dr["LogicalDeviceIndex"]);
        //    int paraType = Convert.ToInt32(dr["ParaType"]);
        //    uint dataID = Convert.ToUInt32(dr["DataID"]);
        //    //电度相关的参数默认为999999999，其他参数默认为0
        //    double fullScal = 0;
        //    if (dataID > 4000000 && dataID < 5000000)
        //        fullScal = 999999999;
        //    if (!Convert.IsDBNull(dr["Reserved"]))
        //    {
        //        string reserved = Convert.ToString(dr["Reserved"]);
        //        bool success = double.TryParse(reserved, out fullScal);
        //        if (!success)
        //        {
        //            if (dataID > 4000000 && dataID < 5000000)
        //                fullScal = 999999999;
        //            else
        //                fullScal = 0;
        //        }
        //    }

        //    DataIDToMeasIDDef resultMapDef = DataIDToMeasIDDef.InvalidMapDef;
        //    //执行参数映射解析
        //    bool result = GetDataIDMapResultWithParaHandle(stationID, channelID, deviceID, paraHandle, cofficient, fullScal, out resultMapDef);
        //    if (result)
        //    {
        //        DeviceDataParam dataIDParam = new DeviceDataParam();
        //        dataIDParam.DeviceID = deviceID;
        //        dataIDParam.DataID = dataID;
        //        dataIDParam.DataTypeID = dataTypeID;
        //        dataIDParam.LogicalDeviceIndex = logicalDeviceIndex;
        //        dataIDParam.ParaType = paraType;
        //        //仅加载定时记录这种类型
        //        if (paraType == 1)
        //            deviceDataIDToMeasIDMap[dataIDParam] = resultMapDef;
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
        private static bool GetDataIDMapResultWithParaHandle(uint stationID, uint channelID, uint deviceID, int paraHandle, double cofficient, double fullScal, out DataIDToMeasIDDef resultDef)
        {
            bool result = false;
            resultDef = DataIDToMeasIDDef.InvalidMapDef;
            resultDef.StationID = stationID;
            resultDef.DeviceID = deviceID;
            resultDef.Cofficient = cofficient;
            resultDef.fullScal = fullScal;

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

        #endregion

        #region 功能函数

        ///// <summary>
        ///// 根据指定的参数对象快速找到映射关系结果
        ///// </summary>
        ///// <param name="param">参数对象</param>
        ///// <param name="resultMap">映射结果对象</param>
        ///// <returns>是否查到结果</returns>
        //public bool FindDataMapDef(DeviceDataParam param, out DataIDToMeasIDDef resultMap)
        //{
        //    bool result = false;
        //    resultMap = DataIDToMeasIDDef.InvalidMapDef;

        //    //从缓存字典中快速查找定位
        //    result = deviceDataIDToMeasIDMap.TryGetValue(param, out resultMap);
        //    if (result)//如果查询成功，则直接返回
        //        return result;

        //    GetDatalogPrivateMapByID(param.DeviceID);

        //    result = deviceDataIDToMeasIDMap.TryGetValue(param, out resultMap);//再次从缓存查询

        //    return result;
        //}


        /// <summary>
        /// 根据指定的参数对象快速找到映射关系结果
        /// </summary>
        /// <param name="param">参数对象</param>
        /// <param name="resultMap">映射结果对象</param>
        /// <returns>是否查到结果</returns>
        public bool FindDataMapDef(DeviceDataParam param, out DATALOG_PRIVATE_MAP result)
        {

            if (!deviceDataIDToPrivateMap.ContainsKey(param))
            {
                GetDatalogPrivateMapByID(param.DeviceID);
            }
            if (deviceDataIDToPrivateMap.ContainsKey(param))
            {
                result = deviceDataIDToPrivateMap[param];
                return true;
            }
            else
            {
                result = new DATALOG_PRIVATE_MAP();
                return false;
            }
        }
        public void FindDataMapDef(List<DeviceDataParam> queryParams)
        {
            //如果映射关系不存在从，则缓存字典中快速查找定位
            foreach (var param in queryParams)
            {
                if (!deviceDataIDToPrivateMap.ContainsKey(param))
                    GetDatalogPrivateMapByID(param.DeviceID);
            }
        }
        /// <summary>
        /// 获取定时记录私有方案表记录
        /// </summary>
        /// <param name="deviceID">监测设备ID</param>
        /// <returns></returns>
        public List<DATALOG_PRIVATE_MAP> GetDatalogPrivateMapByID(uint deviceID)
        {
            List<DATALOG_PRIVATE_MAP> resultList;
            bool hasValue = deviceDatalogPrivateValueMap.TryGetValue(deviceID, out resultList);
            if (hasValue)
                return resultList;

            AddItemTodevDataToAllDataMap(deviceID);
            hasValue = deviceDatalogPrivateValueMap.TryGetValue(deviceID, out resultList);

            if (!hasValue)
                resultList = new List<DATALOG_PRIVATE_MAP>();
            return resultList;
        }

        ///// <summary>
        ///// 当缓存中不存在这个节点的时候，从数据库加载到缓存.一次查询的是一个deviceID对应的所有logicalDeviceIndex的定时记录（paraType=1）映射方案
        ///// </summary>
        ///// <param name="oriDataParam"></param>
        ///// <returns></returns>
        //public static void AddOneItem(DataRow dr, ref int count, Dictionary<uint, List<DATALOG_PRIVATE_MAP>> deviceDatalogPrivateValueMapTemp, Dictionary<DevDataIDType, List<DeviceDataParam>> devDataToAllDataMapTemp, Dictionary<DevDataID, List<DeviceDataParam>> devDataOnlyToAllDataMapTemp, Dictionary<DeviceDataParam, DataIDToMeasIDDef> deviceDataIDToMeasIDMapTemp, Dictionary<StaChaDevID, StaChaDevID> sourceIDNeedUpdateDevicesTemp)
        //{
        //    count++;//用于统计这是第几个元素，当是第一个元素的时候，先清空List，避免List越来越长，存在重复
        //    const int firstTime = 1; //表示是第一个元素
        //    DATALOG_PRIVATE_MAP privateMapItem = new DATALOG_PRIVATE_MAP();
        //    privateMapItem.stationID = Convert.ToUInt32(dr["StationID"]);
        //    privateMapItem.channelID = Convert.ToUInt32(dr["ChannelID"]);
        //    privateMapItem.deviceID = Convert.ToUInt32(dr["DeviceID"]);
        //    privateMapItem.paraHandle = Convert.ToUInt32(dr["ParaHandle"]);
        //    privateMapItem.coefficient = Convert.ToDouble(dr["Coefficient"]);
        //    privateMapItem.dataTypeID = Convert.ToUInt32(dr["DataTypeID"]);
        //    privateMapItem.dataID = Convert.ToUInt32(dr["DataID"]);
        //    privateMapItem.dataIDFlag = Convert.ToByte(dr["DataIDFlag"]);
        //    privateMapItem.paraName = Convert.ToString(dr["ParaName"]);
        //    privateMapItem.paraType = Convert.ToUInt32(dr["ParaType"]);
        //    privateMapItem.logicalDeviceIndex = Convert.ToUInt32(dr["LogicalDeviceIndex"]);
        //    //电度相关的参数默认为999999999，其他参数默认为0
        //    double fullScal = 0;
        //    if (privateMapItem.dataID > 4000000 && privateMapItem.dataID < 5000000)
        //        fullScal = 999999999;
        //    if (!Convert.IsDBNull(dr["Reserved"]))
        //    {
        //        string reserved = Convert.ToString(dr["Reserved"]);
        //        privateMapItem.reserved = Convert.ToString(dr["Reserved"]);

        //        bool success = double.TryParse(reserved, out fullScal);
        //        if (!success)
        //        {
        //            if (privateMapItem.dataID > 4000000 && privateMapItem.dataID < 5000000)
        //                fullScal = 999999999;
        //            else
        //                fullScal = 0;
        //        }
        //    }
        //    //填入deviceDatalogPrivateValueMap
        //    List<DATALOG_PRIVATE_MAP> valueList;
        //    bool hasValueList = deviceDatalogPrivateValueMapTemp.TryGetValue(privateMapItem.deviceID, out valueList);
        //    if ((!hasValueList) || (count == firstTime))//当是第一个元素的时候，该List不应该存在元素，因此要清空List
        //        valueList = new List<DATALOG_PRIVATE_MAP>();

        //    valueList.Add(privateMapItem);
        //    // lock (deviceDatalogPrivateValueMap_locker)
        //    //  {
        //    deviceDatalogPrivateValueMapTemp[privateMapItem.deviceID] = valueList;
        //    // }
        //    //填入devDataParam
        //    DevDataIDType devData = new DevDataIDType(privateMapItem.deviceID, privateMapItem.dataID, privateMapItem.dataTypeID);
        //    DeviceDataParam devDataParam = new DeviceDataParam(privateMapItem.deviceID, privateMapItem.dataID, privateMapItem.dataTypeID,(int)privateMapItem.logicalDeviceIndex, (int)privateMapItem.paraType);

        //    List<DeviceDataParam> devDataParams;
        //    bool hasList = devDataToAllDataMapTemp.TryGetValue(devData, out devDataParams);
        //    if ((!hasList) || (count == firstTime))//当是第一个元素的时候，该List不应该存在元素，因此要清空List
        //        devDataParams = new List<DeviceDataParam>();

        //    devDataParams.Add(devDataParam);
        //    //   lock (devDataToAllDataMap_locker)
        //    //  {
        //    devDataToAllDataMapTemp[devData] = devDataParams;
        //    //  }

        //    //填入devDataOnly
        //    if (privateMapItem.paraType == 1)//只保存定时记录的
        //    {
        //        DevDataID devDataOnly = new DevDataID(privateMapItem.deviceID, privateMapItem.dataID);
        //        DeviceDataParam devDataOnlyParam = new DeviceDataParam(privateMapItem.deviceID, privateMapItem.dataID, privateMapItem.dataTypeID, (int)privateMapItem.logicalDeviceIndex, (int)privateMapItem.paraType);

        //        List<DeviceDataParam> devDataOnlyParams;
        //        bool hasOnlyList = devDataOnlyToAllDataMapTemp.TryGetValue(devDataOnly, out devDataOnlyParams);
        //        if ((!hasOnlyList) || (count == firstTime))//当是第一个元素的时候，该List不应该存在元素，因此要清空List
        //            devDataOnlyParams = new List<DeviceDataParam>();

        //        devDataOnlyParams.Add(devDataOnlyParam);
        //        // lock (devDataOnlyToAllDataMap_locker)
        //        // {
        //        devDataOnlyToAllDataMapTemp[devDataOnly] = devDataOnlyParams;
        //        // }
        //    }

        //    //填入deviceDataIDToMeasIDMap
        //    DataIDToMeasIDDef resultMapDef = DataIDToMeasIDDef.InvalidMapDef;
        //    bool parseSuccess = GetDataIDMapResultWithParaHandle(privateMapItem.stationID, privateMapItem.channelID, privateMapItem.deviceID, (int)privateMapItem.paraHandle, privateMapItem.coefficient, fullScal, out resultMapDef);
        //    StaChaDevID device = new StaChaDevID(privateMapItem.stationID, privateMapItem.channelID, privateMapItem.deviceID);
        //    if (parseSuccess)
        //    {
        //        //  lock (deviceDataIDToMeasIDMap_locker)
        //        // {
        //        deviceDataIDToMeasIDMapTemp[devDataParam] = resultMapDef;
        //        // }

        //        if (sourceIDNeedUpdateDevicesTemp.ContainsKey(device)) //说明已经生成了对应的sourceID，可以从需要更新的设备列表中删除此设备了
        //            sourceIDNeedUpdateDevicesTemp.Remove(device);
        //    }
        //    else //说明还没有生成对应的sourceID，需要加入到需要更新的设备列表中
        //    {
        //        sourceIDNeedUpdateDevicesTemp[device] = device;
        //    }
        //}
        /// <summary>
        /// TODO 测试
        /// 当缓存中不存在这个节点的时候，从数据库加载到缓存.一次查询的是一个deviceID对应的所有logicalDeviceIndex的定时记录（paraType=1）映射方案
        /// </summary>
        /// <param name="oriDataParam"></param>
        /// <returns></returns>
        public static void AddOneItem(DataRow dr, ref int count, Dictionary<uint, List<DATALOG_PRIVATE_MAP>> deviceDatalogPrivateValueMapTemp, Dictionary<DevDataIDType, List<DeviceDataParam>> devDataToAllDataMapTemp, Dictionary<DevDataID, List<DeviceDataParam>> devDataOnlyToAllDataMapTemp, Dictionary<DeviceDataParam, DataIDToMeasIDDef> deviceDataIDToMeasIDMapTemp, Dictionary<DeviceDataParam, DATALOG_PRIVATE_MAP> deviceDataIdToPrivateMapTemp)
        {
            count++;//用于统计这是第几个元素，当是第一个元素的时候，先清空List，避免List越来越长，存在重复
            const int firstTime = 1; //表示是第一个元素
            DATALOG_PRIVATE_MAP privateMapItem = new DATALOG_PRIVATE_MAP();
            privateMapItem.stationID = Convert.ToUInt32(dr["StationID"]);
            privateMapItem.channelID = Convert.ToUInt32(dr["ChannelID"]);
            privateMapItem.deviceID = Convert.ToUInt32(dr["DeviceID"]);
            privateMapItem.paraHandle = Convert.ToUInt32(dr["ParaHandle"]);
            privateMapItem.coefficient = Convert.ToDouble(dr["Coefficient"]);
            privateMapItem.dataTypeID = Convert.ToUInt32(dr["DataTypeID"]);
            privateMapItem.dataID = Convert.ToUInt32(dr["DataID"]);
            privateMapItem.dataIDFlag = Convert.ToByte(dr["DataIDFlag"]);
            privateMapItem.paraName = Convert.ToString(dr["ParaName"]);
            privateMapItem.paraType = Convert.ToUInt32(dr["ParaType"]);
            privateMapItem.logicalDeviceIndex = Convert.ToUInt32(dr["LogicalDeviceIndex"]);
            if (!Convert.IsDBNull(dr["Reserved"]))
            {
                privateMapItem.reserved = Convert.ToString(dr["Reserved"]);
            }
            ////电度相关的参数默认为999999999，其他参数默认为0
            //double fullScal = 0;
            //if (privateMapItem.dataID > 4000000 && privateMapItem.dataID < 5000000)
            //    fullScal = 999999999;
            //if (!Convert.IsDBNull(dr["Reserved"]))
            //{
            //    string reserved = Convert.ToString(dr["Reserved"]);
            //    privateMapItem.reserved = Convert.ToString(dr["Reserved"]);

            //    bool success = double.TryParse(reserved, out fullScal);
            //    if (!success)
            //    {
            //        if (privateMapItem.dataID > 4000000 && privateMapItem.dataID < 5000000)
            //            fullScal = 999999999;
            //        else
            //            fullScal = 0;
            //    }
            //}
            //填入deviceDatalogPrivateValueMap
            List<DATALOG_PRIVATE_MAP> valueList;
            bool hasValueList = deviceDatalogPrivateValueMapTemp.TryGetValue(privateMapItem.deviceID, out valueList);
            if ((!hasValueList) || (count == firstTime))//当是第一个元素的时候，该List不应该存在元素，因此要清空List
                valueList = new List<DATALOG_PRIVATE_MAP>();

            valueList.Add(privateMapItem);
            deviceDatalogPrivateValueMapTemp[privateMapItem.deviceID] = valueList;

            //填入devDataParam
            DevDataIDType devData = new DevDataIDType(privateMapItem.deviceID, privateMapItem.dataID, privateMapItem.dataTypeID);
            DeviceDataParam devDataParam = new DeviceDataParam(privateMapItem.deviceID, privateMapItem.dataID, privateMapItem.dataTypeID, (int)privateMapItem.logicalDeviceIndex, (int)privateMapItem.paraType);

            List<DeviceDataParam> devDataParams;
            bool hasList = devDataToAllDataMapTemp.TryGetValue(devData, out devDataParams);
            if ((!hasList) || (count == firstTime))//当是第一个元素的时候，该List不应该存在元素，因此要清空List
                devDataParams = new List<DeviceDataParam>();

            devDataParams.Add(devDataParam);
            devDataToAllDataMapTemp[devData] = devDataParams;
            if (privateMapItem.paraType == 1)//只保存定时记录的
            {
                DevDataID devDataOnly = new DevDataID(privateMapItem.deviceID, privateMapItem.dataID);
                DeviceDataParam devDataOnlyParam = new DeviceDataParam(privateMapItem.deviceID, privateMapItem.dataID, privateMapItem.dataTypeID, (int)privateMapItem.logicalDeviceIndex, (int)privateMapItem.paraType);

                List<DeviceDataParam> devDataOnlyParams;
                bool hasOnlyList = devDataOnlyToAllDataMapTemp.TryGetValue(devDataOnly, out devDataOnlyParams);
                if ((!hasOnlyList) || (count == firstTime))//当是第一个元素的时候，该List不应该存在元素，因此要清空List
                    devDataOnlyParams = new List<DeviceDataParam>();
                devDataOnlyParams.Add(devDataOnlyParam);
                devDataOnlyToAllDataMapTemp[devDataOnly] = devDataOnlyParams;
            }
            ////填入deviceDataIDToMeasIDMap
            //DataIDToMeasIDDef resultMapDef = DataIDToMeasIDDef.InvalidMapDef;
            //bool parseSuccess = GetDataIDMapResultWithParaHandle(privateMapItem.stationID, privateMapItem.channelID, privateMapItem.deviceID, privateMapItem.paraHandle, privateMapItem.coefficient, fullScal, out resultMapDef);
            //StaChaDevID device = new StaChaDevID(privateMapItem.stationID, privateMapItem.channelID, privateMapItem.deviceID);
            //if (parseSuccess)
            //{
            //    deviceDataIDToMeasIDMapTemp[devDataParam] = resultMapDef;
            //    if (sourceIDNeedUpdateDevicesTemp.ContainsKey(device)) //说明已经生成了对应的sourceID，可以从需要更新的设备列表中删除此设备了
            //        sourceIDNeedUpdateDevicesTemp.Remove(device);
            //}
            //else //说明还没有生成对应的sourceID，需要加入到需要更新的设备列表中
            //{
            //    sourceIDNeedUpdateDevicesTemp[device] = device;
            //}

            //填入deviceDataIDToPrivateMap
            deviceDataIdToPrivateMapTemp[devDataParam] = privateMapItem;
        }

        /// <summary>
        /// TODO 测试
        /// 当缓存中不存在这个节点的时候，从数据库加载到缓存.一次查询的是一个deviceID对应的所有logicalDeviceIndex的定时记录（paraType=1）映射方案
        /// </summary>
        /// <param name="oriDataParam"></param>
        /// <returns></returns>
        public EMSErrorMsg AddItemTodevDataToAllDataMap(uint deviceIDParam)
        {
            EMSErrorMsg resultValue = new EMSErrorMsg(true);
            DataTable queryResult = new DataTable();

            try
            {
                ErrorInfoManager.Instance.WriteLogMessage("AddItemTodevDataToAllDataMap, deviceID=" + deviceIDParam);
                //查找deviceID对应的厂站通道设备
                SysNode deviceNode = PecsNodeManager.PecsNodeInstance.GetNodeByTypeID(PecsNodeType.PECSDEVICE_NODE, deviceIDParam);
                SysNode channelNode = deviceNode.ParentNode; //设备的父节点是通道
                SysNode stationNode = channelNode.ParentNode; //通道的父节点是厂站

                //仅加载定时记录1种类型，和PecTrend保持一致；不需要加载越限参数，因为越限参数是需要配置虚拟定时记录才有的；
                int errorCode = DataLogPrivateMapProvider.Instance.Read(0, stationNode.NodeID, channelNode.NodeID, deviceNode.NodeID, 0, 1, ref queryResult);
                if (errorCode != (int)CSharpDBPlugin.ErrorCode.Success)
                {
                    resultValue.Success = false;
                    resultValue.ErrorMsgInstance = "DatalogPrivateMapProvider.Instance.Read";
                    resultValue.ErrorMessage = CSharpDBPlugin.ErrorQuerier.Instance.GetLastErrorString();
                    return resultValue;
                }

                try
                {
                    Dictionary<uint, List<DATALOG_PRIVATE_MAP>> deviceDatalogPrivateValueMapTemp = new Dictionary<uint, List<DATALOG_PRIVATE_MAP>>();
                    Dictionary<DevDataIDType, List<DeviceDataParam>> devDataToAllDataMapTemp = new Dictionary<DevDataIDType, List<DeviceDataParam>>();
                    Dictionary<DevDataID, List<DeviceDataParam>> devDataOnlyToAllDataMapTemp = new Dictionary<DevDataID, List<DeviceDataParam>>();
                    Dictionary<DeviceDataParam, DataIDToMeasIDDef> deviceDataIDToMeasIDMapTemp = new Dictionary<DeviceDataParam, DataIDToMeasIDDef>();
                    Dictionary<DeviceDataParam, DATALOG_PRIVATE_MAP> deviceDataIdToPrivateMapTemp = new Dictionary<DeviceDataParam, DATALOG_PRIVATE_MAP>();
                    int count = 0;
                    foreach (DataRow dr in queryResult.Rows)
                    {
                        AddOneItem(dr, ref count, deviceDatalogPrivateValueMapTemp, devDataToAllDataMapTemp, devDataOnlyToAllDataMapTemp, deviceDataIDToMeasIDMapTemp, deviceDataIdToPrivateMapTemp);
                    }

                    foreach (var item in deviceDatalogPrivateValueMapTemp)
                    {
                        deviceDatalogPrivateValueMap[item.Key] = item.Value;
                    }
                    foreach (var item in devDataToAllDataMapTemp)
                    {
                        devDataToAllDataMap[item.Key] = item.Value;
                    }
                    foreach (var item in devDataOnlyToAllDataMapTemp)
                    {
                        // 只需要判断一次就可以
                        devDataOnlyToAllDataMap[item.Key] = item.Value;
                    }
                    //foreach (var item in deviceDataIDToMeasIDMapTemp)
                    //{
                    //    deviceDataIDToMeasIDMap[item.Key] = item.Value;
                    //}
                    foreach (var item in deviceDataIdToPrivateMapTemp)
                    {
                        deviceDataIDToPrivateMap[item.Key] = item.Value;
                    }
                    //if (sourceIDNeedUpdateDevicesTemp.Count == 0)
                    //{
                    //    StaChaDevID node = new StaChaDevID(stationNode.NodeID, channelNode.NodeID, deviceIDParam);
                    //    if (sourceIDNeedUpdateDevices.ContainsKey(node)) //说明已经生成了对应的sourceID，可以从需要更新的设备列表中删除此设备了
                    //        sourceIDNeedUpdateDevices.Remove(node);
                    //}
                    //else
                    //{
                    //    foreach (var item in sourceIDNeedUpdateDevicesTemp)
                    //    {
                    //        sourceIDNeedUpdateDevices[item.Key] = item.Value;
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    ErrorInfoManager.Instance.WriteLogMessage("AddItemTodevDataToAllDataMap foreach (DataRow dr in queryResult.Rows) catch exception：" + ex.Message + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                ErrorInfoManager.Instance.WriteLogMessage("AddItemTodevDataToAllDataMap deviceID=" + deviceIDParam + " catch exception " + ex.Message + ex.StackTrace);
            }
            finally
            {
                if (queryResult != null)
                    queryResult.Dispose();
            }

            return resultValue;
        }


        #endregion

    }

    #region 来自EMSSysDataDefine.cs

    /// <summary>
    /// 分时计量专用系统节点类型
    /// </summary>
    public class TOUTariffNodeType
    {
        /// <summary>
        /// 分时方案设置
        /// </summary>
        public const uint TOUSETUP = 0x10900000;

        /// <summary>
        /// 分时计量方案设置
        /// </summary>
        public const uint ENERGY_SETUP = 0x10910000;

        /// <summary>
        /// 分时计量方案节点
        /// </summary>
        public const uint ENERGY_SCHEDULE = 0x10911000;

        /// <summary>
        /// 费率定义设置
        /// </summary>
        public const uint TARIFF_SETUP = 0x10911100;

        /// <summary>
        /// 费率定义
        /// </summary>
        public const uint TARIFF_NODE = 0x10911110;

        /// <summary>
        /// 分时计量日计量时段设置
        /// </summary>
        public const uint ENERGY_DAILYPROFILESETUP = 0x10911200;

        /// <summary>
        /// 分时计量日计量时段
        /// </summary>
        public const uint ENERGY_DAILYPROFILE = 0x10911210;

        /// <summary>
        /// 分时计量年历视图设置
        /// </summary>
        public const uint ENERGY_CALENDERSETUP = 0x10911300;

        /// <summary>
        /// 分时计量[YEAR]年历视图
        /// </summary>
        public const uint ENERGY_CALENDERVIEW = 0x10911310;

        /// <summary>
        /// 分时计量年策略
        /// </summary>
        public const uint ENERGY_TOUSTRATEGY = 0x10911400;

        /// <summary>
        /// 分时计量日期规则
        /// </summary>
        public const uint ENERGY_TOURULE = 0x10911410;

        /// <summary>
        /// 分时越限方案设置
        /// </summary>
        public const uint SETPOINT_SETUP = 0x10920000;

        /// <summary>
        /// 分时越限方案节点
        /// </summary>
        public const uint SETPOINT_SCHEDULE = 0x10921000;

        /// <summary>
        /// 限值组定义设置
        /// </summary>
        public const uint SETPOINT_GROUPSETUP = 0x10921100;

        /// <summary>
        /// 限值组定义
        /// </summary>
        public const uint SETPOINT_GROUP = 0x10921110;

        /// <summary>
        /// 分时越限日越限时段设置
        /// </summary>
        public const uint SETPOINT_DAILYPROFILESETUP = 0x10921200;

        /// <summary>
        /// 分时越限日计量时段
        /// </summary>
        public const uint SETPOINT_DAILYPROFILE = 0x10921210;

        /// <summary>
        /// 分时越限年历视图
        /// </summary>
        public const uint SETPOINT_CALENDERSETUP = 0x10921300;

        /// <summary>
        /// 分时越限[YEAR]年历视图
        /// </summary>
        public const uint SETPOINT_CALENDERVIEW = 0x10921310;

        /// <summary>
        /// 分时越限年策略
        /// </summary>
        public const uint SETPOINT_TOUSTRATEGY = 0x10921400;

        /// <summary>
        /// 分时越限日期规则
        /// </summary>
        public const uint SETPOINT_TOURULE = 0x10921410;

        /// <summary>
        /// 能耗限值计划节点的版本号，一般的分时越限节点的版本号是1000，这里设为1001，便于进行区分
        /// </summary>
        public const uint TargetVersionID = 1001;
    }

    /// <summary>
    /// 定时记录参数信息
    /// </summary>
    public struct DataLogDataParam
    {
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
        /// 翻转值
        /// </summary>
        public double FullScaleValue;

        /// <summary>
        /// 系数
        /// </summary>
        public double Coefficient;

        /// <summary>
        /// 构造函数
        /// </summary>       
        /// <param name="dataID"></param>
        /// <param name="dataType"></param>
        public DataLogDataParam(uint dataID, uint dataType)
        {
            this.DataID = dataID;
            this.DataTypeID = dataType;
            this.LogicalDeviceIndex = 1;
            this.ParaType = 1;
            this.FullScaleValue = 999999999;
            this.Coefficient = 1;
        }
    }

    public struct DemandDataParam
    {
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
        /// 预测需量测点
        /// </summary>
        public uint PredictDemandMeasID;

        /// <summary>
        /// 系数
        /// </summary>
        public double Coefficient;

        /// <summary>
        /// 构造函数
        /// </summary>       
        /// <param name="dataID"></param>
        /// <param name="dataType"></param>
        public DemandDataParam(uint dataID, uint dataType)
        {
            this.DataID = dataID;
            this.DataTypeID = dataType;
            this.LogicalDeviceIndex = 1;
            this.ParaType = 1;
            this.PredictDemandMeasID = 0;
            this.Coefficient = 1;
        }
    }

    /// <summary>
    /// 时间结构
    /// </summary>
    public struct TimeStruct
    {
        public DateTime StartTime;
        public DateTime EndTime;

        public TimeStruct(DateTime stime, DateTime etime)
        {
            this.StartTime = stime;
            this.EndTime = etime;
        }
    }

    public struct NodeDataParam
    {
        public uint nodeID;
        public uint nodeType;

        public NodeDataParam(uint nType, uint nID)
        {
            nodeID = nID;
            nodeType = nType;
        }
    }


    #endregion

    #region 波形解析相关

    /// <summary>
    /// 用于保存cfg文件的数据
    /// 一、*.Cfg文件格式
    /// 
    /**1.总体格式
         Station_name,rec_dev_id,rev_year<CR/LF>
         TT,##A,##D<CR/LF>
         An,ch_id,ph,ccbm,uu,a,b,skew,min,max,primary,secondary,PS<CR/LF>
         An,ch_id,ph,ccbm,uu,a,b,skew,min,max,primary,secondary,PS<CR/LF>
         An,ch_id,ph,ccbm,uu,a,b,skew,min,max,primary,secondary,PS<CR/LF>
         An,ch_id,ph,ccbm,uu,a,b,skew,min,max,primary,secondary,PS<CR/LF>
         Dn,ch_id,ph,ccbm,y<CR/LF>
         Dn,ch_id,ph,ccbm,y<CR/LF>
         If<CR/LF>
         nrates<CR/LF>
         samp,endsamp<CR/LF>
         samp,endsamp<CR/LF>
         dd/mm/yyyy,hh:mm:ss.ssssss<CR/LF>
         dd/mm/yyyy,hh:mm:ss.ssssss<CR/LF>
         ft<CR/LF>
         timemult<CR/LF>*/

    /// </summary>
    public class CfgData
    {
        public CfgData()
        {
            _FirstLine = new FistLine();
            _SampInformation = new SampInformation();
            _SecondLine = new SecondLine();
            _AnList = new List<AnInformation>();
            _DnList = new List<DnInformation>();
            _If = string.Empty;
            _TimeStamps = new TimeStamps();

        }

        /// <summary>
        /// 1) Station_name,rec_dev_id,rev_year<CR/LF>
        /// </summary>
        public FistLine _FirstLine;

        /// <summary>
        /// 2) TT,##A,##D<CR/LF>
        /// </summary>
        public SecondLine _SecondLine;

        /// <summary>
        /// 3) An,ch_id,ph,ccbm,uu,a,b,skew,min,max,primary,secondary,PS<CR/LF>
        /// 模拟量通道信息，由##A决定有多少行，一行表示一个模拟量通道信息
        /// </summary>
        public List<AnInformation> _AnList;

        /// <summary>
        /// 4) Dn,ch_id,ph,ccbm,y<CR/LF> 
        /// 开关量通道信息，由##D决定有多少行，一行表示一个开关量通道信息
        /// </summary>
        public List<DnInformation> _DnList;

        /// <summary>
        ///5) If<CR/LF> If：频率，可选，实数；0～32字符宽度；可使用标准浮点数记法
        /// </summary>
        public string _If;

        /// <summary>
        /// 6)采样率信息
        /// </summary>
        public SampInformation _SampInformation;

        /// <summary>
        /// 7) 日期时间标记
        /// dd/mm/yyyy,hh:mm:ss.ssssss<CR/LF>
        /// dd/mm/yyyy,hh:mm:ss.ssssss<CR/LF>
        /// </summary>
        public TimeStamps _TimeStamps;

        /// <summary>
        /// 8) ft<CR/LF>
        /// ft：文件类型，必填，ASCII或BINARY
        /// </summary>
        public string _Ft;

        /// <summary>
        /// 9) timemult<CR/LF>
        /// timemult：“数据文件中的时间”的系数，单位微秒，必填，实数，1～32字符宽度；
        /// 可使用标准浮点数记法
        /// </summary>
        public double _Timemult;

        public static DateTime ParseTime_Cfg(string str)
        {
            try
            {
                string[] strs = str.Split(",".ToCharArray());
                if (strs.Length == 1)
                {
                    strs = str.Split(" ".ToCharArray());
                }
                string[] dates = strs[0].Split("/".ToCharArray());

                int day = ConvertUtil.StringToInt32(dates[0]);
                int month = ConvertUtil.StringToInt32(dates[1]);
                int year = ConvertUtil.StringToInt32(dates[2]);
                string[] times = strs[1].Split(":".ToCharArray());
                int hour = ConvertUtil.StringToInt32(times[0]);
                int minute = ConvertUtil.StringToInt32(times[1]);
                string[] seconds = times[2].Split(".".ToCharArray());
                int second = ConvertUtil.StringToInt32(seconds[0]);
                int msec = ConvertUtil.StringToInt32(seconds[1]);
                DateTime time = new DateTime(year, month, day, hour, minute, second).AddMilliseconds(msec);
                return time;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout("解析波形cfg文件失败" + ex.Message + "\t cfg: \r\n" + str + ex.StackTrace);
                return DateTime.MinValue;
            }
        }
    }

    /// <summary>
    /// 8) ft<CR/LF>
    /// ft：文件类型，必填，ASCII或BINARY
    /// </summary>
    public enum Ft
    {
        ASCII = 0,
        BINARY = 1,
    }

    public class TimeStamps
    {
        public DateTime _FistTime;
        public DateTime _TriggerTime;
    }

    /// <summary>
    /// nrates<CR/LF>
    ///samp,endsamp<CR/LF>
    ///samp,endsamp<CR/LF>
    ///nrates：数据文件中的采样率个数；必填；整数；0～999
    ///samp：采用率，单位Hz，必填；实数；1～32字符宽度；可使用标准浮点数记法
    ///endsamp：用Samp采样率最后一次采样的序号；必填；整数；1～9999999999
    ///用多个采样率，就有多少行“samp,endsamp<CR/LF>”；
    ///用一个采样率，nrates填0，samp填0，endsamp填data文件中的最后一个采样序号
    /// </summary>
    public class SampInformation
    {
        public uint _Nrates;
        public List<SampAndIndex> _SampList;
    }

    public class SampAndIndex
    {
        /// <summary>
        /// samp：采用率，单位Hz，必填；实数；1～32字符宽度；可使用标准浮点数记法
        /// </summary>
        public double _Samp;

        /// <summary>
        /// endsamp：用Samp采样率最后一次采样的序号；必填；整数；1～9999999999
        /// </summary>
        public uint _Endsamp;
    }

    /// <summary>
    ///  Dn,ch_id,ph,ccbm,y<CR/LF>
    ///  开关量通道信息，由##D决定有多少行，一行表示一个开关量通道信息
    /// </summary>
    public class DnInformation
    {
        /// <summary>
        /// Dn： 开关量通道序号；必填，字母＋数字，取值范围D1～A999999
        /// </summary>
        public string _Dn;

        /// <summary>
        ///  Ch_id：通道标识；可选；数字字母，0～64字符
        /// </summary>
        public string _Ch_id;

        /// <summary>
        ///     Ph：通道相别标识；可选，字母＋数字，0～2字符
        /// </summary>
        public string _Ph;

        /// <summary>
        ///   Ccbm：被监视的电路组件；可选；字母数字，0～64字符
        /// </summary>
        public string _Ccbm;

        /// <summary>
        ///     Y：通道开关量正常的状态；必填；整数；0或1
        /// </summary>
        public int _Y;
    }

    /// <summary>
    /// Dn： 开关量通道序号；必填，字母＋数字，取值范围D1～A999999
    /// </summary>
    public class Dn
    {
        public string _Dn;
        //public char _D;
        //public uint _N;
    }

    /// <summary>
    ///   An,ch_id,ph,ccbm,uu,a,b,skew,min,max,primary,secondary,PS CR/LF
    ///   模拟量通道信息，由##A决定有多少行，一行表示一个模拟量通道信息
    /// </summary>
    public class AnInformation
    {
        /// <summary>
        /// An： 模拟量通道序号；必填，字母＋数字，取值范围A1～A999999
        /// </summary>
        public string _An;

        /// <summary>
        /// ：通道标识；可选；数字字母，0～64字符
        /// </summary>
        public string _Ch_id;

        /// <summary>
        /// Ph：通道相别标识；可选，字母＋数字，0～2字符
        /// </summary>
        public string _Ph;

        /// <summary>
        /// Ccbm：被监视的电路组件；可选；字母数字，0～64字符
        /// </summary>
        public string _Ccbm;

        /// <summary>
        /// Uu：通道单位；必填；字母；1～32字符；可用K或M表示千或兆
        /// </summary>
        public string _Uu;

        /// <summary>
        /// A：通道系数；必填；实数；1～32个字符宽度；可使用标准浮点记法
        /// </summary>
        public double _A;

        /// <summary>
        /// B: 通道偏移量；必填；实数；1～32个字符宽度；可使用标准浮点数记法
        /// （通道转换公式为ax+b， x取自.DAT文件， 单位就是uu）
        /// </summary>
        public double _B;

        /// <summary>
        /// skew：通道时间反称性（微秒）；可选；实数；1～32个字符宽度；可使用标准浮点数记法
        /// </summary>
        public string _Skew;

        /// <summary>
        /// min：通道最小值；必填；整数；1～6个字符宽度；取值范围-99999～99999；
        /// 二进制格式的数据文件取值范围为-32767～+32767
        /// </summary>
        public int _Min;

        /// <summary>
        /// max：通道最大值；必填；整数；1～6个字符宽度；取值范围-99999～99999；
        /// 二进制格式的数据文件取值范围为-32767～+32767
        /// </summary>
        public int _Max;

        /// <summary>
        /// primary：通道电压或电流变比一次侧值；必填；实数；1～32字符宽度
        /// </summary>
        public double _Primary;

        /// <summary>
        /// secondary：通道电压或电流变比二次侧值；必填；实数；1～32字符宽度
        /// </summary>
        public double _Secondary;

        /// <summary>
        /// PS：一次值或二次值标识；必填；字母；1个字符；可为p，P，s，S
        /// </summary>
        public string _PS;

    }

    /// <summary>
    /// An： 模拟量通道序号；必填，字母＋数字，取值范围A1～A999999
    /// </summary>
    public class An
    {
        public string _An;

    }

    /// <summary>
    ///  Station_name,rec_dev_id,rev_year<CR/LF>
    /// </summary>
    public class FistLine
    {
        /// <summary>
        /// 分站地点的名字；可选；字母数字；０～64个字符
        /// </summary>
        public string _Station_name;

        /// <summary>
        /// 记录设备的标识号或名字；可选；字母数字；0～64个字符
        /// </summary>
        public string _Rec_dev_id;

        /// <summary>
        /// 用年表示的COMTRADE文件版本，如2000；必填；数字；4个字符；不配或配置错误将遵照标准的1991年版本。
        /// </summary>
        public uint _Rev_year;
    }

    /// <summary>
    ///  TT,##A,##D<CR/LF>
    /// </summary>
    public class SecondLine
    {
        /// <summary>
        /// 波形通道总个数；必填；数字；整数；取值范围1～999999；等于后面##A和##D的和
        /// </summary>
        public uint _TT;

        /// <summary>
        /// 模拟量通道个数，末尾用A标识；必填，数字+字母取值范围0A～999999A
        /// </summary>
        public uint _OOA;

        /// <summary>
        /// 开关量通道个数，末尾用D标识；必填，数字+字母取值范围0D～999999D
        /// </summary>
        public uint _OOD;
    }

    /// <summary>
    /// 用于解析Cfg文件
    /// </summary>
    internal class CfgFileParser
    {
        public static bool ParseCfgFile(string[] parseCfgFileStrs, out CfgData cfgData)
        {
            cfgData = new CfgData();
            try
            {
                string[] firstLine = parseCfgFileStrs[0].Split(',');
                cfgData._FirstLine._Station_name = firstLine[0];
                cfgData._FirstLine._Rec_dev_id = firstLine[1];
                cfgData._FirstLine._Rev_year = Convert.ToUInt32(firstLine[2]);
                //解析波形通道总数
                GetFistLine(parseCfgFileStrs, ref cfgData);
                const int startSimu = 2;
                //模拟量
                GetA(parseCfgFileStrs, ref cfgData, startSimu);
                uint startD = startSimu + cfgData._SecondLine._OOA;
                //开关量
                GetD(parseCfgFileStrs, ref cfgData, startD);
                uint ifLine = startD + cfgData._SecondLine._OOD;
                //If
                cfgData._If = parseCfgFileStrs[ifLine];

                //采样率信息
                uint sampStartLine = ifLine + 1;
                GetSamp(parseCfgFileStrs, ref cfgData, sampStartLine);
                //日期时间标记
                uint timeStartLine = sampStartLine + (UInt32) cfgData._SampInformation._SampList.Count + 1;

                cfgData._TimeStamps._FistTime = CfgData.ParseTime_Cfg(parseCfgFileStrs[timeStartLine]);
                cfgData._TimeStamps._TriggerTime = CfgData.ParseTime_Cfg(parseCfgFileStrs[timeStartLine + 1]);

                //ft
                uint ftStartLine = timeStartLine + 2;
                cfgData._Ft = parseCfgFileStrs[ftStartLine];

                //9) timemult<CR/LF>
                uint timemultStartLine = ftStartLine + 1;
                cfgData._Timemult = Convert.ToDouble(parseCfgFileStrs[timemultStartLine]);
                return true;
            }
            catch (Exception ex)
            {
                DbgTrace.dout("ParseCfgFile(string[] parseCfgFileStrs,out CfgData cfgData) catch (Exception ex):" + ex.Message + ex.StackTrace);
                return false;
            }
        }

        private static void GetSamp(string[] parseCfgFileStrs, ref CfgData cfgData, uint sampStartLine)
        {
            cfgData._SampInformation._Nrates = Convert.ToUInt32(parseCfgFileStrs[sampStartLine]);
            cfgData._SampInformation._SampList = new List<SampAndIndex>();

            if (cfgData._SampInformation._Nrates == 0)
            {
                string[] sampStrs = parseCfgFileStrs[sampStartLine + 1].Split(',');
                SampAndIndex sampAndIndex = new SampAndIndex();
                sampAndIndex._Samp = Convert.ToDouble(sampStrs[0]);
                sampAndIndex._Endsamp = Convert.ToUInt32(sampStrs[1]);
                cfgData._SampInformation._SampList.Add(sampAndIndex);
            }
            else
            {
                for (uint i = 1; i <= cfgData._SampInformation._Nrates; ++i)
                {
                    string[] sampStrs = parseCfgFileStrs[sampStartLine + i].Split(',');
                    SampAndIndex sampAndIndex = new SampAndIndex();
                    sampAndIndex._Samp = Convert.ToDouble(sampStrs[0]);
                    sampAndIndex._Endsamp = Convert.ToUInt32(sampStrs[1]);
                    cfgData._SampInformation._SampList.Add(sampAndIndex);
                }
            }
        }

        private static void GetD(string[] parseCfgFileStrs, ref CfgData cfgData, uint startD)
        {
            cfgData._DnList = new List<DnInformation>();
            //开关量
            for (int i = 0; i < cfgData._SecondLine._OOD; ++i)
            {
                string[] dnStrs = parseCfgFileStrs[startD + i].Split(',');
                DnInformation dnInformation = new DnInformation();

                dnInformation._Dn = dnStrs[0];
                dnInformation._Ch_id = dnStrs[1];
                dnInformation._Ph = dnStrs[2];
                dnInformation._Ccbm = dnStrs[3];
                dnInformation._Y = Convert.ToInt32(dnStrs[4]);
                cfgData._DnList.Add(dnInformation);
            }
        }

        private static void GetA(string[] parseCfgFileStrs, ref CfgData cfgData, uint startSimu)
        {
            for (int i = 0; i < cfgData._SecondLine._OOA; ++i)
            {
                string[] anStrs = parseCfgFileStrs[startSimu + i].Split(',');
                AnInformation anInformation = new AnInformation();

                anInformation._An = anStrs[0];
                anInformation._Ch_id = anStrs[1];
                anInformation._Ph = anStrs[2];
                anInformation._Ccbm = anStrs[3];
                anInformation._Uu = anStrs[4];
                anInformation._A = Convert.ToDouble(anStrs[5]);
                anInformation._B = Convert.ToDouble(anStrs[6]);
                anInformation._Skew = anStrs[7];
                anInformation._Min = Convert.ToInt32(anStrs[8]);
                anInformation._Max = Convert.ToInt32(anStrs[9]);
                anInformation._Primary = Convert.ToDouble(anStrs[10]);
                anInformation._Secondary = Convert.ToDouble(anStrs[11]);
                anInformation._PS = anStrs[12];

                cfgData._AnList.Add(anInformation);
            }
        }

        private static void GetFistLine(string[] parseCfgFileStrs, ref CfgData cfgData)
        {
            string[] tTADStrs = parseCfgFileStrs[1].Split(',');
            cfgData._SecondLine._TT = Convert.ToUInt32(tTADStrs[0]);
            cfgData._SecondLine._OOA = Convert.ToUInt32(tTADStrs[1].Substring(0, tTADStrs[1].Length - 1));
            cfgData._SecondLine._OOD = cfgData._SecondLine._TT - cfgData._SecondLine._OOA;
            cfgData._AnList = new List<AnInformation>();
        }
    }

    public class ComputeWaveData
    {
        /// <summary>
        /// 获取一个波形文件（一行Data列的数据）的数据
        /// </summary>
        /// <param name="memStream"></param>
        /// <param name="oneWaveData"></param>
        /// <returns></returns>
        public static bool GetOneWave(MemoryStream memStream, out OneWaveData oneWaveData)
        {
            oneWaveData = new OneWaveData();
            CfgData cfgData;
            List<DatDataOneLine> resultList;
            if (!WaveDataDecodeManager.ParseWaveData(memStream, out cfgData, out resultList))
                return false;
            oneWaveData._CfgData = cfgData;
            oneWaveData._DatData = new DatData();
            oneWaveData._DatData._DataList = resultList;
            return true;
        }

        /// <summary>
        /// 获取一条事件对应的波形，波形如果有多个，就进行合并处理
        /// </summary>
        /// <param name="waveDataList">多个需要合并的波形</param>
        /// <param name="totalCombinedWave">合并完成之后的结果</param>
        /// <returns></returns>
        public static bool GetDataForWave(List<OneWaveData> waveDataList, out TotalCombinedWave totalCombinedWave)
        {
            totalCombinedWave = new TotalCombinedWave();
            List<OneWaveResultData> oneWaveResultDataList;
            GetAllWaveData(waveDataList, out oneWaveResultDataList);
            GetCombinedWaveData(oneWaveResultDataList, out totalCombinedWave);
            GetTimeList(waveDataList[0]._CfgData, waveDataList[0]._DatData._DataList, out totalCombinedWave._TimeList);
            return true;
        }

        private static void GetTimeList(CfgData cfgData, List<DatDataOneLine> datDataList, out List<double> timeList)
        {
            timeList = new List<double>();
            //计算timestamp。所有的将要被合并的timestamp都是一样的，所以只采用第一个波形的就可以了
            var startTime = cfgData._TimeStamps._FistTime;
            //如果采样率个数是0，那么dat文件中的n timestamp A1 A2,……Ak S1 S2,……Sm 中的timestamp不为空，这时候，可以根据这个值累加
            if (cfgData._SampInformation._Nrates == 0)
            {
                DoNoSamp(cfgData, datDataList, timeList);
            }
                //根据各个采样率来计算
            else
            {
                DoWithSamp(cfgData, datDataList, timeList);
            }
        }

        private static void DoNoSamp(CfgData cfgData, List<DatDataOneLine> datDataList, List<double> timeList)
        {
            var startTime = cfgData._TimeStamps._FistTime;
            DateTime temp = startTime;
            double timeNeedAdded = 0;
            foreach (var oneLine in datDataList)
            {
                double msec = oneLine._timestamp/1000.0; //转换成毫秒
                timeNeedAdded = /*timeNeedAdded + */ msec;
                temp = startTime.AddMilliseconds(timeNeedAdded);
                //timeList.Add(temp);
                timeList.Add(msec);
            }
        }

        private static double GetSecondAndMiniSecond(DateTime time)
        {
            double tempSecond = time.Second;
            double tempMiniSecond = time.Millisecond;
            double totalMiniSecond = tempSecond*1000 + tempMiniSecond;
            return totalMiniSecond/1000;
        }

        private static void DoWithSamp(CfgData cfgData, List<DatDataOneLine> datDataList, List<double> timeList)
        {
            List<double> tList = new List<double>();
            var startTime = cfgData._TimeStamps._FistTime;
            double currentSamp = cfgData._SampInformation._SampList[0]._Samp;
            int sampIndex = 0;

            ////频率,这时候频率应该可以解析成功，如果解析不成功，应该抛出异常，方便调试，查找问题,所以使用Convert.ToDouble
            double frequency = Convert.ToDouble(cfgData._If);
            //timeList.Add(currentStartTime);
            //采样率
            //单点时间间隔
            var countPerCycle = Convert.ToInt32(currentSamp/frequency);
            double singleDataTime = 1000.0/(frequency*countPerCycle);
            double timeNeedAdded = singleDataTime;

            var sampStartTime = cfgData._TimeStamps._FistTime;
            for (int k = 0; k < cfgData._SampInformation._SampList.Count; k++)
            {
                //某个采样率下结束的周波点数
                int sampleEndIndex = (int) cfgData._SampInformation._SampList[k]._Endsamp;
                int pos = 0;
                if (k > 0)
                {
                    pos = (int) cfgData._SampInformation._SampList[k - 1]._Endsamp;
                }
                //单点时间间隔
                var countPerCycle2 = Convert.ToInt32(cfgData._SampInformation._SampList[k]._Samp/frequency);
                double singleDataTime2 = 1000.0/(frequency*countPerCycle2);
                while (pos < sampleEndIndex)
                {
                    int currendSampPos = pos;
                    if (k > 0)
                        currendSampPos = pos - (int) cfgData._SampInformation._SampList[k - 1]._Endsamp + 1;

                    DateTime logTime = sampStartTime.AddMilliseconds(currendSampPos*singleDataTime2);
                    //计算波形数据秒+毫秒
                    double mesStr = sampStartTime.Second + (sampStartTime.Millisecond + (currendSampPos*singleDataTime2))/1000;
                    //mesStr = Format3No45(mesStr);  为了让Excel显示的时候不出现多个相同的横坐标，要增加横坐标的精度，这里就不保留3位小数，而是保留完整的数
                    tList.Add(mesStr);
                    if (pos == sampleEndIndex - 1)
                        sampStartTime = logTime;
                    pos++;
                }
            }

            DateTime tempTime = cfgData._TimeStamps._FistTime;
            tempTime = tempTime.AddMilliseconds(-tempTime.Millisecond);
            tempTime = tempTime.AddSeconds(-tempTime.Second);

            foreach (var item in tList)
            {
                //timeList.Add(tempTime.AddMilliseconds(item * 1000));
                timeList.Add(item * 1000);
            }
        }

        /// <summary>
        /// 保留3位小数，不做四舍五入
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double Format3No45(double num)
        {
            var str = string.Format("{0}00000", num);
            var tmp = double.Parse(str.Substring(0, str.IndexOf('.') + 5));
            return tmp;
        }

        private static double FormatNoPoint(double num)
        {
            var str = (num).ToString();
            var tmp = double.Parse(str.Substring(0, str.IndexOf('.')));
            return tmp;
        }

        /// <summary>
        /// 只有单通道的才需要合并
        /// </summary>
        /// <param name="oneWaveResultDataList"></param>
        /// <param name="totalCombinedWave"></param>
        private static void GetCombinedWaveData(List<OneWaveResultData> oneWaveResultDataList, out TotalCombinedWave totalCombinedWave)
        {
            totalCombinedWave = new TotalCombinedWave();
            if (oneWaveResultDataList.Count == 0)
                return;

            //合并多个通道的波形数据
            foreach (var oneWave in oneWaveResultDataList)
            {
                CombineOneWave(oneWave, ref totalCombinedWave);
            }
        }

        private static void CombineOneWave(OneWaveResultData oneWaveResultData, ref TotalCombinedWave totalCombinedWave)
        {
            foreach (var item in oneWaveResultData._KaiGuanDic)
            {
                if (totalCombinedWave._ValueDic.ContainsKey(item.Key))
                    continue;

                totalCombinedWave._ValueDic.Add(item.Key, item.Value);
            }
            foreach (var item in oneWaveResultData._ValueMoniLiangDic)
            {
                if (!totalCombinedWave._ValueDic.ContainsKey(item.Key))
                    totalCombinedWave._ValueDic.Add(item.Key, item.Value);
            }
            totalCombinedWave._CfgDataList.Add(oneWaveResultData._CfgData);
        }

        private static void GetAllWaveData(List<OneWaveData> waveDataList, out List<OneWaveResultData> oneWaveResultDataList)
        {
            oneWaveResultDataList = new List<OneWaveResultData>();

            foreach (var oneWaveData in waveDataList)
            {
                OneWaveResultData oneWaveResultData;
                //计算各个数据的值，模拟量：通道转换公式为ax+b ；开关量：直接采用数据
                AddValues(oneWaveData, out oneWaveResultData);
                oneWaveResultDataList.Add(oneWaveResultData);
            }
        }

        private static void AddValues(OneWaveData oneWaveData, out OneWaveResultData oneWaveResultData)
        {
            oneWaveResultData = new OneWaveResultData();
            oneWaveResultData._CfgData = oneWaveData._CfgData;
            //遍历dat的每行数据。
            foreach (var datDataOneLine in oneWaveData._DatData._DataList)
            {
                //一行数据里面
                AddOneLineData(oneWaveData._CfgData, datDataOneLine, ref oneWaveResultData);
            }
        }

        private static void AddOneLineData(CfgData cfgData, DatDataOneLine datDataOneLine, ref OneWaveResultData oneWaveResultData)
        {
            oneWaveResultData._NList.Add(datDataOneLine._N);
            oneWaveResultData._TimestampList.Add(datDataOneLine._timestamp);

            //模拟量
            //i表示一行数据中的列标号。第一个模拟量的标号是0，最后一列的标号是datDataOneLine._AkList.Count-1
            int i = 0;
            foreach (var anData in datDataOneLine._AkList)
            {
                double result = cfgData._AnList[i]._A*anData + cfgData._AnList[i]._B;
                AddValueToDic(cfgData._AnList[i]._An + "," + cfgData._AnList[i]._Ch_id, result, ref oneWaveResultData._ValueMoniLiangDic);
                ++i;
            }

            //开关量
            int j = 0;
            foreach (var item in datDataOneLine._DmList)
            {
                double result = item;
                AddValueToDic(cfgData._DnList[j]._Dn+","+cfgData._DnList[j]._Ch_id, result, ref oneWaveResultData._KaiGuanDic);
                ++j;
            }

        }



        private static void AddValueToDic(string key, double result, ref Dictionary<string, List<double>> valueDic)
        {
            List<double> valueList;
            if (!valueDic.TryGetValue(key, out valueList))
            {
                valueList = new List<double>();
                valueDic.Add(key, valueList);
            }
            valueList.Add(result);
        }

        private static void AddKeys(OneWaveData oneWaveData, ref Dictionary<string, List<double>> valueMoniLiangDic, ref Dictionary<string, List<double>> kaiGuanDic)
        {
            foreach (var item in oneWaveData._CfgData._AnList)
            {
                if (!valueMoniLiangDic.ContainsKey(item._An))
                    valueMoniLiangDic.Add(item._An, new List<double>());
            }
            foreach (var item in oneWaveData._CfgData._DnList)
            {
                if (!kaiGuanDic.ContainsKey(item._Dn))
                    kaiGuanDic.Add(item._Dn, new List<double>());
            }
        }

    }

    internal class ConstDefForWave
    {
        #region 常量

        /// <summary>
        /// binary编码下的模拟量无效值
        /// </summary>
        public static readonly UInt16 InvalidMoniValue_Bin = 0x8000;

        /// <summary>
        /// binary编码下的
        /// </summary>
        public static readonly UInt32 InvalidTimeValue_Bin = 0xFFFFFFF;

        /// <summary>
        /// Ascii编码情形下的模拟量和开关量无效值
        /// </summary>
        public static readonly int InvalidValue_ASCII = 99999;

        #endregion
    }

    internal class ConvertUtil
    {
        public static Int32 StringToInt32(string strValue, Int32 defaultValue = 0)
        {
            Int32 value;
            if (Int32.TryParse(strValue.Trim(), out value))
                return value;
            return defaultValue;
        }

        public static UInt32 StringToUInt32(string strValue, UInt32 defaultValue = 0)
        {
            UInt32 value;
            if (UInt32.TryParse(strValue.Trim(), out value))
                return value;
            return defaultValue;
        }

        public static UInt64 StringToUInt64(string strValue, UInt64 defaultValue = 0)
        {
            UInt64 value;
            if (UInt64.TryParse(strValue.Trim(), out value))
                return value;
            return defaultValue;
        }

        public static double StringToDouble(string strValue, double defaultValue)
        {
            double result;
            if (double.TryParse(strValue, out result))
                return result;
            return defaultValue;
        }
    }

    public class DatData
    {
        public List<DatDataOneLine> _DataList;
    }

    public class DatDataOneLine
    {
        public DatDataOneLine()
        {
            _AkList = new List<double>();
            _DmList = new List<double>();
            _N = uint.MinValue;
            _timestamp = double.MinValue;
        }

        /// <summary>
        /// n：采样序号，必填，整数，1～9999999999
        /// </summary>
        public uint _N;

        /// <summary>
        /// timestamp：时间标识；基本单位微秒；整数，1～10个字符宽度
        /// </summary>
        public double _timestamp;

        /// <summary>
        /// A1…AK: 模拟量各个通道数据，可选，整数，-99999～99998
        /// 填入99999表示无效
        /// </summary>
        public List<double> _AkList;

        /// <summary>
        /// D1…Dm: 开关量各个通道数据，可选，整数，0或1
        /// 填入99999表示无效
        /// </summary>
        public List<double> _DmList;

        ///// <summary>
        ///// 包含模拟量和开关量，在一行中的各个数据
        ///// ///// A1…AK: 模拟量各个通道数据，可选，整数，-99999～99998
        /////// 填入99999表示无效
        /////// D1…Dm: 开关量各个通道数据，可选，整数，0或1
        /////// 填入99999表示无效
        ///// </summary>
        //public List<double> _DataList;
    }

    /// <summary>
    /// 2.BINARY格式的DAT文件总体格式
    ///n timestamp A1 A2,……Ak S1 S2,……Sm
    ///n：采样序号，必填，整数，4字节，00000001～FFFFFFFF
    ///timestamp：时间标识；必填，基本单位微秒；整数，4字节，00000001～FFFFFFFF
    ///如果CFG文件中的nrates和samp非零，则可选；
    ///如果CFG文件中的nrates和samp为零，则必填；
    ///填入FFFFFFFF表示无效
    ///A1…AK: 模拟量各个通道数据，可选，整数，2字节，$8001～$7FFF
    ///		填入$8000表示无效
    ///S1…Sm: 开关量各个通道数据，可选，整数，2字节表示16个开关量，从低位开始填入
    /// </summary>
    internal class DatDataBINARY : DatFileParser
    {

    }

    /// <summary>
    /// 解析Dat文件的数据
    /// </summary>
    internal class DatFileParser
    {
        public static bool ParseDatFile(List<string> datStringList, uint channelNumA, uint channelNumD, out List<DatDataOneLine> datDataList)
        {
            datDataList = new List<DatDataOneLine>();
            try
            {
                foreach (var item in datStringList)
                {
                    DatDataOneLine datData = new DatDataOneLine();
                    datDataList.Add(datData);
                    //对一行字符串进行解析
                    GetData(channelNumA, channelNumD, item, ref datData);
                }
                return true;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        private static void GetData(uint channelNumA, uint channelNumD, string item, ref DatDataOneLine datData)
        {
            string[] oneLine = item.Split(',');
            datData._N = Convert.ToUInt32(oneLine[0]);

            double timeStampTemp;
            if (!double.TryParse(oneLine[1], out timeStampTemp))
            {
                timeStampTemp = double.NaN;
            }
            datData._timestamp = timeStampTemp;
            uint aStartColumn = 2;
            //模拟量
            GetA(channelNumA, ref datData, oneLine, aStartColumn);
            //开关量
            GetD(channelNumA, channelNumD, ref datData, oneLine, aStartColumn);

        }

        private static DatDataOneLine GetD(uint channelNumA, uint channelNumD, ref DatDataOneLine datData, string[] oneLine, uint aStartColumn)
        {
            uint dStartColumn = aStartColumn + channelNumA;
            datData._DmList = new List<double>();
            for (int i = 0; i < channelNumD; ++i)
            {
                double temp;

                if (double.TryParse(oneLine[dStartColumn + i], out temp))
                {
                    temp = Convert.ToDouble(string.Format("{0:N3}", temp));
                    //文档中有对于无效值的描述，但是因为现在PecWave没有做无效值判断，所以暂时不加上这个判断。后面如果需要的话，再加上。
                    //if(temp==99999)
                    //    temp=double.NaN;
                }
                else
                {
                    temp = double.NaN;
                }
                datData._DmList.Add(temp);
            }
            return datData;
        }

        private static void GetA(uint channelNumA, ref DatDataOneLine datData, string[] oneLine, uint aStartColumn)
        {
            datData._AkList = new List<double>();
            for (int i = 0; i < channelNumA; ++i)
            {
                double temp;
                if (double.TryParse(oneLine[aStartColumn + i], out temp))
                {
                    temp = Convert.ToDouble(string.Format("{0:N3}", temp));
                    //文档中有对于无效值的描述，但是因为现在PecWave没有做无效值判断，所以暂时不加上这个判断。后面如果需要的话，再加上。
                    //if(temp==99999)
                    //    temp=double.NaN;
                }
                else
                {
                    temp = double.NaN;
                }

                datData._AkList.Add(temp);
            }

        }
    }

    /// <summary>
    /// 1.ASCII格式的DAT文件总体格式
    ///n,timestamp,A1,A2,……Ak,D1,D2,……Dm<CR/LF>
    ///2.说明如下：
    ///n：采样序号，必填，整数，1～9999999999
    ///timestamp：时间标识；基本单位微秒；整数，1～10个字符宽度
    ///如果CFG文件中的nrates和samp非零，则可选；
    ///如果CFG文件中的nrates和samp为零，则必填；
    ///A1…AK: 模拟量各个通道数据，可选，整数，-99999～99998
    ///	填入99999表示无效
    ///D1…Dm: 开关量各个通道数据，可选，整数，0或1
    //	填入99999表示无效
    /// </summary>
    internal class DatParserASCII : DatFileParser
    {

    }

    /// <summary>
    /// 一个波形数据，包括一个cfg文件，一个dat文件
    /// </summary>
    public class OneWaveData
    {
        public CfgData _CfgData;
        public DatData _DatData;
    }

    internal class OneWaveResultData
    {
        /// <summary>
        /// cfg文件的数据
        /// </summary>
        public CfgData _CfgData;

        /// <summary>
        /// n：采样序号，必填，整数，1～9999999999
        /// </summary>
        public List<uint> _NList;

        /// <summary>
        /// timestamp：时间标识
        /// </summary>
        public List<double> _TimestampList;

        /// <summary>
        /// A1…AK: 模拟量各个通道数据.key是An,ch_id,ph,ccbm,uu,a,b,skew,min,max,primary,secondary,PS<CR/LF>中的An
        /// </summary>
        public Dictionary<string, List<double>> _ValueMoniLiangDic;

        /// <summary>
        /// S1…Sm: 开关量各个通道数据.key是 Dn,ch_id,ph,ccbm,y<CR/LF>中的Dn
        /// </summary>
        public Dictionary<string, List<double>> _KaiGuanDic;


        public OneWaveResultData()
        {
            _CfgData = new CfgData();
            _NList = new List<uint>();
            _TimestampList = new List<double>();
            _ValueMoniLiangDic = new Dictionary<string, List<double>>();
            _KaiGuanDic = new Dictionary<string, List<double>>();
        }


    }

    public class TotalCombinedWave
    {
        public List<CfgData> _CfgDataList;

        /// <summary>
        /// 时间
        /// </summary>
        public List<double> _TimeList;

        /// <summary>
        /// 所有的数据。key是数据对应的字符串，例如Number,Cycle,Ua,Ub,Uc,U4,Ia,Ib,Ic,I4,I5,
        /// </summary>
        public Dictionary<string, List<double>> _ValueDic;

        public TotalCombinedWave()
        {
            _CfgDataList = new List<CfgData>();
            _TimeList = new List<double>();
            _ValueDic = new Dictionary<string, List<double>>();
        }
    }

    public class WaveDataDecodeManager
    {
        private static void GetWaveDataDatInfo_BINARY(BinaryReader binReader, CfgData cfgData, out List<DatDataOneLine> resultList, uint endSamp)
        {
            resultList = new System.Collections.Generic.List<DatDataOneLine>();
            for (int i = 0; i < endSamp; i++)
            {
                DatDataOneLine datDataOneLine = new DatDataOneLine();
                resultList.Add(datDataOneLine);

                datDataOneLine._N = binReader.ReadUInt32();
                datDataOneLine._timestamp = binReader.ReadUInt32();

                for (int m = 0; m < cfgData._SecondLine._OOA; ++m)
                {
                    var aValue = binReader.ReadInt16();
                    datDataOneLine._AkList.Add(aValue);
                }

                //两字节表示16个开关量
                var Dm = new int[cfgData._SecondLine._OOD];
                int ioBatchCount = (int) Math.Ceiling(cfgData._SecondLine._OOD/16.0); //计算需要读取多少次Uint16值作为开关量
                for (int n = 0; n < ioBatchCount; n++)
                {
                    UInt16 ioValue = binReader.ReadUInt16();
                    for (int j = 0; j < 16; j++)
                    {
                        int dmIndex = j + n*16;
                        if (dmIndex == Dm.Length)
                            break;
                        var io = (UInt16) (ioValue >> j & 1);
                        datDataOneLine._DmList.Add(io);
                    }
                }
            }
        }

        /// <summary>
        /// 以回车换行符为间隔解析comtrade格式波形数据为字符串列表
        /// </summary>
        /// <param name="memStream">波形数据流</param>
        /// <param name="resultList">解析字符串串结果列表</param>
        /// <returns>是否成功</returns>
        public static bool ParseWaveData(MemoryStream memStream, out CfgData cfgData, out List<DatDataOneLine> resultList)
        {
            resultList = new System.Collections.Generic.List<DatDataOneLine>();
            cfgData = null;

            if (memStream == null)
                return false;
            try
            {
                BinaryReader binReader;
                // string[] parseCfgFileStrs;
                //只需要模拟量数据，解析模拟量通道数
                int totalAChNum;
                int totalDChNum;
                int endSamp;
                string[] parseCfgFileStrs;
                GetCFGFile(memStream, out binReader, out parseCfgFileStrs);
                if (!CfgFileParser.ParseCfgFile(parseCfgFileStrs, out cfgData))
                    return false;

                //读取Data波形文件二进制文件长度
                int dataLen = binReader.ReadInt32();
                //分Binary和ASCII两种方式解析
                if (parseCfgFileStrs[parseCfgFileStrs.Length - 2] == "BINARY")
                    GetWaveDataDatInfo_BINARY(binReader, cfgData, out resultList, cfgData._SampInformation._SampList[cfgData._SampInformation._SampList.Count - 1]._Endsamp);
                else
                    GetWaveDataDatInfo_ASCII2(binReader, cfgData, out resultList, dataLen);
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 以回车换行符为间隔解析comtrade格式波形数据为字符串列表
        /// </summary>
        /// <param name="memStream">波形数据流</param>
        /// <param name="resultList">解析字符串串结果列表</param>
        /// <returns>是否成功</returns>
        public static bool ParseWaveData(MemoryStream memStream, out string[] parseCfgFileStrs, out List<string> resultList)
        {
            resultList = new List<string>();
            parseCfgFileStrs = null;
            if (memStream == null)
                return false;
            try
            {
                BinaryReader binReader;
                // string[] parseCfgFileStrs;
                //只需要模拟量数据，解析模拟量通道数
                int totalAChNum;
                int totalDChNum;
                int endSamp;
                GetCFGFile(memStream, out binReader, out parseCfgFileStrs, out totalAChNum, out totalDChNum, out endSamp);

                //读取Data波形文件二进制文件长度
                int dataLen = binReader.ReadInt32();
                //分Binary和ASCII两种方式解析
                if (parseCfgFileStrs[parseCfgFileStrs.Length - 2] == "BINARY")
                    GetWaveDataDatInfo_BINARY(binReader, ref resultList, totalAChNum, totalDChNum, endSamp);
                else
                    GetWaveDataDatInfo_ASCII(binReader, ref resultList, dataLen);
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 以回车换行符为间隔解析comtrade格式波形数据为字符串列表
        /// </summary>
        /// <param name="memStream">波形数据流</param>
        /// <param name="resultList">解析字符串串结果列表</param>
        /// <returns>是否成功</returns>
        public static List<string> ParseWaveData(MemoryStream memStream)
        {
            List<string> resultList = new List<string>();
            if (memStream == null)
                return resultList;
            try
            {
                BinaryReader binReader;
                string[] parseCfgFileStrs;
                //只需要模拟量数据，解析模拟量通道数
                int totalAChNum;
                int totalDChNum;
                int endSamp;
                GetCFGList(memStream, ref resultList, out binReader, out parseCfgFileStrs, out totalAChNum, out totalDChNum, out endSamp);

                //读取Data波形文件二进制文件长度
                int dataLen = binReader.ReadInt32();
                //分Binary和ASCII两种方式解析
                if (parseCfgFileStrs[parseCfgFileStrs.Length - 2] == "BINARY")
                    GetWaveDataDatInfo_BINARY(binReader, ref resultList, totalAChNum, totalDChNum, endSamp);
                else
                    GetWaveDataDatInfo_ASCII(binReader, ref resultList, dataLen);
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }

            return resultList;
        }

        private static bool GetCFGFile(MemoryStream memStream, out BinaryReader binReader, out string[] parseCfgFileStrs, out int totalAChNum, out int totalDChNum, out int endSamp)
        {
            bool isNewComtrade = false;
            memStream.Position = 0;
            binReader = new BinaryReader(memStream);
            //头标志，用来和旧的录波格式区分
            uint flag1 = binReader.ReadUInt32();
            if (flag1 == 0xFFFFFFFF)
            {
                isNewComtrade = true;
                //新的波形文件结构进行了调整，需要重新定位，该数据是3.5结构的波形数据
                memStream.Position = 8 + 4 + 4;
            }
            uint hdrLen = 0;
            if (isNewComtrade)
            {
                //获取hdr波形文件文件长度 4
                hdrLen = binReader.ReadUInt32();
                if (hdrLen < 0)
                    memStream.Position += hdrLen;
            }
            int cfgLen = (int) flag1;
            //读取cfg波形文件的长度
            if (isNewComtrade)
                cfgLen = (int) binReader.ReadUInt32();

            //读取cfg波形文件字节(ASCI码)
            byte[] cfgBuffer = binReader.ReadBytes(cfgLen);
            //将cfg波形文件字节转换为字符串
            string cfgFileStr = System.Text.Encoding.Default.GetString(cfgBuffer);
            //以回车换行符为间隔将cfg波形文件进行分割
            parseCfgFileStrs = cfgFileStr.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //解析波形通道总数
            string[] tTADStrs = parseCfgFileStrs[1].Split(',');
            int totalChNum = Convert.ToInt32(tTADStrs[0]);
            totalAChNum = Convert.ToInt32(tTADStrs[1].Substring(0, tTADStrs[1].Length - 1));
            totalDChNum = totalChNum - totalAChNum;
            //Samp采样率最后一次采样的序号
            int nrates = Convert.ToInt32(parseCfgFileStrs[1 + totalChNum + 2]);
            string[] sampInfoStrs = parseCfgFileStrs[1 + totalChNum + 2 + nrates].Split(',');
            endSamp = Convert.ToInt32(sampInfoStrs[1]);
            return true;
        }

        private static bool GetCFGFile(MemoryStream memStream, out BinaryReader binReader, out string[] parseCfgFileStrs)
        {
            bool isNewComtrade = false;
            memStream.Position = 0;
            binReader = new BinaryReader(memStream);
            //头标志，用来和旧的录波格式区分
            uint flag1 = binReader.ReadUInt32();
            if (flag1 == 0xFFFFFFFF)
            {
                isNewComtrade = true;
                //新的波形文件结构进行了调整，需要重新定位，该数据是3.5结构的波形数据
                memStream.Position = 8 + 4 + 4;
            }
            uint hdrLen = 0;
            if (isNewComtrade)
            {
                //获取hdr波形文件文件长度 4
                hdrLen = binReader.ReadUInt32();
                if (hdrLen < 0)
                    memStream.Position += hdrLen;
            }
            int cfgLen = (int) flag1;
            //读取cfg波形文件的长度
            if (isNewComtrade)
                cfgLen = (int) binReader.ReadUInt32();

            //读取cfg波形文件字节(ASCI码)
            byte[] cfgBuffer = binReader.ReadBytes(cfgLen);
            //将cfg波形文件字节转换为字符串
            string cfgFileStr = System.Text.Encoding.Default.GetString(cfgBuffer);
            //以回车换行符为间隔将cfg波形文件进行分割
            parseCfgFileStrs = cfgFileStr.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return true;
        }



        public static List<string> GetCFGList(MemoryStream memStream, ref List<string> resultList, out BinaryReader binReader, out string[] parseCfgFileStrs, out int totalAChNum, out int totalDChNum, out int endSamp)
        {
            GetCFGFile(memStream, out binReader, out parseCfgFileStrs, out totalAChNum, out totalDChNum, out endSamp);
            resultList.AddRange(parseCfgFileStrs);
            return resultList;
        }



        private static void GetWaveDataDatInfo_BINARY(BinaryReader binReader, ref List<string> resultList, int totalChNum, int totalDChNum, int endSamp)
        {
            for (int i = 0; i < endSamp; i++)
            {
                string dataFileStr = string.Empty;
                uint index = binReader.ReadUInt32();
                uint timeStamp = binReader.ReadUInt32();
                dataFileStr = string.Format("{0},{1},{2}", dataFileStr, index, timeStamp);
                //对于开关量16个开关量为一组解析，根据开关量个数来确定组数
                double dChannelGroupCount = Math.Ceiling(Convert.ToDouble(totalDChNum)/16);
                for (int j = 0; j < totalChNum + dChannelGroupCount; j++)
                {
                    int aValue = binReader.ReadInt16();
                    //将字节流转换为字符串
                    dataFileStr = string.Format("{0},{1}", dataFileStr, aValue);
                }

                //两字节表示16个开关量
                int ioBatchCount = (int) Math.Ceiling(Convert.ToDouble(totalDChNum)/16.0); //计算需要读取多少次Uint16值作为开关量
                int settedIoCount = 0;
                for (int k = 0; k < ioBatchCount; k++)
                {
                    UInt16 ioValueSet = binReader.ReadUInt16(); //存储多个开关量
                    for (int j = 0; j < 16; j++)
                    {
                        int dmIndex = j + k*16;
                        if (dmIndex == totalDChNum)
                            break;
                        int io = (UInt16) (ioValueSet >> j & 1);
                        dataFileStr = string.Format("{0},{1}", dataFileStr, io);
                    }
                }
                resultList.Add(dataFileStr);
            }
        }

        private static void GetWaveDataDatInfo_ASCII(BinaryReader binReader, ref List<string> resultList, int dataLength)
        {
            byte[] cfgBuffer = binReader.ReadBytes(dataLength);
            //将cfg波形文件字节转换为字符串
            string dataFileStr = System.Text.Encoding.Default.GetString(cfgBuffer);
            string[] parseDatFileStrs = dataFileStr.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parseDatFileStrs.Length; i++)
            {
                if (parseDatFileStrs[i].Length > 3)
                {
                    resultList.Add("," + parseDatFileStrs[i]);
                }
            }
        }

        private static bool GetWaveDataDatInfo_ASCII2(BinaryReader binReader, CfgData cfgData, out List<DatDataOneLine> resultList, int dataLength)
        {
            List<string> datStringList = new System.Collections.Generic.List<string>();
            GetWaveDataDatInfo_ASCII(binReader, ref datStringList, dataLength);
            if (!DatFileParser.ParseDatFile(datStringList, cfgData._SecondLine._OOA, cfgData._SecondLine._OOD, out resultList))
                return false;

            return true;
        }

        /// <summary>
        /// 根据解析的字符串获取指定位置的数值填充到结果结构中
        /// </summary>
        /// <param name="comtradeWaveDataList">波形文件数据列表</param>
        /// <param name="waveChannelIndexAndPos">波形通道序号和位置</param>
        /// <param name="waveChannelNum">波形通道数目</param>
        /// <param name="resultData">结果集</param>
        /// <returns>是否成功</returns>
        public static void GetQueryOriginalWaveData(ResultWaveDataView resultData, List<string> comtradeWaveDataList)
        {
            if (comtradeWaveDataList == null)
                return;
            try
            {
                //解析波形通道总数信息
                string[] tTADStrs = GetTotalChannelNumber(comtradeWaveDataList);
                //解析模拟量通道信息
                int aChannelNum;
                List<AChannelInfo> aChannelInfoList = GetAnalogInfo(comtradeWaveDataList, tTADStrs, out aChannelNum);
                //解析开关量通道信息
                int dChannelNum = Convert.ToInt32(tTADStrs[2].Trim('D'));
                //解析采样率个数
                int ratesNum = 0;
                if (!string.IsNullOrEmpty(comtradeWaveDataList[2 + aChannelNum + dChannelNum + 1]))
                {
                    ratesNum = Convert.ToInt32(comtradeWaveDataList[2 + aChannelNum + dChannelNum + 1]);
                }
                //解析频率
                double frequency = double.NaN;
                if (!string.IsNullOrEmpty(comtradeWaveDataList[2 + aChannelNum + dChannelNum]))
                {
                    frequency = Convert.ToDouble(comtradeWaveDataList[2 + aChannelNum + dChannelNum]);
                }
                //获取采样率信息
                List<SampInfo> sampInfoList = GetSampInfo(comtradeWaveDataList, aChannelNum, dChannelNum, ratesNum);
                GetWaveTimeInfo(comtradeWaveDataList, resultData, aChannelNum, dChannelNum, frequency, ratesNum, sampInfoList);
                //获取采样点信息以及采样率对应的序号
                GetVIDataListStruct(resultData, aChannelNum, frequency, ratesNum, sampInfoList);
                for (int i = 0; i < aChannelNum; i++)
                {
                    //解析波形通道总数，模拟量通道个数，开关量通道个数
                    string[] channelName = comtradeWaveDataList[2 + i].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    //计算波形打点数据
                    double[] tempChVal = CaculateWaveData(comtradeWaveDataList, aChannelNum, i, aChannelInfoList, dChannelNum, ratesNum, sampInfoList);
                    //通道不存在，添加对应数据，通道存在，合并数据
                    if (!resultData.ChannelNameList.Contains(channelName[1]))
                    {
                        resultData.ChannelNameList.Add(channelName[1]);
                        resultData.ChannelValList.Add(tempChVal);
                    }
                    else
                    {
                        int index = 0;
                        for (int j = 0; j < resultData.ChannelNameList.Count; j++)
                            if (resultData.ChannelNameList[j] == channelName[1])
                                index = j;
                        //如果已经存在对应的通道，合并数据
                        double[] existValueData = resultData.ChannelValList[index];
                        double[] combinValueData = new double[tempChVal.Length + existValueData.Length];
                        existValueData.CopyTo(combinValueData, 0);
                        tempChVal.CopyTo(combinValueData, existValueData.Length);
                        resultData.ChannelValList[index] = combinValueData;
                        CountPerCycAndEndIndexInfo newCycleIndexInfo = new CountPerCycAndEndIndexInfo();
                        newCycleIndexInfo.CountPerCycle = resultData.CountPerCycleAndIndexList[resultData.CountPerCycleAndIndexList.Count - 1].CountPerCycle;
                        newCycleIndexInfo.LastIndex = tempChVal.Length + existValueData.Length;
                        resultData.CountPerCycleAndIndexList[resultData.CountPerCycleAndIndexList.Count - 1] = newCycleIndexInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }
        }

        private static double[] CaculateWaveData(List<string> comtradeWaveDataList, int aChannelNum, int aChannelIndex, List<AChannelInfo> aChannelInfoList, int dChannelNum, int ratesNum, List<SampInfo> sampInfoList)
        {
            double[] tempChVal = new double[sampInfoList[ratesNum - 1].EndSamp];
            int row = 0;
            while (row < tempChVal.Length)
            {
                //解析*.dat文件中数据
                string[] dataInfoStrs = comtradeWaveDataList[2 + aChannelNum + dChannelNum + 2 + ratesNum + 4 + row].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                //数值：（通道转换公式为ax+b， x取自.DAT文件， 单位就是uu）
                double tempValue = double.NaN;
                tempValue = aChannelInfoList[aChannelIndex].ChannelCoef*Convert.ToDouble(dataInfoStrs[aChannelIndex + 2]) + aChannelInfoList[aChannelIndex].ChannelOff;
                tempChVal[row] = DataFormatManager.GetFormattedDoubleByDigits(tempValue, 3); //保留3位小数   
                row++;
            }
            return tempChVal;
        }

        private static void GetVIDataListStruct(ResultWaveDataView resultData, int aChannelNum, double frequency, int ratesNum, List<SampInfo> sampInfoList)
        {
            //每周波采样点数和最后一次采样序号
            CountPerCycAndEndIndexInfo countPerCycAndEndIndexDef = new CountPerCycAndEndIndexInfo();
            for (int k = 0; k < ratesNum; k++)
            {
                countPerCycAndEndIndexDef.CountPerCycle = Convert.ToInt32(sampInfoList[k].Samp/frequency);
                countPerCycAndEndIndexDef.LastIndex = sampInfoList[k].EndSamp;
                resultData.CountPerCycleAndIndexList.Add(countPerCycAndEndIndexDef);
            }
        }

        private static double GetTotalCycleNumber(double frequency, int ratesNum, List<SampInfo> sampInfoList)
        {
            double totalCycleNum = 0;
            for (int k = 0; k < ratesNum; k++)
            {
                if (k == 0)
                {
                    totalCycleNum += (sampInfoList[k].EndSamp*frequency)/sampInfoList[k].Samp;
                }
                else
                {
                    totalCycleNum += ((sampInfoList[k].EndSamp - sampInfoList[k - 1].EndSamp)*frequency)/sampInfoList[k].Samp;
                }
            }
            return totalCycleNum;
        }

        private static void GetWaveTimeInfo(List<string> comtradeWaveDataList, ResultWaveDataView resultData, int aChannelNum, int dChannelNum, double frequency, int ratesNum, List<SampInfo> sampInfoList)
        {
            //解析数据文件中第一个数据的日期时间标识
            string startTime = comtradeWaveDataList[2 + aChannelNum + dChannelNum + 2 + ratesNum];
            //解析触发时间
            string triggerTime = comtradeWaveDataList[2 + aChannelNum + dChannelNum + 2 + ratesNum + 1];

            //获取起始时间
            DateTime tempStartTime = DateTime.MinValue;
            ParseStrToDateTime(startTime, "/", ref tempStartTime);
            resultData.StartTime = tempStartTime;
            //获取触发时间
            DateTime tempTriggerTime = DateTime.MinValue;
            ParseStrToDateTime(triggerTime, "/", ref tempTriggerTime);
            resultData.TriggerTime = tempTriggerTime;
            //采样频率
            resultData.Frequency = frequency;
            //采样点总数
            resultData.TotalNum = sampInfoList[ratesNum - 1].EndSamp;
        }

        private static List<AChannelInfo> GetAnalogInfo(List<string> comtradeWaveDataList, string[] tTADStrs, out int aChannelNum)
        {
            //解析模拟量通道信息
            aChannelNum = Convert.ToInt32(tTADStrs[1].Trim('A'));
            List<AChannelInfo> aChannelInfoList = new List<AChannelInfo>();
            for (int i = 0; i < aChannelNum; i++)
            {
                string[] aChannelInfoStrs = comtradeWaveDataList[2 + i].Split(",".ToCharArray());
                AChannelInfo aChannelDef = new AChannelInfo();
                aChannelDef.ChannelCoef = double.NaN;
                if (!string.IsNullOrEmpty(aChannelInfoStrs[5]))
                {
                    aChannelDef.ChannelCoef = Convert.ToDouble(aChannelInfoStrs[5]);
                }

                aChannelDef.ChannelOff = double.NaN;
                if (!string.IsNullOrEmpty(aChannelInfoStrs[6]))
                {
                    aChannelDef.ChannelOff = Convert.ToDouble(aChannelInfoStrs[6]);
                }

                aChannelInfoList.Add(aChannelDef);
            }
            return aChannelInfoList;
        }

        private static string[] GetTotalChannelNumber(List<string> comtradeWaveDataList)
        {
            //解析波形通道总数，模拟量通道个数，开关量通道个数
            string[] tTADStrs = comtradeWaveDataList[1].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            TTADInfo tTADDef = new TTADInfo();
            tTADDef.TotalChannelNum = 0;
            if (!Convert.IsDBNull(tTADStrs[0]) || !string.IsNullOrEmpty(tTADStrs[0]))
            {
                tTADDef.TotalChannelNum = Convert.ToInt32(tTADStrs[0]);
            }
            return tTADStrs;
        }

        private static List<SampInfo> GetSampInfo(List<string> comtradeWaveDataList, int aChannelNum, int dChannelNum, int ratesNum)
        {
            //解析采样率信息
            List<SampInfo> sampInfoList = new List<SampInfo>();
            for (int k = 0; k < ratesNum; k++)
            {
                string[] sampInfoStrs = comtradeWaveDataList[2 + aChannelNum + dChannelNum + 2 + k].Split(",".ToCharArray());
                SampInfo sampDef = new SampInfo();
                sampDef.Samp = double.NaN;
                if (!string.IsNullOrEmpty(sampInfoStrs[0]))
                {
                    sampDef.Samp = Convert.ToDouble(sampInfoStrs[0]);
                }
                sampDef.EndSamp = 0;
                if (!string.IsNullOrEmpty(sampInfoStrs[1]))
                {
                    sampDef.EndSamp = Convert.ToInt32(sampInfoStrs[1]);
                }
                sampInfoList.Add(sampDef);
            }
            return sampInfoList;
        }

        /// <summary>
        /// 将cfg中dd/MM/yyyy,hh:mm:ss.ssssss装换为MM/dd/yyyy,hh:mm:ss.ssssss格式时间
        /// </summary>
        /// <param name="datetimeStr">日期时间字符</param>
        /// <param name="seperator">分隔符</param>
        /// <param name="resultDateTime">格式化后的日期时间</param>
        /// <returns>是否成功</returns>
        private static bool ParseStrToDateTime(string datetimeStr, string seperator, ref DateTime resultDateTime)
        {
            if (string.IsNullOrEmpty(datetimeStr))
                return false;
            try
            {
                string[] resultStr = datetimeStr.Split(seperator.ToCharArray());
                string combDateTimeStr = string.Format("{0}/{1}/{2}", resultStr[1], resultStr[0], resultStr[2]);
                bool success = DateTime.TryParse(combDateTimeStr, out resultDateTime);
                if (success)
                    return true;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(String.Format("{0}{1}", "ParseStrToDateTime", ex.Message));
            }
            return false;
        }
    }

    /// <summary>
    /// 用于存储波形数据结果的类
    /// </summary>
    public class ResultWaveDataView
    {
        /// <summary>
        /// 起始时间
        /// </summary>
        private DateTime startTime;

        /// <summary>
        /// 触发时间
        /// </summary>
        private DateTime triggerTime;

        /// <summary>
        /// 起始时间
        /// </summary>
        private int numberOfCycle;

        public int NumberOfCycle
        {
            get { return numberOfCycle; }
            set { numberOfCycle = value; }
        }

        /// <summary>
        /// 频率
        /// </summary>
        private double frequency;

        /// <summary>
        /// 总点数
        /// </summary>
        private int totalNum;

        /// <summary>
        /// 每周波采样点数和最后一次采样序号列表
        /// </summary>
        private List<CountPerCycAndEndIndexInfo> countPerCycleAndIndexList;

        /// <summary>
        /// 通道名称
        /// </summary>
        private List<string> channelNameList;

        public List<string> ChannelNameList
        {
            get { return channelNameList; }
            set { channelNameList = value; }
        }

        /// <summary>
        /// 通道数据
        /// </summary>
        private List<double[]> channelValList;

        /// <summary>
        /// Initializes a new instance of the ResultWaveDataView class
        /// </summary>
        /// <param name="startTime">起始时间</param>
        /// <param name="triggerTime">触发时间</param>
        /// <param name="frequency">频率</param>
        /// <param name="totalNum">总点数</param>
        public ResultWaveDataView(DateTime startTime, DateTime triggerTime, double frequency, int totalNum)
        {
            this.startTime = startTime;
            this.triggerTime = triggerTime;
            this.frequency = frequency;
            this.totalNum = totalNum;
            this.countPerCycleAndIndexList = new List<CountPerCycAndEndIndexInfo>();
            this.channelValList = new List<double[]>();
            this.channelNameList = new List<string>();
        }

        #region 公共属性

        /// <summary>
        /// Gets or sets 起始时间
        /// </summary>
        public DateTime StartTime
        {
            get { return this.startTime; }
            set { this.startTime = value; }
        }

        /// <summary>
        /// Gets or sets 触发时间
        /// </summary>
        public DateTime TriggerTime
        {
            get { return this.triggerTime; }
            set { this.triggerTime = value; }
        }

        /// <summary>
        /// Gets or sets 频率
        /// </summary>
        public double Frequency
        {
            get { return this.frequency; }
            set { this.frequency = value; }
        }

        /// <summary>
        /// Gets or sets 总点数
        /// </summary>
        public int TotalNum
        {
            get { return this.totalNum; }
            set { this.totalNum = value; }
        }

        /// <summary>
        /// Gets or sets 每周波采样点数和最后一次采样序号列表
        /// </summary>
        public List<CountPerCycAndEndIndexInfo> CountPerCycleAndIndexList
        {
            get { return this.countPerCycleAndIndexList; }
            set { this.countPerCycleAndIndexList = value; }
        }

        /// <summary>
        /// Gets or sets 通道数据
        /// </summary>
        public List<double[]> ChannelValList
        {
            get { return this.channelValList; }
            set { this.channelValList = value; }
        }

        #endregion
    }

    /// <summary>
    /// 波形通道数量信息
    /// </summary>
    public struct TTADInfo
    {
        /// <summary>
        /// 波形通道总个数
        /// </summary>
        public int TotalChannelNum;

        /// <summary>
        /// 模拟量通道个数
        /// </summary>
        public string AChannelNum;

        /// <summary>
        /// 开关量通道个数
        /// </summary>
        public string DChannelNum;
    }

    /// <summary>
    /// 存放每周波采用点数和最后一次采样序号
    /// </summary>
    public struct CountPerCycAndEndIndexInfo
    {
        /// <summary>
        /// 每周波采样点数
        /// </summary>
        public int CountPerCycle;

        /// <summary>
        /// 该采样率下最后一次采用序号
        /// </summary>
        public int LastIndex;
    }

    /// <summary>
    /// 模拟量通道信息，由##A决定有多少行，一行表示一个模拟量通道信息
    /// </summary>
    public struct AChannelInfo
    {
        /// <summary>
        /// 通道系数；必填；实数；1～32个字符宽度；可使用标准浮点记法
        /// </summary>
        public double ChannelCoef;

        /// <summary>
        ///  通道偏移量；必填；实数；1～32个字符宽度；可使用标准浮点数记法
        /// （通道转换公式为ax+b， x取自.DAT文件， 单位就是uu）
        /// </summary>
        public double ChannelOff;
    }

    /// <summary>
    /// 采样率信息
    /// 用多个采样率，就有多少行“samp,endsamp”；
    /// 用一个采样率，nrates填0，samp填0，endsamp填data文件中的最后一个采样序号
    /// </summary>
    public struct SampInfo
    {
        /// <summary>
        /// 采用率，单位Hz，必填；实数；1～32字符宽度；可使用标准浮点数记法
        /// </summary>
        public double Samp;

        /// <summary>
        /// 用Samp采样率最后一次采样的序号；必填；整数；1～9999999999
        /// </summary>
        public int EndSamp;
    }

    /// <summary>
    /// 波形数据断面，一个时刻的波形数据
    /// </summary>
    public class WaveSlice
    {
        /// <summary>
        /// 采样序号，从1开始
        /// </summary>
        public int index;

        /// <summary>
        /// 时间标识；基本单位微秒；整数，1～10个字符宽度
        /// 如果CFG文件中的nrates和samp非零，则可选；
        /// 如果CFG文件中的nrates和samp为零，则必填；
        /// </summary>
        public uint timeStamp;

        /// <summary>
        /// 模拟量各个通道数据，可选，整数，-99999～99998，填入99999表示无效
        /// </summary>
        public Int32[] Ak;

        /// <summary>
        /// 开关量各个通道数据，可选，整数，0或1，填入99999表示无效
        /// </summary>
        public Int32[] Dm;
    }

    #endregion

    #region Worst case相关

    public class CommonUsedFunctionsForWorstCase
    {
        public static void GetSortedRegionDic(List<SagSwellEvent> regionBList, out SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> regionBDic)
        {
            regionBDic = new SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>>();
            foreach (var item in regionBList)
            {
                AddOneItem(regionBDic, item);
            }
        }

        /// <summary>
        /// 幅值越大越严重，幅值相同的，持续时间越长越严重；
        /// </summary>
        /// <param name="worstCaseList"></param>
        /// <param name="regionBDic"></param>
        public static void GetWorstCaseForMaxMag(out List<SagSwellEvent> worstCaseList, SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> regionBDic)
        {
            worstCaseList = new List<SagSwellEvent>();
            if (regionBDic == null)
                return;
            if (regionBDic.Count == 0)
                return;
            //取regionBDic的最后一个元素，其幅值最大。
            var maxItemMag = regionBDic.Last();
            if (maxItemMag.Value.Count == 0)
                return;
            //持续时间越长越严重；
            worstCaseList = maxItemMag.Value.Last().Value;
        }

        public static void GetSwellDipTransientList(List<SagSwellEvent> regionAList, out SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> swellList, out SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> dipList, out SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> transientList)
        {
            swellList = new SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>>();
            dipList = new SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>>();
            transientList = new SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>>();
            foreach (var item in regionAList)
            {
                EventTypeEnum eventTypeLikeSwell;
                EventTypeManager.GetEventTypeLikeSwellEnum(item.EventCode1, item.FetureInfo.TotalMag, item.EventType, out eventTypeLikeSwell);

                //判断是不是swell
                if (IsSwell(eventTypeLikeSwell))
                {
                    AddOneItem(swellList, item);
                    continue;
                }
                //判断是不是Dip
                if (IsDip(eventTypeLikeSwell))
                {
                    AddOneItem(dipList, item);
                    continue;
                }
                //判断是不是Interrupt
                if (IsInterrupt(eventTypeLikeSwell))
                {
                    AddOneItem(dipList, item);
                    continue;
                }
                //判断是不是transient
                if (IsTransient(eventTypeLikeSwell))
                {
                    AddOneItem(transientList, item);
                    continue;
                }
            }
        }

        private static bool IsTransient(EventTypeEnum eventTypeLikeSwell)
        {
            return eventTypeLikeSwell == EventTypeEnum.Transient;
        }

        private static bool IsInterrupt(EventTypeEnum eventTypeLikeSwell)
        {
            return eventTypeLikeSwell == EventTypeEnum.Interruption;
        }

        private static bool IsDip(EventTypeEnum eventTypeLikeSwell)
        {
            return eventTypeLikeSwell == EventTypeEnum.Sag;
        }

        private static bool IsSwell(EventTypeEnum eventTypeLikeSwell)
        {
            return eventTypeLikeSwell == EventTypeEnum.Swell;
        }

        public static void GetWorstCaseForMinMag(out List<SagSwellEvent> worstCaseList, SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> regionCDic)
        {
            worstCaseList = new List<SagSwellEvent>();
            if (regionCDic == null)
                return;
            if (regionCDic.Count == 0)
                return;
            var maxItemMag = regionCDic.First();

            if (maxItemMag.Value.Count == 0)
                return;
            //持续时间越长越严重；
            worstCaseList = maxItemMag.Value.Last().Value;
        }

        public static void AddOneItem(SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> regionBDic, SagSwellEvent item)
        {
            SortedDictionary<double, List<SagSwellEvent>> oneMag;
            double mag = GetChangedTotalMagForCompareWorseCase(item.FetureInfo.TotalMag);
            if (!regionBDic.TryGetValue(mag, out oneMag))
            {
                oneMag = new SortedDictionary<double, List<SagSwellEvent>>();
                regionBDic.Add(mag, oneMag);
            }
            List<SagSwellEvent> oneDuration;
            double duration = GetSaved3Or6PointForDuration(item.FetureInfo.Duration);
            if (!oneMag.TryGetValue(duration, out oneDuration))
            {
                oneDuration = new List<SagSwellEvent>();
                oneMag.Add(duration, oneDuration);
            }
            oneDuration.Add(item);
        }

        /// <summary>
        /// 用于比较最坏事件的时候
        /// </summary>
        /// <param name="totalMag"></param>
        /// <returns></returns>
        private static double GetChangedTotalMagForCompareWorseCase(double totalMag)
        {
            return GetChangedTotalMagForPoint(totalMag);
        }

        /// <summary>
        /// 用于打点，对负数做处理，Magnitude (%): 263.78% = |-163.78% | + 100%
        /// </summary>
        /// <param name="totalMag"></param>
        /// <returns></returns>
        public static double GetChangedTotalMagForPoint(double totalMag)
        {
            if (totalMag < 0)
            {
                //Magnitude (%): 263.78% = |-163.78% | + 100%
                totalMag = Math.Abs(totalMag); //瞬变事件幅值有可能是复数，需要取绝对值后判断在哪个区域
                totalMag = totalMag + 100;
            }
            //保留3位小数
            totalMag = GetSave3Point(totalMag);
            return totalMag;
        }

        /// <summary>
        /// 显示到浮动信息中的，如果是负数，就要写成
        /// </summary>
        /// <param name="totalMag"></param>
        /// <returns></returns>
        public static string GetMagForFloat(string mag)
        {
            double totalMagWith3Point;
            if (!double.TryParse(mag, out totalMagWith3Point))
                return mag;
            if (totalMagWith3Point >= 0)
                return mag;
            double add100 = GetChangedTotalMagForPoint(totalMagWith3Point);
            //小于0的时候返回字符串“Magnitude (%): 263.78% = |-163.78% | + 100%” instead of just -163.78% or 263.78%.
            string result = string.Format("{0}% = |{1}%| + 100%", GetStingByDouble3Point(add100),
                GetStingByDouble3Point(totalMagWith3Point));
            return result;
        }

        public static double GetSave3Point(double n)
        {
            string temp = GetStingByDouble3Point(n);
            double totalMagWith3Point;
            if (!double.TryParse(temp, out totalMagWith3Point))
                return double.NaN;
            return totalMagWith3Point;
        }

        /// <summary>
        /// 保留3位小数，如果保留3位小数后成为了0，就保留6位小数
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static double GetSaved3Or6PointForDuration(double duration)
        {
            double result = GetSave3Point(duration);
            if (result != 0)
                return result;
            //保留6位小数
            string temp = string.Format("{0:N6}", duration);
            if (!double.TryParse(temp, out result))
                return double.NaN;
            return result;
        }

        /// <summary>
        /// double值保留3位小数，转换成字符串
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string GetStingByDouble3Point(double n)
        {
            if (double.IsNaN(n))
                return EventInformation._NothingString;
            return string.Format("{0:N3}", n);
        }

        /// <summary>
        /// double值保留6位小数，转换成字符串
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string GetStingByDouble6Point(double n)
        {
            if (double.IsNaN(n))
                return EventInformation._NothingString;
            return string.Format("{0:N6}", n);
        }

        /// <summary>
        /// 根据输入的两个点构成的线段，以及新的X坐标值，获取Y坐标值
        /// </summary>
        /// <param name="point1">第一个坐标点(X轴值比第二个坐标X轴值小)</param>
        /// <param name="point2">第二个坐标点</param>
        /// <param name="x_value">X轴值</param>
        /// <param name="y_value">输出，Y轴值</param>
        /// <returns>是否找到值</returns>
        public static bool GetYValueByXValue(SARFIChartValue point1, SARFIChartValue point2, double x_value, out double y_value)
        {
            bool result = true;

            //如果为垂直直线，那么返回最大值
            if (point1.XValue == point2.XValue && point1.XValue == x_value)
            {
                y_value = Double.MaxValue;
                return true;
            }

            //如果不在线段范围内，则查找失败
            if (x_value < point1.XValue || x_value > point2.XValue)
            {
                y_value = Double.NaN;
                return false;
            }

            // 计算斜率    
            double k = (point1.YValue - point2.YValue)/(point1.XValue - point2.XValue);
            // 根据斜率，计算y坐标    
            y_value = k*(x_value - point1.XValue) + point1.YValue;

            return result;
        }

        public static string GetSaved3Or6PointForDurationStr(double duration)
        {
            double result = GetSave3Point(duration);
            if (result != 0)
                return GetStingByDouble3Point(duration);
            //保留6位小数
            return GetStingByDouble6Point(duration);
        }
    }

    public enum EventTypeEnum
    {
        UndefinedType,

        /// <summary>
        /// 瞬变
        /// </summary>
        Transient,

        /// <summary>
        /// 也叫Dip，暂降
        /// </summary>
        Sag,

        /// <summary>
        /// 暂升
        /// </summary>
        Swell,

        /// <summary>
        /// 中断
        /// </summary>
        Interruption,
    }

    public class EventTypeManager
    {
        /// <summary>
        /// 暂升Code列表
        /// </summary>
        private static int[] SwellCodes = new int[] {4, 8, 12, 16, 18, 22};

        /// <summary>
        /// 暂降Code列表Code1
        /// </summary>
        private static int[] SagCodes = new int[] {3, 7, 11, 15, 19, 21};

        /// <summary>
        /// 中断Code列表
        /// </summary>
        private static int[] InteruptCodes = new int[] {6, 10, 14, 20};

        public static void GetEventTypeLikeSwellEnum(int code1, double totalMag, int eventType, out EventTypeEnum eventTypeLikeSwell)
        {
            totalMag = CommonUsedFunctionsForWorstCase.GetChangedTotalMagForPoint(totalMag);
            eventTypeLikeSwell = EventTypeEnum.UndefinedType;
            if (eventType == SysConstDefinition.TRANSIENT_TYPE)
            {
                eventTypeLikeSwell = EventTypeEnum.Transient;
            }
            else if (eventType == SysConstDefinition.SAG_SWELL_TYPE)
            {
                if (SagCodes.Contains(code1))
                    eventTypeLikeSwell = EventTypeEnum.Sag;
                else if (SwellCodes.Contains(code1))
                    eventTypeLikeSwell = EventTypeEnum.Swell;
                else if (InteruptCodes.Contains(code1))
                    eventTypeLikeSwell = EventTypeEnum.Interruption;
                else
                {
                    //无法判断的情况根据幅值判断，如果是0.1到1则是暂降，大于1为暂升，小于0.1为中断
                    if (double.NaN.Equals(totalMag))
                        eventTypeLikeSwell = EventTypeEnum.UndefinedType;
                    else if (totalMag < 10)
                        eventTypeLikeSwell = EventTypeEnum.Interruption;
                    else if (10 <= totalMag && totalMag < 100)
                        eventTypeLikeSwell = EventTypeEnum.Sag;
                    else if (totalMag > 100)
                        eventTypeLikeSwell = EventTypeEnum.Swell;
                }
            }
        }
    }

    public interface IWorstCaseManager
    {
        /// <summary>
        /// 传入一个按照TriggerTime分组的分组内的所有事件，获取该组内的所有最坏事件
        /// </summary>
        /// <param name="oneGroupList">传入一个按照TriggerTime分组的分组内的所有事件</param>
        /// <param name="worstCaseList">所有最坏事件</param>
        void GetWorstCase(List<SagSwellEvent> oneGroupList, out List<SagSwellEvent> worstCaseList);

        void GetRegion(SagSwellEvent item, out string region);
    }

    internal class WorstCaseITICManager : IWorstCaseManager
    {
        /// <summary>
        /// 表示ITIC的点所在的区域
        /// </summary>
        private enum MyRegionITIC
        {
            RegionA,
            RegionB,
            RegionC,
            Others,
            UnknownRegion
        }

        private static List<SARFIChartCurve> GetListSARFIChartCurve()
        {
            if (_sarfiCurve == null)
            {
                List<SARFIChartCurve> sarfiCurveTemp;
                GetSARFIChartCurveListForITIC(out sarfiCurveTemp);
                Interlocked.CompareExchange(ref _sarfiCurve, sarfiCurveTemp, null);
            }
            return _sarfiCurve;
        }

        private static List<SARFIChartCurve> _sarfiCurve = null;

        private static void GetSARFIChartCurveListForITIC(out List<SARFIChartCurve> sarfiCurve)
        {
            List<SARFIChartCurve> resultList = new List<SARFIChartCurve>();

            int defaultCurID = 1; //默认的ID            

            SARFIChartCurve ITICCurve = new SARFIChartCurve((int) ToleranceCurveType.ITICLimit, defaultCurID);
            ITICCurve.CurveName = "ITIC";
            //上限曲线
            ITICCurve.UpLineValues.Add(new SARFIChartValue(-0.398, 500));
            ITICCurve.UpLineValues.Add(new SARFIChartValue(0, 200));
            ITICCurve.UpLineValues.Add(new SARFIChartValue(0.477, 140));
            ITICCurve.UpLineValues.Add(new SARFIChartValue(0.477, 120));
            ITICCurve.UpLineValues.Add(new SARFIChartValue(1.30, 120));
            ITICCurve.UpLineValues.Add(new SARFIChartValue(2.699, 120));
            ITICCurve.UpLineValues.Add(new SARFIChartValue(2.699, 110));
            ITICCurve.UpLineValues.Add(new SARFIChartValue(4, 110));
            ITICCurve.UpLineValues.Add(new SARFIChartValue(5, 110));
            ITICCurve.UpLineValues.Add(new SARFIChartValue(Math.Log10(Int32.MaxValue), 110));

            //下限曲线
            ITICCurve.DownLineValues.Add(new SARFIChartValue(1.30, 0));
            ITICCurve.DownLineValues.Add(new SARFIChartValue(1.30, 70));
            ITICCurve.DownLineValues.Add(new SARFIChartValue(2.699, 70));
            ITICCurve.DownLineValues.Add(new SARFIChartValue(2.699, 80));
            ITICCurve.DownLineValues.Add(new SARFIChartValue(4, 80));
            ITICCurve.DownLineValues.Add(new SARFIChartValue(4, 90));
            ITICCurve.DownLineValues.Add(new SARFIChartValue(5, 90));
            ITICCurve.DownLineValues.Add(new SARFIChartValue(Math.Log10(Int32.MaxValue), 90));
            resultList.Add(ITICCurve);

            sarfiCurve = resultList;
        }

        /// <summary>
        /// 传入一个按照TriggerTime分组的分组内的所有事件，获取该组内的所有最坏事件
        /// </summary>
        /// <param name="oneGroupList">传入一个按照TriggerTime分组的分组内的所有事件</param>
        /// <param name="worstCaseList">所有最坏事件</param>
        public void GetWorstCase(List<SagSwellEvent> oneGroupList, out List<SagSwellEvent> worstCaseList)
        {
            List<SARFIChartCurve> sarfiCurve = GetListSARFIChartCurve();

            GetWorstCaseForOneGroup(sarfiCurve, oneGroupList, out worstCaseList);
        }

        public void GetRegion(SagSwellEvent item, out string region)
        {
            var regionEnum = GetRegionWithMag(GetListSARFIChartCurve(), item.FetureInfo.Duration, item.FetureInfo.TotalMag);
            switch (regionEnum)
            {
                case MyRegionITIC.RegionA:
                    region = "A";
                    break;
                case MyRegionITIC.RegionB:
                    region = "B";
                    break;
                case MyRegionITIC.RegionC:
                    region = "C";
                    break;
                default:
                    region = "U";
                    break;
            }
        }

        /// <summary>
        /// 根据 B > C > A判断最坏事件。
        /// 都是B的，幅值越大越严重，幅值相同的，持续时间越长越严重；B里面只有Swell和transient。
        /// 都是C的，幅值越小越严重，幅值相同的，持续时间越长越严重；C只有Dip。
        /// 都是A的，swell>dip>transient;
        /// 都是swell,幅值越大越严重，幅值相同的，持续时间越长越严重；
        /// 都是都是dip，越小越严重;interupt跟dip一起比较大小，越小越严重。幅值相同的，持续时间越长越严重；
        /// 都是transient，取绝对值后幅值越大越严重;幅值相同的，持续时间越长越严重；负值比较大小的时候，取绝对值后加100.打点的时候，取绝对值后加100.
        /// </summary>
        /// <param name="oneGroupList"></param>
        /// <param name="eventIdList"></param>
        private static void GetWorstCaseForOneGroup(List<SARFIChartCurve> sarfiCurve, List<SagSwellEvent> oneGroupList, out List<SagSwellEvent> worstCaseList)
        {
            //1.找出所有Region 的事件
            List<SagSwellEvent> regionAList;
            List<SagSwellEvent> regionBList;
            List<SagSwellEvent> regionCList;
            GetAllRegionList(sarfiCurve, oneGroupList, out regionAList, out regionBList, out regionCList);
            //2.如果有Region B 的，最坏事件肯定在区域B，就不用做后面的A和C的比较了。没有region B的情况，看region C.没有B，C的情况，看region A.
            GetResultWorstCaseFromBCARegion(out worstCaseList, regionBList, regionCList, regionAList);
        }

        private static void GetResultWorstCaseFromBCARegion(out List<SagSwellEvent> worstCaseList, List<SagSwellEvent> regionBList, List<SagSwellEvent> regionCList,
            List<SagSwellEvent> regionAList)
        {
            worstCaseList = new List<SagSwellEvent>();
            if (regionBList.Count > 0)
            {
                GetRegionBWorstCaseEventId(out worstCaseList, regionBList);
                return;
            }
                //没有region B的情况，看region C
            else if (regionCList.Count > 0)
            {
                GetRegionCWorstCaseEventId(out worstCaseList, regionCList);
                return;
            }
                //没有B，C的情况，看region A
            else if (regionAList.Count > 0)
            {
                GetRegionAWorstCaseEventId(out worstCaseList, regionAList);
                return;
            }
        }

        /// <summary>
        ///   /// 都是A的，swell>dip>transient;
        /// 都是swell,幅值越大越严重，幅值相同的，持续时间越长越严重；
        /// 都是dip，越小越严重;interupt跟dip一起比较大小，越小越严重。幅值相同的，持续时间越长越严重；
        /// 都是transient，取绝对值加1后幅值越大越严重;幅值相同的，持续时间越长越严重；
        /// </summary>
        /// <param name="worstCaseList"></param>
        /// <param name="regionAList"></param>
        private static void GetRegionAWorstCaseEventId(out List<SagSwellEvent> worstCaseList, List<SagSwellEvent> regionAList)
        {
            //key是幅值，value中的key是持续时间，value是该幅值、持续时间的事件列表
            SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> swellList;
            SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> dipList;
            SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> transientList;
            GetSwellDipTransientList(regionAList, out swellList, out dipList, out transientList);
            GetWorstCaseForARegion(out worstCaseList, swellList, dipList, transientList);
        }

        private static void GetWorstCaseForARegion(out List<SagSwellEvent> worstCaseList,
            SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> swellList,
            SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> dipList,
            SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> transientList)
        {
            worstCaseList = new List<SagSwellEvent>();
            //都是swell,幅值越大越严重，幅值相同的，持续时间越长越严重；
            if (swellList.Count > 0)
            {
                GetWorstCaseForMaxMag(out worstCaseList, swellList);
            }
                //都是dip，越小越严重;interupt跟dip一起比较大小，越小越严重。幅值相同的，持续时间越长越严重；
            else if (dipList.Count > 0)
            {
                GetWorstCaseForMinMag(out worstCaseList, dipList);
            }
                // 都是transient，取绝对值再加1后幅值越大越严重;幅值相同的，持续时间越长越严重；
            else if (transientList.Count > 0)
            {
                GetWorstCaseForMaxMag(out worstCaseList, transientList);
            }
        }


        private static void GetSwellDipTransientList(List<SagSwellEvent> regionAList,
            out SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> swellList,
            out SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> dipList,
            out SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> transientList)
        {
            CommonUsedFunctionsForWorstCase.GetSwellDipTransientList(regionAList,
                out swellList,
                out dipList,
                out transientList);
        }

        private static void GetRegionCWorstCaseEventId(out List<SagSwellEvent> worstCaseList, List<SagSwellEvent> regionCList)
        {
            //都是B的，幅值越大越严重，幅值相同的，持续时间越长越严重；
            //key是幅值，value中的key是持续时间，value是该幅值、持续时间的事件列表
            SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> regionCDic;
            GetSortedRegionCDic(regionCList, out regionCDic);
            //取regionCDic的第一个元素，其幅值最小。
            GetWorstCaseForMinMag(out worstCaseList, regionCDic);
        }

        private static void GetWorstCaseForMinMag(out List<SagSwellEvent> worstCaseList, SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> regionCDic)
        {
            CommonUsedFunctionsForWorstCase.GetWorstCaseForMinMag(out worstCaseList, regionCDic);
        }

        private static void GetRegionBWorstCaseEventId(out List<SagSwellEvent> worstCaseList, List<SagSwellEvent> regionBList)
        {
            //都是B的，幅值越大越严重，幅值相同的，持续时间越长越严重；
            //key是幅值，value中的key是持续时间，value是该幅值、持续时间的事件列表
            SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> regionBDic;
            GetSortedRegionBDic(regionBList, out regionBDic);
            //取regionBDic的最后一个元素，其幅值最大。
            GetWorstCaseForMaxMag(out worstCaseList, regionBDic);
        }

        /// <summary>
        /// 幅值越大越严重，幅值相同的，持续时间越长越严重；
        /// </summary>
        /// <param name="worstCaseList"></param>
        /// <param name="regionBDic"></param>
        private static void GetWorstCaseForMaxMag(out List<SagSwellEvent> worstCaseList, SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> regionBDic)
        {
            CommonUsedFunctionsForWorstCase.GetWorstCaseForMaxMag(out worstCaseList, regionBDic);
        }


        private static void GetSortedRegionBDic(List<SagSwellEvent> regionBList, out SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> regionBDic)
        {
            CommonUsedFunctionsForWorstCase.GetSortedRegionDic(regionBList, out regionBDic);
        }

        private static void GetSortedRegionCDic(List<SagSwellEvent> regionCList, out SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> regionCDic)
        {
            CommonUsedFunctionsForWorstCase.GetSortedRegionDic(regionCList, out regionCDic);
        }




        private static void GetAllRegionList(List<SARFIChartCurve> sarfiCurve, List<SagSwellEvent> oneGroupList, out List<SagSwellEvent> regionAList, out List<SagSwellEvent> regionBList, out List<SagSwellEvent> regionCList)
        {
            regionAList = new List<SagSwellEvent>();
            regionBList = new List<SagSwellEvent>();
            regionCList = new List<SagSwellEvent>();
            foreach (var item in oneGroupList)
            {
                var region = GetRegionWithMag(sarfiCurve, item.FetureInfo.Duration, item.FetureInfo.TotalMag);
                if (region == MyRegionITIC.RegionB)
                {
                    regionBList.Add(item);
                    continue;
                }
                if (region == MyRegionITIC.RegionC)
                {
                    regionCList.Add(item);
                    continue;
                }
                if (region == MyRegionITIC.RegionA)
                {
                    regionAList.Add(item);
                    continue;
                }
            }
        }


        /// <summary>
        /// 获取所在的区域
        /// </summary>
        /// <param name="outOfLimit"></param>
        /// <param name="duration"></param>
        /// <param name="totalMag"></param>
        /// <param name="totalMagForPoint"></param>
        private static MyRegionITIC GetRegionWithMag(List<SARFIChartCurve> sarfiCurve, double duration, double totalMag)
        {
            int outOfLimit = GetRMSLimitTypes(sarfiCurve, duration, totalMag);
            return GetRegionTypeByID(outOfLimit);
        }

        /// <summary>
        /// 根据特征值中的OutOfLimit字段以及选中的容忍度标准判断事件标记点的区域
        /// </summary>
        /// <param name="outOfLimit"></param>
        /// <param name="curveType"></param>
        /// <returns></returns>
        private static MyRegionITIC GetRegionTypeByID(int outOfLimit)
        {
            if (outOfLimit == (int) SARFILimitType.OutOfRange)
                return MyRegionITIC.Others;
            if (outOfLimit == (int) SARFILimitType.Normal)
                return MyRegionITIC.RegionA;
            if (outOfLimit == (int) SARFILimitType.OverLimit)
                return MyRegionITIC.RegionB;
            if (outOfLimit == (int) SARFILimitType.BelowLimit)
                return MyRegionITIC.RegionC;
            return MyRegionITIC.UnknownRegion;
        }

        /// <summary>
        /// 判断特征值越限类型
        /// </summary>
        /// <param name="sarfiCurve">标准曲线</param>
        /// <param name="duration">持续时间（对数值）</param>
        /// <param name="totalMag">幅值</param>
        /// <returns>返回越限类型，-1表示不在区域范围内，0-不越限，1-越上限，2-越下限</returns>
        private static int GetRMSLimitTypes(List<SARFIChartCurve> sarfiCurve, double duration, double totalMag)
        {
            if (double.IsNaN(duration) || double.IsNaN(totalMag))
                return (int) SARFILimitType.OutOfRange;

            int result = (int) SARFILimitType.Normal;
            totalMag = GetChangedTotalMag(totalMag);

            if (sarfiCurve.Count != 1)
                return result;

            if (sarfiCurve[0].UpLineValues.Count == 0)
                return result;

            int curveType = sarfiCurve[0].CurveType;

            if (curveType == (int) ToleranceCurveType.ITICLimit)
            {
                //判断是否越上限
                result = OverITICUpCurve(sarfiCurve, duration, totalMag, result, curveType);
                if (result != (int) SARFILimitType.OverLimit)
                {
                    //如果有下限曲线，并且数值点非越上限，则进一步进行越下限判断
                    result = UnderITICDownCurve(sarfiCurve, duration, totalMag, result, curveType);
                }
            }
            return result;
        }

        private static double GetChangedTotalMag(double totalMag)
        {
            return CommonUsedFunctionsForWorstCase.GetChangedTotalMagForPoint(totalMag);
        }

        /// <summary>
        /// 判断是否越ITIC下限
        /// </summary>
        /// <param name="sarfiCurve"></param>
        /// <param name="duration"></param>
        /// <param name="totalMag"></param>
        /// <param name="result"></param>
        /// <param name="curveType"></param>
        /// <returns></returns>
        private static int UnderITICDownCurve(List<SARFIChartCurve> sarfiCurve, double duration, double totalMag, int result, int curveType)
        {
            if (sarfiCurve[0].DownLineValues != null && result == (int) SARFILimitType.Normal)
            {
                for (int i = 0; i < sarfiCurve[0].DownLineValues.Count - 1; i++)
                {
                    SARFIChartValue point1 = sarfiCurve[0].DownLineValues[i];
                    SARFIChartValue point2 = sarfiCurve[0].DownLineValues[i + 1];
                    double x_val = duration;
                    if (curveType == (int) ToleranceCurveType.ITICLimit)
                        x_val = Math.Log10(duration);
                    double y_val;
                    bool exist = GetYValueByXValue(point1, point2, x_val, out y_val);
                    if (exist)
                    {
                        if (y_val == double.MaxValue)
                        {
                            //垂直直线
                            if (totalMag < point1.YValue && totalMag < point2.YValue)
                                result = (int) SARFILimitType.BelowLimit;
                        }
                        else if (totalMag < y_val)
                            result = (int) SARFILimitType.BelowLimit;
                        break;
                    }
                }
            }
            return result;
        }

        private static bool GetYValueByXValue(SARFIChartValue point1, SARFIChartValue point2, double xVal, out double yVal)
        {
            return CommonUsedFunctionsForWorstCase.GetYValueByXValue(point1, point2, xVal, out yVal);
        }

        /// <summary>
        /// 判断是否越ITIC上限
        /// </summary>
        /// <param name="sarfiCurve"></param>
        /// <param name="duration"></param>
        /// <param name="totalMag"></param>
        /// <param name="result"></param>
        /// <param name="curveType"></param>
        /// <returns></returns>
        private static int OverITICUpCurve(List<SARFIChartCurve> sarfiCurve, double duration, double totalMag, int result, int curveType)
        {
            for (int i = 0; i < sarfiCurve[0].UpLineValues.Count - 1; i++)
            {
                SARFIChartValue point1 = sarfiCurve[0].UpLineValues[i];
                SARFIChartValue point2 = sarfiCurve[0].UpLineValues[i + 1];
                double x_val = duration;
                if (curveType == (int) ToleranceCurveType.ITICLimit)
                    x_val = Math.Log10(duration);
                double y_val;
                bool exist = GetYValueByXValue(point1, point2, x_val, out y_val);
                if (exist)
                {
                    if (y_val == double.MaxValue)
                    {
                        if (totalMag > point1.YValue && totalMag > point2.YValue)
                            result = (int) SARFILimitType.OverLimit;
                    }
                    else if (totalMag > y_val)
                        result = (int) SARFILimitType.OverLimit;
                    break;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 容忍度曲线类型
    /// </summary>
    public enum ToleranceCurveType
    {
        /// <summary>
        /// ITIC曲线
        /// </summary>
        ITICLimit = 1,

        /// <summary>
        /// SEMI特征曲线
        /// </summary>
        SEMILimit = 2,
    }

    internal class WorstCaseSEMI100sManager : IWorstCaseManager
    {
        /// <summary>
        /// 表示ITIC的点所在的区域
        /// </summary>
        private enum MyRegionSEMI100s
        {
            RegionA,
            RegionB,
            Others,
            UnknownRegion,

        }

        private static List<SARFIChartCurve> GetListSARFIChartCurve()
        {
            if (_sarfiCurve == null)
            {
                List<SARFIChartCurve> sarfiCurveTemp;
                GetSARFIChartCurveListForSEMI100(out sarfiCurveTemp);
                Interlocked.CompareExchange(ref _sarfiCurve, sarfiCurveTemp, null);
            }
            return _sarfiCurve;
        }

        private static List<SARFIChartCurve> _sarfiCurve = null;

        /// <summary>
        /// SEMI-100s标准的curveID
        /// </summary>
        private const int SEMI100S_CURVE_ID = Int32.MaxValue;

        private static void GetSARFIChartCurveListForSEMI100(out List<SARFIChartCurve> sarfiCurve)
        {
            List<SARFIChartCurve> resultList = new List<SARFIChartCurve>();

            SARFIChartCurve SEMI100sCurve = new SARFIChartCurve((int) ToleranceCurveType.SEMILimit, SEMI100S_CURVE_ID);
            SEMI100sCurve.CurveName = "SEMI-100s";
            SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(10, 0));
            SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(20, 0));
            SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(20, 50));
            SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(200, 50));
            SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(200, 70));
            SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(500, 70));
            SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(500, 80));
            SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(10000, 80));
            SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(10000, 90));
            SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(100000, 90));
            SEMI100sCurve.UpLineValues.Add(new SARFIChartValue(Int32.MaxValue, 90));
            resultList.Add(SEMI100sCurve);

            sarfiCurve = resultList;
        }

        /// <summary>
        /// 传入一个按照TriggerTime分组的分组内的所有事件，获取该组内的所有最坏事件
        /// </summary>
        /// <param name="oneGroupList">传入一个按照TriggerTime分组的分组内的所有事件</param>
        /// <param name="worstCaseList">所有最坏事件</param>
        public void GetWorstCase(List<SagSwellEvent> oneGroupList, out List<SagSwellEvent> worstCaseList)
        {
            List<SARFIChartCurve> sarfiCurve = GetListSARFIChartCurve();

            GetWorstCaseForOneGroup(sarfiCurve, oneGroupList, out worstCaseList);
        }

        public void GetRegion(SagSwellEvent item, out string region)
        {
            var regionEnum = GetRegionWithMag(GetListSARFIChartCurve(), item.FetureInfo.Duration, item.FetureInfo.TotalMag);
            switch (regionEnum)
            {
                case MyRegionSEMI100s.RegionA:
                    region = "A";
                    break;
                case MyRegionSEMI100s.RegionB:
                    region = "B";
                    break;

                default:
                    region = "U";
                    break;
            }
        }

        /// <summary>
        /// For SEMI, only consider B > A and don't consider transients and swells.
        /// So if a Dip event that lasts 1 second or more, which cannot be plotted in the SEMI graph.
        ///  The event will still be considered for worst case determination.
        /// 
        /// 1. B >A
        /// 2.dip,interrupt一起算dip，越小越严重
        /// </summary>
        /// <param name="sarfiCurve"></param>
        /// <param name="oneGroupList"></param>
        /// <param name="worstCaseList"></param>
        private void GetWorstCaseForOneGroup(List<SARFIChartCurve> sarfiCurve, List<SagSwellEvent> oneGroupList, out List<SagSwellEvent> worstCaseList)
        {
            //1.找出所有Region 的事件
            List<SagSwellEvent> regionAList;
            List<SagSwellEvent> regionBList;
            GetAllRegionList(sarfiCurve, oneGroupList, out regionAList, out regionBList);
            //2.如果有Region B 的，最坏事件肯定在区域B，就不用做后面的A的比较了。
            GetResultWorstCaseFromBARegion(out worstCaseList, regionBList, regionAList);
        }

        private static void GetResultWorstCaseFromBARegion(out List<SagSwellEvent> worstCaseList, List<SagSwellEvent> regionBList, List<SagSwellEvent> regionAList)
        {
            worstCaseList = new List<SagSwellEvent>();
            if (regionBList.Count > 0)
            {
                GetRegionBWorstCaseEventId(out worstCaseList, regionBList);
                return;
            }

            //没有B的情况，看region A
            if (regionAList.Count > 0)
            {
                GetRegionAWorstCaseEventId(out worstCaseList, regionAList);
                return;
            }
        }

        private static void GetRegionAWorstCaseEventId(out List<SagSwellEvent> worstCaseList, List<SagSwellEvent> regionAList)
        {
            //Region A 同样只考虑Dip（包含interrupt）。越小越严重。
            GetDipMin(out worstCaseList, regionAList);
        }

        private static void GetSwellDipTransientList(List<SagSwellEvent> regionAList,
            out SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> swellList,
            out SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> dipList,
            out SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> transientList)
        {
            CommonUsedFunctionsForWorstCase.GetSwellDipTransientList(regionAList,
                out swellList,
                out dipList,
                out transientList);
        }

        private static void GetRegionBWorstCaseEventId(out List<SagSwellEvent> worstCaseList, List<SagSwellEvent> regionBList)
        {
            //都是B的，只考虑dip（包含interrupt）。越小越严重。
            GetDipMin(out worstCaseList, regionBList);
        }

        /// <summary>
        /// 只考虑dip（包含interrupt）。越小越严重。
        /// </summary>
        /// <param name="worstCaseList"></param>
        /// <param name="regionList"></param>
        private static void GetDipMin(out List<SagSwellEvent> worstCaseList, List<SagSwellEvent> regionList)
        {
            //key是幅值，value中的key是持续时间，value是该幅值、持续时间的事件列表
            SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> swellList;
            SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> dipList;
            SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> transientList;
            GetSwellDipTransientList(regionList, out swellList, out dipList, out transientList);

            //取regionDic的第一个元素，其幅值最小。
            GetWorstCaseForMinMag(out worstCaseList, dipList);
        }

        private static void GetWorstCaseForMinMag(out List<SagSwellEvent> worstCaseList, SortedDictionary<double, SortedDictionary<double, List<SagSwellEvent>>> regionCDic)
        {
            CommonUsedFunctionsForWorstCase.GetWorstCaseForMinMag(out worstCaseList, regionCDic);
        }


        private void GetAllRegionList(List<SARFIChartCurve> sarfiCurve, List<SagSwellEvent> oneGroupList, out List<SagSwellEvent> regionAList, out List<SagSwellEvent> regionBList)
        {
            regionAList = new List<SagSwellEvent>();
            regionBList = new List<SagSwellEvent>();

            foreach (var item in oneGroupList)
            {
                var region = GetRegionWithMag(sarfiCurve, item.FetureInfo.Duration, item.FetureInfo.TotalMag);
                if (region == MyRegionSEMI100s.RegionB)
                {
                    regionBList.Add(item);
                    continue;
                }

                if (region == MyRegionSEMI100s.RegionA)
                {
                    regionAList.Add(item);
                    continue;
                }
            }
        }


        private MyRegionSEMI100s GetRegionWithMag(List<SARFIChartCurve> sarfiCurve, double duration, double totalMag)
        {
            int outOfLimit = GetRMSLimitTypes(sarfiCurve, duration, totalMag);
            return GetRegionTypeByID(outOfLimit);
        }

        private MyRegionSEMI100s GetRegionTypeByID(int outOfLimit)
        {
            if (outOfLimit == (int) SARFILimitType.BelowLimit)
                return MyRegionSEMI100s.RegionB;
            if (outOfLimit == (int) SARFILimitType.Normal)
                return MyRegionSEMI100s.RegionA;
            if (outOfLimit == (int) SARFILimitType.OutOfRange)
                return MyRegionSEMI100s.Others;
            return MyRegionSEMI100s.UnknownRegion;
        }


        /// <summary>
        /// 判断特征值越限类型
        /// </summary>
        /// <param name="sarfiCurve">标准曲线</param>
        /// <param name="duration">持续时间（对数值）</param>
        /// <param name="totalMag">幅值</param>
        /// <returns>返回越限类型，-1表示不在区域范围内，0-不越限，1-越上限，2-越下限</returns>
        public int GetRMSLimitTypesByPointRange(List<SARFIChartCurve> sarfiCurve, double duration, double totalMag, List<double> pointRange)
        {
            return GetRMSLimitTypes(sarfiCurve, duration, totalMag);
        }

        /// <summary>
        /// 判断特征值越限类型
        /// </summary>
        /// <param name="sarfiCurve">标准曲线</param>
        /// <param name="duration">持续时间（对数值）</param>
        /// <param name="totalMag">幅值</param>
        /// <returns>返回越限类型，0-不越限，1-越上限，2-越下限</returns>
        private int GetRMSLimitTypes(List<SARFIChartCurve> sarfiCurve, double duration, double totalMag)
        {
            if (double.IsNaN(duration) || double.IsNaN(totalMag))
                return (int) SARFILimitType.OutOfRange;

            int result = (int) SARFILimitType.Normal;
            totalMag = GetChangedTotalMag(totalMag);

            if (sarfiCurve.Count != 1)
                return result;
            if (sarfiCurve[0].UpLineValues.Count == 0)
                return result;
            int curveType = sarfiCurve[0].CurveType;
            if (curveType == (int) ToleranceCurveType.SEMILimit)
            {
                //判断是否越下限
                result = UnderSEMICurve(sarfiCurve, duration, totalMag, result);
            }
            return result;
        }

        private double GetChangedTotalMag(double totalMag)
        {
            return CommonUsedFunctionsForWorstCase.GetChangedTotalMagForPoint(totalMag);
        }

        /// <summary>
        /// 判断是否越SEMI下限
        /// </summary>
        /// <param name="sarfiCurve"></param>
        /// <param name="duration"></param>
        /// <param name="totalMag"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private int UnderSEMICurve(List<SARFIChartCurve> sarfiCurve, double duration, double totalMag, int result)
        {
            for (int i = 0; i < sarfiCurve[0].UpLineValues.Count - 1; i++)
            {
                SARFIChartValue point1 = sarfiCurve[0].UpLineValues[i];
                SARFIChartValue point2 = sarfiCurve[0].UpLineValues[i + 1];
                double x_val = duration;
                double y_val;
                bool exist = CommonUsedFunctionsForWorstCase.GetYValueByXValue(point1, point2, x_val, out y_val);
                if (exist)
                {
                    if (y_val == double.MaxValue)
                    {
                        if (totalMag < point1.YValue && totalMag < point2.YValue)
                            result = (int) SARFILimitType.BelowLimit;
                    }
                    else if (totalMag < y_val)
                        result = (int) SARFILimitType.BelowLimit;
                    break;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 越限类型
    /// </summary>
    public enum SARFILimitType
    {
        /// <summary>
        /// 错误
        /// </summary>
        OutOfRange = -1,

        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 越上限
        /// </summary>
        OverLimit = 1,

        /// <summary>
        /// 越下限
        /// </summary>
        BelowLimit = 2,
    }

    #endregion

    #region PC_TB_05表相关

    internal class PC_TB_05_ColumnDataManager
    {
        private static PC_TB_05_ColumnDataManager _Instance;

        public static PC_TB_05_ColumnDataManager GetInstance()
        {
            return _Instance ?? (_Instance = new PC_TB_05_ColumnDataManager());
        }

        private PC_TB_05_ColumnDataManager()
        {

        }

        #region  公有方法

        public uint GetNodeType(DataRow row)
        {
            uint nodeType = 0;
            uint.TryParse(row["NodeType"].ToString(), out nodeType);
            return nodeType;
        }

        public uint GetNodeId(DataRow row)
        {
            uint result = 0;
            uint.TryParse(row["NodeID"].ToString(), out result);
            return result;

        }

        public uint GetParentNodeType(DataRow row)
        {
            uint result = 0;
            uint.TryParse(row["ParentNodeType"].ToString(), out result);
            return result;

        }

        public uint GetParentNodeId(DataRow row)
        {
            uint result = 0;
            uint.TryParse(row["ParentNodeId"].ToString(), out result);
            return result;

        }

        public string GetNodeName(DataRow row)
        {
            return row["NodeName"].ToString();
        }


        #endregion
    }

    internal class PC_TB_05Manager
    {

        #region  私有成员

        private string m_errorMessage = string.Empty;

        private PC_TB_05_ColumnDataManager m_PC_TB_05_ColumnDataManager = PC_TB_05_ColumnDataManager.GetInstance();

        #endregion

        public string GetErrorMessage()
        {
            return m_errorMessage;
        }



        /// <summary>
        /// 从数据库获取所有的设备节点
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        public bool LoadDeviceList(out List<uint> deviceList)
        {
            deviceList = new List<uint>();
            DataTable resultDT = new DataTable();
            bool suc = true;
            try
            {
                uint[] nodeTypes = {SysConstDefinition.PECSDEVICE_NODE};
                int errorCode = SetupTableProvider.Instance.ReadSetupNodesByParentNodeTypeID(0,SysConstDefinition.PECSSYSCONFIG_NODE, SysConstDefinition.PECSSYSCONFIG_NODE, nodeTypes, 0, false, ref resultDT);
                if (errorCode != 0)
                {
                    m_errorMessage = ErrorQuerier.Instance.GetLastErrorString();
                    DbgTrace.dout("{0}", m_errorMessage);
                    suc = false;
                }
                else
                {
                    foreach (DataRow row in resultDT.Rows)
                        AddOneNode(row, ref deviceList);
                }
            }
            catch (System.Exception ex)
            {
                m_errorMessage = ex.Message + ex.StackTrace;
                DbgTrace.dout(ex.Message + ex.StackTrace);
                suc = false;
            }
            finally
            {
                if (resultDT != null)
                    resultDT.Dispose();
            }
            return suc;
        }

        private void AddOneNode(DataRow row, ref List<uint> deviceList)
        {
            uint nodeType = m_PC_TB_05_ColumnDataManager.GetNodeType(row);
            if (nodeType != SysConstDefinition.PECSDEVICE_NODE)
                return;
            uint nodeId = m_PC_TB_05_ColumnDataManager.GetNodeId(row);
            if (!deviceList.Contains(nodeId))
                deviceList.Add(nodeId);
        }
    }

    #endregion

    #region PC_TB_06表相关

    /// <summary>
    /// 用于读取数据库的PD_TB_06表的数据
    /// </summary>
    internal class PD_TB_06_Manager
    {
        #region 成员变量

        public string LastErrorString { get; set; }

        #endregion

        #region 使用数据库接口的时候常用

        private static void Dispose(DataTable eventDT)
        {
            if (eventDT != null)
                eventDT.Dispose();
        }

        private void WriteExceptionLog(Exception ex)
        {
            LastErrorString = ex.Message;
            DbgTrace.dout(ex.Message + ex.StackTrace);
        }

        private void WriteLog(int errorCode)
        {
            LastErrorString = DBInterfaceCommonLib.ErrorQuerier.Instance.GetLastErrorString();
            DbgTrace.dout("EventlogProvider.Instance.ReadEventLogsByMultiStaChnDev invoked failed.{0}.ErrorCode={1}.", LastErrorString, errorCode);
        }

        #endregion

        #region 公有函数

        public bool GetEndEventAndNextStart(DateTime endTime, EventInformation startEventInformation, List<NODE_TYPE_ID> nodeTypeIDList, int[] eventType, out EventInformation eventInformationEnd, out EventInformation nextStarteventInformation)
        {
            eventInformationEnd = null;
            nextStarteventInformation = null;
            try
            {
                var returnReslut  = ReadEventLogsByMultiStaChnDev(nodeTypeIDList, startEventInformation.FullTime, endTime, eventType,new int[] { });
                if (!returnReslut.Success)
                {
                    DbgTrace.dout("GetEndEventAndNextStart:{0}", returnReslut.ErrorMessage);
                    return false;
                }
                //获取起始事件对应的结束事件
                GetEndEventOnly(returnReslut.ResultList, startEventInformation, out eventInformationEnd);
                //获取下一个起始事件
                GetNextStartEventOnly(returnReslut.ResultList, startEventInformation, out nextStarteventInformation);
                return true;
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex);
                return false;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 从DataRow读取PD_TB_06表的一行数据到eventInformation对象
        /// </summary>
        /// <param name="oneRow"></param>
        /// <param name="eventInformation"></param>
        /// <returns></returns>
        public bool GetEventRow(EventLogByDevResponse oneRow, out EventInformation eventInformation)
        {
            return EventInformation.GetEventRow(oneRow, out eventInformation);
        }

        /// <summary>
        /// 从数据库查询PD_TB_06表的数据，存储到eventListResult
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="nodeTypeIDList"></param>
        /// <param name="eventType"></param>
        /// <param name="eventListResult">key 是EventTime列的值+Msec的值</param>
        /// <returns></returns>
        public bool GetEvents(DateTime startTime, DateTime endTime, List<NODE_TYPE_ID> nodeTypeIDList, int[] eventType, out SortedDictionary<DateTime, List<EventInformation>> eventListResult)
        {
            eventListResult = new SortedDictionary<DateTime, List<EventInformation>>();
            try
            {
                var returnReslut = ReadEventLogsByMultiStaChnDev(nodeTypeIDList, startTime, endTime, eventType, new int[] { });
                if (!returnReslut.Success)
                {
                    DbgTrace.dout("GetEvents:{0}", returnReslut.ErrorMessage);
                    return false;
                }
                int countRow = returnReslut.ResultList.Count;
                for (int i = 0; i < returnReslut.ResultList.Count; ++i)
                {
                    EventInformation eventInformation;
                    if (!GetEventRow(returnReslut.ResultList[i], out eventInformation))
                        continue;
                    if (eventInformation.FullTime < startTime)
                        continue;
                    if (eventInformation.FullTime >= endTime)
                        continue;

                    List<EventInformation> tempList;
                    DateTime tempTimeKey = eventInformation.FullTime;
                    if (!eventListResult.TryGetValue(tempTimeKey, out tempList))
                    {
                        tempList = new List<EventInformation>();
                        eventListResult.Add(tempTimeKey, tempList);
                    }
                    tempList.Add(eventInformation);
                }
                return true;
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex);
                return false;
            }
            finally
            {
                //Dispose(eventDT);
            }
        }

        #endregion

        #region 私有函数


        /// <summary>
        /// 查找下一个起始事件
        /// </summary>
        /// <param name="eventDT"></param>
        /// <param name="startEventInformation"></param>
        /// <param name="nextStarteventInformation"></param>
        /// <returns></returns>
        private bool GetNextStartEventOnly(List<EventLogByDevResponse> eventDT, EventInformation startEventInformation, out EventInformation nextStarteventInformation)
        {
            nextStarteventInformation = null;
            DateTime startEventTimeKey = startEventInformation.FullTime;
            for (int i = 0; i < eventDT.Count; ++i)
            {
                EventInformation eventInformation;
                if (!GetEventRow(eventDT[i], out eventInformation))
                    continue;

                DateTime tempTimeKey = eventInformation.FullTime;
                //如果小于起始事件的事件，就无效
                if (tempTimeKey < startEventTimeKey)
                    continue;

                //下一个Code2=1的事件就是下一个起始事件
                if (NotTheRightOne(startEventInformation, eventInformation))
                    continue;

                //到这里则找到了下一个起始事件
                if (eventInformation.Code2 == EventInformation.StartEventCode2 && eventInformation.FullTime > startEventTimeKey)
                {
                    nextStarteventInformation = eventInformation;
                    return true;
                }
            }
            return false;
        }

        private static bool NotTheRightOne(EventInformation startEventInformation, EventInformation eventInformation)
        {
            if (eventInformation.StationId != startEventInformation.StationId)
                return true;
            if (eventInformation.ChannelId != startEventInformation.ChannelId)
                return true;
            if (eventInformation.DeviceId != startEventInformation.DeviceId)
                return true;
            if (eventInformation.StationFlag != startEventInformation.StationFlag)
                return true;
            if (eventInformation.EventType != startEventInformation.EventType)
                return true;
            if (eventInformation.EventByte != startEventInformation.EventByte)
                return true;
            if (eventInformation.Code1 != startEventInformation.Code1)
                return true;
            return false;
        }

        //private bool GetLastStartEvent(DataTable eventDT,DateTime endTime, out EventInformation lastStartEventInformation)
        //{
        //     int countRow = eventDT.Rows.Count;

        //    for (int i = countRow-1; i >= 0; --i)
        //    {
        //        DataRow oneRow = eventDT.Rows[i];
        //        EventInformation eventInformation;
        //        if (!GetEventRow(oneRow, out eventInformation))
        //            continue;
        //        if(eventInformation.FullTime>=endTime)
        //            continue;
        //        //查找最后一个起始事件
        //        return true;
        //    }
        //}

        /// <summary>
        /// 查找起始事件对应的结束事件
        /// </summary>
        /// <param name="eventDT"></param>
        /// <param name="startEventInformation"></param>
        /// <param name="eventInformationEnd"></param>
        /// <returns></returns>
        private bool GetEndEventOnly(List<EventLogByDevResponse> eventDT, EventInformation startEventInformation, out EventInformation eventInformationEnd)
        {
            eventInformationEnd = null;
            DateTime startEventTimeKey = startEventInformation.FullTime;

            for (int i = 0; i < eventDT.Count; ++i)
            {
                EventInformation eventInformation;
                if (!GetEventRow(eventDT[i], out eventInformation))
                    continue;

                DateTime tempTimeKey = eventInformation.FullTime;
                //寻找起始事件的时刻
                if (tempTimeKey < startEventTimeKey)
                    continue;

                //	起始事件和结束事件匹配。电压变动事件（EventType=18），结束事件匹配条件为：
                //StationID,ChannelID,DeviceID,StationFlag,EventType,EventByte,code1这几个字段一致，Code2=1为起始事件，Code2=2为结束事件。
                if (NotTheRightOne(startEventInformation, eventInformation))
                    continue;
                //到这里则找到了结束事件
                if (eventInformation.Code2 == EventInformation.EndEventCode2 && eventInformation.FullTime > startEventTimeKey)
                {
                    eventInformationEnd = eventInformation;
                    return true;
                }
            }
            return false;
        }

        //public static int ReadEventLogsByMultiStaChnDev(List<NODE_TYPE_ID> nodeTypeIDList, DateTime startTime, DateTime endTime, int[] eventType, ref DataTable eventDT)
        //{
        //    return EventlogProvider.Instance.ReadEventLogsByMultiStaChnDev(DBOperationFlag.either,
        //        nodeTypeIDList.ToArray(), startTime, endTime, eventType, null, null, null, true, 0,
        //        ref eventDT);
        //}

        public static WebGeneralResult<EventLogByDevResponse> ReadEventLogsByMultiStaChnDev(List<NODE_TYPE_ID> nodeTypeIDList, DateTime startTime, DateTime endTime, int[] eventType, int[] eventClass)
        {
            StringBuilder sb = new StringBuilder();
            EventLogByDevRequest request = new EventLogByDevRequest();

            request.eventID = 0;
            request.exportType = 0;
            request.filterParam = "0,0";
            request.keyWord = string.Empty;
            request.maxRowCount = 0;
            foreach (NODE_TYPE_ID node in nodeTypeIDList)
            {
                sb.Append(string.Format("{0,{1};", node.nodeType, node.nodeID));
            }
            request.nodeParams = sb.ToString();
            request.offset = 0;
            request.queryTypeIndex = 1;
            request.selectedIndex = 0;
            request.setWait = false;
            request.timeParam = string.Format("{0}:{1}", startTime.ToString(), endTime.ToString());
            sb = new StringBuilder();
            string eventTypeStr = string.Empty;
            if(eventType.Length == 0)
            {
                eventTypeStr = "-1";
            }
            else
            {
                eventTypeStr = string.Join(".", eventType);
            }
            string eventClassStr = string.Empty;
            if (eventClass.Length == 0)
            {
                eventClassStr = "-1";
            }
            else
            {
                eventClassStr = string.Join(".", eventClass);
            }
            request.typeClassParam = string.Format("{0};{1}", eventTypeStr, eventClassStr);
            return BasicDataProvider.GetInstance().ReadEventLogsByDev(request);
        }
        #endregion

    }

    /// <summary>
    /// 查询出PD_TB_06表的所有设备的起始事件
    /// </summary>
    public class PdTb06AllDataManager
    {
        #region 使用数据库接口的时候常用

        public string LastErrorString { get; set; }

        private static void Dispose(DataTable eventDT)
        {
            if (eventDT != null)
                eventDT.Dispose();
        }

        private void WriteExceptionLog(Exception ex)
        {
            LastErrorString = ex.Message;
            DbgTrace.dout(ex.Message + ex.StackTrace);
        }

        private void WriteLog(int errorCode)
        {
            LastErrorString = DBInterfaceCommonLib.ErrorQuerier.Instance.GetLastErrorString();
            DbgTrace.dout("EventlogProvider.Instance.ReadEventLogsByMultiStaChnDev invoked failed.{0}.ErrorCode={1}.", LastErrorString, errorCode);
        }

        #endregion

        #region 真正实现在别的类的函数

        //private static int ReadEventLogsByMultiStaChnDev(List<NODE_TYPE_ID> nodeTypeIDList, DateTime startTime, DateTime endTime, int[] eventType, ref DataTable eventDT)
        //{
        //    return PD_TB_06_Manager.ReadEventLogsByMultiStaChnDev(nodeTypeIDList, startTime, endTime, eventType,
        //        ref eventDT);
        //}

        private static List<NODE_TYPE_ID> GetNodeTypeIDList(List<uint> deviceIdList)
        {
            return DirectionManager.GetNodeTypeIDList(deviceIdList);
        }

        /// <summary>
        /// 从DataRow读取PD_TB_06表的一行数据的部分数据到eventInformation对象
        /// </summary>
        /// <param name="oneRow"></param>
        /// <param name="eventInformation"></param>
        /// <returns></returns>
        private bool GetStartEventKey(EventLogByDevResponse oneRow, out StartEventKey eventInformation)
        {
            return StartEventKey.GetStartEventKey(oneRow, out eventInformation);

        }

        /// <summary>
        /// 获取所有的设备的EventType=18的事件中，最后一个起始事件对应的时间
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="lastStartEventTime"></param>
        /// <returns></returns>
        public bool GetLastEventTime(DateTime startTime, DateTime endTime, out DateTime lastStartEventTime)
        {
            return EndTimeManager.GetInstance().GetLastEventTime(startTime, endTime, out lastStartEventTime);
        }

        #endregion


        #region 获取eventType=17，eventType=18，EventType=19的事件。传入所有需要的设备ID

        #region 成员变量


        #endregion


        #region 成员函数

        //public bool GetAllEvents(List<uint> deviceList, DateTime startTime, DateTime endTime, List<int> rmsEventTypes, int maxRowCount, out Dictionary<SagSwellHandle, List<EventInformation>> startEventMap, out Dictionary<SagSwellHandle, List<EventInformation>> endEventMap, out Dictionary<uint, List<EventInformation>> directionEventMap)
        //{
        //    Initialize(out  startEventMap, out  endEventMap, out directionEventMap);
        //    //查询所有的设备的起始事件
        //    List<NODE_TYPE_ID> nodeTypeIDList = GetNodeTypeIDList(deviceList);
        //    //获取需要查询的eventType
        //    List<int> neededEventType;
        //    GetNeededEventType(rmsEventTypes, out neededEventType);
        //    int[] eventType = neededEventType.ToArray();
        //    DataTable eventDT = new DataTable();
        //    try
        //    {
        //        //eventType=18的结束事件要在_lastStartEventTime 的基础上向后顺延1小时，这里直接多查询1小时的，避免多次查询
        //        int errorCode = EventlogProvider.Instance.ReadEventLogsByMultiStaChnDev(DBOperationFlag.either,
        //           nodeTypeIDList.ToArray(), startTime, endTime.AddSeconds(1).AddHours(1), eventType, null, null, null, true, 0,
        //           ref eventDT);
        //        if (errorCode != 0)
        //        {
        //            WriteLog(errorCode);
        //            return false;
        //        }
        //        //选择出最后一个起始事件的行
        //        int countRow = eventDT.Rows.Count;
        //        for (int i = 0; i < countRow; ++i)
        //        {
        //            DataRow oneRow = eventDT.Rows[i];
        //            EventInformation fullInformation;
        //            if (!EventInformation.GetEventRow(oneRow, out fullInformation))
        //                continue;
        //            int eventTypeTemp = fullInformation.EventType;
        //            if (eventTypeTemp == SysConstDefinition.TRANSIENT_TYPE)
        //            {
        //                AddTransient17Dic(fullInformation, ref startEventMap);
        //            }
        //            else if (eventTypeTemp == SysConstDefinition.SAG_SWELL_TYPE)
        //            {
        //                if (!AddVariant18Dic(startTime, endTime, fullInformation, ref  _Variation18StartEventDic, ref  _Variation18EndEventDic)) continue;
        //            }
        //            else if (eventTypeTemp == SysConstDefinition.SAG_DIRECTION_TYPE)
        //            {
        //                AddDirection19Dic(fullInformation, ref _Direction19Dic);
        //            }
        //        }
        //        return true;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        WriteExceptionLog(ex);
        //        return false;
        //    }
        //    finally
        //    {
        //        Dispose(eventDT);
        //    }
        //}


        /// <summary>
        ///    /// <summary>
        /// 从数据库查询出所有传入设备的eventType=17,18,19的事件
        /// </summary>
        /// <param name="deviceList">查询输入的所有设备ID</param>
        /// <param name="startTime">查询输入的起始时间</param>
        /// <param name="endTime">查询输入的结束时间</param>
        /// <param name="rmsEventTypes">输入需要查询的eventType组合。可以是eventType=17，eventType=18，或者17和18一起传入</param>
        /// <returns>查询是否成功</returns>
        /// </summary>
        /// <param name="deviceList"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="rmsEventTypes"></param>
        /// <param name="maxRowCount"></param>
        /// <param name="_Transient17Dic">     /// <summary>
        /// 存储设备的eventType=17的所有事件。key是deviceId，value是该设备的所有eventType=17的事件
        /// </summary>
        ///private Dictionary<int, List<EventInformation>> _Transient17Dic = null;
        /// <summary>
        /// 存储设备的eventType=18的所有起始事件。key是deviceId，value是该设备的所有eventType=18的事件。中间一个SagHandle类型的key：起始事件跟结束事件匹配的标准就是这些值一致。
        /// </summary>
        ///private Dictionary<int,Dictionary<SagHandle, List<EventInformation>>> _Variation18StartEventDic = null;
        /// <summary>
        /// 存储设备的eventType=18的所有结束事件。key是deviceId，value是该设备的所有eventType=18的事件。中间一个SagHandle类型的key：起始事件跟结束事件匹配的标准就是这些值一致。
        /// </summary>
        /// private Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18EndEventDic = null;
        /// <summary>
        /// 存储eventType=19的所有事件。key是deviceId，value是key的所有eventType=19的事件
        /// </summary>
        ///private Dictionary<int, List<EventInformation>> _Direction19Dic = null;</param>
        /// <param name="_Variation18StartEventDic"></param>
        /// <param name="_Direction19Dic"></param>
        /// <returns></returns>
        public bool GetAllEvents(List<uint> deviceList, DateTime startTime, DateTime endTime, List<int> rmsEventTypes,
            int maxRowCount,
            out Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18StartEventDic,
            out Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18EndEventDic,
            out Dictionary<int, List<EventInformation>> _Direction19Dic)
        {
            Initialize(out _Variation18StartEventDic, out _Variation18EndEventDic, out _Direction19Dic);
            //查询所有的设备的起始事件
            List<NODE_TYPE_ID> nodeTypeIDList = GetNodeTypeIDList(deviceList);
            //获取需要查询的eventType
            List<int> neededEventType;
            GetNeededEventType(rmsEventTypes, out neededEventType);
            int[] eventType = neededEventType.ToArray();
            try
            {
                //eventType=18的结束事件要在_lastStartEventTime 的基础上向后顺延1小时，这里直接多查询1小时的，避免多次查询
                var returnReslut = PD_TB_06_Manager.ReadEventLogsByMultiStaChnDev(nodeTypeIDList, startTime, endTime.AddSeconds(1).AddHours(1), eventType, new int[] { });
                if (!returnReslut.Success)
                {
                    DbgTrace.dout("GetAllEvents:{0}", returnReslut.ErrorMessage);
                    return false;
                }
                //只是按eventTime排序，没有按Msec排序，因此需要再排序
                SortedDictionary<DateTime, List<EventInformation>> eventAllDic;
                //将查询出来的事件按时间排序，放到一个字典中
                AddToEventAllDic(returnReslut.ResultList, out eventAllDic);
                //添加到各自的结果集合
                AddToDic(startTime, endTime, ref _Variation18StartEventDic, ref _Variation18EndEventDic, ref _Direction19Dic, eventAllDic);
                return true;
            }
            catch (System.Exception ex)
            {
                WriteExceptionLog(ex);
                return false;
            }
            finally
            {
                //Dispose(eventDT);
            }
        }

        /// <summary>
        /// 将DataTable中的内容转到字典中。
        /// </summary>
        /// <param name="eventDT">DataTable的内容</param>
        /// <param name="eventAllDic">key是EventTime+Msec，Value是一行记录的内容</param>
        private static void AddToEventAllDic(List<EventLogByDevResponse> eventDT, out SortedDictionary<DateTime, List<EventInformation>> eventAllDic)
        {
            eventAllDic = new SortedDictionary<DateTime, List<EventInformation>>();
            for (int i = 0; i < eventDT.Count; ++i)
            {
                EventInformation fullInformation;
                if (!EventInformation.GetEventRow(eventDT[i], out fullInformation))
                    continue;
                int eventTypeTemp = fullInformation.EventType;
                if (eventTypeTemp == SysConstDefinition.TRANSIENT_TYPE)
                {
                    AddItem(ref eventAllDic, fullInformation);
                }
                else if (eventTypeTemp == SysConstDefinition.SAG_SWELL_TYPE)
                {
                    AddItem(ref eventAllDic, fullInformation);
                }
                else if (eventTypeTemp == SysConstDefinition.SAG_DIRECTION_TYPE)
                {
                    AddItem(ref eventAllDic, fullInformation);
                }
            }
        }

        private void AddToDic(DateTime startTime, DateTime endTime, ref Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18StartEventDic,
            ref Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18EndEventDic, ref Dictionary<int, List<EventInformation>> _Direction19Dic, SortedDictionary<DateTime, List<EventInformation>> event18Dic)
        {
            List<EventInformation> newList = new List<EventInformation>();
            foreach (var itemkeyPair in event18Dic)
            {
                foreach (var fullInformation in itemkeyPair.Value)
                {
                    newList.Add(fullInformation);
                }
            }
            int count = newList.Count;
            for (int i = count - 1; i >= 0; --i)
            {
                var fullInformation = newList[i];
                int eventTypeTemp = fullInformation.EventType;
                if (eventTypeTemp == SysConstDefinition.TRANSIENT_TYPE)
                {
                    AddTo18Dic(fullInformation, ref _Variation18StartEventDic);
                }
                else if (eventTypeTemp == SysConstDefinition.SAG_SWELL_TYPE)
                {
                    if (
                        !AddVariant18Dic(startTime, endTime, fullInformation, ref _Variation18StartEventDic,
                            ref _Variation18EndEventDic)) continue;
                }
                else if (eventTypeTemp == SysConstDefinition.SAG_DIRECTION_TYPE)
                {
                    AddDirection19Dic(fullInformation, ref _Direction19Dic);
                }
            }
        }

        private static void AddItem(ref SortedDictionary<DateTime, List<EventInformation>> event17Dic, EventInformation fullInformation)
        {
            List<EventInformation> informationList;
            if (!event17Dic.TryGetValue(fullInformation.FullTime, out informationList))
            {
                informationList = new List<EventInformation>();
                event17Dic.Add(fullInformation.FullTime, informationList);
            }
            informationList.Add(fullInformation);
        }

        private static void GetNeededEventType(List<int> rmsEventTypes, out List<int> neededEventType)
        {
            neededEventType = new List<int>();
            foreach (var item in rmsEventTypes)
            {
                //如果eventType=18，那么需要查询相关的方向事件，方向事件的eventType=19
                if (item == SysConstDefinition.SAG_SWELL_TYPE || item == (int) RMSEventType.Variation)
                {
                    neededEventType.Add(SysConstDefinition.SAG_SWELL_TYPE);
                    neededEventType.Add(SysConstDefinition.SAG_DIRECTION_TYPE);
                }
                //如果eventType=17，那么查询eventType=17的事件即可
                if (item == SysConstDefinition.TRANSIENT_TYPE || item == (int) RMSEventType.Transient)
                {
                    neededEventType.Add(SysConstDefinition.TRANSIENT_TYPE);
                }
            }
        }

        private void AddDirection19Dic(EventInformation fullInformation, ref Dictionary<int, List<EventInformation>> _Direction19Dic)
        {
            List<EventInformation> eventList;
            if (!_Direction19Dic.TryGetValue(fullInformation.DeviceId, out eventList))
            {
                eventList = new List<EventInformation>();
                _Direction19Dic.Add(fullInformation.DeviceId, eventList);
            }
            eventList.Add(fullInformation);
        }

        private bool AddVariant18Dic(DateTime startTime, DateTime endTime, EventInformation fullInformation, ref Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18StartEventDic, ref Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18EndEventDic)
        {
            var eventInformation = new StartEventKey(fullInformation.EventTime, fullInformation.Msec,
                fullInformation.EventType, fullInformation.Code1, fullInformation.Code2);
            if (!eventInformation.IsStartEventOrEndEvent(startTime, endTime))
                return false;

            AddtoStart18Dic(fullInformation, ref _Variation18StartEventDic);
            AddtoEnd18Dic(fullInformation, ref _Variation18EndEventDic);
            return true;
        }

        private void AddtoStart18Dic(EventInformation fullInformation, ref Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18StartEventDic)
        {
            if (fullInformation.Code2 != EventInformation.StartEventCode2)
                return;
            AddTo18Dic(fullInformation, ref _Variation18StartEventDic);
        }

        private void AddtoEnd18Dic(EventInformation fullInformation, ref Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18EndEventDic)
        {
            if (fullInformation.Code2 != EventInformation.EndEventCode2)
                return;
            AddTo18Dic(fullInformation, ref _Variation18EndEventDic);
        }

        private static void AddTo18Dic(EventInformation fullInformation, ref Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18StartEventDic)
        {
            Dictionary<SagHandle, List<EventInformation>> oneDeviceEventDic;
            if (!_Variation18StartEventDic.TryGetValue(fullInformation.DeviceId, out oneDeviceEventDic))
            {
                oneDeviceEventDic = new Dictionary<SagHandle, List<EventInformation>>();
                _Variation18StartEventDic.Add(fullInformation.DeviceId, oneDeviceEventDic);
            }
            SagHandle sagHandle = new SagHandle(fullInformation.StationId, fullInformation.ChannelId, fullInformation.DeviceId,
                fullInformation.EventByte, fullInformation.EventType, fullInformation.StationFlag, fullInformation.Code1);
            List<EventInformation> eventInformationList;
            if (!oneDeviceEventDic.TryGetValue(sagHandle, out eventInformationList))
            {
                eventInformationList = new List<EventInformation>();
                oneDeviceEventDic.Add(sagHandle, eventInformationList);
            }
            eventInformationList.Add(fullInformation);
        }

        //private void AddTransient17Dic(EventInformation fullInformation, ref   Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18StartEventDic)
        //{
        //    Dictionary<SagHandle, List<EventInformation>> eventList;
        //    if (!_Variation18StartEventDic.TryGetValue(fullInformation.DeviceId, out eventList))
        //    {
        //        eventList = new Dictionary<SagHandle, List<EventInformation>>();
        //        _Variation18StartEventDic.Add(fullInformation.DeviceId, eventList);
        //    }
        //    eventList.Add(fullInformation);
        //}

        //private void AddTransient17Dic(EventInformation fullInformation, ref Dictionary<SagSwellHandle, List<EventInformation>> startEventMap)
        //{
        //    List<EventInformation> eventList;
        //    if (! startEventMap.TryGetValue(new SagSwellHandle(fullInformation.StationId,), out eventList))
        //    {
        //        eventList = new List<EventInformation>();
        //        _Transient17Dic.Add(fullInformation.DeviceId, eventList);
        //    }
        //    eventList.Add(fullInformation);
        //}

        #endregion

        #region 私有函数

        private void Initialize(out Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18StartEventDic, out Dictionary<int, Dictionary<SagHandle, List<EventInformation>>> _Variation18EndEventDic, out Dictionary<int, List<EventInformation>> _Direction19Dic)
        {
            _Variation18StartEventDic = new Dictionary<int, Dictionary<SagHandle, List<EventInformation>>>();
            _Variation18EndEventDic = new Dictionary<int, Dictionary<SagHandle, List<EventInformation>>>();
            _Direction19Dic = new Dictionary<int, List<EventInformation>>();
        }

        private void Initialize(out Dictionary<SagSwellHandle, List<EventInformation>> startEventMap,
            out Dictionary<SagSwellHandle, List<EventInformation>> endEventMap,
            out Dictionary<uint, List<EventInformation>> directionEventMap
            )
        {
            startEventMap = new Dictionary<SagSwellHandle, List<EventInformation>>();
            endEventMap = new Dictionary<SagSwellHandle, List<EventInformation>>();
            directionEventMap = new Dictionary<uint, List<EventInformation>>();
        }

        private bool GetEventType(DataRow oneRow, out int eventTypeTemp)
        {
            if (!int.TryParse(oneRow["EventType"].ToString(), out eventTypeTemp))
                return false;
            return true;
        }

        #endregion


        #endregion

        #region  获取所有传入设备的波形

        private WaveManager _WaveManager = new WaveManager();

        //public bool GetAllWave(List<StationChannelDevice> deviceList, DateTime startTime, DateTime endTime)
        //{
        //    return _WaveManager.GetAllWave(deviceList, startTime, endTime);
        //}

        #endregion





    }

    class WaveInformation
    {
        public int Id { get; set; }

        public DateTime LogTime
        {
            get;
            set;
        }

        public int Msec { get; set; }

        public int StationId { get; set; }
        public int ChannelId { get; set; }
        public int DeviceId { get; set; }
        public int LogType { get; set; }
        public int LogHandle { get; set; }
        public int DataFileLength { get; set; }
        public byte[] DataFile { get; set; }

        public WaveInformation(int id, DateTime logTime, int msec, int stationId, int channelId, int deviceId, int logType, int logHandle, int dataFileLength, byte[] dataFile)
        {
            Id = id;
            LogTime = logTime;
            Msec = msec;
            StationId = stationId;
            ChannelId = channelId;
            DeviceId = deviceId;
            LogType = logType;
            LogHandle = logHandle;
            DataFileLength = dataFileLength;
            DataFile = dataFile;
        }

        public static bool ReadOneRow(DataRow oneRow, out WaveInformation resultWave)
        {
            resultWave = null;
            int idTemp;
            if (!int.TryParse(oneRow["ID"].ToString(), out idTemp))
                return false;
            DateTime logTimeTemp;
            if (!DateTime.TryParse(oneRow["LogTime"].ToString(), out logTimeTemp))
                return false;
            int msecTemp;
            if (!int.TryParse(oneRow["Msec"].ToString(), out msecTemp))
                return false;
            int stationIdTemp;
            if (!int.TryParse(oneRow["StationId"].ToString(), out stationIdTemp))
                return false;
            int channelIdTemp;
            if (!int.TryParse(oneRow["ChannelId"].ToString(), out channelIdTemp))
                return false;
            int deviceIdTemp;
            if (!int.TryParse(oneRow["DeviceId"].ToString(), out deviceIdTemp))
                return false;
            int logTypeTemp;
            if (!int.TryParse(oneRow["LogType"].ToString(), out logTypeTemp))
                return false;
            int logHandleTemp;
            if (!int.TryParse(oneRow["LogHandle"].ToString(), out logHandleTemp))
                return false;
            int dataFileLengthTemp;
            if (!int.TryParse(oneRow["DataFileLength"].ToString(), out dataFileLengthTemp))
                return false;

            byte[] dataFileTemp = null;
            if (!Convert.IsDBNull(oneRow["DataFile"]))
            {
                dataFileTemp = (byte[])oneRow["DataFile"];
            }
            resultWave = new WaveInformation(idTemp, logTimeTemp, msecTemp, stationIdTemp, channelIdTemp, deviceIdTemp, logTypeTemp, logHandleTemp, dataFileLengthTemp, dataFileTemp);
            return true;
        }

    }

    class WaveManager
    {
        #region 使用数据库接口的时候常用
        public string LastErrorString { get; set; }
        private static void Dispose(DataTable eventDT)
        {
            if (eventDT != null)
                eventDT.Dispose();
        }

        private void WriteExceptionLog(Exception ex)
        {
            LastErrorString = ex.Message;
            DbgTrace.dout(ex.Message + ex.StackTrace);
        }
        private void WriteLog(int errorCode)
        {
            LastErrorString = DBInterfaceCommonLib.ErrorQuerier.Instance.GetLastErrorString();
            DbgTrace.dout("EventlogProvider.Instance.ReadEventLogsByMultiStaChnDev invoked failed.{0}.ErrorCode={1}.", LastErrorString, errorCode);
        }

        #endregion

        #region 真正实现在别的类的函数

        //private static int ReadEventLogsByMultiStaChnDev(List<NODE_TYPE_ID> nodeTypeIDList, DateTime startTime, DateTime endTime, int[] eventType, ref DataTable eventDT)
        //{
        //    return EventlogProvider.Instance.ReadEventLogsByMultiStaChnDev(DBOperationFlag.either,
        //            nodeTypeIDList.ToArray(), startTime, endTime, eventType, null, null, null, true, 0,
        //            ref eventDT);
        //}

        private static List<NODE_TYPE_ID> GetNodeTypeIDList(List<uint> deviceIdList)
        {
            return DirectionManager.GetNodeTypeIDList(deviceIdList);
        }
        /// <summary>
        /// 从DataRow读取PD_TB_06表的一行数据的部分数据到eventInformation对象
        /// </summary>
        /// <param name="oneRow"></param>
        /// <param name="eventInformation"></param>
        /// <returns></returns>
        private bool GetStartEventKey(EventLogByDevResponse oneRow, out StartEventKey eventInformation)
        {
            return StartEventKey.GetStartEventKey(oneRow, out eventInformation);

        }
        /// <summary>
        /// 获取所有的设备的EventType=18的事件中，最后一个起始事件对应的时间
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="lastStartEventTime"></param>
        /// <returns></returns>
        public bool GetLastEventTime(DateTime startTime, DateTime endTime, out DateTime lastStartEventTime)
        {
            return EndTimeManager.GetInstance().GetLastEventTime(startTime, endTime, out lastStartEventTime);
        }
        #endregion



        #region  获取所有传入设备的波形

        #region 成员变量
        /// <summary>
        /// 保存查询得到的波形。key是deviceId
        /// </summary>
        private Dictionary<uint, List<WaveInformation>> _WaveDic = new Dictionary<uint, List<WaveInformation>>();
        #endregion

        public bool GetAllWave(List<NodeParam> nodes, DateTime startTime, DateTime endTime, out   Dictionary<StaChaDevID, List<DateTime>> idComtradMap)
        {
            idComtradMap = new Dictionary<StaChaDevID, List<DateTime>>();
            foreach (var deviceNode in nodes)
            {
                DataTable tempDT = new DataTable();
                try
                {
                    uint stationId;
                    uint channelId;
                    uint deviceId;
                    if (!GetNodeStationChannelDeviceId(deviceNode, out stationId, out channelId, out deviceId))
                        continue;
                    int logType = 2;
                    int logHandle = 0;

                    // 根据厂站/通道/设备/记录类型/记录标识等读取指定时间段内的原始记录,,可以指定是否读取dataFile字段,,返回结果集按照时间排序,读取波形记录,最多读取_maxNumberOfEvents条
                    int errorCode = WformLogProvider.Instance.ReadWformLog(0, stationId,
                        channelId, deviceId, logType, logHandle, startTime.AddMilliseconds(-startTime.Millisecond), startTime.Millisecond,
                        endTime.AddMilliseconds(-endTime.Millisecond), endTime.Millisecond, false, 0, ref tempDT);
                    if (errorCode != (int)ErrorCode.Success)
                    {
                        WriteLog(errorCode);
                        return false;
                    }

                    foreach (DataRow dr in tempDT.Rows)
                    {
                        StaChaDevID commuIDItem = new StaChaDevID();
                        commuIDItem.StationID = Convert.ToUInt32(dr["StationID"]);
                        commuIDItem.ChannelID = Convert.ToUInt32(dr["ChannelID"]);
                        commuIDItem.DeviceID = Convert.ToUInt32(dr["DeviceID"]);

                        DateTime logtime = Convert.ToDateTime(dr["Logtime"]);
                        logtime = logtime.AddMilliseconds(Convert.ToDouble(dr["MSec"]));

                        List<DateTime> timeList;
                        if (!idComtradMap.TryGetValue(commuIDItem, out timeList))
                        {
                            timeList = new List<DateTime>();
                            idComtradMap.Add(commuIDItem, timeList);
                        }
                        timeList.Add(logtime);
                    }
                }
                catch (System.Exception ex)
                {
                    WriteExceptionLog(ex);
                    return false;
                }
                finally
                {
                    Dispose(tempDT);
                }
            }
            return true;
        }

        private bool GetNodeStationChannelDeviceId(NodeParam node, out uint stationId, out uint channelId, out uint deviceId)
        {
            stationId = uint.MinValue;
            channelId = uint.MinValue;
            deviceId = uint.MinValue;
            //如果是厂站节点的父节点，就将厂站，通道，设备ID都设置为0
            //如果是厂站节点，就将通道，设备都设置为0；
            //如果是通道节点，就将设备节点设置为0；
            //如果是设备节点，就如实获取对应的厂站ID和通道ID
            switch (node.NodeType)
            {
                case PecsNodeType.PECSSYSCONFIG_NODE:
                    stationId = 0;
                    channelId = 0;
                    deviceId = 0;
                    break;
                case PecsNodeType.PECSSTATION_NODE:
                    stationId = node.NodeID;
                    channelId = 0;
                    deviceId = 0;
                    break;
                case PecsNodeType.PECSCHANNEL_NODE:
                    var nodeOne = PecsNodeManager.PecsNodeInstance.GetNodeByTypeID(node.NodeType, node.NodeID);
                    stationId = nodeOne.ParentNode.NodeID;
                    channelId = nodeOne.NodeID;
                    deviceId = 0;
                    break;
                case PecsNodeType.PECSDEVICE_NODE:
                    var nodeOneDevice = PecsNodeManager.PecsNodeInstance.GetNodeByTypeID(node.NodeType, node.NodeID);
                    stationId = nodeOneDevice.ParentNode.ParentNode.NodeID;
                    channelId = nodeOneDevice.ParentNode.NodeID;
                    deviceId = nodeOneDevice.NodeID;
                    break;

                default:
                    return false;
            }
            return true;
        }

        //public bool GetAllWave(List<StationChannelDevice> deviceList, DateTime startTime, DateTime endTime)
        //{
        //    foreach (var deviceNode in deviceList)
        //    {
        //        DataTable tempDT = new DataTable();
        //        try
        //        {
        //            uint stationId = deviceNode.StationId;
        //            uint channelId = deviceNode.ChannelId;
        //            uint deviceId = deviceNode.DeviceId;
        //            int logType = 2;
        //            int logHandle = 0;
        //            // 根据厂站/通道/设备/记录类型/记录标识等读取指定时间段内的原始记录,,可以指定是否读取dataFile字段,,返回结果集按照时间排序,读取波形记录,最多读取_maxNumberOfEvents条
        //            int errorCode = OriginaLogProvider.Instance.ReadOriglogs(DBOperationFlag.either, stationId,
        //                channelId, deviceId, logType, logHandle, startTime.AddMilliseconds(-startTime.Millisecond), startTime.Millisecond, endTime.AddMilliseconds(-endTime.Millisecond), endTime.Millisecond, 0, ref tempDT);
        //            if (errorCode != (int)ErrorCode.Success)
        //            {
        //                WriteLog(errorCode);
        //                return false;
        //            }

        //            List<WaveInformation> waveList;
        //            if (!_WaveDic.TryGetValue(deviceId, out waveList))
        //            {
        //                waveList = new List<WaveInformation>();
        //                _WaveDic.Add(deviceId, waveList);
        //            }
        //            foreach (DataRow oneRow in tempDT.Rows)
        //            {
        //                WaveInformation resultWave;
        //                if (!WaveInformation.ReadOneRow(oneRow, out resultWave))
        //                    continue;
        //                waveList.Add(resultWave);
        //            }
        //        }
        //        catch (System.Exception ex)
        //        {
        //            WriteExceptionLog(ex);
        //            return false;
        //        }
        //        finally
        //        {
        //            Dispose(tempDT);
        //        }
        //    }
        //    return true;
        //}
        #endregion



        internal bool GetAllWave(List<NodeDataParam> nodes, DateTime startTimeForQuery, DateTime endTimeForQuery, out Dictionary<StaChaDevID, List<DateTime>> idComtradMap)
        {
            List<NodeParam> nodesParam = new List<NodeParam>();
            foreach (var item in nodes)
            {
                nodesParam.Add(new NodeParam(item.nodeType, item.nodeID));
            }
            return GetAllWave(nodesParam, startTimeForQuery, endTimeForQuery, out idComtradMap);
        }
    }
    #endregion

    #region 结构定义
    public class EventInformation
    {
        public const string _NothingString = "--";
        /// <summary>
        /// 对于EventType=18的事件来说，Code2=1表示起始事件
        /// </summary>
        public const int StartEventCode2 = 1;
        /// <summary>
        /// 对于EventType=18的事件来说，Code2=2表示结束事件
        /// </summary>
        public const int EndEventCode2 = 2;
        /// <summary>
        /// 不包含毫秒的eventTime
        /// </summary>
        private DateTime _eventTimeWithoutSecond;
        /// <summary>
        /// 毫秒
        /// </summary>
        private int _msec;

        public int ID { get; set; }
        public DateTime EventTime
        {
            get
            {
                return FullTime;
            }
        }

        public int Msec
        {
            get { return _msec; }
            set { _msec = value; }

        }
        public int StationId { get; set; }
        public int ChannelId { get; set; }
        public int DeviceId { get; set; }
        public int StationFlag { get; set; }
        public int EventType { get; set; }
        public int EventByte { get; set; }
        public int EventClass { get; set; }
        public int Code1 { get; set; }
        public int Code2 { get; set; }
        public string EveStr1 { get; set; }
        public string EveStr2 { get; set; }
        public string Description { get; set; }

        public DateTime FullTime
        {
            get
            {
                return _eventTimeWithoutSecond.AddMilliseconds(Msec);
            }

        }

        public EventInformation(int id, DateTime eventTime, int msec, int stationid, int channelid, int deviceid, int stationFlag,
            int eventType, int eventByte, int eventClass, int code1, int code2, string evestr1, string evestr2, string description)
        {
            ID = id;
            _eventTimeWithoutSecond = eventTime.AddMilliseconds(-eventTime.Millisecond);
            Msec = msec;
            StationId = stationid;
            ChannelId = channelid;
            DeviceId = deviceid;
            StationFlag = stationFlag;
            EventType = eventType;
            EventByte = eventByte;
            EventClass = eventClass;
            Code1 = code1;
            Code2 = code2;
            EveStr1 = evestr1;
            EveStr2 = evestr2;
            Description = description;
        }

        /// <summary>
        /// 从DataRow读取PD_TB_06表的一行数据到eventInformation对象
        /// </summary>
        /// <param name="oneRow"></param>
        /// <param name="eventInformation"></param>
        /// <returns></returns>
        public static bool GetEventRow(EventLogByDevResponse oneRow, out EventInformation eventInformation)
        {
            eventInformation = null;
            int idTemp;
            if (!int.TryParse(oneRow.EventID.ToString(), out idTemp))
                return false;
            DateTime eventTimeTemp;
            if (!DateTime.TryParse(oneRow.EventTime.Split('.')[0], out eventTimeTemp))
                return false;
            int msecTemp;
            if (!int.TryParse(oneRow.EventTime.Split('.')[1], out msecTemp))
                return false;
            int stationIdTemp;
            if (!int.TryParse(oneRow.StationID.ToString(), out stationIdTemp))
                return false;
            int channelIdTemp;
            if (!int.TryParse(oneRow.ChannelID.ToString(), out channelIdTemp))
                return false;
            int deviceIdTemp;
            if (!int.TryParse(oneRow.DeviceID.ToString(), out deviceIdTemp))
                return false;
            int stationFlag;
            if (!int.TryParse(oneRow.StationFlag.ToString(), out stationFlag))
                return false;
            int eventTypeTemp;
            if (!int.TryParse(oneRow.EventType.ToString(), out eventTypeTemp))
                return false;
            int eventByteTemp;
            if (!int.TryParse(oneRow.EventByte.ToString(), out eventByteTemp))
                return false;
            int eventClassTemp;
            if (!int.TryParse(oneRow.EventClass.ToString(), out eventClassTemp))
                return false;
            int code1Temp;
            if (!int.TryParse(oneRow.EventCode1.ToString(), out code1Temp))
                return false;
            int code2Temp;
            if (!int.TryParse(oneRow.EventCode2.ToString(), out code2Temp))
                return false;
            string eveStr1Temp = oneRow.EventTypeStr.ToString();
            string eveStr2Temp = oneRow.EventClassStr.ToString();
            string descriptionTemp = oneRow.Description.ToString();
            eventInformation = new EventInformation(idTemp, eventTimeTemp, msecTemp, stationIdTemp, channelIdTemp, deviceIdTemp, stationFlag, eventTypeTemp, eventByteTemp, eventClassTemp, code1Temp, code2Temp, eveStr1Temp, eveStr2Temp, descriptionTemp);
            return true;
        }
    }

    /// <summary>
    /// EventType=18的事件，起始事件跟结束事件匹配的标准就是这些值一致
    /// </summary>
    public struct SagHandle
    {
        private int _stationId;
        private int _channelId;
        private int _deviceId;
        private int _eventByte;
        private int _eventType;
        private int _stationFlag;
        private int _code1;


        public int StationId
        {
            get { return _stationId; }
            set { _stationId = value; }
        }
        public int ChannelId
        {
            get { return _channelId; }
            set { _channelId = value; }
        }
        public int DeviceId { get { return _deviceId; } set { _deviceId = value; } }
        public int EventByte { get { return _eventByte; } set { _eventByte = value; } }
        public int EventType { get { return _eventType; } set { _eventType = value; } }
        public int StationFlag { get { return _stationFlag; } set { _stationFlag = value; } }
        public int Code1 { get { return _code1; } set { _code1 = value; } }

        public SagHandle(int stationId, int channelId, int deviceId, int eventByte, int eventType, int stationFlag, int code1)
        {
            _stationId = stationId;
            _channelId = channelId;
            _deviceId = deviceId;
            _eventByte = eventByte;
            _eventType = eventType;
            _stationFlag = stationFlag;
            _code1 = code1;
        }


    }
    public struct StationChannelDevice
    {
        private uint _StationId;
        private uint _ChannelId;
        private uint _DeviceId;

        public StationChannelDevice(uint stationId, uint channelId, uint deviceId)
        {
            _StationId = stationId;
            _ChannelId = channelId;
            _DeviceId = deviceId;
        }

        public uint StationId
        {
            get { return _StationId; }
        }

        public uint ChannelId
        {
            get { return _ChannelId; }
        }

        public uint DeviceId
        {
            get { return _DeviceId; }
        }
    }
    class StartEventKey
    {

        /// <summary>
        /// 不包含毫秒的eventTime
        /// </summary>
        private DateTime _eventTimeWithoutSecond;

        public int EventType
        {
            get;
            set;
        }
        public int Code1 { get; set; }
        public int Code2 { get; set; }


        public int Msec { get; set; }
        public DateTime FullTime { get { return _eventTimeWithoutSecond.AddMilliseconds(Msec); } }
        public StartEventKey(DateTime eventTime, int msec, int eventType, int code1, int code2)
        {
            _eventTimeWithoutSecond = eventTime.AddMilliseconds(-eventTime.Millisecond);
            Msec = msec;
            EventType = eventType;
            Code1 = code1;
            Code2 = code2;
        }

        private static List<int> _Code1ForStartEventList = null;

        private List<int> GetCode1ForStartEventIn()
        {
            List<int> resultList = new List<int>();

            foreach (var item in SysConstDefinition.SwellCodes)
            {
                resultList.Add(item);
            }
            foreach (var item in SysConstDefinition.SagCodes)
            {
                resultList.Add(item);
            }
            foreach (var item in SysConstDefinition.InteruptCodes)
            {
                resultList.Add(item);
            }
            //OtherCodes
            foreach (var item in SysConstDefinition.OtherCodes)
            {
                resultList.Add(item);
            }
            return resultList;
        }

        private List<int> GetCode1ForStartEventList()
        {
            if (_Code1ForStartEventList != null)
                return _Code1ForStartEventList;
            _Code1ForStartEventList = GetCode1ForStartEventIn();
            return _Code1ForStartEventList;
        }
        /// <summary>
        /// 判断是否是满足条件的起始事件
        /// 起始事件要满足的条件：1.EventType=18；2.Code2=1；3.Code1在SwellCodes，SagCodes，InteruptCodes,OtherCodes中
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public bool IsStartEvent(DateTime startTime, DateTime endTime)
        {
            DateTime fullTime = FullTime;
            if (fullTime >= endTime)
                return false;
            if (fullTime < startTime)
                return false;
            if (this.Code2 != EventInformation.StartEventCode2)
                return false;
            if (!GetCode1ForStartEventList().Contains(Code1))
                return false;
            return true;
        }
        /// <summary>
        /// 判断是否是结束事件
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private bool IsEndEvent(DateTime startTime, DateTime endTime)
        {
            DateTime fullTime = FullTime;

            DateTime tempEndTime;

            if (!EndTimeManager.GetInstance().GetLastEventTime(startTime, endTime, out tempEndTime))
                return false;
            //参照的结束时间是所有设备的最后一个起始事件的时间，往后偏移一个小时
            tempEndTime = tempEndTime.AddHours(1);
            if (fullTime >= tempEndTime)
                return false;
            if (fullTime < startTime)
                return false;
            if (this.Code2 != EventInformation.EndEventCode2)
                return false;
            if (!GetCode1ForStartEventList().Contains(Code1))
                return false;
            return true;
        }
        /// <summary>
        /// 判断是否是满足条件的起始事件或者结束事件
        /// 起始事件要满足的条件：1.EventType=18；2.Code2=1；3.Code1在SwellCodes，SagCodes，InteruptCodes,OtherCodes中
        /// 结束事件与起始事件匹配的条件之一是Code1相等，因此，结束事件的Code1在SwellCodes，SagCodes，InteruptCodes,OtherCodes中
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public bool IsStartEventOrEndEvent(DateTime startTime, DateTime endTime)
        {
            if (this.Code2 == EventInformation.StartEventCode2)
            {
                return IsStartEvent(startTime, endTime);
            }
            if (this.Code2 == EventInformation.EndEventCode2)
            {
                return IsEndEvent(startTime, endTime);
            }
            return false;
        }

        /// <summary>
        /// 从DataRow读取PD_TB_06表的一行数据的部分数据到eventInformation对象
        /// </summary>
        /// <param name="oneRow"></param>
        /// <param name="eventInformation"></param>
        /// <returns></returns>
        public static bool GetStartEventKey(EventLogByDevResponse oneRow, out StartEventKey eventInformation)
        {
            eventInformation = null;
            int idTemp;
            if (!int.TryParse(oneRow.EventID.ToString(), out idTemp))
                return false;
            DateTime eventTimeTemp;
            if (!DateTime.TryParse(oneRow.EventTime.Split('.')[0].ToString(), out eventTimeTemp))
                return false;
            int msecTemp;
            if (!int.TryParse(oneRow.EventTime.Split('.')[1].ToString(), out msecTemp))
                return false;
            int eventTypeTemp;
            if (!int.TryParse(oneRow.EventType.ToString(), out eventTypeTemp))
                return false;
            int code1Temp;
            if (!int.TryParse(oneRow.EventCode1.ToString(), out code1Temp))
                return false;
            int code2Temp;
            if (!int.TryParse(oneRow.EventCode2.ToString(), out code2Temp))
                return false;
            eventInformation = new StartEventKey(eventTimeTemp, msecTemp, eventTypeTemp, code1Temp, code2Temp);
            return true;
        }
    }

    public enum CurveType
    {
        /// <summary>
        /// ITIC曲线
        /// </summary>
        ITIC = 0,
        /// <summary>
        /// SEMI-100s曲线
        /// </summary>
        SEMI100s = 1,
        /// <summary>
        /// SEMI-1s曲线
        /// </summary>
        SEMI1s = 2,
    }
    public struct EMSACKNOWLEDGEMENT
    {
        public string ackDescription;
        public DateTime ackTime;
        public uint ackUserID;
        public string ackUserName;
        public uint channelID;
        public int code1;
        public int code2;
        public uint deviceID;
        public int eventByte;
        public int eventClass;
        public DateTime eventTime;
        public int eventType;
        public ushort msec;
        public byte stationFlag;
        public uint stationID;

        public EMSACKNOWLEDGEMENT(DateTime eventTime, ushort msec, uint stationID, uint channelID, uint deviceID,
            byte stationFlag, int eventType, int eventByte, int eventClass, int code1, int code2, DateTime ackTime,
            uint ackUserID, string ackUserName, string ackDescription)
        {
            this.ackDescription = ackDescription;
            this.ackTime = ackTime;
            this.ackUserID = ackUserID;
            this.ackUserName = ackUserName;
            this.channelID = channelID;
            this.code1 = code1;
            this.code2 = code2;
            this.deviceID = deviceID;
            this.eventByte = eventByte;
            this.eventClass = eventClass;
            this.eventTime = eventTime;
            this.eventType = eventType;
            this.msec = msec;
            this.stationFlag = stationFlag;
            this.stationID = stationID;
        }
    }

    #endregion

    #region 用于获取direction

    /// <summary>
    /// 用于获取Direction
    /// </summary>
    public class DirectionManager
    {
        #region 公有函数

        public bool GetOneDirection(EventInformation eventInformation, DateTime endTime, out string directionResult)
        {
            //给out参数一个默认的初值
            directionResult = EventInformation._NothingString;
            //得到一个设备的list，用来传给数据库接口查询
            List<NODE_TYPE_ID> nodeTypeIDList = GetNodeTypeIDList((uint) eventInformation.DeviceId);
            //起始事件对应的结束事件
            EventInformation eventInformationEnd;
            //下一个起始事件
            EventInformation nextStarteventInformation;
            //1.查找匹配的结束事件，以及下一个起始事件，看能不能找到。
            //如果没有找到，则相应的结果为null，例如eventInformationEnd==null
            if (
                !GetEndEventAndNextStart(eventInformation, endTime, nodeTypeIDList, out eventInformationEnd,
                    out nextStarteventInformation))
                return false;

            //2.(A)获取所有的设备的最后一个起始事件的时间
            DateTime lastStartEventTime;

            if (!EndTimeManager.GetInstance().GetLastEventTime(eventInformation.FullTime, endTime, out lastStartEventTime))
                return false;

            //2.(B)找出下一个起始事件,与所有的设备的最后一个起始事件的时间两者中，EventTime+Msec较小者
            DateTime tempEndTime = GetMinTime(eventInformationEnd, nextStarteventInformation, lastStartEventTime);
            //向后扩展一个小时
            tempEndTime = tempEndTime.AddHours(1);

            //3.查询从起始事件的时刻到tempEndTime之间的方向事件
            SortedDictionary<DateTime, List<EventInformation>> eventListResult;
            if (!GetDirections(eventInformation.FullTime, tempEndTime, nodeTypeIDList, out eventListResult))
                return false;
            //4.根据4种不同的情况得到Direction的值。
            SetDirectionValue(eventInformation, ref directionResult, eventInformationEnd, nextStarteventInformation, eventListResult);
            return true;
        }

        #endregion

        #region 私有函数

        private static void SetDirectionValue(EventInformation eventInformation, ref string directionResult,
            EventInformation eventInformationEnd, EventInformation nextStarteventInformation, SortedDictionary<DateTime, List<EventInformation>> eventListResult)
        {
            //(A)如果两者都存在：与结束事件无关了，只需要判断方向事件大于等于起始事件时间，小于下一个起始事件时间即可
            if (eventInformationEnd != null && nextStarteventInformation != null)
            {
                DateTime startTimeTemp = eventInformation.FullTime;
                DateTime nextStartTimeTemp = nextStarteventInformation.FullTime;
                foreach (var item in eventListResult)
                {
                    if (item.Key < startTimeTemp)
                        continue;
                    if (item.Key >= nextStartTimeTemp)
                        break;
                    directionResult = item.Value[0].Description;
                    break;
                }
            }
                //(B)结束事件存在，下一个起始事件为空：此时故障方向事件时间只需要>=起始事件即可匹配
            else if (eventInformationEnd != null && nextStarteventInformation == null)
            {
                DateTime startTimeTemp = eventInformation.FullTime;

                foreach (var item in eventListResult)
                {
                    if (item.Key < startTimeTemp)
                        continue;

                    directionResult = item.Value[0].Description;
                    break;
                }
            }
                //(C)说明既没有下一个起始事件也没有结束事件，此时故障方向事件>=起始事件即可匹配
            else if (eventInformationEnd == null && nextStarteventInformation == null)
            {
                DateTime startTimeTemp = eventInformation.FullTime;

                foreach (var item in eventListResult)
                {
                    if (item.Key < startTimeTemp)
                        continue;

                    directionResult = item.Value[0].Description;
                    break;
                }
            }
                //(D)已找到了下一个起始事件，但没有结束事件，说明结束事件丢失.
                //此时故障方向事件时间需要>=起始事件，<下一条起始事件
            else
            {
                DateTime startTimeTemp = eventInformation.FullTime;
                DateTime nextStartTimeTemp = nextStarteventInformation.FullTime;
                foreach (var item in eventListResult)
                {
                    if (item.Key < startTimeTemp)
                        continue;
                    if (item.Key >= nextStartTimeTemp)
                        break;
                    directionResult = item.Value[0].Description;
                    break;
                }
            }
        }

        /// <summary>
        /// 找起始事件对应的结束事件以及下一个起始事件
        /// </summary>
        /// <param name="startEventInformation"></param>
        /// <param name="endTime"></param>
        /// <param name="nodeTypeIDList"></param>
        /// <param name="eventInformationEnd"></param>
        /// <returns></returns>
        private bool GetEndEventAndNextStart(EventInformation startEventInformation, DateTime endTime, List<NODE_TYPE_ID> nodeTypeIDList, out EventInformation eventInformationEnd, out EventInformation nextStarteventInformation)
        {
            int[] eventType = new int[] {SysConstDefinition.SAG_SWELL_TYPE};
            PD_TB_06_Manager pm = new PD_TB_06_Manager();
            return pm.GetEndEventAndNextStart(endTime, startEventInformation, nodeTypeIDList, eventType, out eventInformationEnd, out nextStarteventInformation);
        }

        /// <summary>
        /// 查找EventType=19的记录，保存到eventListResult。其key是EventTime+Msec
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="nodeTypeIDList"></param>
        /// <param name="eventListResult"></param>
        /// <returns></returns>
        private bool GetDirections(DateTime startTime, DateTime endTime, List<NODE_TYPE_ID> nodeTypeIDList, out SortedDictionary<DateTime, List<EventInformation>> eventListResult)
        {
            int[] eventType = new int[] {SysConstDefinition.SAG_DIRECTION_TYPE};
            PD_TB_06_Manager pm = new PD_TB_06_Manager();
            return pm.GetEvents(startTime, endTime, nodeTypeIDList, eventType, out eventListResult);
        }

        /// <summary>
        /// 找出下一个起始事件,与所有的设备的最后一个起始事件的时间两者中，EventTime+Msec较小者
        /// </summary>
        /// <param name="eventInformationEnd"></param>
        /// <param name="nextStarteventInformation"></param>
        /// <returns></returns>
        private DateTime GetMinTime(EventInformation eventInformationEnd, EventInformation nextStarteventInformation, DateTime lastStartEventTime)
        {
            DateTime tempEndTime = lastStartEventTime;
            if (nextStarteventInformation != null && nextStarteventInformation.FullTime < lastStartEventTime)
            {
                tempEndTime = nextStarteventInformation.FullTime;
            }
            return tempEndTime;
        }

        /// <summary>
        /// 为了适应数据库接口的入参而做的转换
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        private List<NODE_TYPE_ID> GetNodeTypeIDList(uint deviceId)
        {
            List<uint> deviceIdList = new List<uint>();
            deviceIdList.Add(deviceId);
            return GetNodeTypeIDList(deviceIdList);
        }

        public static List<NODE_TYPE_ID> GetNodeTypeIDList(List<uint> deviceIdList)
        {
            List<NODE_TYPE_ID> nodeTypeIDList = new List<NODE_TYPE_ID>();
            foreach (var deviceId in deviceIdList)
            {
                NODE_TYPE_ID nodeTypeID = new NODE_TYPE_ID();
                nodeTypeID.nodeType = SysConstDefinition.PECSDEVICE_NODE;
                nodeTypeID.nodeID = deviceId;
                nodeTypeIDList.Add(nodeTypeID);
            }
            return nodeTypeIDList;
        }

        #endregion

    }

    #endregion
}