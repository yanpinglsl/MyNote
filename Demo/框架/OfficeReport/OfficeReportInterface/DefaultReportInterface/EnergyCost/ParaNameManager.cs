using System;
using System.Collections.Generic;
using System.Data;
using CET.PecsNodeManage;
using CSharpDBPlugin;

namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
    class ParaNameManager
    {
        /// <summary>
        /// 用来保存已经查询过的，避免重复查询，提升查询效率
        /// </summary>
        private Dictionary<DeviceDataIDDef, string> m_paraNameDic;
        private static ParaNameManager m_instance;

        public static ParaNameManager GetInstance()
        {
            if (m_instance == null)
                m_instance = new ParaNameManager();
            return m_instance;
        }

        private ParaNameManager()
        {
            if (m_paraNameDic == null)
                m_paraNameDic = new Dictionary<DeviceDataIDDef, string>();
        }
        /// <summary>
        /// 获取ParaName
        /// </summary>
        /// <param name="deviceDataIDDef"></param>
        /// <param name="paraName"></param>
        /// <returns></returns>
        public bool GetParaName(DeviceDataIDDef deviceDataIDDef, out string paraName)
        {
            if (m_paraNameDic.TryGetValue(deviceDataIDDef, out paraName))
                return true;
            if (!DoGetParaName(deviceDataIDDef, out paraName))
                return false;
            m_paraNameDic.Add(deviceDataIDDef, paraName);
            return true;
        }

        /// <summary>
        /// 从数据库获取paraName
        /// </summary>
        /// <param name="deviceDataIDDef"></param>
        /// <param name="paraName"></param>
        /// <returns></returns>
        private bool DoGetParaName(DeviceDataIDDef deviceDataIDDef, out string paraName)
        {
            paraName = string.Empty;
            DataTable DT = new DataTable();
            try
            {

                PecsDeviceNode deviceNode = PecsNodeManager.PecsNodeInstance.GetDeviceNodeByID(deviceDataIDDef.DeviceID);
                if (deviceNode == null)
                    return false;

                DataLogPrivateMapProvider.Instance.Read(0,deviceNode.ParentNode.ParentNode.ParentNode.NodeID, deviceNode.ParentNode.NodeID, deviceNode.NodeID, (uint)deviceDataIDDef.LogicalDeviceIndex, (int)ParaTypes.Datalog, ref DT);
                for (int j = 0; j < DT.Rows.Count; j++)
                {
                    uint DataID;
                    if (!uint.TryParse(DT.Rows[j]["DataID"].ToString(), out DataID))
                        continue;
                    if (DataID != deviceDataIDDef.DataID)
                        continue;
                    int dataTypeID;
                    if (!int.TryParse(DT.Rows[j]["dataTypeID"].ToString(), out dataTypeID))
                        continue;
                    if (dataTypeID != deviceDataIDDef.DataTypeID)
                        continue;


                    paraName = DT.Rows[j]["paraName"].ToString();
                    return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
            finally
            {
                if (DT != null)
                    DT.Dispose();

            }

        }

    }
}
