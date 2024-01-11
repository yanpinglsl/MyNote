using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CET.PecsNodeManage;
using DBInterfaceCommonLib;
using System.Globalization;
using OfficeReportInterface.DefaultReportInterface;
using OfficeReportInterface.DefaultReportInterface.CommonluUsed;
using OfficeReportInterface.DefaultReportInterface.EnergyCost;
using OfficeReportInterface.DefaultReportInterface.PowerQualityEventsOnly;
using CSharpDBPlugin;


namespace OfficeReportInterface
{
    /// <summary>
    /// SingleDeviceUsageDataManager 的摘要说明
    /// </summary>
    public class MultiDeviceUsageDataManager : IDataSheet
    {
        private static uint source =(uint) RepServFileType.MultiUsage;
        #region 公用辅助成员方法

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
            //对于月类型报表，需要根据选择的月份的时间天数来决定报表行数
            int startTimeMaxDay = DefaultTemplatePublicMethod.GetMaxDaysofMonth(parameter.StartTime);
            int endTimeMaxDay = DefaultTemplatePublicMethod.GetMaxDaysofMonth(parameter.EndTime);
            int monthDayNumber = startTimeMaxDay >= endTimeMaxDay ? startTimeMaxDay : endTimeMaxDay;

            DataTable warningDT = DefaultTemplatePublicMethod.ConstructWarningTable(parameter.IsIncludeWarning);
            DataTable summaryDT = ConstructSummaryTable(parameter);
            DataTable resultDT = ConstructDataTable(parameter.PeriodType, parameter.CompareNumber, parameter.DeviceDataIDList);
            FillWarmSummaryDataToTable(parameter, warningDT, summaryDT, monthDayNumber, resultDT);
            resultDTList.Add(warningDT);
            resultDTList.Add(summaryDT);
            resultDTList.Add(resultDT);
            return resultDTList;
        }

        /// <summary>
        /// 构建汇总信息表格
        /// </summary>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        private DataTable ConstructSummaryTable(DefaultReportParameter parameter)
        {
            DataTable resultDT = new DataTable("Summary");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "device");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "1");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, DefaultTemplatePublicMethod.GetDateStringColumnName(parameter.StartTime, parameter.PeriodType));
            string tempName = DefaultTemplatePublicMethod.GetReportTypeNameByPeriodType(parameter.PeriodType);
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, tempName);

            if (parameter.CompareNumber == 2)
            {
                DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "Change");
            }
            //增加参数起始标志行，以便模板VBA进行计算
            DataRow resultRow = resultDT.NewRow();
            resultRow[0] = LocalResourceManager.GetInstance().GetString("0005", "Device");
            //坐标轴格式= "#,##0.00_ "
            resultRow[1] = "#" + NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator + "##0" + NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator + "00_";
            if (parameter.DeviceDataIDList.Count > 0)
                resultRow[2] = PecsNodeManager.PecsNodeInstance.FindDataID(parameter.DeviceDataIDList[0].DataID).DataName;
            if (parameter.PeriodType == 0)
                resultRow[3] = LocalResourceManager.GetInstance().GetString("0045", "Day");
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultRow[0] = LocalResourceManager.GetInstance().GetString("0005", "Device");
            //if (parameter.PeriodType != (int)PeriodType.Year_Type)
            //    resultRow[1] = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            resultRow[1] = DefaultTemplatePublicMethod.GetDateTimeFormat(parameter.PeriodType);
            resultRow[2] = DefaultTemplatePublicMethod.GetDateStringColumnName(parameter.StartTime, parameter.PeriodType);
            if (parameter.CompareNumber == 2)
            {
                resultRow[3] = DefaultTemplatePublicMethod.GetDateStringColumnName(parameter.EndTime, parameter.PeriodType);
                resultRow[4] = LocalResourceManager.GetInstance().GetString("0004", "Change");
            }
            resultDT.Rows.Add(resultRow);
            return resultDT;
        }

        private static void FillWarmSummaryDataToTable(DefaultReportParameter parameter, DataTable dataWarning, DataTable dataSummary, int monthDayNumber, DataTable resultDT)
        {
            //DataIDToMeasIDDef resultMapDef = DataIDToMeasIDDef.InvalidMapDef;
            for (int m = 0; m < parameter.CompareNumber; m++)
            {
                DataTable resultTempDT = resultDT.Clone();
                DateTime startTime = parameter.StartTime;
                if (m == 1)
                    startTime = parameter.EndTime;
                DateTime endTime = DefaultTemplatePublicMethod.CaculateEndTimeByPeriodType(startTime, parameter.PeriodType);
                AddTableDataRow(resultTempDT, startTime, endTime, parameter.DeviceDataIDList, parameter.PeriodType);

                for (int i = 0; i < parameter.DeviceDataIDList.Count; i++)
                {
                    DataAcquisitionDatalog dataAcquisitionDatalog = new DataAcquisitionDatalog();
                    if (!dataAcquisitionDatalog.HasDataMap(parameter.DeviceDataIDList[i]))
                    {
                        DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], WarningKind.MapNotExist, dataWarning, parameter.IsIncludeWarning, source);
                        continue;
                    }
                    DATALOG_PRIVATE_MAP resultMapDef;
                    bool result = ReportWebServiceManager.ReportWebManager.FindDataMapDef(parameter.DeviceDataIDList[i], out resultMapDef);
                    if (!result)
                    {                     
                        DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], WarningKind.SourceIdNotExist, dataWarning, parameter.IsIncludeWarning,source);
                        continue;
                    }
                    //获取该参数对应列的数据
                    List<DataLogOriDef> tempDT = new List<DataLogOriDef>();
                    result = DefaultTemplatePublicMethod.QueryDataLogData(startTime, endTime, parameter.DeviceDataIDList[i], parameter.PeriodType, ref tempDT, ref dataWarning, parameter.IsIncludeWarning,source);
                    if (!result)
                        continue;
                    PrintDataTable(dataWarning);
                    DefaultTemplatePublicMethod.AddDataNullWarnings(parameter, dataWarning, resultTempDT, i, tempDT, source);
                    PrintDataTable(dataWarning);
                }
                //增加汇总行
                DefaultTemplatePublicMethod.AddTotalColumn(resultTempDT, 1, 1);
                DefaultTemplatePublicMethod.AddTotalRow(resultTempDT, 1);
                AddSummaryData(dataSummary, resultTempDT, parameter.DeviceDataIDList, m + 1);
                DefaultTemplatePublicMethod.AddDataLastRow(resultTempDT);
                resultDT.Merge(resultTempDT);
            }
            AddSummaryDataLastRow(dataSummary, parameter.CompareNumber);
        }

        private static void PrintDataTable(DataTable resultDT)
        {
            DefaultTemplatePublicMethod.PrintDataTable(resultDT);
        }

        private static void AddSummaryDataLastRow(DataTable summaryDT, int compareNumber)
        {
            DataRow dr = summaryDT.NewRow();
            dr[0] = "Total";
            for (int j = 2; j < summaryDT.Columns.Count; j++)
            {
                double columnTotal = double.NaN;
                for (int i = 2; i < summaryDT.Rows.Count; i++)
                {
                    double tempValue = double.NaN;
                    if (double.TryParse(summaryDT.Rows[i][j].ToString(), out tempValue))
                    {
                        if (double.IsNaN(columnTotal))
                            columnTotal = 0;
                        columnTotal += tempValue;
                    }
                }
                if (!double.IsNaN(columnTotal))
                    dr[j] = columnTotal;
            }
            if (compareNumber == 2)
                dr[4] = DefaultTemplatePublicMethod.GetChangedValue(dr[2], dr[3]);
            summaryDT.Rows.Add(dr);
        }

        /// <summary>
        /// 增加汇总表行
        /// </summary>
        /// <param name="summaryDataTable"></param>
        /// <param name="isIncludeWarning"></param>
        private static void AddSummaryData(DataTable summaryDataTable, DataTable resultTable, List<DeviceDataIDDef> queryParamList, int compareNumber)
        {
            DataRow dr = null; ;
            for (int i = 0; i < queryParamList.Count; i++)
            {
                if (compareNumber == 2)
                {
                    dr = summaryDataTable.Rows[i + 2];
                    dr[3] = resultTable.Rows[resultTable.Rows.Count - 1][i + 1];
                    dr[4] = DefaultTemplatePublicMethod.GetChangedValue(dr[2], dr[3]);
                }
                else
                {
                    dr = summaryDataTable.NewRow();
                    dr[0] = DefaultTemplatePublicMethod.GetDeviceNameByDeviceDataID(queryParamList[i],source);
                    dr[2] = resultTable.Rows[resultTable.Rows.Count - 1][i + 1];
                    summaryDataTable.Rows.Add(dr);
                }
            }
        }

        private static void AddTableDataRow(DataTable resultDT, DateTime startTime, DateTime endTime, List<DeviceDataIDDef> queryParamList, int periodType)
        {
            //增加参数名称行
            DataRow resultRow = resultDT.NewRow();
            resultRow[0] = DefaultTemplatePublicMethod.GetDateStringColumnName(startTime, periodType);
            for (int i = 0; i < queryParamList.Count; i++)
            {
                resultRow[i + 1] = DefaultTemplatePublicMethod.GetDeviceNameByDeviceDataID(queryParamList[i],source);
            }
            resultRow[queryParamList.Count + 1] = LocalResourceManager.GetInstance().GetString("0044", "Interval Total");
            resultDT.Rows.Add(resultRow);

            //根据时间类型初始化表框架
            if (periodType == 2)
            {
                int startTimeMaxDay = DefaultTemplatePublicMethod.GetMaxDaysofMonth(startTime);
                int endTimeMaxDay = DefaultTemplatePublicMethod.GetMaxDaysofMonth(startTime);
                int monthDayNumber = startTimeMaxDay >= endTimeMaxDay ? startTimeMaxDay : endTimeMaxDay;
                DateTime tempTime = startTimeMaxDay >= endTimeMaxDay ? startTime : endTime;
                for (int i = 0; i < monthDayNumber; i++)
                {
                    resultRow = resultDT.NewRow();
                    resultRow[0] = DefaultTemplatePublicMethod.CaculateTimeIndexByPeriodType(tempTime, periodType);
                    resultDT.Rows.Add(resultRow);
                    tempTime = DefaultTemplatePublicMethod.CaculateIntervelEndTimeByPeriodType(tempTime, periodType);
                }
            }
            else
            {
                DateTime temEndTime = DefaultTemplatePublicMethod.CaculateEndTimeByPeriodType(startTime, periodType);
                while (startTime < temEndTime)
                {
                    resultRow = resultDT.NewRow();
                    resultRow[0] = DefaultTemplatePublicMethod.CaculateTimeIndexByPeriodType(startTime, periodType);
                    resultDT.Rows.Add(resultRow);
                    startTime = DefaultTemplatePublicMethod.CaculateIntervelEndTimeByPeriodType(startTime, periodType);
                }
            }
        }

        /// <summary>
        /// 创建报表结果表格列
        /// </summary>
        /// <param name="periodType"></param>
        /// <param name="compareNumber"></param>
        /// <param name="queryParamList"></param>
        /// <returns></returns>
        private DataTable ConstructDataTable(int periodType, int compareNumber, List<DeviceDataIDDef> queryParamList)
        {
            DataTable resultDT = new DataTable("Data");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, DefaultTemplatePublicMethod.GetIndexCulumnName(periodType));

            for (int i = 0; i < queryParamList.Count; i++)
                DefaultTemplatePublicMethod.AddColumnToTable(resultDT, i.ToString());
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "Interval Total");
            //增加起始标志
            DataRow resultRow = resultDT.NewRow();
            resultRow[0] = "Next";
            resultDT.Rows.Add(resultRow);
            return resultDT;
        }

        #endregion
    }
}
