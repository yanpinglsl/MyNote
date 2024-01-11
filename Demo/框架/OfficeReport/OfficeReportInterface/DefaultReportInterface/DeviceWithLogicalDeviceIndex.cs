namespace OfficeReportInterface.DefaultReportInterface
{
   
    struct DeviceWithLogicalDeviceIndex
    {
        /// <summary>
        /// 监测节点ID
        /// </summary>
        public uint DeviceID;
        /// <summary>
        /// 参数数据类型ID
        /// </summary>
        public int LogicalDeviceIndex;

        public DeviceWithLogicalDeviceIndex(uint deviceId,int logicalDeviceIndex)
        {
            DeviceID = deviceId;
            LogicalDeviceIndex = logicalDeviceIndex;
        }
    }
}
