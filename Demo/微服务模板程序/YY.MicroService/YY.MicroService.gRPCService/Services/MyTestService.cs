using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using YY.MicroService.gRPCService;

namespace YY.MicroService.gRPCServer
{
    /// <summary>
    /// GRPC服务端创建步骤
    /// 1、追加Proto协议文件
    /// 2、重新编译项目（清理->编译），会生成对应的*Grpc.cs类
    ///     注意：此处如果未生成对应文件，可能是因为Proto文件有错误。
    ///     解决办法：可以手动在工程文件中追加以下内容，再编译
    ///     <Protobuf Include="Protos\mytest.proto" GrpcServices="Server" />
    /// 3、追加Services文件
    /// 4、在StartUp.cs文件中追加服务映射
    ///     endpoints.MapGrpcService<MyTestService>();
    /// </summary>
    public class MyTestService : MyTest.MyTestBase
    {
        private ILogger<MyTestService> _logger;
        public MyTestService(ILogger<MyTestService> logger)
        {
            this._logger = logger;
        }

        public override Task<MyTestReply> FindLesson(MyTestRequest request, ServerCallContext context)
        {
            Console.WriteLine($"This is {nameof(MyTestService)} FindLesson");
            return Task.FromResult(new MyTestReply()
            {
                Lesson = new MyTestReply.Types.TestModel()
                {
                    Id = request.Id,
                    Name = "架构师蜕变营",
                    Remark = "温暖的大家庭，靠谱的一家人"
                }
            });
        }
    }
}
