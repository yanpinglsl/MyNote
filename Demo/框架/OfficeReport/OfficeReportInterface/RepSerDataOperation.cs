using System;
using System.Collections.Generic;
using System.Data;
using CET.PecsNodeManage;
using CSharpDBPlugin;
using OfficeReportInterface.DefaultReportInterface;
using OfficeReportInterface.DefaultReportInterface.EnergyCost;
using OfficeReportInterface.DefaultReportInterface.HourlyUsage;
using OfficeReportInterface.DefaultReportInterface.IntelligentSafety;
using OfficeReportInterface.DefaultReportInterface.PowerQualityEventsOnly;
using OfficeReportInterface.DefaultReportInterface.Tabular;
using OfficeReportInterface.ReadingIniFile;

namespace OfficeReportInterface
{
    public class RepSerDataOperation
    {
        private static RepSerDataOperation repserdataoperation = null;

        /// <summary>
        /// DeviceID dataID组合最大个数
        /// </summary>
        private static int _maxCountDeviceDataId = 30;

        /// <summary>
        /// 是否已经从配置文件读取
        /// </summary>
        private static bool _hasReadMaxCountDeviceDataId = false;

        /// <summary>
        /// DeviceID dataID组合最大个数
        /// </summary>
        public static int MaxCountDeviceDataId
        {
            get
            {
                if (_hasReadMaxCountDeviceDataId)
                    return _maxCountDeviceDataId;
                try
                {
                    var iniFile = INIFile.GetIniFileByType(INIFile.IniFileType.OfficeReportIni);
                    var count = iniFile.ReadInt("COUNT_CONFIG", "DEVICE_DATA_MAX_COUNT");
                    if (count > 0)
                        _maxCountDeviceDataId = count;
                }
                catch (Exception ex)
                {
                    ErrorInfoManager.Instance.WriteLogMessage(ex.Message + ex.StackTrace);
                }
                finally
                {
                    _hasReadMaxCountDeviceDataId = true;
                }

                return _maxCountDeviceDataId;
            }
        }

        public static RepSerDataOperation GetInstance()
        {
            if (repserdataoperation == null)
                repserdataoperation = new RepSerDataOperation();
            return repserdataoperation;
        }

        /// <summary>
        /// 查询前初始化web接口
        /// </summary>
        /// <returns></returns>
        public static bool InitializeService(string path)
        {
            try
            {
                if (!ReportWebServiceManager.ReportWebManager.InitialSuccess)
                {
                    ReportWebServiceManager.CurDBSourceInfo = path;
                    try
                    {
                        ErrorMsg errorMessage = ReportWebServiceManager.ReportWebManager.InitializeWebService();
                        DbgTrace.dout("{0}{1}{2}{3}{4}{5}", System.Reflection.MethodBase.GetCurrentMethod(), " ReportWebServiceManager.ReportWebManager.InitializeWebService() ", errorMessage.IsSuccess, errorMessage.ErrorMsgInstance, errorMessage.ErrorMessage, errorMessage.ErrorDescription);
                        return errorMessage.IsSuccess;
                    }
                    catch (Exception ex)
                    {
                        DbgTrace.dout(ex.Message + ex.StackTrace);
                        return false;
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 获取reporting service报表数据
        /// </summary>
        /// <param name="filetype"></param>
        /// <param name="datasourcelabel"></param>
        /// <returns></returns>
        public static List<DataTable> GetRepSerData(DataSourceLabel datasourcelabel, out string errorMessage, uint userID, string userName)
        {
            errorMessage = string.Empty;
            try
            {
                DefaultReportParameter parameter = new DefaultReportParameter();
                parameter.StartTime = datasourcelabel.startTime;
                parameter.EndTime = datasourcelabel.endTime;
                parameter.PeriodType = datasourcelabel.entireRSParms.periodType;
                parameter.CompareNumber = datasourcelabel.entireRSParms.compareNumber;
                parameter.DeviceDataIDList = DefaultTemplatePublicMethod.ConstructParaList(datasourcelabel.entireRSParms.deviceIDs);
                parameter.IsIncludeWarning = datasourcelabel.entireRSParms.isCludeWarning;
                parameter.DateTimeList = datasourcelabel.entireRSParms.dateTimeList;
                parameter.IsIncludeTable = datasourcelabel.entireRSParms.isCludeDataTable;
                parameter.Interval = datasourcelabel.entireRSParms.intervalType;
                parameter.IsDemand = datasourcelabel.entireRSParms.isDemand;
                parameter.IsITIC = datasourcelabel.entireRSParms.isItic;
                parameter.curveID = datasourcelabel.entireRSParms.curveID;
                parameter.IsIncludeAvg = datasourcelabel.entireRSParms.isIncludeAvg;
                parameter.IsIncludeTotal = datasourcelabel.entireRSParms.isIncludeTotal;
                parameter.kWh = datasourcelabel.entireRSParms.kWh;
                parameter.kvarh = datasourcelabel.entireRSParms.kvarh;
                parameter.kVAh = datasourcelabel.entireRSParms.kVAh;
                parameter.kWDemand = datasourcelabel.entireRSParms.kWDemand;
                parameter.kvarDemand = datasourcelabel.entireRSParms.kvarDemand;
                parameter.kVADemand = datasourcelabel.entireRSParms.kVADemand;
                parameter.userID = userID;
                parameter.userName = userName;
                PrintLogs(parameter);

                if (datasourcelabel.source == (uint) RepServFileType.Safety) //智慧安全报表不需要做重名判断
                {
                    return SafetyDataManager.GetStatisticDatas(parameter);
                }
                else
                {
                    //检查数据的合法性
                    if (!CheckLegal(datasourcelabel, ref errorMessage, parameter))
                        return new List<DataTable>();

                    if (datasourcelabel.source == (uint) RepServFileType.EnergyCost)
                    {
                        parameter.TOUProfile = datasourcelabel.entireRSParms.TouProfile;
                        if (parameter.TOUProfile.dayProfileList.Count == 0)
                        {
                            TOURateParser node = new TOURateParser();
                            NewTOUProfile newNode;
                            if (node.GetTOURateByID(parameter.TOUProfile.scheduleID, out newNode))
                                parameter.TOUProfile = newNode;
                        }
                    }

                    //switch (datasourcelabel.source)
                    //{
                    //    //case (uint)RepServFileType.Type100ms:
                    //    //    return Get100msReportData(datasourcelabel);
                    //    //case (uint)RepServFileType.EnergyDemand:
                    //    //    return GetEnergyDemandData(datasourcelabel);
                    //    //case (uint)RepServFileType.SystemConfig:
                    //    //    return GetSystemConfiguationData(datasourcelabel);
                    //}

                    var obj = GetObject(datasourcelabel.source);
                    if (obj == null)
                        return null;
                    if (datasourcelabel.source == (uint) RepServFileType.EventHistory)
                    {
                        parameter.StationChannelDeviceList = GetNodeList(datasourcelabel.entireRSParms.deviceIDs);
                    }
                    List<LogicalDeviceIndex> deviceNodeList = new List<LogicalDeviceIndex>();
                    var deviceDataIDDefList = parameter.DeviceDataIDList;
                    foreach (var item in deviceDataIDDefList)
                    {
                        var deviceIndex = new LogicalDeviceIndex(item.NodeType, item.DeviceID, item.LogicalDeviceIndex);
                        if (deviceNodeList.Contains(deviceIndex))
                            continue;
                        deviceNodeList.Add(deviceIndex);
                    }
                    NamesManager nameManager = NamesManager.GetInstance(datasourcelabel.source);
                    nameManager.HasSameDeviceName(deviceNodeList);
                    return obj.GetDataLogDatas(parameter);
                }
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                errorMessage = LocalResourceManager.GetInstance().GetString("0277", "报表生成失败.");
                return new List<DataTable>();
            }
        }

        private static bool CheckLegalForEnergyCost(DataSourceLabel datasourcelabel, ref string errorMessage,
            DefaultReportParameter parameter)
        {
            List<LogicalDeviceIndex> deviceNodeList;
            GetDeviceNodeList(parameter, out deviceNodeList);

            if (!SelectDevice(ref errorMessage, deviceNodeList)) return false;

            if (!SelectParameter(ref errorMessage, parameter)) return false;

            if (!LessThan30(ref errorMessage, parameter)) return false;
            if (!StartTimeEarlierThanEndTime(ref errorMessage, parameter))
                return false;
            if (!NoTouRate(datasourcelabel, ref errorMessage)) return false;

            return true;
        }

        private static bool CheckLegalForHourlyUsage(DataSourceLabel datasourcelabel, ref string errorMessage,
            DefaultReportParameter parameter)
        {
            List<LogicalDeviceIndex> deviceNodeList;
            GetDeviceNodeList(parameter, out deviceNodeList);
            if (!SelectDevice(ref errorMessage, deviceNodeList)) return false;
            if (!SelectParameter(ref errorMessage, parameter)) return false;
            if (!LessThan30(ref errorMessage, parameter)) return false;
            return true;
        }

        private static bool CheckLegalForEnergyPeriod(DataSourceLabel datasourcelabel, ref string errorMessage,
            DefaultReportParameter parameter)
        {
            List<LogicalDeviceIndex> deviceNodeList;
            GetDeviceNodeList(parameter, out deviceNodeList);
            if (!SelectDevice(ref errorMessage, deviceNodeList)) return false;
            if (!SelectParameter(ref errorMessage, parameter)) return false;
            if (!LessThan30(ref errorMessage, parameter)) return false;
            if (!StartTimeEarlierThanEndTime(ref errorMessage, parameter))
                return false;
            return true;
        }

        private static bool CheckLegalForEventHistory(DataSourceLabel datasourcelabel, ref string errorMessage,
            DefaultReportParameter parameter)
        {
            if (!StartTimeEarlierThanEndTime(ref errorMessage, parameter))
                return false;
            if (!SelectEventClass(datasourcelabel, ref errorMessage, parameter)) return false;
            return true;
        }

        private static bool CheckLegalForPowerQualityOrLoadProfile(DataSourceLabel datasourcelabel, ref string errorMessage, DefaultReportParameter parameter)
        {
            List<LogicalDeviceIndex> deviceNodeList;
            GetDeviceNodeList(parameter, out deviceNodeList);
            if (!SelectDevice(ref errorMessage, deviceNodeList))
                return false;
            if (!SelectParameter(ref errorMessage, parameter))
                return false;
            if (!LessThan30(ref errorMessage, parameter))
                return false;
            if (!StartTimeEarlierThanEndTime(ref errorMessage, parameter))
                return false;
            return true;
        }

        private static bool CheckLegalForTabular(DataSourceLabel datasourcelabel, ref string errorMessage, DefaultReportParameter parameter)
        {
            List<LogicalDeviceIndex> deviceNodeList;
            GetDeviceNodeList(parameter, out deviceNodeList);
            if (!SelectDevice(ref errorMessage, deviceNodeList))
                return false;
            if (!SelectParameter(ref errorMessage, parameter))
                return false;
            if (!LessThan30(ref errorMessage, parameter))
                return false;
            if (!StartTimeEarlierThanEndTime(ref errorMessage, parameter))
                return false;
            if (!LessThan366Day(datasourcelabel, ref errorMessage, parameter))
                return false;
            return true;
        }

        private static bool CheckLegalForTrend(DataSourceLabel datasourcelabel, ref string errorMessage, DefaultReportParameter parameter)
        {
            List<LogicalDeviceIndex> deviceNodeList;
            GetDeviceNodeList(parameter, out deviceNodeList);
            if (!SelectDevice(ref errorMessage, deviceNodeList))
                return false;
            if (!SelectParameter(ref errorMessage, parameter))
                return false;
            if (!LessThan30(ref errorMessage, parameter))
                return false;
            if (!StartTimeEarlierThanEndTime(ref errorMessage, parameter))
                return false;
            return true;
        }

        private static bool CheckLegalForDeviceUsage(DataSourceLabel datasourcelabel, ref string errorMessage, DefaultReportParameter parameter)
        {
            List<LogicalDeviceIndex> deviceNodeList;
            GetDeviceNodeList(parameter, out deviceNodeList);
            if (!SelectDevice(ref errorMessage, deviceNodeList))
                return false;
            if (!SelectParameter(ref errorMessage, parameter))
                return false;
            if (!LessThan30(ref errorMessage, parameter))
                return false;
            if (!UseSingleReportForSingleDevice(datasourcelabel, ref errorMessage, deviceNodeList)) return false;
            return true;
        }

        private static bool CheckLegal(DataSourceLabel datasourcelabel, ref string errorMessage, DefaultReportParameter parameter)
        {
            //如果是普通模板
            if (datasourcelabel.source < (uint) RepServFileType.MinumOfTemplate)
                return true;
            if (datasourcelabel.source == (uint) RepServFileType.EnergyCost)
            {
                return CheckLegalForEnergyCost(datasourcelabel, ref errorMessage, parameter);
            }
            if (datasourcelabel.source == (uint) RepServFileType.EnergyPeriod)
            {
                return CheckLegalForEnergyPeriod(datasourcelabel, ref errorMessage,
                    parameter);
            }
            if (datasourcelabel.source == (uint) RepServFileType.EventHistory)
            {
                return CheckLegalForEventHistory(datasourcelabel, ref errorMessage, parameter);
            }
            if (datasourcelabel.source == (uint) RepServFileType.HourlyUsage)
            {
                return CheckLegalForHourlyUsage(datasourcelabel, ref errorMessage, parameter);
            }

            if (datasourcelabel.source == (uint) RepServFileType.PowerQuality ||
                datasourcelabel.source == (uint) RepServFileType.PowerQualityEventsOnly ||
                datasourcelabel.source == (uint) RepServFileType.LoadProfile)
            {
                return CheckLegalForPowerQualityOrLoadProfile(datasourcelabel, ref errorMessage,
                    parameter);
            }

            if (datasourcelabel.source == (uint) RepServFileType.Tabular)
            {
                return CheckLegalForTabular(datasourcelabel, ref errorMessage,
                    parameter);
            }
            if (datasourcelabel.source == (uint) RepServFileType.Trend)
            {
                CheckLegalForTrend(datasourcelabel, ref errorMessage, parameter);
            }
            if (datasourcelabel.source == (uint) RepServFileType.SingleUsage ||
                datasourcelabel.source == (uint) RepServFileType.MultiUsage)
            {
                return CheckLegalForDeviceUsage(datasourcelabel, ref errorMessage, parameter);
            }

            return true;
        }

        private static bool LessThan366Day(DataSourceLabel datasourcelabel, ref string errorMessage,
            DefaultReportParameter parameter)
        {
            if (datasourcelabel.source == (uint) RepServFileType.Tabular &&
                (parameter.EndTime - parameter.StartTime) > (new TimeSpan(366*24, 0, 0)))
            {
                errorMessage = LocalResourceManager.GetInstance()
                    .GetString("0586", "Query time period must be less than or equal to 366 days.");

                return false;
            }
            return true;
        }

        private static bool NoTouRate(DataSourceLabel datasourcelabel, ref string errorMessage)
        {
            if (datasourcelabel.source == (uint) RepServFileType.EnergyCost)
            {
                var touProfile = datasourcelabel.entireRSParms.TouProfile;
                if (touProfile == null)
                {
                    errorMessage = LocalResourceManager.GetInstance().GetString("0580", "No TOU Rate!");
                    DbgTrace.dout(errorMessage);
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 获取词条。该词条在OfficeReportInterface.dll,OfficeReport.exe中使用，因此这里提取一个函数，避免重复
        /// </summary>
        /// <returns></returns>
        public static string GetLessThanTip()
        {
            return string.Format(LocalResourceManager.GetInstance().GetString("0581", "Device and parameters pairs should less than {0}. "), MaxCountDeviceDataId);
        }

        private static bool LessThan30(ref string errorMessage, DefaultReportParameter parameter)
        {
            if (parameter.DeviceDataIDList.Count >= MaxCountDeviceDataId)
            {
                errorMessage = GetLessThanTip();
                DbgTrace.dout(errorMessage);
                {
                    return false;
                }
            }
            return true;
        }

        private static bool SelectEventClass(DataSourceLabel datasourcelabel, ref string errorMessage,
            DefaultReportParameter parameter)
        {
            if (parameter.Interval == 0 && datasourcelabel.source == (uint) RepServFileType.EventHistory)
            {
                errorMessage = LocalResourceManager.GetInstance().GetString("0584", "Please select one or more event class!");
                return false;
            }
            return true;
        }

        private static bool UseSingleReportForSingleDevice(DataSourceLabel datasourcelabel, ref string errorMessage,
            List<LogicalDeviceIndex> deviceNodeList)
        {
            if (datasourcelabel.source == (uint) RepServFileType.MultiUsage)
            {
                if (deviceNodeList.Count == 1)
                {
                    errorMessage = LocalResourceManager.GetInstance().GetString("0583", "Please use Single Device Usage Report for single device. ");
                    return false;
                }
            }
            return true;
        }

        private static bool hasParameters(DefaultReportParameter parameter)
        {
            if (parameter.kWh == DataIdChooser.NONE_ENERGY
                && parameter.kvarh == DataIdChooser.NONE_ENERGY
                && parameter.kVAh == DataIdChooser.NONE_ENERGY
                && parameter.kWDemand == DataIdChooser.NONE_DEMAND
                && parameter.kvarDemand == DataIdChooser.NONE_DEMAND
                && parameter.kVADemand == DataIdChooser.NONE_DEMAND)
                return false;
            return true;
        }

        private static bool SelectParameter(ref string errorMessage, DefaultReportParameter parameter)
        {
            if (parameter.DeviceDataIDList.Count == 0 || (!hasParameters(parameter)))
            {
                errorMessage = LocalResourceManager.GetInstance().GetString("0579", "Please select parameters.");
                DbgTrace.dout(errorMessage);
                {
                    return false;
                }
            }
            return true;
        }

        private static bool SelectDevice(ref string errorMessage, List<LogicalDeviceIndex> deviceNodeList)
        {
            if (deviceNodeList.Count == 0)
            {
                errorMessage = LocalResourceManager.GetInstance().GetString("0585", "Please select device.");
                return false;
            }
            return true;
        }

        private static void GetDeviceNodeList(DefaultReportParameter parameter, out List<LogicalDeviceIndex> deviceNodeList)
        {
            deviceNodeList = new List<LogicalDeviceIndex>();
            foreach (var item in parameter.DeviceDataIDList)
            {
                var ld = new LogicalDeviceIndex(item.NodeType, item.DeviceID, item.LogicalDeviceIndex);
                if (!deviceNodeList.Contains(ld))
                    deviceNodeList.Add(ld);
            }
        }

        private static bool StartTimeEarlierThanEndTime(ref string errorMessage, DefaultReportParameter parameter)
        {
            if (parameter.EndTime < parameter.StartTime)
            {
                errorMessage = LocalResourceManager.GetInstance()
                    .GetString("0582", "Start time should be earlier than end time! ");
                DbgTrace.dout(errorMessage);
                {
                    return false;
                }
            }
            return true;
        }

        private static void PrintLogs(DefaultReportParameter parameter)
        {
            DbgTrace.dout("下面列出外界传入的参数：");
            try
            {
                DbgTrace.dout("parameter.StartTime ={0}", parameter.StartTime.ToString());
            }
            catch (System.Exception ex)
            {
            }
            try
            {
                DbgTrace.dout("parameter.EndTime ={0}", parameter.EndTime.ToString());
            }
            catch (System.Exception ex)
            {
            }
            try
            {
                DbgTrace.dout("parameter.PeriodType ={0}", parameter.PeriodType.ToString());
            }
            catch (System.Exception ex)
            {
            }
            try
            {
                DbgTrace.dout("parameter.CompareNumber ={0}", parameter.CompareNumber.ToString());
            }
            catch (System.Exception ex)
            {
            }
            try
            {
                DbgTrace.dout("传入的设备参数包括：");
                int count = 0;
                foreach (var item in parameter.DeviceDataIDList)
                {
                    ++count;
                    DbgTrace.dout("({0})parameter.DeviceDataIDList={1}", count, item.ToString());
                }
            }
            catch (System.Exception ex)
            {
            }
            try
            {
                DbgTrace.dout("parameter.IsIncludeWarning  ={0}", parameter.IsIncludeWarning.ToString());
            }
            catch (System.Exception ex)
            {
            }


            try
            {
                if (parameter.DateTimeList != null)
                {
                    DbgTrace.dout("DateTimeList包括：");
                    int count = 0;
                    foreach (var item in parameter.DateTimeList)
                    {
                        ++count;
                        DbgTrace.dout("({0}) {1}", count, item.ToString());
                    }
                }
            }
            catch (System.Exception ex)
            {
            }
            try
            {
                DbgTrace.dout("parameter.IsIncludeTable ={0}", parameter.IsIncludeTable.ToString());
            }
            catch (System.Exception ex)
            {
            }

            try
            {
                DbgTrace.dout("parameter.Interval ={0}", parameter.Interval.ToString());
            }
            catch (System.Exception ex)
            {
            }

            try
            {
                DbgTrace.dout("parameter.IsDemand ={0}", parameter.IsDemand.ToString());
            }
            catch (System.Exception ex)
            {
            }
            try
            {
                DbgTrace.dout("parameter.IsITIC={0}", parameter.IsITIC.ToString());
            }
            catch (System.Exception ex)
            {
            }
            try
            {
                DbgTrace.dout("parameter.IsIncludeAvg ={0}", parameter.IsIncludeAvg.ToString());
            }
            catch (System.Exception ex)
            {
            }
            try
            {
                DbgTrace.dout("parameter.IsIncludeTotal ={0}", parameter.IsIncludeTotal.ToString());
            }
            catch (System.Exception ex)
            {
            }
        }

        public static List<List<uint>> GetNodeList(string nodeListString)
        {
            List<List<uint>> nodeList = new List<List<uint>>();
            string[] resultStrs = nodeListString.Split(';');
            for (int j = 0; j < resultStrs.Length; j++)
            {
                string paraStrs = resultStrs[j];
                nodeList.Add(DataFormatManager.ParseUIntList(paraStrs, ","));
            }
            return nodeList;
        }

        private static IDataSheet GetObject(uint source)
        {
            switch (source)
            {
                case (uint) RepServFileType.Tabular:
                    return new TabularChartDataManager();
                case (uint) RepServFileType.EnergyCost:
                    return new EnergyCostChartDataManager();
                case (uint) RepServFileType.Trend:
                    return new TrendChartDataManager();
                case (uint) RepServFileType.SingleUsage:
                    return new SingleDeviceUsageDataManager();
                case (uint) RepServFileType.MultiUsage:
                    return new MultiDeviceUsageDataManager();
                case (uint) RepServFileType.EnergyPeriod:
                    return new EnergyPeriodDataManager();
                case (uint) RepServFileType.PowerQuality:
                    return new PowerQualityDataManager();
                case (uint) RepServFileType.PowerQualityEventsOnly:
                    return new PowerQualityEventsOnlyManager();
                case (uint) RepServFileType.LoadProfile:
                    return new LoadProfileManager();
                case (uint) RepServFileType.HourlyUsage:
                    return new HourlyUsageChartDataManager();
                case (uint) RepServFileType.EventHistory:
                    return new EventDataQueryManager();

                default:
                    return null;
            }
        }

        ///// <summary>
        ///// 获取100msReport的查询结果
        ///// </summary>
        ///// <param name="datasourcelabel"></param>
        ///// <returns></returns>
        //private static List<DataTable> Get100msReportData(DataSourceLabel datasourcelabel)
        //{
        //    List<DataTable> resulttablelist = new List<DataTable>();
        //    string deviceIDs = string.Empty;
        //    string queryParas = string.Empty;
        //    AnaliseDeviceIDs(datasourcelabel.entireRSParms.deviceIDs, ref deviceIDs, ref queryParas);
        //    DataTable TabularDT = TabularData.GetTabularData(deviceIDs, queryParas,
        //        (int)DataIDParaType.HighDataLogType, datasourcelabel.startTime, datasourcelabel.endTime);
        //    resulttablelist.Add(TabularDT);
        //    return resulttablelist;
        //}

        ///// <summary>
        ///// 获取EnergyDemand的查询结果
        ///// </summary>
        ///// <param name="datasourcelabel"></param>
        ///// <returns></returns>
        //private static List<DataTable> GetEnergyDemandData(DataSourceLabel datasourcelabel)
        //{
        //    List<DataTable> resulttablelist = new List<DataTable>();
        //    DataTable energyDemandTable = new DataTable("EnergyDemandDT");
        //    TOUReportDataManager.DataManager.GetEnergyDemandData(datasourcelabel.startTime, datasourcelabel.endTime, datasourcelabel.entireRSParms.deviceIDs, ref energyDemandTable);
        //    resulttablelist.Add(energyDemandTable);
        //    return resulttablelist;
        //}

        /// <summary>
        /// 对于Tabular报表，将deviceIDs参数进行拆分
        /// </summary>
        /// <param name="strdeviceIDs"></param>
        /// <param name="deviceIDs"></param>
        /// <param name="queryParas"></param>
        private static void AnaliseDeviceIDs(string strdeviceIDs, ref string deviceIDs, ref string queryParas)
        {
            string[] deviceIDStr = strdeviceIDs.Split(';');
            uint deviceID = 0;
            for (int i = 0; i < deviceIDStr.Length; i++)
            {
                //deviceID参数格式——1,2,1;1,3,1
                if (!uint.TryParse(deviceIDStr[i], out deviceID))
                {
                    string paraStrs = deviceIDStr[i];
                    List<uint> paraIDs = DataFormatManager.ParseUIntList(paraStrs, ",");
                    uint measureID = 0;
                    int rptNodeType = 0;
                    int logicIndex = 1;
                    if (paraIDs.Count > 0)
                    {
                        deviceIDs = paraIDs[0].ToString();
                        if (paraIDs.Count > 1)
                            measureID = paraIDs[1];
                        if (paraIDs.Count > 2)
                            rptNodeType = Convert.ToInt32(paraIDs[2]);
                        if (paraIDs.Count > 3)
                            logicIndex = Convert.ToInt32(paraIDs[3]);
                        queryParas += measureID + "," + rptNodeType + "," + logicIndex + ";";
                    }
                }
            }
        }

        /// <summary>
        /// 获取SystemConfiguation的查询结果
        /// </summary>
        /// <param name="datasourcelabel"></param>
        /// <returns></returns>
        private static List<DataTable> GetSystemConfiguationData(DataSourceLabel datasourcelabel)
        {
            List<DataTable> resulttablelist = new List<DataTable>();
            string deviceIDs = datasourcelabel.entireRSParms.deviceIDs;
            //构建返回结果表
            DataTable resultTable = new DataTable("SystemConfiguationDT");
            DataRow row;
            resultTable.Columns.Add("DeviceName");
            resultTable.Columns.Add("DeviceType");
            resultTable.Columns.Add("DeviceAddress");
            resultTable.Columns.Add("DeviceSite");
            resultTable.Columns.Add("DevicePortType");
            resultTable.Columns.Add("DeviceCommuParam");
            resultTable.Columns.Add("SerialNum");
            resultTable.Columns.Add("FirmworkVersion");
            resultTable.Columns.Add("Productor");
            string[] deviceIDStr = deviceIDs.Split(';');
            for (int i = 0; i < deviceIDStr.Length; i++)
            {
                uint deviceID = 0;
                if (uint.TryParse(deviceIDStr[i], out deviceID))
                {
                    PecsDeviceNode deviceNode = PecsNodeManager.PecsNodeInstance.GetDeviceNodeByID(deviceID);
                    if (deviceNode != null)
                    {
                        DataTable tempTable = new DataTable();
                        SetupTableProvider.Instance.ReadSetupNodesByNodeTypeID(0,SysNodeType.PECSDEVICE_NODE, deviceNode.MeterType, false, 1, true, ref tempTable);
                        byte[] bytes = new byte[] {};
                        DataRow tempDR = tempTable.Rows[0];

                        if (tempTable.Rows.Count == 1)
                        {
                            //image类型数据转换为byte[]存储
                            if (!Convert.IsDBNull(tempDR["Data"]))
                            {
                                bytes = (byte[]) tempDR["Data"];
                                for (int j = 0; j < bytes.Length; j++)
                                {
                                    bytes[j] = (byte) (~bytes[j] - 1);
                                }
                            }
                        }
                        string meterTypeName = "";
                        if (tempTable.Rows.Count == 1)
                        {
                            meterTypeName = System.Text.Encoding.Default.GetString(bytes, 0, 128);
                            meterTypeName = meterTypeName.Replace('\0', ' ').Trim();
                        }
                        row = resultTable.NewRow();
                        row["DeviceName"] = deviceNode.NodeName;
                        row["DeviceType"] = meterTypeName;
                        row["DeviceAddress"] = deviceNode.RemoteAddress;
                        row["DeviceSite"] = deviceNode.ParentNode.ParentNode.ParentNode.NodeName;
                        row["DevicePortType"] = deviceNode.PortType;
                        row["DeviceCommuParam"] = deviceNode.CommuParamSetting;
                        row["SerialNum"] = deviceNode.DeviceInfo.SerialNum;
                        row["FirmworkVersion"] = deviceNode.DeviceInfo.FirmworkVersion;
                        row["Productor"] = deviceNode.DeviceInfo.Productor;
                        resultTable.Rows.Add(row);
                    }
                }
            }
            resulttablelist.Add(resultTable);
            return resulttablelist;
        }
    }
}
