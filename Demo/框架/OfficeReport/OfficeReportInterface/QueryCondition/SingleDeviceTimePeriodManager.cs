using System;
using OfficeReportInterface.DefaultReportInterface;

namespace OfficeReportInterface.QueryCondition
{
    public class SingleDeviceTimePeriodManager
    {
        public static bool GetTimeAll(DateTime dtpStartTimeValue,DateTime dtpEndTimeValue, string comboBoxYear1Text,int comboBoxMonth1SelectedIndex , string comboBoxYear2Text,int comboBoxMonth2SelectedIndex , int comboBoxReportTypeSelectedIndex, int comboBoxReportPeriodSelectedIndex, out DateTime startTime, out DateTime endTime)
        {
            startTime = DateTime.MinValue;
            endTime = DateTime.MinValue;
            bool success = true;
            if (comboBoxReportTypeSelectedIndex == 0 && comboBoxReportPeriodSelectedIndex == 4)
            {
                startTime = new DateTime(dtpStartTimeValue.Year, dtpStartTimeValue.Month, dtpStartTimeValue.Day);
                endTime = new DateTime(dtpEndTimeValue.Year, dtpEndTimeValue.Month, dtpEndTimeValue.Day);
            }
            else if (comboBoxReportTypeSelectedIndex == 0 && comboBoxReportPeriodSelectedIndex ==5)
            {
                startTime = new DateTime(dtpStartTimeValue.Year, dtpStartTimeValue.Month, dtpStartTimeValue.Day);
                endTime = new DateTime(dtpEndTimeValue.Year, dtpEndTimeValue.Month, dtpEndTimeValue.Day);
            }
            else if (comboBoxReportTypeSelectedIndex == 1 && comboBoxReportPeriodSelectedIndex == 4)
            {
                startTime = new DateTime(dtpStartTimeValue.Year, dtpStartTimeValue.Month, dtpStartTimeValue.Day);
                startTime = CommonlyUsedForQueryCondition.GetWeekFirstDay(startTime);
                endTime = new DateTime(dtpEndTimeValue.Year, dtpEndTimeValue.Month, dtpEndTimeValue.Day);
                endTime = CommonlyUsedForQueryCondition.GetWeekFirstDay(endTime);
            }
            else if (comboBoxReportTypeSelectedIndex == 1 && comboBoxReportPeriodSelectedIndex == 5)
            {
                startTime = new DateTime(dtpStartTimeValue.Year, dtpStartTimeValue.Month, dtpStartTimeValue.Day);
                startTime = CommonlyUsedForQueryCondition.GetWeekFirstDay(startTime);
                endTime = new DateTime(dtpEndTimeValue.Year, dtpEndTimeValue.Month, dtpEndTimeValue.Day);
                endTime = CommonlyUsedForQueryCondition.GetWeekFirstDay(endTime);
            }
            else if (comboBoxReportTypeSelectedIndex == 2 && comboBoxReportPeriodSelectedIndex == 4)
            {
                startTime = new DateTime(Convert.ToInt32(comboBoxYear1Text), comboBoxMonth1SelectedIndex + 1, 1);
                endTime = startTime.AddMonths(1);
            }
            else if (comboBoxReportTypeSelectedIndex == 2 && comboBoxReportPeriodSelectedIndex == 5)
            {
                startTime = new DateTime(Convert.ToInt32(comboBoxYear1Text), comboBoxMonth1SelectedIndex + 1, 1);
                endTime = new DateTime(Convert.ToInt32(comboBoxYear2Text), comboBoxMonth2SelectedIndex + 1, 1);
            }
            else if (comboBoxReportTypeSelectedIndex == 3 && comboBoxReportPeriodSelectedIndex == 4)
            {
                startTime = new DateTime(Convert.ToInt32(comboBoxYear1Text), 1, 1);
                endTime = startTime.AddYears(1);
            }
            else if (comboBoxReportTypeSelectedIndex == 3 && comboBoxReportPeriodSelectedIndex == 5)
            {
                startTime = new DateTime(Convert.ToInt32(comboBoxYear1Text), 1, 1);
                endTime = new DateTime(Convert.ToInt32(comboBoxYear2Text), 1, 1);
            }
            else if (comboBoxReportTypeSelectedIndex >= 0 && comboBoxReportTypeSelectedIndex <= 3 &&
                     comboBoxReportPeriodSelectedIndex >= 0 && comboBoxReportPeriodSelectedIndex <= 5)
            {
                GetTime(comboBoxReportTypeSelectedIndex, comboBoxReportPeriodSelectedIndex, out startTime,
                    out endTime);
            }
            else
            {
                success = false;
            }
            return success;
        }


        private static bool GetTime(int comboBoxReportTypeSelectedIndex, int comboBoxReportPeriodSelectedIndex, out DateTime startTime, out DateTime endTime)
        {
            startTime = DateTime.MinValue;
            endTime = DateTime.MinValue;
            int compareNumber = int.MinValue;
            DateTime dtpStartTimeValue=DateTime.MinValue;
            DateTime dtpEndTimeValue = DateTime.MinValue;
            if (!GetTimeByIndex(comboBoxReportTypeSelectedIndex, comboBoxReportPeriodSelectedIndex, out compareNumber,
                out dtpStartTimeValue, out dtpEndTimeValue))
                return false;
            if (!GetStartTimeByDateTime(dtpStartTimeValue, comboBoxReportTypeSelectedIndex, out startTime))
                return false;
            if (
                !GetEndTimeByDateTime(dtpEndTimeValue, comboBoxReportTypeSelectedIndex, compareNumber, startTime,
                    out endTime))
                return false;
            return true;
        }


        private static bool GetStartTimeByDateTime(DateTime dtpStartTimeValue, int comboBoxReportTypeSelectedIndex, out DateTime startTime)
        {
            return GetTimeByReportType(dtpStartTimeValue, comboBoxReportTypeSelectedIndex, out startTime);
        }

        private static bool GetEndTimeByDateTime(DateTime dtpEndTimeValue, int comboBoxReportTypeSelectedIndex,int compareNumber,DateTime startTime, out DateTime endTime)
        {
            if (!GetTimeByReportType(dtpEndTimeValue, comboBoxReportTypeSelectedIndex, out endTime))
                return false;
            if (compareNumber ==1)
            {
               endTime= CommonlyUsedForQueryCondition.GetEndTimeByTimeType(startTime, comboBoxReportTypeSelectedIndex);
            }
            return true;
        }
        
        

        private static bool GetTimeByReportType(DateTime dtpStartTimeValue, int comboBoxReportTypeSelectedIndex, out DateTime startTime)
        {
            startTime = DateTime.MinValue;
            bool success = true;
            if (comboBoxReportTypeSelectedIndex == 0)
            {
                startTime = new DateTime(dtpStartTimeValue.Year, dtpStartTimeValue.Month, dtpStartTimeValue.Day);
            }
            else if (comboBoxReportTypeSelectedIndex == 1)
            {
                startTime = new DateTime(dtpStartTimeValue.Year, dtpStartTimeValue.Month, dtpStartTimeValue.Day);
                if(dtpStartTimeValue!=DateTime.MinValue)
                   startTime = CommonlyUsedForQueryCondition.GetWeekFirstDay(startTime);
            }
            else if (comboBoxReportTypeSelectedIndex == 2)
            {
                startTime = new DateTime(dtpStartTimeValue.Year, dtpStartTimeValue.Month, 1);
            }
            else if (comboBoxReportTypeSelectedIndex == 3)
            {
                startTime = new DateTime(dtpStartTimeValue.Year, 1, 1);
            }
            else
            {
                success = false;
            }
            return success;
        }

        private static bool GetTimeByIndex(int reportTypeIndex ,int comboBoxReportPeriodSelectedIndex, out int compareNumber,out DateTime dtpStartTimeValue,out DateTime dtpEndTimeValue)
        {
            dtpStartTimeValue = DateTime.MinValue;
            dtpEndTimeValue = DateTime.MinValue;
            compareNumber = Int32.MinValue;
            if (reportTypeIndex == 0)
            {
                return GetTimeByIndex0(comboBoxReportPeriodSelectedIndex, out compareNumber, out dtpStartTimeValue,
                    out dtpEndTimeValue);
            }
            if (reportTypeIndex == 1)
            {
                return GetTimeByIndex1(comboBoxReportPeriodSelectedIndex, out compareNumber, out dtpStartTimeValue,
                    out dtpEndTimeValue);
            }
            if (reportTypeIndex == 2)
            {
                return GetTimeByIndex2(comboBoxReportPeriodSelectedIndex, out compareNumber, out dtpStartTimeValue,
                    out dtpEndTimeValue);
            }
            if (reportTypeIndex == 3)
            {
                return GetTimeByIndex3(comboBoxReportPeriodSelectedIndex, out compareNumber, out dtpStartTimeValue,
                out dtpEndTimeValue);
            }
            return false;
        }
        #region  年

        private static DateTime GetThisYear()
        {
            return DateTime.Now;
        }

        private static DateTime GetLastYear()
        {
            return GetThisYear().AddYears(-1);
        }
        private static DateTime GetLastLastYear()
        {
            return GetLastYear().AddYears(-1);
        }


        private static bool GetTimeByIndex3(int comboBoxReportPeriodSelectedIndex, out int compareNumber, out DateTime dtpStartTimeValue, out DateTime dtpEndTimeValue)
        {
            bool success = true;
            dtpStartTimeValue = DateTime.MinValue;
            dtpEndTimeValue = DateTime.MinValue;
            compareNumber = Int32.MinValue;
            //本年
            if (comboBoxReportPeriodSelectedIndex == 0)
            {
                compareNumber = 1;
                dtpStartTimeValue = GetThisYear();
            }//上年
            else if (comboBoxReportPeriodSelectedIndex == 1)
            {
                compareNumber = 1;
                dtpStartTimeValue = GetLastYear();
            }//本年和上年
            else if (comboBoxReportPeriodSelectedIndex == 2)
            {
                compareNumber = 2;
                dtpStartTimeValue = GetLastYear();
                dtpEndTimeValue = GetThisYear();
            }//完整的两年
            else if (comboBoxReportPeriodSelectedIndex == 3)
            {
                compareNumber = 2;
                dtpStartTimeValue = GetLastLastYear();
                dtpEndTimeValue = GetLastYear();
            }//特定年
            else if (comboBoxReportPeriodSelectedIndex == 4)
            {
                compareNumber = 1;
            }//特定两年
            else if (comboBoxReportPeriodSelectedIndex == 5)
            {
                compareNumber = 2;
            }
            else
            {
                success = false;
            }
            return success;
        }
        #endregion
   
        #region 月

        private static DateTime GetThisMonth()
        {
            DateTime now = DateTime.Today;
            return now.AddDays(-now.Day + 1);
        }

        private static DateTime GetLastMonth()
        {
            return GetThisMonth().AddMonths(-1);
        }

        private static DateTime GetLastLastMonth()
        {
            return GetLastMonth().AddMonths(-1);
        }
        private static bool GetTimeByIndex2(int comboBoxReportPeriodSelectedIndex, out int compareNumber, out DateTime dtpStartTimeValue, out DateTime dtpEndTimeValue)
        {
            bool success = true;
            dtpStartTimeValue = DateTime.MinValue;
            dtpEndTimeValue = DateTime.MinValue;
            compareNumber = Int32.MinValue;
            //本月
            if (comboBoxReportPeriodSelectedIndex == 0)
            {
                compareNumber = 1;
                dtpStartTimeValue = GetThisMonth();
            }//上月
            else if (comboBoxReportPeriodSelectedIndex == 1)
            {
                compareNumber = 1;
                dtpStartTimeValue = GetLastMonth();
            }//本月和上月
            else if (comboBoxReportPeriodSelectedIndex == 2)
            {
                compareNumber = 2;
                dtpStartTimeValue = GetLastMonth();
                dtpEndTimeValue = GetThisMonth();
            }//完整的两月
            else if (comboBoxReportPeriodSelectedIndex == 3)
            {
                compareNumber = 2;
                dtpStartTimeValue = GetLastLastMonth();
                dtpEndTimeValue = GetLastMonth();
            }//特定月
            else if (comboBoxReportPeriodSelectedIndex == 4)
            {
                compareNumber = 1;
            }//特定两月
            else if (comboBoxReportPeriodSelectedIndex == 5)
            {
                compareNumber = 2;
            }
            else
            {
                success = false;
            }
            return success;
        }

        #endregion

        #region 周

        private static bool GetTimeByIndex1(int comboBoxReportPeriodSelectedIndex, out int compareNumber, out DateTime dtpStartTimeValue, out DateTime dtpEndTimeValue)
        {
            bool success = true;
            dtpStartTimeValue = DateTime.MinValue;
            dtpEndTimeValue = DateTime.MinValue;
            compareNumber = Int32.MinValue;
            //本周
            if (comboBoxReportPeriodSelectedIndex == 0)
            {
                compareNumber = 1;
                dtpStartTimeValue = DateTime.Now;
            }//上周
            else if (comboBoxReportPeriodSelectedIndex == 1)
            {
                compareNumber = 1;
                dtpStartTimeValue = DateTime.Now.AddDays(-7);
            }//今天和昨天
            else if (comboBoxReportPeriodSelectedIndex == 2)
            {
                compareNumber = 2;
                dtpStartTimeValue = DateTime.Now.AddDays(-7);
                dtpEndTimeValue = DateTime.Now;
            }//完整的两周
            else if (comboBoxReportPeriodSelectedIndex == 3)
            {
                compareNumber = 2;
                dtpStartTimeValue = DateTime.Now.AddDays(-14);
                dtpEndTimeValue = DateTime.Now.AddDays(-7);
            }//特定周
            else if (comboBoxReportPeriodSelectedIndex == 4)
            {
                compareNumber = 1;
                dtpStartTimeValue = DateTime.Now;
            }//特定两周
            else if (comboBoxReportPeriodSelectedIndex == 5)
            {
                compareNumber = 2;
                dtpStartTimeValue = DateTime.Now.AddDays(-7);
                dtpEndTimeValue = DateTime.Now;
            }
            else
            {
                success = false;
            }
            return success;
        }
        #endregion

        #region 日
        private static bool GetTimeByIndex0(int comboBoxReportPeriodSelectedIndex, out int compareNumber, out DateTime dtpStartTimeValue, out DateTime dtpEndTimeValue)
        {
            bool success = true;
            dtpStartTimeValue = DateTime.MinValue;
            dtpEndTimeValue = DateTime.MinValue;
            compareNumber = Int32.MinValue;
            //今天
            if (comboBoxReportPeriodSelectedIndex == 0)
            {
                compareNumber = 1;
                dtpStartTimeValue = DateTime.Now;
            }//昨天
            else if (comboBoxReportPeriodSelectedIndex == 1)
            {
                compareNumber = 1;
                dtpStartTimeValue = DateTime.Now.AddDays(-1);
            }//今天和昨天
            else if (comboBoxReportPeriodSelectedIndex == 2)
            {
                compareNumber = 2;
                dtpStartTimeValue = DateTime.Now.AddDays(-1);
                dtpEndTimeValue = DateTime.Now;
            }//完整的两天
            else if (comboBoxReportPeriodSelectedIndex == 3)
            {
                compareNumber = 2;
                dtpStartTimeValue = DateTime.Now.AddDays(-2);
                dtpEndTimeValue = DateTime.Now.AddDays(-1); ;
            }//特定天
            else if (comboBoxReportPeriodSelectedIndex == 4)
            {
                compareNumber = 1;
                dtpStartTimeValue = DateTime.Now;
            }//特定两天
            else if (comboBoxReportPeriodSelectedIndex == 5)
            {
                compareNumber = 2;
                dtpStartTimeValue = DateTime.Now.AddDays(-1);
                dtpEndTimeValue = DateTime.Now;
            }
            else
            {
                success = false;
            }
            return success;
        }
        #endregion
    }
}
