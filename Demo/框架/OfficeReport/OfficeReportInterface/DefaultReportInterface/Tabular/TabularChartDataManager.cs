using System;
using System.Collections.Generic;
using System.Data;
using CSharpDBPlugin;
using DBInterfaceCommonLib;
using OfficeReportInterface.DefaultReportInterface.CommonluUsed;
using OfficeReportInterface.DefaultReportInterface.EnergyCost;
using OfficeReportInterface.DefaultReportInterface.PowerQualityEventsOnly;
using ErrorQuerier = DBInterfaceCommonLib.ErrorQuerier;

namespace OfficeReportInterface.DefaultReportInterface.Tabular
{
    class TabularChartDataManager:IDataSheet
    {
        private static uint source = (uint) RepServFileType.Tabular;
       
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
            DataTable resultDT = ConstructRealDataTable(parameter.StartTime, parameter.EndTime, parameter.DeviceDataIDList);
            //根据传入的参数填充警告表，汇总表，数据表
            FillWarmSummaryDataToTable(parameter, warningDT, resultDT);
            resultDTList.Add(warningDT);
            resultDTList.Add(summaryDT);
            resultDTList.Add(resultDT);
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

            for (int i = 0; i < paraList.Count; i++)
            {
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = i.ToString();
                resultDT.Columns.Add(column);
            }

            DataRow dr = resultDT.NewRow();
            dr[0] = LocalResourceManager.GetInstance().GetString("0557", "Timestamp");
            NamesManager nameManager = NamesManager.GetInstance((uint)RepServFileType.Tabular);
            for (int i = 0; i < paraList.Count; i++)
            {
                string deviceName=string.Empty;
                nameManager.GetDeviceNameWithLoop(paraList[i], out deviceName);
                dr[i + 1] = deviceName;
            }
            resultDT.Rows.Add(dr);

            dr = resultDT.NewRow();
            dr[0] = "Parameters";
            for (int i = 0; i < paraList.Count; i++)
            {
                string measureName = string.Empty;
                nameManager.GetMeasureName(paraList[i], out measureName);
                dr[i + 1] = measureName;
            }
            resultDT.Rows.Add(dr);



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
                var namesManager = NamesManager.GetInstance((uint)RepServFileType.Tabular);
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
                DbgTrace.dout(ex.Message + ex.StackTrace);
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
          
            nameList.Add(LocalResourceManager.GetInstance().GetString("0526", "Tabular Report"));
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

        private static void FillWarmSummaryDataToTable(DefaultReportParameter parameter, DataTable dataWarning, DataTable resultDT)
        {
            DateTime tempStartTime = parameter.StartTime;
            int index = 0;
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
                    DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i],WarningKind.SourceIdNotExist , dataWarning, parameter.IsIncludeWarning,source
                        );
                    continue;
                }
               // DataTable tempDT = new DataTable();
               //// int maxRowCount = Convert.ToInt32(SysConstDefinition.DefaultMaxRowCount);

               // int errorCode = DefaultTemplatePublicMethod.ReadDatalogs(
               //     DBOperationFlag.either, resultMapDef.StationID, resultMapDef.SourceID, resultMapDef.DataIndex, parameter.StartTime,
               //     parameter.EndTime.AddSeconds(1), (int)SysConstDefinition.DefaultMaxRowCount,
               //     ref tempDT, parameter.DeviceDataIDList[i]);
               // //int errorCode = PECSDBInterface.DatalogProvider.Instance.ReadDatalogs(
               // //    DBOperationFlag.either, resultMapDef.StationID, resultMapDef.SourceID, resultMapDef.DataIndex, parameter.StartTime,
               // //    parameter.EndTime.AddSeconds(1), (int)SysConstDefinition.DefaultMaxRowCount,
               // //    ref tempDT);
               // //DbgTrace.dout("调用数据库接口，查询定时记录。返回的错误字符串是:{0}；返回的错误码的值是：{1}。", DBInterfaceCommonLib.ErrorQuerier.Instance.GetLastErrorString(), errorCode);
               // //DbgTrace.dout("传入数据库接口的入参是：DBOperationFlag = {0} , stationID = {1} , sourceID = {2} , paraIndex = {3}, startTime = {4} , endTime.AddSeconds(1) = {5} , Convert.ToInt32(SysConstDefinition.DefaultMaxRowCount) = {6} 。",
               // //      DBOperationFlag.either, resultMapDef.StationID, resultMapDef.SourceID, resultMapDef.DataIndex, parameter.StartTime,
               // //    parameter.EndTime.AddSeconds(1), (int)SysConstDefinition.DefaultMaxRowCount);
              
               // if (errorCode != 0)
               // {
               //     DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], ErrorQuerier.Instance.GetLastErrorString(), dataWarning, parameter.IsIncludeWarning,source);
               //     continue;
               // }
               // if (tempDT.Rows.Count == 0)
               // {
               //     DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], WarningKind.DataIsNull, dataWarning, parameter.IsIncludeWarning,source);
               //     continue;
               // }
               // if (tempDT.Rows.Count >= DefaultTemplatePublicMethod.MAX_COUNT)
               // {
               //     DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], WarningKind.DataOver10000Rows, dataWarning, parameter.IsIncludeWarning,source);
               // }
                List<DataLogOriDef> tempDT = new List<DataLogOriDef>();
                bool isSucess = DefaultTemplatePublicMethod.ReadDatalogs(resultMapDef, parameter.StartTime, parameter.EndTime.AddSeconds(1), parameter.DeviceDataIDList[i], ref tempDT);
                if (!isSucess)
                {
                    DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], ErrorQuerier.Instance.GetLastErrorString(), dataWarning, parameter.IsIncludeWarning, source);
                    continue;
                }
                if (tempDT.Count == 0)
                {
                    DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], WarningKind.DataIsNull, dataWarning, parameter.IsIncludeWarning, source);
                    continue;
                }
                if (tempDT.Count >= DefaultTemplatePublicMethod.MAX_COUNT)
                {
                    DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], WarningKind.DataOver10000Rows, dataWarning, parameter.IsIncludeWarning, source);
                }
                if (index == 0)
                {
                    interval = DefaultTemplatePublicMethod.GetInterval(tempDT, ref index, ref tempStartTime, parameter.DeviceDataIDList[i], dataWarning, parameter.IsIncludeWarning,source);
                    if (tempDT.Count >= DefaultTemplatePublicMethod.MAX_COUNT)
                        parameter.EndTime = Convert.ToDateTime(tempDT[tempDT.Count - 1].LogTime);
                    List<string> warningMessageList=new List<string>();
                    if (index != -1)
                        DefaultTemplatePublicMethod.ConstrutDataTable(tempStartTime, parameter.EndTime, resultDT, interval,out  warningMessageList);
                    foreach(var itemMessage in warningMessageList)
                    {
                        DefaultTemplatePublicMethod.AddDataWarning(itemMessage, dataWarning, parameter.IsIncludeWarning);
                    }
                }
                DateTime dataNullStartTime = DateTime.MinValue;
                int dataNullNuber = 0;
                var dt = DefaultTemplatePublicMethod.GetSameTimeVal(tempStartTime, parameter.EndTime, parameter.DeviceDataIDList[i], tempDT, interval, ref dataNullStartTime, ref dataNullNuber,parameter.StartTime,true);
                //tempDT.Dispose();
                if (dataNullNuber >= 1)
                {
                    DefaultTemplatePublicMethod.DataWarningNullFromTime(dataNullStartTime, parameter.DeviceDataIDList[i], dataWarning,
                                parameter.IsIncludeWarning,source);
                }
                for (int ia = 0; ia < dt.Count; ++ia)
                {
                    //空行
                    var resultRow = resultDT.NewRow();
                    resultDT.Rows.Add(resultRow);
                }
                for (int j = 0; j < dt.Count; j++)
                {
                    resultDT.Rows[j + 2][0] = dt[j].LogTime;
                    resultDT.Rows[j + 2][i + 1] = dt[j].DataValue;
                }
                //dt.Dispose();
                index++;
            }


            while (true)
            {
                if (resultDT.Rows.Count == 0)
                    break;
                DataRow row = resultDT.Rows[resultDT.Rows.Count - 1];
                string time = row[0].ToString();
                if (!IsNeededRow(time, parameter.EndTime))
                {
                    resultDT.Rows.RemoveAt(resultDT.Rows.Count - 1);
                }
                else
                {
                    break;
                }
            }

            GC.Collect();

        }


        private static bool IsNeededRow(string time,DateTime endTime)
        {
            DateTime lastTime;
            if (!DateTime.TryParse(time, out lastTime))
                return false;
            if (endTime <= lastTime)
                return false;
            return true;
        }

        #endregion
    }
}
