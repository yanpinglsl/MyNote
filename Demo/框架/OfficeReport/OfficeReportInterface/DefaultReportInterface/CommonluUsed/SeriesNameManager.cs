namespace OfficeReportInterface.DefaultReportInterface.CommonluUsed
{
    class SeriesNameManager
    {
        /// <summary>
        /// 获取图例名称
        /// </summary>
        /// <param name="deviceDataID"></param>
        /// <returns></returns>
        public static string GetSeriesName(DeviceDataIDDef deviceDataID,uint source)
        {
            string deviceName = DefaultTemplatePublicMethod.GetDeviceNameByDeviceDataID(deviceDataID,source);
            string dataName = DefaultTemplatePublicMethod.GetParanameByParament(deviceDataID,source);
            return string.Format("{0} ({1})", deviceName, dataName);
        }
    }
}
