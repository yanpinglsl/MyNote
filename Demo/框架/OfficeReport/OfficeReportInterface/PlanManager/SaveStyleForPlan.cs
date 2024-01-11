using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeReportInterface.PlanManager
{
     public enum SaveStyleForPlan
    {
         /// <summary>
         /// 按年保存
         /// </summary>
         ByYear=0,
         /// <summary>
         /// 按月保存
         /// </summary>
         ByMonth=1,
         /// <summary>
         /// 按计划保存
         /// </summary>
         ByPlan=2,
         /// <summary>
         /// 保存在固定目录下
         /// </summary>
         ByNone=3,
    }
}
