namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
     public  enum ParaTypes
    {
         /// <summary>
         /// 定时记录类型
         /// </summary>
        Datalog=1,
         /// <summary>
         /// 高速定时记录类型
         /// </summary>
        HighSpeedDatalog=5,

        //这10,11，是任意写的数字，可能根据实际情况修改
        /// <summary>
        /// 日报测点
        /// </summary>
        DayReport=10,
        /// <summary>
        /// 电能测点
        /// </summary>
        EnergyReport=11,
        /// <summary>
        /// 自定义曲线
        /// </summary>
        SelfDefine
    }
}
