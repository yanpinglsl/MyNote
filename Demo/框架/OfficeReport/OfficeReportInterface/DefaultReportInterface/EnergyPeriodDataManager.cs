using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using CSharpDBPlugin;
using OfficeReportInterface.DefaultReportInterface;
using OfficeReportInterface.DefaultReportInterface.CommonluUsed;
using OfficeReportInterface.DefaultReportInterface.EnergyCost;
using OfficeReportInterface.DefaultReportInterface.PowerQualityEventsOnly;


namespace OfficeReportInterface
{
    /// <summary>
    /// EnergyPeriodDataManager 的摘要说明
    /// </summary>
    public class EnergyPeriodDataManager : IDataSheet
    {
        private static uint source = (uint) RepServFileType.EnergyPeriod;

        /// <summary>
        /// 批量查询定时记录数据
        /// </summary>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="intervalType">查询间隔</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="resultDT">查询结果集</param>
        /// <returns>是否成功</returns>
        public List<DataTable> GetDataLogDatas(DefaultReportParameter parameter)
        {
            List<DataTable> resultDTList = new List<DataTable>();
            DataTable warningDT = DefaultTemplatePublicMethod.ConstructWarningTable(parameter.IsIncludeWarning);
            DataTable summaryDT = ConstructDataSummaryTable(parameter);
            DataTable resultDT = CreateDTColumns(parameter);
            //DataIDToMeasIDDef resultMapDef = DataIDToMeasIDDef.InvalidMapDef; 
            for (int j = 1; j < resultDT.Columns.Count - 1; j++)
            {  
                DateTime tempStartTime = parameter.DateTimeList[j - 1];
                DateTime tempEndTime = DefaultTemplatePublicMethod.CaculateEndTimeByPeriodType(tempStartTime, parameter.PeriodType);
                for (int i = 0; i < parameter.DeviceDataIDList.Count; i++)
                {
                    DataAcquisitionDatalog dataAcquisitionDatalog = new DataAcquisitionDatalog();
                    if (!dataAcquisitionDatalog.HasDataMap(parameter.DeviceDataIDList[i]))
                    {
                        DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], WarningKind.MapNotExist, warningDT, parameter.IsIncludeWarning, source);
                        continue;
                    }
                    DATALOG_PRIVATE_MAP resultMapDef;
                    bool result = ReportWebServiceManager.ReportWebManager.FindDataMapDef(parameter.DeviceDataIDList[i], out resultMapDef);
                    if (!result)
                    {
                        DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], WarningKind.SourceIdNotExist, warningDT, parameter.IsIncludeWarning, (uint)RepServFileType.EnergyPeriod);
                        continue;
                    }
                    List<DataLogOriDef> tempDT = new List<DataLogOriDef>();
                    DefaultTemplatePublicMethod.QueryDataLogData(tempStartTime, tempEndTime, parameter.DeviceDataIDList[i], parameter.PeriodType, ref tempDT, ref warningDT, parameter.IsIncludeWarning, source);

                    for (int k = 0; k < tempDT.Count; k++)
                    {
                        var resultValue = resultDT.Rows[k + 1][j];
                        var addValue = tempDT[k].DataValue;
                        var resultValue2 = DefaultTemplatePublicMethod.GetTotalData(resultValue, addValue);
                        resultDT.Rows[k + 1][j] = resultValue2; 

                        PrintDataTable(resultDT);
                    }
                }
            }

            //增加汇总行 
            DefaultTemplatePublicMethod.CaculateRowAndColumnTotal(ref resultDT);
            PrintDataTable(resultDT);
            resultDTList.Add(warningDT);
            resultDTList.Add(summaryDT);
            resultDTList.Add(resultDT);
            return resultDTList;
        }

        private static void PrintDataTable(DataTable resultDT)
        {
            DefaultTemplatePublicMethod.PrintDataTable(resultDT);
        }

        private bool GetMinDateTime(List<DateTime> dateTimeList, out DateTime minTime)
        {
            minTime = DateTime.MaxValue;
            if (dateTimeList.Count == 0)
                return false;
            foreach (var item in dateTimeList )
            {
                if (item < minTime)
                    minTime = item;
            }
            return true;
        }

        /// <summary>
        /// 构建汇总信息表格
        /// </summary>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        private DataTable ConstructDataSummaryTable(DefaultReportParameter parameter)
        {
            DataTable resultDT = new DataTable("Summary");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "Column1");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "Column2");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "Column3");
            //增加参数起始标志行，以便模板VBA进行计算
            DataRow resultRow = resultDT.NewRow();
            resultRow[0] = LocalResourceManager.GetInstance().GetString("0003", "Report Type");
            resultRow[1] = DefaultTemplatePublicMethod.GetReportTypeNameByPeriodType(parameter.PeriodType);
            resultRow[2] = "#" + NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator + "##0" + NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator + "00_";
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultRow[0] = LocalResourceManager.GetInstance().GetString("0009", "Start Date");
            DateTime minTime;
            GetMinDateTime(parameter.DateTimeList, out  minTime);
            resultRow[1] = minTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern) + "\r";
            resultRow[2] = DefaultTemplatePublicMethod.GetDateTimeFormat(parameter.PeriodType);//CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            resultDT.Rows.Add(resultRow);
            DateTime dt = DateTime.Now;
            ;
            resultRow = resultDT.NewRow();
            resultRow[0] = LocalResourceManager.GetInstance().GetString("0010", "Selected Interval");
            resultRow[1] = GetDateInterval(parameter);
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultRow[0] = LocalResourceManager.GetInstance().GetString("0011", "Number of Comparisons");
            resultRow[1] = parameter.DateTimeList.Count - 1;
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultRow[0] = LocalResourceManager.GetInstance().GetString("0021", "Devices");
           
             
       
            resultRow[1] = GetDeviceListName(parameter);
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultRow[0] = LocalResourceManager.GetInstance().GetString("0022", "Parameter");
            resultRow[1] = DefaultTemplatePublicMethod.GetParanameByParament(parameter.DeviceDataIDList[0],source);
            resultDT.Rows.Add(resultRow);

            return resultDT;
        }

        private string GetDeviceListName(DefaultReportParameter parameter)
        {
            string deviceNames;
            var namesManager = NamesManager.GetInstance((uint)RepServFileType.EnergyPeriod);
            namesManager.GetAllDeviceNamesWithLoop(parameter.DeviceDataIDList, out deviceNames);
            return deviceNames;
        }

        private string GetDateInterval(DefaultReportParameter parameter)
        {
            if (parameter.PeriodType == 0)
            {
                if (parameter.CompareNumber == 0)
                    return LocalResourceManager.GetInstance().GetString("0024", "Today vs Previous Days");
                else if (parameter.CompareNumber == 1)
                    return LocalResourceManager.GetInstance().GetString("0025", "Today vs Same Weekday Previous Weeks");
                else
                    return LocalResourceManager.GetInstance().GetString("0029", "Two Specific Days");
            }
            else if (parameter.PeriodType == 1)
            {
                if (parameter.CompareNumber == 0)
                    return LocalResourceManager.GetInstance().GetString("0030", "This Week vs Previous Weeks");
                else if (parameter.CompareNumber == 1)
                    return LocalResourceManager.GetInstance().GetString("0031", "This Week vs Same Week Previous Years");
                else
                    return LocalResourceManager.GetInstance().GetString("0032", "Two Specific Weeks");
            }
            else if (parameter.PeriodType == 2)
            {
                if (parameter.CompareNumber == 0)
                    return LocalResourceManager.GetInstance().GetString("0033", "This Month vs Previous Months");
                else if (parameter.CompareNumber == 1)
                    return LocalResourceManager.GetInstance().GetString("0034", "This Month vs Same Month Previous Years");
                else
                    return LocalResourceManager.GetInstance().GetString("0035", "Two Specific Months");
            }
            else if (parameter.PeriodType == 3)
            {
                if (parameter.CompareNumber == 0)
                    return LocalResourceManager.GetInstance().GetString("0036", "This Year vs Previous Years");
                else
                    return LocalResourceManager.GetInstance().GetString("0038", "Two Specific Years");
            }
            return "";
        }

        #region 私有逻辑函数

        /// <summary>
        /// 创建趋势图表数据列
        /// </summary>
        /// <param name="resultDT">数据列表</param>
        private DataTable CreateDTColumns(DefaultReportParameter parameter)
        {
            DataTable resultDT = new DataTable("Data");
            DataColumn column;
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = DefaultTemplatePublicMethod.GetIndexCulumnName(parameter.PeriodType);
            resultDT.Columns.Add(column);

            for (int i = 0; i < parameter.DateTimeList.Count; i++)
            {
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "DateTime" + i;
                column.DefaultValue = "";
                resultDT.Columns.Add(column);
            }

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "Total";
            column.DefaultValue = "";
            resultDT.Columns.Add(column);

            DataRow dr = resultDT.NewRow();
            dr[0] = DefaultTemplatePublicMethod.GetIndexCulumnName(parameter.PeriodType);

            for (int i = 0; i < parameter.DateTimeList.Count; i++)
            {
                dr[i + 1] = DefaultTemplatePublicMethod.GetDateStringColumnName(parameter.DateTimeList[i], parameter.PeriodType);
            }
            dr[resultDT.Columns.Count - 1] = LocalResourceManager.GetInstance().GetString("0044", "Interval Total");
            resultDT.Rows.Add(dr);
            ConstructDataRows(parameter, ref resultDT);
            return resultDT;
        }

        private static DateTime ConstructDataRows(DefaultReportParameter parameter, ref DataTable resultDT)
        {
            DateTime startTime = parameter.DateTimeList[0];
            DateTime endTime = parameter.DateTimeList[1];
            if (parameter.PeriodType == 2)
            {
                int startTimeMaxDay = DefaultTemplatePublicMethod.GetMaxDaysofMonth(startTime);
                int endTimeMaxDay = DefaultTemplatePublicMethod.GetMaxDaysofMonth(endTime);
                int monthDayNumber = startTimeMaxDay >= endTimeMaxDay ? startTimeMaxDay : endTimeMaxDay;
                DateTime tempTime = startTimeMaxDay >= endTimeMaxDay ? startTime : endTime;
                for (int i = 0; i < monthDayNumber; i++)
                {
                    DataRow resultRow = resultDT.NewRow();
                    resultRow[0] = DefaultTemplatePublicMethod.CaculateTimeIndexByPeriodType(tempTime, parameter.PeriodType);
                    resultDT.Rows.Add(resultRow);
                    tempTime = DefaultTemplatePublicMethod.CaculateIntervelEndTimeByPeriodType(tempTime, parameter.PeriodType);
                }
            }
            else
            {
                DateTime temEndTime = DefaultTemplatePublicMethod.CaculateEndTimeByPeriodType(startTime, parameter.PeriodType);
                while (startTime < temEndTime)
                {
                    DataRow resultRow = resultDT.NewRow();
                    resultRow[0] = DefaultTemplatePublicMethod.CaculateTimeIndexByPeriodType(startTime, parameter.PeriodType);
                    resultDT.Rows.Add(resultRow);
                    startTime = DefaultTemplatePublicMethod.CaculateIntervelEndTimeByPeriodType(startTime, parameter.PeriodType);
                }
            }
            return startTime;
        }

        private string GetPeriodName(int periodType)
        {
            switch (periodType)
            {
                case (int)PeriodType.Day_Type:
                    return LocalResourceManager.GetInstance().GetString("0045", "Day");
                case (int)PeriodType.Week_Type:
                    return LocalResourceManager.GetInstance().GetString("0046", "Week");
                case (int)PeriodType.Month_Type:
                    return LocalResourceManager.GetInstance().GetString("0047", "Month");
                case (int)PeriodType.Year_Type:
                    return LocalResourceManager.GetInstance().GetString("0048", "Year");
                default:
                    return "";
            }
        }

        #endregion
    }
}
