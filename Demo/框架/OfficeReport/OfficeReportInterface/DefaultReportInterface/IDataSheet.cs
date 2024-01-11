using System.Collections.Generic;
using System.Data;

namespace OfficeReportInterface.DefaultReportInterface
{
    interface IDataSheet
    {
        /// <summary>
        /// 获取填充Excel的数据DataTable的List
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        List<DataTable> GetDataLogDatas(DefaultReportParameter parameter);
    }
}
