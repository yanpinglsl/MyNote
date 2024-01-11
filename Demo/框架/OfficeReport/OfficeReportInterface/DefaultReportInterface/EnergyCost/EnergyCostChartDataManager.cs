using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using CSharpDBPlugin;
using DBInterfaceCommonLib;
using OfficeReportInterface.DefaultReportInterface.CommonluUsed;
using OfficeReportInterface.ReadingIniFile;


namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
    class EnergyCostChartDataManager : IDataSheet
    {

        private const uint source = (uint) RepServFileType.EnergyCost;


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
            Initial_Retained_decimal_digits();
            //记录被替代dataId的部分。key是原来的kwh，value是包含    替代的dataId的DeviceDataIDDef对象
            Dictionary<DeviceDataIDDef, DeviceDataIDDef> changedDic;
            //场景是这样的，就是有些装置只有正向，有些只有总，一般情况下都是会有这两个参数，所以在电能参数上相当于是做了一个优先级，如果有总和的话，就以这个参数来展示数据，如果没有这个参数的话，就以正向来展示数据，这种处理，基本上可以让所有的装置都能展示数据。
            parameter.DeviceDataIDList = GetDataIdList(parameter,out changedDic);
            PrintDeviceDataId(parameter.DeviceDataIDList);
            List<DataTable> resultDTList = new List<DataTable>();
            DataTable warningDT = DefaultTemplatePublicMethod.ConstructWarningTable(parameter.IsIncludeWarning);
            DataTable summaryDT = ConstructDataSummaryTable(parameter);

            //根据传入的参数填充警告表，汇总表，数据表
            DataTable resultDT2 = DefaultTemplatePublicMethod.ConstructRealDataTable(parameter.StartTime, parameter.EndTime, parameter.DeviceDataIDList, (uint)RepServFileType.EnergyCost); 
            FillWarmSummaryDataToTable(parameter, warningDT,  resultDT2);
            resultDT2.Dispose();
            DataTable resultDT = FillDataTable(parameter, warningDT);

            resultDTList.Add(warningDT);
            resultDTList.Add(summaryDT);
            resultDTList.Add(resultDT);
            return resultDTList;
        }
        /// <summary>
        /// 初始化保留小数位数
        /// </summary>
        private void Initial_Retained_decimal_digits()
        {
            //从ini文件读取用户名和密码
            //string name = "OfficeReport.ini";
            //string assemblyPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            //assemblyPath = Path.GetDirectoryName(assemblyPath);
            //string fileName = Path.Combine(assemblyPath, name);

            //FileInfo fi = new FileInfo(fileName);
            //if (!fi.Exists)
            //    return;

            try
            {
                var iniFile = INIFile.GetIniFileByType(INIFile.IniFileType.OfficeReportIni);//new INIFile(fileName);
                if(iniFile==null)
                    return;
                var numberStr = iniFile.ReadString("EnergyCost", "Retained_decimal_digits");
                uint count;
                if (uint.TryParse(numberStr, out count))
                {
                    Retained_decimal_digits = count;
                }
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout( ex.Message+ex.StackTrace);
            }
        }

        /// <summary>
        /// 显示结果保留的小数位数
        /// </summary>
        public static uint Retained_decimal_digits = 6;
        /// <summary>
        /// 获取使用当前分割符号分割的整数部分的数据格式
        /// </summary>
        /// <returns></returns>
        public static string GetNumberFormatInfo()
        {
            return "#" + NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator + "##0";
        }
        /// <summary>
        /// 获取数据单位字符串，例如"#,##0.00"，用于设置Excel的单元格数据格式
        /// </summary>
        /// <param name="retainedDecimalDigits"></param>
        /// <returns></returns>
        public static string GetDataFormat(uint retainedDecimalDigits)
        {
            string dataFormat = GetNumberFormatInfo();//获取整数部分的字符串
            if (retainedDecimalDigits <= 0)
            {
                return dataFormat;//如果没有小数部分的字符串，则直接返回整数部分的字符串
            }
            string result = dataFormat + NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;//整数部分字符串，加上整数部分与小数部分的分割符号，例如小数点.
            for (int i = 0; i < retainedDecimalDigits; i++)
            {
                result = result + "0";//在小数点后面增加N个0，表示保留N位小数
            }
            return result;
        }
        private static void  PrintDeviceDataId(List<DeviceDataIDDef> deviceDataIDList)
        {
            DbgTrace.dout("Energy Cost Real Parameters:");
            foreach(var item in deviceDataIDList)
            {
                DbgTrace.dout(string.Format("DeviceID={0};DataID={1};ParaTypeID={2};DataTypeID={3};LogicalDeviceIndex={4};",
                    item.DeviceID,item.DataID,item.ParaTypeID,item.DataTypeID,item.LogicalDeviceIndex));
            }
        }

 

        /// <summary>
        /// 获取某个tariffIndex对应的tariffName
        /// </summary>
        /// <param name="tariffProfileList"></param>
        /// <param name="tariffIndex"></param>
        /// <param name="tariffName"></param>
        /// <returns></returns>
        private static bool GetTariffNameById(List<TOUTariffNode> tariffProfileList,uint tariffIndex,out string tariffName)
        {
            tariffName = string.Empty;

            foreach (var item in tariffProfileList)
            {
                if (tariffIndex == item.NodeID)
                {
                    tariffName = item.NodeName;
                    return true;
                }
            }
            return false;
        }

        private static bool GetIndexToFill(SortedSet<int> usedTariffIndexSet,uint dataId,int tariffNodeId,out int indexOfDataIdAndTariff)
        {
            indexOfDataIdAndTariff = 0;
          
            int countOfUsedTariffIndex = usedTariffIndexSet.Count;
            int indexOfDataId= 0;
            if (!GetIndexOfDataId(dataId, out indexOfDataId))
                return false;
            int nOftariffIndex = 0;
            if (!GetTariffIndex(usedTariffIndexSet, tariffNodeId, out nOftariffIndex))
                return false;
            GetLocation(out indexOfDataIdAndTariff, countOfUsedTariffIndex, indexOfDataId, nOftariffIndex);
           
            return true;
        }
        /// <summary>
        /// 计算出位置
        /// </summary>
        /// <param name="indexOfDataIdAndTariff"></param>
        /// <param name="countOfUsedTariffIndex"></param>
        /// <param name="indexOfDataId"></param>
        /// <param name="nOftariffIndex"></param>
        private static void GetLocation(out int indexOfDataIdAndTariff, int countOfUsedTariffIndex, int indexOfDataId, int nOftariffIndex)
        {
            indexOfDataIdAndTariff = countOfUsedTariffIndex*indexOfDataId + nOftariffIndex;
        }

        /// <summary>
        /// 填写数字的位置
        /// </summary>
        /// <param name="usedTariffIndexSet"></param>
        /// <param name="energyKindWithTariff"></param>
        /// <param name="indexOfDataIdAndTariff"></param>
        /// <param name="timeIndex"></param>
        /// <returns></returns>
        private static bool GetIndexToFillTable(SortedSet<int> usedTariffIndexSet, uint dataId, int tariffNodeId, out int indexOfDataIdAndTariff, out int timeIndex)
        {
            timeIndex = 0;
            if (
                ! GetIndexToFill(usedTariffIndexSet, dataId,
                    tariffNodeId, out indexOfDataIdAndTariff))
                return false;
            GetTimeIndexForSomeDevice(usedTariffIndexSet, indexOfDataIdAndTariff, out timeIndex);
            return true;
        }
        /// <summary>
        /// 获取某个数字对应的时刻的位置
        /// </summary>
        /// <param name="usedTariffIndexSet"></param>
        /// <param name="indexOfDataIdAndTariff"></param>
        /// <param name="timeIndex"></param>
        private static void GetTimeIndexForSomeDevice(SortedSet<int> usedTariffIndexSet, int indexOfDataIdAndTariff,
            out int timeIndex)
        {
            timeIndex = 0;
            timeIndex = indexOfDataIdAndTariff + usedTariffIndexSet.Count*6 + 1;
        }

    
        /// <summary>
        /// 查找费率节点在usedTariffIndexSet的元素位置，从1开始
        /// </summary>
        /// <param name="usedTariffIndexSet"></param>
        /// <param name="tariffIndex"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static bool GetTariffIndex(SortedSet<int> usedTariffIndexSet,int tariffIndex,out int index)
        {
            index = 0;
            if (!usedTariffIndexSet.Contains(tariffIndex))
                return false;

            foreach (var item in usedTariffIndexSet)
            {
                ++index;
                if (item == tariffIndex)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool GetTariffIndex(Dictionary<int, List<double>> unitValueDic, int tariffIndex, out int index)
        {
            index = 0;
            if (!unitValueDic.ContainsKey(tariffIndex))
                return false;

            foreach (var item in unitValueDic)
            {
                ++index;
                if (item.Key == tariffIndex)
                {
                    return true;
                }
            }
            return false;
        }
        

        /// <summary>
        /// 获取大的dataId区间，用来填写dataTable的位置。从0开始
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        private static bool GetIndexOfDataId(uint dataId,out int number)
        {
            number = 0;
            if ( TOUComputer.IsDATAIDKWH(dataId)  )
            {
                number = 0;
                return true;
            }
            if (TOUComputer.IsDATAIDKVARH(dataId))
            {
                number = 1;
                return true;
            }
            if (TOUComputer.IsDATAIDKVAH(dataId) )
            {
                number = 2;
                return true;
            }
            if (dataId == SysConstDefinition.DATAIDKW_DEMAND)
            {
                number = 3;
                return true;
            }
            if (dataId == SysConstDefinition.DATAIDKVAR_DEMAND)
            {
                number = 4;
                return true;
            }
            if (dataId == SysConstDefinition.DATAIDKVA_DEMAND)
            {
                number = 5;
                return true;
            }

            return false; 
        }

       


        /// <summary>
        /// 有些装置只有正向，有些只有总，一般情况下都是会有这两个参数，所以在电能参数上相当于是做了一个优先级，如果有总和的话，就以这个参数来展示数据，如果没有这个参数的话，就以正向来展示数据，这种处理，基本上可以让所有的装置都能展示数据。
        /// </summary>
        /// <param name="deviceDataIDDefList"></param>
        /// <returns></returns>
        private static List<DeviceDataIDDef> GetDataIdList(DefaultReportParameter parameter, out Dictionary<DeviceDataIDDef, DeviceDataIDDef> changedDic)
        {
            return DataIdChooser.GetDataIdList(parameter, out  changedDic);
        }

       
        

        /// <summary>
        /// 填充表格数据
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private DataTable FillDataTable(DefaultReportParameter parameter, DataTable warningDT)
        {
            DataTable resultDT = new DataTable("Data");
            try
            {
                //获取所有设备对应的定时记录数据
                NewTOUCompute newTOUCompute = new NewTOUCompute();

                SortedSet<int> usedTariffIndexSet;
                if (
                    !newTOUCompute.GetUsedTariff(parameter.StartTime, parameter.EndTime, parameter.TOUProfile,
                        out usedTariffIndexSet))
                    return resultDT;
                if (usedTariffIndexSet.Count == 0)
                {
                    string format = LocalResourceManager.GetInstance().GetString("0589", "请检查时间段{0}-{1}的费率方案有没有配置。");
                    string warningInfo = string.Format(format, parameter.StartTime, parameter.EndTime);
                    DefaultTemplatePublicMethod.AddDataWarning(warningInfo, warningDT, parameter.IsIncludeWarning);
                    DbgTrace.dout(warningInfo);
                    return resultDT;
                }
              
                Dictionary<DeviceWithLoop, Dictionary<EnergyKindWithTariff, DataLogValueDef>> resultDataDicTemp;
                //第一个key是dataid，第二个key是tariffIndex，value是最大值及其对应的时刻.这个是用来填充Summary的数据
                Dictionary<uint, Dictionary<uint, Dictionary<DateTime, double>>> maxDicTemp;
                if (!newTOUCompute.GetEnergyDemandDataAll(parameter.StartTime, parameter.EndTime, parameter.DeviceDataIDList,   parameter.TOUProfile, out resultDataDicTemp, out  maxDicTemp))
                    return resultDT;
                if (resultDataDicTemp.Count == 0)
                    return resultDT;
                //if (maxDicTemp.Count==0)
                //    return resultDT;
                //添加累加为空的警告信息
                newTOUCompute.AddWarningsForTariffIndex(warningDT, parameter.TOUProfile, parameter.IsIncludeWarning,
                    source);
                //去掉全是0的Peak
                Dictionary<DeviceWithLoop, Dictionary<EnergyKindWithTariff, DataLogValueDef>>
                    resultDataDicTempWithoutZero =
                        new Dictionary<DeviceWithLoop, Dictionary<EnergyKindWithTariff, DataLogValueDef>>();
                //要是某些费率段定义上没有数据，就不参加放到Data这个sheet了
                FiltUnusedPeak(resultDataDicTemp,out resultDataDicTempWithoutZero, usedTariffIndexSet);
                //key是tariffIndex，value是tariffName
                Dictionary<int, string> usedTariffIndexToNameDic = new Dictionary<int, string>();
                //获取tariffIndex对应的名称
                GetTariffIndexToNameDic(parameter, usedTariffIndexSet, usedTariffIndexToNameDic);

                if (usedTariffIndexToNameDic.Count != usedTariffIndexSet.Count)
                    return new DataTable();
                int tariffProfileCount;
                tariffProfileCount = usedTariffIndexSet.Count;

                Dictionary<DeviceWithLoop, SortedDictionary<EnergyKindWithTariff, DataLogValueDef>> resultDataDic =
                    new Dictionary<DeviceWithLoop, SortedDictionary<EnergyKindWithTariff, DataLogValueDef>>();
                GetResultDataDic(resultDataDicTempWithoutZero, resultDataDic, usedTariffIndexSet);
                //第一个key是dataid，第二个key是tariffIndex，value是最大值及其对应的时刻.
                SortedDictionary<uint, SortedDictionary<uint, Dictionary<DateTime, double>>> maxDic =
                    new SortedDictionary<uint, SortedDictionary<uint, Dictionary<DateTime, double>>>();
                //获取用来填充Summary这个sheet的最大值数据
                GetMaxDic(maxDicTemp, maxDic, usedTariffIndexSet);
                AddResultDatatable(tariffProfileCount, resultDT);
                //添加第一行的内容
                AddTitleFirstLine(resultDT, tariffProfileCount);
                //添加第2行的内容
                AddTitleSecondLine(GetKWhString(parameter.kWh), GetKvarhString(parameter.kvarh), GetKVAhString(parameter.kVAh), resultDT, tariffProfileCount);
                //添加第三行内容
                AddTitleThirdLine(usedTariffIndexToNameDic, resultDT);
                //添加第4行
                AddPeakTimeLine(usedTariffIndexSet, resultDT, maxDic);
                //添加第5行
                //key是tariffNodeId，value中的key是dataId，value是单价
                Dictionary<int, List<double>> unitValueDic = new Dictionary<int, List<double>>();
                GetUnitPriceDic(parameter, usedTariffIndexSet,out unitValueDic);
                if (unitValueDic.Count != usedTariffIndexSet.Count)
                    return new DataTable();
                AddUnitCostLine(unitValueDic,resultDT);
                //依次填写每个设备对应的行
                AddDeviceLines( usedTariffIndexSet,resultDataDic, resultDT);
                AddTotalLine(resultDT, maxDic);
                return resultDT;
            }
            catch (System.Exception ex)
            {
                string log = string.Format(ex.Message + ex.StackTrace);
                DbgTrace.dout("{0}", log);
                resultDT = new DataTable("Data");
                return resultDT;
            }
        }

        private static void GetUnitPriceDic(DefaultReportParameter parameter, SortedSet<int> usedTariffIndexSet,
            out Dictionary<int, List<double>> unitValueDic)
        {
            unitValueDic = new Dictionary<int, List<double>>();
            foreach (var tariffIndex in usedTariffIndexSet)
            {
                foreach (var item in parameter.TOUProfile.tariffProfileList)
                {
                    if (item.NodeID != tariffIndex)
                        continue;
                    if (unitValueDic.ContainsKey(tariffIndex))
                        continue;
                    unitValueDic.Add(tariffIndex, item.TariffValues);
                }
            }
        }

        private static void GetTariffIndexToNameDic(DefaultReportParameter parameter, SortedSet<int> usedTariffIndexSet,
            Dictionary<int, string> usedTariffIndexToNameDic)
        {
            foreach (var tariffIndex in usedTariffIndexSet)
            {
                if (tariffIndex < 0)
                    continue;
                string tariffName;
                if (!GetTariffNameById(parameter.TOUProfile.tariffProfileList, (uint) tariffIndex, out tariffName))
                    continue;
                if (usedTariffIndexToNameDic.ContainsKey(tariffIndex))
                    continue;
                usedTariffIndexToNameDic.Add(tariffIndex, tariffName);
            }
        }

        private static void FiltUnusedPeak(Dictionary<DeviceWithLoop, Dictionary<EnergyKindWithTariff, DataLogValueDef>> resultDataDicTemp, out Dictionary<DeviceWithLoop, Dictionary<EnergyKindWithTariff, DataLogValueDef>> resultDataDicTempWithoutZero, SortedSet<int> usedTariffIndex)
        {
            resultDataDicTempWithoutZero =
                new Dictionary<DeviceWithLoop, Dictionary<EnergyKindWithTariff, DataLogValueDef>>();
            foreach (var item in resultDataDicTemp)
            {
                if (!resultDataDicTempWithoutZero.ContainsKey(item.Key))
                    resultDataDicTempWithoutZero.Add(item.Key,
                        new Dictionary<EnergyKindWithTariff, DataLogValueDef>());
                foreach (var node in item.Value)
                {
                    int tariffIndex = (int) node.Key.TariffNodeId;
                    if (!usedTariffIndex.Contains(tariffIndex))
                        continue;
                    resultDataDicTempWithoutZero[item.Key].Add(node.Key, node.Value);
                }
            }
        }

        private static void AddDeviceLines(SortedSet<int> usedTariffIndexSet, Dictionary<DeviceWithLoop, SortedDictionary<EnergyKindWithTariff, DataLogValueDef>> resultDataDic, DataTable resultDT)
        {
            foreach (var item in resultDataDic)
            {

                var dr = resultDT.NewRow();
                resultDT.Rows.Add(dr);
                //填写设备名称
                string deviceNameWithLoop;
                if (!NamesManager.GetInstance((uint) RepServFileType.EnergyCost).GetDeviceNameWithLoop(item.Key.DeviceId, item.Key.LogicalDeviceIndex,
                    out deviceNameWithLoop))
                {
                    deviceNameWithLoop = string.Format("{0}{1}", item.Key.DeviceId, item.Key.LogicalDeviceIndex);
                }


                dr[0] = deviceNameWithLoop;


                //填写数据
                foreach (var deviceNode in item.Value)
                {
                    int indexOfDataIdAndTariff;
                    int timeIndex;
                    if (
                        !GetIndexToFillTable(usedTariffIndexSet, deviceNode.Key.DataId,
                            (int) deviceNode.Key.TariffNodeId,
                            out indexOfDataIdAndTariff, out timeIndex))
                        continue;
                    string resultTemp = EMPTY_STRING;
                    if (!double.IsNaN(deviceNode.Value.DataValue))
                        resultTemp = GetRetainedDecimalDigitsAfterValue(deviceNode.Value.DataValue);
                    dr[indexOfDataIdAndTariff] = resultTemp;
                    dr[timeIndex] = deviceNode.Value.Logtime;
                }
            }
        }

        /// <summary>
        /// 获取按照设置的保留小数位数格式化之后的字符串
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private static string GetRetainedDecimalDigitsAfterValue(double v)
        {
            string formatNumber = "{0:F" + Retained_decimal_digits.ToString() + "}";
            return string.Format(formatNumber, v);
        }

        private static void GetMaxDic(Dictionary<uint, Dictionary<uint, Dictionary<DateTime, double>>> maxDicTemp, SortedDictionary<uint, SortedDictionary<uint, Dictionary<DateTime, double>>> maxDic, SortedSet<int> usedTariffIndex)
        {
            foreach (var item in maxDicTemp)
            {
                if (!maxDic.ContainsKey(item.Key))
                    maxDic.Add(item.Key, new SortedDictionary<uint, Dictionary<DateTime, double>>());
                foreach (var node in item.Value)
                {
                    if (!usedTariffIndex.Contains((int) node.Key))
                        continue;
                    var temp = maxDic[item.Key];
                    if (!temp.ContainsKey(node.Key))
                        temp.Add(node.Key, node.Value);
                }
            }
        }

        private static void GetResultDataDic(Dictionary<DeviceWithLoop, Dictionary<EnergyKindWithTariff, DataLogValueDef>> resultDataDicTemp, Dictionary<DeviceWithLoop, SortedDictionary<EnergyKindWithTariff, DataLogValueDef>> resultDataDic, SortedSet<int> usedTariffIndex)
        {
            foreach (var item in resultDataDicTemp)
            {
                if (!resultDataDic.ContainsKey(item.Key))
                    resultDataDic.Add(item.Key, new SortedDictionary<EnergyKindWithTariff, DataLogValueDef>());
                var temp = resultDataDic[item.Key];
                foreach (var node in item.Value)
                {
                    if (!usedTariffIndex.Contains((int) node.Key.TariffNodeId))
                        continue;

                    if (!temp.ContainsKey(node.Key))
                        temp.Add(node.Key, node.Value);
                }
            }
        }

        private static void AddTotalLine(DataTable resultDT, SortedDictionary<uint, SortedDictionary<uint, Dictionary<DateTime, double>>> maxDic)
        {
            DataRow dr;
            dr = resultDT.NewRow();
            dr[0] = LocalResourceManager.GetInstance().GetString("0541", "All Devices");
            int index = 0;
            //暂且把kwh，kvarh，kvah的部分也填上
            FillSum(maxDic, ref index, dr);
            //kw，kvar，kva的部分填上数据
            FillSum(maxDic, ref index, dr);

            resultDT.Rows.Add(dr);
        }

        private const string EMPTY_STRING = "--";

        private static void FillSum(SortedDictionary<uint, SortedDictionary<uint, Dictionary<DateTime, double>>> maxDic, ref int index, DataRow dr)
        {
            foreach (var dataIDItem in maxDic)
            {
                foreach (var tariffIndexItem in dataIDItem.Value)
                {
                    ++index;
                    var maxNodeDic = tariffIndexItem.Value;
                    foreach (var node in maxNodeDic)
                    {
                        string temp = EMPTY_STRING;
                        if (!double.IsNaN(node.Value))
                            temp = node.Value.ToString();
                        dr[index] = temp;
                    }

                }
            }
        }


        private static void AddResultDatatable(int tariffProfileCount, DataTable resultDT)
        {
            int columnCount;
            columnCount = (tariffProfileCount*6 + 1)*2;
            for (int i = 0; i < columnCount; ++i)
            {
                DataColumn column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = i.ToString();
                resultDT.Columns.Add(column);
            }
        }

        private static void AddTitleFirstLine(DataTable resultDT, int tariffProfileCount)
        {
            DataRow dr = resultDT.NewRow();
            dr[0] = LocalResourceManager.GetInstance().GetString("0545", "Cost");
            dr[1] = LocalResourceManager.GetInstance().GetString("0529", "Energy Cost");
            dr[3*tariffProfileCount + 1] = LocalResourceManager.GetInstance().GetString("0530", "Demand Cost");
            resultDT.Rows.Add(dr);
        }

        private static void AddTitleSecondLine(string kWh, string kvarh, string kVAh, DataTable resultDT, int tariffProfileCount)
        {
            DataRow dr;
            dr = resultDT.NewRow();
            int cunnentColumn = 0;
            dr[cunnentColumn] = LocalResourceManager.GetInstance().GetString("0548", "Energy");
            cunnentColumn = cunnentColumn + 1;
            List<string> nameList = new List<string>();
            nameList.Add(kWh);
            nameList.Add(kvarh);
            nameList.Add(kVAh);
            //nameList.Add(LocalResourceManager.GetInstance().GetString("0531", "Real Energy (kWh)"));
            //nameList.Add(LocalResourceManager.GetInstance().GetString("0532", "Reactive Energy (kvarh)"));
            //nameList.Add(LocalResourceManager.GetInstance().GetString("0533", "Apparent Energy (kVAh)"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0534", "Real Demand (kW)"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0535", "Reactive Demand (kvar)"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0536", "Apparent Demand (kVA)"));
            foreach (var name in nameList)
            {
                dr[cunnentColumn] = name;
                cunnentColumn = cunnentColumn + tariffProfileCount;
            }
            resultDT.Rows.Add(dr);
        }

        private static void AddTitleThirdLine(Dictionary<int, string> usedTariffIndexToNameDic, DataTable resultDT)
        {
            try
            {
                DataRow dr = resultDT.NewRow();
                dr[0] = LocalResourceManager.GetInstance().GetString("0549", "Tariff");

                int tariffProfileCount = usedTariffIndexToNameDic.Count;
                for (int j = 0; j < 6; ++j)
                {
                    int i = 1;
                    foreach (var item in  usedTariffIndexToNameDic)
                    {
                        dr[j*tariffProfileCount + i] = item.Value;
                        ++i;
                    }
                }

                resultDT.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                DbgTrace.dout("{0}{1}", "AddTitleThirdLine(   Dictionary<int, string> usedTariffIndexToNameDic, DataTable resultDT)   catch (Exception ex): ", ex.Message);
            }

        }


        private static void AddUnitCostLine(Dictionary<int, List<double>> unitValueDic, DataTable resultDT)
        {
            DataRow dr;
            dr = resultDT.NewRow();
            dr[0] = LocalResourceManager.GetInstance().GetString("0544", "Unit Cost");
            int countOfTariffIndex = unitValueDic.Count;
            foreach (var tariffIndexItem in unitValueDic)
            {
                int indexOfDataId = 0;
                foreach (var uintPrice in tariffIndexItem.Value)
                {
                    int nOfTariffIndex = 0;
                    if (!GetTariffIndex(unitValueDic, tariffIndexItem.Key, out nOfTariffIndex))
                        continue;


                    int location;
                    GetLocation(out location, countOfTariffIndex, indexOfDataId, nOfTariffIndex);

                    dr[location] = GetRetainedDecimalDigitsAfterValue(uintPrice);
                    ++indexOfDataId;
                }
            }
            resultDT.Rows.Add(dr);
        }

        private static void AddPeakTimeLine(SortedSet<int> usedTariffIndexSet, DataTable resultDT, SortedDictionary<uint, SortedDictionary<uint, Dictionary<DateTime, double>>> maxDic)
        {
            DataRow dr;
            dr = resultDT.NewRow();
            dr[0] = LocalResourceManager.GetInstance().GetString("0537", "Peak Time");
            int index = 0;

            foreach (var dataIDItem in maxDic)
            {
                foreach (var tariffIndexItem in dataIDItem.Value)
                {
                    var maxNodeDic = tariffIndexItem.Value;
                    foreach (var node in maxNodeDic)
                    {
                        if (!GetIndexToFill(usedTariffIndexSet, dataIDItem.Key, (int) tariffIndexItem.Key, out index))
                            continue;
                        dr[index] = node.Key;
                    }
                }
            }
            resultDT.Rows.Add(dr);
        }

        private static void FillMaxTime(SortedDictionary<uint, SortedDictionary<uint, Dictionary<DateTime, double>>> maxDic, ref int index, DataRow dr)
        {
            foreach (var dataIDItem in maxDic)
            {
                foreach (var tariffIndexItem in dataIDItem.Value)
                {
                    ++index;
                    var maxNodeDic = tariffIndexItem.Value;
                    foreach (var node in maxNodeDic)
                        dr[index] = node.Key;
                }
            }
        }

        private bool GetAllDeviceNamesListWithLoop(List<DeviceDataIDDef> deviceList, out List<string> deviceNameList)
        {
            NamesManager nameManager = NamesManager.GetInstance((uint) RepServFileType.EnergyCost);

            if (!nameManager.GetAllDeviceNamesListWithLoop(deviceList, out deviceNameList))
                return false;
            return true;
        }

        private bool GetDevicesNumber(DefaultReportParameter parameter, out int devicesNumber)
        {
            devicesNumber = 0;
            try
            {
                List<string> deviceNameList;
                if (!GetAllDeviceNamesListWithLoop(parameter.DeviceDataIDList, out deviceNameList))
                    return false;
                devicesNumber = deviceNameList.Count;

                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout("{0}{1}", " EnergyCostChartDataManager.GetDevicesNumber(DefaultReportParameter parameter,out int devicesNumber)  catch (System.Exception ex):", ex.Message);
                return false;
            }
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
                DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "1");
                DefaultTemplatePublicMethod.AddColumnToTable(resultDT, "2");

                //起始结束时间
                DataRow resultRow = resultDT.NewRow();

                resultRow[0] = DataManager.GetStartTimeSting(parameter.StartTime);
                resultRow[1] = DataManager.GetEndTimeString(parameter.EndTime);
                resultDT.Rows.Add(resultRow);
                //空行
                AddEmptyLine(resultDT);

                //警告信息时间格式
                AddWarninMessageFormat(resultDT);

                //数据坐标轴时间格式
                resultRow = resultDT.NewRow();
                resultRow[0] = DataManager.GetTableTimeFormatForExcel(); // CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " HH:mm";
                resultRow[1] = "Curve Time";
                resultDT.Rows.Add(resultRow);

                string dataFormat = GetDataFormat(Retained_decimal_digits);

                //数据格式
                resultRow = resultDT.NewRow();
                resultRow[0] = dataFormat;
                resultRow[1] = Retained_decimal_digits.ToString();
                resultDT.Rows.Add(resultRow);
                string currencyUnit = string.Empty;
                try
                {
                    currencyUnit = parameter.TOUProfile.tariffProfileList[0].CurrencyUnit;
                }
                catch (System.Exception ex)
                {
                    DbgTrace.dout("{0}{1}", "EnergyCostChartDataManager.ConstructDataSummaryTable(DefaultReportParameter parameter)   currencyUnit = parameter.TOUProfile.tariffProfileList[0].CurrencyUnit; catch (System.Exception ex):", ex.Message);

                }


                resultRow = resultDT.NewRow();
                //  resultRow[0] = string.Format("{0}{1}", currencyUnit, dataFormat);
                resultRow[0] = string.Format("{0}", dataFormat);
                resultRow[1] = "Money";
                resultDT.Rows.Add(resultRow);

                for (int i = 0; i < 3; ++i)
                {
                    //空行
                    AddEmptyLine(resultDT);
                }

                resultRow = resultDT.NewRow();
                resultRow[0] = LocalResourceManager.GetInstance().GetString("0547", "DevicesNumber");
                resultDT.Rows.Add(resultRow);

                resultRow = resultDT.NewRow();
                int devicesNumber;
                GetDevicesNumber(parameter, out devicesNumber);
                resultRow[0] = devicesNumber.ToString();
                resultDT.Rows.Add(resultRow);
                //空行
                AddEmptyLine(resultDT);

                //设备
                resultRow = resultDT.NewRow();
                resultRow[0] = LocalResourceManager.GetInstance().GetString("0021", "Devices");
                resultDT.Rows.Add(resultRow);

                var newRow = resultDT.NewRow();
                var namesManager = NamesManager.GetInstance(source);
                string deviceNames;
                if (namesManager.GetAllDeviceNamesWithLoop(parameter.DeviceDataIDList, out deviceNames))
                    newRow[0] = deviceNames;

                resultDT.Rows.Add(newRow);
                //空行
                AddEmptyLine(resultDT);

                AddNamesForEnergy(GetKWhString(parameter.kWh), GetKvarhString(parameter.kvarh), GetKVAhString(parameter.kVAh), resultDT, currencyUnit, parameter);
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout("{0}{1}", "EnergyCostChartDataManager.ConstructDataSummaryTable(DefaultReportParameter parameter) catch (System.Exception ex):", ex.Message+ex.StackTrace);
            }

            return resultDT;
        }

        private static string GetKWhString(int index)
        {
            switch (index)
            {
                case DataIdChooser.TOTAL_INDEX:
                    return LocalResourceManager.GetInstance().GetString("0608", "Active Energy (kWh Total)");
                case DataIdChooser.IMPORT_INDEX:
                    return LocalResourceManager.GetInstance().GetString("0602", "Active Energy (kWh Import)");
                case DataIdChooser.INCREASEMENT_INDEX:
                    return LocalResourceManager.GetInstance().GetString("0605", "Active Energy (kWh Import Interval)");
                case DataIdChooser.AUTOFIT_INDEX:
                    return LocalResourceManager.GetInstance().GetString("0531", "Real Energy (kWh)");
                default:
                    return LocalResourceManager.GetInstance().GetString("0531", "Real Energy (kWh)");
            }
        }

        private static string GetKvarhString(int index)
        {
            switch (index)
            {
                case DataIdChooser.TOTAL_INDEX:
                    return LocalResourceManager.GetInstance().GetString("0609", "Reactive Energy (kvarh Total)");
                case DataIdChooser.IMPORT_INDEX:
                    return LocalResourceManager.GetInstance().GetString("0603", "Reactive Energy (kvarh Import)");
                case DataIdChooser.INCREASEMENT_INDEX:
                    return LocalResourceManager.GetInstance().GetString("0606", "Reactive Energy (kvarh Import Interval)");
                case DataIdChooser.AUTOFIT_INDEX:
                    return LocalResourceManager.GetInstance().GetString("0532", "Reactive Energy (kvarh)");
                default:
                    return LocalResourceManager.GetInstance().GetString("0532", "Reactive Energy (kvarh)");
            }
        }

        private static string GetKVAhString(int index)
        {
            switch (index)
            {
                case DataIdChooser.TOTAL_INDEX:
                    return LocalResourceManager.GetInstance().GetString("0610", "Apparent Energy (kVAh Total)");
                case DataIdChooser.IMPORT_INDEX:
                    return LocalResourceManager.GetInstance().GetString("0604", "Apparent Energy (kVAh Import)");
                case DataIdChooser.INCREASEMENT_INDEX:
                    return LocalResourceManager.GetInstance().GetString("0607", "Apparent Energy (kVAh Import Interval)");
                case DataIdChooser.AUTOFIT_INDEX:
                    return LocalResourceManager.GetInstance().GetString("0533", "Apparent Energy (kVAh)");
                default:
                    return LocalResourceManager.GetInstance().GetString("0533", "Apparent Energy (kVAh)");
            }
        }

        public static void AddWarninMessageFormat(DataTable resultDT)
        {
            DataRow resultRow;
            resultRow = resultDT.NewRow();
            resultRow[0] = DataManager.GetWarningMessageTimeFormatForExcel(); // CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " HH:mm:ss";
            resultRow[1] = "Date Added";
            resultDT.Rows.Add(resultRow);
        }

        private static void AddNamesForEnergy(string kWh, string kvarh, string kVAh, DataTable resultDT, string currencyUnit, DefaultReportParameter parameter)
        {
            DataRow resultRow;
            List<string> nameList = new List<string>();
            string costwithUnit = string.Format("{0} ({1})", LocalResourceManager.GetInstance().GetString("0545", "Cost"), currencyUnit);
            nameList.Add(costwithUnit);
            nameList.Add(LocalResourceManager.GetInstance().GetString("0548", "Energy"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0529", "Energy Cost"));
            nameList.Add(kWh);
            nameList.Add(kvarh);
            nameList.Add(kVAh);
            //nameList.Add(LocalResourceManager.GetInstance().GetString("0531", "Real Energy (kWh)"));
            //nameList.Add(LocalResourceManager.GetInstance().GetString("0532", "Reactive Energy (kvarh)"));
            //nameList.Add(LocalResourceManager.GetInstance().GetString("0533", "Apparent Energy (kVAh)"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0530", "Demand Cost"));
            string kWStr = LocalResourceManager.GetInstance().GetString("0534", "Real Demand (kW)");
            nameList.Add(kWStr);
            string kvarStr = LocalResourceManager.GetInstance().GetString("0535", "Reactive Demand (kvar)");
            nameList.Add(kvarStr);
            string kVAStr = LocalResourceManager.GetInstance().GetString("0536", "Apparent Demand (kVA)");
            nameList.Add(kVAStr);
            nameList.Add(LocalResourceManager.GetInstance().GetString("0538", "SubTotal"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0539", "Total"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0021", "Devices"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0540", "Data Warnings"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0037", "Message"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0541", "All Devices"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0542", "Tariff"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0543", "Usage"));
            string unitCostWithUnit = string.Format("{0} ({1})", LocalResourceManager.GetInstance().GetString("0544", "Unit Cost"), currencyUnit);
            nameList.Add(unitCostWithUnit);
            nameList.Add(LocalResourceManager.GetInstance().GetString("0537", "Peak Time"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0546", "Peak Value"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0547", "DevicesNumber"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0527", "Energy Cost Report"));
            nameList.Add(LocalResourceManager.GetInstance().GetString("0562", "Summary"));

            foreach (var item in nameList)
            {
                resultRow = resultDT.NewRow();
                resultRow[0] = item;

                if (string.Equals(item, kWh))
                    resultRow[1] = parameter.kWh;
                else if (string.Equals(item, kvarh))
                    resultRow[1] = parameter.kvarh;
                else if (string.Equals(item, kVAh))
                    resultRow[1] = parameter.kVAh;
                else if (string.Equals(item, kWStr))
                    resultRow[1] = parameter.kWDemand;
                else if (string.Equals(item, kvarStr))
                    resultRow[1] = parameter.kvarDemand;
                else if (string.Equals(item, kVAStr))
                    resultRow[1] = parameter.kVADemand;

                resultDT.Rows.Add(resultRow);
            }
        }

        public static void AddEmptyLine(DataTable resultDT)
        {
            DataRow resultRow = resultDT.NewRow();
            resultDT.Rows.Add(resultRow);
        }

        private static void FillWarmSummaryDataToTable(DefaultReportParameter parameter, DataTable dataWarning, DataTable resultDT)
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

                DATALOG_PRIVATE_MAP resultMapDef ;
                bool result = ReportWebServiceManager.ReportWebManager.FindDataMapDef(parameter.DeviceDataIDList[i], out resultMapDef);
                if (!result)
                {
                    DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], WarningKind.SourceIdNotExist, dataWarning, parameter.IsIncludeWarning, source);
                    continue;
                }
                //DataTable tempDT = new DataTable();

                //int errorCode = DefaultTemplatePublicMethod.ReadDatalogs(DBOperationFlag.either, resultMapDef.StationID, resultMapDef.SourceID, resultMapDef.DataIndex, parameter.StartTime, parameter.EndTime.AddSeconds(1), (int) SysConstDefinition.DefaultMaxRowCount, ref tempDT, parameter.DeviceDataIDList[i]);

                //if (errorCode != 0)
                //{
                //    DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], DBInterfaceCommonLib.ErrorQuerier.Instance.GetLastErrorString(), dataWarning, parameter.IsIncludeWarning, source);
                //    continue;
                //}
                //if (tempDT.Rows.Count == 0)
                //{
                //    DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], WarningKind.DataIsNull, dataWarning, parameter.IsIncludeWarning, source);
                //    continue;
                //}
                List<DataLogOriDef> tempDT = new List<DataLogOriDef>();
                bool isSucess = DefaultTemplatePublicMethod.ReadDatalogs(resultMapDef, parameter.StartTime, parameter.EndTime.AddSeconds(1), parameter.DeviceDataIDList[i], ref tempDT);
                if (!isSucess)
                {
                    DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], DBInterfaceCommonLib.ErrorQuerier.Instance.GetLastErrorString(), dataWarning, parameter.IsIncludeWarning, source);
                    continue;
                }
                if(tempDT.Count == 0)
                {
                    DefaultTemplatePublicMethod.AddWarnings(parameter.DeviceDataIDList[i], WarningKind.DataIsNull, dataWarning, parameter.IsIncludeWarning, source);
                    continue;
                }
                if (index == 0)
                {
                    interval = DefaultTemplatePublicMethod.GetInterval(tempDT, ref index, ref tempStartTime, parameter.DeviceDataIDList[i], dataWarning, parameter.IsIncludeWarning, source);
                    if (index != -1)
                        DefaultTemplatePublicMethod.ConstrutDataTable(tempStartTime, parameter.EndTime, resultDT, interval);

                }
                DateTime dataNullStartTime = DateTime.MinValue;
                int dataNullNuber = 0;
                var dt = DefaultTemplatePublicMethod.GetSameTimeVal(tempStartTime, parameter.EndTime, parameter.DeviceDataIDList[i], tempDT, interval, ref dataNullStartTime, ref dataNullNuber, parameter.StartTime, false);
                //dt.Dispose();
                //tempDT.Dispose();
                if (dataNullNuber >= 1)
                {
                    DefaultTemplatePublicMethod.DataWarningNullFromTime(dataNullStartTime, parameter.DeviceDataIDList[i], dataWarning,
                        parameter.IsIncludeWarning, source);
                }
                index++;
            }
        }

        #endregion
    }
}
