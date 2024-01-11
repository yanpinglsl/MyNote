using System.Collections.Generic;

namespace OfficeReportInterface
{
    /// <summary>
    ///原始事件记录管理类
    /// </summary>
    public class EventLogManager
    {
        /// <summary>
        /// 事件管理类唯一单例
        /// </summary>
        public static readonly EventLogManager DataManager = new EventLogManager();
        /// <summary>
        /// 获取记录事件起止时间（并做秒级扩展）
        /// </summary>
        /// <param name="queryResult"></param>
        /// <returns></returns>
        public DateTimeParam GetRecordEventTimeParam(List<EventInformation> queryResult)
        {
            DateTimeParam timeParam = new DateTimeParam(DataFormatManager.GetMinDateTime(),DataFormatManager.GetMinDateTime());
            if (queryResult.Count > 0)
            {
                timeParam.StartTime = queryResult[queryResult.Count - 1].EventTime;
                timeParam.EndTime = queryResult[0].EventTime.AddSeconds(1);//扩大1秒
                EndTimeManager.GetInstance().SetLastEventTime(timeParam.EndTime);
            }
            return timeParam;
        }
    }
}