using System;
using System.Collections.Generic;

namespace OfficeReportInterface.DefaultReportInterface
{
    /// <summary>
    /// 专门用来计算Energy Period Over Period 报表的DateTimeList
    /// </summary>
    public class DateTimeListForEnergyPeriod
    {
        public static bool GetDateTimeList(DateTime DateTimeNow, int comboBoxCompareNumberSelectedIndex, DateTime dtpStartTimeValue, DateTime dtpEndTimeValue, int comboBoxReportTypeSelectedIndex, int comboBoxCompareTypeSelectedIndex, string comboBoxYear2Text, int comboBoxMonth2SelectedIndex, string comboBoxYear1Text, int comboBoxMonth1SelectedIndex, out List<DateTime> dateTimeListTemp, out int compareNumber)
        {
            dateTimeListTemp = new List<DateTime>();
            DateTime tempTime = DateTimeNow;
            tempTime = new DateTime(tempTime.Year, tempTime.Month, tempTime.Day);
            compareNumber = comboBoxCompareNumberSelectedIndex + 1;
            //   this.listBoxDate.Items.Clear();
            //  dateTimeList.Clear();
            //   panelDate.Visible = false;
            //      panelMonthYear.Visible = false;
            //日
            if (comboBoxReportTypeSelectedIndex == 0)

                SetDataTimeControlStateOfDay(tempTime, comboBoxCompareTypeSelectedIndex, ref compareNumber,
                    dtpStartTimeValue, dtpEndTimeValue, out dateTimeListTemp);

            //周
            else if (comboBoxReportTypeSelectedIndex == 1)
                SetDataTimeControlStateOfWeek(tempTime, comboBoxCompareTypeSelectedIndex, ref  compareNumber, dtpStartTimeValue, dtpEndTimeValue, out  dateTimeListTemp);
            //月
            else if (comboBoxReportTypeSelectedIndex == 2)

                SetDataTimeControlStateOfMonth(tempTime, comboBoxCompareTypeSelectedIndex, ref compareNumber,
                    comboBoxYear2Text, comboBoxMonth2SelectedIndex, comboBoxYear1Text, comboBoxMonth1SelectedIndex,
                    out dateTimeListTemp);

            //年
            else if (comboBoxReportTypeSelectedIndex == 3)

                SetDataTimeControlStateOfYear(tempTime, comboBoxCompareTypeSelectedIndex, ref compareNumber,
                    comboBoxYear1Text, comboBoxYear2Text, out dateTimeListTemp);

            dateTimeListTemp.Reverse();
            //   List<string> dateStringList = new List<string>();
            // for (int i = 0; i < listBoxDate.Items.Count; i++)
            //   {
            //      dateStringList.Add(listBoxDate.Items[i].ToString());
            //  }
            //   listBoxDate.Items.Clear();
            //   dateStringList.Reverse();
            //  listBoxDate.Items.AddRange(dateStringList.ToArray());

            return true;
        }

        private static void SetDataTimeControlStateOfMonth(DateTime tempTime, int comboBoxCompareTypeSelectedIndex, ref int compareNumber, string comboBoxYear2Text, int comboBoxMonth2SelectedIndex, string comboBoxYear1Text, int comboBoxMonth1SelectedIndex, out List<DateTime> dateTimeList)
        {
            dateTimeList = new List<DateTime>();
            //    this.labelMonth1.Text = LocalResourceManager.GetInstance().GetString("0376", "Month 1:");
            //  this.labelMonth2.Text = LocalResourceManager.GetInstance().GetString("0374", "Month 2:");
            tempTime = new DateTime(tempTime.Year, tempTime.Month, 1);
            if (comboBoxCompareTypeSelectedIndex == 0)
            {
                for (int i = 0; i <= compareNumber; i++)
                {
                    //     this.listBoxDate.Items.Add(GetMonthNameByTime(tempTime.AddMonths(-i)));
                    dateTimeList.Add(tempTime.AddMonths(-i));
                }
            }
            else if (comboBoxCompareTypeSelectedIndex == 1)
            {
                for (int i = 0; i <= compareNumber; i++)
                {
                    //       this.listBoxDate.Items.Add(GetMonthNameByTime(tempTime.AddYears(-i)));
                    dateTimeList.Add(tempTime.AddYears(-i));
                }
            }
            else if (comboBoxCompareTypeSelectedIndex == 2)
            {
                compareNumber = 1;
                //  panelMonthYear.Visible = true;
                //    panelCompareNumber.Visible = false;
                //   SetMonthVisible(true);

                dateTimeList.Add(new DateTime(Convert.ToInt32(comboBoxYear2Text), comboBoxMonth2SelectedIndex + 1, 1));
                dateTimeList.Add(new DateTime(Convert.ToInt32(comboBoxYear1Text), comboBoxMonth1SelectedIndex + 1, 1));
                //   this.listBoxDate.Items.Add(GetMonthNameByTime(new DateTime(Convert.ToInt32(comboBoxYear2.Text), comboBoxMonth2.SelectedIndex + 1, 1)));
                // this.listBoxDate.Items.Add(GetMonthNameByTime(new DateTime(Convert.ToInt32(comboBoxYear1.Text), comboBoxMonth1.SelectedIndex + 1, 1)));
            }
        }

        private static bool SetDataTimeControlStateOfWeek(DateTime tempTime, int comboBoxCompareTypeSelectedIndex, ref int compareNumber, DateTime dtpStartTimeValue, DateTime dtpEndTimeValue, out List<DateTime> dateTimeList)
        {
            dateTimeList = new List<DateTime>();
            if (comboBoxCompareTypeSelectedIndex == 0)
            {
                for (int i = 0; i <= compareNumber; i++)
                {
                    //    this.listBoxDate.Items.Add(LocalResourceManager.GetInstance().GetString("0449","Week Start:") + GetWeekFirstDay(tempTime).AddDays(-i * 7).ToShortDateString());
                    dateTimeList.Add(GetWeekFirstDay(tempTime).AddDays(-i * 7));
                }
                dateTimeList.Reverse();
                return true;
            }
            else if (comboBoxCompareTypeSelectedIndex == 1)
            {
                for (int i = 0; i <= compareNumber; i++)
                {
                    //      this.listBoxDate.Items.Add(LocalResourceManager.GetInstance().GetString("0449","Week Start:") + GetWeekFirstDay(tempTime.AddYears(-i)).ToShortDateString());
                    dateTimeList.Add(GetWeekFirstDay(tempTime.AddYears(-i)));
                }
                dateTimeList.Reverse();
                return true;
            }
            else if (comboBoxCompareTypeSelectedIndex == 2)
            {
                compareNumber = 1;
                //   panelDate.Visible = true;
                //   panelCompareNumber.Visible = false;
                DateTime startTime = GetWeekFirstDay(dtpStartTimeValue);
                DateTime endTime = GetWeekFirstDay(dtpEndTimeValue);
                dateTimeList.Add(new DateTime(endTime.Year, endTime.Month, endTime.Day));
                dateTimeList.Add(new DateTime(startTime.Year, startTime.Month, startTime.Day));

                //    this.listBoxDate.Items.Add(LocalResourceManager.GetInstance().GetString("0449","Week Start:") + endTime.ToShortDateString());   
                //  this.listBoxDate.Items.Add(LocalResourceManager.GetInstance().GetString("0449","Week Start:") + startTime.ToShortDateString());    
                return true;
            }
            return false;
        }

        private static DateTime GetWeekFirstDay(DateTime resultTime)
        {
            return CommonlyUsedForQueryCondition.GetWeekFirstDay(resultTime);

        }
        private static bool SetDataTimeControlStateOfDay(DateTime tempTime, int comboBoxCompareTypeSelectedIndex, ref int compareNumber, DateTime dtpStartTimeValue, DateTime dtpEndTimeValue, out List<DateTime> dateTimeList)
        {
            dateTimeList = new List<DateTime>();
            if (comboBoxCompareTypeSelectedIndex == 0)
            {
                for (int i = 0; i <= compareNumber; i++)
                {
                    //    this.listBoxDate.Items.Add(tempTime.AddDays(-i).ToShortDateString());
                    dateTimeList.Add(tempTime.AddDays(-i));
                }

                return true;
            }
            else if (comboBoxCompareTypeSelectedIndex == 1)
            {
                for (int i = 0; i <= compareNumber; i++)
                {
                    dateTimeList.Add(tempTime.AddDays(-i * 7));
                    //  this.listBoxDate.Items.Add(tempTime.AddDays(-i * 7).ToShortDateString());
                }

                return true;
            }
            else if (comboBoxCompareTypeSelectedIndex == 2)
            {
                compareNumber = 1;
                //    panelDate.Visible = true;
                //    panelCompareNumber.Visible = false;
                DateTime startTime = dtpStartTimeValue;
                DateTime endTime = dtpEndTimeValue;
                dateTimeList.Add(new DateTime(endTime.Year, endTime.Month, endTime.Day));
                dateTimeList.Add(new DateTime(startTime.Year, startTime.Month, startTime.Day));
                //    this.listBoxDate.Items.Add(new DateTime(endTime.Year, endTime.Month, endTime.Day).ToShortDateString());
                //   this.listBoxDate.Items.Add(new DateTime(startTime.Year, startTime.Month, startTime.Day).ToShortDateString());
                return true;
            }
            return false;
        }


        private static void SetDataTimeControlStateOfYear(DateTime tempTime, int comboBoxCompareTypeSelectedIndex, ref int compareNumber, string comboBoxYear1Text, string comboBoxYear2Text, out List<DateTime> dateTimeList)
        {
            tempTime = new DateTime(tempTime.Year,1,1);
            dateTimeList = new List<DateTime>();

            //      this.labelMonth1.Text = LocalResourceManager.GetInstance().GetString("0379", "Year 1:");
            //       this.labelMonth2.Text = LocalResourceManager.GetInstance().GetString("0377", "Year 2:");
            if (comboBoxCompareTypeSelectedIndex == 0)
            {
                for (int i = 0; i <= compareNumber; i++)
                {
                    //       this.listBoxDate.Items.Add(tempTime.AddYears(-i).Year.ToString());
                    dateTimeList.Add(tempTime.AddYears(-i));
                }
                dateTimeList.Reverse();
            }
            else if (comboBoxCompareTypeSelectedIndex == 1)
            {
                compareNumber = 1;
                //    panelMonthYear.Visible = true;
                //  panelCompareNumber.Visible = false;
                //   SetMonthVisible(false);
                dateTimeList.Add(new DateTime(Convert.ToInt32(comboBoxYear2Text), 1, 1));
                dateTimeList.Add(new DateTime(Convert.ToInt32(comboBoxYear1Text), 1, 1));
                //  this.listBoxDate.Items.Add(new DateTime(Convert.ToInt32(comboBoxYear2Text), 1, 1).Year);
                //  this.listBoxDate.Items.Add(new DateTime(Convert.ToInt32(comboBoxYear1Text), 1, 1).Year);
            }
        }

    }
}
