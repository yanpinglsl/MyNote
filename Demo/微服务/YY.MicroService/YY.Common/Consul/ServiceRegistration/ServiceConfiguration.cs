
namespace YY.Common.Consul.ServiceRegistration
{
    public class ServiceConfiguration
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; } = AppDomain.CurrentDomain.FriendlyName;

        /// <summary>
        /// 服务Id
        /// </summary>
        public string ServiceId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 服务地址
        /// </summary>
        public Uri ServiceAddress { get; set; } = null!;
    }


}