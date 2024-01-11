using System;

namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
    struct TimePair
    {
        public DateTime StartTime;
        public DateTime EndTime;

        public TimePair(DateTime startTime,DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
