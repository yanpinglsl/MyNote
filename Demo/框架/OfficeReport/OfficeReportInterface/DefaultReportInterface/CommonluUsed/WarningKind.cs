namespace OfficeReportInterface.DefaultReportInterface.CommonluUsed
{
    public enum WarningKind
    {
        /// <summary>
        /// 数据为空
        /// </summary>
        DataIsNull,
        /// <summary>
        /// SourceId不存在
        /// </summary>
        SourceIdNotExist,
        /// <summary>
        /// 数据超过10000行
        /// </summary>
        DataOver10000Rows,
        /// <summary>
        /// 从某个时刻开始有空数据
        /// </summary>
        DataNullStartFromSomeTime,
        /// <summary>
        /// 数据时间间隔发生了改变
        /// </summary>
        DataIntervalChanged,
        /// <summary>
        /// 用于计算kwh的dataId被替换了
        /// </summary>
        DataIdForKwhChanged,
        /// <summary>
        /// 某个费率的从某时刻开始的费率段的差值计算得到的是空值
        /// </summary>
        DataForTariffIndexEmpty,
        /// <summary>
        /// 映射方案不存在
        /// </summary>
        MapNotExist
    }
}
