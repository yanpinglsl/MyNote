using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using CET.PecsNodeManage;
using CSharpDBPlugin;
using DBInterfaceCommonLib;

namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
    /// <summary>
    /// 用于获取定时记录类型的数据,高速定时记录的数据
    /// </summary>
    class DataAcquisitionDatalog
    {

        #region 成员变量
        private string m_errorMessage = string.Empty;
        public string GetErrorMessage()
        {
            return m_errorMessage;
        }

        public bool HasMap { get; set; }

        public bool HasSourceId { get; set; }

        public bool HasDataAtLeastOne { get; set; }
        public DataAcquisitionDatalog()
        {
            HasMap = false;
            HasSourceId = false;
            HasDataAtLeastOne = false;
        }
        #endregion
      
        #region  提供给外部调用的方法
        public bool HasData(uint deviceId,uint logicalDeviceIndex,uint dataId,uint dataTypeId,int paraType, DateTime startTime,DateTime endTime)
        {
            int paraHandleValue;
            DeviceDataIDDef deviceDataIDDef = new DeviceDataIDDef();
            deviceDataIDDef.DeviceID = deviceId;
            deviceDataIDDef.LogicalDeviceIndex = (int) logicalDeviceIndex;
            deviceDataIDDef.DataID = dataId;
            deviceDataIDDef.DataTypeID = (int)dataTypeId;
            deviceDataIDDef.ParaTypeID =paraType;

            //if (!GetParaHandle(deviceId, logicalDeviceIndex, dataId, dataTypeId, paraType, out paraHandleValue))
            //    return false;
            //HasMap = true;
            //PecsDeviceNode deviceNode = PecsNodeManager.PecsNodeInstance.GetDeviceNodeByID(deviceId);
            //if (deviceNode == null)
            //    return false;
            //uint stationID = deviceNode.ParentNode.ParentNode.NodeID;
            //uint channelID = deviceNode.ParentNode.NodeID;
            //SourceIDAndParaIndex sourceIdAndParaIndex = new SourceIDAndParaIndex();
            //if (!GetSourceIdAndParaIndex(stationID,channelID,deviceId, paraHandleValue, ref sourceIdAndParaIndex))
            //    return false;
            //HasSourceId = true;
            //HasDataAtLeastOne= HasDataMoreThanZero(stationID, sourceIdAndParaIndex, startTime,endTime);
            //return HasDataAtLeastOne;
            return HasDataMoreThanZero(deviceDataIDDef, startTime, endTime);
        }
       
      

        /// <summary>
        /// 判断是否存在映射方案
        /// </summary>
        /// <param name="deviceDataIDDef"></param>
        /// <returns></returns>
        public bool HasDataMap(DeviceDataIDDef deviceDataIDDef)
        {
            //int paraHandle = int.MinValue;
            //return GetParaHandle(deviceDataIDDef.DeviceID, (uint)deviceDataIDDef.LogicalDeviceIndex, deviceDataIDDef.DataID,
            //   (uint) deviceDataIDDef.DataTypeID, deviceDataIDDef.ParaTypeID, out paraHandle);

            DATALOG_PRIVATE_MAP resultMapDef;
            return ReportWebServiceManager.ReportWebManager.FindDataMapDef(deviceDataIDDef, out resultMapDef);
        }
        #endregion
        #region  私有方法

        ///// <summary>
        ///// 第一步，获取ParaHandle
        ///// </summary>
        ///// <param name="aDatalogMapNode">测点</param>
        ///// <param name="paraHandle">获取的paraHandle的值</param>
        ///// <returns>获取数据是否成功</returns>
        //private bool GetParaHandle(uint deviceId,uint logicalDeviceIndex,uint dataId,uint dataTypeId,int paraType, out int paraHandle)
        //{
        //    paraHandle = -1;
                   
        //    if (deviceId == 0) //说明只勾选了“关联设备”但是没有选择设备
        //    {
        //        DbgTrace.dout("Get parahandle failed because deviceID is 0.");
        //        return false;
        //    }
        //    DataTable queryResultMap = new DataTable();
        //    bool suc = false;
        //    try
        //    {
        //        int errorCode = DatalogPrivateMapProvider.Instance.ReadByDataID(deviceId,logicalDeviceIndex, dataId, dataTypeId,paraType , ref queryResultMap); 
        //        if (errorCode != 0)
        //        {
        //            m_errorMessage = ErrorQuerier.Instance.GetLastErrorString();
        //            suc = false;
        //        }
        //        else
        //        {
                   
                
        //            foreach (DataRow row in queryResultMap.Rows)
        //            {
        //                uint logicalDeviceIndexTemp;
        //                if (!uint.TryParse(row["LogicalDeviceIndex"].ToString(), out logicalDeviceIndexTemp))
        //                    continue;
                    
        //                if (logicalDeviceIndexTemp != logicalDeviceIndex)
        //                    continue;
                        
        //                if (!int.TryParse(row["ParaHandle"].ToString(), out paraHandle))
        //                    continue;
        //                suc = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        m_errorMessage = ex.Message+ex.StackTrace;
        //        DbgTrace.dout(ex.Message + ex.StackTrace);
        //        suc = false;
        //    }
        //    finally
        //    {
        //        if (queryResultMap != null)
        //            queryResultMap.Dispose();
        //    }
        //    return suc;

        //}

        private const string NULL_DATABASE_VALUE_STRING = "-2147483648";
        ///// <summary>
        ///// 第二步，获取sourceID，paraIndex
        ///// </summary>
        ///// <param name="oneDevice">设备对象，包括厂站，通道，设备</param>
        ///// <param name="paraHandle"></param>
        ///// <param name="sourceIdAndParaIndex"></param>
        ///// <returns>获取数据是否成功</returns>
        //private bool GetSourceIdAndParaIndex(uint stationID, uint channelID,
        //    uint deviceId,int paraHandle,ref SourceIDAndParaIndex sourceIdAndParaIndex)
        //{
        //    DataTable resultDT = new DataTable();
        //    bool suc = true;
        //    try
        //    {
        //        //根据厂站ID，通道ID，设备ID获取指定设备的定时记录源
        //        int errorCode = DatalogProvider.Instance.ReadDatalogSources(DBOperationFlag.either, stationID, channelID, deviceId, ref resultDT);
        //        if (errorCode != 0)
        //            suc = false;
        //        else
        //        {
        //            for (int i = 0; i < resultDT.Rows.Count; i++)
        //            {
        //                for (int j = 6; j <= 21; j++)//j=6 <=> paraHandle1
        //                {
        //                    if (resultDT.Rows[i][j] == DBNull.Value || resultDT.Rows[i][j].ToString() == NULL_DATABASE_VALUE_STRING)
        //                        continue;
        //                    int tempParaHandle = -1;
        //                    int.TryParse(resultDT.Rows[i][j].ToString(),out tempParaHandle);
        //                    if (paraHandle != tempParaHandle)
        //                        continue;

        //                    int tempSourceId = -1;
        //                    int.TryParse(resultDT.Rows[i]["sourceID"].ToString(), out tempSourceId);

        //                    sourceIdAndParaIndex.SourceID = tempSourceId;
        //                    sourceIdAndParaIndex.ParaIndex = j - 5; //获取paraIndex
        //                    break;
        //                }
        //            }  
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        m_errorMessage = ex.Message+ex.StackTrace;
        //        DbgTrace.dout(ex.Message + ex.StackTrace);
        //        suc = false;
        //    }
        //    finally
        //    {
        //        if (resultDT != null)
        //            resultDT.Dispose();
        //    }
        //    return suc;
        //}


        ///// <summary>
        ///// 判断是否至少有一行数据
        ///// </summary>
        ///// <param name="stationId"></param>
        ///// <param name="sourceIdAndParaIndex"></param>
        ///// <param name="startTime"></param>
        ///// <param name="endTime1"></param>
        ///// <returns></returns>
        //private bool HasDataMoreThanZero(uint stationId, SourceIDAndParaIndex sourceIdAndParaIndex,    DateTime startTime,DateTime endTime1)
        //{
        //    //通过数据库接口获取该设备的该参数在指定时间的定时记录数据表resultDT//PD_TB_01中的column[3]相对于PD_TB_02中的column[6],输出该sourceID的第column[6]列数据 
        //    DataTable resultDT = new DataTable();
        //    bool suc = false;
        //    try
        //    {
        //        DateTime endTime = endTime1.AddMinutes(1);
        //        int errorCode = DatalogProvider.Instance.ReadDatalogs(DBOperationFlag.either, stationId,
        //            (uint)sourceIdAndParaIndex.SourceID, sourceIdAndParaIndex.ParaIndex,startTime,
        //            endTime, (int)SysConstDefinition.DefaultMaxRowCount, ref resultDT);
        //        DbgTrace.dout("调用数据库接口，查询定时记录。返回的错误字符串是:{0}；返回的错误码的值是：{1}。", DBInterfaceCommonLib.ErrorQuerier.Instance.GetLastErrorString(), errorCode);
        //        DbgTrace.dout("传入数据库接口的入参是：DBOperationFlag = {0} , stationID = {1} , sourceID = {2} , paraIndex = {3}, startTime = {4} , endTime.AddSeconds(1) = {5} , Convert.ToInt32(SysConstDefinition.DefaultMaxRowCount) = {6} 。",
        //            DBOperationFlag.either, stationId,
        //            (uint)sourceIdAndParaIndex.SourceID, sourceIdAndParaIndex.ParaIndex, startTime,
        //            endTime, (int)SysConstDefinition.DefaultMaxRowCount);

        //        if (errorCode != 0)
        //        {
        //            m_errorMessage = ErrorQuerier.Instance.GetLastErrorString();
        //            suc = false;
        //        }
        //        else
        //        {
        //            foreach (DataRow row in resultDT.Rows)
        //            {
        //                if (row[3].ToString() ==NULL_DATABASE_VALUE_STRING ||
        //                    row[3] == DBNull.Value)
        //                    continue;
        //                double realValue ;
        //                if (!double.TryParse(row[3].ToString(), out realValue))
        //                    continue;
        //                DateTime logTime;
        //                if (!DateTime.TryParse(row["LOGTIME"].ToString(), out logTime))
        //                    continue;
        //                if (logTime < startTime)
        //                    continue;
        //                if (logTime > endTime1)
        //                    continue;

        //                //只要有一个，就说明是有数据的
        //                suc =true;
        //                break;
        //            }
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        m_errorMessage = ex.Message+ex.StackTrace;
        //        DbgTrace.dout(ex.Message + ex.StackTrace);
        //        suc = false;
        //    }
        //    finally
        //    {
        //        if (resultDT != null)
        //            resultDT.Dispose();
        //    }
        //    return suc;
        //}
        /// <summary>
        /// 判断是否至少有一行数据
        /// </summary>
        /// <param name="stationId"></param>
        /// <param name="sourceIdAndParaIndex"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime1"></param>
        /// <returns></returns>
        private bool HasDataMoreThanZero(DeviceDataIDDef deviceDataIDDef, DateTime startTime, DateTime endTime1)
        {
            bool suc = false;
            try
            {
                DateTime endTime = endTime1.AddMinutes(1);
                List<DataLogOriDef> tempDT = new List<DataLogOriDef>();
                bool isSucess = DefaultTemplatePublicMethod.ReadDatalogs(startTime, endTime, deviceDataIDDef,ref tempDT);
                if (isSucess)
                {
                    foreach (DataLogOriDef item in tempDT)
                    {
                        if (double.IsNaN(item.DataValue))
                            continue;
                        if (item.LogTime < startTime)
                            continue;
                        if (item.LogTime > endTime1)
                            continue;
                        //只要有一个，就说明是有数据的
                        suc = true;
                        break;
                    }
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
                //if (resultDT != null)
                //    resultDT.Dispose();
            }
            return suc;
        }
        #endregion
    }
}
