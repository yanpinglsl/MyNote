using CSharpDBPlugin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace OfficeReportInterface
{
    class AllDataTypeNameManager
    {
        private static AllDataTypeNameManager _instance = null;

        private  List<DataTypeNameDef> dataTypeNameList = null;

        private AllDataTypeNameManager()
        {
            
        }

        public static AllDataTypeNameManager GetInstance()
        {
            if (_instance != null)
                return _instance;
            AllDataTypeNameManager temp = new AllDataTypeNameManager();
            Interlocked.CompareExchange(ref _instance, temp, null);
            return _instance;
        }

        public List<DataTypeNameDef> DataTypeNameList
        {
            get
            {
                if(dataTypeNameList!=null)
                   return dataTypeNameList;

                List<DataTypeNameDef> dataTypeNameListTemp = new List<DataTypeNameDef>();
                LoadDataTypeNameList(out dataTypeNameListTemp);
                Interlocked.CompareExchange(ref dataTypeNameList, dataTypeNameListTemp, null);
                return dataTypeNameList;
            }
        }

        private static void LoadDataTypeNameList(out  List<DataTypeNameDef> dataTypeNameList)
        {
            dataTypeNameList = new List<DataTypeNameDef>();
            DataTable dt = new DataTable();
            DataLogSolutionMapProvider.Instance.ReadDataTypeNameMaps(0, null, ref dt);
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                DataTypeNameDef typeName = new DataTypeNameDef();
                typeName.DataTypeID = Convert.ToInt32(dt.Rows[i]["DataTypeID"]);
                typeName.DataTypeName = Convert.ToString(dt.Rows[i]["DataTypeName"]);
                dataTypeNameList.Add(typeName);
            }
        }


    }
}
