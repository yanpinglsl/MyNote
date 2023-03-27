using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebCoreExtend.AuthorizationExtend.Requirement
{
    /// <summary>
    /// 两种邮箱都能支,二选一
    /// </summary>
    public class DoubleEmailRequirement : IAuthorizationRequirement
    {
    }

    public class QQMailHandler : AuthorizationHandler<DoubleEmailRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DoubleEmailRequirement requirement)
        {
            if (context.User != null && context.User.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                var emailCliamList = context.User.FindAll(c => c.Type == ClaimTypes.Email);//支持多Scheme
                if (emailCliamList.Any(c => c.Value.EndsWith("@qq.com", StringComparison.OrdinalIgnoreCase)))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    //context.Fail();//不设置失败 交给其他处理
                }
            }
            return Task.CompletedTask;
        }
    }

    public class ZhaoxiMailHandler : AuthorizationHandler<DoubleEmailRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DoubleEmailRequirement requirement)
        {
            if (context.User != null && context.User.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                var emailCliamList = context.User.FindAll(c => c.Type == ClaimTypes.Email);//支持多Scheme
                if (emailCliamList.Any(c => c.Value.EndsWith("@ZhaoxiEdu.Net", StringComparison.OrdinalIgnoreCase)))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    //context.Fail();//不设置失败 交给其他处理
                }
            }
            return Task.CompletedTask;
        }
    }
}
