using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using CSharpDBPlugin;
using DBInterfaceCommonLib;
using ErrorCode = DBInterfaceCommonLib.ErrorCode;
using ErrorQuerier = DBInterfaceCommonLib.ErrorQuerier;

namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
    /// <summary>
    /// 新的TOU方案的解析
    /// </summary>
    public  class TOURateParser
    {
        private string m_errorMessage = string.Empty;
        /// <summary>
        /// 获取错误字符串
        /// </summary>
        /// <returns></returns>
        public string GetLastErrorString()
        {
            return m_errorMessage;
        }
        /// <summary>
        /// 通过费率ID得到对应的费率方案
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newTOUProfile"></param>
        /// <returns></returns>
        public bool GetTOURateByID(uint id, out NewTOUProfile newTOUProfile)
        {
            newTOUProfile =new NewTOUProfile();
            List<NewTOUProfile> newTOUProfileList;
            if (!GetTouRate(out newTOUProfileList))
                return false;
            foreach (var item in newTOUProfileList)
            {
                if (item.scheduleID == id)
                {
                    newTOUProfile = item;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取新的TOU方案
        /// </summary>
        /// <param name="newTOUProfileList"></param>
        /// <returns></returns>
        public bool GetTouRate(out List<NewTOUProfile> newTOUProfileList)
        {
            newTOUProfileList = new List<NewTOUProfile>();
            DataTable resultDT = new DataTable();
            bool isSucceed = false;
            try
            {
                
                uint[] nodeTypes = new uint[] { TOUTariffNodeType.ENERGY_SCHEDULE };
                int resultCode = SetupTableProvider.Instance.ReadSetupNodesByParentNodeTypeID(0,TOUTariffNodeType.ENERGY_SETUP, TOUTariffNodeType.ENERGY_SETUP, nodeTypes, 0, true, ref resultDT);
                if (resultCode != (int) ErrorCode.Success)
                {
                    m_errorMessage = ErrorQuerier.Instance.GetLastErrorString();
                    DbgTrace.dout("{0}{1}{2}", "TOURateParser.GetTouRate(out List<NewTOUProfile> newTOUProfileList) ， int resultCode =", resultCode ,
                        m_errorMessage);
                    isSucceed = false;
                }
                else
                {
                    foreach (DataRow row in resultDT.Rows)
                    {
                        NewTOUProfile touProfile = new NewTOUProfile();
                        uint nodeType;
                        if (!uint.TryParse(row["NodeType"].ToString(), out nodeType))
                            continue;
                        uint nodeID;
                        if (!uint.TryParse(row["NodeID"].ToString(), out nodeID))
                            continue;
                        string nodeName = row["NodeName"].ToString();
                        touProfile.name = nodeName;
                        touProfile.scheduleID = nodeID;
                        if (!ParseSchedule(ref touProfile, nodeType, nodeID))
                            continue;
                        if (touProfile.dayProfileList.Count == 0)
                            continue;
                        if (touProfile.tariffProfileList.Count == 0)
                            continue;
                        if (touProfile.yearProfileList.Count == 0)
                            continue;
                        newTOUProfileList.Add(touProfile);
                    }

                    isSucceed = true;
                }
              
            }
            catch (System.Exception ex)
            {
                m_errorMessage = ex.Message;
                DbgTrace.dout("{0}{1}", "TOURateParser.GetTouRate(out List<NewTOUProfile> newTOUProfileList) catch (System.Exception ex): ", ex.Message);
                isSucceed = false;
            }
            finally
            {
                if(resultDT!=null)
                   resultDT.Dispose();
            }
            return isSucceed;
        }


        private bool ParseSchedule(ref NewTOUProfile touProfile, uint parentNodeType, uint parentNodeID)
        {
            DataTable resultDT = new DataTable();
            bool isSucceed = true;
            try
            {
                int resultCode = SetupTableProvider.Instance.ReadSetupNodesByParentNodeTypeID(0,parentNodeType, parentNodeID, null, 0, true, ref resultDT);
                if (resultCode != (int)ErrorCode.Success)
                {
                    m_errorMessage = ErrorQuerier.Instance.GetLastErrorString();
                    DbgTrace.dout("{0}{1}  {2}", " TOURateParser.ParseSchedule(NewTOUProfile touProfile, uint parentNodeType, uint parentNodeID) invoke  SetupTableProvider.Instance.ReadSetupNodesByParentNodeTypeID(parentNodeType, parentNodeID, null, 0, true, ref resultDT) return errorcode =",
                     resultCode,   m_errorMessage);
                    isSucceed = false;
                }
                else
                {
                    foreach (DataRow row in resultDT.Rows)
                    {
                         byte[] nodeData = null;
                        if (!Convert.IsDBNull(row["Data"]))
                            nodeData = (byte[])row["Data"];
                        uint nodeType;
                        if (row["NodeType"] == null)
                            continue;
                        if(!uint.TryParse(row["NodeType"].ToString(),out nodeType))
                            continue;
                        uint nodeID ;
                        if (row["NodeID"] == null)
                            continue;
                        if (!uint.TryParse(row["NodeID"].ToString(), out nodeID))
                            continue;
                        if (row["NodeName"] == null)
                            continue;
                        string nodeName = row["NodeName"].ToString();

                        MemoryStream mStream = nodeData != null ? new MemoryStream(nodeData) : new MemoryStream();
                        switch (nodeType)
                        {
                            case TOUTariffNodeType.ENERGY_CALENDERVIEW:
                                EnergyCalenderView calender = new EnergyCalenderView(nodeName);
                                calender.LoadFromStream(mStream);
                                touProfile.yearProfileList.Add(calender);
                                break;
                            case TOUTariffNodeType.TARIFF_NODE:
                                TOUTariffNode tariffNode = new TOUTariffNode(nodeID, nodeName);
                                tariffNode.LoadFromStream(mStream);
                                touProfile.tariffProfileList.Add(tariffNode);
                                break;
                            case TOUTariffNodeType.ENERGY_DAILYPROFILE:
                                DayProfileStruct dayProfile = new DayProfileStruct();
                                dayProfile.periodIndex = Convert.ToInt32(nodeID);
                                dayProfile.periodName = nodeName;
                                dayProfile.LoadFromStream(mStream);
                                touProfile.dayProfileList.Add(dayProfile);
                                break;
                        }
                    }
                    isSucceed = true;
                }
            }
            catch (System.Exception ex)
            {
                m_errorMessage = ex.Message;
                DbgTrace.dout("{0}{1}", "TOURateParser.ParseSchedule(ref NewTOUProfile touProfile, uint parentNodeType, uint parentNodeID) catch (System.Exception ex): ", ex.Message);
                isSucceed = false;
            }
            finally
            {
                if (resultDT != null)
                    resultDT.Dispose();
            }
            return isSucceed;
        }


    }
}
