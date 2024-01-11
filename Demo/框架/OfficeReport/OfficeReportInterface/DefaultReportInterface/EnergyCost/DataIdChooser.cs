using System.Collections.Generic;
using System;

namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
    public  class DataIdChooser
    {
        private static DataIdChooser m_instance;

        public static DataIdChooser GetInstance()
        {
            if (m_instance == null)
                m_instance = new DataIdChooser();
            return m_instance;
        }
        public const int TOTAL_INDEX = 0;
        public const int IMPORT_INDEX = 1;
        public const int INCREASEMENT_INDEX = 2;
        public const int AUTOFIT_INDEX = 3;
        public const int NONE_ENERGY = 4;
        public const int NONE_DEMAND = 1;
       
        /// <summary>
        /// 总正向有功电度，正向有功电度净值，间隔正向有功电度
        /// 总无功电度 4000032 总无功电度净值4000028 间隔无功电度 4000431
        ///总视在电度 4000048 总视在电度净值 4000044 间隔视在电度 4000433
        /// </summary>
        private Dictionary<uint, List<uint>> m_dataIdDic ;

        private DataIdChooser()
        {
            AddItems();
        }

        /// <summary>
        /// 有些装置只有正向，有些只有总，一般情况下都是会有这两个参数，所以在电能参数上相当于是做了一个优先级，如果有总和的话，就以这个参数来展示数据，如果没有这个参数的话，就以正向来展示数据，这种处理，基本上可以让所有的装置都能展示数据。
        /// </summary>
        /// <param name="deviceDataIDDefList"></param>
        /// <returns></returns>
        public static List<DeviceDataIDDef> GetDataIdList(DefaultReportParameter parameter,out Dictionary<DeviceDataIDDef, DeviceDataIDDef> changedDic)
        {
            DateTime startTime=parameter.StartTime;
            DateTime endTime=parameter.EndTime;
            List<DeviceDataIDDef> deviceDataIDDefList = parameter.DeviceDataIDList;
            int kwhSelectedIndex = parameter.kWh;
            int kvarhSelectedIndex = parameter.kvarh;
            int kVAhSelectedIndex = parameter.kVAh;
            int kwSelectedIndex = parameter.kWDemand;
            int kvarSelectedIndex = parameter.kvarDemand;
            int kVASelectedIndex = parameter.kVADemand;

            changedDic = new Dictionary<DeviceDataIDDef, DeviceDataIDDef>();
            List<DeviceDataIDDef> resultList = new List<DeviceDataIDDef>();
            foreach (var item in deviceDataIDDefList)
            {
                if ((item.DataID == SysConstDefinition.DATAIDKWH && kwhSelectedIndex == NONE_ENERGY))
                {
                    continue;
                }
                if ((item.DataID == SysConstDefinition.DATAIDKVARH && kvarhSelectedIndex == NONE_ENERGY))
                {
                    continue;
                }
                if ((item.DataID == SysConstDefinition.DATAIDKVAH && kVAhSelectedIndex == NONE_ENERGY))
                {
                    continue;
                }

                if ((item.DataID == SysConstDefinition.DATAIDKW_DEMAND && kwSelectedIndex == NONE_DEMAND))
                {
                    continue;
                }
                if ((item.DataID == SysConstDefinition.DATAIDKVAR_DEMAND && kvarSelectedIndex == NONE_DEMAND))
                {
                    continue;
                }
                if ((item.DataID == SysConstDefinition.DATAIDKVA_DEMAND && kVASelectedIndex == NONE_DEMAND))
                {
                    continue;
                }
               

                //如果不是kwh，直接加入
                if (item.DataID != SysConstDefinition.DATAIDKWH && item.DataID != SysConstDefinition.DATAIDKVARH && item.DataID != SysConstDefinition.DATAIDKVAH)
                {
                    resultList.Add(item);
                    continue;
                }
               
                //kwh的selectedIndex是0,1,2的情况
                if ((item.DataID == SysConstDefinition.DATAIDKWH && kwhSelectedIndex == TOTAL_INDEX))
                {
                    resultList.Add(item);
                    continue;
                }
                if ((item.DataID == SysConstDefinition.DATAIDKWH && kwhSelectedIndex == IMPORT_INDEX))
                {
                    uint kwhSecondDataId;
                    DataIdChooser.GetInstance().GetSecondDataId(item.DataID, out kwhSecondDataId);
                    DeviceDataIDDef kwhImportDef = new DeviceDataIDDef(item.NodeType,item.DeviceID, kwhSecondDataId, item.ParaTypeID, item.DataTypeID, item.LogicalDeviceIndex);
                    resultList.Add(kwhImportDef);
                    continue;
                }
                if ((item.DataID == SysConstDefinition.DATAIDKWH && kwhSelectedIndex == INCREASEMENT_INDEX))
                {
                    uint kwhThirdDataId;
                    DataIdChooser.GetInstance().GetThirdDataId(item.DataID, out kwhThirdDataId);
                    DeviceDataIDDef kwhIncreaseDef = new DeviceDataIDDef(item.NodeType, item.DeviceID, kwhThirdDataId, item.ParaTypeID, item.DataTypeID, item.LogicalDeviceIndex);
                    resultList.Add(kwhIncreaseDef);
                    continue;
                }
              
                //kvarh的selectedIndex是0,1,2的情况
                if ((item.DataID == SysConstDefinition.DATAIDKVARH && kvarhSelectedIndex == TOTAL_INDEX))
                {
                    resultList.Add(item);
                    continue;
                }
                if ((item.DataID == SysConstDefinition.DATAIDKVARH && kvarhSelectedIndex == IMPORT_INDEX))
                {
                    uint kvarhSecondDataId;
                    DataIdChooser.GetInstance().GetSecondDataId(item.DataID, out kvarhSecondDataId);
                    DeviceDataIDDef kvarhImportDef = new DeviceDataIDDef(item.NodeType,item.DeviceID, kvarhSecondDataId, item.ParaTypeID, item.DataTypeID, item.LogicalDeviceIndex);
                    resultList.Add(kvarhImportDef);
                    continue;
                }
                if ((item.DataID == SysConstDefinition.DATAIDKVARH && kvarhSelectedIndex == INCREASEMENT_INDEX))
                {
                    uint kvarhThirdDataId;
                    DataIdChooser.GetInstance().GetThirdDataId(item.DataID, out kvarhThirdDataId);
                    DeviceDataIDDef kvarhIncreaseDef = new DeviceDataIDDef(item.NodeType,item.DeviceID, kvarhThirdDataId, item.ParaTypeID, item.DataTypeID, item.LogicalDeviceIndex);
                    resultList.Add(kvarhIncreaseDef);
                    continue;
                }
                //kVAh的selectedIndex是0,1,2的情况
                if ((item.DataID == SysConstDefinition.DATAIDKVAH && kVAhSelectedIndex == TOTAL_INDEX))
                {
                    resultList.Add(item);
                    continue;
                }
                if ((item.DataID == SysConstDefinition.DATAIDKVAH && kVAhSelectedIndex == IMPORT_INDEX))
                {
                    uint kVAhSecondDataId;
                    DataIdChooser.GetInstance().GetSecondDataId(item.DataID, out kVAhSecondDataId);
                    DeviceDataIDDef kVAhImportDef = new DeviceDataIDDef(item.NodeType,item.DeviceID, kVAhSecondDataId, item.ParaTypeID, item.DataTypeID, item.LogicalDeviceIndex);
                    resultList.Add(kVAhImportDef);
                    continue;
                }
                if ((item.DataID == SysConstDefinition.DATAIDKVAH && kVAhSelectedIndex == INCREASEMENT_INDEX))
                {
                    uint kVAhThirdDataId;
                    DataIdChooser.GetInstance().GetThirdDataId(item.DataID, out kVAhThirdDataId);
                    DeviceDataIDDef kVAhIncreaseDef = new DeviceDataIDDef(item.NodeType,item.DeviceID, kVAhThirdDataId, item.ParaTypeID, item.DataTypeID, item.LogicalDeviceIndex);
                    resultList.Add(kVAhIncreaseDef);
                    continue;
                }
                //下面是自适应的情况

                DataAcquisitionDatalog dataAcquisitionDatalog = new DataAcquisitionDatalog();
                bool result = dataAcquisitionDatalog.HasData(item.DeviceID, (uint)item.LogicalDeviceIndex, item.DataID,
                    (uint)item.DataTypeID, (int)ParaTypes.Datalog, startTime, endTime);
                //如果是kwh，且数据存在，则加入
                if (result)
                {
                    resultList.Add(item);
                    continue;
                }

                //如果不是kwh， 查找dataId=DATAIDKWH_IMPORT = 4000004的数据是否存在，如果存在，则加入
                uint secondDataId;
                if (!DataIdChooser.GetInstance().GetSecondDataId(item.DataID, out secondDataId))
                {
                    resultList.Add(item);
                    continue;
                }


                DeviceDataIDDef importDef = new DeviceDataIDDef(item.NodeType,item.DeviceID, secondDataId, item.ParaTypeID, item.DataTypeID, item.LogicalDeviceIndex);
                DataAcquisitionDatalog dataAcquisitionDatalog2 = new DataAcquisitionDatalog();
                bool result2 = dataAcquisitionDatalog2.HasData(importDef.DeviceID, (uint)importDef.LogicalDeviceIndex, importDef.DataID,
                      (uint)importDef.DataTypeID, (int)ParaTypes.Datalog, startTime, endTime);
                if (result2)
                {
                    resultList.Add(importDef);
                    changedDic.Add(item, importDef);
                    continue;
                }
                //如果即不是kwh，又不是DATAIDKWH_IMPORT ，查找dataId=4000429的增量正向有功电度。如果存在，则加入；如果不存在，则加入kwh
                uint thirdDataId;
                if (!DataIdChooser.GetInstance().GetThirdDataId(item.DataID, out thirdDataId))
                {
                    resultList.Add(item);
                    continue;
                }

                DeviceDataIDDef increaseDef = new DeviceDataIDDef(item.NodeType,item.DeviceID, thirdDataId, item.ParaTypeID, item.DataTypeID, item.LogicalDeviceIndex);
                DataAcquisitionDatalog dataAcquisitionDatalog3 = new DataAcquisitionDatalog();
                bool result3 = dataAcquisitionDatalog3.HasData(increaseDef.DeviceID, (uint)increaseDef.LogicalDeviceIndex, increaseDef.DataID,
               (uint)increaseDef.DataTypeID, (int)ParaTypes.Datalog, startTime, endTime);
                if (result3)
                {
                    resultList.Add(increaseDef);
                    changedDic.Add(item, increaseDef);
                    continue;
                }

                //如果都不存在，则看谁有sourceId,要是都没有sourceId，看谁有映射方案。如果都没有映射方案，则选择原来的item
                Dictionary<DataAcquisitionDatalog, DeviceDataIDDef> dataAcquisitionDatalogList = new Dictionary<DataAcquisitionDatalog, DeviceDataIDDef>();
                dataAcquisitionDatalogList.Add(dataAcquisitionDatalog, item);
                dataAcquisitionDatalogList.Add(dataAcquisitionDatalog2, importDef);
                dataAcquisitionDatalogList.Add(dataAcquisitionDatalog3, increaseDef);


                GetRightDataId(dataAcquisitionDatalogList, ref resultList, ref changedDic, item);


            }
            return resultList;
        }

        private static void GetRightDataId(Dictionary<DataAcquisitionDatalog, DeviceDataIDDef> dataAcquisitionDatalogList, ref List<DeviceDataIDDef> resultList, ref  Dictionary<DeviceDataIDDef, DeviceDataIDDef> changedDic, OfficeReportInterface.DeviceDataIDDef item)
        {
            bool hasFound = false;
            foreach (var itemDataAcquisitionDatalog in dataAcquisitionDatalogList)
            {
                if (itemDataAcquisitionDatalog.Key.HasDataAtLeastOne)
                {
                    resultList.Add(itemDataAcquisitionDatalog.Value);
                    if (item.DataID != itemDataAcquisitionDatalog.Value.DataID)
                        changedDic.Add(item, itemDataAcquisitionDatalog.Value);
                    hasFound = true;
                    break;
                }
            }
            if (hasFound == false)
            {
                foreach (var itemDataAcquisitionDatalog in dataAcquisitionDatalogList)
                {
                    if (itemDataAcquisitionDatalog.Key.HasSourceId)
                    {
                        resultList.Add(itemDataAcquisitionDatalog.Value);
                        if (item.DataID != itemDataAcquisitionDatalog.Value.DataID)
                            changedDic.Add(item, itemDataAcquisitionDatalog.Value);
                        hasFound = true;
                        break;
                    }
                }
            }
            if (hasFound == false)
            {
                foreach (var itemDataAcquisitionDatalog in dataAcquisitionDatalogList)
                {
                    if (itemDataAcquisitionDatalog.Key.HasMap)
                    {
                        resultList.Add(itemDataAcquisitionDatalog.Value);
                        if (item.DataID != itemDataAcquisitionDatalog.Value.DataID)
                            changedDic.Add(item, itemDataAcquisitionDatalog.Value);
                        hasFound = true;
                        break;
                    }
                }

            }

            if (hasFound == false)
            {
                resultList.Add(item);
            }
        }


        private void AddItems()
        {
            m_dataIdDic = new Dictionary<uint, List<uint>>();

            var nodeKwh = new List<uint>();
            nodeKwh.Add(SysConstDefinition.DATAIDKWH_IMPORT);
            nodeKwh.Add(SysConstDefinition.DATAIDKWH_INCREASEIMPORT);
            m_dataIdDic.Add(SysConstDefinition.DATAIDKWH, nodeKwh);

            var nodekvarh = new List<uint>();
            nodekvarh.Add(SysConstDefinition.DATAIDKVARH_IMPORT);
            nodekvarh.Add(SysConstDefinition.DATAIDKVARH_INCREASEIMPORT);
            m_dataIdDic.Add(SysConstDefinition.DATAIDKVARH, nodekvarh);

            var nodeKvAh = new List<uint>();
            nodeKvAh.Add(SysConstDefinition.DATAIDKVAH_IMPORT);
            nodeKvAh.Add(SysConstDefinition.DATAIDKVAH_INCREASEIMPORT);
            m_dataIdDic.Add(SysConstDefinition.DATAIDKVAH, nodeKvAh);
        }
        /// <summary>
        /// 获取dataId 的第二选择
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="dataIdSecond"></param>
        /// <returns></returns>
        public bool GetSecondDataId(uint dataId,out uint dataIdSecond)
        {
            return GetNDataId(dataId, 1, out dataIdSecond);
        }
        /// <summary>
        /// 获取dataId 的第三选择
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="dataIdSecond"></param>
        /// <returns></returns>
        public bool GetThirdDataId(uint dataId, out uint dataIdSecond)
        {
            return GetNDataId(dataId, 2, out dataIdSecond);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="index"></param>
        /// <param name="dataIdSecond"></param>
        /// <returns></returns>
        private bool GetNDataId(uint dataId, int index, out uint dataIdSecond)
        {
            dataIdSecond = 0;
            List<uint> nodeList;
            if (!m_dataIdDic.TryGetValue(dataId, out nodeList))
                return false;
            if (nodeList.Count < index)
                return false;
            dataIdSecond = nodeList[index-1];
            return true;
        }

    }
}
