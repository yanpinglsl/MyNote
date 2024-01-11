namespace OfficeReportInterface
{
    public struct DataIDNameTypeDef
    {
        public uint DataID;
        public string DataName;
        public int DataTypeID;

        public DataIDNameTypeDef(uint dataID, string dataName, int dataTypeID)
        {
            this.DataID = dataID;
            this.DataName = dataName;
            this.DataTypeID = dataTypeID;
        }
    }
}
