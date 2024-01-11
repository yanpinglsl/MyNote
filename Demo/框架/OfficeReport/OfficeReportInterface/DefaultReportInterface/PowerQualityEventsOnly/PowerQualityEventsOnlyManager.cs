using System.Collections.Generic;
using System.Data;

namespace OfficeReportInterface.DefaultReportInterface.PowerQualityEventsOnly
{
    public class PowerQualityEventsOnlyManager : IDataSheet
    {
        private  PowerQualityDataManager _PowerQualityDataManager;

        public PowerQualityEventsOnlyManager()
        {
            _PowerQualityDataManager=new PowerQualityDataManager();
            _PowerQualityDataManager.SetSource((uint)RepServFileType.PowerQualityEventsOnly);
            _PowerQualityDataManager.SetMaxNumberOfEvents(10000);
        }


        public List<DataTable> GetDataLogDatas(DefaultReportParameter parameter)
        {
            return _PowerQualityDataManager.GetDataLogDatas(parameter);
        }
    }
}
