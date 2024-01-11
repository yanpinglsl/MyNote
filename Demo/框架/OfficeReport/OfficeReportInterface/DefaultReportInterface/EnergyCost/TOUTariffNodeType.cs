namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
    /// <summary>
    /// 分时计量专用系统节点类型
    /// </summary>
    public class TOUTariffNodeType
    {
        /// <summary>
        /// 分时方案设置
        /// </summary>
        public const uint TOUSETUP = 0x10900000;
        /// <summary>
        /// 分时计量方案设置
        /// </summary>
        public const uint ENERGY_SETUP = 0x10910000;
        /// <summary>
        /// 分时计量方案节点
        /// </summary>
        public const uint ENERGY_SCHEDULE = 0x10911000;
        /// <summary>
        /// 费率定义设置
        /// </summary>
        public const uint TARIFF_SETUP = 0x10911100;
        /// <summary>
        /// 费率定义
        /// </summary>
        public const uint TARIFF_NODE = 0x10911110;
        /// <summary>
        /// 分时计量日计量时段设置
        /// </summary>
        public const uint ENERGY_DAILYPROFILESETUP = 0x10911200;
        /// <summary>
        /// 分时计量日计量时段
        /// </summary>
        public const uint ENERGY_DAILYPROFILE = 0x10911210;
        /// <summary>
        /// 分时计量年历视图设置
        /// </summary>
        public const uint ENERGY_CALENDERSETUP = 0x10911300;
        /// <summary>
        /// 分时计量[YEAR]年历视图
        /// </summary>
        public const uint ENERGY_CALENDERVIEW = 0x10911310;
        /// <summary>
        /// 分时计量年策略
        /// </summary>
        public const uint ENERGY_TOUSTRATEGY = 0x10911400;
        /// <summary>
        /// 分时计量日期规则
        /// </summary>
        public const uint ENERGY_TOURULE = 0x10911410;

        /// <summary>
        /// 分时越限方案设置
        /// </summary>
        public const uint SETPOINT_SETUP = 0x10920000;
        /// <summary>
        /// 分时越限方案节点
        /// </summary>
        public const uint SETPOINT_SCHEDULE = 0x10921000;
        /// <summary>
        /// 限值组定义设置
        /// </summary>
        public const uint SETPOINT_GROUPSETUP = 0x10921100;
        /// <summary>
        /// 限值组定义
        /// </summary>
        public const uint SETPOINT_GROUP = 0x10921110;
        /// <summary>
        /// 分时越限日越限时段设置
        /// </summary>
        public const uint SETPOINT_DAILYPROFILESETUP = 0x10921200;
        /// <summary>
        /// 分时越限日计量时段
        /// </summary>
        public const uint SETPOINT_DAILYPROFILE = 0x10921210;
        /// <summary>
        /// 分时越限年历视图
        /// </summary>
        public const uint SETPOINT_CALENDERSETUP = 0x10921300;
        /// <summary>
        /// 分时越限[YEAR]年历视图
        /// </summary>
        public const uint SETPOINT_CALENDERVIEW = 0x10921310;
        /// <summary>
        /// 分时越限年策略
        /// </summary>
        public const uint SETPOINT_TOUSTRATEGY = 0x10921400;
        /// <summary>
        /// 分时越限日期规则
        /// </summary>
        public const uint SETPOINT_TOURULE = 0x10921410;
    }

}
