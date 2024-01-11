using System;
using System.Collections.Generic;
using OfficeReportInterface.DefaultReportInterface;
using OfficeReportInterface.DefaultReportInterface.CommonluUsed;
using OfficeReportInterface.DefaultReportInterface.EnergyCost;
using OfficeReportInterface.QueryCondition;
using Excel = Microsoft.Office.Interop.Excel;
using PecsReport.PluginInterface;
using PecsReport.CommomNode;
using System.Data;
using CET.PecsNodeManage;
using Microsoft.Win32;
using System.Globalization;
using DBInterfaceCommonLib;


namespace OfficeReportInterface
{
    public class ProcessExcelTempFile
    {
        public ExcelFileOperate excelFileOperate;
        private List<Command> commandList = new List<Command>();
        private List<DataSourceLabel> dataSourceLabelList = new List<DataSourceLabel>();
        private DataSourceLabel[] dataSourceLabelArray = new DataSourceLabel[0];
        private DataResults[] staticResultsArray = new DataResults[0];
        private List<List<DataResults>> allDynamicResults = new List<List<DataResults>>();
        private List<DataResults> eachDynamicResultList = new List<DataResults>();
        private List<DataRetrievalLabel> staticRetrievalLabelList = new List<DataRetrievalLabel>();
        private List<DataRetrievalLabel> dynamicRetrievalLabelList = new List<DataRetrievalLabel>();
        private bool needEndtime = false;
        private bool needSelectDeviceNode = false;
        private uint sysNodeType;
        private uint sysNodeID;
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint userID;
        /// <summary>
        /// 用户名称
        /// </summary>
        public string userName;

        public string ErrorMessage { get; set; }

        public bool IsDefaultTemplateOrSaved()
        {
            if (dataSourceLabelArray.Length != 1)
                return false;
            if (dataSourceLabelArray[0].source < (uint)RepServFileType.MinumOfTemplate)
                return false;
            return true;
        }

        #region 私有变量属性

        /// <summary>
        /// 存储数据源标签数组
        /// </summary>
        public DataSourceLabel[] DataSourceLabelArray
        {
            get { return this.dataSourceLabelArray; }
            set { this.dataSourceLabelArray = value; }
        }

        /// <summary>
        /// 静态数据源标签（对应于静态引用标签）查询结果数组
        /// </summary>
        public DataResults[] StaticResultsArray
        {
            get { return this.staticResultsArray; }
            set { this.staticResultsArray = value; }
        }
        /// <summary>
        /// 存储所有（多日）动态引用标签的查询结果，每个元素为所有动态引用标签一天的数据
        /// </summary>
        public List<List<DataResults>> AllDynamicResults
        {
            get { return this.allDynamicResults; }
            set { this.allDynamicResults = value; }
        }
        /// <summary>
        /// 动态引用标签（对应于动态引用标签）查询结果数组（每日）
        /// </summary>
        public List<DataResults> EachDynamicResultList
        {
            get { return this.eachDynamicResultList; }
            set { this.eachDynamicResultList = value; }
        }

        /// <summary>
        /// 存储静态引用标签
        /// </summary>
        public List<DataRetrievalLabel> StaticRetrievalLabelList
        {
            get { return this.staticRetrievalLabelList; }
            set { this.staticRetrievalLabelList = value; }
        }

        /// <summary>
        /// 存储动态引用标签
        /// </summary>
        public List<DataRetrievalLabel> DynamicRetrievalLabelList
        {
            get { return this.dynamicRetrievalLabelList; }
            set { this.dynamicRetrievalLabelList = value; }
        }

        /// <summary>
        /// 私有ExcelFileOperate实例
        /// </summary>
        public ExcelFileOperate PublicExcelFileOperate
        {
            get { return this.excelFileOperate; }
            set { this.excelFileOperate = value; }
        }

        /// <summary>
        /// 待缓存的交互命令集
        /// </summary>
        public List<Command> CommandList
        {
            get { return this.commandList; }
            set { this.commandList = value; }
        }

        /// <summary>
        /// 查询时是否需要起止时间（针对动态引用标签）
        /// </summary>
        public bool NeedEndtime
        {
            get { return this.needEndtime; }
            set { this.needEndtime = value; }
        }

        /// <summary>
        /// 带节点报表查询时是否需要动态指定节点
        /// </summary>
        public bool NeedSelectDeviceNode
        {
            get { return needSelectDeviceNode; }
            set { needSelectDeviceNode = value; }
        }

        /// <summary>
        /// 查询时选择设备节点的节点id
        /// </summary>
        public uint SysNodeID
        {
            get { return sysNodeID; }
            set { sysNodeID = value; }
        }

        /// <summary>
        /// 查询时选择设备节点的节点type
        /// </summary>
        public uint SysNodeType
        {
            get { return sysNodeType; }
            set { sysNodeType = value; }
        }

        #endregion

        public ProcessExcelTempFile(string path,uint userID,string userName)
        {
            excelFileOperate = new ExcelFileOperate(path);
            this.userID = userID;
            this.userName = userName;
        }

        #region 类外部对报表生成过程的主要操作步骤

        /// <summary>
        /// 解析数据源标签,放入结构体中存储
        /// </summary>
        public bool AnalyzeDataSourceLable()
        {
            try
            {
                Excel.Worksheet sheet = GetDatasourceSheet();
                if (sheet == null)
                    return false;
                //搜索数据源标签
                List<Excel.Range> datasourceLabelRangeList = GetLabelRangeList(sheet, "##");
                bool isUserEndTime = false;
                foreach (var range in datasourceLabelRangeList)
                {
                    //if (range.Value2.ToString().ToUpper().Contains("ENDTIME"))
                    //{
                       // isUserEndTime = true;
                        //// continue;这个continue导致SavedReport不能用，注释掉。后面再使用普通报表的时段报表验证功能是否正常
                   // }
                    SetDSLStruct(range);
                }
                if (dataSourceLabelList.Count == 0)
                    return false;
                dataSourceLabelArray = new DataSourceLabel[dataSourceLabelList.Count]; //将arraylist转换为数组
                dataSourceLabelList.CopyTo(dataSourceLabelArray);
                //if (isUserEndTime && !dataSourceLabelList[0].isQueryCondition && !IsDefaultTemplateOrSaved()) //只有普通报表要设置这个isIncludeAvg = true；如果是Saved Report，不能这样设置
                    //dataSourceLabelArray[0].entireRSParms.isIncludeAvg = true; 由于还不确定是哪种普通报表需要这样的设置，因此暂时先把这里注释掉，等朱全丰确定后再加上
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 将程序中设置的标签参数写入到模板文件中
        /// </summary>
        public bool SetTempFileDSL()
        {
            try
            {
                Excel.Worksheet objSheet = GetDatasourceSheet();
                if (objSheet == null)
                {
                    objSheet = (Excel.Worksheet)excelFileOperate.ObjBook.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    objSheet.Name = "datasource";
                }
                ((Excel.Range)objSheet.Cells[1, 1]).EntireColumn.Clear(); //清除datasource中第一列标签并进行重写 
                Excel.Range objRange = objSheet.Cells[1, 1];
                objRange.Value = "##[Version=3.5.0.0]";
                objRange = objRange.Offset[1, 0];
                for (int j = 0; j < DataSourceLabelArray.Length; j++)
                {
                    string TotalDSLString = "##" + DataSourceLabelArray[j].entireDSLString;
                    objRange.Value = (object)TotalDSLString;
                    objRange = objRange.Offset[1, 0];
                }

                objSheet.Visible = Excel.XlSheetVisibility.xlSheetHidden;
                excelFileOperate.ObjBook.Save();
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 由DataSourceLabel中的各字段整合得到长字符串
        /// </summary>
        /// <param name="datasourcelabel"></param>
        public void SetDSLStringFromOtherParms(ref DataSourceLabel datasourcelabel)
        {
            datasourcelabel.entireDSLString = "[Name=" + datasourcelabel.name + "][Source=" + datasourcelabel.source
                + "][DateSpan=" + datasourcelabel.dateSpan + "][querySpan=" + (int)datasourcelabel.querySpan +
                "][interval=" + (int)datasourcelabel.interval + "][dateOffsetType=" + (int)datasourcelabel.dateOffsetType + "]";
            if (!string.IsNullOrEmpty(datasourcelabel.dateOffSet))
            {
                datasourcelabel.entireDSLString += "[dateOffSet=" + datasourcelabel.dateOffSet + "]";
            }
            if (!string.IsNullOrEmpty(datasourcelabel.node))
            {
                datasourcelabel.entireDSLString += "[node=" + datasourcelabel.node + "]";
            }
        }

        /// <summary>
        /// 将输入时间进行偏移处理
        /// </summary>
        /// <param name="starttime"></param>
        public bool AnalyzeOffsetDate(object objStartTime, object objEndTime)
        {
            try
            {
                DateTime startTime = Convert.ToDateTime(objStartTime);
                DateTime endTime = Convert.ToDateTime(objEndTime);
                //如为reporting service报表，则定制查询
                if (DataSourceLabelArray[0].excelType == FileType.ReportingServiceType)
                {
                    DataSourceLabelArray[0].startTime = startTime;  //为实时输入时间 
                    DataSourceLabelArray[0].endTime = endTime;
                    return true;
                }
                //查询结果结构体与数据源结构体一一对应，为设置NotRealWanted参数
                StaticResultsArray = new DataResults[DataSourceLabelArray.Length];
                for (int i = 0; i < DataSourceLabelArray.Length; i++)
                {
                    DataSourceLabelArray[i].reportNode = ReportFileManager.GetInstance().FindTargetReportNode(DataSourceLabelArray[i].source);
                    int reportFlag = 0;
                    DataSourceLabelArray[i].reportType = ReportFileManager.GetReportQueryType(DataSourceLabelArray[i].reportNode, ref reportFlag);
                    SetDSLExcelType(startTime, endTime, ref DataSourceLabelArray[i]);
                    //以下3类报表则跳过后面的内容
                    if (DataSourceLabelArray[i].reportType == ReportQueryType.ReportPeriod ||
                        DataSourceLabelArray[i].reportType == ReportQueryType.ReportPeriodTemplate ||
                        DataSourceLabelArray[i].reportType == ReportQueryType.ReportFlexibleTemplate)
                        continue;

                    if (!DateOffSetParmProc(ref DataSourceLabelArray[i], ref StaticResultsArray[i], startTime, endTime))
                        return false;
                    DataSourceLabelArray[i].startTime = FormatTime(DataSourceLabelArray[i].startTime, DataSourceLabelArray[i].excelType);
                    DataSourceLabelArray[i].endTime = FormatTime(DataSourceLabelArray[i].endTime, DataSourceLabelArray[i].excelType);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace); 
                return false;
            }
        }

        public List<string> GetSheetName()
        {
            List<string> sheetNameList = new List<string>();
            try
            {
                for (int i = 1; i <= excelFileOperate.ObjBook.Worksheets.Count; i++)
                {
                    Excel.Worksheet objSheet = (Excel.Worksheet)(excelFileOperate.ObjBook.Worksheets.get_Item(i));
                    string sheetname = objSheet.Name;
                    sheetNameList.Add(sheetname);
                }
                return sheetNameList;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return sheetNameList;
            }
        }
        public List<List<DataRetrievalLabel>> GetReferLabelList()
        {
            List<List<DataRetrievalLabel>> dataLabelList = new List<List<DataRetrievalLabel>>();
            try
            {
                for (int i = 1; i <= excelFileOperate.ObjBook.Worksheets.Count; i++)
                {
                    List<DataRetrievalLabel> tempList = new List<DataRetrievalLabel>();
                    Excel.Worksheet objSheet = (Excel.Worksheet)(excelFileOperate.ObjBook.Worksheets.get_Item(i));
                    string sheetname = objSheet.Name;
                    //if (sheetname.ToUpper() == "DATASOURCE")
                    //    continue;
                    //搜索静态引用标签
                    List<Excel.Range> dataRetrievalLableRangeList = GetLabelRangeList(objSheet, "@DataSet");
                    foreach (var range in dataRetrievalLableRangeList)
                    {
                        DataRetrievalLabel temp = new DataRetrievalLabel();
                        temp.rowPosition = range.Row;
                        temp.columnPosition = range.Column;
                        temp.name = range.Value2;
                        tempList.Add(temp);
                    }
                    //搜索动态引用标签（兼容之前的"@AutoSet"）
                    dataRetrievalLableRangeList = GetLabelRangeList(objSheet, "@AutoSet", "@AutoDataSet");
                    foreach (var range in dataRetrievalLableRangeList)
                    {
                        DataRetrievalLabel temp = new DataRetrievalLabel();
                        temp.rowPosition = range.Row;
                        temp.columnPosition = range.Column;
                        temp.name = range.Value2;
                        tempList.Add(temp);
                    }
                    dataLabelList.Add(tempList);
                }
                return dataLabelList;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return dataLabelList;
            }
        }

        /// <summary>
        /// 解析静态引用标签,放入结构体中存储
        /// </summary>
        public bool AnalyzeRetrievalLable()
        {
            try
            {
                List<DataRetrievalLabel> inserReferLabelList = new List<DataRetrievalLabel>();
                for (int i = 1; i <= excelFileOperate.ObjBook.Worksheets.Count; i++)
                {
                    Excel.Worksheet objSheet = (Excel.Worksheet)(excelFileOperate.ObjBook.Worksheets.get_Item(i));
                    string sheetname = objSheet.Name;
                    //搜索静态引用标签
                    List<Excel.Range> dataRetrievalLableRangeList = GetLabelRangeList(objSheet, "@DataSet");
                    foreach (var range in dataRetrievalLableRangeList)
                    {
                        if (range.Value2.ToString().ToUpper().Contains("INSERT"))
                            SetDataRetrievalLableValue(sheetname, ref inserReferLabelList, range);
                        else
                            SetDataRetrievalLableValue(sheetname, ref staticRetrievalLabelList, range);
                    }
                    //搜索动态引用标签（兼容之前的"@AutoSet"）
                    dataRetrievalLableRangeList = GetLabelRangeList(objSheet, "@AutoSet", "@AutoDataSet");
                    foreach (var range in dataRetrievalLableRangeList)
                    {
                        SetDataRetrievalLableValue(sheetname, ref dynamicRetrievalLabelList, range);
                    }
                }
                foreach (var DataRetrievalLabel in inserReferLabelList)                
                {
                    staticRetrievalLabelList.Add(DataRetrievalLabel);
                }
                //若存在动态引用标签则记录，供加载界面时使用
                if (dynamicRetrievalLabelList.Count > 0)
                    needEndtime = true;
                //若无引用标签则返回false，因为这样的模板文件没有意义
                if (StaticRetrievalLabelList.Count == 0 && DynamicRetrievalLabelList.Count == 0)
                    return false;
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 获得静态引用标签查询结果
        /// </summary>
        /// <returns></returns>
        public bool GetStaticCheckResult()
        {
            commandList = new List<Command>();
            for (int i = 0; i < DataSourceLabelArray.Length; i++)
            {
                switch (DataSourceLabelArray[i].excelType)
                {
                    case FileType.PeriodType:
                    case FileType.FlexibleType:
                        if (GetPeriodResultData(DataSourceLabelArray[i], ref StaticResultsArray[i]).dataMusterNum == 0)
                            return false;
                        break;
                    default:
                        GetCommRepResultsAndResetNextTime(ref DataSourceLabelArray[i], ref StaticResultsArray[i], DataSourceLabelArray[i].startTime, this.needSelectDeviceNode, this.sysNodeType, this.sysNodeID, ref commandList);
                        break;
                }               
            }
            if (commandList.Count != 0)   //存在缓存数据缺失
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 将时段报表整合入中间数据集ResultsArray
        /// </summary>
        /// <param name="dataSourceLabel"></param>
        /// <param name="dataResult"></param>
        /// <returns></returns>
        private DataResults GetPeriodResultData(DataSourceLabel dataSourceLabel, ref DataResults dataResult)
        {
            PeriodReportPro periodreportpro = new PeriodReportPro(dataSourceLabel);
            periodreportpro.NodeType = this.sysNodeType;
            periodreportpro.NodeID = this.sysNodeID;
            dataResult = periodreportpro.ManagePeriodReport();
            return dataResult;
        }       

        /// <summary>
        /// 将动态报表整合入中间数据集ResultsArray(动态引用标签连续查询)
        /// </summary>
        /// <param name="dataSourceLabel"></param>
        /// <param name="dataResult"></param>
        /// <param name="commandList"></param>
        private void GetFlexibleResults(DataSourceLabel dataSourceLabel, ref DataResults dataResult, DateTime startTime, bool needDeviceNode, uint sysNodeType, uint sysNodeID, ref List<Command> commandList)
        {
            FlexibleReportPro.GetInstance().FlexRepDateSourceLable = dataSourceLabel;
            dataResult = FlexibleReportPro.GetInstance().ManageFlexibleTemplate(startTime, startTime.AddDays(1), needDeviceNode, sysNodeType, sysNodeID, dataSourceLabel,ref  commandList);
        }

        /// <summary>
        /// 获得动态引用标签查询结果
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public bool GetAllDynamicResults(DateTime starttime, DateTime endtime)
        {
            DateTime starttime2 = FormatTime(starttime, FileType.DayType);//目前默认日类型查询
            DateTime endtime2 = FormatTime(endtime, FileType.DayType);
            while (starttime2 <= endtime2)
            {
                GetEachDynamicResult(starttime2);
                AllDynamicResults.Add(eachDynamicResultList);
                starttime2 = starttime2.AddDays(1);   //目前默认间隔日类型查询
            }
            if (commandList.Count != 0)    //存在缓存数据缺失
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获得动态引用标签（一天的）查询结果
        /// </summary>
        /// <returns></returns>
        public void GetEachDynamicResult(DateTime starttime)
        {
            eachDynamicResultList = new List<DataResults>();
            //根据动态引用标签来对相应的数据源标签进行数据集查询            
            for (int j = 0; j < DynamicRetrievalLabelList.Count; j++)
            {
                if (!NeedCheckData(eachDynamicResultList, DynamicRetrievalLabelList[j]))
                    continue;
                for (int i = 0; i < DataSourceLabelArray.Length; i++)
                {
                    if (DataSourceLabelArray[i].name != DynamicRetrievalLabelList[j].name)
                        continue;
                    GetDynamicResultProc(DataSourceLabelArray[i], starttime, ref commandList, ref eachDynamicResultList);
                }
            }
        }

        /// <summary>
        /// 执行动态引用标签的查询
        /// </summary>
        /// <param name="dataSourceLabel"></param>
        /// <param name="starttime"></param>
        /// <param name="commandList"></param>
        private void GetDynamicResultProc(DataSourceLabel dataSourceLabel, DateTime starttime, ref List<Command> commandList, ref List<DataResults> eachDynamicResultList)
        {
            DataResults interimDCO = new DataResults();
            if (dataSourceLabel.excelType == FileType.FlexibleType)   //动态引用标签模板的动态选择设备节点还没有仔细考虑
                GetFlexibleResults(dataSourceLabel, ref interimDCO, starttime, this.needSelectDeviceNode, this.sysNodeType, this.sysNodeID, ref commandList);
            else
                GetCommRepResultsAndResetNextTime(ref dataSourceLabel, ref interimDCO, starttime, this.needSelectDeviceNode, this.sysNodeType, this.sysNodeID, ref commandList);
            eachDynamicResultList.Add(interimDCO);
        }

        /// <summary>
        /// 获得reporting service报表查询结果
        /// </summary>
        /// <returns></returns>
        public bool GetReportServiceResult()
        {
            try
            {
                if (DataSourceLabelArray[0].excelType != FileType.ReportingServiceType)
                    return false;
                string errorMessage;
                List<DataTable> resultDT = RepSerDataOperation.GetRepSerData(DataSourceLabelArray[0],out errorMessage ,userID,userName);
                if (resultDT==null||(resultDT.Count==0&& (!string.IsNullOrEmpty(errorMessage))))
                {
                    DbgTrace.dout("获取报表数据失败。");
                    ErrorMessage = errorMessage;
                    return false;
                }
                DbgTrace.dout("获取报表数据成功。");
                StaticResultsArray = new DataResults[resultDT.Count];
                for (int i = 0; i < resultDT.Count; i++)
                {
                    StaticResultsArray[i].name = (i + 1).ToString();   //由于结果可能包含若干个datatable，因此强制定义标签Name为“1”，“2”...
                    StaticResultsArray[i].dataMusterNum = 1;
                    StaticResultsArray[i].dataMusterArray = new List<List<string>>[1];
                    StaticResultsArray[i].dataMusterArray[0] = DataTableToList(resultDT[i], dataSourceLabelList[0].source);
                    StaticResultsArray[i].dataMusterParamDiscribtionArray = new DataMusterParamDiscribtion[1];
                    StaticResultsArray[i].dataMusterParamDiscribtionArray[0] = DataMusterParamDiscribtion.DefaultReport;//标识该数据集为ReportingService报表数据集
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
        /// 将datatable转化为二维list
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private List<List<string>> DataTableToList(DataTable dataTable,uint repServFileType)
        {
            List<string> ColumnNamesList = GetColumnNames(dataTable.Columns);
            List<List<string>> resultlist = new List<List<string>>();
            foreach (DataRow row in dataTable.Rows)
            {
                List<string> OneRow = new List<string>(ColumnNamesList.Count);
                foreach (string columnname in ColumnNamesList)
                {
                   
                    string strvalue = row[columnname].ToString();
                 
                    OneRow.Add(strvalue);
                }
                resultlist.Add(OneRow);
            }
            return resultlist;
        }

        /// <summary>
        /// 给时间引用标签赋值
        /// </summary>
        /// <param name="StartTime"></param>
        public void SetCheckDateTime(object starttime, object endtime, string userName)
        {
            DateTime StartTime = Convert.ToDateTime(starttime);
            DateTime EndTime = Convert.ToDateTime(endtime);
            for (int i = 1; i <= excelFileOperate.ObjBook.Worksheets.Count; i++) //查找含有@Date的sheet
            {
                excelFileOperate.ObjSheet = (Excel.Worksheet) (excelFileOperate.ObjBook.Worksheets.get_Item(i));
                SetTime(StartTime, "Start", userName);
                if (endtime != null)
                {
                    SetTime(EndTime, "End", userName);
                }
            }
        }

        /// <summary>
        /// 根据引用标签将查询结果导入到excel中
        /// </summary>
        /// <returns></returns>
        public bool CreateExcel(int Mode, object ObjStartTime, object ObjEndTime)
        {
            try
            {
                string modeString = "1-手动模式";
                if (Mode == 2)
                    modeString = "2-自动模式";
                DbgTrace.dout("根据引用标签将查询结果导入到excel中。起始时间是 {0} ；结束时间是 {1} 。{2}。",ObjStartTime,ObjEndTime,modeString);
                DateTime StartTime = Convert.ToDateTime(ObjStartTime);
                DateTime EndTime = Convert.ToDateTime(ObjEndTime);
                switch (Mode)
                {
                    case 1:
                        return (FillStaticData() && FillDynamicData(StartTime, EndTime));
                    case 2:
                        return FillStaticData();
                    default:
                        return false;
                }
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace); 
                return false;
            }
           
        }

        #endregion

        #region 类内部对报表生成过程的其他相关操作（包括一些特殊的公有操作）

        /// <summary>
        /// 搜索含有关键字（标签）的单元格
        /// </summary>
        /// <param name="keyStr"></param>
        /// <returns></returns>
        private List<Excel.Range> GetLabelRangeList(Excel.Worksheet objSheet, params string[] keyStr)
        {
            List<Excel.Range> dataSourceLabelRangeList = new List<Excel.Range>();
            foreach (string keyString in keyStr)
            {
                Excel.Range item = (Excel.Range)objSheet.UsedRange.Find(keyString, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Excel.XlSearchDirection.xlNext, Type.Missing, Type.Missing, Type.Missing);
                if (item == null)
                    continue;
                RecordDataSourceLabelRange(objSheet, ref dataSourceLabelRangeList, item);
            }
            return dataSourceLabelRangeList;
        }

        /// <summary>
        /// 记录所有的数据源标签
        /// </summary>
        /// <param name="objSheet"></param>
        /// <param name="dataSourceLabelRangeList"></param>
        /// <param name="item"></param>
        private void RecordDataSourceLabelRange(Excel.Worksheet objSheet, ref List<Excel.Range> dataSourceLabelRangeList, Excel.Range item)
        {
            int firstRangeRow = item.Row;
            int firstRangeColumn = item.Column;
            do
            {
                dataSourceLabelRangeList.Add(item);
                item = objSheet.UsedRange.FindNext(item);
            }
            while (item.Column != firstRangeColumn || item.Row != firstRangeRow);
        }

        /// <summary>
        /// 检索datasource Sheet页
        /// </summary>
        /// <returns></returns>
        private Excel.Worksheet GetDatasourceSheet()
        {
            try
            {
                for (int i = 1; i <= excelFileOperate.ObjBook.Worksheets.Count; i++)
                {
                    Excel.Worksheet objSheet = (Excel.Worksheet)(excelFileOperate.ObjBook.Worksheets.get_Item(i));
                    if (objSheet.Name.ToUpper() == "DATASOURCE")
                        return objSheet;
                }
                return null;
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return null;
            }
         
        }

        /// <summary>
        /// 根据报表类型设置时间偏移类型
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="datasourceLabel"></param>
        private void SetDSLExcelType(DateTime startTime, DateTime endTime, ref DataSourceLabel datasourceLabel)
        {
            if (datasourceLabel.reportNode.NodeType == ReportNodeType.REPORTPECSTAR)  //pecstar类型报表
                SetPecsRepExcelType(startTime, endTime, ref datasourceLabel);
            else if (datasourceLabel.reportNode.NodeType == ReportNodeType.REPORTENGSYS)  //EEM类型报表
                SetEEMRepExcelType(ref datasourceLabel);
            //PQ类型报表还未考虑
        }

        /// <summary>
        /// 设置pecs报表的excelType参数
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="datasourceLabel"></param>
        private void SetPecsRepExcelType(DateTime startTime, DateTime endTime, ref DataSourceLabel datasourceLabel)
        {
            switch (datasourceLabel.reportType)
            {
                case ReportQueryType.ReportDay:
                case ReportQueryType.ReportDayTemplate:
                    datasourceLabel.excelType = FileType.DayType;  //日累加
                    break;
                case ReportQueryType.ReportWeek:
                case ReportQueryType.ReportWeekTemplate:
                    datasourceLabel.excelType = FileType.WeekType;  //周累加
                    break;
                case ReportQueryType.ReportMonth:
                case ReportQueryType.ReportMonthTemplate:
                    datasourceLabel.excelType = FileType.MonthType;  //月累加
                    break;
                case ReportQueryType.ReportYear:
                case ReportQueryType.ReportYearTemplate:
                    datasourceLabel.excelType = FileType.YearType;  //年累加
                    break;
                case ReportQueryType.ReportPeriod:
                case ReportQueryType.ReportPeriodTemplate:  //时段报表不考虑连续偏移时间查询多次，即DataNum=1及不设置OffsetDate
                    datasourceLabel.excelType = FileType.PeriodType;  //是否为时段报表的标识，以此在GetCheckResult()中进行数据集整合
                    datasourceLabel.startTime = FormatTime(startTime, FileType.PeriodType);  //时段报表查询时间为实时输入时间 
                    datasourceLabel.endTime = FormatTime(endTime, FileType.PeriodType);
                    break;
                case ReportQueryType.ReportFlexibleTemplate:  //动态报表不考虑连续偏移时间查询多次，即DataNum=1及不设置OffsetDate
                    datasourceLabel.excelType = FileType.FlexibleType;  //是否为动态报表的标识，以此在GetCheckResult()中进行数据集整合
                    datasourceLabel.startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 1, 0, 0);  //动态报表起始查询时间为实时输入时间 
                    GetEndTimeForFlexibleTemplate(endTime, ref datasourceLabel);
                    break;
            }
        }

        /// <summary>
        /// 根据查询起始时间和时间跨度类型获得结束时间
        /// </summary>
        /// <param name="endtime"></param>
        /// <param name="datasourcelabel"></param>
        private void GetEndTimeForFlexibleTemplate(DateTime endtime, ref DataSourceLabel datasourcelabel)
        {
            switch (datasourcelabel.querySpan)
            {
                case QuerySpan.Null:
                    datasourcelabel.endTime = endtime;
                    break;
                case QuerySpan.Day:
                    datasourcelabel.endTime = endtime;//datasourcelabel.startTime.AddDays(1);
                    break;
                case QuerySpan.Week:
                    datasourcelabel.endTime = datasourcelabel.startTime.AddDays(7);
                    break;
                case QuerySpan.Month:
                    datasourcelabel.endTime = datasourcelabel.startTime.AddMonths(1);
                    break;
                case QuerySpan.Quarter:
                    datasourcelabel.endTime = datasourcelabel.startTime.AddMonths(3);
                    break;
                case QuerySpan.Year:
                    datasourcelabel.endTime = datasourcelabel.startTime.AddYears(1);
                    break;
            }
        }

        /// <summary>
        /// 设置EEM报表的excelType参数
        /// </summary>
        /// <param name="datasourcelabel"></param>
        private void SetEEMRepExcelType(ref DataSourceLabel datasourcelabel)
        {
            switch (datasourcelabel.reportType)
            {
                case ReportQueryType.ReportMonthTemplate:
                    datasourcelabel.excelType = FileType.MonthType;  //综合月报表，月累加
                    break;
                case ReportQueryType.ReportWeekTemplate:
                    datasourcelabel.excelType = FileType.YearType;  //综合年报表，年累加
                    break;
            }
        }

        /// <summary>
        /// 处理DateOffSet参数
        /// </summary>
        /// <param name="i"></param>
        /// <param name="startTime"></param>
        private bool DateOffSetParmProc(ref DataSourceLabel datasourceLabel, ref DataResults dataCheckOut, DateTime startTime, DateTime endTime)
        {
            //OffsetDate为空则查询时间无偏移，直接启用输入查询时间针对不同类型报表的标准形式
            if (string.IsNullOrEmpty(datasourceLabel.dateOffSet))
            {
                datasourceLabel.startTime = startTime;
                datasourceLabel.endTime = endTime;
                return true;
            }
            if (datasourceLabel.dateOffsetType == DateOffSetType.AbsoluteOffset)
                return SetResultTimeByAbsolute(ref datasourceLabel, ref dataCheckOut, startTime, endTime);
            else
                return SetTimeByRelative(ref datasourceLabel, startTime, endTime);
        }

        private bool SetResultTimeByAbsolute(ref DataSourceLabel datasourceLabel, ref DataResults dataCheckOut, DateTime startTime, DateTime endTime)
        {
            string[] yearMonthDayStr = datasourceLabel.dateOffSet.Split('/');
            //对年、月、日偏移进行校验
            if (Convert.ToInt32(yearMonthDayStr[2]) < 1 || Convert.ToInt32(yearMonthDayStr[2]) > 31 || Convert.ToInt32(yearMonthDayStr[1]) < 1 || Convert.ToInt32(yearMonthDayStr[1]) > 12
                || Convert.ToInt32(yearMonthDayStr[0]) < 1900 || Convert.ToInt32(yearMonthDayStr[0]) > 9999)
                return false;
            switch (datasourceLabel.excelType)
            {
                case FileType.DayType:
                    datasourceLabel.startTime = FormatTime(startTime, FileType.MonthType).AddDays(Convert.ToInt32(yearMonthDayStr[2]) - 1);
                    datasourceLabel.endTime = datasourceLabel.startTime.AddDays(TimeDiff(startTime, endTime, FileType.DayType) - 1);
                    if (datasourceLabel.startTime.Month != startTime.Month)  //MM-31已偏移至下个月初
                        dataCheckOut.notRealWanted = true;  //标识后在填充excel文件时该数据集全部由“”替代
                    break;
                case FileType.WeekType:
                    string dateTimeString = string.Format("{0}-{1}-{2}", yearMonthDayStr[0], yearMonthDayStr[1], yearMonthDayStr[2]);
                    DateTime tempTime = Convert.ToDateTime(dateTimeString);
                    datasourceLabel.startTime = tempTime;
                    datasourceLabel.endTime = tempTime;
                    break;
                case FileType.MonthType:
                    datasourceLabel.startTime = FormatTime(startTime, FileType.YearType).AddMonths(Convert.ToInt32(yearMonthDayStr[1]) - 1);
                    datasourceLabel.endTime = datasourceLabel.startTime.AddMonths(TimeDiff(startTime, endTime, FileType.MonthType) - 1);
                    break;
                case FileType.YearType:
                    datasourceLabel.startTime = Convert.ToDateTime(yearMonthDayStr[0] + "-01-01 1:00:00");
                    datasourceLabel.endTime = datasourceLabel.startTime.AddYears(TimeDiff(startTime, endTime, FileType.YearType) - 1);
                    break;
            }
            return true;
        }

        private void PrintParameters( int dayOffset, int monthOffset,  int yearOffset, ref DataSourceLabel datasourceLabel, DateTime startTime)
        {
            DbgTrace.dout("DayOffset is {0} . ", dayOffset);
            DbgTrace.dout("MonthOffset is {0}",monthOffset);
            DbgTrace.dout("YearOffset is {0}",yearOffset);
            DbgTrace.dout("Input StartTime is {0}",startTime);
            DbgTrace.dout("StartTime is {0}", datasourceLabel.startTime);
            DbgTrace.dout("EndTime is {0}", datasourceLabel.endTime);
            DbgTrace.dout("DateSpan is {0}", datasourceLabel.dateSpan);
            DbgTrace.dout("ExcelType is {0}",datasourceLabel.excelType);
        }
        private void GetDataSpan(  int dayOffset, int monthOffset,  int yearOffset, ref DataSourceLabel datasourceLabel, DateTime startTime)
        {
            PrintParameters(dayOffset, monthOffset, yearOffset, ref datasourceLabel, startTime);
            if (dayOffset == 0  &&monthOffset!=0&& yearOffset == 0 &&
                   datasourceLabel.excelType == FileType.DayType)
            {
                DateTime tempTime = startTime.AddMonths(monthOffset);
                int daysCount = System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(tempTime.Year, tempTime.Month);
                datasourceLabel.dateSpan = daysCount;
                DbgTrace.dout("Change dateSpan 31 to {0} .",daysCount);

                datasourceLabel.startTime = new DateTime(tempTime.Year,tempTime.Month,1);
                datasourceLabel.endTime = datasourceLabel.startTime.AddDays(daysCount);
                DbgTrace.dout("After change:");
                PrintParameters(dayOffset, monthOffset, yearOffset, ref datasourceLabel, startTime);
            }
            
        }
        /// <summary>
        /// 设置标签的starttime参数
        /// </summary>
        /// <param name="datasourceLabel"></param>
        /// <param name="startTime"></param>
        private bool SetTimeByRelative(ref DataSourceLabel datasourceLabel, DateTime startTime, DateTime endTime)
        {
            try
            {
                string[] dateOffSetStr = datasourceLabel.dateOffSet.Split('/');

                int dayOffset = Convert.ToInt32(dateOffSetStr[2]);
                int monthOffset = Convert.ToInt32(dateOffSetStr[1]);
                int yearOffset = Convert.ToInt32(dateOffSetStr[0]);

                switch (datasourceLabel.excelType)
                {
                    case FileType.DayType:
                    case FileType.WeekType:
                        datasourceLabel.startTime = startTime.AddDays(dayOffset);
                        //自动发送时，null将处理为0001/1/1 0:0:0，此时再负偏移将出错
                        if (endTime != DateTime.MinValue)
                            datasourceLabel.endTime = startTime.AddDays(dayOffset);
                        break;
                    case FileType.MonthType:
                        datasourceLabel.startTime = startTime.AddMonths(monthOffset);
                        if (endTime != DateTime.MinValue)
                            datasourceLabel.endTime = startTime.AddMonths(monthOffset);
                        break;
                    case FileType.YearType:
                        datasourceLabel.startTime = startTime.AddYears(yearOffset);
                        if (endTime != DateTime.MinValue)
                            datasourceLabel.endTime = startTime.AddYears(yearOffset);
                        break;
                }
                GetDataSpan(dayOffset, monthOffset, yearOffset, ref datasourceLabel, startTime);
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 查询各种类型报表的结果集，并根据报表类型设置下次查询的时间
        /// </summary>
        /// <param name="datasourceLabel"></param>
        /// <param name="dataCheckOut"></param>
        /// <param name="commandList"></param>
        private void GetCommRepResultsAndResetNextTime(ref DataSourceLabel datasourceLabel, ref DataResults dataCheckOut, DateTime startTime, bool needDeviceNode, uint sysNodeType, uint sysNodeID, ref List<Command> commandList)
        {
            dataCheckOut.name = datasourceLabel.name;
            //根据起止时间动态构造dateSpan
            if (datasourceLabel.dateSpan == 0)
            {
                DbgTrace.dout("datasourceLabel.dateSpan == 0");
                datasourceLabel.dateSpan = TimeDiff(datasourceLabel.startTime, datasourceLabel.endTime, datasourceLabel.excelType);
                datasourceLabel.needDynamicFormula = true;
                DbgTrace.dout("datasourceLabel.dateSpan changed. It is {0}", datasourceLabel.dateSpan);
            }
            dataCheckOut.dataMusterNum = datasourceLabel.dateSpan;
            dataCheckOut.dataMusterParamDiscribtionArray = new DataMusterParamDiscribtion[datasourceLabel.dateSpan];
            dataCheckOut.dataMusterArray = new List<List<string>>[datasourceLabel.dateSpan];
            DbgTrace.dout("datasourceLabel.dateSpan is "+ datasourceLabel.dateSpan);
            for (int j = 1; j <= datasourceLabel.dateSpan; j++)
            {
                DbgTrace.dout("Get Common report result {0} . StartTime is {1} ." ,j,startTime);
                uint nodeType = 0;
                uint nodeID = 0;
                List<List<string>> resultlist = GetCommRepResultData(datasourceLabel, startTime, needDeviceNode, sysNodeType, sysNodeID, out nodeType, out nodeID);
                //查询未获得数据，则记录待缓存的信息并继续下一个数据集的查询
                if (resultlist.Count == 0)
                {
                    DbgTrace.dout("resultlist.Count == 0");
                    Command commandInfo = new Command(datasourceLabel.source, nodeType, nodeID, startTime);
                    if (!commandList.Contains(commandInfo))
                    {
                        commandList.Add(commandInfo);
                        DbgTrace.dout("commandList.Add(commandInfo);datasourceLabel.source={0}   nodeType={1}  nodeID={2}  startTime={3}", datasourceLabel.source, nodeType, nodeID, startTime);
                    }
                }
                else
                {
                    dataCheckOut.dataMusterArray[j - 1] = resultlist;
                    DbgTrace.dout("dataCheckOut.dataMusterArray[j - 1] = resultlist;j={0}",j);
                }
                    
                ResetStartTime(datasourceLabel, ref startTime);
            }
        }

        /// <summary>
        /// 计算两个日期之间的间隔
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        private int TimeDiff(DateTime dt1, DateTime dt2, FileType flag)
        {
            int span = 0;
            DateTime dateTime1;
            DateTime dateTime2;
            switch (flag)
            {
                case FileType.YearType:
                    dateTime1 = new DateTime(dt1.Year, 1, 1);
                    dateTime2 = new DateTime(dt2.Year, 1, 1);
                    while (dateTime1 <= dateTime2)
                    {
                        span++;
                        dateTime1 = dateTime1.AddYears(1);
                    }
                    break;
                case FileType.MonthType:
                    dateTime1 = new DateTime(dt1.Year, dt1.Month, 1);
                    dateTime2 = new DateTime(dt2.Year, dt2.Month, 1);
                    while (dateTime1 <= dateTime2)
                    {
                        span++;
                        dateTime1 = dateTime1.AddMonths(1);
                    }
                    break;
                case FileType.FlexibleType:
                case FileType.DayType:
                    dateTime1 = new DateTime(dt1.Year, dt1.Month, dt1.Day);
                    dateTime2 = new DateTime(dt2.Year, dt2.Month, dt2.Day);
                    while (dateTime1 <= dateTime2)
                    {
                        span++;
                        dateTime1 = dateTime1.AddDays(1);
                    }
                    break;
            }
            return span;
        }

        /// <summary>
        /// 获取报表查询结果集
        /// </summary>
        /// <param name="dataSourceLabel"></param>
        /// <param name="starttime"></param>
        /// <param name="cacheManager"></param>
        /// <returns></returns>
        private List<List<string>> GetCommRepResultData(DataSourceLabel dataSourceLabel, DateTime starttime, bool needDeviceNode, uint sysNodeType, uint sysNodeID, out uint nodeType, out uint nodeID)
        {
            //模板报表用GetReportResultByNodeID查询
            if (dataSourceLabel.reportNode.NodeType == 272760832 && (int)dataSourceLabel.reportType > 4)
                return GetTemplateRepData(dataSourceLabel, starttime, needDeviceNode, sysNodeType, sysNodeID, out nodeType, out nodeID);
            //普通报表用GetReportResultByStartEndCell查询
            else
            {
                nodeType = 0;
                nodeID = 0;
                PecReportCacheManager.ReportCacheManager cacheManager = new PecReportCacheManager.ReportCacheManager();
                var node= cacheManager.GetReportResultByStartEndCell(dataSourceLabel.source, starttime, (int)dataSourceLabel.excelType, "", "");
                return node;
            }
        }

        /// <summary>
        /// 获取模板报表的查询数据
        /// </summary>
        /// <param name="dataSourceLabel"></param>
        /// <param name="starttime"></param>
        /// <returns></returns>
        private List<List<string>> GetTemplateRepData(DataSourceLabel dataSourceLabel, DateTime starttime, bool needDeviceNode, uint sysNodeType, uint sysNodeID, out uint nodeType, out uint nodeID)
        {
            string nodeIDParam = dataSourceLabel.node;
            if (string.IsNullOrEmpty(nodeIDParam) && needDeviceNode)
            {
                nodeType = sysNodeType;
                nodeID = sysNodeID;
            }
            else
            {
                string[] dataSourceStr = nodeIDParam.Split('(', ')');
                nodeType = Convert.ToUInt32(dataSourceStr[1]);
                nodeID = Convert.ToUInt32(dataSourceStr[2]);
                SysNode sysNode = PecsNodeManager.PecsNodeInstance.GetNodeByTypeID(nodeType, nodeID);
                if (sysNode == null && needDeviceNode)
                {
                    nodeType = sysNodeType;
                    nodeID = sysNodeID;
                }
            }

            if (dataSourceLabel.excelType == FileType.FlexibleType)
                dataSourceLabel.excelType = FileType.DayType;
            PecReportCacheManager.ReportCacheManager cacheManager = new PecReportCacheManager.ReportCacheManager();
            return cacheManager.GetReportResultByNodeID(dataSourceLabel.source, nodeType, nodeID, starttime, (int)dataSourceLabel.excelType);
        }

        /// <summary>
        /// 重设下次查询的starttime参数
        /// </summary>
        /// <param name="datasourcelabel"></param>
        private void ResetStartTime(DataSourceLabel datasourcelabel, ref DateTime startTime)
        {
            switch (datasourcelabel.excelType)
            {
                case FileType.DayType:
                    startTime = startTime.AddDays(1);
                    break;
                case FileType.WeekType:
                    startTime = startTime.AddDays(7);
                    break;
                case FileType.MonthType:
                    startTime = startTime.AddMonths(1);
                    break;
                case FileType.YearType:
                    startTime = startTime.AddYears(1);
                    break;
                case FileType.FlexibleType:
                    startTime = startTime.AddDays(1);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 标识是否需要查询数据
        /// </summary>
        /// <param name="eachDynamicResultsList"></param>
        /// <param name="dataretrievallabel"></param>
        /// <returns></returns>
        private bool NeedCheckData(List<DataResults> eachDynamicResultsList, DataRetrievalLabel dataretrievallabel)
        {
            //检索数据源标签是否已经查询，避免多个引用标签对应的同一个数据源标签被重复查询
            for (int n = 0; n < eachDynamicResultsList.Count; n++)
            {
                if (((DataResults)eachDynamicResultsList[n]).name == dataretrievallabel.name)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 获取列名
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private List<string> GetColumnNames(DataColumnCollection columns)
        {
            List<string> strlist = new List<string>();
            foreach (DataColumn column in columns)
            {
                strlist.Add(column.ColumnName);
            }
            return strlist;
        }



        /// <summary>
        /// 添加数据源标签到标签序列
        /// </summary>
        private bool SetDSLStruct(Excel.Range item)
        {
            DataSourceLabel dataSourceLabel = new DataSourceLabel();  //数据源标签结构体
            dataSourceLabel.entireDSLString = item.Value2.ToString().Substring(2, item.Value2.ToString().Length - 2);
            //DbgTrace.dout("完整的字符串是：{0}", string.Format(@"{0}", dataSourceLabel.entireDSLString));
            if (dataSourceLabel.entireDSLString.ToUpper().Contains("VERSION"))
                return false;
            GetEveryParmFromDSLString(ref dataSourceLabel);
            dataSourceLabelList.Add(dataSourceLabel);
            return true;
        }

        /// <summary>
        /// 根据数据源标签中的DSLString参数构建DataSourceLabel结构体
        /// </summary>
        private void GetEveryParmFromDSLString(ref DataSourceLabel datasourcelabel)
        {
            string[] dataSourceStr = datasourcelabel.entireDSLString.Split('[', ']');
            GetItemsValueToStruct(ref datasourcelabel, dataSourceStr);
            SetDatasourceLabelNode(ref datasourcelabel);
            SetStartTimeAndEndTime(ref datasourcelabel, dataSourceStr);
        }

        private bool SetStartTimeAndEndTime(ref DataSourceLabel datasourcelabel, string[] dataSourceStr)
        {
            if (!datasourcelabel.isQueryCondition)
                return true;
            //有[SelectedStartTime={3}][SelectedEndTime={4}][TimePeriodSelectedIndex={5}]的情况
            if (IsTimeCaseOne(datasourcelabel.source ))
            {
                return StartTimeAndEndTimeWithSelectedIndex(ref datasourcelabel, dataSourceStr);
            }

            //只有一个日期的情况
            if (IsTimeCaseTwo(datasourcelabel.source))
            {
                return StartTimeAndEndTimeWithSelectedIndex(ref datasourcelabel, dataSourceStr);
            }

            //Multiple Device Usage,Single Device Usage的时间获取的情况
            if (IsTimeCaseThree(datasourcelabel.source))
            {
                return StartTimeAndEndTimeCaseThree(ref datasourcelabel, dataSourceStr);
            }

            if (IsEnergyPeriodCase(datasourcelabel.source))
            {
                return TimeListForEnergyPeriod(ref datasourcelabel, dataSourceStr);
            }

            return false;
        }

        private static bool TimeListForEnergyPeriod(ref DataSourceLabel datasourcelabel, string[] dataSourceStr)
        {
            CommonlyUsedString commonlyUsedString =  CommonlyUsedString.GetInstance();

            int comboBoxCompareNumberSelectedIndex;
            if (
                !GetTargetInStringArray(dataSourceStr, commonlyUsedString.COMBO_BOX_COMPARE_NUMBER_SELECTED_INDEX,
                    out comboBoxCompareNumberSelectedIndex))
                return false;


            DateTime dtpStartTimeValue;
            if (!GetTargetInStringArray(dataSourceStr, commonlyUsedString.DTP_START_TIME_VALUE, out dtpStartTimeValue))
                return false;
            DateTime dtpEndTimeValue;
            if (!GetTargetInStringArray(dataSourceStr, commonlyUsedString.DTP_END_TIME_VALUE, out dtpEndTimeValue))
                return false;

            int comboBoxReportTypeSelectedIndex;
            if (
                !GetTargetInStringArray(dataSourceStr, commonlyUsedString.COMBO_BOX_REPORT_TYPE_SELECTED_INDEX,
                    out comboBoxReportTypeSelectedIndex))
                return false;
            int comboBoxCompareTypeSelectedIndex;
            if (
                !GetTargetInStringArray(dataSourceStr, commonlyUsedString.COMBO_BOX_COMPARE_TYPE_SELECTED_INDEX,
                    out comboBoxCompareTypeSelectedIndex))
                return false;

            string comboBoxYear2Text;
            if (!GetTargetInStringArray(dataSourceStr, commonlyUsedString.COMBO_BOX_YEAR2_TEXT, out comboBoxYear2Text))
                return false;

            int comboBoxMonth2SelectedIndex;
            if (
                !GetTargetInStringArray(dataSourceStr, commonlyUsedString.COMBO_BOX_MONTH2_SELECTED_INDEX,
                    out comboBoxMonth2SelectedIndex))
                return false;
            string comboBoxYear1Text;
            if (!GetTargetInStringArray(dataSourceStr, commonlyUsedString.COMBO_BOX_YEAR1_TEXT, out comboBoxYear1Text))
                return false;

            int comboBoxMonth1SelectedIndex;
            if (
                !GetTargetInStringArray(dataSourceStr, commonlyUsedString.COMBO_BOX_MONTH1_SELECTED_INDEX,
                    out comboBoxMonth1SelectedIndex))
                return false;

            List<DateTime> dateTimeListTemp;
            int compareNumber;
            if (
                !  DateTimeListForEnergyPeriod.GetDateTimeList(DateTime.Now, comboBoxCompareNumberSelectedIndex,
                    dtpStartTimeValue, dtpEndTimeValue, comboBoxReportTypeSelectedIndex,
                    comboBoxCompareTypeSelectedIndex, comboBoxYear2Text, comboBoxMonth2SelectedIndex,
                    comboBoxYear1Text, comboBoxMonth1SelectedIndex, out dateTimeListTemp, out compareNumber))
                return false;

            datasourcelabel.entireRSParms.dateTimeList = dateTimeListTemp;
            return true;
        }

        private static bool StartTimeAndEndTimeCaseThree(ref DataSourceLabel datasourcelabel, string[] dataSourceStr)
        {

            DateTime dtpStartTimeValue;
            if (!GetdtpStartTimeValueCaseThree(dataSourceStr, out dtpStartTimeValue))
                return false;
            string comboBoxYear1Text;
            if (!GetcomboBoxYear1TextCaseThree(dataSourceStr, out comboBoxYear1Text))
                return false;
            int comboBoxMonth1SelectedIndex;
            if (!GetcomboBoxMonth1SelectedIndexCaseThree(dataSourceStr, out comboBoxMonth1SelectedIndex))
                return false;
            int comboBoxReportTypeSelectedIndex;
            if (!GetcomboBoxReportTypeSelectedIndexCaseThree(dataSourceStr, out comboBoxReportTypeSelectedIndex))
                return false;
           

            int comboBoxReportPeriodSelectedIndex;
            if (
                !GetTargetInStringArray(dataSourceStr, "comboBoxReportPeriodSelectedIndex",
                    out comboBoxReportPeriodSelectedIndex))
                return false;
            DateTime dtpEndTimeValue;
            if (!GetTargetInStringArray(dataSourceStr, "dtpEndTimeValue", out dtpEndTimeValue))
                return false;
            string comboBoxYear2Text;
            if (!GetTargetInStringArray(dataSourceStr, "comboBoxYear2Text", out comboBoxYear2Text))
                return false;
            int comboBoxMonth2SelectedIndex;
            if (
                !GetTargetInStringArray(dataSourceStr, "comboBoxMonth2SelectedIndex",
                    out comboBoxMonth2SelectedIndex))
                return false;

            DateTime startTimeTemp = DateTime.MinValue;
            DateTime endTimeTemp = DateTime.MinValue;
            if (!SingleDeviceTimePeriodManager.GetTimeAll( dtpStartTimeValue,dtpEndTimeValue,comboBoxYear1Text, comboBoxMonth1SelectedIndex,
                comboBoxYear2Text, comboBoxMonth2SelectedIndex, comboBoxReportTypeSelectedIndex,
                comboBoxReportPeriodSelectedIndex, out startTimeTemp, out endTimeTemp))
            {
                datasourcelabel.startTime = DateTime.MinValue;
                datasourcelabel.endTime = DateTime.MinValue;
            }
            else
            {
                datasourcelabel.startTime = startTimeTemp;
                datasourcelabel.endTime = endTimeTemp;
            }
            return true;
        }

        private static bool GetcomboBoxReportTypeSelectedIndexCaseThree(string[] dataSourceStr, out int comboBoxReportTypeSelectedIndex)
        {
            return GetTargetInStringArray(dataSourceStr, "comboBoxReportTypeSelectedIndex",
                out comboBoxReportTypeSelectedIndex);
            //comboBoxReportTypeSelectedIndex = 0;
            //for (int j = 0; j < dataSourceStr.Length; j++)
            //{
            //    int index = dataSourceStr[j].IndexOf('=') + 1;
            //    if (index < 2)
            //        continue;
            //    string parmDescription = dataSourceStr[j].Substring(0, index - 1).ToUpper();
            //    if (parmDescription == "comboBoxReportTypeSelectedIndex".ToUpper())
            //    {
            //        comboBoxReportTypeSelectedIndex = Convert.ToInt32(dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index));
            //        return true;
            //    }
            //}
            //return false;
        }
        private static bool GetcomboBoxMonth1SelectedIndexCaseThree(string[] dataSourceStr, out int comboBoxMonth1SelectedIndex )
        {
            return GetTargetInStringArray(dataSourceStr, "comboBoxMonth1SelectedIndex", out comboBoxMonth1SelectedIndex);
            //comboBoxMonth1SelectedIndex=0;
            //for (int j = 0; j < dataSourceStr.Length; j++)
            //{
            //    int index = dataSourceStr[j].IndexOf('=') + 1;
            //    if (index < 2)
            //        continue;
            //    string parmDescription = dataSourceStr[j].Substring(0, index - 1).ToUpper();
            //    if (parmDescription == "comboBoxMonth1SelectedIndex".ToUpper())
            //    {
            //        comboBoxMonth1SelectedIndex = Convert.ToInt32(dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index));
            //        return true;
            //    }
            //}
            //return false;
        }
        private static bool GetdtpStartTimeValueCaseThree(string[] dataSourceStr, out DateTime dtpStartTimeValue)
        {
            return GetTargetInStringArray(dataSourceStr, "dtpStartTimeValue", out dtpStartTimeValue);
            //dtpStartTimeValue = DateTime.Now;
            //for (int j = 0; j < dataSourceStr.Length; j++)
            //{
            //    int index = dataSourceStr[j].IndexOf('=') + 1;
            //    if (index < 2)
            //        continue;
            //    string parmDescription = dataSourceStr[j].Substring(0, index - 1).ToUpper();
            //    if (parmDescription == "dtpStartTimeValue".ToUpper())
            //    {
            //        dtpStartTimeValue = Convert.ToDateTime(dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index));
            //        return true;
            //    }
            //}
            //return false;
        }
        public static bool GetTargetInStringArray(string[] dataSourceStr, string target, out DateTime result)
        {
            result = DateTime.MinValue;
            string tempResult;
            if (!GetTargetInStringArray(dataSourceStr, target, out tempResult))
                return false;
            return DateTime.TryParse(tempResult, out result);

        }

        public static bool GetTargetInStringArray(string[] dataSourceStr,string target, out string resultString)
        {
            resultString = string.Empty;
            try
            { 
                for (int j = 0; j < dataSourceStr.Length; j++)
                {
                    int index = dataSourceStr[j].IndexOf('=') + 1;
                    if (index < 2)
                        continue;
                    string parmDescription = dataSourceStr[j].Substring(0, index - 1).ToUpper();
                    if (parmDescription == target.ToUpper())
                    {
                        resultString = Convert.ToString(dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index));
                        return true;
                    }
                }
                return false;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        
        }

        private static bool GetTargetInStringArray(string[] dataSourceStr, string target, out bool result)
        {
            result =false;
            string resultStr;
            if (!GetTargetInStringArray(dataSourceStr, target, out resultStr))
                return false;
            return   bool.TryParse(resultStr, out result);
        }

        public static bool GetTargetInStringArray(string[] dataSourceStr, string target, out int result)
        {
            result = int.MinValue;
            string resultStr;
            if (!GetTargetInStringArray(dataSourceStr, target, out resultStr))
                return false;
            return int.TryParse(resultStr, out result);
        }
        public static bool GetTargetInStringArray(string[] dataSourceStr, string target, out uint result)
        {
            result = uint.MinValue;
            string resultStr;
            if (!GetTargetInStringArray(dataSourceStr, target, out resultStr))
                return false;
            return uint.TryParse(resultStr, out result);
        }

        private static bool GetcomboBoxYear1TextCaseThree(string[] dataSourceStr, out string comboBoxYear1Text)
        {
            return GetTargetInStringArray(dataSourceStr, "comboBoxYear1Text", out comboBoxYear1Text);
            //comboBoxYear1Text = string.Empty;
            //for (int j = 0; j < dataSourceStr.Length; j++)
            //{
            //    int index = dataSourceStr[j].IndexOf('=') + 1;
            //    if (index < 2)
            //        continue;
            //    string parmDescription = dataSourceStr[j].Substring(0, index - 1).ToUpper();
            //    if (parmDescription == "comboBoxYear1Text".ToUpper())
            //    {
            //        comboBoxYear1Text = Convert.ToString(dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index));
            //        return true;
            //    }
            //}
            //return false;
        }


        private static bool IsEnergyPeriodCase(uint source)
        {
            if (source == (uint)RepServFileType.EnergyPeriod)
                return true;
            return false;
        }
        

        private static bool IsTimeCaseThree(uint source)
        {
         
            if (source == (uint)RepServFileType.MultiUsage)
                return true;
            if (source == (uint)RepServFileType.SingleUsage)
                return true;
            return false;
        }


        private static bool StartTimeAndEndTimeWithDate(ref DataSourceLabel datasourcelabel, string[] dataSourceStr)
        {
            DateTime queryStartTime;

            if (!GetSelectedStartTime(dataSourceStr, out queryStartTime))
                return false;
            DateTime queryEndTime;
            if (!GetSelectedEndTime(dataSourceStr, out queryEndTime))
                return false;

            datasourcelabel.startTime = queryStartTime;
            datasourcelabel.endTime = queryEndTime;
            return true;
        }

        /// <summary>
        /// 只有一个日期的情况
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static bool IsTimeCaseTwo(uint source)
        {
            if (source == (uint)RepServFileType.HourlyUsage       )
                return true;
            return false;
        }


        /// <summary>
        /// 有[SelectedStartTime={3}][SelectedEndTime={4}][TimePeriodSelectedIndex={5}]的情况
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static bool IsTimeCaseOne(uint source)
        {
            if (source == (uint) RepServFileType.EnergyCost)
                return true;
            if (source == (uint) RepServFileType.EventHistory)
                return true;
            if (source == (uint) RepServFileType.LoadProfile)
                return true;
            if (source == (uint) RepServFileType.PowerQuality)
                return true;
            if (source == (uint)RepServFileType.PowerQualityEventsOnly)
                return true;
            if (source == (uint) RepServFileType.Tabular)
                return true;
            if (source == (uint) RepServFileType.Trend)
                return true;
            if (source == (uint)RepServFileType.Safety)
                return true;
            return false;
        }

        private static bool StartTimeAndEndTimeWithSelectedIndex(ref DataSourceLabel datasourcelabel, string[] dataSourceStr)
        {
            DateTime queryStartTime;
            DateTime queryEndTime;
    
            if (!GetStartTimeAndEndTimeWithSelectedIndex(out queryStartTime, out queryEndTime, dataSourceStr))
                return false;
            datasourcelabel.startTime = queryStartTime;
            datasourcelabel.endTime = queryEndTime;
            return true;
        }

        public static bool GetStartTimeAndEndTimeWithSelectedIndex(out DateTime startTime, out DateTime endTime, string[] dataSourceStr)
        {
            startTime = DateTime.MinValue;
            endTime = DateTime.MinValue;

            int timePeriodSelectedIndex;
            GetTimePeriodSelectedIndex(dataSourceStr, out timePeriodSelectedIndex);


            DateTime queryStartTime;
            DateTime queryEndTime;
            if (!CommonlyUsedForQueryCondition.GetStartTimeAndEndTimeBySelectedIndex(timePeriodSelectedIndex,
                out queryStartTime,
                out queryEndTime))
            {
                if (!GetSelectedStartTime(dataSourceStr, out queryStartTime))
                    return false;

                if (!GetSelectedEndTime(dataSourceStr, out queryEndTime))
                    return false;
            }

            startTime = queryStartTime;
            endTime = queryEndTime;
            return true;
        }

        public static bool GetTimePeriodSelectedIndex(string[] dataSourceStr, out int timePeriodSelectedIndex)
        {
            timePeriodSelectedIndex = 0;
            for (int j = 0; j < dataSourceStr.Length; j++)
            {
                int index = dataSourceStr[j].IndexOf('=') + 1;
                if (index < 2)
                    continue;
                string parmDescription = dataSourceStr[j].Substring(0, index - 1).ToUpper();
                if (parmDescription == "TimePeriodSelectedIndex".ToUpper())
                {
                    timePeriodSelectedIndex =   Convert.ToInt32(dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index));
                    return true;
                }
            }
            return false;
        }

        private static bool GetSelectedStartTime(string[] dataSourceStr, out DateTime startTime)
        {

            return GetTargetInStringArray(dataSourceStr, "SelectedStartTime", out startTime);
            //for (int j = 0; j < dataSourceStr.Length; j++)
            //{
            //    int index = dataSourceStr[j].IndexOf('=') + 1;
            //    if (index < 2)
            //        continue;
            //    string parmDescription = dataSourceStr[j].Substring(0, index - 1).ToUpper();
            //    if (parmDescription == "SelectedStartTime".ToUpper())
            //    {
            //        startTime = Convert.ToDateTime(dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index));
            //        return true;
            //    }
            //}
            //return false;
        }

        private static bool GetSelectedEndTime(string[] dataSourceStr, out DateTime endTime)
        {
            return GetTargetInStringArray(dataSourceStr, "SelectedEndTime", out endTime);
            //endTime = DateTime.Now;
            //for (int j = 0; j < dataSourceStr.Length; j++)
            //{
            //    int index = dataSourceStr[j].IndexOf('=') + 1;
            //    if (index < 2)
            //        continue;
            //    string parmDescription = dataSourceStr[j].Substring(0, index - 1).ToUpper();
            //    if (parmDescription == "SelectedEndTime".ToUpper())
            //    {
            //        endTime = Convert.ToDateTime(dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index));
            //        return true;
            //    }
            //}
            //return false;
        }

        private static bool GetStartAndEndTime(ref DataSourceLabel datasourcelabel, string parmDescription, string[] dataSourceStr, int j,
        int index)
        {
            if (parmDescription == "SelectedStartTime".ToUpper())
            {
                datasourcelabel.startTime = Convert.ToDateTime(dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index));
                return true;
            }
            if (parmDescription == "SelectedEndTime".ToUpper())
            {
                datasourcelabel.endTime = Convert.ToDateTime(dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index));
                return true;
            }
            if (parmDescription == "TimePeriodSelectedIndex".ToUpper())
            {
                int selectedIndex = Convert.ToInt32(dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index));
                DateTime queryStartTime;
                DateTime queryEndTime;
                if (CommonlyUsedForQueryCondition.GetStartTimeAndEndTimeBySelectedIndex(selectedIndex, out queryStartTime,
                    out queryEndTime))
                {
                    datasourcelabel.startTime = queryStartTime;
                    datasourcelabel.endTime = queryEndTime;
                }
                return true;
            }
            return false;
        }
        private void SetDatasourceLabelNode(ref DataSourceLabel datasourcelabel)
        {
            if (this.needSelectDeviceNode && datasourcelabel.node == null)
            {
                datasourcelabel.node = "(" + this.sysNodeType.ToString() + ")" + SysNodeID;
            }
        }

        private static void GetItemsValueToStruct(ref DataSourceLabel datasourcelabel, string[] dataSourceStr)
        {
            CommonlyUsedString commonlyUsedString = CommonlyUsedString.GetInstance();
            for (int j = 0; j < dataSourceStr.Length; j++)
            {
                int index = dataSourceStr[j].IndexOf('=') + 1;
                if (index < 2)
                    continue;
                string parmDescription = dataSourceStr[j].Substring(0, index - 1).ToUpper();
                if (CommonlyUsedString.IsEqualToUpper(parmDescription,commonlyUsedString.NAME))
                {
                    datasourcelabel.name = string.Empty;
                    string tempName;
                    if (!GetValue(dataSourceStr, j, index, out tempName))
                        continue;
                    datasourcelabel.name = tempName;
                    continue;
                }
                if (CommonlyUsedString.IsEqualToUpper(parmDescription,commonlyUsedString.SOURCE))
                {
                    uint temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.source = temp;
                    if (datasourcelabel.source >= (int) RepServFileType.MinumOfTemplate)
                    {
                        datasourcelabel.excelType = FileType.ReportingServiceType;
                    }
                    continue;
                }
                if (CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.DATEOFFSETTYPE))
                {
                    int temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.dateOffsetType =
                        (DateOffSetType)temp;
                    continue;
                }
                if (CommonlyUsedString.IsEqualToUpper(parmDescription,commonlyUsedString.OFFSETDATE)||CommonlyUsedString.IsEqualToUpper(parmDescription,commonlyUsedString.DATEOFFSET))
                {
                    string temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.dateOffSet = temp;
                    continue;
                }
                if (CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.DATANUM) || CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.DATESPAN))
                {
                    int temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.dateSpan =
                        temp;
                    continue;
                }
                if (CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.QUERYSPAN))
                {
                    int temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;

                    datasourcelabel.querySpan =
                        (QuerySpan)temp;
                    continue;
                }
                if (CommonlyUsedString.IsEqualToUpper(parmDescription,commonlyUsedString.INTERVAL))
                {
                    int temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    //枚举Interval中没有=0的情况！
                    int interval = temp;
                    if (interval == 0)
                        datasourcelabel.interval = Interval.Hour_1;
                    else
                        datasourcelabel.interval = (Interval) interval;
                    continue;
                }
                //获取设备节点
                if ( CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.COMMONREPORTID)||
                    CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.NODE)
                    )
                {
                    string temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;

                    datasourcelabel.node = temp;
                    continue;
                }
                if (  CommonlyUsedString.IsEqualToUpper(parmDescription,commonlyUsedString.DEVICE_IDS)    )
                {
                    //针对deviceId存放的是json的特殊处理（智慧安全用电评估报告中，由于nodeType可能是自定义根节点，自定义组节点，自定义节点，因此这里需要）
                    if (datasourcelabel.source == (uint)RepServFileType.Safety)
                    {
                        datasourcelabel.entireRSParms.deviceIDs = string.Format("[{0}]", dataSourceStr[j + 1]); 
                        continue;
                    }
                    else
                    {
                        string temp;
                        if (!GetValue(dataSourceStr, j, index, out temp))
                            continue;
                        datasourcelabel.entireRSParms.deviceIDs = temp;
                    }
                }
                if ( CommonlyUsedString.IsEqualToUpper(parmDescription,commonlyUsedString.NODETYPE)    )
                {
                    uint temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.entireRSParms.nodeType = temp;
                    continue;
                }
                if (       CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.INTERVAL_TYPE)      )
                {
                    int temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;

                    datasourcelabel.entireRSParms.intervalType =     temp;
                    continue;
                }
                if (   CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.STATTYPE)      )
                {
                    int temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.entireRSParms.statType =
                        temp;   
                    continue;
                }
                if (CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.KWH_SELECTEDINDEX))
                {
                    int temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.entireRSParms.kWh = temp;
                      
                    continue;
                }
                if (CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.KVARH_SELECTEDINDEX))
                {
                    int temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.entireRSParms.kvarh = temp;

                    continue;
                }
                if (CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.KVA_SELECTEDINDEX))
                {
                    int temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.entireRSParms.kVADemand = temp;

                    continue;
                }
                if (CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.KW_SELECTEDINDEX))
                {
                    int temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.entireRSParms.kWDemand = temp;

                    continue;
                }
                if (CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.KVAR_SELECTEDINDEX))
                {
                    int temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.entireRSParms.kvarDemand = temp;

                    continue;
                }
                if (CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.KVAH_SELECTEDINDEX))
                {
                    int temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.entireRSParms.kVAh = temp;

                    continue;
                }

                //是否是保存的查询条件
                if (  CommonlyUsedString.IsEqualToUpper(parmDescription,commonlyUsedString.IS_QUERY_CONDITION)    )
                {
                     bool temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.isQueryCondition =   temp; 
                    continue;
                }

                if ( CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.IS_INCLUDE_WARNING)        )
                {
                    bool temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.entireRSParms.isCludeWarning =    temp;
                    continue;
                }

                if (  CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.TOU_PROFILE_SCHEDULE_ID)     )
                {
                    uint temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;

                    var tempId = temp;
                    var node = new TOURateParser();
                    NewTOUProfile newTOUProfile;
                    if (node.GetTOURateByID(tempId, out newTOUProfile))
                    {
                        datasourcelabel.entireRSParms.TouProfile = newTOUProfile;
                    }
                    continue;
                }

                if (  CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.PERIOD_TYPE)       )
                {
                    int temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.entireRSParms.periodType =       temp;
                    continue;
                }

                if (  CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.IS_INCLUDE_TOTAL)        )
                {
                    bool temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.entireRSParms.isIncludeTotal =       temp;
                    continue;
                }
                if (    CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.IS_INCLUDE_AVERAGE)        )
                {
                    bool temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.entireRSParms.isIncludeAvg =    temp;
                    continue;
                }

                if (    CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.COMPARE_NUMBER)   )
                {
                    int temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.entireRSParms.compareNumber = temp;
                    continue;
                }

                if (    CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.IS_INCLUDE_DATA_TABLE)       )
                {
                    bool temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;

                    datasourcelabel.entireRSParms.isCludeDataTable = temp;
                    continue;
                }

                if (CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.IS_DEMAND))
                {
                    bool temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;

                    datasourcelabel.entireRSParms.isDemand = temp;
                    continue;
                }

                if (CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.IS_ITIC))
                {
                    bool temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.entireRSParms.isItic = temp;
                    continue;
                }
                if (CommonlyUsedString.IsEqualToUpper(parmDescription, commonlyUsedString.CURVEID))
                {
                    int temp;
                    if (!GetValue(dataSourceStr, j, index, out temp))
                        continue;
                    datasourcelabel.entireRSParms.curveID = temp;
                    continue;
                }
            }

         

        }
        private static bool GetValue(string[] dataSourceStr, int j, int index, out bool result)
        {
            result = false;
            string tempResult;
            if (! GetValue(dataSourceStr, j, index, out tempResult))
                return false;
            return bool.TryParse(tempResult, out result);
        }

        private static bool GetValue(string[] dataSourceStr, int j, int index, out uint result)
        {
            result =uint.MinValue;
            string tempResult;
            if (!GetValue(dataSourceStr, j, index, out tempResult))
                return false;
            return uint.TryParse(tempResult, out result);
        }


        public static bool GetValue(string[] dataSourceStr, int j, int index, out int result)
        {
            result = int.MinValue;
            string tempResult;
            if (!GetValue(dataSourceStr, j, index, out tempResult))
                return false;
            return int.TryParse(tempResult, out result);
        }



        private static bool GetValue(string[] dataSourceStr, int j, int index,out string result)
        {
            return DataSourceLabelForQueryConditionChecker.GetValue(dataSourceStr, j, index, out result);
        }


        /// <summary>
        /// 获取某sheet中的引用标签
        /// </summary>
        /// <param name="sheetname"></param>
        /// <param name="list"></param>
        private void SetDataRetrievalLableValue(string sheetname, ref List<DataRetrievalLabel> list, Excel.Range range)
        {
            DataRetrievalLabel dataretrievallabel = new DataRetrievalLabel();  //引用标签结构体
            dataretrievallabel.rowPosition = range.Row;
            dataretrievallabel.columnPosition = range.Column;
            dataretrievallabel.sheetName = sheetname;
            SetDataRetrievalLabelEachParm(range, ref dataretrievallabel);
            list.Add(dataretrievallabel);
        }

        /// <summary>
        /// 解析引用标签并设置各个参数
        /// </summary>
        /// <param name="range"></param>
        /// <param name="dataretrievallabel"></param>
        private void SetDataRetrievalLabelEachParm(Excel.Range range, ref DataRetrievalLabel dataretrievallabel)
        {
            string entireDRLString = range.Value2.ToString().Substring(8, range.Value2.ToString().Length - 8);
            string[] dataSourceStr = entireDRLString.Split('[', ']');
            for (int j = 0; j < dataSourceStr.Length; j++)
            {
                int index = dataSourceStr[j].IndexOf('=') + 1;
                if (index < 2)
                    continue;
                string parmDescription = dataSourceStr[j].Substring(0, index - 1).ToUpper();
                if (parmDescription == "NAME")
                {
                    dataretrievallabel.name = dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index);
                    continue;
                }
                if (parmDescription == "SCOPE")
                {
                    dataretrievallabel.scope = dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index);
                    continue;
                }
                if (parmDescription == "DIRECTION")
                {
                    dataretrievallabel.direction = dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index);
                    continue;
                }
                if (parmDescription == "SPACE")
                {
                    dataretrievallabel.space = Convert.ToInt32(dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index));
                    continue;
                }
                if (parmDescription == "SUBDATASET")
                {
                    dataretrievallabel.subDataset = dataSourceStr[j].Substring(index, dataSourceStr[j].Length - index);
                    continue;
                }
            }
        }

        /// <summary>
        /// 将输入时间按查询函数要求格式化
        /// </summary>
        /// <param name="searchDateTime"></param>
        /// <param name="timeType"></param>
        /// <returns></returns>
        private DateTime FormatTime(DateTime searchDateTime, FileType timeType)
        {
            string dateString = string.Empty;
            switch (timeType)
            {
                case FileType.DayType:   //日
                    dateString = searchDateTime.Year + "-" + searchDateTime.Month + "-" + searchDateTime.Day + " 1:00:00";
                    break;
                case FileType.WeekType:   //周
                    dateString = FormatTimeForWeekType(searchDateTime);
                    break;
                case FileType.MonthType:   //月
                    dateString = searchDateTime.Year + "-" + searchDateTime.Month + "-01 1:00:00";
                    break;
                case FileType.SeasonType:   //季
                    dateString = FormatTimeForSeasonType(searchDateTime);
                    break;
                case FileType.YearType:   //年
                    dateString = searchDateTime.Year + "-01-01 1:00:00";
                    break;
                case FileType.PeriodType:    //实时值查询
                    dateString = searchDateTime.Year + "-" + searchDateTime.Month + "-" + searchDateTime.Day + " " + searchDateTime.Hour +
                        ":" + searchDateTime.Minute + ":00";
                    break;
            }
            return Convert.ToDateTime(dateString);
        }

        /// <summary>
        /// 处理季类型报表的时间规范
        /// </summary>
        /// <param name="searchDateTime"></param>
        /// <returns></returns>
        private string FormatTimeForSeasonType(DateTime searchDateTime)
        {
            string dateString = string.Empty;
            switch (searchDateTime.Month)
            {
                case 1:
                case 2:
                case 3:
                    dateString = searchDateTime.Year + "-" + "01-01 1:00:00";
                    break;
                case 4:
                case 5:
                case 6:
                    dateString = searchDateTime.Year + "-" + "04-01 1:00:00";
                    break;
                case 7:
                case 8:
                case 9:
                    dateString = searchDateTime.Year + "-" + "07-01 1:00:00";
                    break;
                case 10:
                case 11:
                case 12:
                    dateString = searchDateTime.Year + "-" + "10-01 1:00:00";
                    break;
            }
            return dateString;
        }

        /// <summary>
        /// 处理周类型报表的时间规范
        /// </summary>
        /// <param name="searchDateTime"></param>
        /// <returns></returns>
        private string FormatTimeForWeekType(DateTime searchDateTime)
        {
            //自动发送时，null将处理为0001/1/1 0:0:0，此时再负偏移将出错
            if (searchDateTime == DateTime.MinValue)
                return searchDateTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);

            string dateString = string.Empty;
            int addDays = 0;
            if (IsMondayFirstly())
                addDays = 1;
            switch (searchDateTime.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    if (IsMondayFirstly())
                        searchDateTime = searchDateTime.AddDays(-6);
                    dateString = searchDateTime.Year + "-" + searchDateTime.Month + "-" + searchDateTime.Day + " 1:00:00";
                    break;
                case DayOfWeek.Monday:
                    searchDateTime = searchDateTime.AddDays(-1 + addDays);
                    dateString = searchDateTime.Year + "-" + searchDateTime.Month + "-" + (searchDateTime.Day) + " 1:00:00";
                    break;
                case DayOfWeek.Tuesday:
                    searchDateTime = searchDateTime.AddDays(-2 + addDays);
                    dateString = searchDateTime.Year + "-" + searchDateTime.Month + "-" + (searchDateTime.Day) + " 1:00:00";
                    break;
                case DayOfWeek.Wednesday:
                    searchDateTime = searchDateTime.AddDays(-3 + addDays);
                    dateString = searchDateTime.Year + "-" + searchDateTime.Month + "-" + (searchDateTime.Day) + " 1:00:00";
                    break;
                case DayOfWeek.Thursday:
                    searchDateTime = searchDateTime.AddDays(-4 + addDays);
                    dateString = searchDateTime.Year + "-" + searchDateTime.Month + "-" + (searchDateTime.Day) + " 1:00:00";
                    break;
                case DayOfWeek.Friday:
                    searchDateTime = searchDateTime.AddDays(-5 + addDays);
                    dateString = searchDateTime.Year + "-" + searchDateTime.Month + "-" + (searchDateTime.Day) + " 1:00:00";
                    break;
                case DayOfWeek.Saturday:
                    searchDateTime = searchDateTime.AddDays(-6 + addDays);
                    dateString = searchDateTime.Year + "-" + searchDateTime.Month + "-" + (searchDateTime.Day) + " 1:00:00";
                    break;
            }
            return dateString;
        }

        private bool IsMondayFirstly()
        {
            RegistryKey Myreg;
            Myreg = Registry.CurrentUser;
            Myreg = Myreg.CreateSubKey("Control Panel\\International");
            int firstDay = Convert.ToInt32(Myreg.GetValue("iFirstDayOfWeek", 0));
            Myreg.Close();
            if (firstDay == 6)
                return false;
            return true;
        }

        /// <summary>
        /// 填充静态引用标签对应的数据集
        /// </summary>
        /// <returns></returns>
        private bool FillStaticData()
        {
            DbgTrace.dout("FillStaticData");
            return FillData(StaticRetrievalLabelList, StaticResultsArray, 0);
        }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="retrievalLabelList"></param>
        /// <param name="dataResults"></param>
        /// <param name="mode">为0时进行静态引用数据填充，为1时进行动态引用数据填充</param>
        /// <returns></returns>
        private bool FillData(List<DataRetrievalLabel> retrievalLabelList, DataResults[] dataResults, int mode)
        {
            try
            {
                DbgTrace.dout("retrievalLabelList.Count={0}",retrievalLabelList.Count);
                DbgTrace.dout("dataResults.Length={0}", dataResults.Length);
                for (int i = 0; i < retrievalLabelList.Count; i++)  //取引用标签名依次与数据集名进行匹配
                {
                    for (int j = 0; j < dataResults.Length; j++)
                    {
                        DbgTrace.dout("i={0}  ; j={1}",i,j);
                        DbgTrace.dout("retrievalLabelList[i].name ={0}; dataResults[j].name={1}", retrievalLabelList[i].name , dataResults[j].name);
                        if (retrievalLabelList[i].name != dataResults[j].name)
                        {
                            DbgTrace.dout("retrievalLabelList[i].name != dataResults[j].name,continue");
                            continue;
                        }
                        FillDataProcess(retrievalLabelList[i], dataResults[j], mode);
                        break;   //跳出之后的数据源标签搜索，开始下一个引用标签的比对
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
        /// 填充数据集过程
        /// </summary>
        /// <param name="retrievalLabel"></param>
        /// <param name="dataResults"></param>
        /// <param name="mode"></param>
        private void FillDataProcess(DataRetrievalLabel retrievalLabel, DataResults dataResults, int mode)
        {
            try
            {
                Excel.Range range = null;
                Excel.Worksheet sheet = null;
                GetRetrievalLabelRangeAndSheet(retrievalLabel, mode, ref sheet, ref range);
                if (range == null)
                {
                    DbgTrace.dout("range == null,return.");
                    return;
                }
                if (sheet == null)
                {
                    DbgTrace.dout("sheet == null,return.");
                    return;
                }
                bool isNeedInsert = IsFillDataNeedInsert(range.Value2.ToString());
                if (isNeedInsert)
                    InsertFormulaToCell(sheet, ref range, dataResults, retrievalLabel);
                int firstRangeRow, firstRangeColumn, lastRangeRow, lastRangeColumn;
                AnalyseScopeParm(sheet, retrievalLabel, dataResults, out firstRangeRow, out firstRangeColumn, out lastRangeRow, out lastRangeColumn);
                int startIndex;
                int count;
                ProcSubDatasetParm(retrievalLabel.subDataset, dataResults.dataMusterNum, out startIndex, out count);
                for (int k = startIndex; k < startIndex + count; k++)
                {
                    object[,] objValue = GetDataArray(firstRangeRow, firstRangeColumn, lastRangeRow, lastRangeColumn, dataResults, k);
                    if (objValue == null)
                    {
                        DbgTrace.dout("objValue == null");
                        continue;
                    }
                    Excel.Range rangeNeedFill = GetNeedFillRange(range, sheet, firstRangeRow, firstRangeColumn, lastRangeRow, lastRangeColumn);
                    if (rangeNeedFill == null)
                    {
                        DbgTrace.dout("rangeNeedFill == null");
                        continue;
                    }
                    FillExcelFile(ref rangeNeedFill, objValue);
                    OffsetNextRange(retrievalLabel, ref range, firstRangeRow, firstRangeColumn, lastRangeRow, lastRangeColumn);
                }
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
            }
        }

        public bool SaveReferLaberToExcel(List<List<DataRetrievalLabel>> retrievalLaberList)
        {
            Excel.Range range = null;
            Excel.Worksheet sheet = null;
            foreach (List<DataRetrievalLabel> refLaber in retrievalLaberList)
            {
                if (refLaber.Count < 1)
                    continue;
                for (int i = 1; i <= excelFileOperate.ObjBook.Sheets.Count; i++)
                {
                    Excel.Worksheet tempSheet = excelFileOperate.ObjBook.Sheets[i];
                    if (tempSheet.Name == refLaber[0].sheetName)
                        sheet = (Excel.Worksheet)excelFileOperate.ObjBook.Sheets[i];
                }
                for(int i=0;i<refLaber.Count;i++)
                {
                    range = sheet.Cells[refLaber[i].rowPosition+1, refLaber[i].columnPosition+1];
                    range.Value = refLaber[i].name;
                }
            }
            excelFileOperate.ObjBook.Save();
            return true;
        }

        /// <summary>
        /// 解析处理subDataset参数
        /// </summary>
        /// <param name="subDatasetStr"></param>
        /// <param name="sumNum"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        private static void ProcSubDatasetParm(string subDatasetStr, int sumNum, out int startIndex, out int count)
        {
            //设置默认值
            if (string.IsNullOrEmpty(subDatasetStr))
                subDatasetStr = "1,0";
            //从得到的数据集的指定处开始导出
            string[] subStr = subDatasetStr.Split(',');
            startIndex = Convert.ToInt32(subStr[0]) - 1;
            count = Convert.ToInt32(subStr[1]);
            if (count == 0)
                count = sumNum;
        }

        /// <summary>
        /// 定位引用标签的位置
        /// </summary>
        /// <param name="retrievalLabel"></param>
        /// <param name="mode"></param>
        private void GetRetrievalLabelRangeAndSheet(DataRetrievalLabel retrievalLabel, int mode, ref Excel.Worksheet sheet, ref Excel.Range range)
        {
            if (mode == 0)
                sheet = (Excel.Worksheet)excelFileOperate.ObjBook.Sheets[retrievalLabel.sheetName];
            else if (mode == 1)
                sheet = (Excel.Worksheet)excelFileOperate.ObjBook.ActiveSheet;
            range = sheet.Cells[retrievalLabel.rowPosition, retrievalLabel.columnPosition]; 
        }

        /// <summary>
        /// 解析引用数据源标签相关参数，确定数据集如何排列(针对行列数一致的若干个数据集)
        /// </summary>
        /// <param name="retrievalLabel"></param>
        /// <param name="range"></param>
        /// <param name="firstRangeRow"></param>
        /// <param name="firstRangeColumn"></param>
        /// <param name="lastRangeRow"></param>
        /// <param name="lastRangeColumn"></param>
        private void OffsetNextRange(DataRetrievalLabel retrievalLabel, ref Excel.Range range, int firstRangeRow, int firstRangeColumn, int lastRangeRow, int lastRangeColumn)
        {
            if (retrievalLabel.direction == null || retrievalLabel.direction.ToUpper() == "ROW")
                range = range.Offset[lastRangeRow - firstRangeRow + retrievalLabel.space + 1, 0];
            else
                range = range.Offset[0, lastRangeColumn - firstRangeColumn + retrievalLabel.space + 1]; 
        }

        /// <summary>
        /// 获取excel文件需要填充数据的区域
        /// </summary>
        /// <param name="range"></param>
        /// <param name="sheet"></param>
        /// <param name="firstRangeRow"></param>
        /// <param name="firstRangeColumn"></param>
        /// <param name="lastRangeRow"></param>
        /// <param name="lastRangeColumn"></param>
        /// <returns></returns>
        private Excel.Range GetNeedFillRange(Excel.Range range, Excel.Worksheet sheet, int firstRangeRow, int firstRangeColumn, int lastRangeRow, int lastRangeColumn)
        {
            DbgTrace.dout("获取填充区域，传入的参数是： firstRangeRow = {0} ,  firstRangeColumn = {1} ,  lastRangeRow = {2} , lastRangeColumn = {3} .",firstRangeRow  ,  firstRangeColumn  ,  lastRangeRow , lastRangeColumn);
          
            
            try
            {
                int StartRangeRow = range.Row;
                int StartRangeColumn = range.Column;
                return sheet.Range[sheet.Cells[StartRangeRow, StartRangeColumn], sheet.Cells[StartRangeRow + lastRangeRow - firstRangeRow,
                    StartRangeColumn + lastRangeColumn - firstRangeColumn]];
            }
            catch(Exception ex)
            {
                string format = string.Empty;
                if(lastRangeRow>10000)
                   format= LocalResourceManager.GetInstance().GetString("0588", "即将显示的数据量超过了Excel区域限制。建议缩短查询时间范围。");
                ErrorMessage = format;
                DbgTrace.dout("{0}   {1}   Range的范围：({2},{3})({4},{5})", ex.Message, ErrorMessage, range.Row, range.Column, range.Row + lastRangeRow - firstRangeRow, range.Column + lastRangeColumn - firstRangeColumn);
                return null;
            }
        }

        /// <summary>
        /// 填入excel文件的区域  
        /// </summary>
        /// <param name="range"></param>
        /// <param name="objValue"></param>
        /// <param name="k"></param>
        private void FillExcelFile(ref Excel.Range range, object[,] objValue)
        {
            if (range!=null)
                range.Value = objValue;
        }

        private void InsertFormulaToCell(Excel.Worksheet objSheet, ref Excel.Range range, DataResults dataResults,DataRetrievalLabel retrievalLabel)
        {
            int row = range.Row;
            int column = range.Column;
            string cellValue = range.Value2.ToString();
            bool isColumn = FormulaDirectColumn(range.Value2.ToString());
            range.Value2 = "";
            for (int i = 0; i < dataResults.dataMusterArray[0].Count - 1; i++)
            {
                Excel.XlInsertFormatOrigin insetDirectrion = Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow;
                if (isColumn)
                    range.EntireColumn.Insert(insetDirectrion);
                else
                    range.EntireRow.Insert(insetDirectrion);
            }
            Excel.Range tempRange = objSheet.Cells[row, column] as Excel.Range;

            tempRange.Value2 = cellValue;
            range = tempRange;
        }

        /// <summary>
        /// 行方向还是列方向扩展,默认是行方向
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public bool FormulaDirectColumn(string formula)
        {
            string[] formulaArray = formula.Split(']');
            for (int i = 0; i < formulaArray.Length; i++)
            {
                if (formulaArray[i].Contains("Direction"))
                {
                    if (formulaArray[i].Contains("Column"))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否需要通过插入方式写入
        /// </summary>
        /// <param name="referLabel"></param>
        /// <returns></returns>
        public bool IsFillDataNeedInsert(string referLabel)
        {
            string[] formulaArray = referLabel.Split(']');
            for (int i = 0; i < formulaArray.Length; i++)
            {
                if (formulaArray[i].Contains("Insert"))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 添加选定区域数据存入数组，提高访问速度  
        /// </summary>
        /// <param name="firstRangeRow"></param>
        /// <param name="firstRangeColumn"></param>
        /// <param name="lastRangeRow"></param>
        /// <param name="lastRangeColumn"></param>
        /// <param name="datacheckout"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private object[,] GetDataArray(int firstRangeRow, int firstRangeColumn, int lastRangeRow, int lastRangeColumn, DataResults datacheckout, int k)
        {
            object[,] cellValue = new object[lastRangeRow - firstRangeRow + 1, lastRangeColumn - firstRangeColumn + 1];
            if (datacheckout.notRealWanted)
                return cellValue;
            for (int r = firstRangeRow; r <= lastRangeRow; r++)
            {
                for (int c = firstRangeColumn; c <= lastRangeColumn; c++)
                {
                    if (k >= datacheckout.dataMusterNum)
                    {
                        cellValue[r - firstRangeRow, c - firstRangeColumn] = "";
                        continue;
                    }
                    if (r > datacheckout.dataMusterArray[k].Count || c > datacheckout.dataMusterArray[k][0].Count)
                    {
                        cellValue[r - firstRangeRow, c - firstRangeColumn] = "";
                        continue;
                    }             
                    //如果为无效值-2147483648则不显示
                    if (datacheckout.dataMusterArray[k][r - 1][c - 1] == "-2147483648")
                        datacheckout.dataMusterArray[k][r - 1][c - 1] = "";
                    cellValue[r - firstRangeRow, c - firstRangeColumn] = datacheckout.dataMusterArray[k][r - 1][c - 1];
                }
            }
            return cellValue;
        }

        /// <summary>
        /// 提取数据区域的行列范围
        /// </summary>
        /// <param name="dataretrievallabel"></param>
        /// <param name="datacheckout"></param>
        /// <param name="firstRangeRow"></param>
        /// <param name="firstRangeColumn"></param>
        /// <param name="lastRangeRow"></param>
        /// <param name="lastRangeColumn"></param>
        private void AnalyseScopeParm(Excel.Worksheet sheet, DataRetrievalLabel dataretrievallabel, DataResults datacheckout, out int firstRangeRow, out int firstRangeColumn, out int lastRangeRow, out int lastRangeColumn)
        {
            //仅仅是借助objSheet进行scope的解析
            if (dataretrievallabel.scope == null)//没有设置填充范围，动态填充所有内容
            {
                lastRangeRow = datacheckout.dataMusterArray[0].Count;
                lastRangeColumn = datacheckout.dataMusterArray[0][0].Count;
                firstRangeRow = 1;
                firstRangeColumn = 1;
            }
            else
            {
                Excel.Range range = sheet.Range[dataretrievallabel.scope];
                lastRangeRow = ((Excel.Range)range[range.Count]).Row;
                lastRangeColumn = ((Excel.Range)range[range.Count]).Column;
                if (datacheckout.dataMusterParamDiscribtionArray[0] == DataMusterParamDiscribtion.DynamicReport ||
                    datacheckout.dataMusterParamDiscribtionArray[0] == DataMusterParamDiscribtion.DefaultReport)//动态报表引用区域长度不定
                {
                    lastRangeRow = datacheckout.dataMusterArray[0].Count;
                    if (lastRangeRow>0)
                        lastRangeColumn = datacheckout.dataMusterArray[0][0].Count;
                }

                firstRangeRow = range.Row;
                firstRangeColumn = range.Column;
            }
        }

        /// <summary>
        /// 填充动态引用标签对应的数据集
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private bool FillDynamicData(DateTime startTime, DateTime endTime)
        {
            try
            {
                if (DynamicRetrievalLabelList.Count == 0)  //没有动态引用标签则退出
                    return true;
                DateTime starttime2 = FormatTime(startTime, FileType.DayType);//目前默认日类型查询
                DateTime endtime2 = FormatTime(endTime, FileType.DayType);
                int whichCheckOut = 0;  //标识取哪一天的数据集（查询时段天数中的第几个）
                while (starttime2 <= endtime2)
                {
                    DbgTrace.dout("FillDynamicData and startTime=" + starttime2.ToLongTimeString());
                    AddNewSheet(starttime2);
                    DataResults[] checkOutArray = new DataResults[AllDynamicResults[whichCheckOut].Count];
                    AllDynamicResults[whichCheckOut].CopyTo(checkOutArray);
                    FillData(dynamicRetrievalLabelList, checkOutArray, 1);
                    starttime2 = starttime2.AddDays(1);   //目前默认间隔日类型查询
                    whichCheckOut += 1;
                }
                //删掉作为模板用的sheet页
                ((Excel.Worksheet)excelFileOperate.ObjBook.Sheets[dynamicRetrievalLabelList[0].sheetName]).Delete();
                return true;
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 为excel文件添加名为日期的sheet
        /// </summary>
        /// <param name="starttime"></param>
        private void AddNewSheet(DateTime starttime)
        {
            ProcDateSheet(starttime);
            Excel.Worksheet SourceSheet = (Excel.Worksheet)excelFileOperate.ObjBook.Sheets[DynamicRetrievalLabelList[0].sheetName];
            SourceSheet.Copy(Type.Missing, SourceSheet);
            excelFileOperate.ObjSheet = (Excel.Worksheet)excelFileOperate.ObjBook.ActiveSheet;
            excelFileOperate.ObjSheet.Name = starttime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 是否存在名为TEMP的Sheet
        /// </summary>
        /// <returns></returns>
        private bool CheckTEMPSheetExit()
        {
            for (int i = 1; i <= excelFileOperate.ObjBook.Worksheets.Count; i++)
            {
                Excel.Worksheet sheet = (Excel.Worksheet)(excelFileOperate.ObjBook.Worksheets.get_Item(i));
                if (sheet.Name.ToUpper() == "TEMP")
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 查询已经存在yyyy-MM-dd形式的Sheet
        /// </summary>
        /// <param name="starttime"></param>
        private void ProcDateSheet(DateTime starttime)
        {
            for (int i = 1; i <= excelFileOperate.ObjBook.Worksheets.Count; i++)
            {
                Excel.Worksheet sheet = (Excel.Worksheet)(excelFileOperate.ObjBook.Worksheets.get_Item(i));
                if (sheet.Name != starttime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern))
                    continue;
                sheet.Delete();
            }
        }

        /// <summary>
        /// 在excel文件中设置查询时间（没有起止两个时间则为查询时间）
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="dateType"></param>
        private void SetTime(DateTime StartTime, string dateType,string userName)
        {
            Excel.Range range = (Excel.Range)excelFileOperate.ObjSheet.UsedRange.Find("@" + dateType + "Date", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSearchDirection.xlNext, Type.Missing, Type.Missing, Type.Missing);
            if (range != null)
            {
                range.Value = StartTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " HH:mm:ss");
            }
            range = (Excel.Range)excelFileOperate.ObjSheet.UsedRange.Find("@" + dateType + "Year", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSearchDirection.xlNext, Type.Missing, Type.Missing, Type.Missing);
            if (range != null)
            {
                range.Value = StartTime.Year.ToString();
            }
            range = (Excel.Range)excelFileOperate.ObjSheet.UsedRange.Find("@" + dateType + "Month", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSearchDirection.xlNext, Type.Missing, Type.Missing, Type.Missing);
            if (range != null)
            {
                range.Value = StartTime.Month.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " HH:mm:ss");
            }
            range = (Excel.Range)excelFileOperate.ObjSheet.UsedRange.Find("@" + dateType + "Day", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSearchDirection.xlNext, Type.Missing, Type.Missing, Type.Missing);
            if (range != null)
            {
                range.Value = StartTime.Day.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " HH:mm:ss");
            }
            range = (Excel.Range)excelFileOperate.ObjSheet.UsedRange.Find("@Date", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSearchDirection.xlNext, Type.Missing, Type.Missing, Type.Missing);
            if (range != null)
            {
                string currentUserName = userName;
                if (string.IsNullOrEmpty(currentUserName))
                {
                    range.Value = string.Format("{0} {1}  \r",
    LocalResourceManager.GetInstance().GetString("0576", "Generated at:"),
    DateTime.Now.ToString(DataManager.TimeFormatWithSecond));
                }
                else
                {
                    string formatString = LocalResourceManager.GetInstance().GetString("0611", "Generared by : ({0}) at {1}");
                    formatString = string.Format("{0}  \r",formatString);
                    range.Value = string.Format(formatString,   currentUserName,DateTime.Now.ToString(DataManager.TimeFormatWithSecond));
                }
            }
        }
        #endregion
    }
   
}