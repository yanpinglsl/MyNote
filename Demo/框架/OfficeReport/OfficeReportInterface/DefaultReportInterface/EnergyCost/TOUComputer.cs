namespace OfficeReportInterface.DefaultReportInterface.EnergyCost
{
    class TOUComputer
    {

        public static bool IsDATAIDKWH(uint dataId)
        {
            if (dataId == SysConstDefinition.DATAIDKWH || dataId == SysConstDefinition.DATAIDKWH_IMPORT || dataId == SysConstDefinition.DATAIDKWH_INCREASEIMPORT)
                return true;
            return false;
        }

        public static bool IsDATAIDKVARH(uint dataId)
        {
            if (dataId == SysConstDefinition.DATAIDKVARH || dataId == SysConstDefinition.DATAIDKVARH_IMPORT || dataId == SysConstDefinition.DATAIDKVARH_INCREASEIMPORT)
                return true;
            return false;
        }

        public static bool IsDATAIDKVAH(uint dataId)
        {
            if (dataId == SysConstDefinition.DATAIDKVAH || dataId == SysConstDefinition.DATAIDKVAH_IMPORT || dataId == SysConstDefinition.DATAIDKVAH_INCREASEIMPORT)
                return true;
            return false;
        }

        /// <summary>
        /// 判断是不是kwh，kvarh，kVAH中的一个
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        public static bool IsEnergy(uint dataId)
        {

            if (IsDATAIDKWH(dataId) || IsDATAIDKVARH(dataId) || IsDATAIDKVAH(dataId))
                return true;
            return false;
        }
        /// <summary>
        /// 判断是不是kw，kvar，kVA中的一个
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        public static bool IsDemand(uint dataId)
        {
            if (dataId == SysConstDefinition.DATAIDKW_DEMAND)
                return true;
            if (dataId == SysConstDefinition.DATAIDKVAR_DEMAND)
                return true;
            if (dataId == SysConstDefinition.DATAIDKVA_DEMAND)
                return true;
            return false;
        }
    }
}
