using System;
using System.Collections.Generic;

namespace OfficeReportInterface.DefaultReportInterface.CommonluUsed
{
    class WarningInforManager
    {
        private static WarningInforManager m_instance;
        public static WarningInforManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new WarningInforManager();
                return m_instance;
            }
        }

        private Dictionary<WarningKind, string> m_inforDic;

        private WarningInforManager()
        {
            m_inforDic = new Dictionary<WarningKind, string>();
            AddItem(WarningKind.DataIsNull, LocalResourceManager.GetInstance().GetString("0058", "data is Null!"));
            AddItem(WarningKind.SourceIdNotExist,
                LocalResourceManager.GetInstance().GetString("0058", "data is Null!"));
            string messageFormat=  LocalResourceManager.GetInstance().GetString("0059", "Data is over {0} rows, only show first {0} rows.");
            string message=string.Format(messageFormat,DefaultTemplatePublicMethod.MAX_COUNT);
            AddItem(WarningKind.DataOver10000Rows,
             message);
            AddItem(WarningKind.DataNullStartFromSomeTime,
                LocalResourceManager.GetInstance().GetString("0564", "There is missing data starting from {0}!")); 

            AddItem(WarningKind.DataIntervalChanged,
               LocalResourceManager.GetInstance().GetString("0572", "There may be multiple recording interval changes starting from {0}!"));
         //   AddItem(WarningKind.DataIdForKwhChanged, LocalResourceManager.GetInstance().GetString("0575", "Changed to {0}."));
            AddItem(WarningKind.DataForTariffIndexEmpty,
                LocalResourceManager.GetInstance().GetString("0577", "There is missing data starting from {0} for {1}."));
            AddItem(WarningKind.MapNotExist,
             LocalResourceManager.GetInstance().GetString("0002", "data map does not exist!"));
        }

        private void AddItem(WarningKind warningKind, string infor)
        {
            if (!m_inforDic.ContainsKey(warningKind))
                m_inforDic.Add(warningKind, infor);
        }


        private string GetSpliterDot()
        {
            return ": ";
        }

        public string GetWarningMessage(WarningKind warningKind)
        {
            string name;
            if (!m_inforDic.TryGetValue(warningKind, out name))
                return string.Empty;
            return name;
        }
        private string GetMessageWithSplitDot(string seriesName, string warningMessage)
        {
            string result = string.Format("{0}{1}{2}", seriesName, GetSpliterDot(), warningMessage);
            return result;
        }

        /// <summary>
        /// 获取例如“图例名： 映射方案不存在！”
        /// </summary>
        /// <param name="warningKind"></param>
        /// <param name="deviceDataID"></param>
        /// <returns></returns>
        public string GetWarningMessage(WarningKind warningKind, DeviceDataIDDef deviceDataID,uint source)
        {
            string seriesName = SeriesNameManager.GetSeriesName(deviceDataID,source);
            string warningMessage = GetWarningMessage(warningKind);
     
            return GetMessageWithSplitDot( seriesName,  warningMessage);
        }
        /// <summary>
        /// 获取例如“图例名： 映射方案不存在！”
        /// </summary>
        /// <param name="deviceDataID"></param>
        /// <param name="warningMessage"></param>
        /// <returns></returns>
        public string GetWarningMessage(DeviceDataIDDef deviceDataID, string warningMessage,uint source)
        {
            string seriesName = SeriesNameManager.GetSeriesName(deviceDataID,source);
            return GetMessageWithSplitDot(seriesName, warningMessage);
        }

      

        /// <summary>
        /// 从某时刻的数据是空
        /// </summary>
        /// <param name="warningKind"></param>
        /// <param name="deviceDataID"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public string GetWarningMessage(WarningKind warningKind, DeviceDataIDDef deviceDataID, DateTime time,uint source)
        {
            string seriesName = SeriesNameManager.GetSeriesName(deviceDataID,source);
            string warningMessageContents = GetWarningMessage(warningKind);
            string warningMessage = string.Format(warningMessageContents,time.ToString(DataManager.GetWarningMessageTimeFormat()) );
            //if (warningKind == WarningKind.DataIntervalChanged)
            //{
            //    return warningMessage;
            //}
            return GetMessageWithSplitDot(seriesName, warningMessage);
        }

        /// <summary>
        /// //提示kwh的dataId被替换了
        /// </summary>
        /// <param name="warningKind"></param>
        /// <param name="deviceDataID"></param>
        /// <param name="deviceDataIDNew"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public string GetWarningMessage(WarningKind warningKind, DeviceDataIDDef deviceDataID, DeviceDataIDDef deviceDataIDNew, uint source)
        {
            string seriesName = SeriesNameManager.GetSeriesName(deviceDataID, source);
            string seriesNameNew = SeriesNameManager.GetSeriesName(deviceDataIDNew, source);
            string warningMessageContents = GetWarningMessage(warningKind);
            string warningMessage = string.Format(warningMessageContents, seriesNameNew);
      
            return GetMessageWithSplitDot(seriesName, warningMessage);
        }

        /// <summary>
        /// 对于某个费率，从某时刻开始的费率段的数据是空
        /// </summary>
        /// <param name="warningKind"></param>
        /// <param name="deviceDataID"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public string GetWarningMessage(WarningKind warningKind, DeviceDataIDDef deviceDataID, DateTime time,string tariffIndexName, uint source)
        {
            if (warningKind == WarningKind.DataForTariffIndexEmpty)
            {
                string seriesName = SeriesNameManager.GetSeriesName(deviceDataID, source);
                string warningMessageContents = GetWarningMessage(warningKind);
                string warningMessage = string.Format(warningMessageContents, time.ToString(DataManager.GetWarningMessageTimeFormat()), tariffIndexName);

                return GetMessageWithSplitDot(seriesName, warningMessage);
            }
            return string.Empty;
        }

    }
}
