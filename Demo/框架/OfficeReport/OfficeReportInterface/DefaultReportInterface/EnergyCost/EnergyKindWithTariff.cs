using System;

namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
     public  struct EnergyKindWithTariff:IComparable
    {
        public uint DataId;
        public uint TariffNodeId;
        public StatisticType DataType;

        public EnergyKindWithTariff(uint energyKind, uint tariffNodeId, StatisticType dataType)
        {
            DataId = energyKind;
            TariffNodeId = tariffNodeId;
            DataType = dataType;
        }

         public int CompareTo(object obj)
         {
             if (obj == null)
                 return 1;
             var node = (EnergyKindWithTariff)obj;
             if (node.DataId != DataId)
                 return DataId.CompareTo(node.DataId);
             if (node.TariffNodeId != TariffNodeId)
                 return TariffNodeId.CompareTo(node.TariffNodeId);
             return DataType.CompareTo(node.DataType);
         }
    }
}
