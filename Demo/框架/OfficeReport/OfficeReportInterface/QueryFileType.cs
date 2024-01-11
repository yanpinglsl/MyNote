using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeReportInterface
{
    /// <summary>
    /// 模板类型
    /// </summary>
    public enum QueryFileType
    {
        /// <summary>
        /// 普通模板
        /// </summary>
        Normal=0,
        /// <summary>
        /// 预置模板
        /// </summary>
        DefaultTemplate=1,
        /// <summary>
        /// 查询条件模板
        /// </summary>
        SavedReport=2,
    }
}
