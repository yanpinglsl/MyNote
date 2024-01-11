using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeReportInterface.QueryCondition
{
    interface IConditionInformation
    {
    }

    public class ConditionDefaultReport : IConditionInformation
    {
          //case (uint)RepServFileType.MultiUsage:
          //      case (uint)RepServFileType.SingleUsage:
          //          if (!commonlyUsedString.CheckDeviceIDs(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckIsIncludeWarning(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckCompareNumber(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckPeriodType(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckdtpStartTimeValue(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckcomboBoxYear1Text(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckcomboBoxMonth1SelectedIndex(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckcomboBoxReportTypeSelectedIndex(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckComboBoxReportPeriodSelectedIndex(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckDtpEndTimeValue(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckcomboBoxYear2Text(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckcomboBoxMonth2SelectedIndex(datasourceLabelDic))
          //              return false;
          //          return true;
    }

    public class ConditionEnergyCost : IConditionInformation
    {
        public string DevicesIds { get; set; }
        public bool IsIncludeWarning { get; set; }
        public int TouProfileScheduleId { get; set; }
        public DateTime SelectedStartedTime { get; set; }
        public DateTime SelectedEndTime { get; set; }
        public int TimePeriodSelectedIndex { get; set; }
           //case (uint)RepServFileType.EnergyCost:
           //         if (!commonlyUsedString.CheckDeviceIDs(datasourceLabelDic))
           //             return false;
           //         if (!commonlyUsedString.CheckIsIncludeWarning(datasourceLabelDic))
           //             return false;
           //         if (!commonlyUsedString.ChecktouProfileScheduleId(datasourceLabelDic))
           //             return false;
           //         if (!commonlyUsedString.CheckSelectedStartedTime(datasourceLabelDic))
           //             return false;
           //         if (!commonlyUsedString.CheckSelectedEndTime(datasourceLabelDic))
           //             return false;
           //         if (!commonlyUsedString.CheckTimePeriodSelectedIndex(datasourceLabelDic))
           //             return false;
           //        return true;
    }

    public class ConditionEnergyPeriod : IConditionInformation
    {
        
            //case (uint)RepServFileType.EnergyPeriod:
            //        if (!commonlyUsedString.CheckPeriodType(datasourceLabelDic))
            //            return false;
            //        if (!commonlyUsedString.CheckCompareNumber(datasourceLabelDic))
            //            return false;
            //        if (!commonlyUsedString.CheckDeviceIDs(datasourceLabelDic))
            //            return false;
            //        if (!commonlyUsedString.CheckIsIncludeWarning(datasourceLabelDic))
            //            return false;
            //        if (!commonlyUsedString.CheckcomboBoxCompareNumberSelectedIndex(datasourceLabelDic))
            //            return false;
            //        if (!commonlyUsedString.CheckdtpStartTimeValue(datasourceLabelDic))
            //            return false;
            //        if (!commonlyUsedString.CheckDtpEndTimeValue(datasourceLabelDic))
            //            return false;
            //        if (!commonlyUsedString.CheckcomboBoxReportTypeSelectedIndex(datasourceLabelDic))
            //            return false;
            //        if (!commonlyUsedString.CheckcomboBoxCompareTypeSelectedIndex(datasourceLabelDic))
            //            return false;
            //        if (!commonlyUsedString.CheckcomboBoxYear2Text(datasourceLabelDic))
            //            return false;
            //        if (!commonlyUsedString.CheckcomboBoxMonth2SelectedIndex(datasourceLabelDic))
            //            return false;
            //        if (!commonlyUsedString.CheckcomboBoxYear1Text(datasourceLabelDic))
            //            return false;
            //        if (!commonlyUsedString.CheckcomboBoxMonth1SelectedIndex(datasourceLabelDic))
            //            return false;
                 
            //        return true;
    }

    public class ConditionEventQuery : IConditionInformation
    {
         //case (uint)RepServFileType.EventHistory:
         //           if (!commonlyUsedString.CheckDeviceIDs(datasourceLabelDic))
         //               return false;
         //           if (!commonlyUsedString.CheckIsIncludeWarning(datasourceLabelDic))
         //               return false;
         //           if (!commonlyUsedString.CheckSelectedStartedTime(datasourceLabelDic))
         //               return false;
         //           if (!commonlyUsedString.CheckSelectedEndTime(datasourceLabelDic))
         //               return false;
         //           if (!commonlyUsedString.CheckTimePeriodSelectedIndex(datasourceLabelDic))
         //               return false;
         //           if (!commonlyUsedString.CheckintervalType(datasourceLabelDic))
         //               return false;
         //           return true;
    }

    public class ConditionHourlyUsage : IConditionInformation
    {
               //case (uint)RepServFileType.HourlyUsage:
               //     if (!commonlyUsedString.CheckDeviceIDs(datasourceLabelDic))
               //         return false;
               //     if (!commonlyUsedString.CheckIsIncludeWarning(datasourceLabelDic))
               //         return false;
               //     if (!commonlyUsedString.CheckSelectedStartedTime(datasourceLabelDic))
               //         return false;
               //     if (!commonlyUsedString.CheckSelectedEndTime(datasourceLabelDic))
               //         return false;
               //     if (!commonlyUsedString.CheckIsIncludeAverage(datasourceLabelDic))
               //         return false;
               //     if (!commonlyUsedString.CheckIsIncludeTotal(datasourceLabelDic))
               //         return false;
               //     if (!commonlyUsedString.CheckPeriodType(datasourceLabelDic))
               //         return false;
               //     return true;
    }

    public class ConditionPowerQuality : IConditionInformation
    {
          //case (uint)RepServFileType.LoadProfile:
          //      case (uint)RepServFileType.PowerQuality:
          //          if (!commonlyUsedString.CheckDeviceIDs(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckIsIncludeWarning(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckintervalType(datasourceLabelDic))
          //              return false;
          //               if (!commonlyUsedString.CheckisDemand(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckisItic(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckSelectedStartedTime(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckSelectedEndTime(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckTimePeriodSelectedIndex(datasourceLabelDic))
          //              return false;
          //          return true;

    }
    public class ConditionTabular : IConditionInformation
    {
             //case (uint)RepServFileType.Tabular:
             //       if (!commonlyUsedString.CheckDeviceIDs(datasourceLabelDic))
             //           return false;
             //       if (!commonlyUsedString.CheckIsIncludeWarning(datasourceLabelDic))
             //           return false;
             //       if (!commonlyUsedString.CheckSelectedStartedTime(datasourceLabelDic))
             //           return false;
             //       if (!commonlyUsedString.CheckSelectedEndTime(datasourceLabelDic))
             //           return false;
             //       if (!commonlyUsedString.CheckTimePeriodSelectedIndex(datasourceLabelDic))
             //           return false;
             //       return true;
    }

    public class ConditionTrend : IConditionInformation
    {
          //case (uint)RepServFileType.Trend:
          //             if (!commonlyUsedString.CheckDeviceIDs(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckIsIncludeWarning(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckIsIncludeDataTable(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckPeriodType(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckSelectedStartedTime(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckSelectedEndTime(datasourceLabelDic))
          //              return false;
          //          if (!commonlyUsedString.CheckTimePeriodSelectedIndex(datasourceLabelDic))
          //              return false;
          //          return true;
    }
}
