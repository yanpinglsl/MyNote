namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
    class SourceIDAndParaIndex
    {
        private int m_sourceID = 0;
        private int m_paraIndex = 0;

        public SourceIDAndParaIndex(int sourceID,int paraIndex)
        {
            m_sourceID = sourceID;
            m_paraIndex = paraIndex;
        }

        public SourceIDAndParaIndex()
        {
            
        }

        public int SourceID
        {
            get { return m_sourceID; }
            set { m_sourceID = value; }
        }

        public int ParaIndex
        {
            get { return m_paraIndex; }
            set { m_paraIndex = value; }
        }
    }
}
