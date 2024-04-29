using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YY.DistributedTransaction.EFModel;

namespace YY.DistributedTransaction.Order.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private static string PublishName = "RabbitMQ.SQLServer.UserService";

        private readonly IConfiguration _iConfiguration;
        /// <summary>
        /// 构造函数注入---默认IOC容器完成---注册是在AddCAP
        /// </summary>
        private readonly ICapPublisher _iCapPublisher;
        private readonly CommonServiceDbContext _UserServiceDbContext;
        private readonly ILogger<OrderController> _Logger;

        public OrderController(ICapPublisher capPublisher, IConfiguration configuration, CommonServiceDbContext userServiceDbContext, ILogger<OrderController> logger)
        {
            this._iCapPublisher = capPublisher;
            this._iConfiguration = configuration;
            this._UserServiceDbContext = userServiceDbContext;
            this._Logger = logger;
        }

        [Route("/Distributed/Demo/{id}")]//根目录
        public IActionResult Distributed(int? id)
        {
            int index = id ?? 11;
            string publishName = "RabbitMQ.SQLServer.DistributedDemo.User-Order";

            var user = this._UserServiceDbContext.User.Find(1);
            var userNew = new User()
            {
                Name = "Eleven" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                CompanyId = 1,
                CompanyName = "朝夕教育" + index,
                CreateTime = DateTime.Now,
                CreatorId = 1,
                LastLoginTime = DateTime.Now,
                LastModifierId = 1,
                LastModifyTime = DateTime.Now,
                Password = "123456" + index,
                State = 1,
                Account = "Administrator" + index,
                Email = "57265177@qq.com",
                Mobile = "18664876677",
                UserType = 1
            };

            IDictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add("Teacher", "Eleven");
            dicHeader.Add("Student", "Seven");
            dicHeader.Add("Version", "1.2");
            dicHeader.Add("Index", index.ToString());

            using (var trans = this._UserServiceDbContext.Database.BeginTransaction(this._iCapPublisher, autoCommit: false))
            {
                this._UserServiceDbContext.User.Add(userNew);
                this._iCapPublisher.PublishAsync(publishName, user, dicHeader);//带header
                this._UserServiceDbContext.SaveChanges();
                Console.WriteLine("数据库业务数据已经插入");
                trans.Commit();
            }
            this._Logger.LogWarning($"This is EFCoreTransaction Invoke");
            return Ok("Done");
        }


    }
}
