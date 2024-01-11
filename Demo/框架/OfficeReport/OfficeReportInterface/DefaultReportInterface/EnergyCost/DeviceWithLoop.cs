namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
    public   struct DeviceWithLoop
    {
        public uint DeviceId;
        public int LogicalDeviceIndex;
    

        public DeviceWithLoop(uint deviceId, int logicalDeviceIndex)
        {
            DeviceId = deviceId;
            LogicalDeviceIndex = logicalDeviceIndex;
           
        }
    }
}
