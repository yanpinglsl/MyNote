using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YY.MicroService.Framework.PollyExtend;
using YY.MicroService.Model;

namespace YY.MicroService.Interface
{
    [Intercept(typeof(PollyPolicyAttribute))]//表示要polly生效
    public interface IUserService
    {
        User FindUser(int id);

        IEnumerable<User> UserAll();

        #region Polly
        [PollyPolicy]
        [PollyPolicyConfig(FallBackMethod = "UserServiceFallback",
            IsEnableCircuitBreaker = true,
            ExceptionsAllowedBeforeBreaking = 3,
            MillisecondsOfBreak = 1000 * 5,
            CacheTTLMilliseconds = 1000 * 20)]
        User AOPGetById(int id);

        Task<User> GetById(int id);
        #endregion

    }
}
