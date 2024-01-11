using System.Collections.Generic;
using System.Text;
using CET.PecsNodeManage;
using OfficeReportInterface.DefaultReportInterface.EnergyCost;

namespace OfficeReportInterface.DefaultReportInterface
{
    /// <summary>
    /// 用于名称的管理
    /// </summary>
     public  class NamesManager
    {
         private static NamesManager m_instanceSingleDevice = new NamesManager((uint)RepServFileType.SingleUsage);
         private static NamesManager m_instanceMultipleDevice = new NamesManager((uint)RepServFileType.MultiUsage);
         private static NamesManager m_instancePowerQuality = new NamesManager((uint)RepServFileType.PowerQuality);
         private static NamesManager m_instanceEnergyCost = new NamesManager((uint)RepServFileType.EnergyCost);
         private static NamesManager m_instanceEnergyPeriod = new NamesManager((uint)RepServFileType.EnergyPeriod);
         private static NamesManager m_instanceEventHistory = new NamesManager((uint)RepServFileType.EventHistory);
         private static NamesManager m_instanceHoulyUsage = new NamesManager((uint)RepServFileType.HourlyUsage);
         private static NamesManager m_instanceLoadProfile = new NamesManager((uint)RepServFileType.LoadProfile);
         private static NamesManager m_instanceTabular = new NamesManager((uint)RepServFileType.Tabular);
         private static NamesManager m_instanceTrend = new NamesManager((uint)RepServFileType.Trend);
         private static NamesManager m_instancePowerQualityEventsOnly = new NamesManager((uint)RepServFileType.PowerQualityEventsOnly);
         private static NamesManager m_instanceSafety = new NamesManager((uint)RepServFileType.Safety);

         /// <summary>
         /// key是设备名称，value是一串deviceId
         /// </summary>
         private Dictionary<string, List<uint>> m_hasSameNameDeviceId = new Dictionary<string, List<uint>>();

         /// <summary>
         /// 用于分隔设备与设备的字符
         /// </summary>
         public const string SpliterDot = "; ";
         /// <summary>
         /// 用于分隔设备与对应的回路之间的字符
         /// </summary>
         private const string SpliterLine = "-";

        private static bool GetInstanceForSource(uint source,out NamesManager instance )
        {
            switch (source)
            {
                case (uint)RepServFileType.SingleUsage:
                    instance = m_instanceSingleDevice;
                    break;
                case (uint)RepServFileType.MultiUsage:
                    instance = m_instanceMultipleDevice;
                    break;
                case (uint)RepServFileType.PowerQuality:
                    instance = m_instancePowerQuality;
                    break;
                case (uint)RepServFileType.PowerQualityEventsOnly:
                    instance = m_instancePowerQualityEventsOnly;
                    break;
                case (uint)RepServFileType.EnergyCost:
                    instance = m_instanceEnergyCost;
                    break;
                case (uint)RepServFileType.EnergyPeriod:
                    instance = m_instanceEnergyPeriod;
                    break;
                case (uint)RepServFileType.EventHistory:
                    instance = m_instanceEventHistory;
                    break;
                case (uint)RepServFileType.HourlyUsage:
                    instance = m_instanceHoulyUsage;
                    break;
                case (uint)RepServFileType.LoadProfile:
                    instance = m_instanceLoadProfile;
                    break;
                case (uint)RepServFileType.Tabular:
                    instance = m_instanceTabular;
                    break;
                case (uint)RepServFileType.Trend:
                    instance = m_instanceTrend;
                    break;
                case (uint)RepServFileType.Safety:
                    instance = m_instanceSafety;
                    break;
                default:
                    instance = m_instanceSingleDevice;
                    return false;
            }
            return true;
        }

        public static NamesManager GetInstance(uint source)
        {
            NamesManager instance;
            GetInstanceForSource(source, out instance);
            return instance;
        }

        private NamesManager(uint source)
        {
            Source = source;
        }
         /// <summary>
         /// 用于区分是哪个报表的
         /// </summary>
        private uint Source
        {
            get;
            set;
        }
     

        #region 公有函数

        private bool GetAllDeviceNamesListWithLoop( List<LogicalDeviceIndex> deviceList, out List<string> existList)
        {
            existList = new List<string>();
            try
            {
                foreach (var item in deviceList)
                {
                    if(item.NodeType==SysNodeType.PECSDEVICE_NODE)
                    {
                        string deviceName;
                        if (!GetDeviceNameWithLoop(item, out deviceName))
                            continue;
                        if (existList.Contains(deviceName))
                            continue;
                        existList.Add(deviceName);
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        ///// <summary>
        ///// 获取所有的设备名称
        ///// </summary>
        ///// <param name="deviceList"></param>
        ///// <param name="namesList"></param>
        ///// <returns></returns>
        /// <summary>
        /// 获取所有的设备名称，用list保存
        /// </summary>
        /// <param name="deviceList"></param>
        /// <param name="existList"></param>
        /// <returns></returns>
        public bool GetAllDeviceNamesListWithLoop(List<DeviceDataIDDef> deviceList, out List<string> existList)
        {
            existList = new List<string>();
            try
            {
                foreach (var item in deviceList)
                {
                    string deviceName;
                    if (!GetDeviceNameWithLoop(item, out deviceName))
                        continue;
                    if (existList.Contains(deviceName))
                        continue;
                    existList.Add(deviceName);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }
        public bool GetAllDeviceNamesWithLoop(List<LogicalDeviceIndex> deviceList, out string namesForDevice)
        {
            namesForDevice = string.Empty;
            try
            {
                StringBuilder allNames = new StringBuilder();
                List<string> existList;
                if (!GetAllDeviceNamesListWithLoop(deviceList, out existList))
                    return false;
                foreach (var item in existList)
                {
                    allNames.Append(item);
                    allNames.Append(SpliterDot);
                }

                string tempNames = allNames.ToString();

                namesForDevice = tempNames;
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }
        /// <summary>
        /// 获取所有的设备名称，用于显示在一行中
        /// </summary>
        /// <param name="deviceList"></param>
        /// <param name="namesForDevice"></param>
        /// <returns></returns>
        public bool GetAllDeviceNamesWithLoop(List<DeviceDataIDDef> deviceList, out string namesForDevice)
        {
            namesForDevice = string.Empty;
            try
            {
                StringBuilder allNames = new StringBuilder();
                List<string> existList;
                if (!GetAllDeviceNamesListWithLoop(deviceList, out existList))
                    return false;
                foreach (var item in existList)
                {
                    allNames.Append(item);
                    allNames.Append(SpliterDot);
                }

                string tempNames = allNames.ToString();
                namesForDevice = tempNames;
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }


        private bool GetDeviceNameWithLoop( LogicalDeviceIndex deviceDataIdDef, out string deviceNameWithLoop)
        {
            deviceNameWithLoop = string.Empty;
            string name;
            if (!GetDeviceName(deviceDataIdDef.DeviceID, out name))
                return false;
            string loopString;
            if (!GetLoopStringWithSpliter(deviceDataIdDef.LogicalIndex, out loopString))
                return false;
            deviceNameWithLoop = string.Format("{0}{1}", name, loopString);
            return true;
        }
        /// <summary>
        /// 获取包含回路的设备名称
        /// </summary>
        /// <param name="deviceDataIDDef"></param>
        /// <param name="deviceNameWithLoop"></param>
        /// <returns></returns>
        public bool GetDeviceNameWithLoop(DeviceDataIDDef deviceDataIDDef, out string deviceNameWithLoop)
        {
            deviceNameWithLoop = string.Empty;
            string name;
            if (!GetDeviceName(deviceDataIDDef, out name))
                return false;
            string loopString;
            if (!GetLoopStringWithSpliter(deviceDataIDDef, out loopString))
                return false;
            deviceNameWithLoop = string.Format("{0}{1}", name, loopString);
            return true;
        }
        public bool GetDeviceNameWithLoop(uint deviceId, int logicalDeviceIndex, out string deviceNameWithLoop)
        {
            deviceNameWithLoop = string.Empty;
            string name;
            if (!GetDeviceName(deviceId, out name))
                return false;
            string loopString;
            if (!GetLoopStringWithSpliter(logicalDeviceIndex, out loopString))
                return false;
            deviceNameWithLoop = string.Format("{0}{1}", name, loopString);
            return true;
        }
        /// <summary>
        /// 获取设备名称，只是设备名称，不包含回路号
        /// </summary>
        /// <param name="deviceDataIDDef"></param>
        /// <param name="deviceNameWithoutLoop"></param>
        /// <returns></returns>
        private bool GetDeviceName(DeviceDataIDDef deviceDataIDDef, out string deviceNameWithoutLoop)
        {
            return GetDeviceName(deviceDataIDDef.DeviceID, out deviceNameWithoutLoop);
        }
         /// <summary>
         /// 判断是否存在相同名称的设备
         /// </summary>
         /// <param name="deviceNodeList"></param>
         /// <returns></returns>
        public bool HasSameDeviceName(List<LogicalDeviceIndex> deviceNodeList )
        {
            if (deviceNodeList.Count == 0)
                return false;
             bool has = false;
            m_hasSameNameDeviceId = new Dictionary<string, List<uint>>();            
            foreach (var item in deviceNodeList)
            {
                string name = string.Empty;
                if (!GetDeviceName(item.DeviceID, out name))
                    continue;
                List<uint> v;
                if (!m_hasSameNameDeviceId.TryGetValue(name, out v))
                {
                    v = new List<uint>();
                    m_hasSameNameDeviceId.Add(name, v);
                }
                if (!v.Contains(item.DeviceID))
                  v.Add(item.DeviceID);
                if (v.Count > 1)
                    has = true;
            }

             return has;
        }


         /// <summary>
        /// 获取设备名称，只是设备名称，不包含回路号
         /// </summary>
         /// <param name="deviceId"></param>
         /// <param name="deviceNameWithoutLoop"></param>
         /// <returns></returns>
        private bool GetDeviceName(uint deviceId, out string deviceNameWithoutLoop)
        {
            deviceNameWithoutLoop = string.Empty;
            PecsDeviceNode deviceNode = PecsNodeManager.PecsNodeInstance.GetDeviceNodeByID(deviceId);
            if (deviceNode == null)
                return false;
         

             List<uint> v;
             if (m_hasSameNameDeviceId.TryGetValue(deviceNode.NodeName, out v)&&v.Count > 1)
             {
                     deviceNameWithoutLoop = string.Format("{0}({1})", deviceNode.NodeName, deviceId);
             }
             else
             {
                  deviceNameWithoutLoop = deviceNode.NodeName;
             }
            return true;
        }

        /// <summary>
        /// 获取测点名称,dataName-dataTypeName
        /// </summary>
        /// <param name="deviceDataIDDef"></param>
        /// <param name="measureName"></param>
        /// <returns></returns>
        public bool GetMeasureName(DeviceDataIDDef deviceDataIDDef, out string measureName)
        {
            measureName = string.Empty;
            string dataName;
            if (!GetParaName(deviceDataIDDef, out dataName))
                return false;
            string dataTypeName;
            if (!GetDataTypeNameWithSpliter(deviceDataIDDef, out dataTypeName))
                return false;
            string loopString;
            if (!GetLoopStringWithSpliter(deviceDataIDDef, out loopString))
                return false;
        //    measureName = string.Format("{0}{1}{2}", dataName, dataTypeName, loopString);
            measureName = string.Format("{0}{1}", dataName, dataTypeName);
            return true;
        }

        /// <summary>
        /// 获取一系列测点名称，用分号分隔
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        public string GetMeasureNameList(List<DataIDNameTypeDef> nodeList, List<LogicalDeviceIndex> deviceNodeList)
        {
            List<DeviceDataIDDef> deviceDataIDDefList = new List<DeviceDataIDDef>();
            foreach (var deviceItem in deviceNodeList)
            {
                foreach (var dataItem in nodeList)
                {
                   
                    deviceDataIDDefList.Add(new DeviceDataIDDef(deviceItem.DeviceID, dataItem.DataID, 1, dataItem.DataTypeID, deviceItem.LogicalIndex));
                }
            }
            string measureNameList = string.Empty;
            if (GetMeasurementNames(deviceDataIDDefList, out measureNameList))
                return measureNameList;
            return string.Empty;

        }
        public bool GetMeasurementNames(List<DeviceDataIDDef> deviceDataIDDefList,out string measureNameList)
        {
            List<string> paraNameMeasureList = new List<string>();
            foreach (var deviceDataIDDef in deviceDataIDDefList)
            {
                string measureName;
                if (!GetMeasureName(deviceDataIDDef, out measureName))
                    continue;
                if (paraNameMeasureList.Contains(measureName))
                    continue;
                paraNameMeasureList.Add(measureName);

            }
            StringBuilder tempList = new StringBuilder();
            foreach (var measureName in paraNameMeasureList)
            {
                tempList.Append(measureName);
                tempList.Append(SpliterDot);
            }
          
            measureNameList = tempList.ToString();
            return true;
        }


        #endregion



      
        /// <summary>
        /// 获取回路字符串，如果是回路1，就返回空字符串
        /// </summary>
        /// <param name="deviceDataIDDef"></param>
        /// <param name="loopString"></param>
        /// <returns></returns>
        private bool GetLoopStringWithSpliter(DeviceDataIDDef deviceDataIDDef,out string loopString)
        {
            loopString = string.Empty;
            try
            {
                if (deviceDataIDDef.LogicalDeviceIndex > 1)
                    loopString = string.Format("{0}{1}{2}", SpliterLine,LocalResourceManager.GetInstance().GetString("0001", "Loop"),
                        deviceDataIDDef.LogicalDeviceIndex);
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace); 
                return false;
            }
        }


        private bool GetLoopStringWithSpliter(int logicalDeviceIndex, out string loopString)
        {
            loopString = string.Empty;
            try
            {
                if (logicalDeviceIndex > 1)
                    loopString = string.Format("{0}{1}{2}", SpliterLine, LocalResourceManager.GetInstance().GetString("0001", "Loop"),
                        logicalDeviceIndex);
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace); 
                return false;
            }
        }



      
    
       

        /// <summary>
        /// 获取paraName
        /// </summary>
        /// <param name="deviceDataIDDef"></param>
        /// <param name="dataName"></param>
        /// <returns></returns>
        private bool GetParaName(DeviceDataIDDef deviceDataIDDef, out string paraName)
        {
            if (!ParaNameManager.GetInstance().GetParaName(deviceDataIDDef, out paraName))
            {
                if (!GetDataName(deviceDataIDDef.DataID, out paraName))
                    return false;
            }
            return true;
        }

        public static bool GetDataName(uint dataId, out string dataName)
        {
            dataName = string.Empty;
            try
            {
                var node = PecsNodeManager.PecsNodeInstance.FindDataID(dataId);
                dataName = node.DataName;

                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 获取dataTypeName
        /// </summary>
        /// <param name="deviceDataIDDef"></param>
        /// <param name="dataTypeName"></param>
        /// <returns></returns>
        private bool GetDataTypeName(DeviceDataIDDef deviceDataIDDef, out string dataTypeName)
        {
            return GetDataTypeName(deviceDataIDDef.DataTypeID, out dataTypeName);
        }
        /// <summary>
        /// 获取dataTypeName
        /// </summary>
        /// <param name="dataTypeId"></param>
        /// <param name="dataTypeName"></param>
        /// <returns></returns>
        private bool GetDataTypeName(int dataTypeId,out string dataTypeName)
        {
            dataTypeName = string.Empty;
            try
            {
                var node = PecsNodeManager.PecsNodeInstance.FindDataType(dataTypeId);
                dataTypeName = node.DataTypeName;
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace); 
                return false;
            }
        }
        /// <summary>
        /// 加有分隔符的dataTypeName，例如“-最大值”
        /// </summary>
        /// <param name="deviceDataIDDef"></param>
        /// <param name="dataTypeName"></param>
        /// <returns></returns>
        private bool GetDataTypeNameWithSpliter(DeviceDataIDDef deviceDataIDDef, out string dataTypeName)
        {
            return GetDataTypeNameWithSpliter(deviceDataIDDef.DataTypeID, out dataTypeName);
        }

        /// <summary>
        ///  加有分隔符的dataTypeName，例如“-最大值”
        /// </summary>
        /// <param name="deviceDataIDDef"></param>
        /// <param name="dataTypeName"></param>
        /// <returns></returns>
        private bool GetDataTypeNameWithSpliter(int dataTypeId, out string dataTypeName)
        {
            dataTypeName = string.Empty;
            //如果是实时值,就不用显示了
            if (dataTypeId == 1)
                return true;
            string tempName;
            if (!GetDataTypeName(dataTypeId, out tempName))
                return false;

            dataTypeName = string.Format("{0}{1}", SpliterLine, tempName);
            return true;
        }

       
        /// <summary>
        /// 去掉最后一个分隔符
        /// </summary>
        /// <param name="oldString"></param>
        /// <param name="newString"></param>
        /// <returns></returns>
        private bool EriseLastSpliterDot(string oldString,out string newString)
        {
            newString = string.Empty;
            try
            {
                newString = oldString.Substring(0, oldString.Length - SpliterDot.Length);
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace); 
                return false;
            }
           
        }

    }
}
