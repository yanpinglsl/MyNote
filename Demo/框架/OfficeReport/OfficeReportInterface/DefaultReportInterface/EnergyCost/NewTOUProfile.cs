using System.Collections.Generic;

namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
     public class NewTOUProfile
     {
         private uint m_scheduleID = 0;
        //费率方案ID
         public uint scheduleID
         {
             get { return m_scheduleID; }
             set
             {
                 m_scheduleID = value;
             }
         }
        //判断是新的结构还是老的
        public string name;
        public List<EnergyCalenderView> yearProfileList;
        public List<DayProfileStruct> dayProfileList;
        public List<TOUTariffNode> tariffProfileList;

        public NewTOUProfile()
        {
            scheduleID = 0;
            name = "";
            yearProfileList = new List<EnergyCalenderView>();
            dayProfileList = new List<DayProfileStruct>();
            tariffProfileList = new List<TOUTariffNode>();
        }
      
    }
}
