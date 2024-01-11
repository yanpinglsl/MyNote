using System;
using System.Globalization;

namespace OfficeReportInterface.DefaultReportInterface.CommonluUsed
{
     public  class DataManager
    {
        /// <summary>
        ///用于C#的DateTime转string， 时间格式，年月日，时分
        /// </summary>
        public static readonly string TimeFormatWithoutSecond = DateTimeFormatWithHourAndMinute();

        /// <summary>
        ///用于C#的DateTime转string， 时间格式，年月日，时分秒
        /// </summary>
        public static readonly string TimeFormatWithSecond = DateTimeFormat();

        /// <summary>
        /// 用于Excel的单元格设置，时间格式，年月日，时分秒毫秒
        /// </summary>
        public static readonly string TimeFormatWithMinisecondForExcel = GetDateTimeFormatWithMiniSecondForExcel();

        /// <summary>
        /// 用于C#的时间格式，年月日，时分秒毫秒
        /// </summary>
        public static readonly string TimeFormatWithMinisecondForC = GetDateTimeFormatWithMiniSecondForC();
        /// <summary>
        /// 用于Excel的单元格设置，数据的格式,保留2位小数
        /// </summary>
        public static readonly string DataFormat = "#" + NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator + "##0" + NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator + "00";
        /// <summary>
        /// 用于Excel的单元格设置，数据的格式,保留3位小数
        /// </summary>
        public static readonly string DataFormat3 = "#" + NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator + "##0" + NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator + "000";

        public static string GetStartTimeSting(DateTime startTime)
        {
            string result = string.Format("{0} {1}", LocalResourceManager.GetInstance().GetString("0102", "Start Time:"), startTime.ToString(TimeFormatWithSecond))  ;
            return result;

        }

        public static string GetEndTimeString(DateTime endTime)
        {
            string result = string.Format("{0} {1}", LocalResourceManager.GetInstance().GetString("0103", "End Time:"),endTime.ToString(TimeFormatWithSecond))  ;
            return result;
        }
        /// <summary>
        /// 获取表格部分的时间格式，即，年月日 时分  ，适用于EnergyCost，Tabular，Hourly Usage。用于给Excel设置时间格式。
        /// </summary>
        /// <returns></returns>
        public static string GetTableTimeFormatForExcel()
        {
            string format = TimeFormatWithSecond;
            format = GetExcelTimeFormat(format);
            return format;
        }
        /// <summary>
        /// 用在C#中的
        /// </summary>
        /// <returns></returns>
        public static string GetWarningMessageTimeFormat()
        {
            string format = TimeFormatWithSecond;
      
            return format;
        }
        /// <summary>
        /// 获取警告信息部分的时间格式,用在Excel中
        /// </summary>
        /// <returns></returns>
        public static string GetWarningMessageTimeFormatForExcel()
        {
            string format = TimeFormatWithSecond;
            format = GetExcelTimeFormat(format);
            return format;
        }
           /// <summary>
        /// 用于Excel的，年月日 时分秒毫秒
        /// </summary>
        /// <returns></returns>
           private static string GetDateTimeFormatWithMiniSecondForExcel()
        {
               DateTimeFormatManager dateTimeFormatManager = new DateTimeFormatManager();
               return dateTimeFormatManager.GetDateTimeFormatWithMiniSecondForExcel();
        }

           /// <summary>
           /// 用于C#的，年月日 时分秒毫秒
           /// </summary>
           /// <returns></returns>
           private static string GetDateTimeFormatWithMiniSecondForC()
           {
               DateTimeFormatManager dateTimeFormatManager = new DateTimeFormatManager();
               return dateTimeFormatManager.GetDateTimeFormatNormal();
           }

        private static string GetExcelTimeFormat(string formatString)
        {
            DateTimeFormatManager dateTimeFormatManager = new DateTimeFormatManager();
            return dateTimeFormatManager.GetExcelTimeFormat(formatString);
        }

        /// <summary>
        /// 年月日 时分秒
        /// </summary>
        /// <returns></returns>
        private static string DateTimeFormat()
        {
            DateTimeFormatManager dateTimeFormatManager = new DateTimeFormatManager();
            return dateTimeFormatManager.GetDateTime();
        }
        /// <summary>
        /// 年月日 时分
        /// </summary>
        /// <returns></returns>
        private static string DateTimeFormatWithHourAndMinute()
        {
            DateTimeFormatManager dateTimeFormatManager = new DateTimeFormatManager();
            return dateTimeFormatManager.GetDateTimeFormatWithutSecond();
        }
    
    }
}
