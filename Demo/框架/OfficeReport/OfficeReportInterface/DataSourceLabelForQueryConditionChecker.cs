using System.Collections.Generic;
using OfficeReportInterface.DefaultReportInterface;

namespace OfficeReportInterface
{
   public class DataSourceLabelForQueryConditionChecker
    {
        public static bool GetValue(string[] dataSourceStr, int j, int index, out string result)
        {
            result = string.Empty;
            try
            {
                result = dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index);
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        private static void GetArrayFromString(string entireDSLString, out string[] dataSourceStr)
        {
             dataSourceStr = entireDSLString.Split('[', ']');
        }

        /// <summary>
        /// 是否是查询条件，而且是有效的查询条件
        /// </summary>
        /// <param name="entireDSLString"></param>
        /// <returns></returns>
        public static bool CheckQueryCondition(string entireDSLString)
        {
            string[] dataSourceStr;
            GetArrayFromString(entireDSLString, out dataSourceStr);
            Dictionary<string, string> datasourceLabelDic;
            if (!GetDataSourceLabelDic(dataSourceStr, out datasourceLabelDic))
                return false;
            return CheckDataSourceLabel(datasourceLabelDic);
        }

        private static bool GetDataSourceLabelDic(string[] dataSourceStr, out Dictionary<string, string> datasourceLabelDic)
        {
            datasourceLabelDic = new Dictionary<string, string>();
            try
            {
                for (int j = 0; j < dataSourceStr.Length; j++)
                {
                    int index = dataSourceStr[j].IndexOf('=') + 1;
                    if (index < 2)
                        continue;
                    string parmDescription = dataSourceStr[j].Substring(0, index - 1).ToUpper();
                    string tempName;
                    if (!GetValue(dataSourceStr, j, index, out tempName))
                        continue;
                    datasourceLabelDic.Add(parmDescription, tempName);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }
        /// <summary>
        /// 专门为QueryCondition类型的字符串做的数据完整性检查
        /// </summary>
        /// <param name="datasourceLabelDic"></param>
        /// <returns></returns>
        private static bool CheckDataSourceLabel(Dictionary<string, string> datasourceLabelDic)
        {
            CommonlyUsedString commonlyUsedString =CommonlyUsedString.GetInstance();
            uint source;
            if (!commonlyUsedString.CheckSource(datasourceLabelDic, out source))
                return false;
            if((uint)RepServFileType.Safety==source) 
                return true;
            bool isQueryCondition;
            if (!commonlyUsedString.CheckIsQueryCondition(datasourceLabelDic, out isQueryCondition))
                return false;
            if (!isQueryCondition)
                return false;
            switch (source)
            {
                case (uint)RepServFileType.EnergyCost:
                    if (!commonlyUsedString.CheckDeviceIDs(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckIsIncludeWarning(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.ChecktouProfileScheduleId(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckSelectedStartedTime(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckSelectedEndTime(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckTimePeriodSelectedIndex(datasourceLabelDic))
                        return false;
                   return true;


                case (uint)RepServFileType.EnergyPeriod:
                    if (!commonlyUsedString.CheckPeriodType(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckCompareNumber(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckDeviceIDs(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckIsIncludeWarning(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckcomboBoxCompareNumberSelectedIndex(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckdtpStartTimeValue(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckDtpEndTimeValue(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckcomboBoxReportTypeSelectedIndex(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckcomboBoxCompareTypeSelectedIndex(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckcomboBoxYear2Text(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckcomboBoxMonth2SelectedIndex(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckcomboBoxYear1Text(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckcomboBoxMonth1SelectedIndex(datasourceLabelDic))
                        return false;
                 
                    return true;

                case (uint)RepServFileType.EventHistory:
                    if (!commonlyUsedString.CheckDeviceIDs(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckIsIncludeWarning(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckSelectedStartedTime(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckSelectedEndTime(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckTimePeriodSelectedIndex(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckintervalType(datasourceLabelDic))
                        return false;
                    return true;
                case (uint)RepServFileType.HourlyUsage:
                    if (!commonlyUsedString.CheckDeviceIDs(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckIsIncludeWarning(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckSelectedStartedTime(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckSelectedEndTime(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckIsIncludeAverage(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckIsIncludeTotal(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckPeriodType(datasourceLabelDic))
                        return false;
                    return true;
                case (uint)RepServFileType.LoadProfile:
                case (uint)RepServFileType.PowerQuality:
                case (uint)RepServFileType.PowerQualityEventsOnly:
                    if (!commonlyUsedString.CheckDeviceIDs(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckIsIncludeWarning(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckintervalType(datasourceLabelDic))
                        return false;
                         if (!commonlyUsedString.CheckisDemand(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckisItic(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckSelectedStartedTime(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckSelectedEndTime(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckTimePeriodSelectedIndex(datasourceLabelDic))
                        return false;
                    return true;

                case (uint)RepServFileType.Tabular:
                    if (!commonlyUsedString.CheckDeviceIDs(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckIsIncludeWarning(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckSelectedStartedTime(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckSelectedEndTime(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckTimePeriodSelectedIndex(datasourceLabelDic))
                        return false;
                    return true;

                case (uint)RepServFileType.Trend:
                       if (!commonlyUsedString.CheckDeviceIDs(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckIsIncludeWarning(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckIsIncludeDataTable(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckPeriodType(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckSelectedStartedTime(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckSelectedEndTime(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckTimePeriodSelectedIndex(datasourceLabelDic))
                        return false;
                    return true;

                case (uint)RepServFileType.MultiUsage:
                case (uint)RepServFileType.SingleUsage:
                    if (!commonlyUsedString.CheckDeviceIDs(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckIsIncludeWarning(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckCompareNumber(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckPeriodType(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckdtpStartTimeValue(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckcomboBoxYear1Text(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckcomboBoxMonth1SelectedIndex(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckcomboBoxReportTypeSelectedIndex(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckComboBoxReportPeriodSelectedIndex(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckDtpEndTimeValue(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckcomboBoxYear2Text(datasourceLabelDic))
                        return false;
                    if (!commonlyUsedString.CheckcomboBoxMonth2SelectedIndex(datasourceLabelDic))
                        return false;
                    return true;

                case (uint)RepServFileType.Safety:
                    return true;
            }

            return false;
        }


    }
}
