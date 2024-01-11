using System;
using System.Globalization;

namespace OfficeReportInterface
{
    ///// <summary>
    ///// 用于将时间转换为字符串
    ///// </summary>
    //public class DateTimeToStringManager
    //{
    //    private const string FormatString = "yyyy-MM-dd HH:mm:ss.fff";
    //    private const string CultureInfoString = "en-US";
    //    private string m_errorString = string.Empty;
    //    /// <summary>
    //    /// 获取上次的错误字符串
    //    /// </summary>
    //    /// <returns></returns>
    //    public string GetLastErrorString()
    //    {
    //        return m_errorString;
    //    }

    //    /// <summary>
    //    /// 传入字符串，得到对应的DateTime
    //    /// </summary>
    //    /// <param name="dateTimeString"></param>
    //    /// <param name="dateValue"></param>
    //    /// <returns></returns>
    //    public bool GetDateTimeFromString(string dateTimeString, out DateTime dateValue)
    //    {
    //        dateValue = new DateTime();
    //        try
    //        {
    //            if (!DateTime.TryParseExact(dateTimeString, FormatString, new CultureInfo(CultureInfoString),
    //                    DateTimeStyles.None, out dateValue))
    //                return false;
    //            return true;
    //        }
    //        catch (System.Exception ex)
    //        {
    //            m_errorString = ex.Message;
    //            return false;
    //        }

    //    }
    //    /// <summary>
    //    /// 获取DateTime对应的字符串
    //    /// </summary>
    //    /// <param name="time"></param>
    //    /// <param name="dateTimeString"></param>
    //    /// <returns></returns>
    //    public bool GetStringFromDateTime(DateTime time, out string dateTimeString)
    //    {
    //        dateTimeString = string.Empty;
    //        try
    //        {
    //            dateTimeString = time.ToString(FormatString, new CultureInfo(CultureInfoString));
    //            return true;
    //        }
    //        catch (System.Exception ex)
    //        {
    //            m_errorString = ex.Message;
    //            return false;
    //        }
    //    }
    //}

    ///// <summary>
    ///// 用于下载TemplateFiles文件到本地，或者从本地上传文件
    ///// </summary>
    //class DownloadManager
    //{

    //}
}
