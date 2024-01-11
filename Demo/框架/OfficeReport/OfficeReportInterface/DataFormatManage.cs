using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace OfficeReportInterface
{

    /// <summary>
    /// 全局使用的数据格式化管理类，单例模式
    /// 根据指定参数创建指定格式类型的格式化管理类，执行相应的格式化处理
    /// </summary>
    public partial class DataFormatManager
    {
        #region 常用的数据转换
        /// <summary>
        /// 自定义的区域数字设置对象
        /// </summary>
        private static NumberFormatInfo definedProvider = NumberFormatInfo.CurrentInfo.Clone() as NumberFormatInfo;

        /// <summary>
        /// 存储所有时间格式的统一转换格式
        /// </summary>
        public static readonly string UniversalDateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";


        /// <summary>
        /// 保证线程安全的锁对象
        /// </summary>
        private static object lockHelper = new object();
        /// <summary>
        /// double数据(已经转换成string)用逗号分隔显示
        /// </summary>
        /// <param name="oriValue">已经转换成string的double值</param>
        /// <returns>带有逗号的字符串</returns>
        public static string GetSplitDoubleStr(string oriValue)
        {
            //----------------------如果是负数，先去掉字符串一开始的负号---------------------------
            var index = oriValue.IndexOf("-", System.StringComparison.Ordinal);
            if (index == 0)
                oriValue = oriValue.Replace("-", "");
            //--------------------每隔三位，加上逗号-----------------------------------------------------------------
            string resultStr = string.Empty;
            string[] stres = oriValue.Split('.');
            if (stres.Length > 0)
            {
                resultStr = stres[0];
                int oriLenth = resultStr.Length;
                int oriIndex = oriLenth % 3;
                if (oriIndex == 0)
                    oriIndex = 3;
                for (int i = 0; i < (oriLenth - 1) / 3; i++)
                {
                    if (oriIndex < resultStr.Length)
                        resultStr = resultStr.Insert(oriIndex, ",");
                    oriIndex = oriIndex + 4;
                }
            }

            if (stres.Length == 2)
                resultStr += "." + stres[1];
            //----------------加上负号，如果有负号的话-------------------------------------------------------------------------------
            if (index == 0)
                resultStr = "-" + resultStr;
            //-----------------返回结果-----------------------------------------------------------------------------
            return resultStr;
        }
        /// <summary>
        /// double数据用逗号分隔显示
        /// </summary>
        /// <param name="oriValue">原始的double数据</param>
        /// <returns></returns>
        public static string GetSplitDouble(double oriValue)
        {
            string resultStr = string.Empty;

            string[] stres = oriValue.ToString().Split('.');
            if (stres.Length > 0)
            {
                resultStr = stres[0];
                int oriLenth = resultStr.Length;
                int oriIndex = oriLenth % 3;
                if (oriIndex == 0)
                    oriIndex = 3;
                for (int i = 0; i < (oriLenth - 1) / 3; i++)
                {
                    if (oriIndex < resultStr.Length)
                        resultStr = resultStr.Insert(oriIndex, ",");
                    oriIndex = oriIndex + 4;
                }
            }

            if (stres.Length == 2)
                resultStr += "." + stres[1];

            return resultStr;
        }
        /// <summary>
        /// 返回指定时间参数的毫秒数(从1970年1月1日开始算起)
        /// </summary>
        /// <param name="currentTime">当前时间</param>
        /// <returns>时间差毫秒数</returns>
        public static double GetUnixMilliseconds(DateTime currentTime)
        {
            DateTime baseTime = new DateTime(1970, 1, 1);
            DateTime universalTime = currentTime.ToUniversalTime();

            TimeSpan timeInterval = currentTime.ToUniversalTime() - baseTime;
            return timeInterval.TotalMilliseconds;
        }

        /// <summary>
        /// 将从1970年1月1日开始算起的毫秒数，转换成本地实际时间
        /// </summary>
        /// <param name="currentTime">时间差毫秒数</param>
        /// <returns>本地时间</returns>
        public static DateTime GetLocalTimeFromMilliseconds(double currentTime)
        {
            DateTime baseTime = new DateTime(1970, 1, 1);
            DateTime dt = baseTime.AddMilliseconds(currentTime);
            return dt.ToLocalTime();
        }

        /// <summary>
        /// 获取标准时间字符串格式
        /// </summary>
        /// <param name="oriTime">当前时间</param>
        /// <returns>时间字符串</returns>
        public static string GetFormatTimeString(DateTime oriTime)
        {
            if (oriTime == oriTime.Date)
                return oriTime.ToString("yyyy-MM-dd");

            if (oriTime.Millisecond > 0)
                return oriTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            else
                return oriTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 获取服务器本地时区（分钟）
        /// </summary>
        /// <returns></returns>
        public static int GetServerLocalTimeZone()
        {
            TimeSpan sp = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
            return (int)sp.TotalMinutes;
        }

        /// <summary>
        /// 返回指定的最大时间，3000-01-01
        /// </summary>
        /// <returns></returns>
        public static DateTime GetMaxDateTime()
        {
            return new DateTime(3000, 1, 1);
        }

        /// <summary>
        /// 返回指定的最小时间，1900-01-01
        /// </summary>
        /// <returns></returns>
        public static DateTime GetMinDateTime()
        {
            return new DateTime(1900, 1, 1);
        }

        /// <summary>
        /// 返回指定最小时间格式化字符串，1900-01-01
        /// </summary>
        /// <returns></returns>
        public static string GetMinDateTimeStr()
        {
            return GetFormatTimeString(GetMinDateTime());
        }
        /// <summary>
        /// 使用指定的小数位格式化处理当前的 double 数据
        /// </summary>
        /// <param name="oriValue">原始数据值</param>
        /// <param name="digiNum">指定小数位</param>
        /// <returns>结果值</returns>
        public static double GetFormattedDoubleByDigits(double oriValue, int digiNum)
        {
            double result = oriValue;

            if (!double.IsNaN(oriValue))
            {
                //设置小数位
                definedProvider.NumberDecimalDigits = digiNum;
                result = Convert.ToDouble(oriValue.ToString("F", definedProvider));
            }

            return result;
        }

        /// <summary>
        /// 从指定的字符串中解析 uint 类型的数据对象列表，使用指定的分隔标识
        /// </summary>
        /// <param name="oriStr">原始字符串</param>
        /// <param name="seperator">分隔符</param>
        /// <returns>结果列表</returns>
        public static List<uint> ParseUIntList(string oriStr, string seperator)
        {
            //将字符数组中的每一个字符串转化为指定类型
            List<uint> resultList = new List<uint>();

            //如果原始字符串为空，则返回空
            if (string.IsNullOrEmpty(oriStr) || string.IsNullOrEmpty(seperator))
            {
                return resultList;
            }

            //使用指定分隔符获取字符串数组
            string[] resultStrs = oriStr.Split(seperator.ToCharArray());
            foreach (string resultstr in resultStrs)
            {
                uint result = 0;

                //将转换成功的加入至结果列表中
                if (uint.TryParse(resultstr, out result))
                {
                    resultList.Add(result);
                }
            }

            return resultList;
        }

        /// <summary>
        /// 从指定的字符串中解析int类型的数据对象列表，使用指定的分隔标识
        /// </summary>
        /// <param name="oriStr">原始字符串</param>
        /// <param name="seperator">分隔符</param>
        /// <returns>解析后的整型数组</returns>
        public static List<int> ParseIntList(string oriStr, string seperator)
        {
            //将字符数组中的每一个字符串转化为指定类型
            List<int> resultList = new List<int>();

            //如果原始字符串为空，则返回空
            if (string.IsNullOrEmpty(oriStr) || string.IsNullOrEmpty(seperator))
            {
                return resultList;
            }

            //使用指定分隔符获取字符串数组
            string[] resultStrs = oriStr.Split(seperator.ToCharArray());
            foreach (string resultstr in resultStrs)
            {
                int result = 0;
                //将转换成功的加入至结果列表中
                if (int.TryParse(resultstr, out result))
                {
                    resultList.Add(result);
                }
            }

            return resultList;
        }

        /// <summary>
        /// 从指定的字符串中解析string类型的数据对象列表，使用指定的分隔标识
        /// </summary>
        /// <param name="oriStr">被解析字符串</param>
        /// <param name="seperator">分隔符</param>
        /// <returns>解析后的字符列表</returns>
        public static List<string> ParseStringList(string oriStr, string seperator)
        {
            //将字符数组中的每一个字符串转化为指定类型
            List<string> resultList = new List<string>();

            //如果原始字符串为空，则返回空
            if (string.IsNullOrEmpty(oriStr) || string.IsNullOrEmpty(seperator))
            {
                return resultList;
            }

            //使用指定分隔符获取字符串数组
            string[] resultStrs = oriStr.Split(seperator.ToCharArray());
            foreach (string resultstr in resultStrs)
            {
                if (!string.IsNullOrEmpty(resultstr))
                {
                    resultList.Add(resultstr);
                }
            }

            return resultList;
        }

        /// <summary>
        /// 从指定的字符串中解析string类型的数据对象列表，使用指定的分隔标识
        /// </summary>
        /// <param name="oriStr"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static List<string> ParseFeatureStringList(string oriStr, string seperator)
        {
            //将字符数组中的每一个字符串转化为指定类型
            List<string> resultList = new List<string>();

            //如果原始字符串为空，则返回空
            if (String.IsNullOrEmpty(oriStr) || String.IsNullOrEmpty(seperator))
                return resultList;

            //使用指定分隔符获取字符串数组
            string[] resultStrs = oriStr.Split(seperator.ToCharArray());
            foreach (string resultstr in resultStrs)
                resultList.Add(resultstr);

            return resultList;
        }

        /// <summary>
        /// 从指定的字符串解析转换为DateTime类型对象
        /// </summary>
        /// <param name="dateTimeStr">时间字符串</param>
        /// <returns>时间对象</returns>
        public static DateTime ParseDateTimeFromStr(string dateTimeStr)
        {
            //默认设置为当天时刻
            DateTime resultTime = DateTime.Today;

            //尝试进行转换，成功则返回，失败则返回最小时间标识
            if (DateTime.TryParse(dateTimeStr, out resultTime))
            {
                return resultTime;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 重载函数，默认设置小数位为2
        /// </summary>
        /// <param name="oriValue">原始值</param>
        /// <returns>格式化后的值</returns>
        public static double GetFormattedDoubleByDigits(double oriValue)
        {
            return GetFormattedDoubleByDigits(oriValue, 2);
        }
        #endregion

        #region 增加的Json数据格式转换
 


        /// <summary>
        /// 格式化对象实例字典
        /// </summary>
        private static Dictionary<FormatType, IEEMDataFormat> DataFormatSet = new Dictionary<FormatType, IEEMDataFormat>();


       


        /// <summary>
        /// 获取全局数据格式化实例对象
        /// </summary>
        /// <returns></returns>
        public static IEEMDataFormat Create(FormatType formatType)
        {
            IEEMDataFormat dataFormatInstance = null;

            //如果该类型实例已经创建，则直接返回该实例即可
            if (DataFormatSet.TryGetValue(formatType, out dataFormatInstance))
                return dataFormatInstance;
            else
            {
                //线程安全
                lock (lockHelper)
                {
                    if (DataFormatSet.TryGetValue(formatType, out dataFormatInstance))
                        return dataFormatInstance;
                    else
                    {
                        switch (formatType)
                        {
                            case FormatType.JsonType:
                                dataFormatInstance = new JsonDataFormatManager();
                                break;
                            case FormatType.XmlType:
                                dataFormatInstance = new XmlDataFormatManager();
                                break;
                        }

                        DataFormatSet.Add(formatType, dataFormatInstance);
                    }
                }

                return dataFormatInstance;
            }
        }
        #endregion
      
    }
    /// <summary>
    /// 标识格式化类型
    /// </summary>
    public enum FormatType
    {
        JsonType = 1,
        XmlType = 2
    }
    /// <summary>
    /// JSon格式的返回值对象结构
    /// </summary>
    public struct JsonHeadDef
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success;

        /// <summary>
        /// 结果集行数
        /// </summary>
        public int totalCounts;

        /// <summary>
        /// 结果集字符串
        /// </summary>
        public object results;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="result"></param>
        /// <param name="recordNum"></param>
        /// <param name="resultstr"></param>
        public JsonHeadDef(bool result, int recordNum, object resultstr)
        {
            this.success = result;
            this.totalCounts = recordNum;
            this.results = resultstr;
        }
    }

    /// <summary>
    /// 专用于实时数据查询的格式封装结构，增加了用于返回最晚实时数据时间的属性
    /// </summary>
    public struct JsonRealTimeHeadDef
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success;

        /// <summary>
        /// 结果集行数
        /// </summary>
        public int totalCounts;

        /// <summary>
        /// 实时数据更新时间
        /// </summary>
        public DateTime updateTime;

        /// <summary>
        /// 结果集字符串
        /// </summary>
        public object results;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="result"></param>
        /// <param name="recordNum"></param>
        /// <param name="resultstr"></param>
        public JsonRealTimeHeadDef(bool result, int recordNum, object resultstr, DateTime realTime)
        {
            this.success = result;
            this.totalCounts = recordNum;
            this.updateTime = realTime;
            this.results = resultstr;
        }
    }

    /// <summary>
    /// Json格式数据的格式化管理类，用于将内部数据结构格式化为Json格式
    /// </summary>
    public class JsonDataFormatManager : IEEMDataFormat
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public JsonDataFormatManager()
        {

        }

        #region IEEMDataFormat 成员
        /// <summary>
        /// 将指定的数据对象序列化为Json格式字符串
        /// </summary>
        /// <param name="targetObj">待序列化对象</param>
        /// <returns></returns>
        public string SerializeObject(object targetObj)
        {
            //直接调用Json.Net进行转换处理
            string jsonResult = JsonConvert.SerializeObject(targetObj, new JavaScriptDateTimeConverter());

            return jsonResult;
        }


        /// <summary>
        /// 将指定的数据对象序列化为Json格式的字符串，添加指定的数据头
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="resultCount"></param>
        /// <returns></returns>
        public string SerializeObjectWithHeader(object targetObj, int resultCount)
        {

            JsonHeadDef jsonDef = new JsonHeadDef(true, resultCount, targetObj);

            string jsonResult = JsonConvert.SerializeObject(jsonDef, new JavaScriptDateTimeConverter());

            return jsonResult;
        }

        public string SerializeRealTimeWithHeader(object targetObj, int resultCount, DateTime realDataTime)
        {
            JsonRealTimeHeadDef jsonDef = new JsonRealTimeHeadDef(true, resultCount, targetObj, realDataTime);

            string jsonResult = JsonConvert.SerializeObject(jsonDef, new JavaScriptDateTimeConverter());

            return jsonResult;
        }


        /// <summary>
        /// 将指定格式的json格式字符串转换为相应的类型
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="inputStr">Json字符串</param>
        /// <returns>结果对象</returns>
        public T DeserializeObject<T>(string inputStr)
        {
            T resultList = default(T);

            try
            {
                resultList = JsonConvert.DeserializeObject<T>(inputStr);
            }
            catch
            {

            }

            return resultList;
        }

        #endregion
    }
    /// <summary>
    /// EEMSys数据传输格式化接口
    /// </summary>
    public interface IEEMDataFormat
    {
        /// <summary>
        /// 格式化指定的对象为Json格式
        /// </summary>
        /// <param name="targetObj">待序列化对象</param>
        /// <returns>Json格式字符串</returns>
        string SerializeObject(object targetObj);


        /// <summary>
        /// 将指定的数据对象序列化为Json格式的字符串，添加指定的数据头
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="resultCount"></param>
        /// <returns></returns>
        string SerializeObjectWithHeader(object targetObj, int resultCount);


        /// <summary>
        /// 专用预序列化实时数据查询结果为Json格式的字符串，添加指定的数据头
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="resultCount"></param>
        /// <param name="realDataTime"></param>
        /// <returns></returns>
        string SerializeRealTimeWithHeader(object targetObj, int resultCount, DateTime realDataTime);


        /// <summary>
        /// 将指定格式的json格式字符串转换为相应的类型
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="inputStr">输入格式字符串</param>
        /// <returns>结果对象</returns>
        T DeserializeObject<T>(string inputStr);
    }
    /// <summary>
    /// Xml格式数据的格式化管理类，用于将内部数据结构格式化为Xml格式
    /// </summary>
    public class XmlDataFormatManager : IEEMDataFormat
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public XmlDataFormatManager()
        {


        }

        #region IEEMDataFormat 成员
        /// <summary>
        /// 序列化指定对象为Xml格式字符串
        /// </summary>
        /// <param name="targetObj"></param>
        /// <returns></returns>
        public string SerializeObject(object targetObj)
        {
            return String.Empty;
        }


        public string SerializeObjectWithHeader(object targetObj, int resultCount)
        {
            return String.Empty;
        }


        public string SerializeRealTimeWithHeader(object targetObj, int resultCount, DateTime realDataTime)
        {
            return String.Empty;
        }

        public T DeserializeObject<T>(string inputStr)
        {
            return default(T);
        }

        #endregion

    }


}