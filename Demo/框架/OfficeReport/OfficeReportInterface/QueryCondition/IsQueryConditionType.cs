namespace OfficeReportInterface.QueryCondition
{
     public  class IsQueryConditionType
    {
         public static bool IsQueryConditionFile(string filePathItem, string queryConditionPath)
         {
             try
             {
                 if (filePathItem.IndexOf(queryConditionPath, 0) == 0)
                     return true;
                 return false;
             }
             catch (System.Exception ex)
             {
                 DbgTrace.dout(ex.Message + ex.StackTrace);
                 return false;
             }
         }
    }
}
