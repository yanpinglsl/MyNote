using System.Globalization;

namespace OfficeReportInterface.DefaultReportInterface.CommonluUsed
{
    class DateTimeFormatManager
    {
        #region 成员变量
        //这里只能用readonly，不能用const。用const会报错，因为CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern不是常量。这里就体现出了readonly与const的不同
        public readonly string m_shortDatePattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        private readonly string m_longTimePattern = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
        private readonly string m_timeSeparator = CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator;
        private readonly string m_dateSeparator = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator;
        #endregion

        #region 私有方法

        /// <summary>
        /// 获取时分秒毫秒,用于C#的转换
        /// </summary>
        /// <returns></returns>
        private string ChangeTimeFormatWithfff()
        {
            string timePattern = m_longTimePattern;
            if (timePattern.Contains("ss"))
                timePattern = timePattern.Replace("ss", "ss.fff");
            else if (timePattern.Contains("s"))
                timePattern = timePattern.Replace("s", "s.fff");
            return timePattern;
        }
        public  string GetExcelTimeFormat(string formatString)
        {
            string format = formatString;
            format = format.Replace("tt", "AM/PM");
            return format;
        }

        /// <summary>
        /// 获取时分秒毫秒,用于Excel的转换
        /// </summary>
        /// <returns></returns>
        private string ChangeTimeFormatWithfffForExcel()
        {
            string timePattern = m_longTimePattern;

            //转换成Excel用的
            timePattern = GetExcelTimeFormat(timePattern);

            if (timePattern.Contains("ss"))
                timePattern = timePattern.Replace("ss", "ss.000");
            else if (timePattern.Contains("s"))
                timePattern = timePattern.Replace("s", "s.000");
            return timePattern;
        }
        /// <summary>
        /// 获取时分
        /// </summary>
        /// <returns></returns>
        private string GetHourAndSecond()
        {
            string hourAndMinute = m_longTimePattern;
            hourAndMinute = hourAndMinute.Replace(string.Format(@"{0}{1}", m_timeSeparator, "ss"), "");
            hourAndMinute = hourAndMinute.Replace(string.Format(@"{0}{1}", "ss", m_timeSeparator), "");
            hourAndMinute = hourAndMinute.Replace(string.Format(@"{0}{1}", m_timeSeparator, "s"), "");
            hourAndMinute = hourAndMinute.Replace(string.Format(@"{0}{1}", "s", m_timeSeparator), "");
            hourAndMinute = hourAndMinute.Replace(string.Format(@"{0}{1}", m_timeSeparator, "SS"), "");
            hourAndMinute = hourAndMinute.Replace(string.Format(@"{0}{1}", "SS", m_timeSeparator), "");
            hourAndMinute = hourAndMinute.Replace(string.Format(@"{0}{1}", m_timeSeparator, "S"), "");
            hourAndMinute = hourAndMinute.Replace(string.Format(@"{0}{1}", "S", m_timeSeparator), "");
            hourAndMinute = hourAndMinute.Replace("S", "");
            hourAndMinute = hourAndMinute.Replace("s", "");
            return hourAndMinute;
        }
      
        #endregion

        #region 公有方法，不是用于Chart控件
        /// <summary>
        /// 获取 年月日 时分
        /// </summary>
        /// <returns></returns>
        public string GetDateTimeFormatWithutSecond()
        {
            return string.Format(@"{0} {1}", m_shortDatePattern, GetHourAndSecond());
        }
        /// <summary>
        /// 获取年月日 时分秒
        /// </summary>
        /// <returns></returns>
        public string GetDateTime()
        {
            return string.Format(@"{0} {1}", m_shortDatePattern,m_longTimePattern);
        }

        /// <summary>
        /// 获取 年月日 时分秒 毫秒 的格式字符串，不是用于Chart控件
        /// </summary>
        /// <returns></returns>
        public string GetDateTimeFormatNormal()
        {
            return string.Format(@"{0} {1}", m_shortDatePattern, ChangeTimeFormatWithfff());
        }
        /// <summary>
        /// 用于Excel的，年月日 时分秒毫秒
        /// </summary>
        /// <returns></returns>
        public string GetDateTimeFormatWithMiniSecondForExcel()
        {
            return string.Format(@"{0} {1}", m_shortDatePattern, ChangeTimeFormatWithfffForExcel());
        }

        /// <summary>
        /// 获取 年月 的格式字符串 例如：   YYYY/MM  ，不是用于Chart控件
        /// </summary>
        /// <returns></returns>
        public string GetYearAndMonthNormal()
        {
            string yearAndMonth = m_shortDatePattern;
            yearAndMonth = yearAndMonth.Replace(string.Format(@"{0}{1}", "dd", m_dateSeparator), "");
            yearAndMonth = yearAndMonth.Replace(string.Format(@"{0}{1}", "d", m_dateSeparator), "");
            yearAndMonth = yearAndMonth.Replace(string.Format(@"{0}{1}", m_dateSeparator, "dd"), "");
            yearAndMonth = yearAndMonth.Replace(string.Format(@"{0}{1}", m_dateSeparator, "d"), "");
            yearAndMonth = yearAndMonth.Replace("d", "");
            yearAndMonth = yearAndMonth.Replace(string.Format(@"{0}{1}", "DD", m_dateSeparator), "");
            yearAndMonth = yearAndMonth.Replace(string.Format(@"{0}{1}", "D", m_dateSeparator), "");
            yearAndMonth = yearAndMonth.Replace(string.Format(@"{0}{1}", m_dateSeparator, "DD"), "");
            yearAndMonth = yearAndMonth.Replace(string.Format(@"{0}{1}", m_dateSeparator, "D"), "");
            yearAndMonth = yearAndMonth.Replace("D", "");


            return yearAndMonth;

        }
        /// <summary>
        /// 获取 年 的格式字符串 例如：   YYYY  ，不是用于Chart控件
        /// </summary>
        /// <returns></returns>
        public string GetYearNormal()
        {
            string yearAndMonth = GetYearAndMonthNormal();
            yearAndMonth = yearAndMonth.Replace(string.Format(@"{0}{1}", "MM", m_dateSeparator), "");
            yearAndMonth = yearAndMonth.Replace(string.Format(@"{0}{1}", "M", m_dateSeparator), "");
            yearAndMonth = yearAndMonth.Replace(string.Format(@"{0}{1}", m_dateSeparator,"MM"), "");
            yearAndMonth = yearAndMonth.Replace(string.Format(@"{0}{1}",  m_dateSeparator,"M"), "");
            yearAndMonth = yearAndMonth.Replace( "M", "");

            yearAndMonth = yearAndMonth.Replace(string.Format(@"{0}{1}", "mm", m_dateSeparator), "");
            yearAndMonth = yearAndMonth.Replace(string.Format(@"{0}{1}", "m", m_dateSeparator), "");
            yearAndMonth = yearAndMonth.Replace(string.Format(@"{0}{1}", m_dateSeparator, "mm"), "");
            yearAndMonth = yearAndMonth.Replace(string.Format(@"{0}{1}", m_dateSeparator, "m"), "");
            yearAndMonth = yearAndMonth.Replace("m", "");
            return yearAndMonth;
        }
        

     
        #endregion

    }
}
