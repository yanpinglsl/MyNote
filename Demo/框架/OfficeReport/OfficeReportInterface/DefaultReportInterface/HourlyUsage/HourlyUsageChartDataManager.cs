using System;
using System.Collections.Generic;
using System.Data;
using CSharpDBPlugin;
using DBInterfaceCommonLib;
using OfficeReportInterface.DefaultReportInterface.CommonluUsed;
using OfficeReportInterface.DefaultReportInterface.EnergyCost;
using OfficeReportInterface.DefaultReportInterface.PowerQualityEventsOnly;


namespace OfficeReportInterface.DefaultReportInterface.HourlyUsage
{
    class HourlyUsageChartDataManager : IDataSheet
    {
        private static uint source = (uint) RepServFileType.HourlyUsage;
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
            try
            {
                //对于月类型报表，需要根据选择的月份的时间天数来决定报表行数
                int startTimeMaxDay = DefaultTemplatePublicMethod.GetMaxDaysofMonth(parameter.StartTime);
                int endTimeMaxDay = DefaultTemplatePublicMethod.GetMaxDaysofMonth(parameter.EndTime);
                int monthDayNumber = startTimeMaxDay >= endTimeMaxDay ? startTimeMaxDay : endTimeMaxDay;


                DataTable warningDT = DefaultTemplatePublicMethod.ConstructWarningTable(parameter.IsIncludeWarning);
                DataTable summaryDT = ConstructDataSummaryTable(parameter);
                DataTable resultDT = ConstructRealDataTable(parameter.StartTime, parameter.EndTime, parameter.DeviceDataIDList);
                //根据传入的参数填充警告表，汇总表，数据表
                FillWarmSummaryDataToTable( parameter, warningDT, summaryDT, monthDayNumber, resultDT);
                resultDTList.Add(warningDT);
                resultDTList.Add(summaryDT);
                resultDTList.Add(resultDT);
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout("{0}{1}", "HourlyUsageChartDataManager.GetDataLogDatas(DefaultReportParameter parameter) catch (System.Exception ex):", ex.Message);
        
            }
       
            return resultDTList;
        }

    
        // <summary>
        /// 创建返回的结果集表格
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="periodType"></param>
        /// <param name="intervalType"></param>
        /// <param name="resultDTList"></param>
        /// <param name="resultDT"></param>
        /// <param name="dr2"></param>
        private static DataTable ConstructRealDataTable(DateTime startTime, DateTime endTime, List<DeviceDataIDDef> paraList)
        {
            DataTable resultDT = new DataTable("Data");
            DataColumn column;
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "TimeStamp";
            resultDT.Columns.Add(column);

            for (int i = 0; i <= paraList.Count; i++)
            {
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = i.ToString();
                resultDT.Columns.Add(column);
            }


            return resultDT;
        }
        ///// <summary>
        ///// 构建汇总信息表格
        ///// </summary>
        ///// <param name="queryParam"></param>
        ///// <returns></returns>
        private DataTable ConstructDataSummaryTable(DefaultReportParameter parameter)
        {
            DataTable resultDT = new DataTable("Summary");
            try
            {
                DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "Measurements");
                DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "1");
                DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "2");

                //起始结束时间
                DataRow resultRow = resultDT.NewRow();
                resultRow[0] = DataManager.GetStartTimeSting(parameter.StartTime);
                resultRow[1] = DataManager.GetEndTimeString(parameter.EndTime);
                resultDT.Rows.Add(resultRow);
                //空行
                resultRow = resultDT.NewRow();
                resultDT.Rows.Add(resultRow);
                //警告信息时间格式
                resultRow = resultDT.NewRow();
                resultRow[0] = DataManager.GetWarningMessageTimeFormatForExcel();
                resultRow[1] = "Date Added";
                resultDT.Rows.Add(resultRow);

                //数据坐标轴时间格式
                resultRow = resultDT.NewRow();
                resultRow[0] = DataManager.GetTableTimeFormatForExcel();
                resultRow[1] = "Curve Time";
                resultDT.Rows.Add(resultRow);

                //数据格式
                resultRow = resultDT.NewRow();
                resultRow[0] = DataManager.DataFormat;
                resultRow[1] = "Curve Data";
                resultDT.Rows.Add(resultRow);
                for (int i = 0; i < 7; ++i)
                {
                    //空行
                    resultRow = resultDT.NewRow();
                    resultDT.Rows.Add(resultRow);
                }

                //设备
                resultRow = resultDT.NewRow();
                resultRow[0] = LocalResourceManager.GetInstance().GetString("0021", "Devices");
                resultRow[2] = LocalResourceManager.GetInstance().GetString("0555", "Measurement");
                resultDT.Rows.Add(resultRow);

                var newRow = resultDT.NewRow();
                var namesManager = NamesManager.GetInstance((uint)RepServFileType.HourlyUsage);
                string deviceNames;
                if (namesManager.GetAllDeviceNamesWithLoop(parameter.DeviceDataIDList, out deviceNames))
                    newRow[0] = deviceNames;
                string measureNames;
                if (namesManager.GetMeasurementNames(parameter.DeviceDataIDList, out measureNames))
                    newRow[2] = measureNames;
                resultDT.Rows.Add(newRow);


                //空行
                AddEmptyLine(resultDT);

                AddNamesForLocal(resultDT);
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout("{0}{1}", "HourlyUsageChartDataManager.ConstructDataSummaryTable(DefaultReportParameter parameter) catch (System.Exception ex):", ex.Message);
        
            }
            return resultDT;
        }
        private static void AddNamesForLocal(DataTable resultDT)
        {
            DataRow resultRow;
            List<string> nameList = new List<string>();
            nameList.Add(LocalResourceManager.GetInstance().GetString("0021", "Devices"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0556", "Data Table"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0540", "Data Warnings"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0037", "Message"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0555", "Measurement"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0557", "Timestamp"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0558", "Average"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0539", "Total"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0552", "Hourly Usage Report"));
            foreach (var item in nameList)
            {
                resultRow = resultDT.NewRow();
                resultRow[0] = item;
                resultDT.Rows.Add(resultRow);
            }
        }
        private static void AddEmptyLine(DataTable resultDT)
        {
            DataRow resultRow;
            resultRow = resultDT.NewRow();
            resultDT.Rows.Add(resultRow);
        }

        private static void FillWarmSummaryDataToTable(DefaultReportParameter parameter, DataTable dataWarning, DataTable dataSummary, int monthDayNumber, DataTable resultDT)
        {

            //DataIDToMeasIDDef resultMapDef = DataIDToMeasIDDef.InvalidMapDef;
            DateTime tempStartTime = parameter.StartTime;
            int index = 0;

            DataRow dr1 = resultDT.NewRow();
            dr1[0] = LocalResourceManager.GetInstance().GetString("0557", "Timestamp");
            resultDT.Rows.Add(dr1);
            DataRow dr2 = resultDT.NewRow();
            dr2[0] = LocalResourceManager.GetInstance().GetString("0555", "Measurement");
            resultDT.Rows.Add(dr2);
            NamesManager nameManager = NamesManager.GetInstance((uint)RepServFileType.HourlyUsage);
            List<DataRow> rowList;
            AddTableDataRow(resultDT, parameter.StartTime,out rowList);

            AddTotalRow(parameter, ref resultDT);


            for (int i = 0; i < parameter.DeviceDataIDList.Count; i++)
            {
                GetFirstSecondeLineData(parameter, nameManager, i, dr1, dr2);
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

                var result2 = QueryDataLogData(i,rowList, parameter.StartTime, parameter.EndTime, parameter.DeviceDataIDList[i], monthDayNumber, parameter.PeriodType, ref dataWarning, parameter.IsIncludeWarning);
                if (!result2)
                    continue;
                index++;
            }
        }

        private static void AddTotalRow(DefaultReportParameter parameter, ref DataTable resultDT)
        {
            if (parameter.IsIncludeTotal)
            {
                DataRow resultRow = resultDT.NewRow();
                string temp = LocalResourceManager.GetInstance().GetString("0539", "Total");
                resultRow[0] = temp;
                resultDT.Rows.Add(resultRow);
            }
        }
        /// <summary>
        /// 查询 3/5 分钟间隔日报统计结果
        /// </summary>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="compareNumber">间隔类型</param>
        /// <param name="statType">统计类型</param>
        /// <param name="stationID">厂站号</param>
        /// <param name="sourceID">定时记录源号</param>
        /// <param name="paraIndex">参数序号</param>
        /// <param name="resultDT">查询结果集</param>
        /// <returns>是否成功</returns>
        private static bool QueryDataLogData(int columnN, List<DataRow> rowList,DateTime startTime, DateTime endTime, DeviceDataIDDef deviceDataID, int monthMaxDay, int periodType,  ref DataTable dataWarning, bool isShowWarning)
        {
            List<DeviceDataIDDef> tipedList = new List<DeviceDataIDDef>();
            //根据不同的间隔类型，自动格式化不同的查询起始时间和结束时间
            //DataTable tempDT = new DataTable();
          
            //int errorCode = PECSDBInterface.DatalogProvider.Instance.ReadDatalogs(
            //    DBOperationFlag.either, stationID, sourceID, paraIndex, startTime, endTime.AddSeconds(1),
            //    Convert.ToInt32(SysConstDefinition.DefaultMaxRowCount)
            //    , ref tempDT);
            //WriteLog(startTime, endTime, stationID, sourceID, paraIndex, errorCode);

            //int errorCode = DefaultTemplatePublicMethod.ReadDatalogs(
            //    DBOperationFlag.either, stationID, sourceID, paraIndex, startTime, endTime.AddSeconds(1),
            //    Convert.ToInt32(SysConstDefinition.DefaultMaxRowCount)
            //    , ref tempDT, deviceDataID);

            //if (errorCode != (int) ErrorCode.Success || tempDT.Rows.Count == 0)
            //{
            //    DefaultTemplatePublicMethod.DataWarningNullFromTime(startTime, deviceDataID, dataWarning, isShowWarning,source);
            //    return false;
            //}
            List<DataLogOriDef> tempDT = new List<DataLogOriDef>();
            bool isSucess = DefaultTemplatePublicMethod.ReadDatalogs(startTime, endTime.AddSeconds(1), deviceDataID, ref tempDT);
            if (!isSucess || tempDT.Count == 0)
            {
                DefaultTemplatePublicMethod.DataWarningNullFromTime(startTime, deviceDataID, dataWarning, isShowWarning, source);
                return false;
            }
            int index = 0;
            var tempStartTime = startTime;
            int interval = DefaultTemplatePublicMethod.GetInterval(tempDT, ref index, ref tempStartTime, deviceDataID, dataWarning, isShowWarning,source);
            DateTime dataNullStartTime = DateTime.MinValue;
            int dataNullNuber = 0;
            var dt = DefaultTemplatePublicMethod.GetSameTimeVal(tempStartTime, endTime, deviceDataID, tempDT, interval, ref dataNullStartTime, ref dataNullNuber,startTime,false);

            if (dataNullNuber >= 1)
            {
                tipedList.Add(deviceDataID);
                DefaultTemplatePublicMethod.DataWarningNullFromTime(dataNullStartTime, deviceDataID, dataWarning, isShowWarning,source);
            }

            //增量类型电度值计算和差值计算不一致
            if (deviceDataID.DataID >= 4000429 && deviceDataID.DataID <= 4000433)
            {
                int rowNumber = 0;
                int count = rowList.Count;
                try
                {
                 
                    while (startTime < endTime)//累计值的不需要第一个时间点的数据
                    {
                        DateTime tempEndTime = DefaultTemplatePublicMethod.CaculateIntervelEndTimeByPeriodType(startTime, periodType);
                        int dataNullNumber = 0;
                        if (rowNumber >= count)
                            break;
                        DataRow dataRow = rowList[rowNumber];
                        var resultValue= DefaultTemplatePublicMethod.CaculateAcculateValue(startTime, tempEndTime, tempDT, ref dataNullNumber);
                        double tempResult;
                        if (double.TryParse(resultValue.ToString(), out tempResult) && (!tempResult.Equals(double.NaN)))
                        {
                            dataRow[columnN.ToString()] = tempResult;
                        }
                        else
                        {
                            dataRow[columnN.ToString()] = string.Empty;
                            if (!tipedList.Contains(deviceDataID))
                            {
                                DefaultTemplatePublicMethod.AddWarnings(deviceDataID, WarningKind.DataNullStartFromSomeTime, startTime,
                         dataWarning,
                         isShowWarning,source);
                                tipedList.Add(deviceDataID);
                            }
                    
                        }

                        startTime = startTime.AddHours(1);
                        ++rowNumber;
                    }
                }
                catch (System.Exception ex)
                {
                    DbgTrace.dout("{0}{1}", "HourlyUsageChartDataManager .QueryDataLogData catch (System.Exception ex): ", ex.Message);
                }
            }
            else
            {
                //      int dataNullNuber = 0;
                //    DateTime dataNullStartTime = DateTime.MinValue;
                //使用查询数据结果中的时间列中的每一行的时刻值，获取对应的原始数据值，放入对应的数据列
                List<DataLogOriDef> resultList = new List<DataLogOriDef>();
                var resultDT = DefaultTemplatePublicMethod.GetSameTimeVal(startTime, endTime, tempDT, periodType);

               
                //对结果集求差值
                for (int i = 0; i < resultDT.Count - 1; i++)
                {
                    //double tempValue1;
                    //if (!double.TryParse(resultDT[i].DataValue.ToString(), out tempValue1))
                    //{
                    //    DateTime time;
                    //    if (DateTime.TryParse(resultDT.Rows[i]["logTime"].ToString(), out time))
                    //    {
                    //        if (!tipedList.Contains(deviceDataID))
                    //        {
                    //            DefaultTemplatePublicMethod.AddWarnings(deviceDataID, WarningKind.DataNullStartFromSomeTime, time,
                    //         dataWarning,
                    //         isShowWarning,source);
                    //            tipedList.Add(deviceDataID);
                    //        }
                         
                    //    }
                    //}

                    if (!tipedList.Contains(deviceDataID))
                    {
                        DefaultTemplatePublicMethod.AddWarnings(deviceDataID, WarningKind.DataNullStartFromSomeTime, resultDT[i].LogTime,
                     dataWarning,
                     isShowWarning, source);
                        tipedList.Add(deviceDataID);
                    }

                    double value = DefaultTemplatePublicMethod.GetDiffValue(resultDT[i + 1].DataValue, resultDT[i].DataValue, deviceDataID);
                    resultList.Add(new DataLogOriDef(resultDT[i].LogTime, value));
                }
                int row = 0;
                foreach (var rowData in rowList)
                {
                    rowData[columnN.ToString()] = resultList[row].DataValue;
                    ++row;
                }
            }
            return true;
        }


        private static void GetFirstSecondeLineData(DefaultReportParameter parameter, NamesManager nameManager, int i,
            DataRow dr1, DataRow dr2)
        {
            string deviceName = string.Empty;
            nameManager.GetDeviceNameWithLoop(parameter.DeviceDataIDList[i], out deviceName);
            dr1[i + 1] = deviceName;

            string measureName = string.Empty;
            nameManager.GetMeasureName(parameter.DeviceDataIDList[i], out measureName);
            dr2[i + 1] = measureName;

            if (i == (parameter.DeviceDataIDList.Count - 1) && (parameter.IsIncludeAvg == true))
            {
                string average = LocalResourceManager.GetInstance().GetString("0558", "Average");
                dr1[i + 2] = average;
                dr2[i + 2] = average;
            }
        }

    
        private static void AddTableDataRow(DataTable resultDT, DateTime startTime,out List<DataRow> rowList)
        {
            DateTime tempStartTime = startTime.AddHours(1);
            DateTime temEndTime = startTime.AddDays(1);
            rowList = new List<DataRow>();
            string format = CommonluUsed.DataManager.TimeFormatWithoutSecond;
            while (tempStartTime <= temEndTime)
            {
                DataRow  resultRow = resultDT.NewRow();
                string temp0 = tempStartTime.AddHours(-1).ToString(format);
                string temp1 = tempStartTime.ToString(format);
                string temp = string.Format("{0} ~ {1}",temp0,temp1);
                resultRow[0] =temp ;
                resultDT.Rows.Add(resultRow);
                rowList.Add(resultRow);
                tempStartTime = tempStartTime.AddHours(1);
            }
        }

        #endregion

    
    }
}
