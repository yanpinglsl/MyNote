using System;
using System.Collections.Generic;
using System.Reflection;
using OfficeReportInterface.DefaultReportInterface;

namespace OfficeReportInterface.QueryCondition
{
    public class PreProcessExcelTempFile
    {
        public static bool GetDeviceDataIDByDeviceIdString(string deviceIds, out List<LogicalDeviceIndex> deviceNodeList, out List<DataIDNameTypeDef> dataIDList)
        {
            deviceNodeList = new List<LogicalDeviceIndex>();
            dataIDList = new List<DataIDNameTypeDef>();
            var nodeList = DefaultTemplatePublicMethod.ConstructParaList(deviceIds);
            foreach (var item in nodeList)
            {
                LogicalDeviceIndex deviceNode = new LogicalDeviceIndex(item.NodeType,item.DeviceID, item.LogicalDeviceIndex);
                if (!deviceNodeList.Contains(deviceNode))
                {
                    deviceNodeList.Add(deviceNode);
                }
                string dataName = string.Empty;
                if (!NamesManager.GetDataName(item.DataID, out dataName))
                    continue;
                DataIDNameTypeDef dataIdName = new DataIDNameTypeDef(item.DataID, dataName, item.DataTypeID);
                if (!dataIDList.Contains(dataIdName))
                {
                    dataIDList.Add(dataIdName);
                }
            }
            return true;
        }


        public static bool GetProcessExcelTempFile(string absolutePath, out ProcessExcelTempFile processExcelTempFile, out string tipMsg,uint userID,string userName)
        {
            processExcelTempFile = new ProcessExcelTempFile(absolutePath,userID,userName);
            string error = string.Empty;
            tipMsg = string.Empty;
            try
            {
                if (!processExcelTempFile.PublicExcelFileOperate.ExcelFileOpened(ref error))
                {
                    DbgTrace.dout(String.Format("{0}{1}{2}", MethodBase.GetCurrentMethod(), " Open Excel File Error :", error));
                    tipMsg = LocalResourceManager.GetInstance().GetString("0277", "报表生成失败.");
                    return false;
                }
                if (!processExcelTempFile.AnalyzeDataSourceLable() || !processExcelTempFile.AnalyzeRetrievalLable())
                {
                    string log = String.Format("{0}{1}", MethodBase.GetCurrentMethod(), " if (!processExcelTempFile.AnalyzeDataSourceLable() || !processExcelTempFile.AnalyzeRetrievalLable())");
                    DbgTrace.dout("{0}", log);

                    tipMsg = LocalResourceManager.GetInstance().GetString("0578", "模板文件标签解析出错.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                DbgTrace.dout(ex.Message+ex.StackTrace);
            }
            return true;
        }
    }
}
