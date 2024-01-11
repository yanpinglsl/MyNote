using System.Collections.Generic;

namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
    /// <summary>
    /// 日费率方案结果
    /// </summary>
    public class TariffPeriodTime
    {
        public int tariffIndex;
        public List<DateTimePair> periodTimeList;
        public TariffPeriodTime()
        {
            periodTimeList = new List<DateTimePair>();
        }
    }
}
