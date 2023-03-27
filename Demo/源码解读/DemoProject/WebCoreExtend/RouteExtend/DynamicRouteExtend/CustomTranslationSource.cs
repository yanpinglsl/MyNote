
namespace WebCoreExtend.RouteExtend.DynamicRouteExtend
{
    /// <summary>
    /// 做映射---数据源
    /// </summary>
    public class CustomTranslationSource
    {
        /// <summary>
        /// 映射规则，可以是任意数据源和任意配置
        /// </summary>
        private static Dictionary<string, Dictionary<string, string>> MappingRuleDictionary
            = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "en", new Dictionary<string, string>
                {
                    { "route", "Route" },
                    { "info", "info" }
                }
            },
            {
                "ch", new Dictionary<string, string>
                {
                    { "route1", "Route" },
                    { "info1", "info" }
                }
            },
            {
                "hk", new Dictionary<string, string>
                {
                    { "route2", "Route" },
                    { "info2", "info" }
                }
            },
        };
        /// <summary>
        /// 根据区域，将控制器和Action，做个映射
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<string> Mapping(string lang, string value)
        {
            await Task.CompletedTask;
            var area = lang.ToLowerInvariant();
            var mapValue = value.ToLowerInvariant();
            if (MappingRuleDictionary.ContainsKey(area) && MappingRuleDictionary[area].ContainsKey(mapValue))
            {
                return MappingRuleDictionary[area][mapValue];
            }
            else
            {
                return null;
            }
        }
    }
}
