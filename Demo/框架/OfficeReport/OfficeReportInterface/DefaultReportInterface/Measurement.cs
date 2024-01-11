namespace OfficeReportInterface.DefaultReportInterface
{
    struct Measurement
    {
        /// <summary>
        /// 参数ID
        /// </summary>
        public uint DataID;

        /// <summary>
        /// 定时记录类型ID
        /// </summary>
        public int ParaTypeID;

        /// <summary>
        /// 参数数据类型ID
        /// </summary>
        public int DataTypeID;

        /// <summary>
        /// 参数数据类型ID
        /// </summary>
        public int LogicalDeviceIndex;

        public Measurement(uint dataId, int paraTypeId, int dataTypeId, int logicalDeviceIndex)
        {
            DataID = dataId;
            ParaTypeID = paraTypeId;
            DataTypeID = dataTypeId;
            LogicalDeviceIndex = logicalDeviceIndex;
        }
    }

}
