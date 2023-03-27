using Consul;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YY.MicroService.Framework.ConsulExtend;
using YY.MicroService.Framework.HttpApiExtend;
using YY.MicroService.gRPCService;
using YY.MicroService.Interface;
using YY.MicroService.Model;

namespace YY.MicroService.Client.Controllers
{
    /// <summary>
    /// GRPC客户端创建步骤
    /// 常规操作
    /// 1、引入Nuget包：Grpc.Net.Client、Google.Protobuf、Grpc.Tools
    /// 2、追加Proto协议文件（可直接从服务端复制）
    /// 3、重新编译项目（清理->编译），会生成对应的*Grpc.cs类
    ///    也可以手动在工程文件中追加以下内容，再编译（注意GrpcServices必须是Client）
    ///     <Protobuf Include="Protos\mytest.proto" GrpcServices="Client" />
    /// 4、调用GRPC服务
    /// </summary>
    public class GrpcTestController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyTest.MyTestClient _lessonClient;
        private readonly AbstractConsulDispatcher _abstractConsulDispatcher;
        public GrpcTestController(ILogger<HomeController> logger
            , MyTest.MyTestClient lessonClient
            , AbstractConsulDispatcher abstractConsulDispatcher
            )
        {
            this._logger = logger;
            this._lessonClient = lessonClient;
            this._abstractConsulDispatcher = abstractConsulDispatcher;
        }
        /// <summary>
        /// 直接调用服务，写在调用的地方
        /// http://localhost:5177/Grpc/Index
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            //常规使用  https
            //{
            //    string targetUrl = "https://localhost:7017";
            //    using (var channel = GrpcChannel.ForAddress(targetUrl))
            //    {
            //        var client = new MyTest.MyTestClient(channel);
            //        var replySync = client.FindLesson(new MyTestRequest() { Id = 234 });
            //        //var replyAsync = await client.FindLessonAsync(new MyTestRequest() { Id = 123 });
            //        string result = JsonConvert.SerializeObject(replySync.Lesson);
            //        Console.WriteLine($"_lessonClient1 {Thread.CurrentThread.ManagedThreadId} 服务返回数据1:{result} ");
            //        base.ViewBag.Result = result;
            //    }
            //}

            ////使用  http---性能比https要高一些
            //{
            //    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            //    string targetUrl = "http://localhost:5017";
            //    using (var channel = GrpcChannel.ForAddress(targetUrl))
            //    {
            //        var client = new MyTest.MyTestClient(channel);
            //        var reply = await client.FindLessonAsync(new MyTestRequest() { Id = 123 });
            //        string result = Newtonsoft.Json.JsonConvert.SerializeObject(reply.Lesson);
            //        Console.WriteLine($"_lessonClient2 {Thread.CurrentThread.ManagedThreadId} 服务返回数据1:{result} ");
            //        base.ViewBag.Result = result;
            //    }
            //}

            {
                //program配置，此处IOC依赖直接注入
                var reply = await this._lessonClient.FindLessonAsync(new MyTestRequest() { Id = 123 });
                string result = Newtonsoft.Json.JsonConvert.SerializeObject(reply.Lesson);
                Console.WriteLine($"_lessonClient3 {Thread.CurrentThread.ManagedThreadId} 服务返回数据1:{result} ");
                base.ViewBag.Result = result;
            }

            return View();
        }

        public async Task<IActionResult> Init()
        {
            //使用  http+Consul---Consul只负责解析集群问题
            {
                string url = this._abstractConsulDispatcher.MapAddress("http://gRPCService/");
                Console.WriteLine($"This is from {url}");

                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                using (var channel = GrpcChannel.ForAddress(url))
                {
                    var client = new MyTest.MyTestClient(channel);
                    var reply = await client.FindLessonAsync(new MyTestRequest() { Id = 123 });
                    string result = Newtonsoft.Json.JsonConvert.SerializeObject(reply.Lesson);
                    Console.WriteLine($"_lessonClient4 {Thread.CurrentThread.ManagedThreadId} 服务返回数据1:{result} ");
                    base.ViewBag.Result = result;
                }
            }

            return View();
        }
    }
}
