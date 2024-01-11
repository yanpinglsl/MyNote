using System;
using System.Collections.Generic;
using System.Linq;
using CET.PecsNodeManage;
using System.Data;
using CSharpDBPlugin;


namespace OfficeReportInterface.DefaultReportInterface
{
    public   class PointsNodeManager
    {
        #region 对外接口函数
        /// <summary>
        /// 根据传入的报表类型，设备+回路号，得到相应的测点（在WebService接口中使用）
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="deviceIDListString">设备Id1,回路号1;设备Id2,回路号2;设备Id3,回路号3;</param>
        /// <returns></returns>
        public static List<PointsNode> GetPointsNodeList(uint reportType, string deviceIDListString)
        {
            List<LogicalDeviceIndex> deviceIDList;
            GetLogicalDeviceIndexListByString(deviceIDListString, out deviceIDList);
            return GetPointsNodeList(reportType, deviceIDList);
        }


        /// <summary>
        /// 根据传入的报表类型，设备+回路号，得到相应的测点
        /// </summary>
        /// <param name="reportType"></param>
        /// <param name="deviceIDList"></param>
        /// <returns></returns>
        public static List<PointsNode> GetPointsNodeList(uint reportType, List<LogicalDeviceIndex> deviceIDList)
        {
            List<DataIDNameTypeDef> allDataIDList = null;
            GetallDataIDList(out allDataIDList, deviceIDList);
            List<PointsNode> pointsNodeList = new List<PointsNode>();
            List<DataTypeNameDef> dataTypeNameList = null;
            LoadDataTypeNameList(out dataTypeNameList);
            if (reportType == (uint)RepServFileType.SingleUsage || reportType == (uint)RepServFileType.MultiUsage || reportType == (uint)RepServFileType.EnergyPeriod || reportType == (uint)RepServFileType.HourlyUsage)
            {
                //电度相关
                AddDegreetPara(ref pointsNodeList, dataTypeNameList, allDataIDList);
                ////其他参数
                AddOtherPara(ref pointsNodeList, dataTypeNameList, allDataIDList);
            }
            else
            {
                //加载电压相关参数
                AddVoltPara(ref pointsNodeList, dataTypeNameList, allDataIDList);
                //加载电流相关参数
                AddCurrentPara(ref pointsNodeList, dataTypeNameList, allDataIDList);
                //加载功率相关参数
                AddPowerPara(ref pointsNodeList, dataTypeNameList, allDataIDList);
                //加载暂态相关参数
                AddTransientPara(ref pointsNodeList, dataTypeNameList, allDataIDList);
                //电度相关
                AddDegreetPara(ref pointsNodeList, dataTypeNameList, allDataIDList);
                //停电相关
                AddStopPara(ref pointsNodeList, dataTypeNameList, allDataIDList);
                //非电量测点
                AddNonElectricityPara(ref pointsNodeList,  dataTypeNameList, allDataIDList);
                //需量相关
                AddDemandPara(ref pointsNodeList, dataTypeNameList, allDataIDList);
                //实时最值
                AddRealPara(ref pointsNodeList, dataTypeNameList, allDataIDList);
                //其他参数
                AddOtherPara(ref pointsNodeList, dataTypeNameList, allDataIDList);
            }
            SetLeaf(pointsNodeList);
            return pointsNodeList;
        }
        #endregion

        #region 被调用的私有函数
       
        private static bool GetLogicalDeviceIndexListByString(string parameters, out List<LogicalDeviceIndex> deviceIDList)
        {
            deviceIDList = new List<LogicalDeviceIndex>();
            try
            {
                List<string> resultStrs = DataFormatManager.ParseStringList(parameters, ";");
                foreach (var item in resultStrs)
                {
                    List<uint> paraIDs = DataFormatManager.ParseUIntList(item, ",");
                    if (paraIDs.Count != 2)
                        continue;
                    LogicalDeviceIndex logicalDeviceIndex = new LogicalDeviceIndex();
                    logicalDeviceIndex.DeviceID = paraIDs[0];
                    logicalDeviceIndex.LogicalIndex = (int)paraIDs[1];
                    deviceIDList.Add(logicalDeviceIndex);
                }
            }
            catch (System.Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }
      
        private static void SetLeaf( List<PointsNode> pointsNodeList)
        {
            foreach (var item in pointsNodeList)
            {
                if (item.Nodes.Count == 0)
                {
                    item.leaf = true;
                }
                else
                {
                    item.leaf = false;
                }
                   
                var tempList = item.Nodes;
                SetLeaf( tempList);
            }
        }

     
        #endregion

        #region 实时最值
        /// <summary>
        ///实时最值
        /// </summary>
        private static void AddRealPara(ref List<PointsNode> pointsNodeList, List<DataTypeNameDef> dataTypeNameList, List<DataIDNameTypeDef> allDataIdList)
        {
            PointsNode commonVoltParaTypePointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0327", "实时最值"));
            AddMeasureNodeToTreeView(commonVoltParaTypePointsNode, GetDataIdNameList(800, 899,allDataIdList),dataTypeNameList);
            AdddChildIfNotLeaf(pointsNodeList, commonVoltParaTypePointsNode);
        }  
        #endregion

        #region 需量相关
        /// <summary>
        /// 需量相关
        /// </summary>
        private static void AddDemandPara(ref List<PointsNode> pointsNodeList, List<DataTypeNameDef> dataTypeNameList, List<DataIDNameTypeDef> allDataIdList)
        {
            PointsNode commonVoltParaTypePointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0326", "需量相关"));
            AddMeasureNodeToTreeView(commonVoltParaTypePointsNode, GetDataIdNameList(700, 799,allDataIdList),dataTypeNameList);
            AdddChildIfNotLeaf(pointsNodeList, commonVoltParaTypePointsNode);
        }
        #endregion

        #region 非电量测点相关
        /// <summary>
        /// 添加非电量测点
        /// </summary>
        private static void AddNonElectricityPara(ref List<PointsNode> pointsNodeList, List<DataTypeNameDef> dataTypeNameList, List<DataIDNameTypeDef> allDataIdList)
        {
            PointsNode commonVoltParaTypePointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0646", "Non-electricity Points"));
            AddMeasureNodeToTreeView(commonVoltParaTypePointsNode, GetDataIdNameList(600, 699, allDataIdList), dataTypeNameList);
            AdddChildIfNotLeaf(pointsNodeList, commonVoltParaTypePointsNode);
        }

        #endregion

        #region 停电相关
        /// <summary>
        /// 停电相关
        /// </summary>
        private static void AddStopPara(ref List<PointsNode> pointsNodeList, List<DataTypeNameDef> dataTypeNameList, List<DataIDNameTypeDef> allDataIdList)
        {
            PointsNode commonVoltParaTypePointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0324", "停电相关"));
            AddMeasureNodeToTreeView(commonVoltParaTypePointsNode, GetDataIdNameList(500, 599,allDataIdList),dataTypeNameList);
            AdddChildIfNotLeaf(pointsNodeList, commonVoltParaTypePointsNode);
        }
        #endregion

        #region 电度相关

        /// <summary>
        /// 加载电度相关
        /// </summary>
        private static void AddDegreetPara(ref List<PointsNode> pointsNodeList, List<DataTypeNameDef> dataTypeNameList, List<DataIDNameTypeDef> allDataIdList)
        {
            PointsNode commonVoltParaTypePointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0323", "电度相关"));
            AddMeasureNodeToTreeView(commonVoltParaTypePointsNode, GetDataIdNameList(400, 499,allDataIdList),dataTypeNameList);
            AdddChildIfNotLeaf(pointsNodeList, commonVoltParaTypePointsNode);
        }
        #endregion

        #region 暂态相关参数

        /// <summary>
        /// 暂态相关参数
        /// </summary>
        private static void AddTransientPara(ref List<PointsNode> pointsNodeList, List<DataTypeNameDef> dataTypeNameList, List<DataIDNameTypeDef> allDataIdList)
        {
            PointsNode commonVoltParaTypePointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0322", "暂态相关"));
            AddMeasureNodeToTreeView(commonVoltParaTypePointsNode, GetDataIdNameList(300, 301,allDataIdList),dataTypeNameList);
            AdddChildIfNotLeaf(pointsNodeList, commonVoltParaTypePointsNode);
        }

        private static void AdddChildIfNotLeaf(List<PointsNode> pointsNodeList, PointsNode commonVoltParaTypePointsNode)
        {
            if (commonVoltParaTypePointsNode.Nodes.Count > 0)
            {
                pointsNodeList.Add(commonVoltParaTypePointsNode);
            }
        }

        #endregion

        #region 加载功率相关参数
        /// <summary>
        /// 加载功率相关参数，包含A,B,C以及总功率
        /// </summary>
        /// <param name="parentNode">父节点</param>
        /// <param name="startDataIDIndex">参数开始序号</param>
        /// <param name="phaseName">相名称</param>
        private static void AddPowerParaByFourPhase(PointsNode parentNode, int startDataIDIndex, string phaseName,List<DataTypeNameDef> dataTypeNameList, List<DataIDNameTypeDef> allDataIdList)
        {
            //A相
            PointsNode phaseParaPointsNode = new PointsNode(phaseName + "A");
            AddMeasureNodeToTreeView(phaseParaPointsNode, GetDataIdNameList(startDataIDIndex, startDataIDIndex + 1,allDataIdList),dataTypeNameList);
            AdddChildIfNotLeaf(parentNode, phaseParaPointsNode);
            //B相
            phaseParaPointsNode = new PointsNode(phaseName + "B");
            AddMeasureNodeToTreeView(phaseParaPointsNode, GetDataIdNameList(startDataIDIndex + 1, startDataIDIndex + 2,allDataIdList),dataTypeNameList);
            AdddChildIfNotLeaf(parentNode, phaseParaPointsNode);
            //C相
            phaseParaPointsNode = new PointsNode(phaseName + "C");
            AddMeasureNodeToTreeView(phaseParaPointsNode, GetDataIdNameList(startDataIDIndex + 2, startDataIDIndex + 3,allDataIdList),dataTypeNameList);
            AdddChildIfNotLeaf(parentNode, phaseParaPointsNode);
            //总
            phaseParaPointsNode = new PointsNode(phaseName + LocalResourceManager.GetInstance().GetString("0129", "存在引用了无效报表源的数据源标签.保存失败."));
            AddMeasureNodeToTreeView(phaseParaPointsNode, GetDataIdNameList(startDataIDIndex + 3, startDataIDIndex + 4,allDataIdList),dataTypeNameList);
            AdddChildIfNotLeaf(parentNode, phaseParaPointsNode);
        }
        private static void AddPowerPara(ref List<PointsNode> pointsNodeList, List<DataTypeNameDef> dataTypeNameList, List<DataIDNameTypeDef> allDataIdList)
        {
            PointsNode commonVoltParaTypePointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0321", "功率和功率因数相关"));

            //普通电压参数：dataid范围100-101*10000
            PointsNode voltParaPointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0411", "Common PF Points"));
         
            AddMeasureNodeToTreeView(voltParaPointsNode, GetDataIdNameList(200, 201,allDataIdList),dataTypeNameList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);

            voltParaPointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0414", "Harmonic kW"));
           
            AddPowerParaByFourPhase(voltParaPointsNode, 201, "",dataTypeNameList,allDataIdList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);
            voltParaPointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0415", "Harmonic kvar"));

            AddPowerParaByFourPhase(voltParaPointsNode, 205, "", dataTypeNameList, allDataIdList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);
            voltParaPointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0417", "Harmonic kVA"));

            AddPowerParaByFourPhase(voltParaPointsNode, 209, "", dataTypeNameList, allDataIdList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);

            AdddChildIfNotLeaf(pointsNodeList, commonVoltParaTypePointsNode);
        }
        #endregion


        #region 电流相关
        private static void AddCurrentPara(ref List<PointsNode> pointsNodeList, List<DataTypeNameDef> dataTypeNameList, List<DataIDNameTypeDef> allDataIdList)
        {
            PointsNode commonVoltParaTypePointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0320", "电流相关"));

            //普通电压参数：dataid范围100-101*10000
            PointsNode voltParaPointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0383", "Common Current Points"));
            AddMeasureNodeToTreeView(voltParaPointsNode, GetDataIdNameList(100, 101, allDataIdList), dataTypeNameList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);
            //谐波电压含有率:dataid范围101*10000-104*10000
            voltParaPointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0384", "HR of Fundamental of Current"));
            AddParaByThreePhase(voltParaPointsNode, 101, "I", dataTypeNameList, allDataIdList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);
            //谐波电压有效值:dataid范围104*10000-107*10000
            voltParaPointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0389", "RMS of Harmonic Current"));
            AddParaByThreePhase(voltParaPointsNode, 104, "I", dataTypeNameList, allDataIdList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);
            //谐波电压相角:dataid范围107*10000-110*10000
            voltParaPointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0391", "Phase Angle of Harmonic Current"));
            AddParaByThreePhase(voltParaPointsNode, 107, "I", dataTypeNameList, allDataIdList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);
            //间谐波电压含有率:dataid范围110*10000-113*10000
            voltParaPointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0392", "HR of Interharmonic Current"));
            AddParaByThreePhase(voltParaPointsNode, 110, "I", dataTypeNameList, allDataIdList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);
            //间谐波电压有效值:dataid范围113*10000-116*10000
            voltParaPointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0394", "RMS of Interharmonic Current"));
            AddParaByThreePhase(voltParaPointsNode, 113, "I", dataTypeNameList, allDataIdList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);
            //间谐波电压相角:dataid范围116*10000-119*10000
            voltParaPointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0446", "Phase Angle of Interharmonic Current"));
            AddParaByThreePhase(voltParaPointsNode, 116, "I", dataTypeNameList, allDataIdList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);
            //间谐波电压相角:dataid范围119*10000-122*10000
            voltParaPointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0396", "I Freq IH"));
            AddParaByThreePhase(voltParaPointsNode, 119, "I", dataTypeNameList, allDataIdList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);

            AdddChildIfNotLeaf(pointsNodeList, commonVoltParaTypePointsNode);
        }

        #endregion
       

        #region 电压相关
        /// <summary>
        /// 加载分相参数，六相,从起始dataid，每项+10000
        /// </summary>
        private static void AddParaBySixPhase(PointsNode parentNode, int startDataIDIndex, List<DataTypeNameDef> dataTypeNameList,
            List<DataIDNameTypeDef> allDataIDList)
        {
            //A相
            PointsNode phaseParaPointsNode = new PointsNode("UA");
            AddMeasureNodeToTreeView(phaseParaPointsNode, GetDataIdNameList(startDataIDIndex, startDataIDIndex + 1,allDataIDList),dataTypeNameList);
            AdddChildIfNotLeaf(parentNode, phaseParaPointsNode);
            //B相
            phaseParaPointsNode = new PointsNode("UB");
            AddMeasureNodeToTreeView(phaseParaPointsNode, GetDataIdNameList(startDataIDIndex + 1, startDataIDIndex + 2, allDataIDList), dataTypeNameList);
            AdddChildIfNotLeaf(parentNode, phaseParaPointsNode);
            //C相
            phaseParaPointsNode = new PointsNode("UC");
            AddMeasureNodeToTreeView(phaseParaPointsNode, GetDataIdNameList(startDataIDIndex + 2, startDataIDIndex + 3, allDataIDList), dataTypeNameList);
            AdddChildIfNotLeaf(parentNode, phaseParaPointsNode);
            //AB相
            phaseParaPointsNode = new PointsNode("UAB");
            AddMeasureNodeToTreeView(phaseParaPointsNode, GetDataIdNameList(startDataIDIndex + 3, startDataIDIndex + 4, allDataIDList), dataTypeNameList);
            AdddChildIfNotLeaf(parentNode, phaseParaPointsNode);
            //BC相
            phaseParaPointsNode = new PointsNode("UBC");
            AddMeasureNodeToTreeView(phaseParaPointsNode, GetDataIdNameList(startDataIDIndex + 4, startDataIDIndex + 5, allDataIDList), dataTypeNameList);
            AdddChildIfNotLeaf(parentNode, phaseParaPointsNode);
            //CA相
            phaseParaPointsNode = new PointsNode("UCA");
            AddMeasureNodeToTreeView(phaseParaPointsNode, GetDataIdNameList(startDataIDIndex + 5, startDataIDIndex + 6, allDataIDList), dataTypeNameList);
            AdddChildIfNotLeaf(parentNode, phaseParaPointsNode);

        }

        private static void AdddChildIfNotLeaf(PointsNode parentNode, PointsNode phaseParaPointsNode)
        {
            if (phaseParaPointsNode.Nodes.Count > 0)
            {
                parentNode.Nodes.Add(phaseParaPointsNode);
            }
        }

        /// <summary>
        /// 加载谐波电压相关参数，参数dataid是以10000为间隔进行分组
        /// </summary>
        private static void AddVoltPara(ref List<PointsNode> pointsNodeList, List<DataTypeNameDef> dataTypeNameList,
            List<DataIDNameTypeDef> allDataIDList)
        {
            PointsNode commonVoltParaTypePointsNode =
                new PointsNode(LocalResourceManager.GetInstance().GetString("0319", "电压相关"));

            //普通电压参数：dataid范围0-1*10000
            var temp = GetDataIdNameList(0, 1, allDataIDList);
            PointsNode voltParaPointsNode =
                new PointsNode(LocalResourceManager.GetInstance().GetString("0398", "Common Voltage Points"));
            AddMeasureNodeToTreeView(voltParaPointsNode, temp, dataTypeNameList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);
          
            //谐波电压含有率:dataid范围1*10000-7*10000
            voltParaPointsNode =
                new PointsNode(LocalResourceManager.GetInstance().GetString("0399", "HR of Fundamental Voltage"));
            AddParaBySixPhase(voltParaPointsNode, 1,dataTypeNameList,allDataIDList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);
            //谐波电压有效值:dataid范围7*10000-13*10000
            voltParaPointsNode =
                new PointsNode(LocalResourceManager.GetInstance().GetString("0400", "RMS of Harmonic Voltage"));
            AddParaBySixPhase(voltParaPointsNode, 7, dataTypeNameList, allDataIDList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);
            //谐波电压相角:dataid范围13*10000-19*10000
            voltParaPointsNode =
                new PointsNode(LocalResourceManager.GetInstance().GetString("0401", "Phase Angle of Harmonic Voltage"));
            AddParaBySixPhase(voltParaPointsNode, 13, dataTypeNameList, allDataIDList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);
            //间谐波电压含有率:dataid范围19*10000-25*10000
            voltParaPointsNode =
                new PointsNode(LocalResourceManager.GetInstance().GetString("0403", "HR of Interharmonic Voltage"));
            AddParaBySixPhase(voltParaPointsNode, 19, dataTypeNameList, allDataIDList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);
            //间谐波电压有效值:dataid范围25*10000-31*10000
            voltParaPointsNode =
                new PointsNode(LocalResourceManager.GetInstance().GetString("0404", "RMS of Interharmonic Voltage"));
            AddParaBySixPhase(voltParaPointsNode, 25, dataTypeNameList, allDataIDList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);
            //间谐波电压相角:dataid范围31*10000-37*10000
            voltParaPointsNode =
                new PointsNode(LocalResourceManager.GetInstance()
                    .GetString("0405", "Phase Angle of Interharmonic Voltage"));
            AddParaBySixPhase(voltParaPointsNode, 31, dataTypeNameList, allDataIDList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);
            //间谐波电压相角:dataid范围31*10000-37*10000
            voltParaPointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0407", "V Freq IH"));
            AddParaByThreePhase(voltParaPointsNode, 37, "U",dataTypeNameList,allDataIDList);
            AdddChildIfNotLeaf(commonVoltParaTypePointsNode, voltParaPointsNode);

            AdddChildIfNotLeaf(pointsNodeList, commonVoltParaTypePointsNode);
        }
        /// <summary>
        /// 加载分相参数，3相,从起始dataid，每项+10000
        /// </summary>
        /// <param name="parentNode">父节点</param>
        /// <param name="startDataIDIndex">参数开始序号</param>
        /// <param name="phaseName">相名称</param>
        private static void AddParaByThreePhase(PointsNode parentNode, int startDataIDIndex, string phaseName, List<DataTypeNameDef> dataTypeNameList,
            List<DataIDNameTypeDef> allDataIDList)
        {
            //A相
            PointsNode phaseParaPointsNode = new PointsNode(phaseName + "A");
            AddMeasureNodeToTreeView(phaseParaPointsNode, GetDataIdNameList(startDataIDIndex, startDataIDIndex + 1,allDataIDList),dataTypeNameList);
            AdddChildIfNotLeaf(parentNode, phaseParaPointsNode);
            //B相
            phaseParaPointsNode = new PointsNode(phaseName + "B");
            AddMeasureNodeToTreeView(phaseParaPointsNode, GetDataIdNameList(startDataIDIndex + 1, startDataIDIndex + 2, allDataIDList), dataTypeNameList);
            AdddChildIfNotLeaf(parentNode, phaseParaPointsNode);
            //C相
            phaseParaPointsNode = new PointsNode(phaseName + "C");
            AddMeasureNodeToTreeView(phaseParaPointsNode, GetDataIdNameList(startDataIDIndex + 2, startDataIDIndex + 3, allDataIDList), dataTypeNameList);
            AdddChildIfNotLeaf(parentNode, phaseParaPointsNode);
        }
        #endregion

        #region 其他参数
        /// <summary>
        /// 其他参数
        /// </summary>
        private static void AddOtherPara(ref List<PointsNode> pointsNodeList, List<DataTypeNameDef> dataTypeNameList, List<DataIDNameTypeDef> allDataIDList)
        {
            List<DataIDNameTypeDef> dataIdNameList = GetDataIdNameList(1000, 1001, allDataIDList);
            if (dataIdNameList.Count == 0)
                return;
            PointsNode commonVoltParaTypePointsNode = new PointsNode(LocalResourceManager.GetInstance().GetString("0325", "其他"));
            AddMeasureNodeToTreeView(commonVoltParaTypePointsNode, dataIdNameList, dataTypeNameList);
            AdddChildIfNotLeaf(pointsNodeList, commonVoltParaTypePointsNode);
        }
        #endregion

        #region 通用函数
        private static void GetallDataIDList(out List<DataIDNameTypeDef> allDataIDList, List<LogicalDeviceIndex> deviceIDList)
        {
            allDataIDList = new List<DataIDNameTypeDef>();
            allDataIDList.Clear();
            for (int i = 0; i < deviceIDList.Count; i++)
            {
                List<DataIDNameTypeDef> tempDataDef = new List<DataIDNameTypeDef>();
                PecsDeviceNode deviceNode = PecsNodeManager.PecsNodeInstance.GetDeviceNodeByID(deviceIDList[i].DeviceID);
                if (deviceNode == null)
                    continue;
                DataTable DT = new DataTable();
                DataLogPrivateMapProvider.Instance.Read(0,deviceNode.ParentNode.ParentNode.ParentNode.NodeID, deviceNode.ParentNode.NodeID, deviceNode.NodeID, Convert.ToUInt32(deviceIDList[i].LogicalIndex), 1, ref DT);
                for (int j = 0; j < DT.Rows.Count; j++)
                {
                    uint DataID = Convert.ToUInt32(DT.Rows[j]["DataID"]);
                    string paraName = DT.Rows[j]["paraName"].ToString();
                    int dataTypeID = Convert.ToInt32(DT.Rows[j]["dataTypeID"].ToString());
                    DataIDNameTypeDef dataIdDef = new DataIDNameTypeDef(DataID, paraName, dataTypeID);//PecsNodeManager.PecsNodeInstance.FindDataID(datahandleDef.DataID);
                    if (dataIdDef.DataID >= 0)
                        tempDataDef.Add(dataIdDef);
                }
                if (i == 0)
                    allDataIDList = tempDataDef;
                allDataIDList = allDataIDList.ToArray().Intersect(tempDataDef).ToList();
            }

        }
        /// <summary>
        /// 返回一定dataid范围内的参数表
        /// </summary>
        /// <param name="mindataid">参数区间</param>
        /// <param name="maxdataid">参数区间</param>
        /// <returns></returns>
        private static List<DataIDNameTypeDef> GetDataIdNameList(int mindataid, int maxdataid, List<DataIDNameTypeDef> allDataIDList)
        {
            List<DataIDNameTypeDef> dataIdDefList = new List<DataIDNameTypeDef>();
            for (int i = 0; i < allDataIDList.Count; i++)
            {
                DataIDNameTypeDef dataIDDef = allDataIDList[i];
                if ((Convert.ToInt32(dataIDDef.DataID) > mindataid * 10000) && (Convert.ToInt32(dataIDDef.DataID) < maxdataid * 10000))
                {
                    dataIdDefList.Add(dataIDDef);
                }
            }
            return dataIdDefList;
        }
        private static string GetDataTypeNameByTypeID(int dataTypeID, List<DataTypeNameDef> dataTypeNameList)
        {
            for (int i = 0; i < dataTypeNameList.Count; i++)
            {
                if (dataTypeNameList[i].DataTypeID == dataTypeID)
                    return dataTypeNameList[i].DataTypeName;
            }
            return "";
        }
        private static void LoadDataTypeNameList(out  List<DataTypeNameDef> dataTypeNameList)
        {
            dataTypeNameList = AllDataTypeNameManager.GetInstance().DataTypeNameList;
        }
        /// <summary>
        /// 添加测点
        /// </summary>
        /// <param name="dataIdDefLis"></param>
        private static void AddMeasureNodeToTreeView(PointsNode rootNode, List<DataIDNameTypeDef> dataIdDefLis, List<DataTypeNameDef> dataTypeNameList)
        {
            //添加测点
            for (int i = 0; i < dataIdDefLis.Count; i++)
            {
                DataIDNameTypeDef dataIdDef = dataIdDefLis[i];
                string dataTypeName = GetDataTypeNameByTypeID(dataIdDef.DataTypeID, dataTypeNameList);
                PointsNode dataNameIDPointsNode = new PointsNode(dataIdDef.DataName + "-" + dataTypeName + "(" + dataIdDef.DataID + ")");

                dataNameIDPointsNode.Tag = dataIdDef;
                dataNameIDPointsNode.ImageIndex = 1;
                dataNameIDPointsNode.SelectedImageIndex = 1;
                rootNode.Nodes.Add(dataNameIDPointsNode);
            }
        }
        #endregion

    }
}
