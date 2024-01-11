using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using BasicDataInterface.Models.Response;
using OfficeReportInterface.DefaultReportInterface.IntelligentSafety;
using OfficeReportInterface.ReadingIniFile;
namespace OfficeReportInterface
{
   public class PowerQualityInterfaceManager
    {
        private static PowerQualityInterfaceManager _Instance = null;
        /// <summary>
        /// 单例
        /// </summary>
        /// <returns></returns>
        public static PowerQualityInterfaceManager GetInstance()
        {
            if (_Instance != null)
                return _Instance;
            PowerQualityInterfaceManager temp = new PowerQualityInterfaceManager();
            Interlocked.CompareExchange(ref _Instance, temp, null);
            return _Instance;
        }
        /// <summary>
        /// 分组间隔
        /// </summary>
        private string GROUP_INTERVAL;
        /// <summary>
        /// 构造函数
        /// </summary>
        private PowerQualityInterfaceManager()
        {
            //EMSWebServiceManager.EMSWebManager.InitializeWebService();
            string groupInterval;
            //从OfficeReport.ini文件读取。
            ReadGroupIntervalFromIni(out groupInterval);

            GROUP_INTERVAL = groupInterval;
        }

        /// <summary>
        /// 查询事件
        /// </summary>
        /// <param name="maxRowCount">事件最大条数</param>
        /// <param name="parameter">入参</param>
        /// <param name="actual">查询结果</param>
        /// <returns></returns>
        public bool GetPowerQualityEvents(int maxRowCount, DefaultReportParameter parameter, out List<SagSwellEvent> actual)
        {
            RMSAnalysisManager target = new RMSAnalysisManager();
            UserValidateMsgWithCode loginResult;
            SafetyDataManager.InitializeIemsWebServiceAndLogin(parameter.userID, out loginResult);

            var nodeParams = new List<NodeParam>();
            var devices = parameter.DeviceDataIDList;
            foreach (var item in devices)
            {
                var node = new NodeParam((uint) NodeType.DeviceNode, item.DeviceID);
                if (nodeParams.Contains(node))
                    continue;
                nodeParams.Add(node);
            }

            List<int> rmsEventTypes = new List<int>(){1,2};

            var timeParam = new DateTimeParam(parameter.StartTime, parameter.EndTime);

            NodeParam curveTypeID = new NodeParam();
            GetCurveTypeId(parameter, out curveTypeID);
            parameter.curveID = (int)curveTypeID.NodeID;//因为传入的可能是0，这里要调整成真正的curveID
            //这个是itic或semi的范围
            string pointRange;
            GetPointRange(parameter, out pointRange);

            if (!SafetyDataManager.hasIEMSWeb) //如果没有部署iEMSWeb，则保持原来的计算
            {
                actual = target.QuerySagSwellEvents(nodeParams, rmsEventTypes, timeParam, 0, curveTypeID, parameter.userID, maxRowCount, pointRange, GROUP_INTERVAL); 
            } 
            else //从iEMSWeb获取数据
            {
                StringBuilder PQNodeParams = new StringBuilder();
                foreach (var item in parameter.DeviceDataIDList)
                {
                    PQNodeParams.Append((uint) NodeType.DeviceNode);
                    PQNodeParams.Append(",");
                    PQNodeParams.Append(item.DeviceID);
                    PQNodeParams.Append(";");
                }

                WebGeneralResult<SagSwellEvent> result = PowerQualityDataManager.QuerySagSwellEventsForSARFI(parameter.userID,  PQNodeParams.ToString(), rmsEventTypes[0] + "," + rmsEventTypes[1], parameter.StartTime + "," + parameter.EndTime, curveTypeID.NodeType + "," + curveTypeID.NodeID, pointRange, maxRowCount, -100, GROUP_INTERVAL);
                actual = result.ResultList;
                return result.Success;
            }
            return true;
        }


       /// <summary>
       /// 读取OfficeReport.ini文件，获取GroupInterval
       /// </summary>
       /// <param name="groupInterval"></param>
       /// <returns></returns>
       private static void ReadGroupIntervalFromIni(out string groupInterval)
       {
           groupInterval = string.Format("{0},{1}", 1, 3);
           string interval1 = string.Empty;
           string interval2 = string.Empty;
           //--------------从OfficeReport.ini文件中读取GROUPINTERVAL1，GROUPINTERVAL2数据-----------------
           //   string fileName = Path.Combine(DbgTrace.GetAssemblyPath(), "OfficeReport.ini");
           //  if (!File.Exists(fileName))
           //  {
           //     DbgTrace.dout("未找到文件" + fileName);
           // }
           // else
           // {
           try
           {
               var iniFile = INIFile.GetIniFileByType(INIFile.IniFileType.OfficeReportIni);
               if (iniFile == null)
                   return;
               // var iniFile = new INIFile(fileName);
               interval1 = iniFile.ReadString("POWERQUALITY", "GROUPINTERVAL1");
               interval2 = iniFile.ReadString("POWERQUALITY", "GROUPINTERVAL2");
           }
           catch (Exception ex)
           {
               DbgTrace.dout(ex.Message + ex.StackTrace);
           }
           // }
           //--------------组合数据字符串------------------------------------------------------------------------
           uint intervalTemp11;
           if (!uint.TryParse(interval1, out intervalTemp11)) //首先确认是合法的数字，如果获取到的字符串不能成功转换成合法数字，则使用默认值。
               intervalTemp11 = 1;
           uint intervalTemp2;
           if (!uint.TryParse(interval2, out intervalTemp2))
               intervalTemp2 = 3;
           groupInterval = string.Format("{0},{1}", intervalTemp11, intervalTemp2);
       }

       /// <summary>
       /// 获取pointRange字符串，用于查询入参
       /// </summary>
       /// <param name="parameter"></param>
       /// <param name="pointRange"></param>
       private void GetPointRange(DefaultReportParameter parameter, out string pointRange)
       {
           if (SafetyDataManager.hasIEMSWeb)
           {
               if (parameter.curveID <= 1) // ITIC和SEMI-1s标准的curveID是1. 如果curveID=1，说明是ITIC标准或者SEMI-1s；如果curveID=0，说明可能是没有设置过curveID，保存的查询条件。这时候使用默认的第一个
               {
                   pointRange = parameter.IsITIC ? "-1,5,0,500" : "50,1000,0,100";
               }
               else //如果是自定义的容忍度标准则按照自定义的标准来算
               {
                   WebGeneralResult<SARFIChartCurve> result = PowerQualityDataManager.GetSARFIChartFeatureValues(parameter.userID, PowerQualityDataManager.GetCurveType(parameter.IsITIC), parameter.curveID);
                   double xMinValue = 50; //x轴最小值，需要根据当前应用的容忍度标准获取；ITIC的x轴最小值固定是-1，SEMI的x轴最小值默认是50，可自定义
                   double xMaxValue = 0; //x轴最大值，需要根据当前应用的容忍度标准获取；ITIC的x轴最小值固定是-1，SEMI的x轴最小值默认是0
                   double yMaxValue = 0; //y轴最大值，需要根据当前应用的容忍度标准获取；ITIC的y轴最小值固定是0，SEMI的y轴最小值默认是0
                   foreach (var item in result.ResultList[0].UpLineValues)
                   {
                       //获取当前曲线的x，y轴的范围最大值，用于传给接口函数获取数据
                       xMinValue = Math.Min(item.XValue, xMinValue);
                       xMaxValue = Math.Max(item.XValue, xMaxValue);
                       yMaxValue = Math.Max(item.YValue, yMaxValue);
                   }
                   if (result.ResultList[0].DownLineValues != null) //SEMI的DownLineValues是null
                       foreach (var item in result.ResultList[0].DownLineValues)
                       {
                           //获取当前曲线的x，y轴的范围最大值，用于传给接口函数获取数据
                           xMinValue = Math.Min(item.XValue, xMinValue);
                           xMaxValue = Math.Max(item.XValue, xMaxValue);
                           yMaxValue = Math.Max(item.YValue, yMaxValue);
                       }

                   pointRange = parameter.IsITIC ? "-1," + xMaxValue + ",0," + "500"/*yMaxValue*/ : xMinValue + "," + xMaxValue + ",0," + "100"/*yMaxValue*/;//马来西亚NUR现场提出，SEMI1s打点了，SEMI-100s没有打点，不对。原因是SEMI1s使用的上限值是100，而SEMI-100s使用的上限值是90
               }
           }
           else
           {
               if (parameter.IsITIC)
               {
                   //iemsweb传入的是"-1,5,0,500"。标准的是这个。iems有其自定义的。标准的没有别的了。
                   pointRange = "-1,5,0,500";
               }
               else
               {
                   //这个是SEMI-100s的入参"10,100000,0,90"
                   pointRange = "10,100000,0,90";
               }
               //SEMI-1s的入参是"50,1000,0,100"，暂时没有实现
           }
       }


       /// <summary>
        /// ITIC和SEMI-1s标准的curveID
        /// </summary>
        public const uint defaultCurID=1;
        /// <summary>
        /// SEMI-100s标准的curveID
        /// </summary>
        public const int SEMI100S_CURVE_ID = Int32.MaxValue;
        
        /// <summary>
        /// 获取ITIC/SEMI曲线的type、ID     
        /// <param name="curveType">待加载的曲线类型,1-ITIC, 2-SEMI, 不可为0</param>
        /// <param name="curveID">曲线ID，1-表示默认曲线，不可为0</param>
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="curveTypeId"></param>
        private void GetCurveTypeId(DefaultReportParameter parameter, out NodeParam curveTypeId)
        {
            if (SafetyDataManager.hasIEMSWeb)//正确部署了iEMSWeb，可以从iEMSWeb获取数据的情况
            {
                if (parameter.IsITIC)
                {
                    if (parameter.curveID > 0)//入参中的curveID合法
                    {
                        curveTypeId = new NodeParam((int)RMSAnalysisManager.ToleranceCurveType.ITICLimit,(uint) parameter.curveID);
                    }
                    else
                    {
                        curveTypeId = new NodeParam((uint)RMSAnalysisManager.ToleranceCurveType.ITICLimit, defaultCurID);
                    }
                }
                else
                {
                    if (parameter.curveID > 0)//入参中的curveID合法
                    {
                        curveTypeId = new NodeParam((int)RMSAnalysisManager.ToleranceCurveType.SEMILimit,(uint) parameter.curveID);
                    }
                    else
                    {
                        curveTypeId = new NodeParam((int)RMSAnalysisManager.ToleranceCurveType.SEMILimit, SEMI100S_CURVE_ID);
                    }
                }
            }
            else//未正确部署iemsweb，保留原来的功能
            {
                if (parameter.IsITIC)
                {
                    curveTypeId = new NodeParam((int)RMSAnalysisManager.ToleranceCurveType.ITICLimit, defaultCurID);
                }
                else
                {
                    curveTypeId = new NodeParam((int)RMSAnalysisManager.ToleranceCurveType.SEMILimit, SEMI100S_CURVE_ID);
                }      
            }
        }
    }
}
