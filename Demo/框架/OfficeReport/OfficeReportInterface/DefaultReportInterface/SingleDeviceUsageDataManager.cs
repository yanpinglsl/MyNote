using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
//using CET.PecsNodeManage;
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
    public class SingleDeviceUsageDataManager : IDataSheet
    {

        private static uint source = (uint)RepServFileType.SingleUsage;
         /// <summary>
        /// 获取SingleDevice Usage报表所有结果集数据
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public List<DataTable> GetDataLogDatas(DefaultReportParameter parameter)
        {
            List<DataTable> resultDTList = new List<DataTable>();
            //对于月类型报表，需要根据选择的月份的时间天数来决定报表行数
            int startTimeMaxDay = DefaultTemplatePublicMethod.GetMaxDaysofMonth(parameter.StartTime);
            int endTimeMaxDay = DefaultTemplatePublicMethod.GetMaxDaysofMonth(parameter.EndTime);
            int monthDayNumber = startTimeMaxDay >= endTimeMaxDay ? startTimeMaxDay : endTimeMaxDay;

            DataTable warningDT = DefaultTemplatePublicMethod.ConstructWarningTable(parameter.IsIncludeWarning);
            DataTable summaryDT = ConstructDataSummaryTable(parameter);
            DataTable resultDT = ConstructDataTable(parameter.StartTime, parameter.EndTime, parameter.PeriodType, parameter.CompareNumber);
            //根据传入的参数填充警告表，汇总表，数据表
            FillWarmSummaryDataToTable(parameter, warningDT, summaryDT, monthDayNumber, resultDT);
            resultDTList.Add(warningDT);
            resultDTList.Add(summaryDT);
            resultDTList.Add(resultDT);
            return resultDTList;
        }

        private static void FillWarmSummaryDataToTable(DefaultReportParameter parameter, DataTable dataWarning, DataTable dataSummary, int monthDayNumber, DataTable resultDT)
        {
            for (int i = 0; i < parameter.DeviceDataIDList.Count; i++)
            {
                DataTable resultTempDT = resultDT.Clone();
                AddTableDataRow(resultTempDT, parameter.StartTime, parameter.EndTime, parameter.DeviceDataIDList[i], parameter.PeriodType, parameter.CompareNumber);

                DataAcquisitionDatalog dataAcquisitionDatalog = new DataAcquisitionDatalog();
                if (!dataAcquisitionDatalog.HasDataMap(parameter.DeviceDataIDList[i]))
                {
                    DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], WarningKind.MapNotExist, dataWarning, parameter.IsIncludeWarning, source);
                    continue;
                }
                //bool result = ReportWebServiceManager.ReportWebManager.FindDataMapDef(parameter.DeviceDataIDList[i], out resultMapDef);
                //if (!result)
                //{

                //    DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], WarningKind.SourceIdNotExist, dataWarning, parameter.IsIncludeWarning, (uint)RepServFileType.SingleUsage);
                //    continue;
                //}
                DATALOG_PRIVATE_MAP resultMapDef;
                bool result = ReportWebServiceManager.ReportWebManager.FindDataMapDef(parameter.DeviceDataIDList[i], out resultMapDef);
                if (!result)
                {
                    DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], WarningKind.SourceIdNotExist, dataWarning, parameter.IsIncludeWarning, (uint)RepServFileType.SingleUsage);
                    continue;
                }
                //时段1数据
                List<DataLogOriDef> tempDT = new List<DataLogOriDef>();
                result = DefaultTemplatePublicMethod.QueryDataLogData(parameter.StartTime, DefaultTemplatePublicMethod.CaculateEndTimeByPeriodType(parameter.StartTime, parameter.PeriodType), parameter.DeviceDataIDList[i], parameter.PeriodType,ref tempDT, ref dataWarning, parameter.IsIncludeWarning,source);
                if (!result)
                    continue;
                int j = 1;

                for (int k = 0; k < tempDT.Count; k++)  
                {
                    PrintDataTable(resultTempDT);
                    var resultValue = resultTempDT.Rows[k + 2][j];
                    var addValue = tempDT[k].DataValue;
                    var resultValue2 = DefaultTemplatePublicMethod.GetTotalData(resultValue, addValue);
                    resultTempDT.Rows[k + 2][j] = resultValue2;
                    PrintDataTable(resultTempDT);
                }
                
                if (parameter.CompareNumber == 2)//查询时间只有一个时段，则没有时段2的数据
                {
                    j = 2;
                    //时段2数据
                    tempDT = new List<DataLogOriDef>();
                    result = DefaultTemplatePublicMethod.QueryDataLogData(parameter.EndTime, DefaultTemplatePublicMethod.CaculateEndTimeByPeriodType(parameter.EndTime, parameter.PeriodType), parameter.DeviceDataIDList[i], parameter.PeriodType,ref tempDT, ref dataWarning, parameter.IsIncludeWarning,source);
                    
                    for (int k = 0; k < tempDT.Count; k++)
                    {
                        PrintDataTable(resultTempDT);
                        var resultValue = resultTempDT.Rows[k + 2][j];
                        var addValue = tempDT[k].DataValue;
                        var resultValue2 = DefaultTemplatePublicMethod.GetTotalData(resultValue, addValue);
                        resultTempDT.Rows[k + 2][j] = resultValue2;
                        PrintDataTable(resultDT);
                    }
                }
                //增加汇总行
                DefaultTemplatePublicMethod.AddTotalRow(resultTempDT, 2);
                AddSummaryData(dataSummary, resultTempDT, parameter.DeviceDataIDList[i], parameter.CompareNumber);
                DefaultTemplatePublicMethod.AddDataLastRow(resultTempDT);
                resultDT.Merge(resultTempDT);
            }
        }

        private static void PrintDataTable(DataTable resultDT)
        {
            DefaultTemplatePublicMethod.PrintDataTable(resultDT);
        }

        private static void AddDataNullWarnings(DefaultReportParameter parameter, DataTable dataWarning, DataTable resultTempDT, int rowIndex, int columnIndex, DataTable tempDT)
        {
            //记录空缺数据的数目
            int dataNullNuber = 0;
            //记录第一条空缺数据的时间
            DateTime dataNullTime = DateTime.Now;
            for (int j = 0; j < tempDT.Rows.Count; j++)
            {
                if (DefaultTemplatePublicMethod.IsNaN(tempDT.Rows[j][3]))
                {
      
                    dataNullNuber++;
                    if (dataNullNuber == 1)
                        dataNullTime = Convert.ToDateTime(tempDT.Rows[j]["LogTime"]);
                }

            }
            if (dataNullNuber > 0)
            {

                DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[rowIndex], WarningKind.DataNullStartFromSomeTime,  dataNullTime, dataWarning, parameter.IsIncludeWarning,source);

            }
        }

        #region 私有辅助成员方法

        /// <summary>
        /// 增加汇总表行
        /// </summary>
        /// <param name="summaryDataTable"></param>
        /// <param name="isIncludeWarning"></param>
        private static void AddSummaryData(DataTable summaryDataTable, DataTable resultTable, DeviceDataIDDef para, int compareNumber)
        {
            DataRow dr = summaryDataTable.NewRow();
            dr[0] = DefaultTemplatePublicMethod.GetParanameByParament(para,source);
            dr[2] = resultTable.Rows[resultTable.Rows.Count - 1][1];
            if (compareNumber == 2)
            {
                dr[3] = resultTable.Rows[resultTable.Rows.Count - 1][2];
                dr[4] = DefaultTemplatePublicMethod.GetChangedValue(dr[2], dr[3]);
            }
            summaryDataTable.Rows.Add(dr);
        }

        /// <summary>
        /// 创建返回的结果集表格
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="periodType"></param>
        /// <param name="intervalType"></param>
        /// <param name="resultDTList"></param>
        /// <param name="resultDT"></param>
        /// <param name="dr2"></param>
        private DataTable ConstructDataTable(DateTime startTime, DateTime endTime, int periodType, int compareNumber)
        {
            DataTable resultDT = new DataTable("DataTable");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "1");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "2");
            //有比较时段，报表数据列加1
            if (compareNumber == 2)
                DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "3");
            //增加起始标志
            DataRow resultRow = resultDT.NewRow();
            resultRow[0] = "Next";
            resultDT.Rows.Add(resultRow);
            return resultDT;
        }

        private static void AddTableDataRow(DataTable resultDT, DateTime startTime, DateTime endTime, DeviceDataIDDef queryParam, int periodType, int compareNumber)
        {
            //增加参数名称行
            DataRow resultRow = resultDT.NewRow();
            resultRow[0] = DefaultTemplatePublicMethod.GetIndexCulumnName(periodType);
            resultRow[1] = DefaultTemplatePublicMethod.GetParanameByParament(queryParam,source);
            resultDT.Rows.Add(resultRow);
            //增加表头日期行
            resultRow = resultDT.NewRow();
            resultRow[1] = DefaultTemplatePublicMethod.GetDateStringColumnName(startTime, periodType);
            if (compareNumber == 2)
                resultRow[2] = DefaultTemplatePublicMethod.GetDateStringColumnName(endTime, periodType);
            resultDT.Rows.Add(resultRow);
            //根据时间类型初始化表框架
            if (periodType == 2)
            {
                int startTimeMaxDay = DefaultTemplatePublicMethod.GetMaxDaysofMonth(startTime);
                int endTimeMaxDay = DefaultTemplatePublicMethod.GetMaxDaysofMonth(endTime);
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
        /// 构建汇总信息表格
        /// </summary>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        private DataTable ConstructDataSummaryTable(DefaultReportParameter parameter)
        {
            DataTable resultDT = new DataTable("Summary");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "Measurements");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "1");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "2");

            if (parameter.CompareNumber == 2)
            {
                DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "3");
                DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "Change");
            }
            //增加参数起始标志行，以便模板VBA进行计算
            DataRow resultRow = resultDT.NewRow();
            resultRow[0] = LocalResourceManager.GetInstance().GetString("0027", "Parameters");
            //坐标轴格式= "#,##0.00_ "
            resultRow[1] = "#" + NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator + "##0" + NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator + "00_";
            if (parameter.DeviceDataIDList.Count > 0)
                resultRow[2] = DefaultTemplatePublicMethod.GetDeviceNameByDeviceDataID(parameter.DeviceDataIDList[0],source);
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultRow[0] = LocalResourceManager.GetInstance().GetString("0027", "Parameters");
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

        #endregion
    }
}
