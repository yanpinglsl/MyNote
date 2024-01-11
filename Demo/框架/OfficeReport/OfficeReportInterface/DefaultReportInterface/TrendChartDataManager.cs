using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using DBInterfaceCommonLib;
using System.Globalization;
using OfficeReportInterface.DefaultReportInterface;
using OfficeReportInterface.DefaultReportInterface.CommonluUsed;
using OfficeReportInterface.DefaultReportInterface.EnergyCost;
using OfficeReportInterface.DefaultReportInterface.PowerQualityEventsOnly;
using OfficeReportInterface.ReadingIniFile;
using CSharpDBPlugin;


namespace OfficeReportInterface
{
    /// <summary>
    /// TrendChartManager 的摘要说明
    /// </summary>
    public class TrendChartDataManager : IDataSheet
    {
        private static uint source = (uint) RepServFileType.Trend;
        #region 公用辅助成员方法

        /// <summary>
        /// 批量查询定时记录数据
        /// </summary>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="intervalType">查询间隔</param>
        /// <param name="statType">统计类型</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="resultDT">查询结果集</param>
        /// <returns>是否成功</returns>
        public List<DataTable> GetDataLogDatas(DefaultReportParameter parameter)
        {
            List<DataTable> resultDTList = new List<DataTable>();
            DataTable warningDT = DefaultTemplatePublicMethod.ConstructWarningTable(parameter.IsIncludeWarning);
            DataTable summaryDT = ConstructDataSummaryTable(parameter);
            DataTable resultDT = DefaultTemplatePublicMethod.ConstructRealDataTable(parameter.StartTime, parameter.EndTime, parameter.DeviceDataIDList, (uint)RepServFileType.Trend);
            //根据传入的参数填充警告表，汇总表，数据表
            FillWarmSummaryDataToTable(parameter, warningDT, summaryDT, resultDT);
            resultDTList.Add(warningDT);
            resultDTList.Add(summaryDT);
            resultDTList.Add(resultDT);
            return resultDTList;
        }

        public static DataTable ConstructDataSummaryTable(DefaultReportParameter parameter)
        {
            DataTable resultDT = new DataTable("Summary");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "1");
            DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "2");
            DataRow resultRow = resultDT.NewRow();
       
            resultRow[0] = DataManager.GetStartTimeSting(parameter.StartTime);
            resultRow[1] = DataManager.GetEndTimeString(parameter.EndTime);
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            string deviceParaStr = string.Empty;
            uint deviceID = 0;
            for (int i = 0; i < parameter.DeviceDataIDList.Count; i++)
            {
                if (deviceID != parameter.DeviceDataIDList[i].DeviceID)
                {
                    if (deviceID != 0)
                    {
                        deviceParaStr = deviceParaStr.Substring(0, deviceParaStr.Length - 1) + "),";
                    }
                    deviceParaStr = deviceParaStr + DefaultTemplatePublicMethod.GetDeviceNameByDeviceDataID(parameter.DeviceDataIDList[i], (uint)RepServFileType.Trend) + "(";
                    deviceParaStr = deviceParaStr + DefaultTemplatePublicMethod.GetParanameByParament(parameter.DeviceDataIDList[i],source) + ",";
                }
                else
                    deviceParaStr = deviceParaStr + DefaultTemplatePublicMethod.GetParanameByParament(parameter.DeviceDataIDList[i],source) + ",";
                deviceID = parameter.DeviceDataIDList[i].DeviceID;
            }
            deviceParaStr = deviceParaStr.Substring(0, deviceParaStr.Length - 1) + ")";
            resultRow[0] = deviceParaStr;
            resultDT.Rows.Add(resultRow);

            resultRow = resultDT.NewRow();
            resultRow[0] = GetDataFormatString();//例如"#,##0.00"。

            resultRow[1] = parameter.IsIncludeTable.ToString();
            resultDT.Rows.Add(resultRow);
            //chart类型
            resultRow = resultDT.NewRow();

            resultRow[0] = GetChartType(parameter.PeriodType);
            resultRow[1] = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " HH:mm";
            resultDT.Rows.Add(resultRow);
            return resultDT;
        }
        #region 从OfficeReport.ini文件读取数据在Excel中使用的字符串格式字符串，例如"#,##0.00"
        /// <summary>
        /// 数据在Excel中的格式，例如"#,##0.00"。
        /// </summary>
        private static string _dataFormat = "#,##0.00";
        /// <summary>
        /// 是否已经读取过OfficeReport.ini中配置的保留小数位数的信息
        /// </summary>
        private static bool _hasReadIniDataFormat = false;

        /// <summary>
        /// 获取Trend数据在Excel中的格式字符串，例如"#,##0.00"
        /// </summary>
        /// <returns></returns>
        private static string GetDataFormatString()
        {
            if (_hasReadIniDataFormat) //如果已经读取过OfficeReport.ini中的保留小数位数，则直接使用这个数据
                return _dataFormat;
            try
            {
                //string fileName = DbgTrace.GetOfficeReportIniFilePath(); //获取OfficeReport.ini文件的路径
                const uint defaultCount = 2; //默认保留2位小数
                uint retainedDecimalDigits = defaultCount; //默认保留2位小数，如果OfficeReport.ini文件不存在，则会使用这里设置的默认值
                //if (File.Exists(fileName))
                //{
                var iniFile = INIFile.GetIniFileByType(INIFile.IniFileType.OfficeReportIni); //new INIFile(fileName);
                if (iniFile != null)
                {
                    var retainedDecimalDigitsStr = iniFile.ReadString("Trend", "RetainedDecimalDigits"); //读取[Trend]下的RetainedDecimalDigits配置项的值

                    if (!uint.TryParse(retainedDecimalDigitsStr, out retainedDecimalDigits)) //如果从OfficeReport.ini文件读取到配置的Trend的保留小数位数，则使用这个数值；如果没读取到，则设置为默认的保留2位小数
                    {
                        DbgTrace.dout("Read Trend RetainedDecimalDigits failed. Use default value 2 . "); //未成功读取到这个配置项的合法数据，写日志
                        retainedDecimalDigits = defaultCount; //默认保留2位小数
                    }
                    if (retainedDecimalDigits > 6) //如果设置的值超过6，则使用默认的2，丁玉凤认可的这个值6
                        retainedDecimalDigits = defaultCount;
                }

                //}
                //else
                //{
                //    DbgTrace.dout("File "+fileName+" not exists.");//OfficeReport.ini文件不存在，写日志
                //}
                _dataFormat = EnergyCostChartDataManager.GetDataFormat(retainedDecimalDigits); //生成Excel使用的格式字符串,保留到成员变量 _dataFormat
                _hasReadIniDataFormat = true; //设置为true，标识已经读取过OfficeReport.ini中的保留小数位数
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }
            return _dataFormat;
        }

        #endregion
        private static string GetChartType(int type)
        {
            switch (type)
            {
                case 0:
                    return "xlLine";
                case 1:
                    return "xlBarClustered";
                case 2:
                    return "xlColumnClustered";
                case 3:
                    return "xlPie";
                case 4:
                    return "xlBarStacked";
                case 5:
                    return "xlColumnStacked";
                default:
                    return "xlLine";
            }
        }

        private static void FillWarmSummaryDataToTable(DefaultReportParameter parameter, DataTable dataWarning, DataTable dataSummary, DataTable resultDT)
        {
            int index = 0;
            DateTime tempStartTime = parameter.StartTime;
            int interval = 3600;
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
                    DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i],WarningKind.SourceIdNotExist, dataWarning, parameter.IsIncludeWarning,source);
                    continue;
                }
                //DataTable tempDT = new DataTable();
                //int maxRowCount = Convert.ToInt32(SysConstDefinition.DefaultMaxRowCount);
                //int errorCode = DefaultTemplatePublicMethod.ReadDatalogs(
                //    DBOperationFlag.either, resultMapDef.StationID, resultMapDef.SourceID, resultMapDef.DataIndex,
                //    parameter.StartTime, parameter.EndTime.AddSeconds(1), (int)SysConstDefinition.DefaultMaxRowCount,
                //    ref tempDT, parameter.DeviceDataIDList[i]);
                //int errorCode = DefaultTemplatePublicMethod.ReadDatalogs(resultMapDef, parameter.StartTime, parameter.EndTime.AddSeconds(1), parameter.DeviceDataIDList[i]);
                //if (errorCode != 0)
                //{
                //    DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], DBInterfaceCommonLib.ErrorQuerier.Instance.GetLastErrorString(), dataWarning, parameter.IsIncludeWarning,source);
                //    continue;
                //}
                List<DataLogOriDef> tempDT = new List<DataLogOriDef>();
                bool isSucess =  DefaultTemplatePublicMethod.ReadDatalogs(resultMapDef, parameter.StartTime, parameter.EndTime.AddSeconds(1), parameter.DeviceDataIDList[i],ref tempDT);
                if (!isSucess)
                {
                    DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], DBInterfaceCommonLib.ErrorQuerier.Instance.GetLastErrorString(), dataWarning, parameter.IsIncludeWarning, source);
                    continue;
                }
                if (tempDT.Count >= DefaultTemplatePublicMethod.MAX_COUNT)
                {
                    DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i],WarningKind.DataOver10000Rows , dataWarning, parameter.IsIncludeWarning,source);
                }
                if (index == 0)
                {
                    interval = DefaultTemplatePublicMethod.GetInterval(tempDT, ref index, ref tempStartTime, parameter.DeviceDataIDList[i], dataWarning, parameter.IsIncludeWarning,source);
                    if (tempDT.Count >= DefaultTemplatePublicMethod.MAX_COUNT)
                        parameter.EndTime = Convert.ToDateTime(tempDT[tempDT.Count - 1].LogTime);
                    List<string> errorMessageList = new List<string>();
                    if (index != -1)
                        DefaultTemplatePublicMethod.ConstrutDataTable(tempStartTime, parameter.EndTime, resultDT, interval,out errorMessageList);
                }
                DateTime dataNullStartTime = DateTime.MinValue;
                int dataNullNuber = 0;

                List<DataLogOriDef> dt = DefaultTemplatePublicMethod.GetSameTimeVal(tempStartTime, parameter.EndTime, parameter.DeviceDataIDList[i], tempDT, interval, ref dataNullStartTime, ref dataNullNuber,parameter.StartTime,true);
                if (dataNullNuber >= 1)
                {
                    DefaultTemplatePublicMethod.DataWarningNullFromTime(dataNullStartTime, parameter.DeviceDataIDList[i], dataWarning,
                                parameter.IsIncludeWarning,source);                
                }
                for (int j = 0; j < dt.Count; j++)
                {
                    resultDT.Rows[j + 2][i + 1] = dt[j].DataValue;
                }
                index++;
            }
        }
        #endregion
    }
}
