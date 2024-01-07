namespace SwaggerDemo
{
    /// <summary>
    /// 该类型定义了 WebAPI 版本的信息。
    /// </summary>
    public static class ApiVersionInfo
    {
        /// <summary>
        /// 初始化默认值。
        /// </summary>
        static ApiVersionInfo()
        {
            V1 = string.Empty;
            V2 = string.Empty;
            V3 = string.Empty;
            V4 = string.Empty;
        }
        /// <summary>
        /// 获取或者设置 V1 版本。
        /// </summary>
        public static string V1;
        /// <summary>
        /// 获取或者设置 V2 版本。
        /// </summary>
        public static string V2;
        /// <summary>
        /// 获取或者设置 V3 版本。
        /// </summary>
        public static string V3;
        /// <summary>
        /// 获取或者设置 V4 版本。
        /// </summary>
        public static string V4;
    }
}
