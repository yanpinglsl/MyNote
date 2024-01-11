using System;
using Microsoft.Win32;

namespace OfficeReportInterface.DefaultReportInterface
{
     public  class CommonlyUsedForQueryCondition
     {

       


        public static DateTime GetStartTime(DateTime dtpStartTimeValue, string comboBoxYear1Text,
      int comboBoxMonth1SelectedIndex, int comboBoxReportTypeSelectedIndex)
        {
            DateTime resultTime = DateTime.Now;
            if (comboBoxReportTypeSelectedIndex == 0)
            {
                resultTime = new DateTime(dtpStartTimeValue.Year, dtpStartTimeValue.Month, dtpStartTimeValue.Day);
            }
            else if (comboBoxReportTypeSelectedIndex == 1)
            {
                resultTime = new DateTime(dtpStartTimeValue.Year, dtpStartTimeValue.Month, dtpStartTimeValue.Day);
                resultTime = GetWeekFirstDay(resultTime);
            }
            else if (comboBoxReportTypeSelectedIndex == 2)
            {
                resultTime = new DateTime(Convert.ToInt32(comboBoxYear1Text), comboBoxMonth1SelectedIndex + 1, 1);
            }
            else
            {
                resultTime = new DateTime(Convert.ToInt32(comboBoxYear1Text), 1, 1);
            }
            return resultTime;
        }

        public static DateTime GetEndTime(int comboBoxReportTypeSelectedIndex, int comboBoxReportPeriodSelectedIndex, DateTime dtpEndTimeValue, string comboBoxYear2Text, int comboBoxMonth2SelectedIndex, DateTime startTime)
        {
            DateTime resultTime = DateTime.Now;
            if (comboBoxReportTypeSelectedIndex == 0)
            {
                resultTime = new DateTime(dtpEndTimeValue.Year, dtpEndTimeValue.Month, dtpEndTimeValue.Day);
            }
            else if (comboBoxReportTypeSelectedIndex == 1)
            {
                resultTime = new DateTime(dtpEndTimeValue.Year, dtpEndTimeValue.Month, dtpEndTimeValue.Day);
                resultTime = GetWeekFirstDay(resultTime);
            }
            else if (comboBoxReportTypeSelectedIndex == 2)
            {
                resultTime = new DateTime(Convert.ToInt32(comboBoxYear2Text), comboBoxMonth2SelectedIndex + 1, 1);
            }
            else
            {
                resultTime = new DateTime(Convert.ToInt32(comboBoxYear2Text), 1, 1);
            }
            if (comboBoxReportPeriodSelectedIndex == 0 || comboBoxReportPeriodSelectedIndex == 1 || comboBoxReportPeriodSelectedIndex == 4)
            {
                resultTime = GetEndTimeByTimeType(startTime, comboBoxReportTypeSelectedIndex);
            }
            return resultTime;
        }
        public static DateTime GetEndTimeByTimeType(DateTime startTime, int timeType)
        {
            if (timeType == 0)
                return startTime.AddDays(1);
            else if (timeType == 1)
                return startTime.AddDays(7);
            else if (timeType == 2)
                return startTime.AddMonths(1);
            else
                return startTime.AddYears(1);
        }
        public static bool GetStartTimeAndEndTimeBySelectedIndex(int SelectedIndex, out DateTime queryStartTime, out DateTime queryEndTime)
        {
            queryStartTime = DateTime.Now;
            queryEndTime = DateTime.Now;
            if (SelectedIndex == 0)
            {
                return false;
            }
            if (SelectedIndex == 2)//today
            {
                queryStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                queryEndTime = queryStartTime.AddDays(1);
                return true;
            }
            if (SelectedIndex == 3)//Yeaterday
            {
                DateTime tempTime = DateTime.Now.AddDays(-1);
                queryStartTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day);
                queryEndTime = queryStartTime.AddDays(1);
                return true;
            }
            if (SelectedIndex == 4)//Since Yeaterday
            {
                DateTime tempTime = DateTime.Now.AddDays(-1);
                queryStartTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day);
                tempTime = DateTime.Now;
                queryEndTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day).AddDays(1);
                return true;
            }
            if (SelectedIndex == 5)//Week To Date
            {
                queryStartTime = GetWeekFirstDay(DateTime.Now);
                queryEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1);
                return true;
            }
            if (SelectedIndex == 6)//Month To Date
            {
                queryStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                queryEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1);
                return true;
            }
            if (SelectedIndex == 7)//Year To Date
            {
                queryStartTime = new DateTime(DateTime.Now.Year, 1, 1);
                queryEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1);
                return true;
            }
            if (SelectedIndex == 9)//Last Week
            {
                DateTime tempTime =GetWeekFirstDay(DateTime.Now);
                queryStartTime = GetWeekFirstDay(DateTime.Now.AddDays(-7));
                queryEndTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day);
                return true;
            }
            if (SelectedIndex == 10)//Last Two Week
            {
                DateTime tempTime = GetWeekFirstDay(DateTime.Now);
                queryStartTime = GetWeekFirstDay(DateTime.Now.AddDays(-14));
                queryEndTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day);
                return true;
            }
            if (SelectedIndex == 11)//Last Month
            {
                DateTime tempTime = DateTime.Now.AddMonths(-1);
                queryStartTime = new DateTime(tempTime.Year, tempTime.Month, 1);
                queryEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                return true;
            }
            if (SelectedIndex == 12)//Last Year
            {
                DateTime tempTime = DateTime.Now.AddYears(-1);
                queryStartTime = new DateTime(tempTime.Year, 1, 1);
                queryEndTime = new DateTime(DateTime.Now.Year, 1, 1);
                return true;
            }
            if (SelectedIndex == 14)//Last 7 days
            {
                DateTime tempTime = DateTime.Now.AddDays(-6);
                queryStartTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day);
                queryEndTime = queryStartTime.AddDays(7);
                return true;
            }
            if (SelectedIndex == 15)//Last 7 full days
            {
                DateTime tempTime = DateTime.Now.AddDays(-7);
                queryStartTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day);
                queryEndTime = queryStartTime.AddDays(7);
                return true;
            }
            if (SelectedIndex == 16)//Last 14 days
            {
                DateTime tempTime = DateTime.Now.AddDays(-13);
                queryStartTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day);
                queryEndTime = queryStartTime.AddDays(14);
                return true;
            }
            if (SelectedIndex == 17)//Last 14 full days
            {
                DateTime tempTime = DateTime.Now.AddDays(-14);
                queryStartTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day);
                queryEndTime = queryStartTime.AddDays(14);
                return true;
            }
            if (SelectedIndex == 18)//Last 30 days
            {
                DateTime tempTime = DateTime.Now.AddDays(-29);
                queryStartTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day);
                queryEndTime = queryStartTime.AddDays(30);
                return true;
            }
            if (SelectedIndex == 19)//Last 30 full days
            {
                DateTime tempTime = DateTime.Now.AddDays(-30);
                queryStartTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day);
                queryEndTime = queryStartTime.AddDays(30);
                return true;
            }
            if (SelectedIndex == 21)//Last 5th to 5th
            {
                DateTime tempTime = DateTime.Now.AddMonths(-1);
                queryStartTime = new DateTime(tempTime.Year, tempTime.Month, 5);
                tempTime = DateTime.Now;
                queryEndTime = new DateTime(tempTime.Year, tempTime.Month, 5);
                return true;
            }
            if (SelectedIndex == 22)//Last 10th to 10th
            {
                DateTime tempTime = DateTime.Now.AddMonths(-1);
                queryStartTime = new DateTime(tempTime.Year, tempTime.Month, 10);
                tempTime = DateTime.Now;
                queryEndTime = new DateTime(tempTime.Year, tempTime.Month, 10);
                return true;
            }
            if (SelectedIndex == 23)//Last 15th to 15th
            {
                DateTime tempTime = DateTime.Now.AddMonths(-1);
                queryStartTime = new DateTime(tempTime.Year, tempTime.Month, 15);
                tempTime = DateTime.Now;
                queryEndTime = new DateTime(tempTime.Year, tempTime.Month, 15);
                return true;
            }
            if (SelectedIndex == 24)//Last 20th to 20th
            {
                DateTime tempTime = DateTime.Now.AddMonths(-1);
                queryStartTime = new DateTime(tempTime.Year, tempTime.Month, 20);
                tempTime = DateTime.Now;
                queryEndTime = new DateTime(tempTime.Year, tempTime.Month, 20);

                return true;
            }
            if (SelectedIndex == 25)//Last 25th to 25th
            {
                DateTime tempTime = DateTime.Now.AddMonths(-1);
                queryStartTime = new DateTime(tempTime.Year, tempTime.Month, 25);
                tempTime = DateTime.Now;
                queryEndTime = new DateTime(tempTime.Year, tempTime.Month, 25);

                return true;
            }

            return false;
        }

         private static int GetFirstDayOffset()
        {
            //周一=0，周二=1，周三=2，周四=3，周五=4，周六=5，周日=6.
            RegistryKey Myreg;
            Myreg = Registry.CurrentUser;
            Myreg = Myreg.CreateSubKey("Control Panel\\International");
            return Convert.ToInt32(Myreg.GetValue("iFirstDayOfWeek", 0));
        }
        public static DateTime GetWeekFirstDay(DateTime resultTime)
        {
            int addDays = GetFirstDayOffset();
            int todayIndex = 0;
            switch (resultTime.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    todayIndex = 0;
                    break;
                case DayOfWeek.Tuesday:
                    todayIndex = 1;
                    break;
                case DayOfWeek.Wednesday:
                    todayIndex = 2;
                    break;
                case DayOfWeek.Thursday:
                    todayIndex = 3;
                    break;
                case DayOfWeek.Friday:
                    todayIndex = 4;
                    break;
                case DayOfWeek.Saturday:
                    todayIndex = 5;
                    break;
                case DayOfWeek.Sunday:
                    todayIndex = 6;
                    break;
            }
            if ((todayIndex - addDays) >= 0)
                resultTime = resultTime.AddDays(addDays - todayIndex);
            else
                resultTime = resultTime.AddDays(-7 - todayIndex + addDays);
            resultTime = new DateTime(resultTime.Year, resultTime.Month, resultTime.Day);
            return resultTime;
        }
     }
}
