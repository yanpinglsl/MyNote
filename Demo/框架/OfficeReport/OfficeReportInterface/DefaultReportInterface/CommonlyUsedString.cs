using System;
using System.Collections.Generic;
using System.Text;

namespace OfficeReportInterface.DefaultReportInterface
{
    public class CommonlyUsedString
    {
        private static CommonlyUsedString m_CommonlyUsedString=null;
        public static CommonlyUsedString GetInstance()
        {
            if (m_CommonlyUsedString==null)
            {
                m_CommonlyUsedString = new CommonlyUsedString();
            }
            return m_CommonlyUsedString;
        }

        private CommonlyUsedString()
        {
            
        }

        public readonly string PERIOD_TYPE = "PeriodType";
        public readonly string IS_INCLUDE_WARNING = "isIncludeWarning";
        public readonly string SELECTED_STARTED_TIME = "SelectedStartTime";
        public readonly string SELECTED_END_TIME = "SelectedEndTime";
        public readonly string COMPARE_NUMBER = "compareNumber";
        public readonly string DEVICE_IDS = "deviceIds";

        public readonly string IS_INCLUDE_AVERAGE = "IsIncludeAverage";

        public readonly string IS_INCLUDE_TOTAL = "IsIncludeTotal";
        public readonly string IS_INCLUDE_DATA_TABLE = "IsIncludeDataTable";

        public readonly string TIME_PERIOD_SELECTED_INDEX = "TimePeriodSelectedIndex";

        public readonly string TOU_PROFILE_SCHEDULE_ID = "touProfileScheduleID";

        public readonly string IS_QUERY_CONDITION = "IsQueryCondition";

        public readonly string SOURCE = "Source";

        public readonly string NAME = "Name";

        public readonly string KWH_SELECTEDINDEX = "kWh_SelectedIndex";
        public readonly string KVARH_SELECTEDINDEX = "kvarh_SelectedIndex";
        public readonly string KVAH_SELECTEDINDEX = "kVAh_SelectedIndex";

        public readonly string KW_SELECTEDINDEX = "kW_SelectedIndex";
        public readonly string KVAR_SELECTEDINDEX = "kvar_SelectedIndex";
        public readonly string KVA_SELECTEDINDEX = "kVA_SelectedIndex";

        public string GetKWH_SELECTEDINDEXString(int index)
        {
            return GetEqualString(KWH_SELECTEDINDEX, index);
        }
        public string GetKVARH_SELECTEDINDEXString(int index)
        {
            return GetEqualString(KVARH_SELECTEDINDEX, index);
        }
        public string GetKVAH_SELECTEDINDEXString(int index)
        {
            return GetEqualString(KVAH_SELECTEDINDEX, index);
        }

        public string GetKW_SELECTEDINDEXString(int index)
        {
            return GetEqualString(KW_SELECTEDINDEX, index);
        }
        public string GetKVAR_SELECTEDINDEXString(int index)
        {
            return GetEqualString(KVAR_SELECTEDINDEX, index);
        }
        public string GetKVA_SELECTEDINDEXString(int index)
        {
            return GetEqualString(KVA_SELECTEDINDEX, index);
        }



        public string GetIsIncludeWarningString(bool isIncludeWarningBool)
        {
            return GetEqualString(IS_INCLUDE_WARNING, isIncludeWarningBool);
        }

        public bool CheckIsIncludeDataTable(Dictionary<string, string> datasourceLabelDic)
        {
            return CheckBool(datasourceLabelDic, IS_INCLUDE_DATA_TABLE);
        }

        public bool CheckIsIncludeWarning(Dictionary<string, string> datasourceLabelDic)
        {
            return CheckBool(datasourceLabelDic, IS_INCLUDE_WARNING);
        }

        private string GetEqualString(object a, object b)
        {
            return string.Format(@"[{0}={1}]", a, b);
        }

        public string GetPeriodTypeString(int periodType)
        {
            return GetEqualString(PERIOD_TYPE, periodType);
        }

        public bool CheckPeriodType(Dictionary<string, string> datasourceLabelDic)
        {
            return CheckInt(datasourceLabelDic, PERIOD_TYPE);
        }

        private bool CheckInt(Dictionary<string, string> datasourceLabelDic, string keySTring)
        {
            keySTring = keySTring.ToUpper();
            string temp;
            if (!datasourceLabelDic.TryGetValue(keySTring, out temp))
                return false;
            int result;
            if (!int.TryParse(temp, out result))
                return false;
            return true;
        }

        private bool CheckUInt(Dictionary<string, string> datasourceLabelDic, string keySTring)
        {
            uint result;
            return CheckUIntWithResult(datasourceLabelDic, keySTring, out result);
        }

        private bool CheckUIntWithResult(Dictionary<string, string> datasourceLabelDic, string keySTring,
            out uint resultValue)
        {
            keySTring = keySTring.ToUpper();
            resultValue = uint.MinValue;
            string temp;
            if (!datasourceLabelDic.TryGetValue(keySTring, out temp))
                return false;

            if (!uint.TryParse(temp, out resultValue))
                return false;
            return true;
        }


        public string GetDeviceIDsString(string deviceIDs)
        {
            return GetEqualString(DEVICE_IDS, deviceIDs);
        }

        public bool CheckDeviceIDs(Dictionary<string, string> datasourceLabelDic)
        {
            return CheckString(datasourceLabelDic, DEVICE_IDS);
        }

        private bool CheckString(Dictionary<string, string> datasourceLabelDic, string keyString)
        {
            keyString = keyString.ToUpper();
            string temp;
            if (!datasourceLabelDic.TryGetValue(keyString, out temp))
                return false;
            if (temp == string.Empty)
                return false;
            return true;
        }

        public string GetIsIncludeAverageString(bool isIncludeAverage)
        {
            return GetEqualString(IS_INCLUDE_AVERAGE, isIncludeAverage);
        }

        public bool CheckIsIncludeAverage(Dictionary<string, string> datasourceLabelDic)
        {
            return CheckBool(datasourceLabelDic, IS_INCLUDE_AVERAGE);
        }

        public string GetIsIncludeTotalString(string isIncludeTotal)
        {
            return GetEqualString(IS_INCLUDE_TOTAL, isIncludeTotal);
        }

        public bool CheckIsIncludeTotal(Dictionary<string, string> datasourceLabelDic)
        {
            return CheckBool(datasourceLabelDic, IS_INCLUDE_TOTAL);
        }

        private bool CheckBool(Dictionary<string, string> datasourceLabelDic, string keyString)
        {
            bool result;
            return CheckBoolWithResult(datasourceLabelDic, keyString, out result);
        }

        public bool CheckBoolWithResult(Dictionary<string, string> datasourceLabelDic, string keyString, out bool result)
        {
            keyString = keyString.ToUpper();
            result = false;
            string temp;
            if (!datasourceLabelDic.TryGetValue(keyString, out temp))
                return false;

            if (!bool.TryParse(temp, out result))
                return false;
            return true;
        }

        public string GetSelectedStartTimeString(DateTime startTime)
        {
            return GetEqualString(SELECTED_STARTED_TIME, startTime);
        }

        public bool CheckSelectedStartedTime(Dictionary<string, string> datasourceLabelDic)
        {
            return CheckDateTime(datasourceLabelDic, SELECTED_STARTED_TIME);
        }

        private bool CheckDateTime(Dictionary<string, string> datasourceLabelDic, string keyString)
        {
            keyString = keyString.ToUpper();
            string temp;
            if (!datasourceLabelDic.TryGetValue(keyString, out temp))
                return false;
            DateTime result;
            if (!DateTime.TryParse(temp, out result))
                return false;
            return true;
        }

        public string GetSelectedEndTimeString(DateTime endTime)
        {
            return GetEqualString(SELECTED_END_TIME, endTime);
        }

        public bool CheckSelectedEndTime(Dictionary<string, string> datasourceLabelDic)
        {
            return CheckDateTime(datasourceLabelDic, SELECTED_END_TIME);
        }

        public string GetTimePeriodSelectedIndexString(int index)
        {
            return GetEqualString(TIME_PERIOD_SELECTED_INDEX, index);
        }

        public bool CheckTimePeriodSelectedIndex(Dictionary<string, string> datasourceLabelDic)
        {
            return CheckInt(datasourceLabelDic, TIME_PERIOD_SELECTED_INDEX);
        }

        public string GettouProfileScheduleIdString(uint id)
        {
            return GetEqualString(TOU_PROFILE_SCHEDULE_ID, id);
        }

        public bool ChecktouProfileScheduleId(Dictionary<string, string> datasourceLabelDic)
        {
            return CheckUInt(datasourceLabelDic, TOU_PROFILE_SCHEDULE_ID);
        }

        public string GetIsQueryConditionString(bool isQueryCondition)
        {
            return GetEqualString(IS_QUERY_CONDITION, isQueryCondition);
        }

        public bool CheckIsQueryCondition(Dictionary<string, string> datasourceLabelDic, out bool resultValue)
        {
            return CheckBoolWithResult(datasourceLabelDic, IS_QUERY_CONDITION, out resultValue);
        }

        public string GetSourceString(uint source)
        {
            return GetEqualString(SOURCE, source);
        }

        public bool CheckSource(Dictionary<string, string> datasourceLabelDic, out uint source)
        {
            return CheckUIntWithResult(datasourceLabelDic, SOURCE, out source);
        }

        public string GetNameString(string name)
        {
            return GetEqualString(NAME, name);
        }

        public bool CheckName(Dictionary<string, string> datasourceLabelDic)
        {
            return CheckString(datasourceLabelDic, NAME);
        }

        public string GetQueryConditionString(string[] args)
        {
            StringBuilder result = new StringBuilder(@"##");
            result.Append(args);
            return result.ToString();
        }



        #region Enery Period Over Period Report

        public string GetCompareNumberString(int compareNumber)
        {
            return GetEqualString(COMPARE_NUMBER, compareNumber);
        }

        public bool CheckCompareNumber(Dictionary<string, string> datasourceLabelDic)
        {
            return CheckInt(datasourceLabelDic, COMPARE_NUMBER);
        }

        public readonly string COMBO_BOX_COMPARE_NUMBER_SELECTED_INDEX = "comboBoxCompareNumberSelectedIndex";

         public string GetcomboBoxCompareNumberSelectedIndexString(int selectedIndex)
         {
             return GetEqualString(COMBO_BOX_COMPARE_NUMBER_SELECTED_INDEX, selectedIndex);
         }

         public bool CheckcomboBoxCompareNumberSelectedIndex(Dictionary<string, string> datasourceLabelDic)
         {
             return CheckInt(datasourceLabelDic, COMBO_BOX_COMPARE_NUMBER_SELECTED_INDEX);
         }

         public readonly string DTP_START_TIME_VALUE = "dtpStartTimeValue";

         public string GetdtpStartTimeValueString(DateTime dateTime)
         {
             return GetEqualString(DTP_START_TIME_VALUE, dateTime);
         }

         public bool CheckdtpStartTimeValue(Dictionary<string, string> datasourceLabelDic)
         {
             return CheckDateTime(datasourceLabelDic, DTP_START_TIME_VALUE);
         }

         public readonly string DTP_END_TIME_VALUE = "dtpEndTimeValue";

         public string GetDtpEndTimeValueString(DateTime dateTime)
         {
             return GetEqualString(DTP_END_TIME_VALUE, dateTime);
         }

        public bool CheckDtpEndTimeValue(Dictionary<string, string> datasourceLabelDic)
        {
            return CheckDateTime(datasourceLabelDic, DTP_END_TIME_VALUE);
        }

         public readonly string COMBO_BOX_REPORT_TYPE_SELECTED_INDEX = "comboBoxReportTypeSelectedIndex";
             
         public string    GetcomboBoxReportTypeSelectedIndexString(int index)
         {
             return GetEqualString(COMBO_BOX_REPORT_TYPE_SELECTED_INDEX, index);
         }

        public bool CheckcomboBoxReportTypeSelectedIndex(Dictionary<string, string> datasourceLabelDic)
        {
            return CheckInt(datasourceLabelDic, COMBO_BOX_REPORT_TYPE_SELECTED_INDEX);
        }

         public readonly string COMBO_BOX_COMPARE_TYPE_SELECTED_INDEX = "comboBoxCompareTypeSelectedIndex";

         public string GetcomboBoxCompareTypeSelectedIndexString(int index)
         {
             return GetEqualString(COMBO_BOX_COMPARE_TYPE_SELECTED_INDEX, index);
         }

         public bool CheckcomboBoxCompareTypeSelectedIndex(Dictionary<string, string> datasourceLabelDic)
         {
             return CheckInt(datasourceLabelDic, COMBO_BOX_COMPARE_TYPE_SELECTED_INDEX);
         }

         public readonly string COMBO_BOX_YEAR2_TEXT = "comboBoxYear2Text";

         public string GetcomboBoxYear2TextString(string text)
         {
             return GetEqualString(COMBO_BOX_YEAR2_TEXT, text);
         }

         public bool CheckcomboBoxYear2Text(Dictionary<string, string> datasourceLabelDic)
         {
             return CheckString(datasourceLabelDic, COMBO_BOX_YEAR2_TEXT);
         }

         public readonly string COMBO_BOX_MONTH2_SELECTED_INDEX = "comboBoxMonth2SelectedIndex";

         public string GetcomboBoxMonth2SelectedIndexString(int index)
         {
             return GetEqualString(COMBO_BOX_MONTH2_SELECTED_INDEX, index);
         }

         public bool CheckcomboBoxMonth2SelectedIndex(Dictionary<string, string> datasourceLabelDic)
         {
             return CheckInt(datasourceLabelDic, COMBO_BOX_MONTH2_SELECTED_INDEX);
         }

         public readonly string COMBO_BOX_YEAR1_TEXT = "comboBoxYear1Text";

         public string GetcomboBoxYear1TextString(string text)
         {
             return GetEqualString(COMBO_BOX_YEAR1_TEXT, text);
         }

         public bool CheckcomboBoxYear1Text(Dictionary<string, string> datasourceLabelDic)
         {
             return CheckString(datasourceLabelDic, COMBO_BOX_YEAR1_TEXT);
         }

         public readonly string COMBO_BOX_MONTH1_SELECTED_INDEX = "comboBoxMonth1SelectedIndex";

         public string GetcomboBoxMonth1SelectedIndexString(int index)
         {
             return GetEqualString(COMBO_BOX_MONTH1_SELECTED_INDEX, index);
         }

         public bool CheckcomboBoxMonth1SelectedIndex(Dictionary<string, string> datasourceLabelDic)
         {
             return CheckInt(datasourceLabelDic, COMBO_BOX_MONTH1_SELECTED_INDEX);
         }

         #endregion

#region PowerQuality

         public readonly string INTERVAL_TYPE = "INTERVALTYPE";

         public string GetintervalTypeString(int type)
         {
             return GetEqualString(INTERVAL_TYPE, type);
         }

         public bool CheckintervalType(Dictionary<string, string> datasourceLabelDic)
         {
             return CheckInt(datasourceLabelDic, INTERVAL_TYPE);
         }

         public readonly string IS_DEMAND = "isDemand";

         public string GetisDemandString(bool d)
         {
             return GetEqualString(IS_DEMAND, d);
         }

         public bool CheckisDemand(Dictionary<string, string> datasourceLabelDic)
         {
             return CheckBool(datasourceLabelDic, IS_DEMAND);
         }

         public readonly string IS_ITIC = "isItic";

        public readonly string CURVEID = "curveID";

         public string GetisIticString(bool i)
         {
             return GetEqualString(IS_ITIC, i);
         }
         public string GetCurveIDString(int curveID)
         {
             return GetEqualString(CURVEID, curveID);
         }

         public bool CheckisItic(Dictionary<string, string> datasourceLabelDic)
         {
             return CheckBool(datasourceLabelDic, IS_ITIC);
         }

         #endregion


         public static bool IsEqualToUpper(string a,string b)
         {
             return string.Equals(a.ToUpper(), b.ToUpper());
         }


#region 普通报表也用到的

         public readonly string DATEOFFSETTYPE = "DATEOFFSETTYPE";
         public readonly string OFFSETDATE = "OFFSETDATE";
         public readonly string DATEOFFSET = "DATEOFFSET";
         public readonly string DATANUM = "DATANUM";
         public readonly string DATESPAN = "DATESPAN";
         public readonly string QUERYSPAN = "QUERYSPAN";
         public readonly string INTERVAL = "INTERVAL";
         public readonly string COMMONREPORTID = "COMMONREPORTID";
         public readonly string NODE = "NODE";

         public readonly string NODETYPE = "NODETYPE";

         public readonly string STATTYPE = "STATTYPE";

         #endregion


#region DefaultReport

         public readonly string COMBO_BOX_REPORT_PERIOD_SELECTED_INDEX = "comboBoxReportPeriodSelectedIndex";
         public string GetComboBoxReportPeriodSelectedIndexString(int selectedIndex)
         {
             return GetEqualString(COMBO_BOX_REPORT_PERIOD_SELECTED_INDEX, selectedIndex);
         }

         public bool CheckComboBoxReportPeriodSelectedIndex(Dictionary<string, string> datasourceLabelDic)
         {
             return CheckInt(datasourceLabelDic, COMBO_BOX_REPORT_PERIOD_SELECTED_INDEX);
         }
#endregion
       
    }
}
