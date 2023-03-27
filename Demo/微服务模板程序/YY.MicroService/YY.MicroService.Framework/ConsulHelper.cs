using Consul;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace YY.MicroService.Framework
{
    public static class ConsulHelper
    {
        /// <summary>
        /// Consul注册
        /// </summary>
        /// <param name="configuration"></param>
        public static void ConsulRegist(this IConfiguration configuration)
        {
            ConsulClient client = new ConsulClient(c =>
            {
                c.Address = new Uri("http://localhost:8500/");
                c.Datacenter = "dc1";
            });//找到consul

            string ip = string.IsNullOrWhiteSpace(configuration["ip"]) ? "192.168.3.230" : configuration["ip"];
            int port = int.Parse(configuration["port"]);//命令行参数必须传入
            int weight = string.IsNullOrWhiteSpace(configuration["weight"]) ? 1 : int.Parse(configuration["weight"]);

            client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = "service" + Guid.NewGuid(),//独一无二，
                Name = "ZhaoxiUserService",//Group--分组--因为有多个实例组成的集群
                Address = ip,//
                Port = port,//
                Tags = new string[] { weight.ToString() },//标签
                Check = new AgentServiceCheck()
                {
                    Interval = TimeSpan.FromSeconds(12),//间隔12s一次
                    HTTP = $"http://{ip}:{port}/Api/Health/Index",//控制器
                    Timeout = TimeSpan.FromSeconds(5),//检测等待时间
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(120)//失败后多久移除
                }
            });
            //命令行参数获取
            Console.WriteLine($"注册成功：{ip}:{port}--weight:{weight}");
        }
    }
}
