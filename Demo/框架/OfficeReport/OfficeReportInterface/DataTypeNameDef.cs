using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeReportInterface
{
    public struct DataTypeNameDef
    {
        public int DataTypeID;
        public string DataTypeName;

        public DataTypeNameDef(int dataTypeID, string dataTypeName)
        {
            this.DataTypeID = dataTypeID;
            this.DataTypeName = dataTypeName;
        }
    }
}
