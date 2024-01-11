using System;
using System.Collections.Generic;
using System.Data;
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
    /// LoadProfileManager 的摘要说明
    /// </summary>
    public class LoadProfileManager: IDataSheet
    {
        private static uint source = (uint) RepServFileType.LoadProfile;
        /// <summary>
        /// 获取 LoadProfile 数据
        /// </summary>
        /// <param name="deviceIDs">设备ID</param>
        /// <param name="phaseType">相别</param>
        /// <param name="dataType">参数类型</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="resultDT">结果集</param>
        /// <returns>是否成功</returns>
        public List<DataTable> GetDataLogDatas(DefaultReportParameter parameter)
        {
            List<DataTable> resultDTList = new List<DataTable>();

            //警告信息内容
            DataTable warningDT = DefaultTemplatePublicMethod.ConstructWarningTable(parameter.IsIncludeWarning);
            //汇总信息内容表
            DataTable summaryDT = ConstructDataSummaryTable(parameter);
            DataTable resultDT = CreateLoadProfileDTColumns(parameter);
            GetLoadProfileTotalDT(parameter, resultDT, warningDT);
            resultDTList.Add(warningDT);
            resultDTList.Add(summaryDT);
            resultDTList.Add(resultDT);
            return resultDTList;
        }

        private DataTable CreateLoadProfileDTColumns(DefaultReportParameter parameter)
        {
            DataTable resultDT = new DataTable("LoadProfile");
            //设备ID
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("Logtime", "String"));
            //kw
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("kw", "String"));
            //kvar
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("kvar", "String"));
            //kVA
            resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("kVA", "String"));
            for (int i = 0; i < parameter.DeviceDataIDList.Count; i++)
            {
                //kw
                resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("kw" + i, "String"));
                //kvar
                resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("kvar" + i, "String"));
                //kVA
                resultDT.Columns.Add(PublicFunction.CreateDataTableColumn("kVA" + i, "String"));
            }

            DataRow dr = resultDT.NewRow();
            dr[0] = LocalResourceManager.GetInstance().GetString("0007", "Power");
            if (parameter.IsDemand)
                dr[0] = LocalResourceManager.GetInstance().GetString("0008", "Demand");
            //创建数据表头设备节点名称
            for (int i = 0; i < parameter.DeviceDataIDList.Count; i++)
                dr[i * 3 + 4] = DefaultTemplatePublicMethod.GetDeviceNameByDeviceDataID(parameter.DeviceDataIDList[i], (uint)RepServFileType.LoadProfile);
            resultDT.Rows.Add(dr);

            dr = resultDT.NewRow();
            dr[1] = "kW";
            dr[2] = "kvar";
            dr[3] = "kVA";
            for (int i = 0; i < parameter.DeviceDataIDList.Count; i++)
            {
                dr[4 + i * 3] = "kW";
                dr[5 + i * 3] = "kvar";
                dr[6 + i * 3] = "kVA";
            }
            resultDT.Rows.Add(dr);
            return resultDT;
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

            //起始结束时间
            DataRow resultRow = resultDT.NewRow();
            resultRow[0] = DataManager.GetStartTimeSting(parameter.StartTime);
            resultRow[1] = DataManager.GetEndTimeString(parameter.EndTime); resultDT.Rows.Add(resultRow);
            //空行
            resultRow = resultDT.NewRow();
            resultDT.Rows.Add(resultRow);
            //警告信息时间格式
            resultRow = resultDT.NewRow();
            resultRow[0] = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " HH:mm:ss";
            resultRow[1] = "Date Added";
            resultDT.Rows.Add(resultRow);

            //数据坐标轴时间格式
            resultRow = resultDT.NewRow();
            resultRow[0] = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " HH:mm";
            resultRow[1] = "Curve Time";
            resultDT.Rows.Add(resultRow);

            //数据格式
            resultRow = resultDT.NewRow();
            resultRow[0] = "#" + NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator + "##0" + NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator + "00";
            resultRow[1] = "Curve Data";
            resultDT.Rows.Add(resultRow);
            //空行
            resultRow = resultDT.NewRow();
            resultDT.Rows.Add(resultRow);
            //空行
            resultRow = resultDT.NewRow();
            resultDT.Rows.Add(resultRow);
            //空行
            resultRow = resultDT.NewRow();
            resultDT.Rows.Add(resultRow);
            //空行
            resultRow = resultDT.NewRow();
            resultDT.Rows.Add(resultRow);
            //设备数
            resultRow = resultDT.NewRow();
            resultRow[0] = "DevicesNumber";
            resultDT.Rows.Add(resultRow);
            //设备数
            resultRow = resultDT.NewRow();
            resultRow[0] = parameter.DeviceDataIDList.Count;
            resultDT.Rows.Add(resultRow);
            //空行
            resultRow = resultDT.NewRow();
            resultDT.Rows.Add(resultRow);
            //设备
            resultRow = resultDT.NewRow();
            resultRow[0] = "Devices";
            resultDT.Rows.Add(resultRow);

            for (int i = 0; i < parameter.DeviceDataIDList.Count; i++)
            {
                resultRow = resultDT.NewRow();
                resultRow[0] = DefaultTemplatePublicMethod.GetDeviceNameByDeviceDataID(parameter.DeviceDataIDList[i],source);
                resultDT.Rows.Add(resultRow);
            }
            return resultDT;
        }

        /// <summary>
        /// 获取 LoadProfileTotal 数据
        /// </summary>
        /// <param name="deviceIDs">设备ID</param>
        /// <param name="dataType">参数类型</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="resultDT">结果集</param>
        /// <returns>是否成功</returns>
        public bool GetLoadProfileTotalDT(DefaultReportParameter parameter, DataTable resultDT, DataTable dataWarning)
        {
            //先求单个设备的负载
            GetDeviceLoadProfile(parameter, resultDT, dataWarning);
            //再求汇总
            GetTotalLoadProfile(resultDT);
            return true;
        }

        private void GetDeviceLoadProfile(DefaultReportParameter parameter, DataTable resultDT, DataTable dataWarning)
        {
            List<DeviceDataIDDef> queryParamList = CreateQueryParas(parameter.DeviceDataIDList, parameter.IsDemand);
            //DataIDToMeasIDDef resultMapDef = DataIDToMeasIDDef.InvalidMapDef;
            int index = 0;
            DateTime tempStartTime = parameter.StartTime;
            //根据传入的DeviceID和DataID进行组合，形成查询条件列表
            int interval = 3600;
            for (int i = 0; i < queryParamList.Count; i++)
            {
                DataAcquisitionDatalog dataAcquisitionDatalog = new DataAcquisitionDatalog();
                if (!dataAcquisitionDatalog.HasDataMap(queryParamList[i]))
                {
                    DefaultTemplatePublicMethod.AddWarnings(queryParamList[i], WarningKind.MapNotExist, dataWarning, parameter.IsIncludeWarning, source);
                    continue;
                }
                //bool result = ReportWebServiceManager.ReportWebManager.FindDataMapDef(queryParamList[i], out resultMapDef);
                //if (!result)
                //{
                //    DefaultTemplatePublicMethod.AddWarnings(queryParamList[i],WarningKind.SourceIdNotExist , dataWarning, parameter.IsIncludeWarning,source);
                //    continue;
                //}
                //DataTable tempDT = new DataTable();
                //int maxRowCount = Convert.ToInt32(SysConstDefinition.DefaultMaxRowCount);
                //int errorCode = DefaultTemplatePublicMethod.ReadDatalogs(
                //    DBOperationFlag.either, resultMapDef.StationID, resultMapDef.SourceID, resultMapDef.DataIndex,
                //    parameter.StartTime, parameter.EndTime.AddSeconds(1), (int)SysConstDefinition.DefaultMaxRowCount,
                //    ref tempDT, queryParamList[i]);

                //if (errorCode != 0)
                //{
                //    DefaultTemplatePublicMethod.AddWarnings(queryParamList[i], DBInterfaceCommonLib.ErrorQuerier.Instance.GetLastErrorString(), dataWarning, parameter.IsIncludeWarning,source);
                //    continue;
                //}
                //if (tempDT.Rows.Count == 0)
                //{
                //    DefaultTemplatePublicMethod.AddWarnings(queryParamList[i],WarningKind.DataIsNull , dataWarning, parameter.IsIncludeWarning,source);
                //    continue;
                //}
                //if (tempDT.Rows.Count >= DefaultTemplatePublicMethod.MAX_COUNT)
                //{
                //    var temp = queryParamList[i];
                //    DefaultTemplatePublicMethod.AddWarnings(temp, WarningKind.DataOver10000Rows, dataWarning, parameter.IsIncludeWarning, source);
                //}
                DATALOG_PRIVATE_MAP resultMapDef;
                bool result = ReportWebServiceManager.ReportWebManager.FindDataMapDef(parameter.DeviceDataIDList[i], out resultMapDef);
                if (!result)
                {
                    DefaultTemplatePublicMethod.AddWarnings(queryParamList[i], WarningKind.SourceIdNotExist, dataWarning, parameter.IsIncludeWarning, source);
                    continue;
                }
                List<DataLogOriDef> tempDT = new List<DataLogOriDef>();
                bool isSucess = DefaultTemplatePublicMethod.ReadDatalogs(resultMapDef, parameter.StartTime, parameter.EndTime.AddSeconds(1), parameter.DeviceDataIDList[i], ref tempDT);
                if (!isSucess)
                {
                    DefaultTemplatePublicMethod.AddWarnings(queryParamList[i], DBInterfaceCommonLib.ErrorQuerier.Instance.GetLastErrorString(), dataWarning, parameter.IsIncludeWarning, source);
                    continue;
                }
                if(tempDT.Count == 0)
                {
                    DefaultTemplatePublicMethod.AddWarnings(queryParamList[i], WarningKind.DataIsNull, dataWarning, parameter.IsIncludeWarning, source);
                    continue;
                }
                if (tempDT.Count >= DefaultTemplatePublicMethod.MAX_COUNT)
                {
                    var temp = queryParamList[i];
                    DefaultTemplatePublicMethod.AddWarnings(temp, WarningKind.DataOver10000Rows, dataWarning, parameter.IsIncludeWarning, source);
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
                var dt = DefaultTemplatePublicMethod.GetSameTimeVal(tempStartTime, parameter.EndTime, queryParamList[i], tempDT, interval, ref dataNullStartTime, ref dataNullNuber,parameter.StartTime,true);
                if (dataNullNuber >= 1)
                {
                 
                    DefaultTemplatePublicMethod.AddWarnings(queryParamList[i], WarningKind.DataNullStartFromSomeTime,dataNullStartTime, dataWarning, parameter.IsIncludeWarning,source);
                }
                for (int j = 0; j < dt.Count; j++)
                {
                    resultDT.Rows[j + 2][i + 4] = dt[j].DataValue;
                }
                index++;
            }
        }

        private static void GetTotalLoadProfile(DataTable resultDT)
        {
            for (int i = 2; i < resultDT.Rows.Count; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    double columnTotal = double.NaN;
                    for (int j = 4 + k; j < resultDT.Columns.Count; j += 3)
                    {
                        double tempValue = double.NaN;
                        if (double.TryParse(resultDT.Rows[i][j].ToString(), out tempValue))
                        {
                            if (double.IsNaN(columnTotal))
                                columnTotal = 0;
                            columnTotal += tempValue;
                        }
                    }
                    if (!double.IsNaN(columnTotal))
                        resultDT.Rows[i][k + 1] = columnTotal;
                }
            }
        }

        #region 私有逻辑函数

        /// <summary>
        /// 创建总功率查询参数
        /// </summary>
        /// <param name="deviceID">设备ID</param>
        /// <param name="dataType">参数类型</param>
        /// <param name="queryParamList">查询参数列表</param>
        private List<DeviceDataIDDef> CreateQueryParas(List<DeviceDataIDDef> deviceDataIDList, bool isDemand)
        {
            List<DeviceDataIDDef> queryParamList = new List<DeviceDataIDDef>();
            for (int i = 0; i < deviceDataIDList.Count; i++)
            {
                DeviceDataIDDef deviceDataID = deviceDataIDList[i];
                int dataType = deviceDataID.DataTypeID;
                uint deviceID = deviceDataID.DeviceID;
                uint nodeType = deviceDataID.NodeType;
                if (isDemand)
                {
                    DeviceDataIDDef newParam = new DeviceDataIDDef(nodeType,deviceID, SysConstDefinition.DATAIDKW_DEMAND, (int)DataIDParaType.DatalogType, dataType, deviceDataID.LogicalDeviceIndex);
                    queryParamList.Add(newParam);
                    newParam = new DeviceDataIDDef(nodeType, deviceID, SysConstDefinition.DATAIDKVAR_DEMAND, (int)DataIDParaType.DatalogType, dataType, deviceDataID.LogicalDeviceIndex);
                    queryParamList.Add(newParam);
                    newParam = new DeviceDataIDDef(nodeType, deviceID, SysConstDefinition.DATAIDKVA_DEMAND, (int)DataIDParaType.DatalogType, dataType, deviceDataID.LogicalDeviceIndex);
                    queryParamList.Add(newParam);
                }
                else
                {
                    DeviceDataIDDef newParam = new DeviceDataIDDef(nodeType, deviceID, SysConstDefinition.DATAIDKWTOTAL, (int)DataIDParaType.DatalogType, dataType, deviceDataID.LogicalDeviceIndex);
                    queryParamList.Add(newParam);
                    newParam = new DeviceDataIDDef(nodeType, deviceID, SysConstDefinition.DATAIDKVARTOTAL, (int)DataIDParaType.DatalogType, dataType, deviceDataID.LogicalDeviceIndex);
                    queryParamList.Add(newParam);
                    newParam = new DeviceDataIDDef(nodeType, deviceID, SysConstDefinition.DATAIDKVATOTAL, (int)DataIDParaType.DatalogType, dataType, deviceDataID.LogicalDeviceIndex);
                    queryParamList.Add(newParam);
                }
            }
            return queryParamList;
        }
        #endregion
    }
}
