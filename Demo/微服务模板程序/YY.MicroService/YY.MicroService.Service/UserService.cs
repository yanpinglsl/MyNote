using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Timeout;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YY.MicroService.Framework.ConsulExtend;
using YY.MicroService.Framework.HttpApiExtend;
using YY.MicroService.Interface;
using YY.MicroService.Model;

namespace YY.MicroService.Service
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> logger;
        private readonly AbstractConsulDispatcher abstractConsulDispatcher;
        private readonly IHttpAPIInvoker httpAPIInvoker;
        private AsyncPolicyWrap<User>? _policyWrap;

        public UserService(ILogger<UserService> logger,
            AbstractConsulDispatcher abstractConsulDispatcher,
            IHttpAPIInvoker httpAPIInvoker)
        {
            this.logger = logger;
            this.abstractConsulDispatcher = abstractConsulDispatcher;
            this.httpAPIInvoker = httpAPIInvoker;
            // 初始化Polly策略
            InitPollyPolicy();
        }

        #region DataInit
        private List<User> _UserList = new List<User>()
        {
            new User()
            {
                Id=1,
                Account="Administrator",
                Email="57265177@qq.com",
                Name="Eleven",
                Password="1234567890",
                LoginTime=DateTime.Now,
                Role="Admin"
            },
             new User()
            {
                Id=2,
                Account="Apple",
                Email="57265177@qq.com",
                Name="Apple",
                Password="1234567890",
                LoginTime=DateTime.Now,
                Role="Admin"
            },
              new User()
            {
                Id=3,
                Account="Cole",
                Email="57265177@qq.com",
                Name="Cole",
                Password="1234567890",
                LoginTime=DateTime.Now,
                Role="Admin"
            },
        };
        #endregion

        #region 测试接口
        public User FindUser(int id)
        {
            return this._UserList.Find(u => u.Id == id);
        }

        public IEnumerable<User> UserAll()
        {
            return this._UserList;
        }
        #endregion

        #region Polly测试
        /// <summary>
        /// 初始化Polly策略
        /// </summary>
        private void InitPollyPolicy()
        {
            // 调用超时策略
            var timeout = Polly.Policy
                  // 超过一秒钟，就设定超时
                  .TimeoutAsync(1, TimeoutStrategy.Pessimistic, (context, ts, task) =>
                  {
                      logger.LogInformation("PollyService调用超时");
                      return Task.CompletedTask;
                  });
            // 调用熔断策略
            var circuitBreakerPolicy = Polly.Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                  exceptionsAllowedBeforeBreaking: 5,             // 连续5次异常
                  durationOfBreak: TimeSpan.FromMilliseconds(10),       // 断开10秒钟
                  onBreak: (exception, breakDelay) =>
                  {
                      //熔断以后，需要处理的动作；  记录日志；
                      logger.LogInformation($"PollyService服务出现=========>熔断");
                      logger.LogInformation($"熔断:{breakDelay.TotalMilliseconds } ms, 异常: " + exception.Message);
                  },
                  onReset: () => //// 熔断器关闭时
                  {
                      logger.LogInformation($"PollyService服务熔断器关闭了");
                  },
                  onHalfOpen: () => // 熔断时间结束时，从断开状态进入半开状态
                  {
                      logger.LogInformation($"PollyService服务熔断时间到，进入半开状态");
                  });

            _policyWrap = Policy<User>
                .Handle<Exception>()
                .FallbackAsync(UserServiceFallback(10), (x) =>
                {
                    logger.LogInformation($"AccountService进行了服务降级 -- {x.Exception.Message}");
                    return Task.CompletedTask;
                })
                .WrapAsync(circuitBreakerPolicy)
                .WrapAsync(timeout);
        }

        /// <summary>
        /// 降级方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User UserServiceFallback(int id)
        {
            Console.WriteLine("调用了->PollyServiceFallback方法进行降级处理");
            return new User()
            {
                Id = 9999,
                Account = "Administrator",
                Email = "57265177@qq.com",
                Name = "降级用户",
                Password = "1234567890",
                LoginTime = DateTime.Now,
                Role = "Admin"
            };
        }

        /// <summary>
        /// 【非AOP方式定义】根据用户ID查询用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<User> GetById(int id)
        {
            return await _policyWrap!.ExecuteAsync(() =>
            {
                string url = $"http://YYUserService/api/users/get?id={id}";
                string realUrl = this.abstractConsulDispatcher.MapAddress(url);
                string content = this.httpAPIInvoker.InvokeApi(realUrl);
                var user = JsonConvert.DeserializeObject<User>(content);
                return Task.FromResult(user);
            });
        }

        /// <summary>
        /// 【AOP方式定义】根据用户ID查询用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public User AOPGetById(int id)
        {
            string url = $"http://YYUserService/api/users/get?id={id}";
            string realUrl = this.abstractConsulDispatcher.MapAddress(url);
            string content = this.httpAPIInvoker.InvokeApi(realUrl);
            var Polly = JsonConvert.DeserializeObject<User>(content)!;
            return Polly;
        }
        #endregion
    }
}
