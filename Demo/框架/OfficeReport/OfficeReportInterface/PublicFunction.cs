using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace OfficeReportInterface
{
    /// <summary>
    ///PublicFunction 的摘要说明
    /// </summary>
    public class PublicFunction
    {
        public PublicFunction()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 由于构造返回结果需要使用DataTable,每个接口都需要构造表结构，提取公共函数
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="dataType">参数类型</param>
        public static DataColumn CreateDataTableColumn(string columnName, string dataType)
        {
            DataColumn column;
            //设备ID
            column = new DataColumn();
            column.DataType = System.Type.GetType("System." + dataType);
            column.ColumnName = columnName;
            return column;
        }
    }
}