using Consul;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YY.MicroService.Framework.ConsulExtend;

namespace YY.MicroService.Framework.ConsulExtend
{
   public class ConsulRegister: IConsulRegister
    {
        private readonly ConsulRegisterOptions _consulRegisterOptions;
        private readonly ConsulClientOptions _consulClientOptions;
        public ConsulRegister(IOptionsMonitor<ConsulRegisterOptions> consulRegisterOptions, IOptionsMonitor<ConsulClientOptions> consulClientOptions)
        {
            this._consulRegisterOptions = consulRegisterOptions.CurrentValue;
            this._consulClientOptions = consulClientOptions.CurrentValue;
        }

        public async Task UseConsulRegist()
        {
            using (ConsulClient client = new ConsulClient(c =>
            {
                c.Address = new Uri($"http://{this._consulClientOptions.IP}:{this._consulClientOptions.Port}/");
                c.Datacenter = this._consulClientOptions.Datacenter;
            }))
            {
                await client.Agent.ServiceRegister(new AgentServiceRegistration()
                {
                    ID = $"{this._consulRegisterOptions.GroupName}-{this._consulRegisterOptions.IP}-{this._consulRegisterOptions.Port}",//唯一Id
                    Name = this._consulRegisterOptions.GroupName,//组名称-Group
                    Address = this._consulRegisterOptions.IP,
                    Port = this._consulRegisterOptions.Port,
                    Tags = new string[] { this._consulRegisterOptions.Tag ?? "Tags is null" },
                    Check = new AgentServiceCheck()
                    {
                        Interval = TimeSpan.FromSeconds(this._consulRegisterOptions.Interval),
                        HTTP = this._consulRegisterOptions.HealthCheckUrl,
                        Timeout = TimeSpan.FromSeconds(this._consulRegisterOptions.Timeout),
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(this._consulRegisterOptions.DeregisterCriticalServiceAfter),
                    }
                });
                Console.WriteLine($"{JsonConvert.SerializeObject(this._consulRegisterOptions)} 完成注册");
            }
        }

        public async Task UseConsulRegistgRPC()
        {
            using (ConsulClient client = new ConsulClient(c =>
            {
                c.Address = new Uri($"http://{this._consulClientOptions.IP}:{this._consulClientOptions.Port}/");
                c.Datacenter = this._consulClientOptions.Datacenter;
            }))
            {
                await client.Agent.ServiceRegister(new AgentServiceRegistration()
                {
                    ID = $"{this._consulRegisterOptions.IP}-{this._consulRegisterOptions.Port}-{this._consulRegisterOptions.GroupName}",//唯一Id
                    Name = this._consulRegisterOptions.GroupName,//组名称-Group
                    Address = this._consulRegisterOptions.IP,
                    Port = this._consulRegisterOptions.Port,
                    Tags = new string[] { this._consulRegisterOptions.Tag ?? "Tags is null" },
                    Check = new AgentServiceCheck()
                    {
                        Interval = TimeSpan.FromSeconds(this._consulRegisterOptions.Interval),
                        //HTTP = this._consulRegisterOption.HealthCheckUrl,
                        GRPC = this._consulRegisterOptions.HealthCheckUrl,//gRPC特有
                        GRPCUseTLS = false,//支持http
                        Timeout = TimeSpan.FromSeconds(this._consulRegisterOptions.Timeout),
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(this._consulRegisterOptions.DeregisterCriticalServiceAfter),

                    }
                });
                Console.WriteLine($"{JsonConvert.SerializeObject(this._consulRegisterOptions)} 完成注册");
            }
        }
    }
}
