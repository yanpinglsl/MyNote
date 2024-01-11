using System;
using System.Collections.Generic;
using OfficeReportInterface;
using OfficeReportInterface.DefaultReportInterface;
using OfficeReportInterface.DefaultReportInterface.IntelligentSafety;

namespace OfficeReportInterface.DefaultReportInterface
{
     public class CommonlyUsedFunctions
    {
        /// <summary>
        /// 获取设备节点，整合为数据源标签中的DeviceIDs参数
        /// </summary>
        /// <returns></returns>
         public static string GetParametersStr(List<LogicalDeviceIndex> deviceNodeList, List<DataIDNameTypeDef> dataIDList)
         {
             string paraMemterString = string.Empty;
             if (dataIDList == null)
             {
                 return SafetyDataManager.GetJsonStr(deviceNodeList);
             }
             else
             {
                 for (int i = 0; i < deviceNodeList.Count; i++)
                 {
                     for (int j = 0; j < dataIDList.Count; j++)
                     {
                         paraMemterString += deviceNodeList[i].DeviceID + "," + dataIDList[j].DataID.ToString() + "," + deviceNodeList[i].LogicalIndex + "," + dataIDList[j].DataTypeID + ";";
                     }
                 }
             }
             return paraMemterString;
         }

      

        private static string GetReportExportByDateTime(DateTime time)
        {
            return time.ToString("yyyyMMddHHmmss");
        }

        public static string GetReportExportTime()
        {
            return GetReportExportByDateTime(DateTime.Now);
        }
         /// <summary>
         /// 获取生成的报表名称
         /// </summary>
         /// <param name="fileName"></param>
         /// <returns></returns>
        public static string GetReportFileResultName(string fileName)
        {
            int index = fileName.IndexOf('.');
            string fileDateTimeString = CommonlyUsedFunctions.GetReportExportTime();
            return fileName.Remove(index) + " (" + fileDateTimeString + ")" + fileName.Substring(index, fileName.Length - index);
        }
         /// <summary>
         /// 对文件名称加上时间标字符串
         /// </summary>
         /// <param name="oralFileName"></param>
         /// <param name="fileNameWithTime"></param>
         /// <returns></returns>
         public static bool GetFileNameWithTime(string oralFileName,out string fileNameWithTime)
         {
             fileNameWithTime = string.Empty;
             try
             {
                 if (oralFileName == null)
                     return false;
                 if (oralFileName == string.Empty)
                     return false;
                 int index = oralFileName.LastIndexOf('.');
                 string fileDateTimeString = GetReportExportTime();
                 fileNameWithTime = oralFileName.Remove(index) + " (" + fileDateTimeString + ")" + oralFileName.Substring(index, oralFileName.Length - index);
                 return true;
             }
             catch (System.Exception ex)
             {
                 DbgTrace.dout(ex.Message + ex.StackTrace);
                 return false;
             }
         }

        public static bool GetDeviceTreeNeedCheckBox(uint source)
        {
            switch (source)
            {
                case (uint)RepServFileType.EnergyPeriod:
                    return true;
                case (uint)RepServFileType.MultiUsage:
                    return true;
                case (uint)RepServFileType.EnergyCost:
                    return true;
                case (uint)RepServFileType.HourlyUsage:
                    return true;
                case (uint)RepServFileType.Tabular:
                    return true;
                case (uint)RepServFileType.Trend:
                    return true;

                case (uint)RepServFileType.SingleUsage:
                    return false;
                default:
                    return false;
            }
        }

        public static bool GetMeasureTreeNeedCheckBox(uint source)
        {
            switch (source)
            {
                case (uint)RepServFileType.SingleUsage:
                    return true;
                case (uint)RepServFileType.Trend:
                    return true;
                case (uint)RepServFileType.Tabular:
                    return true;
                case (uint)RepServFileType.HourlyUsage:
                    return true;
                case (uint)RepServFileType.MultiUsage:
                case (uint)RepServFileType.EnergyPeriod:
                    return false;
                default:
                    return false;
            }
        }

        public static bool GetStartTimeAndEndTimeBySelectedIndex(int SelectedIndex, out DateTime queryStartTime, out DateTime queryEndTime)
        {
           return CommonlyUsedForQueryCondition.GetStartTimeAndEndTimeBySelectedIndex(SelectedIndex, out queryStartTime,
                out queryEndTime);
        }
    }
}
