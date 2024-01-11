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
    ///系统节点管理类
    /// </summary>
    public class SystemNodeManager
    {
        /// <summary>
        /// 全局唯一单例
        /// </summary>
        public static readonly SystemNodeManager DataManager = new SystemNodeManager();


        /// <summary>
        /// 厂站ID与时区ID的映射关系
        /// </summary>
        private readonly Dictionary<uint, string> _stationTimezoneMap = new Dictionary<uint, string>();

        /// <summary>
        /// 初始化加载厂站时区信息
        /// </summary>
        public EMSErrorMsg GetStationTimeZoneInfo()
        {
            EMSErrorMsg resultMsg = new EMSErrorMsg(true);
            try
            {
                DataTable queryResult = new DataTable();
                int errorCode = SetupTableProvider.Instance.ReadSetupNodesByParentNodeTypeID(0,PecsNodeType.PECSSYSCONFIG_NODE, PecsNodeType.PECSSYSCONFIG_NODE, new uint[] { PecsNodeType.PECSCOMMUNI_NODE }, 2, true, ref queryResult);
                if (errorCode != (int)ErrorCode.Success)
                {
                    ErrorInfoManager.Instance.WriteDBInterfaceLog(errorCode, "SetupTableProvider.ReadSetupNodesByNodeTypeID");
                    resultMsg.Success = false;
                    resultMsg.ErrorMsgInstance = "SetupTableProvider.ReadSetupNodesByNodeTypeID";
                    return resultMsg;
                }
                foreach (DataRow dr in queryResult.Rows)
                {
                    uint stationID = Convert.ToUInt32(dr["NodeID"]);
                    byte[] datas = (byte[])dr["Data"];
                    MemoryStream ms = new MemoryStream(datas);
                    BinaryReader binReader = new BinaryReader(ms);
                    int stringLen = binReader.ReadInt32();
                    byte[] buffbytes = binReader.ReadBytes(stringLen);
                    string commuInfo = SysNode.GetStringFromBytes(buffbytes);
                    binReader.Close();
                    ms.Close();

                    string timezoneid = GetTimeZoneIDFromCommuStr(commuInfo);
                    if (!string.IsNullOrWhiteSpace(timezoneid))
                        _stationTimezoneMap[stationID] = timezoneid;
                }
            }
            catch (Exception ex)
            {
                resultMsg.Success = false;
                resultMsg.ErrorMsgInstance = "SystemNodeManager.GetStationTimeZoneInfo";
                resultMsg.ErrorMessage = ex.Message;
            }
            return resultMsg;
        }
        /// <summary>
        /// 查找厂站节点中设置的时区信息(与UTC时间的分钟差)
        /// </summary>
        /// <param name="stationID">厂站ID</param>
        /// <returns></returns>
        public int FindTimeZoneOffsetByStation(uint stationID)
        {
            double timeZone = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalMinutes;

            try
            {
                string timeZoneID;
                this._stationTimezoneMap.TryGetValue(stationID, out timeZoneID);
                if (!string.IsNullOrWhiteSpace(timeZoneID))
                    timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneID).BaseUtcOffset.TotalMinutes;
            }
            catch
            {
                timeZone = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalMinutes;
            }

            return (int)timeZone;
        }

        /// <summary>
        /// 查找设备时区信息(与UTC时间的分钟差)
        /// </summary>
        /// <param name="deviceID">设备ID</param>
        /// <returns></returns>
        public int FindTimeZoneOffsetByDevice(uint deviceID)
        {
            uint stationID = FindStationID(deviceID);
            return FindTimeZoneOffsetByStation(stationID);
        }

        /// <summary>
        /// 从通信节点字符串信息中解析出时区ID
        /// </summary>
        /// <param name="commuInfo"></param>
        /// <returns></returns>
        private string GetTimeZoneIDFromCommuStr(string commuInfo)
        {
            string timezoneid = string.Empty;
            if (!commuInfo.Contains("[TIMEZONE]"))
                return timezoneid;

            string newCommuInfo = commuInfo.Replace("\r\n", "$");
            string[] commList = newCommuInfo.Split('$');
            for (int i = 0; i < commList.Length; i++)
            {
                if (commList[i].Contains("[TIMEZONE]") && i + 1 < commList.Length)
                {
                    string[] tempStres = commList[i + 1].Split('=');
                    if (tempStres.Length == 2)
                        timezoneid = tempStres[1].Trim();
                    break;
                }
            }
            return timezoneid;
        }

        /// <summary>
        /// 根据设备ID查找其所属的厂站ID
        /// </summary>
        /// <param name="deviceID">设备ID</param>
        /// <returns></returns>
        public uint FindStationID(uint deviceID)
        {
            uint resultID = 0;
            SysNode sysnode = PecsNodeManager.PecsNodeInstance.GetDeviceNodeByID(deviceID);
            if (sysnode != null)
                resultID = sysnode.ParentNode.ParentNode.ParentNodeID;
            return resultID;
        }


        /// <summary>
        /// 根据设备ID找到其所属通道ID
        /// </summary>
        /// <param name="deviceID">设备ID</param>
        /// <returns></returns>
        public uint FindChannelID(uint deviceID)
        {
            uint resultID = 0;
            SysNode sysnode = PecsNodeManager.PecsNodeInstance.GetDeviceNodeByID(deviceID);
            if (sysnode != null)
                resultID = sysnode.ParentNodeID;
            return resultID;
        }
    }
}