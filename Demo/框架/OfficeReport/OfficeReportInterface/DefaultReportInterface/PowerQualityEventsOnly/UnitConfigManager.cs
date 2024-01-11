using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace OfficeReportInterface
{
    /// <summary>
    /// 单位配置管理类
    /// </summary>
    class UnitConfigManager
    {
        public UnitConfigManager()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }        

        /// <summary>
        /// 唯一实例
        /// </summary>
        public readonly static UnitConfigManager DataManager = new UnitConfigManager();

        /// <summary>
        /// 保存单位组及其相关信息映射关系
        /// </summary>
        private Dictionary<int, UnitConfigDef> groupidUnitMap;

        /// <summary>
        /// 保存其他参数分类，参数ID与其单位信息映射关系
        /// </summary>
        private Dictionary<uint, DataUnitDef> otherDataidUnitMap;        

        /// <summary>
        /// 单位配置文件名称
        /// </summary>
        private static string UnitConfigFile = "UnitConfig.ini";        

        /// <summary>
        /// 初始化单位信息
        /// </summary>
        /// <returns></returns>
        public EMSErrorMsg Initialize()
        {
            EMSErrorMsg resultMsg = new EMSErrorMsg(true);

            this.groupidUnitMap = new Dictionary<int, UnitConfigDef>();
            this.otherDataidUnitMap = new Dictionary<uint, DataUnitDef>();
            try
            {
                SysConstDef.ProfileDirPath = Path.Combine(DbgTrace.IsOfficeReport()?DbgTrace.GetAssemblyPath():DbgTrace.GetCurrentParentPath(), "Profile");//桌面版要放到执行目录下的Profile；web版要放到bin目录外面的Profile
                ConfigFileManager.DataManager.IniPath = Path.Combine(SysConstDef.ProfileDirPath, UnitConfigFile);
                DbgTrace.dout("Profile路径=" + SysConstDef.ProfileDirPath +"         "+ UnitConfigFile + "    路径=" + ConfigFileManager.DataManager.IniPath);//打印输出，用于排查问题
                if (!Directory.Exists(SysConstDef.ProfileDirPath))
                    Directory.CreateDirectory(SysConstDef.ProfileDirPath);
                if (!ConfigFileManager.DataManager.ExistINIFile())
                    File.Create(ConfigFileManager.DataManager.IniPath).Close();

                int groupCount = ConfigFileManager.DataManager.ReadInt32("FORMAT", "UnitGroupCount");
                for (int i = 0; i < groupCount; i++)
                {
                    int groupid = i + 1;
                    UnitConfigDef unitdef = new UnitConfigDef(groupid);
                    string sectionFlag = string.Format("UNITGROUP{0}", i + 1);
                    unitdef.GroupID = groupid;
                    unitdef.GroupName = ConfigFileManager.DataManager.ReadString(sectionFlag, "GroupName");
                    unitdef.UnitName = ConfigFileManager.DataManager.ReadString(sectionFlag, "Uint");
                    unitdef.Coefficient = ConfigFileManager.DataManager.ReadDouble(sectionFlag, "Coefficient");
                    GeDataIDList(sectionFlag, out unitdef.SigleDataids, out unitdef.RangeDataids);
                    this.groupidUnitMap[groupid] = unitdef;
                }

                string othersFlag = "SPECIAL";
                int otherDataCount = ConfigFileManager.DataManager.ReadInt32(othersFlag, "TotalCount");
                for (int i = 0; i < otherDataCount; i++)
                {
                    string dataStr = ConfigFileManager.DataManager.ReadString(othersFlag, string.Format("DataID{0}", i + 1));
                    List<string> dataItems = DataFormatManager.ParseStringList(dataStr, ",");
                    if (dataItems.Count == 3)
                    {
                        uint dataid;
                        double coef = 1;
                        bool success = uint.TryParse(dataItems[0], out dataid);
                        if (success)
                            success = double.TryParse(dataItems[2], out coef);
                        if (!success)
                            continue;

                        DataUnitDef newItem = new DataUnitDef(dataid);
                        newItem.Index = i + 1;
                        newItem.UnitName = dataItems[1];
                        newItem.Coefficient = coef;
                        this.otherDataidUnitMap[dataid] = newItem;
                    }
                }
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message + ex.StackTrace);
                resultMsg.Success = false;
                resultMsg.ErrorMessage = ex.Message + ex.StackTrace;
            }
            return resultMsg;
        }

        /// <summary>
        /// 获取参数名称
        /// </summary>
        /// <param name="dataIDs">查询的参数列表</param>
        /// <returns></returns>
        public List<string> GetDataIDName(List<uint> dataIDs)
        {
            List<string> resultList = new List<string>();

            Dictionary<uint, string> dataIDMap = DataParameterInfoDef.DataManager.GetDataIDNameMap(dataIDs);
            foreach (uint dataid in dataIDs)
            {
                string dataName;
                bool exist = dataIDMap.TryGetValue(dataid, out dataName);
                if (!exist)
                    dataName = string.Empty;
                resultList.Add(dataName);
            }

            return resultList;
        }

        /// <summary>
        /// 在单位组中查找单位信息
        /// </summary>
        /// <returns></returns>
        public UnitConfigView FindDataUnitInGroup(uint dataID)
        {            
            UnitConfigDef targetUint = new UnitConfigDef(0);     
            List<DataIDRange> rangeDataids = new  List<DataIDRange>();
            List<uint> dataids = new List<uint>();
            foreach (UnitConfigDef unitCofig in this.groupidUnitMap.Values)
            {
                if (unitCofig.SigleDataids.Contains(dataID))
                {
                    targetUint = unitCofig;
                    dataids.Add(dataID); 
                    break;
                }

                foreach (DataIDRange rangeData in unitCofig.RangeDataids)
                {
                    if (dataID >= rangeData.StartData && dataID <= rangeData.EndData)
                    {
                        targetUint = unitCofig;    
                        rangeDataids.Add(rangeData);
                        break;
                    }
                }
            }

            UnitConfigView resultValue = new UnitConfigView(targetUint.GroupID);
            resultValue.Coefficient = targetUint.Coefficient;
            resultValue.GroupName = targetUint.GroupName;
            resultValue.UnitName = targetUint.UnitName;            
            resultValue.RangeDataids = GetRangeDataNameList(rangeDataids);
            resultValue.SigleDataids = GetDataIDNameList(dataids);

            return resultValue;
        }

        /// <summary>
        /// 查找单参数单位信息
        /// </summary>
        /// <param name="dataID">参数ID</param>
        /// <returns></returns>
        public List<DataUnitView> FindDataUnitByID(uint dataID)
        {
            List<DataUnitView> resultList = new List<DataUnitView>();                      

            DataUnitDef resultDef;
            bool exist = this.otherDataidUnitMap.TryGetValue(dataID, out resultDef);
            if (!exist)
                return resultList;

            List<uint> dataids = new List<uint>();
            dataids.Add(dataID);
            Dictionary<uint, string> dataIDMap = DataParameterInfoDef.DataManager.GetDataIDNameMap(dataids);

            DataUnitView resultItem = new DataUnitView(dataID);
            resultItem.Coefficient = resultDef.Coefficient;
            resultItem.DataID = resultDef.DataID;
            dataIDMap.TryGetValue(resultDef.DataID, out resultItem.DataName);
            resultItem.UnitName = resultDef.UnitName;
            resultList.Add(resultItem);

            return resultList;
        }

        /// <summary>
        /// 查找参数单位信息
        /// </summary>
        /// <param name="dataID">参数ID</param>
        /// <returns></returns>
        public List<DataUnitInfo> FindDataUnitInfoByID(uint dataID)
        {
            List<DataUnitInfo> resultList = new List<DataUnitInfo>();

            //优先找特殊的参数单位信息
            List<DataUnitView> specialDataList = FindDataUnitByID(dataID);           
            foreach (DataUnitView specialData in specialDataList)
            {
                DataUnitInfo resultItem = new DataUnitInfo(0);
                resultItem.GroupName = LocalResourceManager.GetInstance().GetString("1070", "Others");//其他
                resultItem.Coefficient = specialData.Coefficient;
                resultItem.DataFlag = 1;//表示单参数
                resultItem.UnitName = specialData.UnitName;
                resultItem.SingleData.DataID = specialData.DataID;
                resultItem.SingleData.DataName = specialData.DataName;
                resultList.Add(resultItem);
            }

            if (resultList.Count > 0)
                return resultList;

            //再找单位组中的信息
            UnitConfigView dataInfo = FindDataUnitInGroup(dataID);
            foreach (DataIDNameDef singleData in dataInfo.SigleDataids)
            {
                DataUnitInfo resultItem = new DataUnitInfo(dataInfo.GroupID);
                resultItem.GroupName = dataInfo.GroupName;
                resultItem.DataFlag = 1;
                resultItem.Coefficient = dataInfo.Coefficient;
                resultItem.UnitName = dataInfo.UnitName;
                resultItem.SingleData = singleData;
                resultList.Add(resultItem);
            }
            foreach (RangeDataNameDef rangeData in dataInfo.RangeDataids)
            {
                DataUnitInfo resultItem = new DataUnitInfo(dataInfo.GroupID);
                resultItem.GroupName = dataInfo.GroupName;
                resultItem.DataFlag = 2;
                resultItem.Coefficient = dataInfo.Coefficient;
                resultItem.UnitName = dataInfo.UnitName;
                resultItem.RangeData = rangeData;
                resultList.Add(resultItem);
            }

            return resultList;
        }

        /// <summary>
        /// 查找dataID的单位信息
        /// </summary>
        /// <param name="dataID">参数ID</param>
        /// <returns></returns>
        public DataUnitDef FindDataUnit(uint dataID)
        {
            DataUnitDef resultValue = new DataUnitDef(dataID);

            //优先从单参数单位设置中找
            bool exist = this.otherDataidUnitMap.TryGetValue(dataID, out resultValue);
            if (exist)
                return resultValue;

            //再从组单位信息中找
            UnitConfigDef targetUint = new UnitConfigDef(0);
            foreach (UnitConfigDef unitCofig in this.groupidUnitMap.Values)
            {
                if (unitCofig.SigleDataids.Contains(dataID))
                {
                    targetUint = unitCofig;
                    break;
                }

                foreach (DataIDRange rangeData in unitCofig.RangeDataids)
                {
                    if (dataID >= rangeData.StartData && dataID <= rangeData.EndData)
                    {
                        targetUint = unitCofig;
                        break;
                    }
                }
            }

            resultValue = new DataUnitDef(dataID);
            resultValue.UnitName = targetUint.UnitName;
            resultValue.Coefficient = targetUint.Coefficient;

            return resultValue;
        }

        /// <summary>
        /// 添加参数到单位组中
        /// </summary>
        /// <param name="dataID">参数ID</param>
        /// <param name="groupID">单位组ID</param>
        /// <returns></returns>
        public EMSErrorMsg AddDataIDIntoGroup(uint startDataID, uint endDataID, int groupID)
        {
            EMSErrorMsg resultMsg = new EMSErrorMsg(true);

            //检查dataID取值范围
            if (startDataID <= 0 || endDataID<=0)
            {
                resultMsg.Success = false;
                resultMsg.ErrorCode = (int)EMSWebErrorCode.returnError;
                resultMsg.ErrorMessage = LocalResourceManager.GetInstance().GetString("1241", "Please enter a number greater than 0!");//请输入大于0的数！
                ErrorInfoManager.Instance.WriteLogMessage("AddDataIDIntoGroup", resultMsg.ErrorMessage);
                return resultMsg;
            }

            //检查是否已存在于其他组
            foreach (UnitConfigDef unitgroup in this.groupidUnitMap.Values)
            {
                resultMsg = CheckDataID(startDataID, endDataID, unitgroup);
                if (!resultMsg.Success)
                    return resultMsg;
            }

            UnitConfigDef unitdef;
            bool exists = groupidUnitMap.TryGetValue(groupID, out unitdef);
            if (!exists)
            {
                resultMsg.Success = false;
                resultMsg.ErrorCode = (int)EMSWebErrorCode.returnError;
                resultMsg.ErrorMessage = LocalResourceManager.GetInstance().GetString("1242","The unit group does not exist!");
                ErrorInfoManager.Instance.WriteLogMessage("AddDataIDIntoGroup", resultMsg.ErrorMessage);
                return resultMsg;
            }

            //保存配置文件
            bool success = true;
            string dataStr = string.Empty;
            if (startDataID == endDataID)            
                //单参数的情况
                dataStr = startDataID.ToString();              
            else            
                //参数范围的情况
                dataStr = string.Format("{0}..{1}",startDataID,endDataID);   
            string sectionFlag = string.Format("UNITGROUP{0}", groupID);
            string dataids = ConfigFileManager.DataManager.ReadString(sectionFlag, "IncludeDatas");
            if (string.IsNullOrWhiteSpace(dataids))
                dataids = dataStr;
            else
                dataids += "," + dataStr;
            success = this.WriteValue(sectionFlag, "IncludeDatas", dataids);

            if (!success)
            {
                resultMsg.Success = false;
                resultMsg.ErrorCode = (int)EMSWebErrorCode.returnError;
                resultMsg.ErrorMessage = LocalResourceManager.GetInstance().GetString("1243", "Failed to write Configuration File.");
                ErrorInfoManager.Instance.WriteLogMessage("AddDataIDIntoGroup", resultMsg.ErrorMessage);
                return resultMsg;
            }

            //更新缓存            
            if (startDataID == endDataID)
                unitdef.SigleDataids.Add(startDataID);
            else
                unitdef.RangeDataids.Add(new DataIDRange(startDataID,endDataID));
            groupidUnitMap[groupID] = unitdef;  

            return resultMsg;
        }

        /// <summary>
        /// 检查DataID是否已存在于该组中
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="unitdef"></param>
        /// <returns></returns>
        private EMSErrorMsg CheckDataID(uint startDataID, uint endDataID, UnitConfigDef unitdef)
        {
            EMSErrorMsg resultMsg = new EMSErrorMsg(true);

            bool hasAdded = false;
            uint dataid = startDataID;
            while (dataid <= endDataID)
            {
                hasAdded = unitdef.SigleDataids.Contains(dataid);
                dataid++;
                if (hasAdded)
                    break;
            }

            if (!hasAdded)
            {
                foreach (DataIDRange rangeData in unitdef.RangeDataids)
                {
                    if ((startDataID >= rangeData.StartData && startDataID <= rangeData.EndData)
                        || (endDataID >= rangeData.StartData && endDataID <= rangeData.EndData)
                        || (startDataID <= rangeData.StartData && endDataID >= rangeData.EndData))
                    {
                        hasAdded = true;
                        break;
                    }
                }
            }

            if (hasAdded)
            {
                resultMsg.Success = false;
                resultMsg.ErrorCode = (int)EMSWebErrorCode.returnError;
                resultMsg.ErrorMessage = string.Format(LocalResourceManager.GetInstance().GetString("1244", "DataID already exists in group \"{0}\"!"), unitdef.GroupName);
                ErrorInfoManager.Instance.WriteLogMessage("AddDataIDIntoGroup", resultMsg.ErrorMessage);                
            }
            return resultMsg;
        }

        /// <summary>
        /// 从单位组移除相关参数
        /// </summary>
        /// <param name="startDataID">起始参数</param>
        /// <param name="endDataID">结束参数，如果起始参数等于结束参数，那么表示移除单参数</param>
        /// <param name="groupID">单位组ID</param>
        /// <returns></returns>
        public EMSErrorMsg MoveDataIDFromGroup(uint startDataID, uint endDataID, int groupID)
        {
            EMSErrorMsg resultMsg = new EMSErrorMsg(true);

            UnitConfigDef unitdef;
            bool exists = groupidUnitMap.TryGetValue(groupID, out unitdef);
            if (!exists)
            {
                resultMsg.Success = false;
                resultMsg.ErrorCode = (int)EMSWebErrorCode.returnError;
                resultMsg.ErrorMessage = LocalResourceManager.GetInstance().GetString("1242", "The unit group does not exist!");
                ErrorInfoManager.Instance.WriteLogMessage("AddDataIDIntoGroup", resultMsg.ErrorMessage);
                return resultMsg;
            }

            string sectionFlag = string.Format("UNITGROUP{0}", groupID);
            string dataids = ConfigFileManager.DataManager.ReadString(sectionFlag, "IncludeDatas");
            List<string> dataItems = DataFormatManager.ParseStringList(dataids,",");
            if (startDataID == endDataID)
            {                
                unitdef.SigleDataids.Remove(startDataID);
                //更新缓存
                groupidUnitMap[groupID] = unitdef;
                dataItems.Remove(startDataID.ToString());
            }
            else
            {
                DataIDRange dataRange = new DataIDRange();
                dataRange.StartData = startDataID;
                dataRange.EndData = endDataID;
                unitdef.RangeDataids.Remove(dataRange);
                //更新缓存
                groupidUnitMap[groupID] = unitdef;
                dataItems.Remove(string.Format("{0}..{1}",startDataID,endDataID));
            }

            //重组字符串
            string newDataidsStr = string.Empty;
            for (int i = 0; i < dataItems.Count;i++ )
            {
                if (string.IsNullOrWhiteSpace(newDataidsStr))
                    newDataidsStr += dataItems[i];
                else
                    newDataidsStr += "," + dataItems[i];
            }
            //更新配置文件
            bool success = this.WriteValue(sectionFlag, "IncludeDatas", newDataidsStr);
            if (!success)
            {
                resultMsg.Success = false;
                resultMsg.ErrorCode = (int)EMSWebErrorCode.returnError;
                resultMsg.ErrorMessage = LocalResourceManager.GetInstance().GetString("1243", "Write Config File Error!");
                ErrorInfoManager.Instance.WriteLogMessage("MoveDataIDFromGroup", resultMsg.ErrorMessage);
                return resultMsg;
            }

            return resultMsg;
        }

        /// <summary>
        /// 添加参数单位信息（只添加到Others组中）
        /// </summary>
        /// <param name="dataID">参数ID</param>
        /// <param name="unitName">单位名称</param>
        /// <param name="coef">系数</param>
        /// <returns></returns>
        public EMSErrorMsg AddDataIDUnit(uint dataID, string unitName, double coef)
        {
            EMSErrorMsg resultMsg = new EMSErrorMsg(true);

            //检查dataID取值范围
            if(dataID<=0)
            {
                resultMsg.Success = false;
                resultMsg.ErrorCode = (int)EMSWebErrorCode.returnError;
                resultMsg.ErrorMessage = LocalResourceManager.GetInstance().GetString("1241", "Please enter a number greater than 0!");
                ErrorInfoManager.Instance.WriteLogMessage("AddDataIDUnit", resultMsg.ErrorMessage);
                return resultMsg;
            }

            //检查是否已存在该参数
            DataUnitDef resultDef;
            bool exist = this.otherDataidUnitMap.TryGetValue(dataID, out resultDef);
            if (exist)
            {
                resultMsg.Success = false;
                resultMsg.ErrorCode = (int)EMSWebErrorCode.returnError;
                string groupName = LocalResourceManager.GetInstance().GetString("1070", "Others");//其他
                resultMsg.ErrorMessage = string.Format(LocalResourceManager.GetInstance().GetString("1244", "DataID already exists in group \"{0}\"!"), groupName);
                ErrorInfoManager.Instance.WriteLogMessage("AddDataIDUnit", resultMsg.ErrorMessage);
                return resultMsg;
            }

            string sectionFlag = "SPECIAL";
            int dataCount = ConfigFileManager.DataManager.ReadInt32(sectionFlag, "TotalCount") + 1;
            bool success = this.WriteValue(sectionFlag, "TotalCount", dataCount.ToString());
            if (success)
            {
                string keyFlag = string.Format("DataID{0}",dataCount);
                string value = string.Format("{0},{1},{2}", dataID, unitName, coef);
                success = this.WriteValue(sectionFlag, keyFlag, value);
            }

            if (!success)
            {
                resultMsg.Success = false;
                resultMsg.ErrorCode = (int)EMSWebErrorCode.returnError;
                resultMsg.ErrorMessage = LocalResourceManager.GetInstance().GetString("1243","Write Config File Error!");
                ErrorInfoManager.Instance.WriteLogMessage("AddDataIDUnit", resultMsg.ErrorMessage);
                return resultMsg;
            }
            
            //更新缓存
            DataUnitDef unitdef = new DataUnitDef(dataID);
            unitdef.Index = dataCount;
            unitdef.UnitName = unitName;
            unitdef.Coefficient = coef;
            this.otherDataidUnitMap[dataID] = unitdef;    
            
            return resultMsg;
        }

        /// <summary>
        /// 增加单位组
        /// </summary>
        /// <param name="groupName">单位组名称</param>
        /// <param name="unitName">单位名称</param>
        /// <param name="coef"></param>
        /// <returns></returns>
        public EMSErrorMsg AddUnitGroup(string groupName, string unitName, double coef)
        {
            EMSErrorMsg resultMsg = new EMSErrorMsg(true);

            resultMsg = CheckUnitGroup(-1, groupName);
            if (!resultMsg.Success)
                return resultMsg;

            int groupid = this.groupidUnitMap.Count + 1;
            bool success = this.WriteValue("FORMAT", "UnitGroupCount", groupid.ToString());
            if (success)
            {
                string sectionFlag = string.Format("UNITGROUP{0}", groupid);
                if (success)
                    success = this.WriteValue(sectionFlag, "GroupName", groupName);
                if (success)
                    success = this.WriteValue(sectionFlag, "Uint", unitName);
                if (success)
                    success = this.WriteValue(sectionFlag, "Coefficient", coef.ToString());
                if (success)
                    success = this.WriteValue(sectionFlag, "IncludeDatas", string.Empty);
            }

            if (!success)
            {
                resultMsg.Success = false;
                resultMsg.ErrorCode = (int)EMSWebErrorCode.returnError;
                resultMsg.ErrorMessage = LocalResourceManager.GetInstance().GetString("1243","Write Config File Error!");
                ErrorInfoManager.Instance.WriteLogMessage("AddUnitGroup", resultMsg.ErrorMessage);
                return resultMsg;
            }

            //更新缓存           
            UnitConfigDef unitdef = new UnitConfigDef(groupid);
            unitdef.GroupName = groupName;
            unitdef.UnitName = unitName;
            unitdef.Coefficient = coef;
            this.groupidUnitMap[groupid] = unitdef;
            resultMsg.ErrorMessage = groupid.ToString();                            
            
            return resultMsg;
        }

        /// <summary>
        /// 检查是否有同名的单位组
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private EMSErrorMsg CheckUnitGroup(int groupID, string groupName)
        {
            EMSErrorMsg resultMsg = new EMSErrorMsg(true);            
            foreach (UnitConfigDef uintDef in this.groupidUnitMap.Values)
            {
                if (uintDef.GroupID!=groupID && uintDef.GroupName.Trim().ToUpper() == groupName.Trim().ToUpper())
                {
                    resultMsg.Success = false;
                    resultMsg.ErrorCode = (int)EMSWebErrorCode.returnError;
                    resultMsg.ErrorMsgInstance = "AddUnitGroup";
                    resultMsg.ErrorMessage = string.Format(LocalResourceManager.GetInstance().GetString("1245", "Unit Group \"{0}\" already exists. Please re-enter."), groupName);
                    return resultMsg;
                }
            }
            return resultMsg;
        }

        /// <summary>
        /// 删除单位组
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public EMSErrorMsg DeleteUnitGroup(int groupID)
        {
            EMSErrorMsg resultMsg = new EMSErrorMsg(true);

            string sectionFlag = string.Format("UNITGROUP{0}", groupID);
            bool success = this.WriteValue(sectionFlag, null, null);
            if (!success)
            {
                resultMsg.Success = false;
                resultMsg.ErrorCode = (int)EMSWebErrorCode.returnError;
                resultMsg.ErrorMessage = LocalResourceManager.GetInstance().GetString("1246", "Failed to delete Unit.");
                ErrorInfoManager.Instance.WriteLogMessage("DeleteUnitGroup", resultMsg.ErrorMessage);
                return resultMsg;
            }

            //更新缓存
            if (this.groupidUnitMap.ContainsKey(groupID))
                this.groupidUnitMap.Remove(groupID);
           
            return resultMsg;
        }

        /// <summary>
        /// 更新单位组相关信息
        /// </summary>
        /// <param name="groupID">单位组ID</param>
        /// <param name="groupName">单位组名称</param>
        /// <param name="unitName">单位</param>
        /// <param name="coef">系数</param>
        /// <returns></returns>
        public EMSErrorMsg UpdateUnitGroup(int groupID, string newGroupName, string newUnitName, double newCoef)
        {
            EMSErrorMsg resultMsg = new EMSErrorMsg(true);

            resultMsg = CheckUnitGroup(groupID, newGroupName);
            if (!resultMsg.Success)
                return resultMsg;

            string sectionFlag = string.Format("UNITGROUP{0}", groupID);
            bool success = this.WriteValue(sectionFlag, "GroupName", newGroupName);
            if(success)
                success= this.WriteValue(sectionFlag, "Uint", newUnitName);
            if (success)
                success = this.WriteValue(sectionFlag, "Coefficient", newCoef.ToString());

            if (!success)
            {
                resultMsg.Success = false;
                resultMsg.ErrorCode = (int)EMSWebErrorCode.returnError;
                resultMsg.ErrorMessage = LocalResourceManager.GetInstance().GetString("1243","Write Config File Error!");
                ErrorInfoManager.Instance.WriteLogMessage("UpdateUnitGroup", resultMsg.ErrorMessage);
                return resultMsg;
            }

            //更新缓存
            UnitConfigDef unitdef;
            bool exists = this.groupidUnitMap.TryGetValue(groupID, out unitdef);
            if (exists)
            {
                unitdef.GroupName = newGroupName;
                unitdef.UnitName = newUnitName;
                unitdef.Coefficient = newCoef;
                this.groupidUnitMap[groupID] = unitdef;  
            }

            return resultMsg;
        }

        /// <summary>
        /// 每次写入ini文件时需要先设置路径，否则可能路径是EntryGroup.ini，因为初始化的时候将ConfigFileManager.DataManager.IniPath设置成了EntryGroup.ini
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        private bool WriteValue(string Section, string Key, string Value)
        {
            ConfigFileManager.DataManager.IniPath = Path.Combine(SysConstDef.ProfileDirPath, UnitConfigFile);
            return ConfigFileManager.DataManager.WriteValue(Section, Key, Value);
        }

        /// <summary>
        /// 获取单位组信息列表
        /// </summary>
        /// <returns></returns>
        public List<GroupIDNameDef> GetGroupIDNameList()
        {
            List<GroupIDNameDef> resultList = new List<GroupIDNameDef>();

            foreach (int groupid in this.groupidUnitMap.Keys)
            {
                GroupIDNameDef resultItem = new GroupIDNameDef();
                resultItem.GroupID = groupid;
                resultItem.GroupName = groupidUnitMap[groupid].GroupName;
                resultList.Add(resultItem);
            }

            GroupIDNameDef otherItem = new GroupIDNameDef();
            otherItem.GroupID = 0;
            otherItem.GroupName = LocalResourceManager.GetInstance().GetString("1070", "Others");//其他
            resultList.Add(otherItem);

            return resultList;
        }

        /// <summary>
        /// 获取单位配置列表(除了ID为0的组)
        /// </summary>
        /// <param name="groupID">单位组</param>
        /// <returns></returns>
        public UnitConfigView GetUnitConfigView(int groupID)
        {
            UnitConfigView resultValue = new UnitConfigView(groupID);

            UnitConfigDef unitdef;
            bool exists = groupidUnitMap.TryGetValue(groupID, out unitdef);
            if (!exists)
            {
                ErrorInfoManager.Instance.WriteLogMessage("GetUnitConfigView", LocalResourceManager.GetInstance().GetString("1242","The unit group does not exist!"));
                return resultValue;
            }

            resultValue.GroupName = unitdef.GroupName;
            resultValue.UnitName = unitdef.UnitName;
            resultValue.Coefficient = unitdef.Coefficient;
            resultValue.SigleDataids = GetDataIDNameList(unitdef.SigleDataids);
            resultValue.RangeDataids = GetRangeDataNameList(unitdef.RangeDataids);           

            return resultValue;
        }

        /// <summary>
        /// 获取其他单位的列表(groupid=0)
        /// </summary>
        /// <returns></returns>
        public List<DataUnitView> GetOtherDataUintView()
        {
            List<DataUnitView> resultList = new List<DataUnitView>();

            List<uint> dataids = this.otherDataidUnitMap.Keys.ToList<uint>();
            Dictionary<uint, string> dataIDMap = DataParameterInfoDef.DataManager.GetDataIDNameMap(dataids);

            foreach (uint dataid in this.otherDataidUnitMap.Keys)
            {
                DataUnitView resultItem = new DataUnitView(dataid);
                DataUnitDef resultDef = otherDataidUnitMap[dataid];
                resultItem.Coefficient = resultDef.Coefficient;
                resultItem.DataID = resultDef.DataID;
                dataIDMap.TryGetValue(resultDef.DataID, out resultItem.DataName);
                resultItem.UnitName = resultDef.UnitName;
                resultList.Add(resultItem);
            }

            return resultList;
        }

        /// <summary>
        /// 删除参数单位信息
        /// </summary>
        /// <param name="dataID">参数ID</param>
        /// <returns></returns>
        public EMSErrorMsg DeleteDataUnitInfo(uint dataID)
        {
            EMSErrorMsg resultMsg = new EMSErrorMsg(true);

            int index = this.otherDataidUnitMap[dataID].Index;
            bool success = this.WriteValue("SPECIAL", string.Format("DataID{0}",index), null);
            if (!success)
            {
                resultMsg.Success = false;
                resultMsg.ErrorCode = (int)EMSWebErrorCode.returnError;
                resultMsg.ErrorMessage = LocalResourceManager.GetInstance().GetString("1243","Write Config File Error!");
                ErrorInfoManager.Instance.WriteLogMessage("DeleteDataUnitInfo", resultMsg.ErrorMessage);
                return resultMsg;
            }

            //删除缓存数据
            this.otherDataidUnitMap.Remove(dataID);   

            return resultMsg;
        }

        /// <summary>
        /// 修改参数单位信息
        /// </summary>
        /// <param name="dataID">参数ID</param>
        /// <param name="newUnitName">新单位</param>
        /// <param name="newCoef">新系数</param>
        /// <returns></returns>
        public EMSErrorMsg UpdateDataUnitInfo(uint dataID, string newUnitName, double newCoef)
        {
            EMSErrorMsg resultMsg = new EMSErrorMsg(true);

            DataUnitDef unitdef;
            bool exists = this.otherDataidUnitMap.TryGetValue(dataID, out unitdef);
            if (!exists)
            {
                resultMsg.Success = false;
                resultMsg.ErrorCode = (int)EMSWebErrorCode.returnError;
                resultMsg.ErrorMessage = LocalResourceManager.GetInstance().GetString("1247","The dataID does not exist!");
                ErrorInfoManager.Instance.WriteLogMessage("UpdateDataUnitInfo", resultMsg.ErrorMessage);
                return resultMsg;
            }

            string sectionFlag = "SPECIAL";
            string keyFlag = string.Format("DataID{0}", unitdef.Index);
            string value = string.Format("{0},{1},{2}", unitdef.DataID, newUnitName, newCoef);
            bool success = this.WriteValue(sectionFlag, keyFlag, value);
            if (!success)
            {
                resultMsg.Success = false;
                resultMsg.ErrorCode = (int)EMSWebErrorCode.returnError;
                resultMsg.ErrorMessage = LocalResourceManager.GetInstance().GetString("1243","Write Config File Error!");
                ErrorInfoManager.Instance.WriteLogMessage("UpdateDataUnitInfo", resultMsg.ErrorMessage);
                return resultMsg;
            }

            //更新缓存
            unitdef.UnitName = newUnitName;
            unitdef.Coefficient = newCoef;
            this.otherDataidUnitMap[dataID] = unitdef;

            return resultMsg;
        }

        private List<RangeDataNameDef> GetRangeDataNameList(List<DataIDRange> rangeDataids)
        {
            List<RangeDataNameDef> dataRangeList = new List<RangeDataNameDef>();
            List<uint> rangeDatas = new List<uint>();
            foreach (DataIDRange dataRange in rangeDataids)
            {
                rangeDatas.Add(dataRange.StartData);
                rangeDatas.Add(dataRange.EndData);
            }

            Dictionary<uint, string> dataRangeIDMap = DataParameterInfoDef.DataManager.GetDataIDNameMap(rangeDatas);
            foreach (DataIDRange dataRange in rangeDataids)
            {
                RangeDataNameDef dataRangeView = new RangeDataNameDef();
                dataRangeView.StartDataID = dataRange.StartData;
                dataRangeIDMap.TryGetValue(dataRange.StartData, out dataRangeView.StartDataName);
                dataRangeView.EndDataID = dataRange.EndData;
                dataRangeIDMap.TryGetValue(dataRange.EndData, out dataRangeView.EndDataName);
                dataRangeList.Add(dataRangeView);
            }
            return dataRangeList;
        }

        private List<DataIDNameDef> GetDataIDNameList(List<uint> dataIds)
        {
            Dictionary<uint, string> dataIDMap = DataParameterInfoDef.DataManager.GetDataIDNameMap(dataIds);
            List<DataIDNameDef> dataids = new List<DataIDNameDef>();
            foreach (uint dataid in dataIds)
            {
                DataIDNameDef dataItem = new DataIDNameDef();
                dataItem.DataID = dataid;
                dataIDMap.TryGetValue(dataid, out dataItem.DataName);
                dataids.Add(dataItem);
            }
            return dataids;
        }

        /// <summary>
        /// 获取DataID列表
        /// </summary>
        /// <param name="sectionFlag"></param>
        /// <param name="singleDataids"></param>
        /// <param name="rangeDataids"></param>
        private void GeDataIDList(string sectionFlag, out List<uint> singleDataids, out List<DataIDRange> rangeDataids)
        {
            string datasString = ConfigFileManager.DataManager.ReadString(sectionFlag, "IncludeDatas");
            singleDataids = new List<uint>();
            rangeDataids = new List<DataIDRange>();
            List<string> dataStrList = DataFormatManager.ParseStringList(datasString, ",");
            foreach (string dataStr in dataStrList)
            {
                List<string> dataids = DataFormatManager.ParseStringList(dataStr, ".");
                if (dataids.Count == 1)
                {
                    uint singleDataid = 0;
                    bool success = uint.TryParse(dataids[0], out singleDataid);
                    if (success)
                        singleDataids.Add(singleDataid);
                }

                if (dataids.Count == 3)
                {
                    DataIDRange newRange = new DataIDRange();
                    bool success = uint.TryParse(dataids[0], out newRange.StartData);
                    if (success)
                        success = uint.TryParse(dataids[2], out newRange.EndData);
                    if (success)
                        rangeDataids.Add(newRange);
                }
            }
        }
    }

    public struct DataIDRange
    {
        public uint StartData;
        public uint EndData;
        public DataIDRange(uint sdata, uint edata)
        {
            this.StartData = sdata;
            this.EndData = edata;
        }
    }

    public struct UnitConfigDef
    {
        public int GroupID;
        public string GroupName;
        public double Coefficient;
        public string UnitName;
        public List<uint> SigleDataids;
        public List<DataIDRange> RangeDataids;
        public UnitConfigDef(int groupid)
        {
            this.GroupID = groupid;    
            this.GroupName = string.Empty;
            this.Coefficient = 1;
            this.UnitName = string.Empty;
            this.SigleDataids = new List<uint>();
            this.RangeDataids = new List<DataIDRange>();
        }
    }

    public struct DataIDNameDef
    {
        public uint DataID;
        public string DataName;
    }

    public struct RangeDataNameDef
    {
        public uint StartDataID;
        public string StartDataName;
        public uint EndDataID;
        public string EndDataName;
    }

    
    public struct DataUnitInfo
    {
        /// <summary>
        /// 单位组ID
        /// </summary>
        public int GroupID;
        /// <summary>
        /// 单位组名称
        /// </summary>
        public string GroupName;
        /// <summary>
        /// 乘系数
        /// </summary>
        public double Coefficient;
        /// <summary>
        /// 单位
        /// </summary>
        public string UnitName;
        /// <summary>
        /// 参数标记，1-单参数，2-连续参数
        /// </summary>
        public int DataFlag;
        /// <summary>
        /// 单参数信息
        /// </summary>
        public DataIDNameDef SingleData;
        /// <summary>
        /// 连续参数信息
        /// </summary>
        public RangeDataNameDef RangeData;

        public DataUnitInfo(int groupID)
        {
            this.GroupID = groupID;
            this.GroupName = string.Empty;
            this.Coefficient = 1;
            this.DataFlag = 1;
            this.UnitName = string.Empty;
            this.SingleData = new DataIDNameDef();
            this.RangeData = new RangeDataNameDef();
        }
    }

    public struct UnitConfigView
    {
        public int GroupID;
        public string GroupName;
        public double Coefficient;
        public string UnitName;
        public List<DataIDNameDef> SigleDataids;
        public List<RangeDataNameDef> RangeDataids;
        public UnitConfigView(int groupid)
        {
            this.GroupID = groupid;    
            this.GroupName = string.Empty;
            this.Coefficient = 1;
            this.UnitName = string.Empty;
            this.SigleDataids = new List<DataIDNameDef>();
            this.RangeDataids = new List<RangeDataNameDef>();
        }
    }

    public struct DataUnitDef
    {
        public int Index;
        public uint DataID;
        public double Coefficient;
        public string UnitName;
        public DataUnitDef(uint dataid)
        {
            this.DataID = dataid;
            this.Coefficient = 1;
            this.UnitName = string.Empty;
            this.Index = 0;
        }
    }

    public struct DataUnitView
    {
        public uint DataID;
        public string DataName;
        public double Coefficient;
        public string UnitName;
        public DataUnitView(uint dataid)
        {
            this.DataID = dataid;
            this.DataName = string.Empty;
            this.Coefficient = 1;
            this.UnitName = string.Empty;
        }
    }

    public struct GroupIDNameDef
    {        
        public int GroupID;
        public string GroupName;
    }
}